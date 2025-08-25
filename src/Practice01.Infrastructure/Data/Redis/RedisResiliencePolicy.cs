using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Practice01.Application.Common.Data;
using StackExchange.Redis;
using System;

namespace Practice01.Infrastructure.Data.Redis
{ public class RedisResiliencePolicy : IResiliencePolicy
    {
        private readonly ILogger<RedisResiliencePolicy> _logger;

        public RedisResiliencePolicy(ILogger<RedisResiliencePolicy> logger)
        {
            _logger = logger;
        }

        // Retry policy: thử lại 3 lần, backoff 100ms, 200ms, 400ms
        public AsyncRetryPolicy CreateRetryPolicy() =>
            Policy.Handle<RedisException>(IsTransient)
                  .WaitAndRetryAsync(
                      retryCount: 3,
                      sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt - 1)),
                      onRetry: (exception, timeSpan, retryCount, context) =>
                      {
                          _logger.LogWarning(exception,
                              "Redis Retry {RetryCount} after {Delay}ms due to transient error: {Message}",
                              retryCount,
                              timeSpan.TotalMilliseconds,
                              exception.Message);
                      }
                  );

        // Circuit breaker: ngắt khi 10 errors liên tiếp, giữ 10s trước half-open
        public AsyncCircuitBreakerPolicy CreateCircuitBreakerPolicy() =>
            Policy.Handle<RedisException>(IsTransient)
                  .CircuitBreakerAsync(
                      exceptionsAllowedBeforeBreaking: 10,
                      durationOfBreak: TimeSpan.FromSeconds(10),
                      onBreak: (ex, breakDelay) =>
                      {
                          _logger.LogWarning(ex,
                              "Redis Circuit broken for {BreakSeconds}s due to: {Message}",
                              breakDelay.TotalSeconds,
                              ex.Message);
                      },
                      onReset: () =>
                      {
                          _logger.LogInformation("Redis Circuit reset.");
                      }
                  );

        // Xác định lỗi tạm thời của Redis
        private static bool IsTransient(RedisException ex)
        {
            // Các lỗi connect timeout, socket failure, tức thời
            return ex is RedisConnectionException
                || ex.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase)
                || ex.Message.Contains("busy", StringComparison.OrdinalIgnoreCase);
        }
    }
}
