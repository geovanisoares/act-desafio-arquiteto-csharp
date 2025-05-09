using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using act_ms_transaction.Api.DTOs.Parameters;
using act_ms_transaction.Api.Validators;
using act_ms_transaction.Domain.Enums;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace act_ms_transaction.Api.DTOs.Requests
{
    public record CreateTransactionRequest
    {
        [Required]
        [SwaggerSchema("Data da transação")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        [DefaultValue("2025-05-07")]
        public string Date { get; set; }

        [Required]
        [TransactionTypeValidator]
        [SwaggerSchema("Tipo da transação ('I' para Income, 'E' para Expense)")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        [DefaultValue('I')]
        public char Type { get; set; }

        [Required]
        [SwaggerSchema("Valor da transação")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        [DefaultValue(1500.50)]
        public decimal Value { get; set; }

        [Required]
        [SwaggerSchema("Descrição da transação")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        [DefaultValue("Compra de Equipamentos")]
        public string? Description { get; set; }

        [Required]
        [SwaggerSchema("Criado por")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        [DefaultValue("f47ac10b-58cc-4372-a567-0e02b2c3d479")]
        public string? CreatedBy { get; set; }
    }
}
