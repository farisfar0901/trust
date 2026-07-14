using Trust.Api.Application.Common;
using Trust.Api.Application.Repositories;
using Trust.Api.Application.Services.Members.DTOs;
using Trust.Api.Domain.Admin;

namespace Trust.Api.Application.Services.Members;

/// <summary>
/// Default <see cref="IMemberService"/> implementation.
/// </summary>
public sealed class MemberService : IMemberService
{
    private readonly IMemberRepository _memberRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    /// <summary>
    /// Creates the service.
    /// </summary>
    public MemberService(IMemberRepository memberRepository, IDateTimeProvider dateTimeProvider)
    {
        _memberRepository = memberRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    /// <inheritdoc />
    public async Task<MemberResponse> CreateAsync(
        MemberCreateRequest request, long creatingAdminId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        await EnsureEmailIsAvailableAsync(request.Email, excludingId: null, cancellationToken);

        var member = new Member
        {
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            MembershipType = request.MembershipType,
            Status = request.Status,
            JoinedDate = request.JoinedDate,
            PhotoUrl = request.PhotoUrl,
            CreatedByAdminId = creatingAdminId,
            UpdatedByAdminId = creatingAdminId,
        };

        await _memberRepository.AddAsync(member, cancellationToken);
        return ToResponse(member);
    }

    /// <inheritdoc />
    public async Task<MemberResponse?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var member = await _memberRepository.GetByIdAsync(id, cancellationToken);
        return member is null ? null : ToResponse(member);
    }

    /// <inheritdoc />
    public async Task<PagedResult<MemberResponse>> GetPagedAsync(
        string? status,
        string? searchText,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _memberRepository.GetPagedAsync(
            status, searchText, pageNumber, pageSize, cancellationToken);

        return new PagedResult<MemberResponse>
        {
            Items = items.Select(ToResponse).ToList(),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
    }

    /// <inheritdoc />
    public async Task<MemberResponse?> UpdateAsync(
        long id, MemberUpdateRequest request, long updatingAdminId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var member = await _memberRepository.GetByIdAsync(id, cancellationToken);
        if (member is null)
        {
            return null;
        }

        await EnsureEmailIsAvailableAsync(request.Email, excludingId: id, cancellationToken);

        member.FullName = request.FullName;
        member.Email = request.Email;
        member.Phone = request.Phone;
        member.Address = request.Address;
        member.MembershipType = request.MembershipType;
        member.Status = request.Status;
        member.JoinedDate = request.JoinedDate;
        member.PhotoUrl = request.PhotoUrl;
        member.UpdatedByAdminId = updatingAdminId;

        await _memberRepository.UpdateAsync(member, cancellationToken);
        return ToResponse(member);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        var member = await _memberRepository.GetByIdAsync(id, cancellationToken);
        if (member is null)
        {
            return false;
        }

        member.DeletedAt = _dateTimeProvider.UtcNow;
        await _memberRepository.UpdateAsync(member, cancellationToken);
        return true;
    }

    /// <summary>
    /// Verifies that no other member already uses the given email address.
    /// </summary>
    /// <exception cref="ArgumentException">Another member already uses this email.</exception>
    private async Task EnsureEmailIsAvailableAsync(string email, long? excludingId, CancellationToken cancellationToken)
    {
        var isTaken = await _memberRepository.ExistsWithEmailAsync(email, excludingId, cancellationToken);
        if (isTaken)
        {
            throw new ArgumentException($"A member with email '{email}' already exists.", nameof(email));
        }
    }

    /// <summary>
    /// Maps a <see cref="Member"/> entity to its response DTO.
    /// </summary>
    private static MemberResponse ToResponse(Member member) => new()
    {
        Id = member.Id,
        FullName = member.FullName,
        Email = member.Email,
        Phone = member.Phone,
        Address = member.Address,
        MembershipType = member.MembershipType,
        Status = member.Status,
        JoinedDate = member.JoinedDate,
        PhotoUrl = member.PhotoUrl,
        CreatedAt = member.CreatedAt,
        UpdatedAt = member.UpdatedAt,
    };
}
