namespace act_ms_consolidation.Domain.Interfaces
{
    public interface IConsolidationCacheRepository
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan expiration);
        Task RemoveAsync(string key);
    }
}
