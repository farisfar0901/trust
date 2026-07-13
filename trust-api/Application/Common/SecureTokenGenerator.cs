using System.Security.Cryptography;
using System.Text;

namespace Trust.Api.Application.Common;

/// <summary>
/// Default <see cref="ISecureTokenGenerator"/> implementation using the
/// .NET cryptographic random number generator and SHA-256 hashing.
/// </summary>
public sealed class SecureTokenGenerator : ISecureTokenGenerator
{
    /// <summary>
    /// Number of random bytes in each generated token. 32 bytes (256 bits)
    /// gives a negligible chance of collision.
    /// </summary>
    private const int TokenSizeInBytes = 32;

    /// <inheritdoc />
    public string GenerateToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(TokenSizeInBytes);
        return Convert.ToBase64String(randomBytes);
    }

    /// <inheritdoc />
    public string Hash(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var tokenBytes = Encoding.UTF8.GetBytes(token);
        var hashBytes = SHA256.HashData(tokenBytes);
        return Convert.ToBase64String(hashBytes);
    }
}
