using DemoPick.Helpers;
using DemoPick.Data;
using System;
using System.Security.Cryptography;
using System.Text;

namespace DemoPick.Helpers
{
    internal static class AuthPasswordCrypto
    {
        private const int Pbkdf2Iterations = 100_000;
        private const int PasswordHashBytes = 32;

        // Tao hoac tinh ra du lieu Generate Salt tu cac thong tin dau vao hien co.
        internal static byte[] GenerateSalt(int size)
        {
            var salt = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        // Kiem tra dieu kien Hash Password va tra ve ket qua dung/sai cho luong xu ly.
        internal static byte[] HashPassword(string password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password ?? string.Empty, salt, Pbkdf2Iterations, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(PasswordHashBytes);
            }
        }

        // Kiem tra dieu kien Hash Password Legacy Sha1 va tra ve ket qua dung/sai cho luong xu ly.
        internal static byte[] HashPasswordLegacySha1(string password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password ?? string.Empty, salt, Pbkdf2Iterations))
            {
                return pbkdf2.GetBytes(PasswordHashBytes);
            }
        }

        // Ho tro xu ly Fixed Time Equals de cac noi khac dung lai cung mot quy tac.
        internal static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }

        // Tao hoac tinh ra du lieu Generate Random Password tu cac thong tin dau vao hien co.
        internal static string GenerateRandomPassword(int length)
        {
            const string alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789!@#$%";
            if (length < 8) length = 8;

            int n = alphabet.Length;
            int maxValid = 256 - (256 % n);

            byte[] singleByte = new byte[1];
            using (var rng = RandomNumberGenerator.Create())
            {
                var sb = new StringBuilder(length);
                while (sb.Length < length)
                {
                    rng.GetBytes(singleByte);
                    if (singleByte[0] < maxValid)
                    {
                        sb.Append(alphabet[singleByte[0] % n]);
                    }
                }
                return sb.ToString();
            }
        }
    }
}


