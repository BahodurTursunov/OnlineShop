namespace ServerLibrary.Services.Contracts.Cache
{
    public interface IRedisCacheService
    {
        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
        Task<T?> SetAsync<T>(string key, T value, TimeSpan? lifeTime, CancellationToken cancellationToken = default);
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    }
}
