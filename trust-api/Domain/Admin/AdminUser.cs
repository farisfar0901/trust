using System.Net;

namespace Trust.Api.Domain.Admin;

public sealed class AdminUser
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Editor";
    public bool IsActive { get; set; } = true;
    public bool MfaEnabled { get; set; }
    public string? MfaSecret { get; set; }
    public short FailedLoginAttempts { get; set; }
    public DateTimeOffset? LockedUntil { get; set; }
    public DateTimeOffset? PasswordChangedAt { get; set; }
    public DateTimeOffset? LastLoginAt { get; set; }
    public IPAddress? LastLoginIp { get; set; }
    public long? CreatedByAdminId { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public AdminUser? CreatedByAdmin { get; set; }
    public ICollection<AdminUser> ProvisionedAdmins { get; set; } = new List<AdminUser>();
    public ICollection<AdminAuthToken> AuthTokens { get; set; } = new List<AdminAuthToken>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}
