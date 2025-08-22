using System.Security.Claims;

namespace Practice01.Application.Common.User;

public static class UserInfoExtension
{
    public static Guid? GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        var claim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null)
        {
            return null;
        }
        
        if (Guid.TryParse(claim.Value, out var userId))
        {
            return userId;
        }
        
        return null;
    }
    
    public static string GetUserName(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
    }
    
    public static string GetUserEmail(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
    }
    
    public static List<Claim> GetUserRole(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindAll(ClaimTypes.Role).ToList();
    }
    
    public static bool IsInRole(this ClaimsPrincipal user, string role)
    {
        return user.IsInRole(role);
    }
}