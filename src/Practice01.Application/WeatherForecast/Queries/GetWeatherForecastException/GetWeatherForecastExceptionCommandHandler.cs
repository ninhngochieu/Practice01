using MediatR;

namespace Practice01.Application.WeatherForecast.Queries.GetWeatherForecastException;

public class GetWeatherForecastExceptionCommandHandler : IRequestHandler<GetWeatherForecastExceptionCommand>
{
    public Task Handle(GetWeatherForecastExceptionCommand request, CancellationToken cancellationToken)
    {
        throw new Exception("Test exception");
    }
}