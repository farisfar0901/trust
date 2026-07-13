namespace Trust.Api.Domain.Admin;

public sealed class Event
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public DateTimeOffset EventDate { get; set; }
    public bool IsPublished { get; set; } = true;
    public string? CoverImageUrl { get; set; }
    public long? CreatedByAdminId { get; set; }
    public long? UpdatedByAdminId { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public AdminUser? CreatedByAdmin { get; set; }
    public AdminUser? UpdatedByAdmin { get; set; }
    public ICollection<GalleryMedia> GalleryMedia { get; set; } = new List<GalleryMedia>();
}
