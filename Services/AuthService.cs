using DemoPick.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace DemoPick.Services
{
    internal static class AuthService
    {
        private const int MaxFailedLoginAttempts = 5;
        private const int LockoutMinutes = 5;

        internal static bool TryLogin(string identifier, string password, out AuthUser user, out string error)
        {
            user = null;
            error = null;

            if (string.IsNullOrWhiteSpace(identifier) || string.IsNullOrWhiteSpace(password))
            {
                error = "Vui lòng nhập tài khoản và mật khẩu.";
                return false;
            }

            string id = identifier.Trim();

            DataTable dt;
            try
            {
                dt = DatabaseHelper.ExecuteQuery(
                    SqlQueries.Auth.LoginCandidatesByIdentifier,
                    new SqlParameter("@Id", id));
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLog("Auth Login Query Error", ex, "AuthService.TryLogin");
                error = "Không thể kết nối CSDL. Hãy kiểm tra SQL Server/connection string.";
                return false;
            }

            if (dt.Rows.Count == 0)
            {
                error = "Sai tài khoản hoặc mật khẩu.";
                return false;
            }

            // If identifier maps to multiple accounts (e.g., duplicate FullName), do not pick arbitrarily.
            // We only allow login if the password matches exactly one ACTIVE account.
            AuthUser matched = null;
            int matchCount = 0;

            DateTime now = DateTime.Now;
            bool anyActiveButLocked = false;

            if (dt.Rows.Count == 1)
            {
                var only = dt.Rows[0];
                bool onlyActive = only["IsActive"] != DBNull.Value && Convert.ToBoolean(only["IsActive"]);
                if (!onlyActive)
                {
                    error = "Tài khoản đã bị khóa.";
                    return false;
                }

                if (only.Table.Columns.Contains("LockoutUntil") && only["LockoutUntil"] != DBNull.Value)
                {
                    DateTime until = Convert.ToDateTime(only["LockoutUntil"]);
                    if (until > now)
                    {
                        error = $"Tài khoản đang bị khóa tạm thời. Vui lòng thử lại sau ({until:HH:mm}).";
                        return false;
                    }
                }
            }

            foreach (DataRow row in dt.Rows)
            {
                bool isActive = row["IsActive"] != DBNull.Value && Convert.ToBoolean(row["IsActive"]);
                if (!isActive)
                    continue;

                if (row.Table.Columns.Contains("LockoutUntil") && row["LockoutUntil"] != DBNull.Value)
                {
                    DateTime until = Convert.ToDateTime(row["LockoutUntil"]);
                    if (until > now)
                    {
                        anyActiveButLocked = true;
                        continue;
                    }
                }

                byte[] storedHash = row["PasswordHash"] as byte[];
                byte[] storedSalt = row["PasswordSalt"] as byte[];
                if (storedHash == null || storedSalt == null)
                    continue;

                byte[] computed = HashPassword(password, storedSalt);
                if (!FixedTimeEquals(storedHash, computed))
                    continue;

                matchCount++;
                if (matchCount > 1)
                {
                    error = "Thông tin đăng nhập không duy nhất. Vui lòng dùng Email hoặc SĐT để đăng nhập.";
                    return false;
                }

                matched = new AuthUser
                {
                    AccountId = Convert.ToInt32(row["AccountID"]),
                    Username = row["Username"].ToString(),
                    FullName = row["FullName"] == DBNull.Value ? "" : row["FullName"].ToString(),
                    Role = row["Role"] == DBNull.Value ? "" : row["Role"].ToString(),
                };
            }

            if (matched == null)
            {
                if (dt.Rows.Count == 1)
                {
                    try
                    {
                        int accountId = Convert.ToInt32(dt.Rows[0]["AccountID"]);
                        RecordFailedLoginAttempt(accountId);
                    }
                    catch (Exception ex)
                    {
                        DatabaseHelper.TryLogThrottled(
                            throttleKey: "Auth.RecordFailedLoginAttempt",
                            eventDesc: "Auth FailedLogin Attempt Error",
                            ex: ex,
                            context: "AuthService.TryLogin",
                            minSeconds: 300);
                    }
                }

                if (anyActiveButLocked)
                {
                    error = "Tài khoản đang bị khóa tạm thời. Vui lòng thử lại sau.";
                    return false;
                }
                error = "Sai tài khoản hoặc mật khẩu.";
                return false;
            }

            try
            {
                ResetFailedLogin(matched.AccountId);
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLogThrottled(
                    throttleKey: "Auth.ResetFailedLogin",
                    eventDesc: "Auth ResetFailedLogin Error",
                    ex: ex,
                    context: "AuthService.TryLogin",
                    minSeconds: 300);
            }

            user = matched;
            return true;
        }

        private static void RecordFailedLoginAttempt(int accountId)
        {
            // Best-effort lockout. If the DB schema doesn't have these columns yet, ignore errors.
            DatabaseHelper.ExecuteNonQuery(
                SqlQueries.Auth.RecordFailedLoginAttempt,
                new SqlParameter("@Id", accountId),
                new SqlParameter("@Max", MaxFailedLoginAttempts),
                new SqlParameter("@Minutes", LockoutMinutes));
        }

        private static void ResetFailedLogin(int accountId)
        {
            DatabaseHelper.ExecuteNonQuery(
                SqlQueries.Auth.ResetFailedLogin,
                new SqlParameter("@Id", accountId));
        }

        internal static bool TryRegister(string fullName, string email, string phone, string password, string confirmPassword, out string error)
        {
            error = null;

            string username = (email ?? "").Trim();
            if (string.IsNullOrWhiteSpace(username))
            {
                error = "Vui lòng nhập Email/Tên đăng nhập.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                error = "Vui lòng nhập mật khẩu.";
                return false;
            }

            if (!string.Equals(password, confirmPassword ?? "", StringComparison.Ordinal))
            {
                error = "Mật khẩu xác nhận không khớp.";
                return false;
            }

            if (password.Length < 6)
            {
                error = "Mật khẩu phải có ít nhất 6 ký tự.";
                return false;
            }

            try
            {
                var exists = DatabaseHelper.ExecuteScalar(
                    SqlQueries.Auth.UsernameOrEmailExists,
                    new SqlParameter("@U", username),
                    new SqlParameter("@E", (object)(email ?? "") ?? DBNull.Value)
                );
                if (Convert.ToInt32(exists) > 0)
                {
                    error = "Tài khoản đã tồn tại.";
                    return false;
                }

                byte[] salt = GenerateSalt(16);
                byte[] hash = HashPassword(password, salt);

                DatabaseHelper.ExecuteNonQuery(
                    SqlQueries.Auth.RegisterStaffAccount,
                    new SqlParameter("@Username", username),
                    new SqlParameter("@Email", (object)email ?? DBNull.Value),
                    new SqlParameter("@Phone", (object)phone ?? DBNull.Value),
                    new SqlParameter("@FullName", (object)fullName ?? DBNull.Value),
                    new SqlParameter("@Hash", SqlDbType.VarBinary, 32) { Value = hash },
                    new SqlParameter("@Salt", SqlDbType.VarBinary, 16) { Value = salt },
                    new SqlParameter("@Role", AppConstants.Roles.Staff)
                );

                return true;
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLog("Auth Register Error", ex, "AuthService.TryRegister");
                error = "Không thể đăng ký (lỗi CSDL).";
                return false;
            }
        }

        internal static bool TrySeedAdminIfEmpty(out string seededUsername, out string seededPassword)
        {
            seededUsername = null;
            seededPassword = null;

            object cntObj = DatabaseHelper.ExecuteScalar(SqlQueries.Auth.StaffAccountsCount);
            int cnt = Convert.ToInt32(cntObj);
            if (cnt > 0) return false;

            string username = "admin";
            string password = Environment.GetEnvironmentVariable("DEMOPICK_BOOTSTRAP_ADMIN_PASSWORD");
            if (string.IsNullOrWhiteSpace(password))
            {
                password = GenerateRandomPassword(14);
            }

            byte[] salt = GenerateSalt(16);
            byte[] hash = HashPassword(password, salt);

            DatabaseHelper.ExecuteNonQuery(
                SqlQueries.Auth.SeedAdmin,
                new SqlParameter("@Username", username),
                new SqlParameter("@Hash", SqlDbType.VarBinary, 32) { Value = hash },
                new SqlParameter("@Salt", SqlDbType.VarBinary, 16) { Value = salt }
            );

            seededUsername = username;
            seededPassword = password;
            return true;
        }

        private static string GenerateRandomPassword(int length)
        {
            const string alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789!@#$%";
            if (length < 8) length = 8;

            byte[] bytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            var sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                sb.Append(alphabet[bytes[i] % alphabet.Length]);
            }
            return sb.ToString();
        }

        internal static bool TryChangePassword(int accountId, string oldPassword, string newPassword, string confirmNewPassword, out string error)
        {
            error = null;

            if (accountId <= 0)
            {
                error = "Phiên đăng nhập không hợp lệ. Vui lòng đăng nhập lại.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmNewPassword))
            {
                error = "Vui lòng nhập đầy đủ thông tin.";
                return false;
            }

            if (!string.Equals(newPassword, confirmNewPassword, StringComparison.Ordinal))
            {
                error = "Mật khẩu xác nhận không khớp.";
                return false;
            }

            if (newPassword.Length < 6)
            {
                error = "Mật khẩu mới phải có ít nhất 6 ký tự.";
                return false;
            }

            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery(
                    SqlQueries.Auth.ChangePasswordLoadHashSalt,
                    new SqlParameter("@Id", accountId));

                if (dt.Rows.Count == 0)
                {
                    error = "Không tìm thấy tài khoản hoặc tài khoản đã bị khóa.";
                    return false;
                }

                var row = dt.Rows[0];
                byte[] storedHash = row["PasswordHash"] as byte[];
                byte[] storedSalt = row["PasswordSalt"] as byte[];
                if (storedHash == null || storedSalt == null)
                {
                    error = "Tài khoản không hợp lệ (thiếu dữ liệu mật khẩu).";
                    return false;
                }

                byte[] computedOld = HashPassword(oldPassword, storedSalt);
                if (!FixedTimeEquals(storedHash, computedOld))
                {
                    error = "Mật khẩu cũ không đúng.";
                    return false;
                }

                byte[] newSalt = GenerateSalt(16);
                byte[] newHash = HashPassword(newPassword, newSalt);

                DatabaseHelper.ExecuteNonQuery(
                    SqlQueries.Auth.ChangePasswordUpdateHashSalt,
                    new SqlParameter("@Hash", SqlDbType.VarBinary, 32) { Value = newHash },
                    new SqlParameter("@Salt", SqlDbType.VarBinary, 16) { Value = newSalt },
                    new SqlParameter("@Id", accountId));

                return true;
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLog("Auth ChangePassword Error", ex, "AuthService.TryChangePassword");
                error = "Không thể đổi mật khẩu (lỗi CSDL).";
                return false;
            }
        }

        private static byte[] GenerateSalt(int size)
        {
            var salt = new byte[size];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        private static byte[] HashPassword(string password, byte[] salt)
        {
            // PBKDF2
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000))
            {
                return pbkdf2.GetBytes(32);
            }
        }

        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }
    }
}
