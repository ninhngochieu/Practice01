using MediatR;

namespace Practice01.Application.User.Command.GetUserInfo;

public class GetUserInfoCommandHandler : IRequestHandler<GetUserInfoCommand, UserInfoDto>
{
    public Task<UserInfoDto> Handle(GetUserInfoCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}