using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Practice01.Application.Common.Validation;
using Practice01.Presentation.Common.ObjectResult;

namespace Practice01.Presentation.Middleware;

public class GlobalExceptionMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly ErrorCollector _errorCollector;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger,
        ErrorCollector errorCollector,
        [FromKeyedServices("ApiResponseJsonSerializerOptions")]
        JsonSerializerOptions jsonSerializerOptions)
    {
        _logger = logger;
        _errorCollector = errorCollector;
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (exception is FluentValidation.ValidationException)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var validationApiResponse = new ApiResponse
            {
                Ok = false,
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "Validation Error",
                Code = "VALIDATION_ERROR",
                Details = _errorCollector.Details,
                Metadata = new Metadata
                {
                    RequestId = context.TraceIdentifier,
                    TraceId = Activity.Current?.Id ?? string.Empty
                }
            };

            var validationResult = JsonSerializer.Serialize(validationApiResponse, _jsonSerializerOptions);
            context.Response.WriteAsync(validationResult);
            _logger.LogError("Validation Error: {validationResult}", validationResult);
            return Task.CompletedTask;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        var apiResponse = new ApiResponse
        {
            Ok = false,
            StatusCode = (int)HttpStatusCode.InternalServerError,
            Message = exception.Message,
            Code = "INTERNAL_SERVER_ERROR",
            Metadata = new Metadata
            {
                RequestId = context.TraceIdentifier,
                TraceId = Activity.Current?.Id ?? string.Empty
            }
        };

        var result = JsonSerializer.Serialize(apiResponse, _jsonSerializerOptions);
        context.Response.WriteAsync(result);
        _logger.LogError("Error: {result}", result);
        return Task.CompletedTask;
    }
}
