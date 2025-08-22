using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Practice01.Application.TestApiKey.Queries.TestApiKey;
using Practice01.Presentation.Common.ObjectResult;

namespace Practice01.Presentation.Controllers;

[Authorize(AuthenticationSchemes = "ApiKeyScheme")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class TestApiKeyController : ControllerBase
{
    private readonly CustomObjectResult _customObjectResult;
    private readonly ISender _sender;

    public TestApiKeyController(CustomObjectResult customObjectResult, ISender sender)
    {
        _customObjectResult = customObjectResult;
        _sender = sender;
    }
    
    [HttpGet]
    [MapToApiVersion("1.0")]
    public async Task<IResult> GetEmpty()
    {
        var result =  await _sender.Send(new TestApiKeyCommand());
        return _customObjectResult.Return(result);
    }
}