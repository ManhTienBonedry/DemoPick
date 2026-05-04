using DemoPick.Helpers;
using DemoPick.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Reflection;

namespace DemoPick.Data
{
    internal static class MigrationsRunner
    {
        internal sealed class MigrationChecksumMismatchException : InvalidOperationException
        {
            internal string MigrationId { get; }

            // Khoi tao MigrationChecksumMismatchException va thiet lap trang thai ban dau can dung.
            internal MigrationChecksumMismatchException(string migrationId)
                : base($"Migration '{migrationId}' was already applied but its contents changed.")
            {
                MigrationId = migrationId;
            }
        }

        // Ap dung cac migration CSDL chua chay de dua schema len phien ban moi nhat.
        internal static void ApplyPendingMigrations()
        {
            var builder = Db.CreateBuilder();
            string dbName = builder.InitialCatalog;
            if (string.IsNullOrWhiteSpace(dbName))
                throw new InvalidOperationException("Connection string is missing Initial Catalog/Database name.");

            EnsureMigrationsTableExists();

            Dictionary<string, byte[]> applied = LoadAppliedMigrations();

            var embedded = GetEmbeddedMigrations()
                .OrderBy(m => m.MigrationId, StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var mig in embedded)
            {
                string migrationId = mig.MigrationId;
                if (string.Equals(migrationId, "0000__README.sql", StringComparison.OrdinalIgnoreCase))
                    continue;

                byte[] bytes = ReadAllBytes(mig.ResourceName);
                byte[] checksum = ComputeSha256(bytes);

                if (applied.TryGetValue(migrationId, out var appliedChecksum))
                {
                    if (appliedChecksum != null && appliedChecksum.Length == 32 && !checksum.SequenceEqual(appliedChecksum))
                    {
                        throw new MigrationChecksumMismatchException(migrationId);
                    }

                    continue;
                }

                string script = Encoding.UTF8.GetString(bytes);
                if (!string.IsNullOrEmpty(script) && script[0] == '\uFEFF')
                {
                    script = script.Substring(1);
                }
                SqlScriptRunner.ExecuteScript(script, Db.ConnectionString);
                MarkApplied(migrationId, checksum);

                applied[migrationId] = checksum;
            }
        }

        private sealed class EmbeddedMigration
        {
            internal string ResourceName { get; set; }
            internal string MigrationId { get; set; }
        }

        // Lay hoac nap du lieu cho Get Embedded Migrations tu CSDL/nguon cau hinh.
        private static IEnumerable<EmbeddedMigration> GetEmbeddedMigrations()
        {
            var asm = Assembly.GetExecutingAssembly();
            foreach (string res in asm.GetManifestResourceNames())
            {
                // Expected: DemoPick.Database.Migrations.0001__something.sql
                int idx = res.IndexOf(".Database.Migrations.", StringComparison.OrdinalIgnoreCase);
                if (idx < 0)
                    continue;

                if (!res.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
                    continue;

                string after = res.Substring(idx + ".Database.Migrations.".Length);
                if (string.IsNullOrWhiteSpace(after))
                    continue;

                yield return new EmbeddedMigration
                {
                    ResourceName = res,
                    MigrationId = after
                };
            }
        }

        // Lay hoac nap du lieu cho Read All Bytes tu CSDL/nguon cau hinh.
        private static byte[] ReadAllBytes(string resourceName)
        {
            var asm = Assembly.GetExecutingAssembly();
            using (var s = asm.GetManifestResourceStream(resourceName))
            {
                if (s == null)
                    throw new InvalidOperationException($"Cannot open embedded migration resource: {resourceName}");

                using (var ms = new System.IO.MemoryStream())
                {
                    s.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }

        // Dam bao dieu kien Ensure Migrations Table Exists da san sang truoc khi chay buoc xu ly tiep theo.
        private static void EnsureMigrationsTableExists()
        {
            DatabaseHelper.ExecuteNonQuery(SqlQueries.Migrations.EnsureMigrationsTableExists);
        }

        // Lay hoac nap du lieu cho Load Applied Migrations tu CSDL/nguon cau hinh.
        private static Dictionary<string, byte[]> LoadAppliedMigrations()
        {
            var result = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);

            DataTable dt = DatabaseHelper.ExecuteQuery(SqlQueries.Migrations.LoadAppliedMigrations);
            foreach (DataRow row in dt.Rows)
            {
                string id = row["MigrationId"] as string;
                if (string.IsNullOrWhiteSpace(id))
                    continue;

                byte[] checksum = row["Checksum"] == DBNull.Value ? null : (byte[])row["Checksum"];
                result[id] = checksum;
            }

            return result;
        }

        // Luu hoac ghi nhan Mark Applied vao trang thai he thong/CSDL khi nghiep vu yeu cau.
        private static void MarkApplied(string migrationId, byte[] checksum)
        {
            DatabaseHelper.ExecuteNonQuery(
                SqlQueries.Migrations.MarkApplied,
                new SqlParameter("@Id", migrationId),
                new SqlParameter("@Checksum", (object)checksum ?? DBNull.Value));
        }

        // Tao hoac tinh ra du lieu Compute Sha256 tu cac thong tin dau vao hien co.
        private static byte[] ComputeSha256(byte[] bytes)
        {
            using (var sha = SHA256.Create())
            {
                return sha.ComputeHash(bytes);
            }
        }
    }
}


