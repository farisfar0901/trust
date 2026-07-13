namespace Trust.Api.Application.Common;

/// <summary>
/// Hashes and verifies admin passwords. Implementations must never store or
/// return the original plain-text password.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Creates a salted hash of <paramref name="password"/> that is safe to
    /// store in the <c>admin_users.password_hash</c> column.
    /// </summary>
    /// <param name="password">The plain-text password supplied by the admin.</param>
    /// <returns>A hash string that can later be checked with <see cref="VerifyPassword"/>.</returns>
    string HashPassword(string password);

    /// <summary>
    /// Checks whether <paramref name="password"/> matches the previously
    /// hashed value.
    /// </summary>
    /// <param name="password">The plain-text password to check.</param>
    /// <param name="hashedPassword">The stored hash, produced by <see cref="HashPassword"/>.</param>
    /// <returns><see langword="true"/> if the password is correct.</returns>
    bool VerifyPassword(string password, string hashedPassword);
}
