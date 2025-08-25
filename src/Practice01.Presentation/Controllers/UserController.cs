using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Practice01.Application.User.Command.GetUserInfo;
using Practice01.Application.User.Command.Login;
using Practice01.Application.User.Command.RegisterNewUser;
using Practice01.Presentation.Common.ObjectResult;

namespace Practice01.Presentation.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UserController
{
    private readonly ISender _sender;
    private readonly CustomObjectResult _customObjectResult;

    public UserController(ISender sender, CustomObjectResult customObjectResult)
    {
        _sender = sender;
        _customObjectResult = customObjectResult;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpPost("Register")]
    [MapToApiVersion("1.0")]
    [AllowAnonymous]
    public async Task<IResult> Register([FromBody] RegisterNewUserCommand registerNewUserCommand)
    {
        var result = await _sender.Send(registerNewUserCommand);
        return _customObjectResult.Return(result);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpPost("Login")]
    [MapToApiVersion("1.0")]
    [AllowAnonymous]
    public async Task<IResult> Login([FromBody] UserLoginCommand userLoginCommand)
    {
        var result = await _sender.Send(userLoginCommand);
        return _customObjectResult.Return(result);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("Me")]
    [MapToApiVersion("1.0")]
    public async Task<IResult> GetUserInfo()
    {
        var result = await _sender.Send(new GetUserInfoCommand());
        return _customObjectResult.Return(result);
    }
    
    [HttpGet("Admin")]
    [MapToApiVersion("1.0")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IResult> AllowAdmin()
    {
        return _customObjectResult.Return("Admin");
    }
    
    [HttpGet("Manager")]
    [MapToApiVersion("1.0")]
    [Authorize(Policy = "ManagerPolicy")]
    public async Task<IResult> AllowManager()
    {
        return _customObjectResult.Return("Manager");
    }
    
    [HttpGet("Member")]
    [MapToApiVersion("1.0")]
    [Authorize(Policy = "MemberPolicy")]
    public async Task<IResult> AllowMember()
    {
        return _customObjectResult.Return("Member");
    }
}