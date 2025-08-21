using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Practice01.Application.WeatherForecast.Queries.GetWeatherForecast;
using Practice01.Domain.Entities;

namespace Practice01.Presentation.Controllers;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly ISender _sender;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, ISender sender)
    {
        _logger = logger;
        _sender = sender;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet(Name = "GetWeatherForecast")]
    [MapToApiVersion("1.0")]
    public async Task<OkObjectResult> GetV1()
    {
        var result = await _sender.Send(new GetWeatherForecastCommand());
        return Ok(result);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet(Name = "GetWeatherForecast")]
    [MapToApiVersion("2.0")]
    public async Task<OkObjectResult> GetV2()
    {
        var result = await _sender.Send(new GetWeatherForecastCommand()
        {
            Date = null
        });
        return Ok(result);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("Exception",Name = "Exception")]
    [MapToApiVersion("2.0")]
    public async Task<OkObjectResult> ThrowExV2()
    {
        var result = await _sender.Send(new GetWeatherForecastCommand());
        return Ok(result);
    }
}