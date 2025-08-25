using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Practice01.Application.Common.Data;

namespace Practice01.Infrastructure.Data.Ef;

public class PostgresResiliencePolicy : IResiliencePolicy
{
    private readonly ILogger<PostgresResiliencePolicy> _logger;

    public PostgresResiliencePolicy(ILogger<PostgresResiliencePolicy> logger)
    {
        _logger = logger;
    }

    // Retry policy: thử lại 3 lần, tăng dần delay 200ms, 400ms, 800ms, với logging
    public AsyncRetryPolicy CreateRetryPolicy() =>
        Policy.Handle<Exception>(IsTransient)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(200 * Math.Pow(2, attempt - 1)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(exception,
                        "Retry {RetryCount} after {Delay}ms due to transient error: {Message}",
                        retryCount,
                        timeSpan.TotalMilliseconds,
                        exception.Message);
                }
            );

    // Circuit breaker: ngắt khi có 5 lỗi liên tiếp, giữ 30s trước khi half-open
    public AsyncCircuitBreakerPolicy CreateCircuitBreakerPolicy() =>
        Policy.Handle<Exception>(IsTransient)
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(15),
                onBreak: (ex, breakDelay) =>
                {
                    // Log khi breaker active
                    // Console.WriteLine($"Circuit broken for {breakDelay.TotalSeconds}s due to: {ex.Message}");
                    _logger.LogWarning($"Circuit broken for {breakDelay.TotalSeconds}s due to: {ex.Message}");
                },
                onReset: () =>
                {
                    // Log khi breaker reset
                    // Console.WriteLine("Circuit reset.");
                    _logger.LogWarning("Circuit reset.");
                }
            );

    // Xác định lỗi tạm thời (Transient) của PostgreSQL
    private static bool IsTransient(Exception ex)
    {
        // Ví dụ kiểm tra NpgsqlException có SQLState thuộc nhóm retryable
        if (ex is Npgsql.NpgsqlException npgEx)
        {
            // 40001 = serialization_failure, 55P03 = lock_not_available
            return new[] { "40001", "55P03" }.Contains(npgEx.SqlState);
        }

        return false;
    }
}