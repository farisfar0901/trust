using Trust.Api.Domain.Admin;

namespace Trust.Api.Application.Repositories;

/// <summary>
/// Data access for <see cref="AdminUser"/> records. Contains only reads and
/// writes — no business rules about login attempts, lockouts, or roles.
/// </summary>
public interface IAdminUserRepository
{
    /// <summary>
    /// Finds an active (non-deleted) admin by email address.
    /// </summary>
    /// <param name="email">The email address to search for.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The matching admin, or <see langword="null"/> if none exists.</returns>
    Task<AdminUser?> GetByEmailAsync(string email, CancellationToken cancellationToken);

    /// <summary>
    /// Finds an admin by id, including soft-deleted rows so callers can
    /// decide for themselves how to handle a deleted account.
    /// </summary>
    /// <param name="id">The admin's id.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The matching admin, or <see langword="null"/> if none exists.</returns>
    Task<AdminUser?> GetByIdAsync(long id, CancellationToken cancellationToken);

    /// <summary>
    /// Persists changes made to an existing <see cref="AdminUser"/>.
    /// </summary>
    /// <param name="adminUser">The admin, with its properties already updated by the caller.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task UpdateAsync(AdminUser adminUser, CancellationToken cancellationToken);
}
