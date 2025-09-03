using System.Net;
using Dapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Practice01.Application.Common.Data;
using Practice01.Application.Common.Datetime;
using Practice01.Application.Common.Validation;

namespace Practice01.Application.TestUserDapper.Command;

public class CreateUserDapperCommand : IRequest
{
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
}

public class CreateUserDapperCommandHandler : IRequestHandler<CreateUserDapperCommand>
{
    private readonly ErrorCollector _errorCollector;
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateUserDapperCommandHandler(ErrorCollector errorCollector, ISqlConnectionFactory sqlConnectionFactory,
        IDateTimeProvider dateTimeProvider)
    {
        _errorCollector = errorCollector;
        _sqlConnectionFactory = sqlConnectionFactory;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task Handle(CreateUserDapperCommand request, CancellationToken cancellationToken)
    {
        var sqlConnection = _sqlConnectionFactory.GetConnection();
        var exists =
            await sqlConnection.QueryFirstOrDefaultAsync<bool>(
                @"
                        select top(1) CAST(1 AS BIT) 
                        from AspNetUsers anu 
                        where 
                            anu.UserName = @UserName", new { request.UserName });
        if (exists)
        {
            _errorCollector.Error(HttpStatusCode.BadRequest, "USER_ALREADY_EXISTS", "User already exists");
            return;
        }

        var hasher = new PasswordHasher<Domain.Entities.Users.User>();
        var newUser = new
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = request.DateOfBirth,
            IsActive = true,
            CreatedDate = _dateTimeProvider.Now,
            UserName = request.UserName,
            NormalizedUserName = request.UserName,
            Email = request.Email,
            NormalizedEmail = request.Email.ToUpper(),
            EmailConfirmed = true,
            PasswordHash = hasher.HashPassword(null, request.UserName),
            PhoneNumber = request.PhoneNumber,
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = false,
            LockoutEnd = false,
            LockoutEnabled = false,
            AccessFailedCount = 0
        };
        
        var rowCount = await sqlConnection.ExecuteAsync(
            @"
                INSERT INTO AspNetUsers 
                (
                    Id, 
                    FirstName, 
                    LastName, 
                    DateOfBirth, 
                    IsActive, 
                    CreatedDate, 
                    UserName, 
                    NormalizedUserName, 
                    Email, 
                    NormalizedEmail, 
                    EmailConfirmed, 
                    PasswordHash, 
                    PhoneNumber,
                    PhoneNumberConfirmed,
                    TwoFactorEnabled,
                    LockoutEnabled,
                    AccessFailedCount
                ) 
                VALUES 
                (
                    @Id,
                    @FirstName,
                    @LastName,
                    @DateOfBirth,
                    @IsActive,
                    @CreatedDate,
                    @UserName,
                    @NormalizedUserName,
                    @Email,
                    @NormalizedEmail,
                    @EmailConfirmed,
                    @PasswordHash,
                    @PhoneNumber,
                    @PhoneNumberConfirmed,
                    @TwoFactorEnabled,
                    @LockoutEnabled,
                    @AccessFailedCount
                )",
            newUser);
        
        if (rowCount == 0)
        {
            _errorCollector.Error(HttpStatusCode.BadRequest, "CREATE_USER_DAPPER_ERROR", "Create User Dapper Error");
            return;
        }
        _errorCollector.Success(HttpStatusCode.Created, "CREATE_USER_DAPPER_SUCCESS", "Create User Dapper Success");
    }
}