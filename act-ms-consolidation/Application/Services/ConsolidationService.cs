using act_ms_consolidation.Api.DTOs;
using act_ms_consolidation.Application.Exceptions;
using act_ms_consolidation.Application.Interfaces;
using act_ms_consolidation.Domain.Interfaces;

namespace act_ms_consolidation.Application.Services
{
    public class ConsolidationService : IConsolidationService
    {
        private readonly IConsolidationRepository _consolidationRepository;
        private readonly IConsolidationCacheRepository _cacheRepository;
        private readonly ILogger<ConsolidationService> _logger;
        private const string CachePrefix = "consolidation_";

        public ConsolidationService(IConsolidationRepository consolidationRepository, IConsolidationCacheRepository cacheRepository, ILogger<ConsolidationService> logger)
        {
            _consolidationRepository = consolidationRepository;
            _cacheRepository = cacheRepository;
            _logger = logger;
        }

        public async Task<ConsolidationResponse> GetDailyConsolidationAsync(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                _logger.LogWarning("Date field is required");
                throw new BusinessException("Date field is required");
            }

            if (!DateTime.TryParse(date, out _))
            {
                throw new BusinessException("Invalid date format. Use yyyy-MM-dd.");
            }

            string cacheKey = $"{CachePrefix}{date}";
            
            _logger.LogInformation($"Checking cache for date: {date}");

            var cachedData = await _cacheRepository.GetAsync<ConsolidationResponse>(cacheKey);

            if (cachedData != null)
            {
                _logger.LogInformation($"Cache hit for {date}");
                return cachedData;
            }

            _logger.LogInformation($"Cache miss for {date}. Retrieving data from repository.");

            var consolidation = await _consolidationRepository.GetDailyConsolidationAsync(date);

            if (consolidation == null)
            {
                _logger.LogWarning("No data found for date: {Date}", date);
            }

            var response = new ConsolidationResponse
            {
                Date = consolidation.Date,
                TotalIncome = consolidation.TotalIncome,
                TotalExpense = consolidation.TotalExpense,
                Balance = consolidation.Balance
            };

            _logger.LogInformation("Caching data for date: {Date} with key: {CacheKey}", date, cacheKey);
            await _cacheRepository.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));

            return response;
            
           
        }
    }
}
