using MediatR;

namespace Practice01.Application.User.Command.Login;

public class UserLoginCommand : IRequest<TokenDto>
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}