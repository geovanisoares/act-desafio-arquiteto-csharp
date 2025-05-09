using act_ms_transaction.Domain.Entities;
using act_ms_transaction.Domain.Interfaces;
using act_ms_transaction.Infrastructure.EFModels;
using act_ms_transaction.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using act_ms_transaction.Application.Mappers;
using act_ms_transaction.Application.DTOs;

namespace act_ms_transaction.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TransactionEntity> CreateAsync(TransactionEntity transaction)
        {
            var model = TransactionMapper.MapToEFModel(transaction);
            model.UpdatedBy = transaction.CreatedBy;
            await _context.Transactions.AddAsync(model);
            await _context.SaveChangesAsync();
            return TransactionMapper.MapToEntity(model);
        }

        public async Task<TransactionEntity> UpdateAsync(TransactionEntity transaction)
        {
            var model = await _context.Transactions.FindAsync(transaction.Id);
            if (model == null) throw new KeyNotFoundException("Transaction not found.");

            model.Date = transaction.Date.ToString("yyyy-MM-dd");
            model.Type = transaction.Type == Domain.Enums.TransactionType.Income ? "I" : "E";
            model.Value = transaction.Value;
            model.Description = transaction.Description;
            model.UpdatedAt = DateTime.UtcNow;
            model.UpdatedBy = transaction.UpdatedBy;

            _context.Transactions.Update(model);
            await _context.SaveChangesAsync();
            return TransactionMapper.MapToEntity(model);
        }

        public async Task<bool> DeleteAsync(Guid transactionId)
        {
            var model = await _context.Transactions.FindAsync(transactionId);
            if (model == null) return false;

            _context.Transactions.Remove(model);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TransactionEntity?> GetByIdAsync(Guid transactionId)
        {
            var model = await _context.Transactions.FindAsync(transactionId);
            return model != null ? TransactionMapper.MapToEntity(model) : null;
        }

        public async Task<PagedResult<TransactionEntity>> GetAllAsync(int pageNumber, int pageSize, string? orderBy, bool asc)
        {
            IQueryable<TransactionModel> query = _context.Transactions;

            var validFields = new[] { "date", "value", "createdat" };

            if (!string.IsNullOrEmpty(orderBy) && !validFields.Contains(orderBy.ToLower()))
            {
                string validOptions = string.Join(", ", validFields);
                throw new ArgumentException($"Campo de ordenação inválido. Use um dos seguintes: {validOptions}.");
            }

            int totalCount = await query.CountAsync();

            query = orderBy?.ToLower() switch
            {
                "date" => asc ? query.OrderBy(t => t.Date) : query.OrderByDescending(t => t.Date),
                "value" => asc ? query.OrderBy(t => t.Value) : query.OrderByDescending(t => t.Value),
                "createdat" => asc ? query.OrderBy(t => t.CreatedAt) : query.OrderByDescending(t => t.CreatedAt),
                _ => query.OrderBy(t => t.Id)
            };

            int skip = (pageNumber - 1) * pageSize;
            var transactionModels = await query.Skip(skip).Take(pageSize).ToListAsync();

            var transactionEntities = transactionModels.Select(TransactionMapper.MapToEntity).ToList();

            return new PagedResult<TransactionEntity>
            {
                Items = transactionEntities,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
