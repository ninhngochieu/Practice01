using Practice01.Application.User.Command.Login;

namespace Practice01.Application.Common.Token;

public interface IJwtTokenService
{
    Task<TokenDto> GenerateToken(Domain.Entities.Users.User user);
}