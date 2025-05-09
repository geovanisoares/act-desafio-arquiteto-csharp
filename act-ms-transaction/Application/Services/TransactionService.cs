using act_ms_transaction.Api.DTOs.Parameters;
using act_ms_transaction.Api.DTOs.Responses;
using act_ms_transaction.Application.DTOs;
using act_ms_transaction.Application.Interfaces;
using act_ms_transaction.Application.Mappers;
using act_ms_transaction.Domain.Entities;
using act_ms_transaction.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace act_ms_transaction.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<TransactionEntity> CreateAsync(TransactionEntity transaction)
        {
            transaction.CreatedAt = DateTime.UtcNow;
            return await _transactionRepository.CreateAsync(transaction);
        }

        public async Task<TransactionEntity> UpdateAsync(TransactionEntity transaction)
        {
            transaction.UpdatedAt = DateTime.UtcNow;
            return await _transactionRepository.UpdateAsync(transaction);
        }

        public async Task<bool> DeleteAsync(Guid transactionId)
        {
            return await _transactionRepository.DeleteAsync(transactionId);
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
