/*using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using ServerLibrary.Services.Contracts.RedisCache;
using System.Text.Json;

namespace ServerLibrary.Services.Implementations.Cache
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisCacheService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
        {
            _cache = cache;
            _jsonOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken)
        {
            var cachedData = await _cache.GetStringAsync(key, cancellationToken);

            if (cachedData == null)
            {
                _logger.LogDebug($"Cache miss for key: {key}");
                return default;
            }

            _logger.LogDebug($"Cache hit for key: {key}");
            return JsonSerializer.Deserialize<T>(cachedData, _jsonOptions);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken)
        {
            await _cache.RemoveAsync(key, cancellationToken);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration, CancellationToken cancellationToken)
        {
            try
            {
                var serialized = JsonSerializer.Serialize(value, _jsonOptions);
                await _cache.SetStringAsync(key, serialized, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
                }, cancellationToken);

                _logger.LogDebug($"Set cache for key: {key} with expiration: {expiration}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to cache key: {key}");
            }

        }
    }
}
*/