// Path: ServerLibrary/Services/Implementations/CacheService.cs
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using ServerLibrary.Services.Contracts.Cache;
using System.Text.Json;

namespace ServerLibrary.Services.Implementations.Cache
{
    public class RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger) : IRedisCacheService
    {
        private readonly IDistributedCache _cache = cache;
        private readonly ILogger<RedisCacheService> _logger = logger;

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var json = await _cache.GetStringAsync(key, cancellationToken);
            return json is null ? default : JsonSerializer.Deserialize<T>(json);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? lifeTime, CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(value);

            await _cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = lifeTime
            }, cancellationToken);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            await _cache.RemoveAsync(key, cancellationToken);
        }
    }
}
