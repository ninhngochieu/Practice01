using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Practice01.Application.TestUserDapper.Command;
using Practice01.Application.TestUserDapper.Query;
using Practice01.Presentation.Common.ObjectResult;

namespace Practice01.Presentation.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class TestDapperController : ControllerBase
{
    private readonly CustomObjectResult _customObjectResult;
    private readonly ISender _sender;

    public TestDapperController(CustomObjectResult customObjectResult, ISender sender)
    {
        _customObjectResult = customObjectResult;
        _sender = sender;
    }
    
    [HttpGet]
    [MapToApiVersion("1.0")]
    public async Task<IResult> GetUserDapper()
    {
        var result =  await _sender.Send(new GetUserDapperQuery());
        return _customObjectResult.Return(result);
    }
    
    [HttpPost]
    [MapToApiVersion("1.0")]
    public async Task<IResult> CreateUserDapper([FromBody] CreateUserDapperCommand createUserDapperCommand)
    {
        await _sender.Send(createUserDapperCommand);
        return _customObjectResult.Return();
    }
    
    [HttpPut("{id}")]
    [MapToApiVersion("1.0")]
    public async Task<IResult> UpdateUserDapper(Guid id,[FromBody] UpdateUserDapperCommand updateUserDapperCommand)
    {
        updateUserDapperCommand.Id = id;
        await _sender.Send(updateUserDapperCommand);
        return _customObjectResult.Return();
    }
    
    [HttpDelete("{id}")]
    [MapToApiVersion("1.0")]
    public async Task<IResult> DeleteUserDapper(Guid id)
    {
        await _sender.Send(new DeleteUserDapperCommand()
        {
            Id = id
        });
        return _customObjectResult.Return();
    }
}
