using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trust.Api.Domain.Admin;

namespace Trust.Api.Data.Configurations;

public sealed class AnnouncementConfiguration : IEntityTypeConfiguration<Announcement>
{
    public void Configure(EntityTypeBuilder<Announcement> builder)
    {
        builder.ToTable("announcements");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Message).HasColumnName("message").IsRequired();
        builder.Property(x => x.Priority).HasColumnName("priority").HasMaxLength(20).IsRequired().HasDefaultValue("Info");
        builder.Property(x => x.StartDate).HasColumnName("start_date").HasColumnType("date")
            .IsRequired().HasDefaultValueSql("CURRENT_DATE");
        builder.Property(x => x.EndDate).HasColumnName("end_date").HasColumnType("date");
        builder.Property(x => x.IsActive).HasColumnName("is_active").IsRequired().HasDefaultValue(true);
        builder.Property(x => x.CreatedByAdminId).HasColumnName("created_by_admin_id");
        builder.Property(x => x.UpdatedByAdminId).HasColumnName("updated_by_admin_id");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamptz");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz")
            .IsRequired().HasDefaultValueSql("now()").ValueGeneratedOnAdd();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamptz")
            .IsRequired().HasDefaultValueSql("now()").ValueGeneratedOnAdd();

        builder.ToTable(t => t.HasCheckConstraint("chk_announcements_priority", "priority IN ('Info', 'Urgent')"));
        builder.ToTable(t => t.HasCheckConstraint(
            "chk_announcements_date_range", "end_date IS NULL OR end_date >= start_date"));

        // Covers "currently active" lookups directly.
        builder.HasIndex(x => new { x.IsActive, x.StartDate, x.EndDate })
            .HasDatabaseName("ix_announcements_active_window").HasFilter("deleted_at IS NULL");

        builder.HasOne(x => x.CreatedByAdmin)
            .WithMany()
            .HasForeignKey(x => x.CreatedByAdminId)
            .HasConstraintName("fk_announcements_created_by")
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.UpdatedByAdmin)
            .WithMany()
            .HasForeignKey(x => x.UpdatedByAdminId)
            .HasConstraintName("fk_announcements_updated_by")
            .OnDelete(DeleteBehavior.SetNull);
    }
}
