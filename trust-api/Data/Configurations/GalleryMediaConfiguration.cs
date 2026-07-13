using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trust.Api.Domain.Admin;

namespace Trust.Api.Data.Configurations;

public sealed class GalleryMediaConfiguration : IEntityTypeConfiguration<GalleryMedia>
{
    public void Configure(EntityTypeBuilder<GalleryMedia> builder)
    {
        builder.ToTable("gallery_media");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.EventId).HasColumnName("event_id");
        builder.Property(x => x.Title).HasColumnName("title").HasMaxLength(200);
        builder.Property(x => x.Caption).HasColumnName("caption");
        builder.Property(x => x.MediaUrl).HasColumnName("media_url").HasMaxLength(500).IsRequired();
        builder.Property(x => x.MediaType).HasColumnName("media_type").HasMaxLength(10).IsRequired().HasDefaultValue("Photo");
        builder.Property(x => x.MimeType).HasColumnName("mime_type").HasMaxLength(100);
        builder.Property(x => x.FileSizeBytes).HasColumnName("file_size_bytes");
        builder.Property(x => x.Year).HasColumnName("year").IsRequired();
        builder.Property(x => x.TakenAt).HasColumnName("taken_at").HasColumnType("date");
        builder.Property(x => x.DisplayOrder).HasColumnName("display_order").IsRequired().HasDefaultValue(0);
        builder.Property(x => x.IsPublished).HasColumnName("is_published").IsRequired().HasDefaultValue(true);
        builder.Property(x => x.CreatedByAdminId).HasColumnName("created_by_admin_id");
        builder.Property(x => x.UpdatedByAdminId).HasColumnName("updated_by_admin_id");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamptz");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz")
            .IsRequired().HasDefaultValueSql("now()").ValueGeneratedOnAdd();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamptz")
            .IsRequired().HasDefaultValueSql("now()").ValueGeneratedOnAdd();

        builder.ToTable(t => t.HasCheckConstraint("chk_gallery_media_type", "media_type IN ('Photo', 'Video')"));
        builder.ToTable(t => t.HasCheckConstraint("chk_gallery_media_year", "year >= 2000"));

        builder.HasIndex(x => x.EventId).HasDatabaseName("ix_gallery_media_event_id").HasFilter("deleted_at IS NULL");
        builder.HasIndex(x => x.Year).HasDatabaseName("ix_gallery_media_year").HasFilter("deleted_at IS NULL");
        builder.HasIndex(x => x.MediaType).HasDatabaseName("ix_gallery_media_media_type").HasFilter("deleted_at IS NULL");
        // Covers "published items, newest first" list views.
        builder.HasIndex(x => new { x.IsPublished, x.CreatedAt })
            .HasDatabaseName("ix_gallery_media_published_created").HasFilter("deleted_at IS NULL");
        // Covers ordered event-gallery reads.
        builder.HasIndex(x => new { x.EventId, x.DisplayOrder })
            .HasDatabaseName("ix_gallery_media_event_display_order").HasFilter("deleted_at IS NULL");

        // SET NULL, not CASCADE: deleting an event detaches its media rather
        // than destroying them.
        builder.HasOne(x => x.Event)
            .WithMany(x => x.GalleryMedia)
            .HasForeignKey(x => x.EventId)
            .HasConstraintName("fk_gallery_media_event")
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.CreatedByAdmin)
            .WithMany()
            .HasForeignKey(x => x.CreatedByAdminId)
            .HasConstraintName("fk_gallery_media_created_by")
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.UpdatedByAdmin)
            .WithMany()
            .HasForeignKey(x => x.UpdatedByAdminId)
            .HasConstraintName("fk_gallery_media_updated_by")
            .OnDelete(DeleteBehavior.SetNull);
    }
}
