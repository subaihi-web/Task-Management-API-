using System.Security.Claims;

namespace TaskManagement.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static long GetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? user.FindFirstValue("UserId");

        if (!long.TryParse(claim, out var userId))
        {
            throw new UnauthorizedAccessException("UserId claim is missing or invalid.");
        }

        return userId;
    }
}
