using System.Net;
using MediatR;
using Practice01.Application.Common.Validation;
using Practice01.Application.WeatherForecast.Queries.GetWeatherForecastV2;

namespace Practice01.Application.WeatherForecast.Queries.GetWeatherForecastV1;

public class GetWeatherForecastV1CommandHandler : IRequestHandler<GetWeatherForecastV1Command, List<Domain.Entities.WeatherForecast>>
{
    private readonly ErrorCollector _errorCollector;

    public GetWeatherForecastV1CommandHandler(ErrorCollector errorCollector)
    {
        _errorCollector = errorCollector;
    }
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];
    public Task<List<Domain.Entities.WeatherForecast>> Handle(GetWeatherForecastV1Command request, CancellationToken cancellationToken)
    {
        var result = Enumerable.Range(1, 5).Select(index => new Domain.Entities.WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        
        _errorCollector.Success(HttpStatusCode.OK, "GET_WEATHER_FORECAST_V1_SUCCESS","Get WeatherForecast V1 Success");
        return Task.FromResult(result.ToList());
    }
}