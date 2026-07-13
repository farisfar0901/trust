using System.Net;

namespace Trust.Api.Domain.Admin;

public sealed class AdminAuthToken
{
    public long Id { get; set; }
    public long AdminId { get; set; }
    public string TokenType { get; set; } = string.Empty;
    public string TokenHash { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? UsedAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public IPAddress? CreatedIp { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public AdminUser Admin { get; set; } = null!;
}
