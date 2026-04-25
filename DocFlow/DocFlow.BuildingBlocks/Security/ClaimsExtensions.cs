using System.Security.Claims;

namespace DocFlow.BuildingBlocks.Security;

public static class ClaimsExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue("sub") ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);
        return value is null ? Guid.Empty : Guid.Parse(value);
    }

    public static Guid GetTenantId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue("tenant_id");
        return value is null ? Guid.Empty : Guid.Parse(value);
    }
}
