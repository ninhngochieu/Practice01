using MediatR;
using Microsoft.AspNetCore.Identity;
using Practice01.Application.Common.Token;
using Practice01.Application.Common.Validation;

namespace Practice01.Application.User.Command.Login;

public class UserLoginCommandHandler : IRequestHandler<UserLoginCommand, TokenDto?>
{
    private readonly ErrorCollector _errorCollector;
    private readonly UserManager<Domain.Entities.User> _userManager;
    private readonly IJwtTokenService _jwtTokenService;

    public UserLoginCommandHandler(
        ErrorCollector errorCollector,
        UserManager<Domain.Entities.User> userManager,
        IJwtTokenService jwtTokenService)
    {
        _errorCollector = errorCollector;
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<TokenDto?> Handle(UserLoginCommand request, CancellationToken cancellationToken)
    {
        // 1. Tìm user theo username hoặc email
        var user = await _userManager.FindByNameAsync(request.Username)
                   ?? await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            _errorCollector.Error(
                System.Net.HttpStatusCode.Unauthorized,
                "LOGIN_FAILED",
                "Invalid username/email or password"
            );
            return null;
        }

        // 2. Check password
        var validPassword = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!validPassword)
        {
            _errorCollector.Error(
                System.Net.HttpStatusCode.Unauthorized,
                "LOGIN_FAILED",
                "Invalid username/email or password"
            );
            return null;
        }

        // 👉 Nếu tới đây là đăng nhập thành công,
        // bạn sẽ tiếp tục sinh JWT token (hoặc refresh token)
        // và trả về TokenDto
        // (chưa viết vì bạn mới yêu cầu check login trước)

        _errorCollector.Success(
            System.Net.HttpStatusCode.OK,
            "LOGIN_SUCCESS",
            "User login successful"
        );
        
        var token = _jwtTokenService.GenerateToken(user); 
        return new TokenDto
        {
            AccessToken = "TODO: generate JWT",
            RefreshToken = "TODO: generate refresh token",
            ExpiresIn = 3600
        };
    }
}