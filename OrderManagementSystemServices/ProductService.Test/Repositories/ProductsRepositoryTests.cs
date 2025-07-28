using ProductService.Data.Context;
using ProductService.Data.Entities;
using ProductService.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ProductService.Test.Repositories
{
    public class ProductsRepositoryTests
    {
        private readonly ProductsContext _context;
        private readonly ProductsRepository _repository;

        public ProductsRepositoryTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<ProductsContext>()
                .UseInMemoryDatabase(databaseName: $"ProductsDb_{Guid.NewGuid()}")
                .Options;

            _context = new ProductsContext(options);
            _repository = new ProductsRepository(_context);
        }

        [Fact]
        public async Task GetProductByIdAsync_ExistingId_ReturnsProduct()
        {
            // Arrange
            var product = new Products
            {
                Id = 1,
                Name = "Test Product",
                Price = 100.00M,
                AvailableStock = 10
            };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetProductByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(product.Id, result.Id);
            Assert.Equal(product.Name, result.Name);
            Assert.Equal(product.Price, result.Price);
            Assert.Equal(product.AvailableStock, result.AvailableStock);
        }

        [Fact]
        public async Task GetProductByIdAsync_NonExistingId_ReturnsNull()
        {
            // Act
            var result = await _repository.GetProductByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetProductByNameAsync_ExistingName_ReturnsProduct()
        {
            // Arrange
            var product = new Products
            {
                Name = "Test Product",
                Price = 100.00M,
                AvailableStock = 10
            };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetProductByNameAsync("Test Product");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(product.Name, result.Name);
            Assert.Equal(product.Price, result.Price);
            Assert.Equal(product.AvailableStock, result.AvailableStock);
        }

        [Fact]
        public async Task GetProductByNameAsync_NonExistingName_ReturnsNull()
        {
            // Act
            var result = await _repository.GetProductByNameAsync("Non-existing Product");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateProductAsync_ValidProduct_ReturnsCreatedProduct()
        {
            // Arrange
            var product = new Products
            {
                Name = "New Product",
                Price = 150.00M,
                AvailableStock = 20
            };

            // Act
            var result = await _repository.CreateProductAsync(product);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(0, result.Id); // Should have an assigned ID
            Assert.Equal(product.Name, result.Name);
            Assert.Equal(product.Price, result.Price);
            Assert.Equal(product.AvailableStock, result.AvailableStock);

            // Verify it's in the database
            var dbProduct = await _context.Products.FindAsync(result.Id);
            Assert.NotNull(dbProduct);
        }

        [Fact]
        public async Task UpdateProductAsync_ValidProduct_ReturnsUpdatedProduct()
        {
            // Arrange
            var product = new Products
            {
                Name = "Original Product",
                Price = 100.00M,
                AvailableStock = 10
            };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            product.Name = "Updated Product";
            product.Price = 200.00M;
            product.AvailableStock = 15;

            // Act
            var result = await _repository.UpdateProductAsync(product);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(product.Name, result.Name);
            Assert.Equal(product.Price, result.Price);
            Assert.Equal(product.AvailableStock, result.AvailableStock);

            // Verify changes are persisted
            var dbProduct = await _context.Products.FindAsync(product.Id);
            Assert.NotNull(dbProduct);
            Assert.Equal(product.Name, dbProduct.Name);
            Assert.Equal(product.Price, dbProduct.Price);
            Assert.Equal(product.AvailableStock, dbProduct.AvailableStock);
        }

        [Fact]
        public async Task GetAllProductsAsync_ReturnsAllProducts()
        {
            // Arrange
            var products = new List<Products>
            {
                new Products { Name = "Product 1", Price = 100.00M, AvailableStock = 10 },
                new Products { Name = "Product 2", Price = 200.00M, AvailableStock = 20 }
            };
            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllProductsAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.All(products, product =>
                Assert.Contains(resultList, r =>
                    r.Name == product.Name &&
                    r.Price == product.Price &&
                    r.AvailableStock == product.AvailableStock));
        }

        [Fact]
        public async Task GetAllProductsAsync_EmptyDatabase_ReturnsEmptyList()
        {
            // Act
            var result = await _repository.GetAllProductsAsync();

            // Assert
            Assert.Empty(result);
        }
    }
} 