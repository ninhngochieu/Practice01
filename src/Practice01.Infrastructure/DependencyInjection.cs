using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Practice01.Startup.Data;

namespace Practice01.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, ConfigurationManager builderConfiguration)
    {
        var connectionString = builderConfiguration.GetConnectionString("Practice01StartupContextConnection") ??
                               throw new InvalidOperationException(
                                   "Connection string 'Practice01StartupContextConnection' not found.");

        services.AddDbContext<Practice01StartupContext>(options => options.UseNpgsql(connectionString));
    }
}