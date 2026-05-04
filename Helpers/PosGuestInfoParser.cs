using DemoPick.Helpers;
using DemoPick.Data;
using System;
using System.Text;

namespace DemoPick.Helpers
{
    internal static class PosGuestInfoParser
    {
        // Dien giai hoac xac dinh Parse Guest Info tu du lieu dau vao de cac buoc sau dung thong nhat.
        internal static void ParseGuestInfo(string guestNameRaw, out string fullName, out string phone)
        {
            fullName = string.Empty;
            phone = string.Empty;

            string raw = (guestNameRaw ?? string.Empty).Trim();
            if (raw.Length == 0) return;

            int sep = raw.LastIndexOf(" - ", StringComparison.Ordinal);
            if (sep >= 0)
            {
                fullName = raw.Substring(0, sep).Trim();
                phone = raw.Substring(sep + 3).Trim();
            }
            else
            {
                int dash = raw.LastIndexOf('-');
                if (dash > 0 && dash < raw.Length - 1)
                {
                    fullName = raw.Substring(0, dash).Trim();
                    phone = raw.Substring(dash + 1).Trim();
                }
                else
                {
                    fullName = raw;
                }
            }

            phone = NormalizePhone(phone);

            if (string.IsNullOrWhiteSpace(fullName) && !string.IsNullOrWhiteSpace(phone))
                fullName = "Khach " + phone;
        }

        // Ap dung hoac chuan hoa trang thai Normalize Phone de du lieu/giao dien nhat quan.
        private static string NormalizePhone(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            var sb = new StringBuilder(input.Length);
            foreach (char c in input)
            {
                if (char.IsDigit(c)) sb.Append(c);
            }

            return sb.ToString();
        }
    }
}


