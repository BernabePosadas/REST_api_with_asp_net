using Microsoft.EntityFrameworkCore;

namespace TodoApi.Models
{
    public class POSContext : DbContext
    {
        public POSContext(DbContextOptions<POSContext> options)
            : base(options)
        {
        }

        public DbSet<POSItems> Items { get; set; }
    }
}