using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Practice01.Presentation.Common.ObjectResult;

namespace Practice01.Presentation.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class TestHttpClientFactoryController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly CustomObjectResult _customObjectResult;

    public TestHttpClientFactoryController(IHttpClientFactory httpClientFactory, CustomObjectResult customObjectResult)
    {
        _httpClientFactory = httpClientFactory;
        _customObjectResult = customObjectResult;
    }

    [HttpGet("call-external")]
    public async Task<IResult> CallExternalApi()
    {
        using var client = _httpClientFactory.CreateClient("TestApi");

        var response = await client.GetAsync("/posts/1"); // Gọi thử API public (jsonplaceholder)
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return _customObjectResult.Return(content);
    }
}