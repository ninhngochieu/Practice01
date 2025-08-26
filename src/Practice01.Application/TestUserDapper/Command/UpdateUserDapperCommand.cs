using System.Net;
using Dapper;
using MediatR;
using Practice01.Application.Common.Data;
using Practice01.Application.Common.Validation;

namespace Practice01.Application.TestUserDapper.Command;

public class UpdateUserDapperCommand : IRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public Guid Id { get; set; }
}

public class UpdateUserDapperCommandHandler : IRequestHandler<UpdateUserDapperCommand>
{
    private readonly ErrorCollector _errorCollector;
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public UpdateUserDapperCommandHandler(ErrorCollector errorCollector, ISqlConnectionFactory sqlConnectionFactory)
    {
        _errorCollector = errorCollector;
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task Handle(UpdateUserDapperCommand request, CancellationToken cancellationToken)
    {
        var sqlConnection = _sqlConnectionFactory.GetConnection();
        var exists =
            await sqlConnection.QueryFirstOrDefaultAsync<bool>(
                @"
                        select CAST(1 AS BOOLEAN) 
                        from ""AspNetUsers"" anu 
                        where 
                            anu.""Id"" = @Id
                            limit 1", new { request.Id });
        if (!exists)
        {
            _errorCollector.Error(HttpStatusCode.BadRequest, "USER_NOT_FOUND", "User not found");
            return;
        }

        var updatingInfo = new
        {
            request.Id,
            request.FirstName,
            request.LastName,
            request.DateOfBirth,
            request.Email,
            request.PhoneNumber
        };

        var rowCount = await sqlConnection.ExecuteAsync(@"
                        update ""AspNetUsers"" anu 
                        set 
                            ""FirstName"" = @FirstName,
                            ""LastName"" = @LastName,
                            ""DateOfBirth"" = @DateOfBirth,
                            ""Email"" = @Email,
                            ""PhoneNumber"" = @PhoneNumber
                        where 
                            anu.""Id"" = @Id", updatingInfo);

        if (rowCount == 0)
        {
            _errorCollector.Error(HttpStatusCode.BadRequest, "USER_NOT_FOUND", "User not found");
            return;
        }

        _errorCollector.Success(HttpStatusCode.NoContent, "UPDATE_USER_SUCCESS", "Update User Success");
    }
}