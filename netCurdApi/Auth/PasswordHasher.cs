using System.Security.Cryptography;
using System.Text;

namespace netCurdApi.Auth;

public static class PasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    public static string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize);

        return $"pbkdf2:{Iterations}:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(key)}";
    }

    public static bool Verify(string password, string storedHash)
    {
        if (string.IsNullOrWhiteSpace(storedHash))
            return false;

        if (storedHash.StartsWith("pbkdf2:", StringComparison.Ordinal))
            return VerifyPbkdf2(password, storedHash);

        // Legacy seed data uses an unsalted SHA-256 hex hash.
        if (storedHash.Length == 64 && storedHash.All(Uri.IsHexDigit))
            return VerifySha256(password, storedHash);

        return false;
    }

    private static bool VerifyPbkdf2(string password, string storedHash)
    {
        var parts = storedHash.Split(':');
        if (parts.Length != 4 || !int.TryParse(parts[1], out var iterations))
            return false;

        byte[] salt;
        byte[] key;
        try
        {
            salt = Convert.FromBase64String(parts[2]);
            key = Convert.FromBase64String(parts[3]);
        }
        catch (FormatException)
        {
            return false;
        }

        var computed = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, Algorithm, key.Length);
        return CryptographicOperations.FixedTimeEquals(computed, key);
    }

    private static bool VerifySha256(string password, string storedHash)
    {
        var computed = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
        return CryptographicOperations.FixedTimeEquals(
            Encoding.ASCII.GetBytes(computed),
            Encoding.ASCII.GetBytes(storedHash.ToUpperInvariant()));
    }
}
