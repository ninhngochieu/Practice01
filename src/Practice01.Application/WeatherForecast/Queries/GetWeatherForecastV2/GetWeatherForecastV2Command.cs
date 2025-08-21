using MediatR;

namespace Practice01.Application.WeatherForecast.Queries.GetWeatherForecastV2;

public class GetWeatherForecastV2Command : IRequest<List<Domain.Entities.WeatherForecast>>
{
    public DateTime? Date { get; set; }
}