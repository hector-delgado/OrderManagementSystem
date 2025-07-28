
using ProductService.Data.Entities;
using ProductService.Data.Repositories.Interfaces;
using ProductService.Models;
using ProductService.Services;

namespace ProductService.Services.Implementation
{
    public class ProductsService : IProductsService
    {
        private readonly IProductsRepository _productsRepository;

        public ProductsService(IProductsRepository productsRepository)
        {
            _productsRepository = productsRepository;
        }

        public async Task<Products> GetProductById(int id)
        {
            return await _productsRepository.GetProductByIdAsync(id);
        }

        public async Task<Products> CreateProduct(CreateProductDto product)
        {
            try
            {
                // Check if a product with the same name already exists
                var existingProduct = await _productsRepository.GetProductByNameAsync(product.Name);
                if (existingProduct != null)
                {
                    return null;
                }

                var entity = new Products
                {
                    Name = product.Name,
                    Price = product.Price,
                    AvailableStock = product.AvailableStock
                };
                return await _productsRepository.CreateProductAsync(entity);
            }
            catch (Exception ex)
            {
                // Check for SQL constraint violation (unique name)
                if (ex.InnerException != null && ex.InnerException.Message.Contains("UNIQUE"))
                {
                    return null;
                }
                throw;
            }
        }

        public async Task<Products> UpdateProduct(UpdateProductDto product)
        {
            try
            {
                // Check if the product exists
                var existingProduct = await _productsRepository.GetProductByIdAsync(product.Id);
                if (existingProduct == null)
                {
                    return null;
                }

                // Check if the new name conflicts with another product (if name is being changed)
                if (!string.Equals(existingProduct.Name, product.Name, StringComparison.OrdinalIgnoreCase))
                {
                    var productWithSameName = await _productsRepository.GetProductByNameAsync(product.Name);
                    if (productWithSameName != null && productWithSameName.Id != product.Id)
                    {
                        return null;
                    }
                }

                // Update the existing product
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.AvailableStock = product.AvailableStock;

                return await _productsRepository.UpdateProductAsync(existingProduct);
            }
            catch (Exception ex)
            {
                // Check for SQL constraint violation (unique name)
                if (ex.InnerException != null && ex.InnerException.Message.Contains("UNIQUE"))
                {
                    return null;
                }
                throw;
            }
        }

        public async Task<IEnumerable<Products>> GetAllProducts()
        {
            return await _productsRepository.GetAllProductsAsync();
        }
    }
} 