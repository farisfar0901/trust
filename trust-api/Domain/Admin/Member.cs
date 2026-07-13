namespace Trust.Api.Domain.Admin;

public sealed class Member
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string MembershipType { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public DateOnly JoinedDate { get; set; }
    public string? PhotoUrl { get; set; }
    public long? CreatedByAdminId { get; set; }
    public long? UpdatedByAdminId { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public AdminUser? CreatedByAdmin { get; set; }
    public AdminUser? UpdatedByAdmin { get; set; }
}
