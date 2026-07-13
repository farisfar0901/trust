using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trust.Api.Domain.Admin;

namespace Trust.Api.Data.Configurations;

public sealed class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("events");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Slug).HasColumnName("slug").HasMaxLength(220).IsRequired();
        builder.Property(x => x.Category).HasColumnName("category").HasMaxLength(100);
        builder.Property(x => x.Description).HasColumnName("description");
        builder.Property(x => x.Location).HasColumnName("location").HasMaxLength(255);
        builder.Property(x => x.EventDate).HasColumnName("event_date").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.IsPublished).HasColumnName("is_published").IsRequired().HasDefaultValue(true);
        builder.Property(x => x.CoverImageUrl).HasColumnName("cover_image_url").HasMaxLength(500);
        builder.Property(x => x.CreatedByAdminId).HasColumnName("created_by_admin_id");
        builder.Property(x => x.UpdatedByAdminId).HasColumnName("updated_by_admin_id");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamptz");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz")
            .IsRequired().HasDefaultValueSql("now()").ValueGeneratedOnAdd();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamptz")
            .IsRequired().HasDefaultValueSql("now()").ValueGeneratedOnAdd();

        // No stored "is_upcoming" column by design — derive event_date >= now()
        // at query time instead of persisting a value that can go stale.

        builder.HasIndex(x => x.Slug).HasDatabaseName("ux_events_slug").IsUnique().HasFilter("deleted_at IS NULL");
        builder.HasIndex(x => x.EventDate).HasDatabaseName("ix_events_event_date").HasFilter("deleted_at IS NULL");
        // Covers the actual admin/public query shape: published events sorted by date.
        builder.HasIndex(x => new { x.IsPublished, x.EventDate })
            .HasDatabaseName("ix_events_published_date").HasFilter("deleted_at IS NULL");

        builder.HasOne(x => x.CreatedByAdmin)
            .WithMany()
            .HasForeignKey(x => x.CreatedByAdminId)
            .HasConstraintName("fk_events_created_by")
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.UpdatedByAdmin)
            .WithMany()
            .HasForeignKey(x => x.UpdatedByAdminId)
            .HasConstraintName("fk_events_updated_by")
            .OnDelete(DeleteBehavior.SetNull);
    }
}
