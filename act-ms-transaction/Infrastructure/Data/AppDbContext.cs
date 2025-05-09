using act_ms_transaction.Infrastructure.EFModels;
using Microsoft.EntityFrameworkCore;

namespace act_ms_transaction.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<TransactionModel> Transactions { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TransactionModel>().Property(t => t.Type)
                .HasMaxLength(1);
        }
    }
}
