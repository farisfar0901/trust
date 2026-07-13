using System.Security.Cryptography;

namespace Trust.Api.Application.Common;

/// <summary>
/// Hashes passwords using PBKDF2 (RFC 2898) with a random salt per password
/// and a SHA-256 pseudo-random function. This uses only the .NET base class
/// library, so it needs no extra NuGet package.
/// </summary>
public sealed class Pbkdf2PasswordHasher : IPasswordHasher
{
    /// <summary>
    /// Number of PBKDF2 iterations used for new hashes. Stored alongside
    /// each hash so it can be increased later without breaking the ability
    /// to verify passwords hashed with an older, lower iteration count.
    /// </summary>
    private const int Iterations = 100_000;

    private const int SaltSizeInBytes = 16;
    private const int KeySizeInBytes = 32;

    /// <inheritdoc />
    public string HashPassword(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var salt = RandomNumberGenerator.GetBytes(SaltSizeInBytes);
        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySizeInBytes);

        return FormatHash(Iterations, salt, key);
    }

    /// <inheritdoc />
    public bool VerifyPassword(string password, string hashedPassword)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        ArgumentException.ThrowIfNullOrWhiteSpace(hashedPassword);

        if (!TryParseHash(hashedPassword, out var iterations, out var salt, out var expectedKey))
        {
            return false;
        }

        var actualKey = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expectedKey.Length);

        // Constant-time comparison prevents timing attacks from leaking how
        // many leading bytes of the hash were correct.
        return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
    }

    /// <summary>
    /// Combines the iteration count, salt, and derived key into a single
    /// storable string in the form <c>iterations.salt.key</c>.
    /// </summary>
    private static string FormatHash(int iterations, byte[] salt, byte[] key)
        => $"{iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";

    /// <summary>
    /// Splits a stored hash string back into its iteration count, salt, and key.
    /// </summary>
    private static bool TryParseHash(string hashedPassword, out int iterations, out byte[] salt, out byte[] key)
    {
        iterations = 0;
        salt = [];
        key = [];

        var parts = hashedPassword.Split('.', 3);
        if (parts.Length != 3)
        {
            return false;
        }

        if (!int.TryParse(parts[0], out iterations))
        {
            return false;
        }

        try
        {
            salt = Convert.FromBase64String(parts[1]);
            key = Convert.FromBase64String(parts[2]);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
