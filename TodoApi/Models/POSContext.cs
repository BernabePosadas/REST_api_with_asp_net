using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Diagnostics;
namespace TodoApi.Models
{
    public class POSContext : DbContext
    {
        public POSContext(DbContextOptions<POSContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder
                .Entity<AccessRights>().HasOne(e => e.users).WithOne(e => e.rights)
                .OnDelete(DeleteBehavior.Cascade);
        public DbSet<POSItems> Items { get; set; }
        public DbSet<UserProfile> Users { get; set; }
    }
}