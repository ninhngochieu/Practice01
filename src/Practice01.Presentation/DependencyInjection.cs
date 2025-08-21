using System.Text.Json;
using Asp.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Practice01.Presentation.Common.ObjectResult;
using Practice01.Presentation.Middleware;

namespace Practice01.Presentation;

public static class DependencyInjection
{
    public static void AddPresentation(this IServiceCollection services)
    {
        services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.ConfigureOptions<ConfigureSwaggerOptions>();


        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            // options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
            options.ReportApiVersions = true;
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV"; // Kết quả: v1, v1.1, v2…
            options.SubstituteApiVersionInUrl = true;
        });
        services.AddScoped<GlobalExceptionMiddleware>();
        services.AddScoped<CustomObjectResult>();
        services.AddHttpContextAccessor();
        services.AddKeyedSingleton("ApiResponseJsonSerializerOptions",new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }
}