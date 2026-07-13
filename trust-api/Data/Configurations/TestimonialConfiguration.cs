using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trust.Api.Domain.Admin;

namespace Trust.Api.Data.Configurations;

public sealed class TestimonialConfiguration : IEntityTypeConfiguration<Testimonial>
{
    public void Configure(EntityTypeBuilder<Testimonial> builder)
    {
        builder.ToTable("testimonials");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.AuthorName).HasColumnName("author_name").HasMaxLength(150).IsRequired();
        builder.Property(x => x.Role).HasColumnName("role").HasMaxLength(30).IsRequired();
        builder.Property(x => x.Quote).HasColumnName("quote").IsRequired();
        builder.Property(x => x.PhotoUrl).HasColumnName("photo_url").HasMaxLength(500);
        builder.Property(x => x.Rating).HasColumnName("rating");
        builder.Property(x => x.DisplayOrder).HasColumnName("display_order").IsRequired().HasDefaultValue(0);
        builder.Property(x => x.IsPublished).HasColumnName("is_published").IsRequired().HasDefaultValue(true);
        builder.Property(x => x.CreatedByAdminId).HasColumnName("created_by_admin_id");
        builder.Property(x => x.UpdatedByAdminId).HasColumnName("updated_by_admin_id");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamptz");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz")
            .IsRequired().HasDefaultValueSql("now()").ValueGeneratedOnAdd();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamptz")
            .IsRequired().HasDefaultValueSql("now()").ValueGeneratedOnAdd();

        builder.ToTable(t => t.HasCheckConstraint(
            "chk_testimonials_role", "role IN ('Donor', 'Volunteer', 'Beneficiary', 'Other')"));
        builder.ToTable(t => t.HasCheckConstraint(
            "chk_testimonials_rating", "rating IS NULL OR rating BETWEEN 1 AND 5"));

        builder.HasIndex(x => new { x.IsPublished, x.DisplayOrder })
            .HasDatabaseName("ix_testimonials_published_order").HasFilter("deleted_at IS NULL");

        builder.HasOne(x => x.CreatedByAdmin)
            .WithMany()
            .HasForeignKey(x => x.CreatedByAdminId)
            .HasConstraintName("fk_testimonials_created_by")
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.UpdatedByAdmin)
            .WithMany()
            .HasForeignKey(x => x.UpdatedByAdminId)
            .HasConstraintName("fk_testimonials_updated_by")
            .OnDelete(DeleteBehavior.SetNull);
    }
}
