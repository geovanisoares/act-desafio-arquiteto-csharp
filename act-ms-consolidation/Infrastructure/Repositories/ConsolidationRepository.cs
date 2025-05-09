using act_ms_consolidation.Domain.Entities;
using act_ms_consolidation.Domain.Interfaces;
using act_ms_consolidation.Infrastructure.Data;
using act_ms_consolidation.Infrastructure.EFModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace act_ms_consolidation.Infrastructure.Repositories
{
    public class ConsolidationRepository : IConsolidationRepository
    {
        private readonly AppDbContext _context;

        public ConsolidationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ConsolidationEntity> GetDailyConsolidationAsync(string date)
        {
            var transactions = await _context.Transactions
                .Where(t => t.Date == date)
                .Select(t => new TransactionQueryModel
                {
                    Date = t.Date,
                    Type = t.Type,
                    Value = t.Value
                })
                .ToListAsync();

            var totalIncome = transactions.Where(t => t.Type == "I").Sum(t => t.Value);
            var totalExpense = transactions.Where(t => t.Type == "E").Sum(t => t.Value);
            var balance = totalIncome - totalExpense;

            return new ConsolidationEntity
            {
                Date = date,
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                Balance = balance
            };
        }
    }
}
