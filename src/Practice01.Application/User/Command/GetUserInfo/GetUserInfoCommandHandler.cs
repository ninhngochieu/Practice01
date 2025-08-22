using System.Net;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Practice01.Application.Common.User;
using Practice01.Application.Common.Validation;

namespace Practice01.Application.User.Command.GetUserInfo;

public class GetUserInfoCommandHandler : IRequestHandler<GetUserInfoCommand, UserInfoDto?>
{
    private readonly IUserProvider _userProvider;
    private readonly ErrorCollector _errorCollector;
    private readonly UserManager<Domain.Entities.User> _userManager;

    public GetUserInfoCommandHandler(IUserProvider userProvider,
        ErrorCollector errorCollector,
        UserManager<Domain.Entities.User> userManager)
    {
        _userProvider = userProvider;
        _errorCollector = errorCollector;
        _userManager = userManager;
    }

    public async Task<UserInfoDto?> Handle(GetUserInfoCommand request, CancellationToken cancellationToken)
    {
        var userClaimPrincipal = _userProvider.User;
        if (userClaimPrincipal is null)
        {
            _errorCollector.Error(System.Net.HttpStatusCode.Unauthorized, "INVALID_USER_AUTHENTICATION", "Invalid user authentication");
            return null;
        }

        var userId = userClaimPrincipal.GetUserId();
        if (!userId.HasValue)
        {
            _errorCollector.Error(HttpStatusCode.Unauthorized, "INVALID_USER_AUTHENTICATION", "Invalid user authentication");
            return null;
        }
        
        var user = await _userManager.FindByIdAsync(userId.Value.ToString());
        if (user == null)
        {
            _errorCollector.Error(HttpStatusCode.NotFound, "USER_NOT_FOUND", "User not found");
            return null;
        }
        
        _errorCollector.Success(HttpStatusCode.OK, "GET_USER_INFO_SUCCESS", "Get user info success");
        return new UserInfoDto()
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            DateOfBirth = user.DateOfBirth
        };
    }
}

