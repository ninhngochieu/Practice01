using System.Net;
using Dapper;
using MediatR;
using Practice01.Application.Common.Data;
using Practice01.Application.Common.Validation;

namespace Practice01.Application.TestUserDapper.Command;

public class DeleteUserDapperCommand : IRequest
{
    public Guid Id { get; set; }
}

public class DeleteUserDapperCommandHandler : IRequestHandler<DeleteUserDapperCommand>
{
    private readonly ErrorCollector _errorCollector;
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public DeleteUserDapperCommandHandler(ErrorCollector errorCollector, ISqlConnectionFactory sqlConnectionFactory)
    {
        _errorCollector = errorCollector;
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task Handle(DeleteUserDapperCommand request, CancellationToken cancellationToken)
    {
        var sqlConnection = _sqlConnectionFactory.GetConnection();
        var rowCount =
            await sqlConnection.ExecuteAsync(@"
                delete 
                from AspNetUsers 
                where Id = @Id",
                new { request.Id });
        if (rowCount == 0)
        {
            _errorCollector.Error(HttpStatusCode.BadRequest, "USER_NOT_FOUND", "User not found");
            return;
        }

        _errorCollector.Success(HttpStatusCode.NoContent, "DELETE_USER_SUCCESS", "Delete User Success");
    }
}