using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using HealthChecks.UI.Client;
using KafkaFlow;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Practice01.Application;
using Practice01.Infrastructure;
using Practice01.Infrastructure.Data;
using Practice01.Infrastructure.Data.Ef;
using Practice01.Infrastructure.Loggers;
using Practice01.Presentation;
using Practice01.Presentation.Middleware;
using Serilog;
using Serilog.Formatting.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPresentation(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithProcessId()
    .Enrich.WithThreadId()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} " +
        "RequestId={RequestId} TraceId={TraceId} {NewLine}{Exception}")
    .WriteTo.File(
        "Logs/log-.txt",                // thư mục & file (nên để log- vì có rollingInterval)
        rollingInterval: RollingInterval.Day, // tách file theo ngày
        retainedFileCountLimit: 7,      // giữ lại 7 ngày log
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj} " +
                        "RequestId={RequestId} TraceId={TraceId}{NewLine}{Exception}"
    )
    .WriteTo.Kafka(
        bootstrapServers: "kafka:9092",
        topic: "logs.app",
        formatter: new JsonFormatter(renderMessage: true))
    .CreateLogger();
builder.Host.UseSerilog();


builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxConcurrentConnections = 1000;
    options.Limits.MaxConcurrentUpgradedConnections = 1000;
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10MB
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
    options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);
});


var app = builder.Build();

// app.UseMiddleware<ApiKeyMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<SerilogEnrichMiddleware>();
app.UseMiddleware<HttpRequestMetricsMiddleware>();
app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<Practice01StartupContext>();
        await db.Database.MigrateAsync();
    }
    
    // Swagger
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var desc in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", desc.GroupName.ToUpperInvariant());
        }
    });
}

// Todo
// Setup health check postgres, mongo, redis, elasticsearch, kafka

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// For microservices
// app.MapHealthChecksUI(options =>
// {
//     options.UIPath = "/health-check-ui";
// })
//     .AllowAnonymous()
//     ;


var kafkaBus = app.Services.CreateKafkaBus();
await kafkaBus.StartAsync();

await app.RunAsync();

app.Run();