using Microsoft.EntityFrameworkCore;
using Trust.Api.Application.Repositories;
using Trust.Api.Domain.Admin;

namespace Trust.Api.Data.Repositories;

/// <summary>
/// Entity Framework Core implementation of <see cref="IAdminUserRepository"/>.
/// </summary>
public sealed class AdminUserRepository : IAdminUserRepository
{
    private readonly TrustDbContext _dbContext;

    /// <summary>
    /// Creates the repository.
    /// </summary>
    /// <param name="dbContext">The database context used for all queries and writes.</param>
    public AdminUserRepository(TrustDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<AdminUser?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _dbContext.AdminUsers
            .FirstOrDefaultAsync(admin => admin.Email == email && admin.DeletedAt == null, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AdminUser?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        return await _dbContext.AdminUsers
            .FirstOrDefaultAsync(admin => admin.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(AdminUser adminUser, CancellationToken cancellationToken)
    {
        _dbContext.AdminUsers.Update(adminUser);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
