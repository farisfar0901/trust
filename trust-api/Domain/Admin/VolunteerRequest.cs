using System.Net;

namespace Trust.Api.Domain.Admin;

public sealed class VolunteerRequest
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? PreferredArea { get; set; }
    public string? Message { get; set; }
    public string Status { get; set; } = "Pending";
    public string? ReviewNote { get; set; }
    public long? ReviewedByAdminId { get; set; }
    public DateTimeOffset? ReviewedAt { get; set; }
    public IPAddress? IpAddress { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public AdminUser? ReviewedByAdmin { get; set; }
    public Volunteer? PromotedVolunteer { get; set; }
}
