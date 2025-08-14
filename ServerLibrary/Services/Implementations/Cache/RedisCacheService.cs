// ServerLibrary/Services/Implementations/Cache/RedisCacheService.cs
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using ServerLibrary.Services.Contracts.Cache;
using System.Text.Json;

namespace ServerLibrary.Services.Implementations.Cache
{
    public class RedisCacheService<T> : IRedisCacheService<T> where T : class
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisCacheService<T>> _logger;
        private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };

        public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService<T>> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var json = await _cache.GetStringAsync(key, cancellationToken);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return default;
                }

                return JsonSerializer.Deserialize<T>(json, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cache GET error for key '{Key}'", key);
                return default;
            }
        }

        public async Task<T> SetAsync(string key, T value, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var json = JsonSerializer.Serialize(value, _jsonOptions);
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
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cache SET error for key '{Key}'", key);
                return default;
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
                _logger.LogError(ex, "Cache REMOVE error for key '{Key}'", key);
            }
        }
    }
}
