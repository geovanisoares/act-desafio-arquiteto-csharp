using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace act_ms_transaction.Infrastructure.EFModels
{
    [Table("Transactions")]
    public class TransactionModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Column(TypeName = "varchar(10)")]
        public string Date { get; set; }

        [Required]
        [Column(TypeName = "char(1)")]
        public string Type { get; set; } // "I" ou "E"

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Value { get; set; }

        [Required]
        [Column(TypeName = "varchar(255)")]
        public string Description { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string UpdatedBy { get; set; }
    }
}
