using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice01.Application.Common.Metrics;
public interface IHttpRequestMetricService
{
    void IncrementHttpRequest(string endpoint, string statusCode);
    void RecordHttpRequestDuration(string endpoint, double totalMilliseconds);
}
