using System.Text.Json;
using Practice01.Application.Common.Cache;
using StackExchange.Redis;

namespace Practice01.Infrastructure.Services;

public class RedisCacheService : IRedisCacheService
{
    private readonly IDatabase _database;

    public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var result = await _database.StringGetAsync(key);
        if (result.IsNull)
        {
            return default;
        }
        return result.HasValue ? JsonSerializer.Deserialize<T>(result) : default;
    }

    public Task SetAsync<T>(string key, T value)
    {
        var json = JsonSerializer.Serialize(value);
        return _database.StringSetAsync(key, json);
    }

    public Task RemoveAsync(string key)
    {
        return _database.KeyDeleteAsync(key);
    }
    
    public Task<bool> KeyExistsAsync(string key)
    {
        return _database.KeyExistsAsync(key);
    }

    public Task<bool> KeyDeleteAsync(string key)
    {
        return _database.KeyDeleteAsync(key);
    }

    public Task<bool> KeyExpireAsync(string key, TimeSpan expiration)
    {
        return _database.KeyExpireAsync(key, expiration);
    }

    public Task<bool> KeyPersistAsync(string key)
    {
        return _database.KeyPersistAsync(key);
    }
}