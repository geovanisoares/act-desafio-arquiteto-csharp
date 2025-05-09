namespace act_ms_consolidation.Api.DTOs
{
    public record ConsolidationResponse
    {
        public string Date { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Balance { get; set; }
    }
}
