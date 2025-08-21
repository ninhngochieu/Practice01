using FluentValidation;

namespace Practice01.Application.WeatherForecast.Queries.GetWeatherForecast;

public class GetWeatherForecastValidator : AbstractValidator<GetWeatherForecastCommand>
{
    public GetWeatherForecastValidator()
    {
        RuleFor(wf => wf.Date).NotNull().WithMessage("Date is required");
    }
}