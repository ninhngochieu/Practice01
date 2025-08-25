using Polly.CircuitBreaker;
using Polly.Retry;

namespace Practice01.Application.Common.Data;

public interface IResiliencePolicy
{
    public AsyncRetryPolicy CreateRetryPolicy();
    public AsyncCircuitBreakerPolicy CreateCircuitBreakerPolicy();
}