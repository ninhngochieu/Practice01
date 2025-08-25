using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Practice01.Application.TestApiKey.Queries.TestRedis.Delete;
using Practice01.Application.TestApiKey.Queries.TestRedis.Get;
using Practice01.Application.TestApiKey.Queries.TestRedis.Set;
using Practice01.Presentation.Common.ObjectResult;

namespace Practice01.Presentation.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class TestRedisController : ControllerBase
{
    private readonly CustomObjectResult _customObjectResult;
    private readonly ISender _sender;

    public TestRedisController(CustomObjectResult customObjectResult, ISender sender)
    {
        _customObjectResult = customObjectResult;
        _sender = sender;
    }
    [HttpGet]
    [MapToApiVersion("1.0")]
    public async Task<IResult> GetCache([FromQuery] string key)
    {
        var result =  await _sender.Send(new TestGetRedisQuery(key));
        return _customObjectResult.Return(result);
    }
    
    [HttpPost]
    [MapToApiVersion("1.0")]
    public async Task<IResult> SetCache([FromBody]TestSetRedisCommand testSetRedisCommand)
    {
        await _sender.Send(testSetRedisCommand);
        return _customObjectResult.Return();
    }
    
    [HttpPut]
    [MapToApiVersion("1.0")]
    public async Task<IResult> UpdateCache([FromBody] TestUpdateRedisCommand testUpdateRedisCommand)
    {
        await _sender.Send(testUpdateRedisCommand);
        return _customObjectResult.Return();
    }
    
    [HttpDelete]
    [MapToApiVersion("1.0")]
    public async Task<IResult> DeleteCache([FromQuery] string key)
    {
        await _sender.Send(new TestDeleteRedisCommand(key));
        return _customObjectResult.Return();
    }
}