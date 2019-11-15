using Microsoft.EntityFrameworkCore;

namespace TodoApi.Models
{
    public class TransactionContext : DbContext
    {
        public TransactionContext(DbContextOptions<TransactionContext> options)
            : base(options)
        {
        }
        public DbSet<TransactionRequest> cart_items { get; set; }
    }
}