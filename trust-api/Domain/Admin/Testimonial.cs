namespace Trust.Api.Domain.Admin;

public sealed class Testimonial
{
    public long Id { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Quote { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public short? Rating { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPublished { get; set; } = true;
    public long? CreatedByAdminId { get; set; }
    public long? UpdatedByAdminId { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public AdminUser? CreatedByAdmin { get; set; }
    public AdminUser? UpdatedByAdmin { get; set; }
}
