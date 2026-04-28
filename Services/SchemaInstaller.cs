using System;
using System.Data.SqlClient;
#if DEBUG
using System.Windows.Forms;
#endif

namespace DemoPick.Services
{
    internal static class SchemaInstaller
    {
        internal static void EnsureDatabaseAndSchema()
        {
            EnsureDatabaseAndSchemaCore(rebuild: false);
        }

        internal static void RebuildDatabaseAndSchema()
        {
            EnsureDatabaseAndSchemaCore(rebuild: true);
        }

        private static void EnsureDatabaseAndSchemaCore(bool rebuild)
        {
            var baseBuilder = new SqlConnectionStringBuilder(Db.ConnectionString);
            string dbName = baseBuilder.InitialCatalog;
            if (string.IsNullOrWhiteSpace(dbName))
                throw new InvalidOperationException("Connection string is missing Initial Catalog/Database name.");

            var master = new SqlConnectionStringBuilder(baseBuilder.ConnectionString)
            {
                InitialCatalog = "master"
            };

            if (rebuild)
            {
                DropDatabaseIfExists(master.ConnectionString, dbName);
            }

            EnsureDatabaseExists(master.ConnectionString, dbName);

            var targetDb = new SqlConnectionStringBuilder(baseBuilder.ConnectionString)
            {
                InitialCatalog = dbName
            };

            // Run bootstrap SQL from embedded resources (avoid tampering with .sql files on disk).
            SqlScriptRunner.ExecuteEmbeddedResourceSuffix(
                ".Database.PickleBallDB_Complete.sql",
                targetDb.ConnectionString);

            // SECURITY: never seed a predictable admin in Release builds.
            // DEBUG-only convenience seeding: either uses env var DEMOPICK_BOOTSTRAP_ADMIN_PASSWORD or generates a random one.
#if DEBUG
            try
            {
                if (AuthService.TrySeedAdminIfEmpty(out string seededUser, out string seededPass))
                {
                    bool hasBootstrapPassword = !string.IsNullOrWhiteSpace(
                        Environment.GetEnvironmentVariable("DEMOPICK_BOOTSTRAP_ADMIN_PASSWORD"));

                    bool suppressUi = string.Equals(
                        Environment.GetEnvironmentVariable("DEMOPICK_SUPPRESS_UI"),
                        "1",
                        StringComparison.OrdinalIgnoreCase);

                    if (!suppressUi)
                    {
                        string passHint = hasBootstrapPassword
                            ? "Password: [HIDDEN - set via DEMOPICK_BOOTSTRAP_ADMIN_PASSWORD]"
                            : "Password: [HIDDEN - random generated for DEBUG; set DEMOPICK_BOOTSTRAP_ADMIN_PASSWORD for deterministic bootstrap]";

                        MessageBox.Show(
                            $"[DEBUG] Đã tạo tài khoản Admin mặc định.\n\nUser: {seededUser}\n{passHint}\n\n" +
                            "Vì lý do bảo mật, mật khẩu không hiển thị trên UI.",
                            "Bootstrap Admin (DEBUG)",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
            }
            catch
            {
                // Best effort.
            }
#endif
        }

        private static void DropDatabaseIfExists(string masterConnectionString, string dbName)
        {
            using (var conn = new SqlConnection(masterConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = @"
IF DB_ID(@db) IS NOT NULL
BEGIN
    DECLARE @sql NVARCHAR(MAX) =
        N'ALTER DATABASE ' + QUOTENAME(@db) + N' SET SINGLE_USER WITH ROLLBACK IMMEDIATE; '
        + N'DROP DATABASE ' + QUOTENAME(@db) + N';';
    EXEC sys.sp_executesql @sql;
END
";
                cmd.Parameters.AddWithValue("@db", dbName);
                cmd.ExecuteNonQuery();
            }
        }

        private static void EnsureDatabaseExists(string masterConnectionString, string dbName)
        {
            using (var conn = new SqlConnection(masterConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = @"
IF DB_ID(@db) IS NULL
BEGIN
    DECLARE @sql NVARCHAR(MAX) = N'CREATE DATABASE ' + QUOTENAME(@db);
    EXEC sys.sp_executesql @sql;
END
";
                cmd.Parameters.AddWithValue("@db", dbName);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
