using Microsoft.EntityFrameworkCore;
using Trust.Api.Application.Repositories;
using Trust.Api.Domain.Admin;

namespace Trust.Api.Data.Repositories;

/// <summary>
/// Entity Framework Core implementation of <see cref="IVolunteerRequestRepository"/>.
/// </summary>
public sealed class VolunteerRequestRepository : IVolunteerRequestRepository
{
    private readonly TrustDbContext _dbContext;

    /// <summary>
    /// Creates the repository.
    /// </summary>
    /// <param name="dbContext">The database context used for all queries and writes.</param>
    public VolunteerRequestRepository(TrustDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<VolunteerRequest?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        return await _dbContext.VolunteerRequests
            .FirstOrDefaultAsync(request => request.Id == id && request.DeletedAt == null, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<(IReadOnlyList<VolunteerRequest> Items, int TotalCount)> GetPagedAsync(
        string? status, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = _dbContext.VolunteerRequests
            .Where(request => request.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(request => request.Status == status);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(request => request.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <inheritdoc />
    public async Task AddAsync(VolunteerRequest volunteerRequest, CancellationToken cancellationToken)
    {
        _dbContext.VolunteerRequests.Add(volunteerRequest);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(VolunteerRequest volunteerRequest, CancellationToken cancellationToken)
    {
        _dbContext.VolunteerRequests.Update(volunteerRequest);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
