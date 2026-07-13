using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Trust.Api.Domain.Admin;

namespace Trust.Api.Application.Common;

/// <summary>
/// Creates HS256-signed JSON Web Tokens using only the .NET base class
/// library, so no extra NuGet package is required. The token format follows
/// RFC 7519, so it can be validated later by standard JWT bearer middleware
/// configured with the same signing key.
/// </summary>
public sealed class HmacAccessTokenGenerator : IAccessTokenGenerator
{
    private const int AccessTokenLifetimeMinutes = 15;
    private readonly IConfiguration _configuration;
    private readonly IDateTimeProvider _dateTimeProvider;

    /// <summary>
    /// Creates the generator.
    /// </summary>
    /// <param name="configuration">Used to read the <c>Jwt:SigningKey</c> and <c>Jwt:Issuer</c> settings.</param>
    /// <param name="dateTimeProvider">Supplies the current time for the token's issued-at and expiry claims.</param>
    public HmacAccessTokenGenerator(IConfiguration configuration, IDateTimeProvider dateTimeProvider)
    {
        _configuration = configuration;
        _dateTimeProvider = dateTimeProvider;
    }

    /// <inheritdoc />
    public AccessToken GenerateAccessToken(AdminUser adminUser)
    {
        ArgumentNullException.ThrowIfNull(adminUser);

        var issuedAt = _dateTimeProvider.UtcNow;
        var expiresAt = issuedAt.AddMinutes(AccessTokenLifetimeMinutes);

        var header = new JwtHeader();
        var payload = new JwtPayload
        {
            Subject = adminUser.Id,
            Email = adminUser.Email,
            Role = adminUser.Role,
            IssuedAtUnixSeconds = issuedAt.ToUnixTimeSeconds(),
            ExpiresAtUnixSeconds = expiresAt.ToUnixTimeSeconds(),
            TokenId = Guid.NewGuid().ToString("N"),
            Issuer = _configuration["Jwt:Issuer"] ?? "TrustAdminApi",
        };

        var token = SignToken(header, payload);
        return new AccessToken(token, expiresAt);
    }

    /// <summary>
    /// Builds the three-part <c>header.payload.signature</c> JWT string.
    /// </summary>
    private string SignToken(JwtHeader header, JwtPayload payload)
    {
        var encodedHeader = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(header));
        var encodedPayload = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(payload));
        var signingInput = $"{encodedHeader}.{encodedPayload}";

        var signature = ComputeSignature(signingInput);
        return $"{signingInput}.{signature}";
    }

    /// <summary>
    /// Computes the HMAC-SHA256 signature over the given signing input using
    /// the configured signing key.
    /// </summary>
    private string ComputeSignature(string signingInput)
    {
        var signingKey = _configuration["Jwt:SigningKey"];
        if (string.IsNullOrWhiteSpace(signingKey))
        {
            throw new InvalidOperationException(
                "Jwt:SigningKey is not configured. Set it via an environment variable or " +
                "user-secrets before issuing access tokens — never commit a real signing key.");
        }

        var keyBytes = Encoding.UTF8.GetBytes(signingKey);
        var inputBytes = Encoding.UTF8.GetBytes(signingInput);
        var signatureBytes = HMACSHA256.HashData(keyBytes, inputBytes);

        return Base64UrlEncode(signatureBytes);
    }

    /// <summary>
    /// Base64url-encodes bytes as required by the JWT specification
    /// (standard Base64 with URL-safe characters and no padding).
    /// </summary>
    private static string Base64UrlEncode(byte[] bytes)
        => Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');

    private sealed class JwtHeader
    {
        [JsonPropertyName("alg")]
        public string Algorithm { get; init; } = "HS256";

        [JsonPropertyName("typ")]
        public string Type { get; init; } = "JWT";
    }

    private sealed class JwtPayload
    {
        [JsonPropertyName("sub")]
        public long Subject { get; init; }

        [JsonPropertyName("email")]
        public string Email { get; init; } = string.Empty;

        [JsonPropertyName("role")]
        public string Role { get; init; } = string.Empty;

        [JsonPropertyName("iat")]
        public long IssuedAtUnixSeconds { get; init; }

        [JsonPropertyName("exp")]
        public long ExpiresAtUnixSeconds { get; init; }

        [JsonPropertyName("jti")]
        public string TokenId { get; init; } = string.Empty;

        [JsonPropertyName("iss")]
        public string Issuer { get; init; } = string.Empty;
    }
}
