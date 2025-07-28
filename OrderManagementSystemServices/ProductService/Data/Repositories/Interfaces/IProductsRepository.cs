using ProductService.Data.Entities;

namespace ProductService.Data.Repositories.Interfaces
{
    public interface IProductsRepository
    {
        Task<Products> GetProductByIdAsync(int id);
        Task<Products> CreateProductAsync(Products products);
        Task<Products> GetProductByNameAsync(string name);
        Task<Products> UpdateProductAsync(Products product);
        Task<IEnumerable<Products>> GetAllProductsAsync();
    }
} 