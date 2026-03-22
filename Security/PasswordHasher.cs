using System.Security.Cryptography;

namespace CPTM_Backend.Security;

public static class PasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;

    public static (string Hash, string Salt) HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            KeySize);

        return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
    }

    public static bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        var saltBytes = Convert.FromBase64String(storedSalt);
        var computedHash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            saltBytes,
            Iterations,
            HashAlgorithmName.SHA256,
            KeySize);

        var storedHashBytes = Convert.FromBase64String(storedHash);
        return CryptographicOperations.FixedTimeEquals(computedHash, storedHashBytes);
    }
}
