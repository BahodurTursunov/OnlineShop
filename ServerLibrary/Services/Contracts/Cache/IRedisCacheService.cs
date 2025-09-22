namespace ServerLibrary.Services.Contracts.Cache
{
    public interface IRedisCacheService<T> where T : class
    {
        Task<T?> GetAsync(string key, CancellationToken cancellationToken = default);
        Task<bool> SetAsync(string key, T value, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null, CancellationToken cancellationToken = default);
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    }
}
