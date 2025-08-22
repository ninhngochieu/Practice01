using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Practice01.Presentation.Common.ObjectResult;

namespace Practice01.Presentation.Common.Handler;

public class CustomAuthorizationResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public CustomAuthorizationResultHandler(AuthorizationMiddlewareResultHandler defaultHandler,
        [FromKeyedServices("ApiResponseJsonSerializerOptions")]
        JsonSerializerOptions jsonSerializerOptions)
    {
        _defaultHandler = defaultHandler;
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Forbidden)
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";

            var result = JsonSerializer.Serialize(new ApiResponse()
            {
                Ok = false,
                StatusCode = 403,
                Message = "You don't have permission to access this resource",
                Code = "FORBIDDEN",
                Metadata = new Metadata
                {
                    RequestId = context.TraceIdentifier,
                    TraceId = Activity.Current?.Id ?? string.Empty
                }
            }, _jsonSerializerOptions);

            await context.Response.WriteAsync(result);
            return;
        }

        await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}