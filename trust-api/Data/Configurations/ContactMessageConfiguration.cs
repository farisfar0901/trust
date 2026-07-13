using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trust.Api.Domain.Admin;

namespace Trust.Api.Data.Configurations;

public sealed class ContactMessageConfiguration : IEntityTypeConfiguration<ContactMessage>
{
    public void Configure(EntityTypeBuilder<ContactMessage> builder)
    {
        builder.ToTable("contact_messages");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.FullName).HasColumnName("full_name").HasMaxLength(150).IsRequired();
        builder.Property(x => x.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
        builder.Property(x => x.Subject).HasColumnName("subject").HasMaxLength(200);
        builder.Property(x => x.Message).HasColumnName("message").IsRequired();
        builder.Property(x => x.IsRead).HasColumnName("is_read").IsRequired().HasDefaultValue(false);
        builder.Property(x => x.IsSpam).HasColumnName("is_spam").IsRequired().HasDefaultValue(false);
        builder.Property(x => x.IpAddress).HasColumnName("ip_address").HasColumnType("inet");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamptz");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz")
            .IsRequired().HasDefaultValueSql("now()").ValueGeneratedOnAdd();

        builder.ToTable(t => t.HasCheckConstraint(
            "chk_contact_messages_email_format", "email ~* '^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$'"));

        // Covers "unread, newest first" — the actual admin list-view query.
        builder.HasIndex(x => new { x.IsRead, x.CreatedAt })
            .HasDatabaseName("ix_contact_messages_read_created").HasFilter("deleted_at IS NULL");
    }
}
