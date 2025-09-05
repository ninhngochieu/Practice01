using Asp.Versioning;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Practice01.Presentation.Common.EventSources;
using Practice01.Presentation.Common.Handler;
using Practice01.Presentation.Common.ObjectResult;
using Practice01.Presentation.Interceptors;
using Practice01.Presentation.Middleware;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;

namespace Practice01.Presentation;

public static class DependencyInjection
{
    public static void AddPresentation(this IServiceCollection services, ConfigurationManager builderConfiguration)
    {
        services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            // c.SwaggerDoc("v1", new() { Title = "My API", Version = "v1" });

            // Định nghĩa ApiKey scheme
            c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
            {
                Description = "Nhập API Key vào ô bên dưới. Header: X-API-KEY. Ex: 12345-ABCDE",
                Name = "X-API-KEY",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "ApiKeyScheme"
            });

            // Bearer JWT Scheme
            c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Description = "Nhập JWT token vào đây (ví dụ: eyJhbGci...)",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            // Áp dụng scheme mặc định cho tất cả endpoints
            c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "ApiKey"
                        }
                    },
                    Array.Empty<string>()
                },
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
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
        services.AddScoped<SerilogEnrichMiddleware>();
        services.AddScoped<CustomObjectResult>();
        services.AddHttpContextAccessor();
        services.AddKeyedSingleton("ApiResponseJsonSerializerOptions", new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // services.AddSingleton<ApiKeyMiddleware>();
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKeyScheme", null)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builderConfiguration["Jwt:Issuer"],

                    ValidateAudience = true,
                    ValidAudience = builderConfiguration["Jwt:Audience"],

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builderConfiguration["Jwt:Key"]!)),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        var jsonSerializerOptions =
                            context.HttpContext.RequestServices.GetKeyedService<JsonSerializerOptions>(
                                "ApiResponseJsonSerializerOptions");

                        context.HandleResponse(); // ngăn default response
                        context.Response.ContentType = "application/json";

                        context.Response.StatusCode = 401;

                        var result = System.Text.Json.JsonSerializer.Serialize(new ApiResponse()
                        {
                            Ok = false,
                            StatusCode = 401,
                            Message = "Unauthorized",
                            Code = "UNAUTHORIZED",
                            Metadata = new Metadata
                            {
                                RequestId = context.HttpContext.TraceIdentifier,
                                TraceId = Activity.Current?.Id ?? string.Empty
                            }
                        }, jsonSerializerOptions);

                        return context.Response.WriteAsync(result);
                    }
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy("MemberPolicy",
                policy => { policy.RequireRole("Administrator", "Manager", "Member"); })
            .AddPolicy("ManagerPolicy", policy => { policy.RequireRole("Administrator", "Manager"); })
            .AddPolicy("AdminPolicy", policy => { policy.RequireRole("Administrator"); });

        services.AddSingleton<AuthorizationMiddlewareResultHandler>(sp => new AuthorizationMiddlewareResultHandler());
        services.AddSingleton<IAuthorizationMiddlewareResultHandler, CustomAuthorizationResultHandler>();

        services.AddRateLimiter(options =>
        {
            const bool autoReplenishment = true;
            const int permitLimit = 100; // cho phép 100 request
            const int queueLimit = 20; // thêm hàng chờ
            var window = TimeSpan.FromSeconds(1);
            const QueueProcessingOrder queueProcessingOrder = QueueProcessingOrder.OldestFirst;
            const int segmentsPerWindow = 10; // chia cửa sổ thành 10 đoạn để phân tán đều

            var replenishmentPeriod = TimeSpan.FromMilliseconds(100);
            const int tokensPerPeriod = 10; // nạp thêm 10 token mỗi 100ms (tương đương 100 req/s)
            const int tokenLimit = 100; // số token tối đa

            // options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            //     RateLimitPartition.GetFixedWindowLimiter(
            //         partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            //         factory: partition => new FixedWindowRateLimiterOptions
            //         {
            //             AutoReplenishment = autoReplenishment,
            //             PermitLimit = permitLimit, // Cho phép 2000 requests
            //             QueueLimit = queueLimit, // Queue 100 requests thay vì 0
            //             Window = window // Trong 1 giây
            //         }));

            // options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            //     RateLimitPartition.GetSlidingWindowLimiter(
            //         partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            //         factory: partition => new SlidingWindowRateLimiterOptions()
            //         {
            //             AutoReplenishment = autoReplenishment,
            //             PermitLimit = permitLimit, // Cho phép 2000 requests
            //             QueueLimit = queueLimit, // Queue 100 requests thay vì 0
            //             Window = window,
            //             QueueProcessingOrder = queueProcessingOrder,
            //             SegmentsPerWindow = segmentsPerWindow
            //         }));

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetTokenBucketLimiter(
                    partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                    factory: partition => new TokenBucketRateLimiterOptions()
                    {
                        AutoReplenishment = autoReplenishment,
                        QueueLimit = queueLimit, // Queue 100 requests thay vì 0,
                        QueueProcessingOrder = queueProcessingOrder,
                        ReplenishmentPeriod = replenishmentPeriod,
                        TokensPerPeriod = tokensPerPeriod,
                        TokenLimit = tokenLimit,
                    }));

            options.AddFixedWindowLimiter("fixed-window", opt =>
            {
                opt.AutoReplenishment = autoReplenishment;
                opt.PermitLimit = permitLimit; // Tăng từ 10 lên 2000
                opt.QueueLimit = queueLimit; // Tăng từ 0 lên 100
                opt.Window = window; // Giảm từ 1 phút xuống 1 giây
                opt.QueueProcessingOrder = queueProcessingOrder;
            });

            options.AddSlidingWindowLimiter(
                "sliding-window", opt =>
                {
                    opt.AutoReplenishment = autoReplenishment;
                    opt.PermitLimit = permitLimit; // Tăng từ 10 lên 2000
                    opt.QueueLimit = queueLimit; // Tăng từ 0 lên 100  
                    opt.Window = window; // Giảm từ 1 phút xuống 1 giây
                    opt.SegmentsPerWindow = segmentsPerWindow; // Tăng từ 6 lên 10 để làm mịn hơn
                    opt.QueueProcessingOrder = queueProcessingOrder;
                });

            options.AddTokenBucketLimiter(
                "token-bucket", opt =>
                {
                    opt.AutoReplenishment = autoReplenishment;
                    opt.QueueLimit = queueLimit; // Tăng từ 0 lên 100
                    opt.QueueProcessingOrder = queueProcessingOrder;
                    opt.ReplenishmentPeriod = replenishmentPeriod; // Giảm từ 1 phút xuống 1 giây
                    opt.TokenLimit = tokenLimit; // Tăng từ 10 lên 2500 (cho phép burst)
                    opt.TokensPerPeriod = tokensPerPeriod; // Giảm từ 100 xuống 2000/giây
                });

            options.AddConcurrencyLimiter(
                "concurrency", opt =>
                {
                    opt.QueueLimit = queueLimit; // Tăng từ 0 lên 200
                    opt.QueueProcessingOrder = queueProcessingOrder;
                    opt.PermitLimit = permitLimit; // Tăng từ 0 lên 500 concurrent requests
                });


            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (context, token) =>
            {
                var jsonSerializerOptions =
                    context.HttpContext.RequestServices.GetKeyedService<JsonSerializerOptions>(
                        "ApiResponseJsonSerializerOptions");
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILogger<Microsoft.AspNetCore.RateLimiting.OnRejectedContext>>();

                context.HttpContext.Response.ContentType = "application/json";
                var apiResponse = new ApiResponse
                {
                    Ok = false,
                    StatusCode = StatusCodes.Status429TooManyRequests,
                    Message = "Too many requests, please try again later.",
                    Code = "TOO_MANY_REQUESTS",
                    Metadata = new Metadata
                    {
                        RequestId = context.HttpContext.TraceIdentifier,
                        TraceId = System.Diagnostics.Activity.Current?.Id ?? string.Empty
                    }
                };

                var json = JsonSerializer.Serialize(apiResponse, jsonSerializerOptions);
                await context.HttpContext.Response.WriteAsync(json, token);

                var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";
                var user = context.HttpContext.User?.Identity?.IsAuthenticated == true
                    ? context.HttpContext.User.Identity.Name
                    : "Anonymous";
                logger.LogWarning(
                    "Rate limit exceeded. IP: {IP}, User: {User}, Path: {Path}, Method: {Method}",
                    ip,
                    user,
                    context.HttpContext.Request.Path,
                    context.HttpContext.Request.Method);
            };
        });

        services.AddHealthChecks()
            .AddRedis(
                redisConnectionString: builderConfiguration.GetConnectionString("RedisConnection") ??
                                       throw new InvalidOperationException(),
                name: "redis",
                failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
                tags: ["db", "cache", "redis"]
            )
            //.AddNpgSql(builderConfiguration.GetConnectionString("Practice01StartupContextConnection") ??
            //           throw new InvalidOperationException(
            //               "Connection string 'Practice01StartupContextConnection' not found."), name: "postgres",
            //    failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
            //    tags: ["db", "sql"])
            ;

        // For microservices
        // services.AddHealthChecksUI(
        //         options =>
        //     {
        //         // options.SetEvaluationTimeInSeconds(10); // tần suất check lại
        //         // options.MaximumHistoryEntriesPerEndpoint(50); // lưu history
        //         // options.AddHealthCheckEndpoint("Basic HealthCheck", "/health"); // endpoint gốc
        //     })
        //     .AddInMemoryStorage();

        services.AddSingleton<HttpRequestMetricsMiddleware>();
        services.AddSingleton<MyAppEventSource>();
        services.AddHttpLogging(logging =>
        {
            logging.LoggingFields = HttpLoggingFields.All;
            logging.RequestHeaders.Add("sec-ch-ua");
            logging.ResponseHeaders.Add("MyResponseHeader");
            logging.MediaTypeOptions.AddText("application/javascript");
            logging.RequestBodyLogLimit = 4096;
            logging.ResponseBodyLogLimit = 4096;
            logging.CombineLogs = true;
        });
        services.AddHttpLoggingInterceptor<SampleHttpLoggingInterceptor>();
    }
}