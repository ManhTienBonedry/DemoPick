using DemoPick.Helpers;
using DemoPick.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace DemoPick.Data
{
    public class DatabaseMaintenanceService
    {
        // Thu thuc hien Try Heal Corrupted Court Names, neu du lieu khong hop le thi dung va tra thong bao phu hop.
        public void TryHealCorruptedCourtNames()
        {
            // Self-heal corrupted database text (tr?i instead of trời)
            try
            {
                DatabaseHelper.ExecuteNonQuery(
                    "UPDATE Courts SET Name = REPLACE(Name, 'tr?i', N'trời') WHERE Name LIKE '%tr?i%';"
                );
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLog("DB Self-heal Skipped", ex, "DatabaseMaintenanceService.TryHealCorruptedCourtNames");
            }
        }

        // Thu thuc hien Try Purge Orphan Pos Checkout Logs, neu du lieu khong hop le thi dung va tra thong bao phu hop.
        public void TryPurgeOrphanPosCheckoutLogs(int scanTop = 300)
        {
            try
            {
                var dt = DatabaseHelper.ExecuteQuery(
                    "SELECT TOP (@Take) LogID, SubDesc FROM dbo.SystemLogs WHERE EventDesc = N'POS Checkout' ORDER BY CreatedAt DESC",
                    new SqlParameter("@Take", Math.Max(1, Math.Min(scanTop, 2000))));

                var logToInvoice = new List<KeyValuePair<int, int>>();
                var invoiceIds = new HashSet<int>();

                foreach (DataRow row in dt.Rows)
                {
                    int logId = row["LogID"] == DBNull.Value ? 0 : Convert.ToInt32(row["LogID"]);
                    if (logId <= 0) continue;

                    string sub = row["SubDesc"] == DBNull.Value ? "" : (row["SubDesc"]?.ToString() ?? "");
                    int invoiceId = TryExtractInvoiceIdFromPosCheckoutSubDesc(sub);
                    if (invoiceId <= 0) continue;

                    logToInvoice.Add(new KeyValuePair<int, int>(logId, invoiceId));
                    invoiceIds.Add(invoiceId);
                }

                if (invoiceIds.Count == 0) return;

                var existing = LoadExistingInvoiceIds(invoiceIds);

                var logsToDelete = new List<int>();
                foreach (var pair in logToInvoice)
                {
                    if (!existing.Contains(pair.Value))
                    {
                        logsToDelete.Add(pair.Key);
                    }
                }

                if (logsToDelete.Count == 0) return;

                // Delete in batches to avoid huge parameter lists.
                const int batchSize = 50;
                for (int i = 0; i < logsToDelete.Count; i += batchSize)
                {
                    int take = Math.Min(batchSize, logsToDelete.Count - i);
                    var sb = new StringBuilder();
                    var ps = new List<SqlParameter>();
                    sb.Append("DELETE FROM dbo.SystemLogs WHERE LogID IN (");
                    for (int k = 0; k < take; k++)
                    {
                        if (k > 0) sb.Append(", ");
                        string pName = "@L" + k;
                        sb.Append(pName);
                        ps.Add(new SqlParameter(pName, logsToDelete[i + k]));
                    }
                    sb.Append(")");

                    DatabaseHelper.ExecuteNonQuery(sb.ToString(), ps.ToArray());
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLog("DB Purge Orphan POS Logs Skipped", ex, "DatabaseMaintenanceService.TryPurgeOrphanPosCheckoutLogs");
            }
        }

        // Lay hoac nap du lieu cho Load Existing Invoice Ids tu CSDL/nguon cau hinh.
        private static HashSet<int> LoadExistingInvoiceIds(HashSet<int> invoiceIds)
        {
            var existing = new HashSet<int>();
            if (invoiceIds == null || invoiceIds.Count == 0) return existing;

            var sb = new StringBuilder();
            var ps = new List<SqlParameter>();
            sb.Append("SELECT InvoiceID FROM dbo.Invoices WHERE InvoiceID IN (");

            int idx = 0;
            foreach (int id in invoiceIds)
            {
                if (idx > 0) sb.Append(", ");
                string pName = "@I" + idx;
                sb.Append(pName);
                ps.Add(new SqlParameter(pName, id));
                idx++;
            }

            sb.Append(")");

            var dt = DatabaseHelper.ExecuteQuery(sb.ToString(), ps.ToArray());
            foreach (DataRow row in dt.Rows)
            {
                if (row[0] == DBNull.Value) continue;
                existing.Add(Convert.ToInt32(row[0]));
            }

            return existing;
        }

        // Thu thuc hien Try Extract Invoice Id From Pos Checkout Sub Desc, neu du lieu khong hop le thi dung va tra thong bao phu hop.
        private static int TryExtractInvoiceIdFromPosCheckoutSubDesc(string subDesc)
        {
            if (string.IsNullOrWhiteSpace(subDesc)) return 0;
            string s = subDesc.Trim();

            // New format: "HĐ #12 • ..."
            int hash = s.IndexOf('#');
            if (hash >= 0)
            {
                int i = hash + 1;
                while (i < s.Length && char.IsWhiteSpace(s[i])) i++;
                int j = i;
                while (j < s.Length && char.IsDigit(s[j])) j++;
                if (j > i && int.TryParse(s.Substring(i, j - i), out int idFromHash))
                    return idFromHash;
            }

            // Legacy debug format: "InvoiceID=12; ..."
            const string key = "InvoiceID=";
            int pos = s.IndexOf(key, StringComparison.OrdinalIgnoreCase);
            if (pos >= 0)
            {
                int i = pos + key.Length;
                while (i < s.Length && char.IsWhiteSpace(s[i])) i++;
                int j = i;
                while (j < s.Length && char.IsDigit(s[j])) j++;
                if (j > i && int.TryParse(s.Substring(i, j - i), out int idFromKey))
                    return idFromKey;
            }

            return 0;
        }
    }
}


