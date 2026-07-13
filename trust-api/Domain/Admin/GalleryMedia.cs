namespace Trust.Api.Domain.Admin;

public sealed class GalleryMedia
{
    public long Id { get; set; }
    public long? EventId { get; set; }
    public string? Title { get; set; }
    public string? Caption { get; set; }
    public string MediaUrl { get; set; } = string.Empty;
    public string MediaType { get; set; } = "Photo";
    public string? MimeType { get; set; }
    public long? FileSizeBytes { get; set; }
    public short Year { get; set; }
    public DateOnly? TakenAt { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPublished { get; set; } = true;
    public long? CreatedByAdminId { get; set; }
    public long? UpdatedByAdminId { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public Event? Event { get; set; }
    public AdminUser? CreatedByAdmin { get; set; }
    public AdminUser? UpdatedByAdmin { get; set; }
}
