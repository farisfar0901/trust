using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trust.Api.Domain.Admin;

namespace Trust.Api.Data.Configurations;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.AdminId).HasColumnName("admin_id");
        builder.Property(x => x.Action).HasColumnName("action").HasMaxLength(20).IsRequired();
        builder.Property(x => x.EntityType).HasColumnName("entity_type").HasMaxLength(50).IsRequired();
        builder.Property(x => x.EntityId).HasColumnName("entity_id");
        builder.Property(x => x.Metadata).HasColumnName("metadata").HasColumnType("jsonb");
        builder.Property(x => x.IpAddress).HasColumnName("ip_address").HasColumnType("inet");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz")
            .IsRequired().HasDefaultValueSql("now()").ValueGeneratedOnAdd();

        builder.ToTable(t => t.HasCheckConstraint("chk_audit_logs_action",
            "action IN ('Create', 'Update', 'Delete', 'Publish', 'Unpublish', 'Approve', 'Reject', 'Login', 'LoginFailed')"));

        builder.HasIndex(x => x.AdminId).HasDatabaseName("ix_audit_logs_admin_id");
        builder.HasIndex(x => new { x.EntityType, x.EntityId }).HasDatabaseName("ix_audit_logs_entity");
        builder.HasIndex(x => x.CreatedAt).HasDatabaseName("ix_audit_logs_created_at");

        // (entity_type, entity_id) is an intentional polymorphic reference, not
        // a strict FK — a generic audit table can't target ten entity tables
        // with one foreign key.
        builder.HasOne(x => x.Admin)
            .WithMany(x => x.AuditLogs)
            .HasForeignKey(x => x.AdminId)
            .HasConstraintName("fk_audit_logs_admin")
            .OnDelete(DeleteBehavior.SetNull);
    }
}
