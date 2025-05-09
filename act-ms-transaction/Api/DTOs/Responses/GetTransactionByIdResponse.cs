using act_ms_transaction.Api.DTOs.Parameters;

namespace act_ms_transaction.Api.DTOs.Responses
{
    public record GetTransactionByIdResponse
    {
        public Guid Id { get; set; }
        public string Date { get; set; }
        public char Type { get; set; } 
        public decimal Value { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
    }
}
