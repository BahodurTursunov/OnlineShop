using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using ServerLibrary.Services.Contracts.Cache;
using System.Text.Json;

namespace ServerLibrary.Services.Implementations.Cache
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        //private readonly ILogger<CacheService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public CacheService(IDistributedCache cache, ILogger<CacheService> logger)
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
            var cached = await _cache.GetStringAsync(key, cancellationToken);
            return string.IsNullOrWhiteSpace(cached)
                ? default
                : JsonSerializer.Deserialize<T>(cached, _jsonOptions);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken)
        {
            await _cache.RemoveAsync(key, cancellationToken);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration, CancellationToken cancellationToken)
        {
            var serialized = JsonSerializer.Serialize(value, _jsonOptions);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(30)
            };

            await _cache.SetStringAsync(key, serialized, options, cancellationToken);
        }
    }
}
