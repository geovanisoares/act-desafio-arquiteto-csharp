namespace act_ms_consolidation.Application.Interfaces
{
    public interface IConsolidationCacheHandler
    {
        Task HandleConsolidationCacheAsync(string date);
    }
}
