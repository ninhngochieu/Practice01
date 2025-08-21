using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Practice01.Application;
using Practice01.Infrastructure;
using Practice01.Presentation;
using Practice01.Presentation.Middleware;
using Serilog;

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
app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();

app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        context.Response.Headers["RateLimit-Limit"] = "100";
        context.Response.Headers["RateLimit-Remaining"] = "95";
        context.Response.Headers["RateLimit-Reset"] = DateTimeOffset.UtcNow.AddSeconds(60).ToUnixTimeSeconds().ToString();
        return Task.CompletedTask;
    });
    await next();
});

app.Run();