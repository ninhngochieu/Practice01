using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Practice01.Presentation.Middleware;

public class SerilogEnrichMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // RequestId (tự sinh nếu chưa có)
        var requestId = context.TraceIdentifier;

        // TraceId (từ Activity.Current hoặc HttpContext nếu có)
        var traceId = System.Diagnostics.Activity.Current?.TraceId.ToString() 
                      ?? context.TraceIdentifier;

        using (LogContext.PushProperty("RequestId", requestId))
        using (LogContext.PushProperty("TraceId", traceId))
        {
            await next(context);
        }
    }
}