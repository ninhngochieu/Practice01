using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Wrap;
using Practice01.Application.Common.Cache;
using Practice01.Application.Common.Data;
using StackExchange.Redis;

namespace Practice01.Infrastructure.Services;

public class RedisCacheService : IRedisCacheService
{
    private readonly IDatabase _database;
    private readonly AsyncPolicyWrap _resiliencePolicy;

    public RedisCacheService(IConnectionMultiplexer connectionMultiplexer,
        [FromKeyedServices("RedisResiliencePolicy")]
        IResiliencePolicy resiliencePolicy)
    {
        _database = connectionMultiplexer.GetDatabase();
        _resiliencePolicy = Policy.WrapAsync(
            resiliencePolicy.CreateRetryPolicy(),
            resiliencePolicy.CreateCircuitBreakerPolicy()
        );
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        return await _resiliencePolicy.ExecuteAsync(async () =>
        {
            var result = await _database.StringGetAsync(key);
            if (result.IsNull)
            {
                return default;
            }

            return result.HasValue ? JsonSerializer.Deserialize<T>(result) : default;
        });
    }

    public Task SetAsync<T>(string key, T value)
    {
        return _resiliencePolicy.ExecuteAsync(async () =>
        {
            var json = JsonSerializer.Serialize(value);
            await _database.StringSetAsync(key, json);
        });
    }

    public Task RemoveAsync(string key)
    {
        return _resiliencePolicy.ExecuteAsync(async () => { await _database.KeyDeleteAsync(key); });
    }

    public Task<bool> KeyExistsAsync(string key)
    {
        return _resiliencePolicy.ExecuteAsync(async () => await _database.KeyExistsAsync(key));
    }

    public Task<bool> KeyDeleteAsync(string key)
    {
        return _resiliencePolicy.ExecuteAsync(async () => await _database.KeyDeleteAsync(key));
    }

    public Task<bool> KeyExpireAsync(string key, TimeSpan expiration)
    {
        return _resiliencePolicy.ExecuteAsync(async () => await _database.KeyExpireAsync(key, expiration));
    }

    public Task<bool> KeyPersistAsync(string key)
    {
        return _resiliencePolicy.ExecuteAsync(async () => await _database.KeyPersistAsync(key));
    }
}