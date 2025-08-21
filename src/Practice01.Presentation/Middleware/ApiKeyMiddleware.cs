using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Practice01.Presentation.Common.ObjectResult;

namespace Practice01.Presentation.Middleware;

public class ApiKeyMiddleware : IMiddleware
{
    private readonly ILogger<ApiKeyMiddleware> _logger;
    private readonly IConfiguration _configuration;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public ApiKeyMiddleware(ILogger<ApiKeyMiddleware> logger, 
        IConfiguration configuration,         
        [FromKeyedServices("ApiResponseJsonSerializerOptions")]
        JsonSerializerOptions jsonSerializerOptions)
    {
        _logger = logger;
        _configuration = configuration;
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Lấy API Key từ Header
        if (!context.Request.Headers.TryGetValue("X-API-KEY", out var extractedApiKey))
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            var validationApiResponse = new ApiResponse
            {
                Ok = false,
                StatusCode = (int)HttpStatusCode.Unauthorized,
                Message = "Failed to extract API Key",
                Code = "FAILED_TO_EXTRACT_API_KEY",
                Details = [],
                Metadata = new Metadata
                {
                    RequestId = context.TraceIdentifier,
                    TraceId = Activity.Current?.Id ?? string.Empty
                }
            };

            var extractApiKeyResult = JsonSerializer.Serialize(validationApiResponse, _jsonSerializerOptions);
            await context.Response.WriteAsync(extractApiKeyResult);
            _logger.LogError("Failed to extract API Key: {extractApiKeyResult}", extractApiKeyResult);
            return;
        }

        // Lấy list API key hợp lệ từ appsettings
        var validApiKeys = _configuration.GetSection("ApiKeys").GetChildren()
            .Select(x => x.Value)
            .ToList();

        // Kiểm tra
        if (!validApiKeys.Contains(extractedApiKey))
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            var validationApiResponse = new ApiResponse
            {
                Ok = false,
                StatusCode = (int)HttpStatusCode.Unauthorized,
                Message = "Invalid API Key",
                Code = "INVALID_API_KEY",
                Details = [],
                Metadata = new Metadata
                {
                    RequestId = context.TraceIdentifier,
                    TraceId = Activity.Current?.Id ?? string.Empty
                }
            };

            var invalidApiKeyResult = JsonSerializer.Serialize(validationApiResponse, _jsonSerializerOptions);
            await context.Response.WriteAsync(invalidApiKeyResult);
            _logger.LogError("Invalid API Key: {extractApiKeyResult}", invalidApiKeyResult);
            return;
        }

        // Nếu hợp lệ → tiếp tục
        await next(context);
    }
}