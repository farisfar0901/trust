namespace Trust.Api.Application.Common;

/// <summary>
/// Generates opaque, cryptographically random tokens (used for refresh and
/// password-reset tokens) and hashes them for safe storage.
/// </summary>
public interface ISecureTokenGenerator
{
    /// <summary>
    /// Creates a new random token string. The raw value is only ever shown
    /// to the caller once; only its hash is persisted.
    /// </summary>
    string GenerateToken();

    /// <summary>
    /// Computes the hash of <paramref name="token"/> that should be stored
    /// in the database instead of the raw token.
    /// </summary>
    string Hash(string token);
}
