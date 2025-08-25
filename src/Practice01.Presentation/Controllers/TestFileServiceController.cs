using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Practice01.Application.TestFileService.Queries;
using Practice01.Application.TestFileService.Queries.Excel;
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
    public async Task<IResult> WriteExcel()
    {
        var result =  await _sender.Send(new TestWriteExcelFileCommand());
        return _customObjectResult.Return(result);
    }
    
    [HttpPost("json")]
    [MapToApiVersion("1.0")]
    public async Task<IResult> WriteJson()
    {
        var testWriteJsonFileCommand = new TestWriteJsonFileCommand();
        var result =  await _sender.Send(testWriteJsonFileCommand);
        return _customObjectResult.Return(result);
    }
    
    [HttpGet("excel")]
    [MapToApiVersion("1.0")]
    public async Task<IResult> GetExcel([FromQuery] TestGetExcelFileQuery testGetExcelFileQuery)
    {
        var result = await _sender.Send(testGetExcelFileQuery);
        result.Position = 0;
        
        var fileName = testGetExcelFileQuery.FileName;
        return _customObjectResult.ReturnFile(result, 
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
            fileName);
    }

    [HttpGet("json")]
    [MapToApiVersion("1.0")]
    public async Task<IResult> GetJson([FromQuery] TestGetJsonFileQuery testGetJsonFileQuery)
    {
        var result = await _sender.Send(testGetJsonFileQuery);
        result.Position = 0;
        
        
        var fileName = testGetJsonFileQuery.FileName;
        return _customObjectResult.ReturnFile(result, 
            "application/json", 
            fileName);
    }
}