using System.Diagnostics;
using System.Diagnostics.Metrics;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Practice01.Presentation.Common.ObjectResult;

namespace Practice01.Presentation.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class TestOpenTelemetryController : ControllerBase
{
    private readonly CustomObjectResult _customObjectResult;

    public TestOpenTelemetryController(CustomObjectResult customObjectResult)
    {
        _customObjectResult = customObjectResult;
    }
    // Tracing
    private static readonly ActivitySource ActivitySource = new("Practice01.Presentation");

    [HttpGet("trace")]
    public IResult GetTrace()
    {
        using var activity = ActivitySource.StartActivity("TestTrace");
        activity?.SetTag("custom.tag", "trace-endpoint");
        activity?.AddEvent(new ActivityEvent("Trace endpoint called"));

        // business logic demo
        var result = new { Message = "Tracing OK", Time = DateTime.UtcNow };
        return _customObjectResult.Return(result);
    }

    //[HttpGet("metric")]
    //public IResult GetMetric()
    //{
    //    RequestCounter.Add(1,
    //        new KeyValuePair<string, object?>("endpoint", "metric"),
    //        new KeyValuePair<string, object?>("version", "v1"),
    //        new KeyValuePair<string, object?>("version", "v2"));

    //    var result = new { Message = "Metric incremented", Time = DateTime.UtcNow };
    //    return _customObjectResult.Return(result);
    //}
}