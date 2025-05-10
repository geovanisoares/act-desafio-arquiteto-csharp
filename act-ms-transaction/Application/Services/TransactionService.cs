using act_ms_transaction.Api.DTOs.Responses;
using act_ms_transaction.Application.DTOs;
using act_ms_transaction.Application.Interfaces;
using act_ms_transaction.Application.Mappers;
using act_ms_transaction.Domain.Entities;
using act_ms_transaction.Domain.Interfaces;

namespace act_ms_transaction.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMessageService _messageService;

        public TransactionService(ITransactionRepository transactionRepository, IMessageService messageService)
        {
            _transactionRepository = transactionRepository;
            _messageService = messageService;
        }

        public async Task<TransactionEntity> CreateAsync(TransactionEntity transaction)
        {
            transaction.CreatedAt = DateTime.UtcNow;
            var createdTransaction = await _transactionRepository.CreateAsync(transaction);

            // Publica apenas a data
            await _messageService.PublishAsync(createdTransaction.Date.ToString("yyyy-MM-dd"));

            return createdTransaction;
        }

        public async Task<TransactionEntity> UpdateAsync(TransactionEntity transaction)
        {
            transaction.UpdatedAt = DateTime.UtcNow;
            var updatedTransaction = await _transactionRepository.UpdateAsync(transaction);

            // Publica apenas a data
            await _messageService.PublishAsync(updatedTransaction.Date.ToString("yyyy-MM-dd"));

            return updatedTransaction;
        }

        public async Task<bool> DeleteAsync(Guid transactionId)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);

            if (transaction == null)
            {
                Console.WriteLine($"Transaction with ID {transactionId} not found.");
                return false;
            }

            // Deleta a transação
            var isDeleted = await _transactionRepository.DeleteAsync(transactionId);

            if (isDeleted)
            {
                await _messageService.PublishAsync(transaction.Date.ToString("yyyy-MM-dd"));
            }

            return isDeleted;
        }

        public async Task<TransactionEntity?> GetByIdAsync(Guid transactionId)
        {
            return await _transactionRepository.GetByIdAsync(transactionId);
        }

        public async Task<PagedResult<GetTransactionResponse>> GetAllAsync(int pageNumber, int pageSize, string? orderBy, bool asc)
        {
            var pagedEntities = await _transactionRepository.GetAllAsync(pageNumber, pageSize, orderBy, asc);

            var response = new PagedResult<GetTransactionResponse>
            {
                Items = pagedEntities.Items.Select(TransactionMapper.MapToResponse),
                TotalCount = pagedEntities.TotalCount,
                PageNumber = pagedEntities.PageNumber,
                PageSize = pagedEntities.PageSize
            };

            return response;
        }
    }
}
