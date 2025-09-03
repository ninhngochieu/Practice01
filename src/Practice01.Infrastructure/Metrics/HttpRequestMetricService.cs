using Microsoft.AspNetCore.Http;
using Practice01.Application.Common.Metrics;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice01.Infrastructure.Metrics;
public class HttpRequestMetricService : IHttpRequestMetricService
{
    private Meter _httpMeter;
    private Counter<int> _httpRequests;
    private Histogram<double> _httpRequestDuration;

    public HttpRequestMetricService(IMeterFactory meterFactory)
    {
        _httpMeter = meterFactory.Create("Practice01.Presentation", "1.0");
        _httpRequests = _httpMeter.CreateCounter<int>(
            "http_requests_total", "Total HTTP requests");
        _httpRequestDuration = _httpMeter.CreateHistogram<double>(
            "http_request_duration_ms", "HTTP request duration in milliseconds");
    }

    public void IncrementHttpRequest(string endpoint, string statusCode)
    {
        _httpRequests.Add(1,
            new KeyValuePair<string, object>("endpoint", endpoint),
            new KeyValuePair<string, object>("status_code", statusCode));
    }

    public void RecordHttpRequestDuration(string endpoint, double durationMs)
    {
        _httpRequestDuration.Record(durationMs,
            new KeyValuePair<string, object>("endpoint", endpoint));
    }
}
