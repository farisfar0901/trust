using Trust.Api.Domain.Admin;

namespace Trust.Api.Application.Repositories;

/// <summary>
/// Data access for <see cref="AdminAuthToken"/> records (refresh and
/// password-reset tokens). Contains only reads and writes — deciding
/// whether a token is still valid is business logic for the service layer.
/// </summary>
public interface IAdminAuthTokenRepository
{
    /// <summary>
    /// Finds a token by the hash of its raw value.
    /// </summary>
    /// <param name="tokenHash">The hash produced by <c>ISecureTokenGenerator.Hash</c>.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The matching token row, or <see langword="null"/> if none exists.</returns>
    Task<AdminAuthToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken);

    /// <summary>
    /// Inserts a new token row.
    /// </summary>
    /// <param name="authToken">The token to insert.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task AddAsync(AdminAuthToken authToken, CancellationToken cancellationToken);

    /// <summary>
    /// Persists changes made to an existing token row (for example marking
    /// it used or revoked).
    /// </summary>
    /// <param name="authToken">The token, with its properties already updated by the caller.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task UpdateAsync(AdminAuthToken authToken, CancellationToken cancellationToken);
}
