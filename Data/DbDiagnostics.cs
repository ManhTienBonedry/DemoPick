using DemoPick.Helpers;
using DemoPick.Data;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DemoPick.Data
{
    internal static class DbDiagnostics
    {
        // Tao thong bao loi khoi tao CSDL than thien de hien thi cho nguoi dung.
        internal static string BuildDbInitErrorMessage(Exception ex)
        {
#if !DEBUG
            return BuildSafeDbInitErrorMessage(ex);
#else
            var b = Db.CreateBuilder();

            string server = Safe(b.DataSource);
            string database = Safe(b.InitialCatalog);
            string auth = b.IntegratedSecurity ? "Windows (Integrated Security)" : "SQL Auth";

            var sb = new StringBuilder();
            sb.AppendLine("Không thể khởi tạo/kết nối cơ sở dữ liệu (schema/migrations).\n");

            sb.AppendLine("Đang dùng cấu hình:");
            sb.AppendLine($"- Server: {server}");
            sb.AppendLine($"- Database: {database}");
            sb.AppendLine($"- Auth: {auth}");
            sb.AppendLine();

            if (TryGetMigrationChecksumMismatch(ex, out string migrationId))
            {
                sb.AppendLine("Phát hiện lỗi migrations (checksum mismatch):");
                sb.AppendLine($"- Migration bị thay đổi sau khi đã apply: {migrationId}");
                sb.AppendLine();
                sb.AppendLine("Cách xử lý khuyến nghị:");
                sb.AppendLine("- KHÔNG sửa file migration đã apply. Hãy tạo file migration mới (ví dụ 0002__...sql) để thay đổi tiếp.");
                sb.AppendLine("- Nếu bạn lỡ sửa: hãy revert file migration về đúng nội dung phiên bản đã chạy.");
                sb.AppendLine("- DEV-only: nếu muốn reset toàn bộ DB cho sạch, hãy drop database (hoặc đổi tên Database trong DefaultConnection) rồi chạy lại app để tự tạo schema + migrations.");
                sb.AppendLine();
                sb.AppendLine("Ghi chú:");
                sb.AppendLine("- App lưu checksum trong bảng dbo.__Migrations để chống 'drift'.");
                sb.AppendLine();
            }

            if (TryGetSqlException(ex, out var sqlEx))
            {
                sb.AppendLine("Nguyên nhân thường gặp (gợi ý theo lỗi hiện tại):");
                foreach (string tip in BuildTips(sqlEx, b).Distinct())
                {
                    sb.AppendLine("- " + tip);
                }
                sb.AppendLine();
            }
            else
            {
                sb.AppendLine("Gợi ý kiểm tra nhanh:");
                sb.AppendLine("- Đảm bảo SQL Server instance đang chạy (ví dụ: SQL Server (SQLEXPRESS)).");
                sb.AppendLine("- Mở SSMS và thử connect tới server ở trên.");
                sb.AppendLine("- Kiểm tra lại connection string DefaultConnection trong App.config.");
                sb.AppendLine();
            }

            sb.AppendLine("Chi tiết kỹ thuật:");
            sb.AppendLine(BuildTechnicalDetails(ex));

            return sb.ToString();
#endif
        }

#if !DEBUG
        // Tao hoac tinh ra du lieu Build Safe Db Init Error Message tu cac thong tin dau vao hien co.
        private static string BuildSafeDbInitErrorMessage(Exception ex)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Không thể khởi tạo/kết nối cơ sở dữ liệu.");
            sb.AppendLine();

            if (TryGetMigrationChecksumMismatch(ex, out _))
            {
                sb.AppendLine("Phát hiện lỗi migrations (nội dung migrations đã bị thay đổi sau khi áp dụng).");
                sb.AppendLine("Vui lòng cài lại ứng dụng hoặc liên hệ quản trị viên.");
                sb.AppendLine();
            }

            sb.AppendLine("Gợi ý kiểm tra nhanh:");
            sb.AppendLine("- Đảm bảo SQL Server instance đang chạy (ví dụ: SQL Server (SQLEXPRESS)).");
            sb.AppendLine("- Kiểm tra lại cấu hình kết nối trong App.config.");
            sb.AppendLine();
            sb.AppendLine("(Bản Release ẩn chi tiết kỹ thuật để an toàn.)");
            return sb.ToString();
        }
#endif

        // Tao hoac tinh ra du lieu Build Technical Details tu cac thong tin dau vao hien co.
        private static string BuildTechnicalDetails(Exception ex)
        {
            var sb = new StringBuilder();

            Exception cur = ex;
            int depth = 0;
            while (cur != null && depth < 6)
            {
                if (depth > 0) sb.AppendLine("--- Inner ---");

                sb.AppendLine(cur.GetType().FullName);
                sb.AppendLine(cur.Message);

                if (cur is SqlException se)
                {
                    sb.AppendLine($"SqlException.Number={se.Number}");
                    if (se.Errors != null && se.Errors.Count > 0)
                    {
                        var first = se.Errors[0];
                        sb.AppendLine($"SqlError: {first.Number} (State={first.State}, Class={first.Class})");
                    }
                }

                cur = cur.InnerException;
                depth++;
            }

            return sb.ToString().Trim();
        }

        // Thu thuc hien Try Get Sql Exception, neu du lieu khong hop le thi dung va tra thong bao phu hop.
        private static bool TryGetSqlException(Exception ex, out SqlException sqlEx)
        {
            sqlEx = null;
            Exception cur = ex;
            int depth = 0;
            while (cur != null && depth < 8)
            {
                if (cur is SqlException se)
                {
                    sqlEx = se;
                    return true;
                }
                cur = cur.InnerException;
                depth++;
            }
            return false;
        }

        // Thu thuc hien Try Get Migration Checksum Mismatch, neu du lieu khong hop le thi dung va tra thong bao phu hop.
        private static bool TryGetMigrationChecksumMismatch(Exception ex, out string migrationId)
        {
            migrationId = null;
            Exception cur = ex;
            int depth = 0;
            while (cur != null && depth < 8)
            {
                if (cur is MigrationsRunner.MigrationChecksumMismatchException mm)
                {
                    migrationId = mm.MigrationId;
                    return true;
                }
                cur = cur.InnerException;
                depth++;
            }
            return false;
        }

        // Tao hoac tinh ra du lieu Build Tips tu cac thong tin dau vao hien co.
        private static string[] BuildTips(SqlException ex, SqlConnectionStringBuilder b)
        {
            // Common SQL Server issues
            switch (ex.Number)
            {
                case 53: // network path not found
                case 2:
                case 40:
                case 26:
                    return new[]
                    {
                        "SQL Server/instance không truy cập được. Kiểm tra instance name (ví dụ .\\SQLEXPRESS) và dịch vụ SQL Server có đang Running.",
                        "Nếu máy bạn không có SQLEXPRESS, hãy sửa DefaultConnection trong App.config theo instance bạn đang có.",
                    };

                case 18456: // login failed
                    return new[]
                    {
                        "Sai quyền đăng nhập SQL Server. Nếu dùng Windows Auth, kiểm tra tài khoản Windows có quyền truy cập instance/database.",
                        "Nếu bạn muốn dùng SQL Auth, hãy chỉnh DefaultConnection (User ID/Password) và đảm bảo login tồn tại.",
                    };

                case 4060: // Cannot open database requested by the login
                    return new[]
                    {
                        "Database trong DefaultConnection không tồn tại hoặc user không có quyền. Bạn có thể bấm Rebuild DB (DEV) hoặc chạy app để tự tạo DB.",
                        "Kiểm tra tên Database=... trong App.config có đúng không.",
                    };

                case 15247: // User does not have permission
                case 262:
                case 229:
                    return new[]
                    {
                        "Tài khoản hiện tại thiếu quyền tạo DB/tạo table/proc/trigger. Hãy chạy app bằng user có quyền hoặc cấp quyền trên SQL Server.",
                        "Nếu không cần auto tạo DB, bạn có thể chạy script schema thủ công trong SSMS với quyền admin.",
                    };

                default:
                    return new[]
                    {
                        "Mở SSMS và thử chạy SELECT 1 trên database để xác nhận kết nối và quyền.",
                        "Kiểm tra lại DefaultConnection trong App.config (Server/Database).",
                    };
            }
        }

        // Xu ly thao tac du lieu Safe lien quan toi CSDL hoac cau hinh he thong.
        private static string Safe(string s)
        {
            return string.IsNullOrWhiteSpace(s) ? "(empty)" : s;
        }
    }
}


