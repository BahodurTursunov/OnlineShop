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
            var cached = await _cache.GetStringAsync(key, cancellationToken);

            if (string.IsNullOrWhiteSpace(cached))
            {
                return default;
            }

            try
            {
                return JsonSerializer.Deserialize<T>(cached);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize cache entry for key: {Key}", key);
                return default;
            }
        }

        public async Task SetAsync<T>(
            string key,
            T value,
            TimeSpan? absoluteExpireTime = null,
            TimeSpan? slidingExpireTime = null,
            CancellationToken cancellationToken = default)
        {
            var options = new DistributedCacheEntryOptions();

            if (absoluteExpireTime.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = absoluteExpireTime;
            }

            if (slidingExpireTime.HasValue)
            {
                options.SlidingExpiration = slidingExpireTime;
            }

            var json = JsonSerializer.Serialize(value);

            await _cache.SetStringAsync(key, json, options, cancellationToken);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            await _cache.RemoveAsync(key, cancellationToken);
        }
    }
}
