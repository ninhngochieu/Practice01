using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Practice01.Application.Common.Data;

namespace Practice01.Infrastructure.Data.MongoDb
{
    public class MongoResiliencePolicy : IResiliencePolicy
    {
        private readonly ILogger<MongoResiliencePolicy> _logger;

        public MongoResiliencePolicy(ILogger<MongoResiliencePolicy> logger)
        {
            _logger = logger;
        }

        // Retry policy: thử lại 3 lần, backoff 200ms, 400ms, 800ms, với logging
        public AsyncRetryPolicy CreateRetryPolicy() =>
            Policy.Handle<MongoException>(IsTransient)
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(200 * Math.Pow(2, attempt - 1)),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning(exception,
                            "MongoDB Retry {RetryCount} after {Delay}ms due to transient error: {Message}",
                            retryCount,
                            timeSpan.TotalMilliseconds,
                            exception.Message);
                    }
                );

        // Circuit breaker: ngắt khi 7 lỗi liên tiếp, giữ 15s trước half-open
        public AsyncCircuitBreakerPolicy CreateCircuitBreakerPolicy() =>
            Policy.Handle<MongoException>(IsTransient)
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 7,
                    durationOfBreak: TimeSpan.FromSeconds(15),
                    onBreak: (ex, breakDelay) =>
                    {
                        _logger.LogWarning(ex,
                            "MongoDB Circuit broken for {BreakSeconds}s due to: {Message}",
                            breakDelay.TotalSeconds,
                            ex.Message);
                    },
                    onReset: () => { _logger.LogInformation("MongoDB Circuit reset."); }
                );

        // Xác định lỗi tạm thời của MongoDB
        private static bool IsTransient(MongoException ex)
        {
            // Các lỗi tạm thời: kết nối, timeout, duplicate key không retry
            if (ex is MongoConnectionException)
            {
                return true;
            }

            if (ex is MongoExecutionTimeoutException)
            {
                return true;
            }

            // Lỗi transaction có label retryable
            if (ex.HasErrorLabel("TransientTransactionError"))
            {
                return true;
            }

            return false;
        }
    }
}