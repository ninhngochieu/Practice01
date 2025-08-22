using System.Security.Claims;

namespace Practice01.Application.Common.User;

public interface IUserProvider
{
    ClaimsPrincipal? User { get; }
}