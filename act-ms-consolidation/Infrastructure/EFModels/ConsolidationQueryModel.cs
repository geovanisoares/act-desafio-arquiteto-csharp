namespace act_ms_consolidation.Infrastructure.EFModels
{
    public class ConsolidationQueryModel
    {
        public string Date { get; set; }
        public string Type { get; set; } // "I" ou "E"
        public decimal Value { get; set; }
    }
}
