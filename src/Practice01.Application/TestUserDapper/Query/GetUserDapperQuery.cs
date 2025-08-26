using Dapper;
using MediatR;
using Practice01.Application.Common.Data;

namespace Practice01.Application.TestUserDapper.Query;

public class GetUserDapperQuery : IRequest<List<UserDto>>
{
}

public class GetUserDapperQueryHandler : IRequestHandler<GetUserDapperQuery, List<UserDto>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetUserDapperQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<List<UserDto>> Handle(GetUserDapperQuery request, CancellationToken cancellationToken)
    {
        var sqlBuilder = new SqlBuilder();
        var template = sqlBuilder.AddTemplate("public.\"AspNetUsers\" /**where**/");
        
        sqlBuilder.Where("\"IsActive\"", true);
        var sqlQuery =
            $@"SELECT 
                    ""Id"", 
                    ""FirstName"", 
                    ""LastName"", 
                    ""DateOfBirth"", 
                    ""IsActive"", 
                    ""UserName"", 
                    ""NormalizedUserName"", 
                    ""Email"", 
                    ""NormalizedEmail"",
                    ""PhoneNumber"" 
            From {template.RawSql}
            ";
        
        var connection = _sqlConnectionFactory.GetConnection();
        var users = (await connection.QueryAsync<UserDto>(sqlQuery)).AsList();

        return users;
    }
}

public class UserDto
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool IsActive { get; set; }
    public string? UserName { get; set; }
    public string? NormalizedUserName { get; set; }
    public string? Email { get; set; }
    public string? NormalizedEmail { get; set; }
    public string? PhoneNumber { get; set; }
}