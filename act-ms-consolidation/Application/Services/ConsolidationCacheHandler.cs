using act_ms_consolidation.Application.Interfaces;
using act_ms_consolidation.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace act_ms_consolidation.Application.Services
{
    public class ConsolidationCacheHandler : IConsolidationCacheHandler
    {
        private readonly IConsolidationCacheRepository _cacheRepository;
        private const string CachePrefix = "consolidation_";

        public ConsolidationCacheHandler(IConsolidationCacheRepository cacheRepository)
        {
            _cacheRepository = cacheRepository;
        }

        public async Task HandleConsolidationCacheAsync(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                Console.WriteLine("Invalid date received for cache invalidation.");
                return;
            }

            string cacheKey = $"{CachePrefix}{date}";

            Console.WriteLine($"Invalidating cache for key: {cacheKey}");
            await _cacheRepository.RemoveAsync(cacheKey);
        }
    }
}
