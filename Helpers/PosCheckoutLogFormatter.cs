using DemoPick.Helpers;
using DemoPick.Data;
using DemoPick.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoPick.Helpers
{
    internal static class PosCheckoutLogFormatter
    {
        // Kiem tra dieu kien Is Test Mode va tra ve ket qua dung/sai cho luong xu ly.
        internal static bool IsTestMode()
        {
            try
            {
                string v = Environment.GetEnvironmentVariable("DEMOPICK_TEST_MODE");
                if (string.IsNullOrWhiteSpace(v)) return false;
                v = v.Trim();
                return v == "1" || v.Equals("true", StringComparison.OrdinalIgnoreCase) || v.Equals("yes", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        // Tao hoac tinh ra du lieu Build Pos Checkout Log Sub Desc tu cac thong tin dau vao hien co.
        internal static string BuildPosCheckoutLogSubDesc(
            int invoiceId,
            string courtName,
            decimal finalAmount,
            string paymentMethod,
            IReadOnlyList<CartLine> lines)
        {
            var parts = new List<string>();
            if (invoiceId > 0) parts.Add("HĐ #" + invoiceId);

            string court = (courtName ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(court))
            {
                parts.Add(court);
            }

            string items = BuildPosProductSummary(lines);
            if (!string.IsNullOrWhiteSpace(items))
            {
                parts.Add(items);
            }

            if (finalAmount > 0)
            {
                parts.Add(finalAmount.ToString("N0") + "đ");
            }

            string methodText = ToPaymentMethodDisplay(paymentMethod);
            if (!string.IsNullOrWhiteSpace(methodText))
            {
                parts.Add(methodText);
            }

            return parts.Count == 0 ? string.Empty : string.Join(" • ", parts);
        }

        // Ho tro xu ly To Payment Method Display de cac noi khac dung lai cung mot quy tac.
        private static string ToPaymentMethodDisplay(string paymentMethod)
        {
            string s = (paymentMethod ?? string.Empty).Trim();
            if (s.Length == 0) return string.Empty;

            if (string.Equals(s, "Cash", StringComparison.OrdinalIgnoreCase)) return "Tiền mặt";
            if (string.Equals(s, "Bank", StringComparison.OrdinalIgnoreCase)) return "Chuyển khoản";
            if (string.Equals(s, "Transfer", StringComparison.OrdinalIgnoreCase)) return "Chuyển khoản";
            return s;
        }

        // Tao hoac tinh ra du lieu Build Pos Product Summary tu cac thong tin dau vao hien co.
        private static string BuildPosProductSummary(IReadOnlyList<CartLine> lines)
        {
            if (lines == null || lines.Count == 0) return string.Empty;

            var byName = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if (line == null) continue;
                if (line.ProductId <= 0) continue;
                if (line.Quantity <= 0) continue;

                string name = (line.ProductName ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(name)) continue;

                if (byName.ContainsKey(name)) byName[name] += line.Quantity;
                else byName.Add(name, line.Quantity);
            }

            if (byName.Count == 0) return string.Empty;

            var items = new List<KeyValuePair<string, int>>(byName);
            items.Sort((a, b) => b.Value.CompareTo(a.Value));

            const int maxItemsToShow = 2;
            var sb = new StringBuilder();
            int shown = 0;

            for (int i = 0; i < items.Count && shown < maxItemsToShow; i++)
            {
                string name = items[i].Key ?? string.Empty;
                int qty = items[i].Value;
                if (qty <= 0) continue;

                if (sb.Length > 0) sb.Append(", ");
                sb.Append(TruncateForUi(name, 18));
                sb.Append(" x");
                sb.Append(qty);
                shown++;
            }

            int remaining = items.Count - shown;
            if (remaining > 0)
            {
                sb.Append(" +");
                sb.Append(remaining);
            }

            return sb.ToString();
        }

        // Ho tro xu ly Truncate For Ui de cac noi khac dung lai cung mot quy tac.
        private static string TruncateForUi(string text, int maxLen)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            if (maxLen <= 0) return string.Empty;
            if (text.Length <= maxLen) return text;
            if (maxLen == 1) return "…";
            return text.Substring(0, maxLen - 1) + "…";
        }
    }
}


