using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Practice01.Application.Common.Token;
using Practice01.Domain.Entities;
using Practice01.Domain.Entities.Users;

namespace Practice01.Infrastructure.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<User> _userManager;

    public JwtTokenService(IConfiguration configuration, UserManager<User> userManager)
    {
        _configuration = configuration;
        _userManager = userManager;
    }

    public async Task<TokenDto> GenerateToken(User user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

// 2. Key & signing credentials
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

// 3. Tạo TokenDescriptor
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:ExpirationInMinutes")),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = creds
        };

// 4. Dùng JsonWebTokenHandler sinh token string
        var tokenHandler = new JsonWebTokenHandler();
        var tokenString = tokenHandler.CreateToken(descriptor);

// 5. Trả TokenDto
        return new TokenDto
        {
            AccessToken = tokenString,
            ExpiresInMinutes = _configuration.GetValue<int>("Jwt:ExpirationInMinutes"),
            RefreshToken = ""
        };
    }
}