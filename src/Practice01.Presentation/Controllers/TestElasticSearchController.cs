using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Practice01.Application.TestElasticSearch;
using Practice01.Presentation.Common.ObjectResult;

namespace Practice01.Presentation.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class TestElasticSearchController : ControllerBase
{
    private readonly ISender sender;
    private readonly CustomObjectResult customObjectResult1;

    public TestElasticSearchController(ISender sender, CustomObjectResult customObjectResult1)
    {
        this.sender = sender;
        this.customObjectResult1 = customObjectResult1;
    }


    [HttpPost]
    public async Task<IResult> Create([FromBody] CreateIndexCommand indexCommand)
    {
        var id = await sender.Send(indexCommand);
        return customObjectResult1.Return(id);
    }

    [HttpGet("{date}/{id}")]
    public async Task<IActionResult> Get(string id, DateTime date)
    {
        throw new NotImplementedException();
    }

    [HttpPut("{date}/{id}")]
    public async Task<IActionResult> Update(string id, DateTime date, [FromBody] UpdateIndexCommand log)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{date}/{id}")]
    public async Task<IActionResult> Delete(string id, DateTime date)
    {
        throw new NotImplementedException();
    }
}
