using act_ms_consolidation.Application.Interfaces;
using act_ms_consolidation.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace act_ms_consolidation.Application.Services
{
    public class ConsolidationCacheHandler : IConsolidationCacheHandler
    {
        private readonly IConsolidationCacheRepository _cacheRepository;
        private readonly ILogger<ConsolidationCacheHandler> _logger;
        private const string CachePrefix = "consolidation_";

        public ConsolidationCacheHandler(IConsolidationCacheRepository cacheRepository, ILogger<ConsolidationCacheHandler> logger)
        {
            _cacheRepository = cacheRepository;
            _logger = logger;
        }

        public async Task HandleConsolidationCacheAsync(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                _logger.LogWarning("Received invalid date for cache invalidation.");
                return;
            }

            string cacheKey = $"{CachePrefix}{date}";

            _logger.LogInformation($"Invalidating cache for key: {cacheKey}");

            try
            {
                await _cacheRepository.RemoveAsync(cacheKey);
                _logger.LogInformation($"Cache successfully invalidated for key: {cacheKey}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while invalidating cache for key: {cacheKey}");
            }
        }
    }
}
