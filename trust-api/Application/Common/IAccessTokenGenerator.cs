using Trust.Api.Domain.Admin;

namespace Trust.Api.Application.Common;

/// <summary>
/// Issues short-lived access tokens for authenticated admin users. The
/// generated token is self-contained (a signed JWT); it is not persisted,
/// unlike refresh tokens which are stored in <c>admin_auth_tokens</c>.
/// </summary>
public interface IAccessTokenGenerator
{
    /// <summary>
    /// Creates a signed access token for <paramref name="adminUser"/>.
    /// </summary>
    /// <param name="adminUser">The admin the token is issued to.</param>
    /// <returns>The signed token string and the moment it expires.</returns>
    AccessToken GenerateAccessToken(AdminUser adminUser);
}

/// <summary>
/// A signed access token and the moment it stops being valid.
/// </summary>
/// <param name="Token">The signed JWT string.</param>
/// <param name="ExpiresAt">The instant the token expires.</param>
public sealed record AccessToken(string Token, DateTimeOffset ExpiresAt);
