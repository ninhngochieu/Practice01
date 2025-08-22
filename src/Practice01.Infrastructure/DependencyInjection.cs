using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Practice01.Application.Common.Data;
using Practice01.Application.Common.Token;
using Practice01.Domain.Entities;
using Practice01.Infrastructure.Data;
using Practice01.Infrastructure.Services;
using StackExchange.Redis;
using Role = Practice01.Domain.Entities.Role;

namespace Practice01.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, ConfigurationManager builderConfiguration)
    {
        var pgConnectionString = builderConfiguration.GetConnectionString("Practice01StartupContextConnection") ??
                               throw new InvalidOperationException(
                                   "Connection string 'Practice01StartupContextConnection' not found.");

        services.AddDbContext<Practice01StartupContext>(options =>
        {
            options.UseNpgsql(pgConnectionString, builder =>
            {
                builder.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorCodesToAdd: null);
            });
        });
        services.AddScoped<ISqlConnectionFactory>(sp => new SqlConnectionFactory(pgConnectionString));
        services.AddIdentity<User, Role>(options =>
            {
                // Chính sách password
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;

                // Lockout
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
                options.Lockout.MaxFailedAccessAttempts = 5;

                // User
                options.User.RequireUniqueEmail = true;

            }).AddEntityFrameworkStores<Practice01StartupContext>()
            .AddDefaultTokenProviders();

        var redisConnectionString = builderConfiguration.GetConnectionString("RedisConnection") ??
                               throw new InvalidOperationException(
                                   "Connection string 'RedisConnection' not found.");
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var configuration = ConfigurationOptions.Parse(redisConnectionString, true);

            configuration.AbortOnConnectFail = false; // Cho phép retry khi mất kết nối
            configuration.ConnectRetry = 3; // Retry tối đa 3 lần
            configuration.ReconnectRetryPolicy = new ExponentialRetry(5000); // Retry sau 5s, exponential

            return ConnectionMultiplexer.Connect(configuration);
        });

        services.AddScoped<IJwtTokenService, JwtTokenService>();
    }
}