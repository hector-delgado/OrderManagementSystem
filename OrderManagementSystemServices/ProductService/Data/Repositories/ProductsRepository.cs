using ProductService.Data.Context;
using ProductService.Data.Entities;
using ProductService.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ProductService.Data.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly ProductsContext _context;

        public ProductsRepository(ProductsContext context)
        {
            _context = context;
        }  

        public async Task<Products> GetProductByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Products> GetProductByNameAsync(string name)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Name == name);
        }

        public async Task<Products> CreateProductAsync(Products product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Products> UpdateProductAsync(Products product)
        {
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<IEnumerable<Products>> GetAllProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }
    }
}
