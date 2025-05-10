using act_ms_transaction.Api.DTOs.Responses;
using act_ms_transaction.Application.DTOs;
using act_ms_transaction.Application.Exeptions;
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
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(ITransactionRepository transactionRepository, IMessageService messageService, ILogger<TransactionService> logger)
        {
            _transactionRepository = transactionRepository;
            _messageService = messageService;
            _logger = logger;
        }

        public async Task<TransactionEntity> CreateAsync(TransactionEntity transaction)
        {
            transaction.CreatedAt = DateTime.UtcNow;

            _logger.LogInformation("Creating transaction: {@Transaction}", transaction);

            var createdTransaction = await _transactionRepository.CreateAsync(transaction);

            await _messageService.PublishAsync(createdTransaction.Date.ToString("yyyy-MM-dd"));

            _logger.LogInformation("Transaction created: {@Transaction}", createdTransaction);

            return createdTransaction;
        }

        public async Task<TransactionEntity> UpdateAsync(TransactionEntity transaction, Guid transactionId)
        {
            if (transactionId == Guid.Empty)
            {
                _logger.LogWarning("Id field not found");
                throw new BusinessException("id field is required.");
            }

            var transactionToUpdate = await _transactionRepository.GetByIdAsync(transactionId);

            if (transactionToUpdate == null)
            {
                _logger.LogWarning($"Transaction with ID {transactionId} not found");
                throw new NotFoundException($"Transaction with ID {transactionId} not found");
            }

            transaction.Id = transactionId;
            transaction.UpdatedAt = DateTime.UtcNow;

            _logger.LogInformation($"Updating transaction: {transactionId}");

            var updatedTransaction = await _transactionRepository.UpdateAsync(transaction);

            await _messageService.PublishAsync(updatedTransaction.Date.ToString("yyyy-MM-dd"));

            _logger.LogInformation($"Transaction updated: {transactionId}");

            return updatedTransaction;
        }

        public async Task<bool> DeleteAsync(Guid transactionId)
        {
            _logger.LogInformation($"Attempting to delete transaction with ID {transactionId}");

            var transaction = await _transactionRepository.GetByIdAsync(transactionId);

            if (transaction == null)
            {
                _logger.LogWarning($"Transaction with ID {transactionId} not found");
                throw new NotFoundException($"Transaction with ID {transactionId} not found");
            }

            var isDeleted = await _transactionRepository.DeleteAsync(transactionId);

            if (isDeleted)
            {
                await _messageService.PublishAsync(transaction.Date.ToString("yyyy-MM-dd"));
                _logger.LogInformation("Transaction deleted: {TransactionId}", transactionId);
            }

            return isDeleted;
        }

        public async Task<TransactionEntity?> GetByIdAsync(Guid transactionId)
        {
            if (transactionId == Guid.Empty)
            {
                _logger.LogWarning("Id field not found");
                throw new BusinessException("id field is required.");
            }

            _logger.LogInformation($"Fetching transaction with ID {transactionId}");

            var transaction = await _transactionRepository.GetByIdAsync(transactionId);

            if (transaction == null)
            {
                _logger.LogWarning($"Transaction with ID {transactionId} not found");
                throw new NotFoundException($"Transaction with ID {transactionId} not found");
            }

            _logger.LogInformation($"Transaction with ID {transactionId} fetched");

            return transaction;
        }

        public async Task<PagedResult<GetTransactionResponse>> GetAllAsync(int pageNumber, int pageSize, string? orderBy, bool asc)
        {
            _logger.LogInformation("Fetching transactions - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

            var pagedEntities = await _transactionRepository.GetAllAsync(pageNumber, pageSize, orderBy, asc);

            _logger.LogInformation("Fetched {Count} transactions", pagedEntities.Items.Count());

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
