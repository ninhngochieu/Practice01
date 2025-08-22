namespace Practice01.Application.User.Command.Login;

public class TokenDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int? ExpiresIn { get; set; }
}