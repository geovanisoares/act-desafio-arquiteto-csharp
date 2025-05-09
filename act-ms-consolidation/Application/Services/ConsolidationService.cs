using act_ms_consolidation.Api.DTOs;
using act_ms_consolidation.Application.Interfaces;
using act_ms_consolidation.Domain.Interfaces;

namespace act_ms_consolidation.Application.Services
{
    public class ConsolidationService : IConsolidationService
    {
        private readonly IConsolidationRepository _consolidationRepository;
        private readonly IConsolidationCacheRepository _cacheRepository;
        private const string CachePrefix = "consolidation_";

        public ConsolidationService(IConsolidationRepository consolidationRepository, IConsolidationCacheRepository cacheRepository)
        {
            _consolidationRepository = consolidationRepository;
            _cacheRepository = cacheRepository;
        }

        public async Task<ConsolidationResponse> GetDailyConsolidationAsync(string date)
        {
            string cacheKey = $"{CachePrefix}{date}";
            var cachedData = await _cacheRepository.GetAsync<ConsolidationResponse>(cacheKey);

            if (cachedData != null)
            {
                Console.WriteLine($"Cache hit for {date}");
                return cachedData;
            }

            Console.WriteLine($"Cache miss for {date}");
            var consolidation = await _consolidationRepository.GetDailyConsolidationAsync(date);

            var response = new ConsolidationResponse
            {
                Date = consolidation.Date,
                TotalIncome = consolidation.TotalIncome,
                TotalExpense = consolidation.TotalExpense,
                Balance = consolidation.Balance
            };

            await _cacheRepository.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));

            return response;
        }
    }
}
