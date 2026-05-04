using DemoPick.Helpers;
using DemoPick.Data;
using System.Data.SqlClient;

namespace DemoPick.Helpers
{
    internal static class AuthLoginAttemptTracker
    {
        // Luu hoac ghi nhan Record Failed Login Attempt vao trang thai he thong/CSDL khi nghiep vu yeu cau.
        internal static void RecordFailedLoginAttempt(int accountId, int maxFailedLoginAttempts, int lockoutMinutes)
        {
            // Best-effort lockout. If the DB schema doesn't have these columns yet, ignore errors.
            DatabaseHelper.ExecuteNonQuery(
                SqlQueries.Auth.RecordFailedLoginAttempt,
                new SqlParameter("@Id", accountId),
                new SqlParameter("@Max", maxFailedLoginAttempts),
                new SqlParameter("@Minutes", lockoutMinutes));
        }

        // Xoa, huy hoac dat lai du lieu Reset Failed Login theo dung dieu kien nghiep vu.
        internal static void ResetFailedLogin(int accountId)
        {
            DatabaseHelper.ExecuteNonQuery(
                SqlQueries.Auth.ResetFailedLogin,
                new SqlParameter("@Id", accountId));
        }
    }
}


