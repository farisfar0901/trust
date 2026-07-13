using Microsoft.EntityFrameworkCore;
using Trust.Api.Application.Repositories;
using Trust.Api.Domain.Admin;

namespace Trust.Api.Data.Repositories;

/// <summary>
/// Entity Framework Core implementation of <see cref="IGalleryMediaRepository"/>.
/// </summary>
public sealed class GalleryMediaRepository : IGalleryMediaRepository
{
    private readonly TrustDbContext _dbContext;

    /// <summary>
    /// Creates the repository.
    /// </summary>
    /// <param name="dbContext">The database context used for all queries and writes.</param>
    public GalleryMediaRepository(TrustDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<GalleryMedia?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        return await _dbContext.GalleryMedia
            .FirstOrDefaultAsync(media => media.Id == id && media.DeletedAt == null, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<(IReadOnlyList<GalleryMedia> Items, int TotalCount)> GetPagedAsync(
        long? eventId,
        bool? isPublished,
        string? searchText,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.GalleryMedia
            .Where(media => media.DeletedAt == null);

        if (eventId.HasValue)
        {
            query = query.Where(media => media.EventId == eventId.Value);
        }

        if (isPublished.HasValue)
        {
            query = query.Where(media => media.IsPublished == isPublished.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var pattern = $"%{searchText}%";
            query = query.Where(media =>
                EF.Functions.ILike(media.Title!, pattern) || EF.Functions.ILike(media.Caption!, pattern));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(media => media.DisplayOrder)
            .ThenByDescending(media => media.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <inheritdoc />
    public async Task AddAsync(GalleryMedia galleryMedia, CancellationToken cancellationToken)
    {
        _dbContext.GalleryMedia.Add(galleryMedia);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(GalleryMedia galleryMedia, CancellationToken cancellationToken)
    {
        _dbContext.GalleryMedia.Update(galleryMedia);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
