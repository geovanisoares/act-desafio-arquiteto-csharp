namespace act_ms_consolidation.Domain.Entities
{
    public class ConsolidationEntity
    {
        public string Date { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Balance { get; set; }
    }
}
