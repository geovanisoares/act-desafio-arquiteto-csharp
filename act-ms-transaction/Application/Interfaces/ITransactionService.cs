using act_ms_transaction.Api.DTOs.Parameters;
using act_ms_transaction.Api.DTOs.Responses;
using act_ms_transaction.Application.DTOs;
using act_ms_transaction.Domain.Entities;

namespace act_ms_transaction.Application.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionEntity> CreateAsync(TransactionEntity transaction);
        Task<TransactionEntity> UpdateAsync(TransactionEntity transaction, Guid id);
        Task<bool> DeleteAsync(Guid transactionId);
        Task<TransactionEntity?> GetByIdAsync(Guid transactionId);
        Task<PagedResult<GetTransactionResponse>> GetAllAsync(int pageNumber, int pageSize, string? orderBy, bool asc);
    }
}
