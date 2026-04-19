using System.Text;

namespace DemoPick.Services
{
    internal static class PhoneNumberValidator
    {
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

        internal static bool IsValidTenDigits(string input)
        {
            string digits = NormalizeDigits(input);
            return digits.Length == 10;
        }
    }
}