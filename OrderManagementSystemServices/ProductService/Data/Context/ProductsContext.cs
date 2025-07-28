using ProductService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ProductService.Data.Context
{
    public class ProductsContext : DbContext
    {
        public ProductsContext(DbContextOptions<ProductsContext> options) : base(options)
        {
        }

        public DbSet<Products> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Products>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");
        }
    }
}
