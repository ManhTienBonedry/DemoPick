using DemoPick.Helpers;
using DemoPick.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace DemoPick.Data
{
    internal static class SqlScriptRunner
    {
        private static readonly Regex GoRegex = new Regex(
            @"^\s*GO(?:\s+(?<count>\d+))?\s*(?:--.*)?$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        private static readonly Regex UseRegex = new Regex(
            @"^\s*USE\s+(?:\[(?<db>[^\]]+)\]|(?<db>[^\s;]+))\s*;?\s*$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline);

        // Thuc thi Execute File, thuong la cau lenh hoac script thao tac voi CSDL.
        internal static void ExecuteFile(string scriptPath, string baseConnectionString)
        {
            if (string.IsNullOrWhiteSpace(scriptPath))
                throw new ArgumentException("scriptPath is required", nameof(scriptPath));

            if (!File.Exists(scriptPath))
                throw new FileNotFoundException("SQL script not found", scriptPath);

            string script = File.ReadAllText(scriptPath, Encoding.UTF8);
            ExecuteScript(script, baseConnectionString);
        }

        // Thuc thi Execute Embedded Resource Suffix, thuong la cau lenh hoac script thao tac voi CSDL.
        internal static void ExecuteEmbeddedResourceSuffix(string resourceNameSuffix, string baseConnectionString)
        {
            if (string.IsNullOrWhiteSpace(resourceNameSuffix))
                throw new ArgumentException("resourceNameSuffix is required", nameof(resourceNameSuffix));

            var asm = Assembly.GetExecutingAssembly();
            var matches = asm
                .GetManifestResourceNames()
                .Where(n => n.EndsWith(resourceNameSuffix, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (matches.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Missing embedded SQL resource (suffix '{resourceNameSuffix}')." +
                    " Ensure the .sql file Build Action is Embedded Resource.");
            }

            if (matches.Count > 1)
            {
                throw new InvalidOperationException(
                    $"Ambiguous embedded SQL resource suffix '{resourceNameSuffix}'. Matches: {string.Join(", ", matches)}");
            }

            string match = matches[0];

            using (Stream s = asm.GetManifestResourceStream(match))
            {
                if (s == null)
                    throw new InvalidOperationException($"Failed to open embedded resource stream: {match}");

                using (var reader = new StreamReader(s, Encoding.UTF8, detectEncodingFromByteOrderMarks: true))
                {
                    string script = reader.ReadToEnd();
                    ExecuteScript(script, baseConnectionString);
                }
            }
        }

        // Thuc thi Execute Script, thuong la cau lenh hoac script thao tac voi CSDL.
        internal static void ExecuteScript(string script, string baseConnectionString)
        {
            if (script == null) throw new ArgumentNullException(nameof(script));
            if (string.IsNullOrWhiteSpace(baseConnectionString))
                throw new ArgumentException("baseConnectionString is required", nameof(baseConnectionString));

            var batches = SplitOnGo(script);

            // Start in master unless the caller already forces a DB.
            var builder = new SqlConnectionStringBuilder(baseConnectionString);
            string currentDb = string.IsNullOrWhiteSpace(builder.InitialCatalog) ? "master" : builder.InitialCatalog;

            using (var conn = new SqlConnection(BuildConnectionString(builder, currentDb)))
            {
                conn.Open();

                for (int i = 0; i < batches.Count; i++)
                {
                    string batch = batches[i]?.Trim();
                    if (string.IsNullOrWhiteSpace(batch))
                        continue;

                    // Switch DB context if the batch is a USE statement.
                    var useMatch = UseRegex.Match(batch);
                    if (useMatch.Success)
                    {
                        string nextDb = useMatch.Groups["db"].Value;
                        if (!string.IsNullOrWhiteSpace(nextDb) && !string.Equals(nextDb, currentDb, StringComparison.OrdinalIgnoreCase))
                        {
                            currentDb = nextDb;
                            conn.Close();
                            conn.ConnectionString = BuildConnectionString(builder, currentDb);
                            conn.Open();
                        }
                        continue;
                    }

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = 60;
                        cmd.CommandText = batch;
                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch (SqlException ex)
                        {
                            string preview = batch.Length <= 300 ? batch : batch.Substring(0, 300) + "...";
                            throw new InvalidOperationException(
                                $"SQL batch failed (batch #{i + 1}, db={currentDb}). Preview: {preview}",
                                ex);
                        }
                    }
                }
            }
        }

        // Tao hoac tinh ra du lieu Build Connection String tu cac thong tin dau vao hien co.
        private static string BuildConnectionString(SqlConnectionStringBuilder baseBuilder, string initialCatalog)
        {
            var b = new SqlConnectionStringBuilder(baseBuilder.ConnectionString)
            {
                InitialCatalog = initialCatalog
            };
            return b.ConnectionString;
        }

        // Dien giai hoac xac dinh Split On Go tu du lieu dau vao de cac buoc sau dung thong nhat.
        private static List<string> SplitOnGo(string script)
        {
            var batches = new List<string>();
            var sb = new StringBuilder();

            using (var sr = new StringReader(script))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var goMatch = GoRegex.Match(line);
                    if (goMatch.Success)
                    {
                        int count = 1;
                        string countText = goMatch.Groups["count"].Value;
                        if (!string.IsNullOrWhiteSpace(countText) && int.TryParse(countText, out int parsed) && parsed > 1)
                            count = parsed;

                        string batchText = sb.ToString();
                        if (!string.IsNullOrWhiteSpace(batchText))
                        {
                            for (int i = 0; i < count; i++)
                                batches.Add(batchText);
                        }
                        sb.Clear();
                        continue;
                    }

                    sb.AppendLine(line);
                }
            }

            // last batch
            if (sb.Length > 0)
                batches.Add(sb.ToString());

            return batches;
        }
    }
}


