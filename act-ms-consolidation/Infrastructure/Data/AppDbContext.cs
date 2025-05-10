using act_ms_consolidation.Infrastructure.EFModels;
using Microsoft.EntityFrameworkCore;

namespace act_ms_consolidation.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ConsolidationQueryModel> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConsolidationQueryModel>().HasNoKey().ToTable("Transactions");
        }
    }
}
