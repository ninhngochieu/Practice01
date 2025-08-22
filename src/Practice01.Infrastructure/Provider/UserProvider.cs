using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Practice01.Application.Common.User;

namespace Practice01.Infrastructure.Provider;

public class UserProvider : IUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public ClaimsPrincipal? User => _httpContextAccessor?.HttpContext?.User ?? null;
}