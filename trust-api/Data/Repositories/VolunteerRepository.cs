using Trust.Api.Application.Repositories;
using Trust.Api.Domain.Admin;

namespace Trust.Api.Data.Repositories;

/// <summary>
/// Entity Framework Core implementation of <see cref="IVolunteerRepository"/>.
/// </summary>
public sealed class VolunteerRepository : IVolunteerRepository
{
    private readonly TrustDbContext _dbContext;

    /// <summary>
    /// Creates the repository.
    /// </summary>
    /// <param name="dbContext">The database context used for all queries and writes.</param>
    public VolunteerRepository(TrustDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task AddAsync(Volunteer volunteer, CancellationToken cancellationToken)
    {
        _dbContext.Volunteers.Add(volunteer);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
