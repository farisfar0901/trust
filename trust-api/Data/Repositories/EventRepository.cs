using Microsoft.EntityFrameworkCore;
using Trust.Api.Application.Common;
using Trust.Api.Application.Repositories;
using Trust.Api.Domain.Admin;

namespace Trust.Api.Data.Repositories;

/// <summary>
/// Entity Framework Core implementation of <see cref="IEventRepository"/>.
/// </summary>
public sealed class EventRepository : IEventRepository
{
    private readonly TrustDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    /// <summary>
    /// Creates the repository.
    /// </summary>
    /// <param name="dbContext">The database context used for all queries and writes.</param>
    /// <param name="dateTimeProvider">Supplies the current time for upcoming/completed filtering.</param>
    public EventRepository(TrustDbContext dbContext, IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
    }

    /// <inheritdoc />
    public async Task<Event?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        return await _dbContext.AdminEvents
            .FirstOrDefaultAsync(eventItem => eventItem.Id == id && eventItem.DeletedAt == null, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Event?> GetBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        return await _dbContext.AdminEvents
            .FirstOrDefaultAsync(eventItem => eventItem.Slug == slug && eventItem.DeletedAt == null, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<(IReadOnlyList<Event> Items, int TotalCount)> GetPagedAsync(
        bool? isPublished,
        bool? isUpcoming,
        string? searchTitle,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.AdminEvents
            .Where(eventItem => eventItem.DeletedAt == null);

        if (isPublished.HasValue)
        {
            query = query.Where(eventItem => eventItem.IsPublished == isPublished.Value);
        }

        if (isUpcoming.HasValue)
        {
            var now = _dateTimeProvider.UtcNow;
            query = isUpcoming.Value
                ? query.Where(eventItem => eventItem.EventDate >= now)
                : query.Where(eventItem => eventItem.EventDate < now);
        }

        if (!string.IsNullOrWhiteSpace(searchTitle))
        {
            query = query.Where(eventItem => EF.Functions.ILike(eventItem.Title, $"%{searchTitle}%"));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(eventItem => eventItem.EventDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <inheritdoc />
    public async Task AddAsync(Event eventItem, CancellationToken cancellationToken)
    {
        _dbContext.AdminEvents.Add(eventItem);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Event eventItem, CancellationToken cancellationToken)
    {
        _dbContext.AdminEvents.Update(eventItem);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
