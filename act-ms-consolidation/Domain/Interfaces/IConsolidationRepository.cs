using act_ms_consolidation.Domain.Entities;

namespace act_ms_consolidation.Domain.Interfaces
{
    public interface IConsolidationRepository
    {
        Task<ConsolidationEntity> GetDailyConsolidationAsync(string date);
    }
}
