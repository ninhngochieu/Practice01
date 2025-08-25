namespace Practice01.Application.Common.Cache;

public interface IRedisCacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value);
    Task RemoveAsync(string key);
    Task<bool> KeyExistsAsync(string key);
    Task<bool> KeyDeleteAsync(string key);
    Task<bool> KeyExpireAsync(string key, TimeSpan expiration);
    Task<bool> KeyPersistAsync(string key);
}