namespace ProductService.Services
{
    using ProductService.Data.Entities;
    using ProductService.Models;
    public interface IProductsService
    {
        Task<Products> GetProductById(int id);
        Task<Products> CreateProduct(CreateProductDto product);
        Task<Products> UpdateProduct(UpdateProductDto product);
        Task<IEnumerable<Products>> GetAllProducts();
    }
} 