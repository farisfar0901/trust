namespace Trust.Api.Domain.Admin;

public sealed class Volunteer
{
    public long Id { get; set; }
    public long? VolunteerRequestId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? AreaOfWork { get; set; }
    public string Status { get; set; } = "Active";
    public DateOnly JoinedDate { get; set; }
    public long? CreatedByAdminId { get; set; }
    public long? UpdatedByAdminId { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public VolunteerRequest? VolunteerRequest { get; set; }
    public AdminUser? CreatedByAdmin { get; set; }
    public AdminUser? UpdatedByAdmin { get; set; }
}
