using act_ms_consolidation.Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace act_ms_consolidation.Infrastructure.Repositories
{
    public class ConsolidationCacheRepository: IConsolidationCacheRepository
    {
        private readonly IDistributedCache _cache;

        public ConsolidationCacheRepository(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var cachedData = await _cache.GetStringAsync(key);
            if (cachedData == null) return default;
            return JsonConvert.DeserializeObject<T>(cachedData);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            var serializedData = JsonConvert.SerializeObject(value);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };
            await _cache.SetStringAsync(key, serializedData, options);
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}
