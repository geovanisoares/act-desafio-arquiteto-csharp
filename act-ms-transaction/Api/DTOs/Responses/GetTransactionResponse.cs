namespace act_ms_transaction.Api.DTOs.Responses
{
    public record GetTransactionResponse
    {
        public Guid Id { get; set; }
        public string Date { get; set; }  // Mantém o formato string para facilitar a visualização na API
        public decimal Value { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
