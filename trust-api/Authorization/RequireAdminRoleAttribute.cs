using Microsoft.AspNetCore.Authorization;

namespace Trust.Api.Authorization;

/// <summary>
/// Restricts an endpoint to authenticated admins whose JWT <c>role</c> claim
/// matches one of the given roles (for example <c>SuperAdmin</c>,
/// <c>Admin</c>, or <c>Editor</c>). A thin wrapper over
/// <see cref="AuthorizeAttribute"/> so controllers can write
/// <c>[RequireAdminRole("SuperAdmin", "Admin")]</c> instead of building the
/// comma-separated <see cref="AuthorizeAttribute.Roles"/> string by hand.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class RequireAdminRoleAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Restricts the endpoint to admins with one of the given roles.
    /// </summary>
    /// <param name="roles">The admin roles allowed to access the endpoint.</param>
    public RequireAdminRoleAttribute(params string[] roles)
    {
        Roles = string.Join(',', roles);
    }
}
