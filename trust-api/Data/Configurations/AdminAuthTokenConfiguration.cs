using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trust.Api.Domain.Admin;

namespace Trust.Api.Data.Configurations;

public sealed class AdminAuthTokenConfiguration : IEntityTypeConfiguration<AdminAuthToken>
{
    public void Configure(EntityTypeBuilder<AdminAuthToken> builder)
    {
        builder.ToTable("admin_auth_tokens");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.AdminId).HasColumnName("admin_id").IsRequired();
        builder.Property(x => x.TokenType).HasColumnName("token_type").HasMaxLength(20).IsRequired();
        builder.Property(x => x.TokenHash).HasColumnName("token_hash").HasMaxLength(255).IsRequired();
        builder.Property(x => x.ExpiresAt).HasColumnName("expires_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.UsedAt).HasColumnName("used_at").HasColumnType("timestamptz");
        builder.Property(x => x.RevokedAt).HasColumnName("revoked_at").HasColumnType("timestamptz");
        builder.Property(x => x.CreatedIp).HasColumnName("created_ip").HasColumnType("inet");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz")
            .IsRequired().HasDefaultValueSql("now()").ValueGeneratedOnAdd();

        builder.ToTable(t => t.HasCheckConstraint("chk_admin_auth_tokens_type", "token_type IN ('RefreshToken', 'PasswordReset')"));

        builder.HasIndex(x => x.TokenHash).HasDatabaseName("uq_admin_auth_tokens_token_hash").IsUnique();
        builder.HasIndex(x => x.AdminId).HasDatabaseName("ix_admin_auth_tokens_admin_id");
        builder.HasIndex(x => x.ExpiresAt).HasDatabaseName("ix_admin_auth_tokens_expires_at");

        // Only relationship in the schema that cascades: a token has no meaning without its account.
        builder.HasOne(x => x.Admin)
            .WithMany(x => x.AuthTokens)
            .HasForeignKey(x => x.AdminId)
            .HasConstraintName("fk_admin_auth_tokens_admin")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
