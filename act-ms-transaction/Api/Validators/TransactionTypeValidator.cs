using System;
using System.ComponentModel.DataAnnotations;

namespace act_ms_transaction.Api.Validators
{
    public class TransactionTypeValidator : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not char type)
            {
                return new ValidationResult("O tipo deve ser um caractere.");
            }

            if (type != 'I' && type != 'E')
            {
                return new ValidationResult("O tipo da transação deve ser 'I' (Income) ou 'E' (Expense).");
            }

            return ValidationResult.Success;
        }
    }
}
