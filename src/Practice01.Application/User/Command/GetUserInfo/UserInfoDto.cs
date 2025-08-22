namespace Practice01.Application.User.Command.GetUserInfo;

public class UserInfoDto
{
    public Guid Id { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
}