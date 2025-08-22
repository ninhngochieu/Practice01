namespace Practice01.Application.Common.Token;

public class TokenDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int? ExpiresInMinutes { get; set; }
}