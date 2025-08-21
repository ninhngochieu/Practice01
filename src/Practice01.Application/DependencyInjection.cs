using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Practice01.Application.Common.Behaviours;
using Practice01.Application.Common.Validation;

namespace Practice01.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped<ErrorCollector>();
    }
}