using FluentValidation;

namespace Practice01.Application.WeatherForecast.Queries.GetWeatherForecastV2;

public class GetWeatherForecastV2Validator : AbstractValidator<GetWeatherForecastV2Command>
{
    public GetWeatherForecastV2Validator()
    {
        RuleFor(wf => wf.Date).NotNull().WithMessage("Date is required");
    }
}