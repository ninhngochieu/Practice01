using DnsClient;
using Elastic.Clients.Elasticsearch;
using KafkaFlow;
using KafkaFlow.Serializer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Polly;
using Practice01.Application.Common.Cache;
using Practice01.Application.Common.Data;
using Practice01.Application.Common.File;
using Practice01.Application.Common.Metrics;
using Practice01.Application.Common.Producers;
using Practice01.Application.Common.Token;
using Practice01.Application.Common.User;
using Practice01.Domain.Entities.Books;
using Practice01.Domain.Entities.Users;
using Practice01.Infrastructure.Data.Ef;
using Practice01.Infrastructure.Data.MongoDb;
using Practice01.Infrastructure.Data.MongoDb.Books;
using Practice01.Infrastructure.Data.Redis;
using Practice01.Infrastructure.Kafkaflow.Consumers;
using Practice01.Infrastructure.Kafkaflow.Producers;
using Practice01.Infrastructure.Metrics;
using Practice01.Infrastructure.Provider;
using Practice01.Infrastructure.Services;
using Practice01.Infrastructure.Workers;
using Serilog;
using Serilog.Core;
using StackExchange.Redis;
using Role = Practice01.Domain.Entities.Roles.Role;

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
            options.UseSqlServer(pgConnectionString, builder =>
            {
                builder.EnableRetryOnFailure(5,TimeSpan.FromSeconds(10), null);
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
        services.AddScoped<IUserProvider, UserProvider>();

        var mongoDbConnectionString = builderConfiguration.GetConnectionString("MongoDbConnection");
        var mongoUrl = new MongoUrl(mongoDbConnectionString);
        var settings = MongoClientSettings.FromUrl(mongoUrl);
        settings.MaxConnectionPoolSize = 200;
        settings.MinConnectionPoolSize = 20;
        settings.ConnectTimeout = TimeSpan.FromSeconds(5);
        settings.SocketTimeout = TimeSpan.FromSeconds(30);
        settings.HeartbeatInterval = TimeSpan.FromSeconds(10);
        settings.RetryWrites = true;
        settings.RetryReads = true;
        settings.HeartbeatTimeout = TimeSpan.FromSeconds(10);
        settings.WaitQueueTimeout = TimeSpan.FromSeconds(2);

        services.AddSingleton<IMongoClient>(sp =>
        {
            var mongoClient = new MongoClient(settings);
            return mongoClient;
        });
        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var mongoClient = sp.GetRequiredService<IMongoClient>();
            var database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
            return database;
        });

        services.AddSingleton<IRedisCacheService, RedisCacheService>();
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<IBookRepository, BookRepository>();
        services.AddKeyedSingleton<IResiliencePolicy, MongoResiliencePolicy>("MongoResiliencePolicy");
        services.AddKeyedSingleton<IResiliencePolicy, PostgresResiliencePolicy>("PostgresResiliencePolicy");
        services.AddKeyedSingleton<IResiliencePolicy, RedisResiliencePolicy>("RedisResiliencePolicy");

        var kafkaBootstrapServers = builderConfiguration["Kafka:BootstrapServers"] ??
                                    throw new InvalidOperationException("Kafka:BootstrapServers not found.");
        const string producerName = "PrintConsole";
        const string topicName = "sample-topic";
        services.AddKafka(kafka => kafka
            .UseConsoleLog()
            .AddCluster(cluster => cluster
                .WithBrokers([kafkaBootstrapServers])
                .CreateTopicIfNotExists(topicName, 6, 1)
                .AddProducer(
                    producerName,
                    producer => producer
                        .DefaultTopic(topicName)
                        .AddMiddlewares(m => m.AddSerializer<NewtonsoftJsonSerializer>())
                )
                .AddConsumer(consumer => consumer
                    .Topic(topicName)
                    .WithGroupId("print-console-handler")
                    .WithBufferSize(100)
                    .WithWorkersCount(3)
                    .AddMiddlewares(middlewares => middlewares
                        .AddDeserializer<NewtonsoftJsonDeserializer>()
                        .AddTypedHandlers(h => h.AddHandler<PrintConsoleHandler>())
                    )
                )
            )
        );

        // services.AddHostedService<PrintConsoleWorker>();
        services.AddKeyedSingleton("VietnamTimeZoneInfo",
            TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
        services.AddSingleton<Practice01.Application.Common.Datetime.IDateTimeProvider, VietnameDateTimeProvider>();
        services.AddSingleton<ITestMessageProducer, TestMessageProducer>();
        services.AddHostedService<SerilogToKafkaLogWorker>();
        services.AddHostedService<KafkaSerilogToElasticSearchWorker>();
        services.AddSingleton<ElasticsearchClient>(sp =>
        {
            var elasticSettings =
                new ElasticsearchClientSettings(new Uri(builderConfiguration["Elasticsearch:Url"] ??
                                                        throw new InvalidOperationException()));
            return new ElasticsearchClient(elasticSettings);
        });
        services.AddHttpClient("TestApi",
                client => { client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com"); })
            .AddStandardResilienceHandler(options =>
            {
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(10);

                // Retry
                options.Retry.MaxRetryAttempts = 3;
                options.Retry.BackoffType = DelayBackoffType.Constant;
                options.Retry.Delay = TimeSpan.FromMilliseconds(500);
                options.Retry.UseJitter = false;
                options.Retry.OnRetry = args =>
                {
                    Log.Warning("Retry {Attempt}: {Reason}",
                        args.AttemptNumber,
                        args.Outcome.Exception?.Message ?? "unknown");
                    return ValueTask.CompletedTask;
                };

                // Circuit Breaker
                options.CircuitBreaker.FailureRatio = 0.8;
                options.CircuitBreaker.MinimumThroughput = 5;
                options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
                options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(10);

                options.CircuitBreaker.OnOpened = args =>
                {
                    Log.Error("Circuit opened due to failures");
                    return ValueTask.CompletedTask;
                };
                options.CircuitBreaker.OnClosed = args =>
                {
                    Log.Information("Circuit closed, normal operation resumed");
                    return ValueTask.CompletedTask;
                };
                options.CircuitBreaker.OnHalfOpened = args =>
                {
                    Log.Warning("Circuit half-opened, testing requests");
                    return ValueTask.CompletedTask;
                };
            });
        
        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(serviceName: "Practice01", serviceVersion: "1.0.0");
        services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService("MyService"))
            .WithTracing(tracing =>
            {
                tracing
                    .SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation()
                    //.AddHttpClientInstrumentation()
                    .AddSource("Practice01.Presentation")
                    .AddConsoleExporter()
                    //.AddOtlpExporter(options =>
                    //{
                    //    options.Endpoint = new Uri("http://otel-collector:4317");
                    //})
                    ;
                // gửi trace qua OTLP
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation()
                    //.AddHttpClientInstrumentation()
                    .AddMeter("Practice01.Presentation")
                    .AddConsoleExporter()
                    //.AddOtlpExporter(options =>
                    //{
                    //    options.Endpoint = new Uri("http://otel-collector:4317");
                    //})
                    ;
            });
        services.AddSingleton<IHttpRequestMetricService, HttpRequestMetricService>();
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
    }
}