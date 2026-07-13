using Trust.Api.Domain.Admin;

namespace Trust.Api.Application.Repositories;

/// <summary>
/// Data access for <see cref="Volunteer"/> records. Only the operation
/// needed to promote an approved <see cref="VolunteerRequest"/> is included
/// here; full roster management belongs to a future Volunteers module.
/// </summary>
public interface IVolunteerRepository
{
    /// <summary>
    /// Inserts a new volunteer roster entry.
    /// </summary>
    /// <param name="volunteer">The volunteer to insert.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task AddAsync(Volunteer volunteer, CancellationToken cancellationToken);
}
