using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Practice01.Application.Common.Data;
using Practice01.Infrastructure.Data;
using Practice01.Startup.Data;

namespace Practice01.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, ConfigurationManager builderConfiguration)
    {
        var connectionString = builderConfiguration.GetConnectionString("Practice01StartupContextConnection") ??
                               throw new InvalidOperationException(
                                   "Connection string 'Practice01StartupContextConnection' not found.");

        services.AddDbContext<Practice01StartupContext>(options =>
        {
            options.UseNpgsql(connectionString, builder =>
            {
                builder.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorCodesToAdd: null);
            });
        });
        services.AddScoped<ISqlConnectionFactory>(sp => new SqlConnectionFactory(connectionString));
    }
}