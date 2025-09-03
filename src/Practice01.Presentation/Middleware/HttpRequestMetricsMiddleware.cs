using Microsoft.AspNetCore.Http;
using Practice01.Application.Common.Metrics;
using System.Diagnostics;

namespace Practice01.Presentation.Middleware;

public class HttpRequestMetricsMiddleware : IMiddleware
{
    private readonly IHttpRequestMetricService httpRequestMetricService;

    public HttpRequestMetricsMiddleware(IHttpRequestMetricService httpRequestMetricService)
    {
        this.httpRequestMetricService = httpRequestMetricService;
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            await next(context);
        }
        finally
        {
            sw.Stop();
            var endpoint = context.Request.Path.ToString();
            var statusCode = context.Response.StatusCode.ToString();

            httpRequestMetricService.IncrementHttpRequest(endpoint, statusCode);
            httpRequestMetricService.RecordHttpRequestDuration(endpoint, sw.Elapsed.TotalMilliseconds);
        }
    }
}