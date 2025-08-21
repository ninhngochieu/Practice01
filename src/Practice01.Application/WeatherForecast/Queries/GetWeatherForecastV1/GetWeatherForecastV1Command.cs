using MediatR;

namespace Practice01.Application.WeatherForecast.Queries.GetWeatherForecastV1;

public class GetWeatherForecastV1Command : IRequest<List<Domain.Entities.WeatherForecast>>
{
    public DateTime? Date { get; set; }
}