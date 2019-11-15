using Microsoft.EntityFrameworkCore;

namespace TodoApi.Models
{
    public class TransactionContext : DbContext
    {
        public TransactionContext(DbContextOptions<TransactionContext> options)
            : base(options)
        {
        }

        public DbSet<TransactionRequest> Items { get; set; }
    }
}