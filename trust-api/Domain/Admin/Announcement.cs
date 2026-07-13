namespace Trust.Api.Domain.Admin;

public sealed class Announcement
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Priority { get; set; } = "Info";
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public long? CreatedByAdminId { get; set; }
    public long? UpdatedByAdminId { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public AdminUser? CreatedByAdmin { get; set; }
    public AdminUser? UpdatedByAdmin { get; set; }
}
