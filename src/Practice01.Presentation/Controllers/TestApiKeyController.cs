using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

    public TestApiKeyController(CustomObjectResult customObjectResult)
    {
        _customObjectResult = customObjectResult;
    }
    
    [HttpGet("Empty-Result")]
    [MapToApiVersion("1.0")]
    public Task<IResult> GetEmpty()
    {
        return Task.FromResult(_customObjectResult.Return());
    }
}