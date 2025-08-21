using MediatR;

namespace Practice01.Application.WeatherForecast.Queries.GetWeatherForecast;

public class GetWeatherForecastCommand : IRequest<List<Domain.Entities.WeatherForecast>>
{
    public DateTime? Date { get; set; }
}