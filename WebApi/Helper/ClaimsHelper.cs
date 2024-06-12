using System.Security.Claims;

namespace WebApi.Helper;

public abstract class ClaimsHelper
{
    public static Guid GetUserId(ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        throw new Exception("User ID claim not found or invalid.");
    }
}