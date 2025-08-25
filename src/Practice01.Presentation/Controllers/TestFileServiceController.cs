using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Practice01.Application.TestFileService.Queries;
using Practice01.Presentation.Common.ObjectResult;

namespace Practice01.Presentation.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class TestFileServiceController : ControllerBase
{
    private readonly CustomObjectResult _customObjectResult;
    private readonly ISender _sender;

    public TestFileServiceController(CustomObjectResult customObjectResult, ISender sender)
    {
        _customObjectResult = customObjectResult;
        _sender = sender;
    }
    
    [HttpPost("excel")]
    [MapToApiVersion("1.0")]
    public async Task<IResult> GetEmpty()
    {
        var result =  await _sender.Send(new TestWriteExcelFileCommand());
        return _customObjectResult.Return(result);
    }
}