using System;
using System.Configuration;
using System.Data.SqlClient;

namespace DemoPick.Services
{
    internal static class Db
    {
        internal const string DefaultConnectionName = "DefaultConnection";
        internal const string ConnectionStringEnvVar = "DEMOPICK_CONNECTION_STRING";
        internal const string ProtectConnectionStringsEnvVar = "DEMOPICK_PROTECT_CONNECTIONSTRINGS";
        internal const string ProtectConnectionStringsAppSetting = "ProtectConnectionStrings";

        private static bool _protectAttempted;

        internal static string ConnectionString
        {
            get
            {
                var env = Environment.GetEnvironmentVariable(ConnectionStringEnvVar);
                if (!string.IsNullOrWhiteSpace(env))
                    return env.Trim();

                TryProtectConnectionStringsSectionIfEnabled();

                var cs = ConfigurationManager.ConnectionStrings[DefaultConnectionName]?.ConnectionString;
                if (string.IsNullOrWhiteSpace(cs))
                    throw new InvalidOperationException($"Missing connection string: {DefaultConnectionName}");
                return cs;
            }
        }

        internal static void TryProtectConnectionStringsSectionIfEnabled()
        {
            if (_protectAttempted) return;
            _protectAttempted = true;

            if (!IsProtectionEnabled()) return;

            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var section = config.GetSection("connectionStrings");
                if (section == null) return;

                if (section.SectionInformation.IsProtected) return;

                section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
                section.SectionInformation.ForceSave = true;
                config.Save(ConfigurationSaveMode.Modified);

                // Refresh so ConfigurationManager picks up the protected section.
                ConfigurationManager.RefreshSection("connectionStrings");
            }
            catch
            {
                // Best-effort only.
            }
        }

        private static bool IsProtectionEnabled()
        {
            try
            {
                if (IsTruthy(Environment.GetEnvironmentVariable(ProtectConnectionStringsEnvVar)))
                    return true;
            }
            catch { }

            try
            {
                return IsTruthy(ConfigurationManager.AppSettings[ProtectConnectionStringsAppSetting]);
            }
            catch
            {
                return false;
            }
        }

        private static bool IsTruthy(string v)
        {
            if (string.IsNullOrWhiteSpace(v)) return false;
            v = v.Trim();
            return v == "1" || v.Equals("true", StringComparison.OrdinalIgnoreCase) || v.Equals("yes", StringComparison.OrdinalIgnoreCase);
        }

        internal static SqlConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        internal static SqlConnectionStringBuilder CreateBuilder()
        {
            return new SqlConnectionStringBuilder(ConnectionString);
        }
    }
}
