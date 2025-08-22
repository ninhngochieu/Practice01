using System.Net;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Practice01.Application.Common.Validation;

namespace Practice01.Application.User.Command.RegisterNewUser;

public class RegisterNewUserCommandHandler
    : IRequestHandler<RegisterNewUserCommand, Guid?>
{
    private readonly UserManager<Domain.Entities.User> _userManager;
    private readonly ErrorCollector _errorCollector;

    public RegisterNewUserCommandHandler(UserManager<Domain.Entities.User> userManager, ErrorCollector errorCollector)
    {
        _userManager = userManager;
        _errorCollector = errorCollector;
    }

    public async Task<Guid?> Handle(RegisterNewUserCommand request, CancellationToken cancellationToken)
    {
        var user = new Domain.Entities.User
        {
            UserName = request.UserName,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = request.DateOfBirth,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(x => new CollectedError()
            {
                Code = x.Code,
                Field = x.Code,
                Message = x.Description
            }).ToList();

            _errorCollector.Error(HttpStatusCode.BadRequest, "REGISTER_NEW_USER_ERROR", "Register new user error",
                errors);
            return null;
        }

        // bạn có thể custom exception+
        // var errors = string.Join(";", result.Errors.Select(e => e.Description));
        // throw new Exception($"Cannot create user: {errors}");

        try
        {
            var roleResult = await _userManager.AddToRoleAsync(user, "MEMBER");
        }
        catch (Exception e)
        {
            _errorCollector.Success(HttpStatusCode.OK,
                "REGISTER_NEW_USER_PARTIAL_SUCCESS",
                "Register new user success with partial errors", [
                    new CollectedError
                    {
                        Code = "ROLE_ERROR",
                        Field = "ROLE_ERROR",
                        Message = e.Message
                    }
                ]);
            return user.Id;
        }
        
        _errorCollector.Success(HttpStatusCode.OK, "REGISTER_NEW_USER_SUCCESS", "Register new user success");
        return user.Id;   
    }
}