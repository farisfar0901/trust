using Microsoft.EntityFrameworkCore;
using Trust.Api.Application.Repositories;
using Trust.Api.Domain.Admin;

namespace Trust.Api.Data.Repositories;

/// <summary>
/// Entity Framework Core implementation of <see cref="IAdminAuthTokenRepository"/>.
/// </summary>
public sealed class AdminAuthTokenRepository : IAdminAuthTokenRepository
{
    private readonly TrustDbContext _dbContext;

    /// <summary>
    /// Creates the repository.
    /// </summary>
    /// <param name="dbContext">The database context used for all queries and writes.</param>
    public AdminAuthTokenRepository(TrustDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<AdminAuthToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken)
    {
        return await _dbContext.AdminAuthTokens
            .FirstOrDefaultAsync(token => token.TokenHash == tokenHash, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(AdminAuthToken authToken, CancellationToken cancellationToken)
    {
        _dbContext.AdminAuthTokens.Add(authToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(AdminAuthToken authToken, CancellationToken cancellationToken)
    {
        _dbContext.AdminAuthTokens.Update(authToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
