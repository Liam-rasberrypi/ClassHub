using System;
using System.Security.Cryptography;

namespace ClassHub.Services
{
    /// <summary>Versioned, bounded PBKDF2-SHA256 password hashes. Passwords are never trimmed.</summary>
    public static class PasswordHasher
    {
        private const int Iterations = 600_000;
        private const int SaltBytes = 16;
        private const int HashBytes = 32;
        private const string Prefix = "v1$pbkdf2-sha256$600000$";

        public static void ValidateNewPassword(string password)
        {
            if (password == null || password.Length < 12 || password.Length > 128)
                throw new ArgumentException("管理员密码应为 12–128 个字符。", nameof(password));
        }

        public static string Hash(string password)
        {
            ValidateNewPassword(password);
            byte[] salt = RandomNumberGenerator.GetBytes(SaltBytes);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashBytes);
            try
            {
                return Prefix + Convert.ToBase64String(salt) + "$" + Convert.ToBase64String(hash);
            }
            finally
            {
                CryptographicOperations.ZeroMemory(hash);
            }
        }

        public static bool Verify(string password, string storedHash)
        {
            if (password == null || password.Length > 128 || !TryParse(storedHash, out byte[] salt, out byte[] expected))
                return false;
            byte[] actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashBytes);
            try
            {
                return CryptographicOperations.FixedTimeEquals(actual, expected);
            }
            finally
            {
                CryptographicOperations.ZeroMemory(actual);
                CryptographicOperations.ZeroMemory(expected);
            }
        }

        public static bool IsValidHash(string storedHash) => TryParse(storedHash, out _, out _);

        private static bool TryParse(string storedHash, out byte[] salt, out byte[] hash)
        {
            salt = Array.Empty<byte>();
            hash = Array.Empty<byte>();
            // Exact lengths and iteration text prevent attacker-controlled PBKDF2 work factors.
            if (storedHash == null || storedHash.Length != Prefix.Length + 24 + 1 + 44 ||
                !storedHash.StartsWith(Prefix, StringComparison.Ordinal))
                return false;
            string saltText = storedHash.Substring(Prefix.Length, 24);
            if (storedHash[Prefix.Length + 24] != '$')
                return false;
            string hashText = storedHash.Substring(Prefix.Length + 25, 44);
            try
            {
                salt = Convert.FromBase64String(saltText);
                hash = Convert.FromBase64String(hashText);
                return salt.Length == SaltBytes && hash.Length == HashBytes &&
                    Convert.ToBase64String(salt) == saltText && Convert.ToBase64String(hash) == hashText;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
