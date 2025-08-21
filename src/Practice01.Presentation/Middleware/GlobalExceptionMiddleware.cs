using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Practice01.Presentation.Middleware;

public class GlobalExceptionMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = exception switch
        {
            ArgumentNullException _ => HttpStatusCode.BadRequest,
            UnauthorizedAccessException _ => HttpStatusCode.Forbidden,
            KeyNotFoundException _ => HttpStatusCode.NotFound,
            _ => HttpStatusCode.InternalServerError
        };

        var result = JsonSerializer.Serialize(new
        {
            error = exception.Message,
            status = (int)code
        });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        return context.Response.WriteAsync(result);
    }
}