using MediatR;

namespace Practice01.Application.WeatherForecast.Queries.GetWeatherForecastV2;

public class GetWeatherForecastV2CommandHandler : IRequestHandler<GetWeatherForecastV2Command, List<Domain.Entities.WeatherForecast>>
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];
    public Task<List<Domain.Entities.WeatherForecast>> Handle(GetWeatherForecastV2Command request, CancellationToken cancellationToken)
    {
        var result = Enumerable.Range(1, 5).Select(index => new Domain.Entities.WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        return Task.FromResult(result.ToList());
    }
}