using act_ms_transaction.Domain.Enums;

namespace act_ms_transaction.Domain.Entities
{
    public class TransactionEntity
    { 
        public Guid Id { get; set; }
        public DateTime Date { get; set; } 
        public TransactionType Type { get; set; }
        public decimal Value { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }

    }
}
