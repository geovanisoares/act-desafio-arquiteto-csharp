using act_ms_transaction.Api.DTOs.Requests;
using act_ms_transaction.Api.DTOs.Parameters;
using act_ms_transaction.Domain.Entities;
using act_ms_transaction.Domain.Enums;
using System;
using act_ms_transaction.Infrastructure.EFModels;
using act_ms_transaction.Api.DTOs.Responses;

namespace act_ms_transaction.Application.Mappers
{
    public static class TransactionMapper
    {
        /// <summary>
        /// Mapeia CreateTransactionRequest para Domain.
        /// </summary>
        public static TransactionEntity MapToDomain(CreateTransactionRequest request)
        {
            return new TransactionEntity
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Parse(request.Date),
                Type = ConvertCharToEnum(request.Type),
                Value = request.Value,
                Description = request.Description ?? string.Empty,
                CreatedBy = request.CreatedBy ?? "unknown",
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Mapeia Domain para CreateTransactionRequest.
        /// </summary>
        public static CreateTransactionRequest MapToCreateRequest(TransactionEntity transaction)
        {
            return new CreateTransactionRequest
            {
                Date = transaction.Date.ToString("yyyy-MM-dd"),
                Type = ConvertEnumToChar(transaction.Type),
                Value = transaction.Value,
                Description = transaction.Description,
                CreatedBy = transaction.CreatedBy
            };
        }

        /// <summary>
        /// Mapeia UpdateTransactionRequest para Domain.
        /// </summary>
        public static TransactionEntity MapToDomain(UpdateTransactionRequest request)
        {

            return new TransactionEntity
            {
                Date = DateTime.Parse(request.Date),
                Type = ConvertCharToEnum(request.Type),
                Value = request.Value,
                Description = request.Description ?? string.Empty,
                UpdatedBy = request.UpdatedBy ?? "unknown",
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static TransactionEntity MapToEntity(TransactionModel model)
        {
            return new TransactionEntity
            {
                Id = model.Id,
                Date = DateTime.Parse(model.Date),
                Type = model.Type == "I" ? Domain.Enums.TransactionType.Income : Domain.Enums.TransactionType.Expense,
                Value = model.Value,
                Description = model.Description,
                CreatedAt = model.CreatedAt,
                CreatedBy = model.CreatedBy,
                UpdatedAt = model.UpdatedAt,
                UpdatedBy = model.UpdatedBy
            };
        }

        public static TransactionModel MapToEFModel(TransactionEntity entity)
        {
            return new TransactionModel
            {
                Id = entity.Id,
                Date = entity.Date.ToString("yyyy-MM-dd"),
                Type = entity.Type == Domain.Enums.TransactionType.Income ? "I" : "E",
                Value = entity.Value,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt,
                CreatedBy = entity.CreatedBy,
                UpdatedAt = entity.UpdatedAt,
                UpdatedBy = entity.UpdatedBy
            };
        }

        public static GetTransactionResponse MapToResponse(TransactionEntity entity)
        {
            return new GetTransactionResponse
            {
                Id = entity.Id,
                Date = entity.Date.ToString("yyyy-MM-dd"),  // Mantém o formato padronizado
                Value = entity.Value,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt,
                CreatedBy = entity.CreatedBy,
                UpdatedAt = entity.UpdatedAt,
                UpdatedBy = entity.UpdatedBy
            };
        }

        /// <summary>
        /// Método para converter TransactionType para char ('I' ou 'E')
        /// </summary>
        private static char ConvertEnumToChar(TransactionType transactionType)
        {
            return transactionType switch
            {
                TransactionType.Income => 'I',
                TransactionType.Expense => 'E',
                _ => throw new ArgumentException("Tipo de transação inválido")
            };
        }

        /// <summary>
        /// Converte `char` ('I' ou 'E') para `TransactionType`.
        /// </summary>
        public static TransactionType ConvertCharToEnum(char type)
        {
            return type switch
            {
                'I' => TransactionType.Income,
                'E' => TransactionType.Expense,
                _ => throw new ArgumentException($"Tipo de transação inválido: {type}")
            };
        }
    }
}
