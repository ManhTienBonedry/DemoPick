using DemoPick.Helpers;
using DemoPick.Data;
using System.Text;

namespace DemoPick.Helpers
{
    internal static class PhoneNumberValidator
    {
        // Ap dung hoac chuan hoa trang thai Normalize Digits de du lieu/giao dien nhat quan.
        internal static string NormalizeDigits(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            var sb = new StringBuilder(input.Length);
            foreach (char c in input)
            {
                if (char.IsDigit(c)) sb.Append(c);
            }

            return sb.ToString();
        }

        // Kiem tra dieu kien Is Valid Ten Digits va tra ve ket qua dung/sai cho luong xu ly.
        internal static bool IsValidTenDigits(string input)
        {
            string digits = NormalizeDigits(input);
            return digits.Length == 10;
        }
    }
}

