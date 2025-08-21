using FluentValidation;
using Practice01.Application.WeatherForecast.Queries.GetWeatherForecastV2;

namespace Practice01.Application.WeatherForecast.Queries.GetWeatherForecastV1;

public class GetWeatherForecastV1Validator : AbstractValidator<GetWeatherForecastV1Command>
{
    public GetWeatherForecastV1Validator()
    {
        RuleFor(wf => wf.Date).NotNull().WithMessage("Date is required");
    }
}