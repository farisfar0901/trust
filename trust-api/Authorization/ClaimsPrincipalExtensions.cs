using System.Security.Claims;

namespace Trust.Api.Authorization;

/// <summary>
/// Convenience accessors for reading admin-specific claims off the current
/// <see cref="ClaimsPrincipal"/>, once it has been authenticated by the JWT
/// bearer middleware.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Reads the id of the currently authenticated admin from the JWT
    /// <c>sub</c> claim.
    /// </summary>
    /// <param name="user">The authenticated user's claims principal.</param>
    /// <returns>The admin's id.</returns>
    /// <exception cref="InvalidOperationException">
    /// The user has no valid <c>sub</c> claim. This should never happen
    /// behind an <c>[Authorize]</c>-protected endpoint.
    /// </exception>
    public static long GetAdminId(this ClaimsPrincipal user)
    {
        var subjectClaim = user.FindFirst("sub");
        if (subjectClaim is null || !long.TryParse(subjectClaim.Value, out var adminId))
        {
            throw new InvalidOperationException("The current user does not have a valid 'sub' claim.");
        }

        return adminId;
    }

    /// <summary>
    /// Reads the currently authenticated admin's email from the JWT
    /// <c>email</c> claim.
    /// </summary>
    /// <param name="user">The authenticated user's claims principal.</param>
    /// <returns>The admin's email, or <see langword="null"/> if the claim is missing.</returns>
    public static string? GetAdminEmail(this ClaimsPrincipal user)
        => user.FindFirst("email")?.Value;

    /// <summary>
    /// Reads the currently authenticated admin's role from the JWT
    /// <c>role</c> claim.
    /// </summary>
    /// <param name="user">The authenticated user's claims principal.</param>
    /// <returns>The admin's role, or <see langword="null"/> if the claim is missing.</returns>
    public static string? GetAdminRole(this ClaimsPrincipal user)
        => user.FindFirst("role")?.Value;
}
