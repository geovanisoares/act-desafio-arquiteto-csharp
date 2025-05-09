using act_ms_transaction.Application.DTOs;
using act_ms_transaction.Domain.Entities;

namespace act_ms_transaction.Domain.Interfaces
{
    public interface ITransactionRepository
    {
        Task<TransactionEntity> CreateAsync(TransactionEntity transaction);
        Task<TransactionEntity> UpdateAsync(TransactionEntity transaction);
        Task<bool> DeleteAsync(Guid transactionId);
        Task<TransactionEntity?> GetByIdAsync(Guid transactionId);
        Task<PagedResult<TransactionEntity>> GetAllAsync(int pageNumber, int pageSize, string? orderBy, bool asc);

    }
}
