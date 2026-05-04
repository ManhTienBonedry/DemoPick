using DemoPick.Helpers;
using DemoPick.Data;
using System;
using System.Collections.Generic;

namespace DemoPick.Helpers
{
    internal static class InventoryTransactionFormatter
    {
        // Thu thuc hien Try Extract Invoice Id, neu du lieu khong hop le thi dung va tra thong bao phu hop.
        internal static int TryExtractInvoiceId(string subDesc)
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
            int idx = s.IndexOf(key, StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
            {
                int i = idx + key.Length;
                while (i < s.Length && char.IsWhiteSpace(s[i])) i++;
                int j = i;
                while (j < s.Length && char.IsDigit(s[j])) j++;
                if (j > i && int.TryParse(s.Substring(i, j - i), out int idFromKey))
                    return idFromKey;
            }

            return 0;
        }

        // Ho tro xu ly Map Event For Ui de cac noi khac dung lai cung mot quy tac.
        internal static string MapEventForUi(string eventDesc)
        {
            string e = (eventDesc ?? string.Empty).Trim();
            if (string.Equals(e, "POS Checkout", StringComparison.OrdinalIgnoreCase)) return "POS";
            if (string.Equals(e, "Nhập Kho Trực Tiếp", StringComparison.OrdinalIgnoreCase)) return "Nhập kho";
            if (string.Equals(e, "Xóa Sản phẩm Kho", StringComparison.OrdinalIgnoreCase)) return "Xóa kho";
            return e;
        }

        // Ho tro xu ly Format Sub Desc For Ui de cac noi khac dung lai cung mot quy tac.
        internal static string FormatSubDescForUi(string eventDesc, string subDesc)
        {
            string e = (eventDesc ?? string.Empty).Trim();
            string sub = (subDesc ?? string.Empty).Replace("\r", " ").Replace("\n", " ").Trim();
            if (sub.Length == 0) return string.Empty;

            if (string.Equals(e, "POS Checkout", StringComparison.OrdinalIgnoreCase))
            {
                string formatted = TryFormatPosCheckoutSubDesc(sub);
                return string.IsNullOrWhiteSpace(formatted) ? sub : formatted;
            }

            return sub;
        }

        // Thu thuc hien Try Format Pos Checkout Sub Desc, neu du lieu khong hop le thi dung va tra thong bao phu hop.
        private static string TryFormatPosCheckoutSubDesc(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;

            // Handle legacy key-value logs first.
            if (raw.IndexOf("InvoiceID=", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var kv = ParseKeyValuePairs(raw);

                int invoiceIdLegacy = TryGetInt(kv, "InvoiceID");
                string courtLegacy = TryGetString(kv, "Court");
                string totalLegacy = TryGetString(kv, "Total");
                string methodLegacy = TryGetString(kv, "Method");
                string methodUiLegacy = ToPaymentMethodDisplay(methodLegacy);

                var partsLegacy = new List<string>();

                string courtUiLegacy = NormalizeCourtForUi(courtLegacy);
                if (!string.IsNullOrWhiteSpace(courtUiLegacy)) partsLegacy.Add(courtUiLegacy);

                if (invoiceIdLegacy > 0) partsLegacy.Add("HĐ" + invoiceIdLegacy);
                if (!string.IsNullOrWhiteSpace(totalLegacy) && totalLegacy != "-") partsLegacy.Add(totalLegacy);
                if (!string.IsNullOrWhiteSpace(methodUiLegacy)) partsLegacy.Add(methodUiLegacy);

                return partsLegacy.Count == 0 ? raw : string.Join(" • ", partsLegacy);
            }

            // New style logs: normalize token order for compact/professional display.
            string[] tokens = raw.Split(new[] { '•' }, StringSplitOptions.RemoveEmptyEntries);
            string invoiceToken = string.Empty;
            string courtToken = string.Empty;
            string itemsToken = string.Empty;
            string amountToken = string.Empty;
            string paymentToken = string.Empty;

            for (int i = 0; i < tokens.Length; i++)
            {
                string token = (tokens[i] ?? string.Empty).Trim();
                if (token.Length == 0) continue;

                int invoiceId = TryExtractInvoiceId(token);
                if (invoiceId > 0)
                {
                    invoiceToken = "HĐ" + invoiceId;
                    continue;
                }

                if (token.IndexOf("đ", StringComparison.OrdinalIgnoreCase) >= 0 && string.IsNullOrWhiteSpace(amountToken))
                {
                    amountToken = token;
                    continue;
                }

                string paymentUi = ToPaymentMethodDisplay(token);
                if (!string.IsNullOrWhiteSpace(paymentUi) &&
                    (string.Equals(paymentUi, "Tiền mặt", StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(paymentUi, "Chuyển khoản", StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(paymentUi, token, StringComparison.OrdinalIgnoreCase)))
                {
                    if (string.IsNullOrWhiteSpace(paymentToken))
                    {
                        paymentToken = paymentUi;
                        continue;
                    }
                }

                if (LooksLikeItemSummaryToken(token) && string.IsNullOrWhiteSpace(itemsToken))
                {
                    itemsToken = NormalizeItemSummaryToken(token);
                    continue;
                }

                if (string.IsNullOrWhiteSpace(courtToken))
                {
                    courtToken = NormalizeCourtForUi(token);
                }
            }

            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(courtToken)) parts.Add(courtToken);
            if (!string.IsNullOrWhiteSpace(invoiceToken)) parts.Add(invoiceToken);
            if (!string.IsNullOrWhiteSpace(itemsToken)) parts.Add(itemsToken);
            if (!string.IsNullOrWhiteSpace(amountToken) && amountToken != "-") parts.Add(amountToken);
            if (!string.IsNullOrWhiteSpace(paymentToken)) parts.Add(paymentToken);

            return parts.Count == 0 ? raw : string.Join(" • ", parts);
        }

        // Ho tro xu ly Looks Like Item Summary Token de cac noi khac dung lai cung mot quy tac.
        private static bool LooksLikeItemSummaryToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return false;

            string t = token.Trim();
            if (t.IndexOf(" x", StringComparison.OrdinalIgnoreCase) >= 0) return true;
            if (t.IndexOf(",", StringComparison.OrdinalIgnoreCase) >= 0) return true;
            if (t.StartsWith("+", StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        // Ap dung hoac chuan hoa trang thai Normalize Item Summary Token de du lieu/giao dien nhat quan.
        private static string NormalizeItemSummaryToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return string.Empty;

            string[] chunks = token.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var normalized = new List<string>();
            int hiddenCount = 0;

            for (int i = 0; i < chunks.Length; i++)
            {
                string c = (chunks[i] ?? string.Empty).Trim();
                if (c.Length == 0) continue;

                if (c.StartsWith("+", StringComparison.Ordinal))
                {
                    int add;
                    if (int.TryParse(c.Substring(1).Trim(), out add) && add > 0)
                    {
                        hiddenCount += add;
                    }
                    continue;
                }

                int xPos = c.LastIndexOf(" x", StringComparison.OrdinalIgnoreCase);
                if (xPos > 0)
                {
                    string name = c.Substring(0, xPos).Trim();
                    string qtyRaw = c.Substring(xPos + 2).Trim();
                    int qty;
                    if (int.TryParse(qtyRaw, out qty) && qty > 0 && !string.IsNullOrWhiteSpace(name))
                    {
                        normalized.Add(qty + " " + name);
                        continue;
                    }
                }

                normalized.Add(c);
            }

            if (normalized.Count == 0 && hiddenCount <= 0)
            {
                return token;
            }

            string head = string.Join(", ", normalized.ToArray());
            if (hiddenCount > 0)
            {
                if (head.Length > 0) head += " ";
                head += "(+" + hiddenCount + " món)";
            }
            return head;
        }

        // Ap dung hoac chuan hoa trang thai Normalize Court For Ui de du lieu/giao dien nhat quan.
        private static string NormalizeCourtForUi(string court)
        {
            string c = (court ?? string.Empty).Trim();
            if (c.Length == 0 || c == "-") return string.Empty;

            if (c.StartsWith("Sân ", StringComparison.OrdinalIgnoreCase))
            {
                return c;
            }

            return "Sân " + c;
        }

        // Dien giai hoac xac dinh Parse Key Value Pairs tu du lieu dau vao de cac buoc sau dung thong nhat.
        private static Dictionary<string, string> ParseKeyValuePairs(string raw)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(raw)) return dict;

            string[] parts = raw.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parts.Length; i++)
            {
                string p = (parts[i] ?? string.Empty).Trim();
                if (p.Length == 0) continue;

                int eq = p.IndexOf('=');
                if (eq <= 0) continue;

                string key = p.Substring(0, eq).Trim();
                string val = p.Substring(eq + 1).Trim();
                if (key.Length == 0) continue;

                dict[key] = val;
            }

            return dict;
        }

        // Thu thuc hien Try Get Int, neu du lieu khong hop le thi dung va tra thong bao phu hop.
        private static int TryGetInt(Dictionary<string, string> kv, string key)
        {
            if (kv == null || string.IsNullOrWhiteSpace(key)) return 0;
            if (!kv.TryGetValue(key, out var raw)) return 0;
            if (string.IsNullOrWhiteSpace(raw)) return 0;
            return int.TryParse(raw.Trim(), out int val) ? val : 0;
        }

        // Thu thuc hien Try Get String, neu du lieu khong hop le thi dung va tra thong bao phu hop.
        private static string TryGetString(Dictionary<string, string> kv, string key)
        {
            if (kv == null || string.IsNullOrWhiteSpace(key)) return string.Empty;
            if (!kv.TryGetValue(key, out var raw)) return string.Empty;
            return (raw ?? string.Empty).Trim();
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
    }
}


