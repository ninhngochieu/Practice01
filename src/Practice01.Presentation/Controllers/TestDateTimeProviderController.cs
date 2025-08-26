using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Practice01.Application.Common.Datetime;
using Practice01.Presentation.Common.ObjectResult;

namespace Practice01.Presentation.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class TestDateTimeProviderController : ControllerBase
{
    private readonly CustomObjectResult _customObjectResult;
    private readonly IDateTimeProvider _dateTimeProvider;

    public TestDateTimeProviderController(CustomObjectResult customObjectResult, IDateTimeProvider dateTimeProvider)
    {
        _customObjectResult = customObjectResult;
        _dateTimeProvider = dateTimeProvider;
    }

    [HttpGet]
    public Task<IResult> GetVietnamTime()
    {
        return Task.FromResult(_customObjectResult.Return(_dateTimeProvider.Now));
    }
}