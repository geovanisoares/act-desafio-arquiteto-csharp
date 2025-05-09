using act_ms_consolidation.Api.DTOs;

namespace act_ms_consolidation.Application.Interfaces
{
    public interface IConsolidationService
    {
        Task<ConsolidationResponse> GetDailyConsolidationAsync(string date);
    }
}
