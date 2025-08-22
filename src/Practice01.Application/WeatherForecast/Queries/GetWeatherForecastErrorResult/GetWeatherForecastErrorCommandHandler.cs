using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using Practice01.Application.Common.Validation;

namespace Practice01.Application.WeatherForecast.Queries.GetWeatherForecastErrorResult;

public class GetWeatherForecastErrorCommandHandler : IRequestHandler<GetWeatherForecastErrorCommand>
{
    private readonly ErrorCollector _errorCollector;
    private readonly ILogger<GetWeatherForecastErrorCommandHandler> _logger;

    public GetWeatherForecastErrorCommandHandler(ErrorCollector errorCollector,
        ILogger<GetWeatherForecastErrorCommandHandler> logger)
    {
        _errorCollector = errorCollector;
        _logger = logger;
    }
    public Task Handle(GetWeatherForecastErrorCommand request, CancellationToken cancellationToken)
    {
        var random = new Random();
        var randomNumber = random.Next(0, 1000);
        
        if (randomNumber % 2 != 0)
        {
            _errorCollector.Error(HttpStatusCode.BadRequest,"GET_WEATHER_FORECAST_ERROR","Get WeatherForecast Error");
            _logger.LogError("Get WeatherForecast Error with random number {randomNumber}", randomNumber);
            return Task.CompletedTask;
        }
        
        if (randomNumber % 3 != 0)
        {
            _errorCollector.Error(HttpStatusCode.Conflict,"GET_WEATHER_FORECAST_ERROR","Get WeatherForecast Error");
            _logger.LogError("Get WeatherForecast Error with random number {randomNumber}", randomNumber);
            return Task.CompletedTask;
        }
        
        _errorCollector.Success(HttpStatusCode.OK,"GET_WEATHER_FORECAST_SUCCESS","Get WeatherForecast Success");
        _logger.LogInformation("Get WeatherForecast Success with random number {randomNumber}", randomNumber);
        return Task.CompletedTask;
    }
}