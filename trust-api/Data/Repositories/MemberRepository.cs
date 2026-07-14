using Microsoft.EntityFrameworkCore;
using Trust.Api.Application.Repositories;
using Trust.Api.Domain.Admin;

namespace Trust.Api.Data.Repositories;

/// <summary>
/// Entity Framework Core implementation of <see cref="IMemberRepository"/>.
/// </summary>
public sealed class MemberRepository : IMemberRepository
{
    private readonly TrustDbContext _dbContext;

    /// <summary>
    /// Creates the repository.
    /// </summary>
    /// <param name="dbContext">The database context used for all queries and writes.</param>
    public MemberRepository(TrustDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<Member?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        return await _dbContext.Members
            .FirstOrDefaultAsync(member => member.Id == id && member.DeletedAt == null, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<(IReadOnlyList<Member> Items, int TotalCount)> GetPagedAsync(
        string? status,
        string? searchText,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Members
            .Where(member => member.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(member => member.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var pattern = $"%{searchText}%";
            query = query.Where(member => EF.Functions.ILike(member.FullName, pattern));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(member => member.FullName)
            .ThenByDescending(member => member.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsWithEmailAsync(string email, long? excludingId, CancellationToken cancellationToken)
    {
        var query = _dbContext.Members
            .Where(member => member.DeletedAt == null && EF.Functions.ILike(member.Email, email));

        if (excludingId.HasValue)
        {
            query = query.Where(member => member.Id != excludingId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(Member member, CancellationToken cancellationToken)
    {
        _dbContext.Members.Add(member);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Member member, CancellationToken cancellationToken)
    {
        _dbContext.Members.Update(member);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
