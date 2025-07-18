// Path: ServerLibrary/Services/Implementations/CacheService.cs
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using ServerLibrary.Services.Contracts.Cache;
using System.Text.Json;

namespace ServerLibrary.Services.Implementations.Cache
{
    public class RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger) : IRedisCacheService<T>
    {
        private readonly IDistributedCache _cache = cache;
        private readonly ILogger<RedisCacheService> _logger = logger;

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var json = await _cache.GetStringAsync(key, cancellationToken);
                if (string.IsNullOrEmpty(json))
                {
                    return default;
                }
                return json is null ? default : JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while getting cache for key '{key}");
                return default;
            }
        }

        public async Task<T> SetAsync(string key, T value, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var json = JsonSerializer.Serialize(value);
                var options = new DistributedCacheEntryOptions();

                if (absoluteExpiration.HasValue)
                {
                    options.SetAbsoluteExpiration(absoluteExpiration.Value);
                }

                if (slidingExpiration.HasValue)
                {
                    options.SetSlidingExpiration(slidingExpiration.Value);
                }

                await _cache.SetStringAsync(key, json, options, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while setting cache for key '{key}'");
            }

        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                await _cache.RemoveAsync(key, cancellationToken);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while removing cache for key '{key}'");
            }
        }


    }
}
