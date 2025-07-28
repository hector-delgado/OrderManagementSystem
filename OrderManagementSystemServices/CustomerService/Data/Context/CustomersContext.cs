using Microsoft.EntityFrameworkCore;
using CustomerService.Data.Entities;

namespace CustomerService.Data.Context
{
    public class CustomersContext : DbContext
    {
        public CustomersContext(DbContextOptions<CustomersContext> options) : base(options)
        {
        }

        public DbSet<Customers> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customers>()
                .HasIndex(c => c.Email)
                .IsUnique();
        }
    }
}
