using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Practice01.Application.WeatherForecast.Queries.GetWeatherForecastV1;
using Practice01.Application.WeatherForecast.Queries.GetWeatherForecastV2;
using Practice01.Presentation.Common;
using Practice01.Presentation.Common.ObjectResult;

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
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly ISender _sender;
    private readonly CustomObjectResult _customObjectResult;

    public WeatherForecastController(ILogger<WeatherForecastController> logger,
        ISender sender,
        CustomObjectResult customObjectResult)
    {
        _logger = logger;
        _sender = sender;
        _customObjectResult = customObjectResult;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet(Name = "GetWeatherForecast")]
    [MapToApiVersion("1.0")]
    public async Task<IResult> GetV1()
    {
        var result = await _sender.Send(new GetWeatherForecastV1Command());
        // return Ok(result);
        return _customObjectResult.Return(result);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet(Name = "GetWeatherForecast")]
    [MapToApiVersion("2.0")]
    public async Task<IResult> GetV2(DateTime? date)
    {
        var result = await _sender.Send(new GetWeatherForecastV2Command()
        {
            Date = date
        });
        return _customObjectResult.Return(result);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("Exception",Name = "Exception")]
    [MapToApiVersion("2.0")]
    public async Task<IResult> ThrowExV2()
    {
        var result = await _sender.Send(new GetWeatherForecastV2Command());
        return _customObjectResult.Return(result);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("Empty-Result")]
    [MapToApiVersion("1.0")]
    public async Task<IResult> GetEmpty()
    {
        return _customObjectResult.Return();
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("Error-Result")]
    [MapToApiVersion("1.0")]
    public async Task<IResult> GetError()
    {
        await _sender.Send(new GetWeatherForecastErrorCommand());
        return _customObjectResult.Return();
    }
}