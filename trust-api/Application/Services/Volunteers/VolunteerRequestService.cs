using System.Net;
using Trust.Api.Application.Common;
using Trust.Api.Application.Repositories;
using Trust.Api.Application.Services.Volunteers.DTOs;
using Trust.Api.Domain.Admin;

namespace Trust.Api.Application.Services.Volunteers;

/// <summary>
/// Default <see cref="IVolunteerRequestService"/> implementation.
/// </summary>
public sealed class VolunteerRequestService : IVolunteerRequestService
{
    private const string PendingStatus = "Pending";
    private const string ApprovedStatus = "Approved";
    private const string ActiveVolunteerStatus = "Active";

    private readonly IVolunteerRequestRepository _volunteerRequestRepository;
    private readonly IVolunteerRepository _volunteerRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    /// <summary>
    /// Creates the service.
    /// </summary>
    public VolunteerRequestService(
        IVolunteerRequestRepository volunteerRequestRepository,
        IVolunteerRepository volunteerRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _volunteerRequestRepository = volunteerRequestRepository;
        _volunteerRepository = volunteerRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    /// <inheritdoc />
    public async Task<VolunteerRequestResponse> SubmitAsync(
        VolunteerRequestCreateRequest request, string? ipAddress, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var volunteerRequest = new VolunteerRequest
        {
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            City = request.City,
            PreferredArea = request.PreferredArea,
            Message = request.Message,
            Status = PendingStatus,
            IpAddress = ParseIpAddress(ipAddress),
        };

        await _volunteerRequestRepository.AddAsync(volunteerRequest, cancellationToken);
        return ToResponse(volunteerRequest);
    }

    /// <inheritdoc />
    public async Task<VolunteerRequestResponse?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var volunteerRequest = await _volunteerRequestRepository.GetByIdAsync(id, cancellationToken);
        return volunteerRequest is null ? null : ToResponse(volunteerRequest);
    }

    /// <inheritdoc />
    public async Task<PagedResult<VolunteerRequestResponse>> GetPagedAsync(
        string? status, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _volunteerRequestRepository.GetPagedAsync(
            status, pageNumber, pageSize, cancellationToken);

        return new PagedResult<VolunteerRequestResponse>
        {
            Items = items.Select(ToResponse).ToList(),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
    }

    /// <inheritdoc />
    public async Task<VolunteerRequestResponse?> ReviewAsync(
        long id, VolunteerRequestReviewRequest request, long reviewingAdminId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var volunteerRequest = await _volunteerRequestRepository.GetByIdAsync(id, cancellationToken);
        if (volunteerRequest is null)
        {
            return null;
        }

        if (volunteerRequest.Status != PendingStatus)
        {
            throw new InvalidOperationException(
                $"Volunteer request {id} has already been reviewed and cannot be reviewed again.");
        }

        volunteerRequest.Status = request.Status;
        volunteerRequest.ReviewNote = request.ReviewNote;
        volunteerRequest.ReviewedByAdminId = reviewingAdminId;
        volunteerRequest.ReviewedAt = _dateTimeProvider.UtcNow;
        await _volunteerRequestRepository.UpdateAsync(volunteerRequest, cancellationToken);

        if (request.Status == ApprovedStatus)
        {
            await PromoteToVolunteerAsync(volunteerRequest, cancellationToken);
        }

        return ToResponse(volunteerRequest);
    }

    /// <summary>
    /// Creates a new active volunteer roster entry from an approved request.
    /// </summary>
    private async Task PromoteToVolunteerAsync(VolunteerRequest volunteerRequest, CancellationToken cancellationToken)
    {
        var volunteer = new Volunteer
        {
            VolunteerRequestId = volunteerRequest.Id,
            FullName = volunteerRequest.FullName,
            Email = volunteerRequest.Email,
            Phone = volunteerRequest.Phone,
            City = volunteerRequest.City,
            AreaOfWork = volunteerRequest.PreferredArea,
            Status = ActiveVolunteerStatus,
            JoinedDate = DateOnly.FromDateTime(_dateTimeProvider.UtcNow.UtcDateTime),
            CreatedByAdminId = volunteerRequest.ReviewedByAdminId,
        };

        await _volunteerRepository.AddAsync(volunteer, cancellationToken);
    }

    /// <summary>
    /// Maps a <see cref="VolunteerRequest"/> entity to its response DTO.
    /// </summary>
    private static VolunteerRequestResponse ToResponse(VolunteerRequest volunteerRequest) => new()
    {
        Id = volunteerRequest.Id,
        FullName = volunteerRequest.FullName,
        Email = volunteerRequest.Email,
        Phone = volunteerRequest.Phone,
        City = volunteerRequest.City,
        PreferredArea = volunteerRequest.PreferredArea,
        Message = volunteerRequest.Message,
        Status = volunteerRequest.Status,
        ReviewNote = volunteerRequest.ReviewNote,
        ReviewedByAdminId = volunteerRequest.ReviewedByAdminId,
        ReviewedAt = volunteerRequest.ReviewedAt,
        CreatedAt = volunteerRequest.CreatedAt,
    };

    /// <summary>
    /// Parses an IP address string, returning <see langword="null"/> instead
    /// of throwing if it is missing or not a valid address.
    /// </summary>
    private static IPAddress? ParseIpAddress(string? ipAddress)
        => IPAddress.TryParse(ipAddress, out var parsed) ? parsed : null;
}
