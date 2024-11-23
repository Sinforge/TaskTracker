using System.Security.Cryptography;

namespace AuthService.Utils;

public static class PasswordUtils
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100000;

    public static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);

        var saltAndHash = new byte[SaltSize + KeySize];
        Buffer.BlockCopy(salt, 0, saltAndHash, 0, SaltSize);
        Buffer.BlockCopy(hash, 0, saltAndHash, SaltSize, KeySize);

        return Convert.ToBase64String(saltAndHash);
    }
    
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        var saltAndHash = Convert.FromBase64String(hashedPassword);

        var salt = new byte[16];
        var hash = new byte[32];
        Buffer.BlockCopy(saltAndHash, 0, salt, 0, salt.Length);
        Buffer.BlockCopy(saltAndHash, salt.Length, hash, 0, hash.Length);

        var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100000, HashAlgorithmName.SHA256, hash.Length);

        return CryptographicOperations.FixedTimeEquals(hash, hashToCompare);
    }
}