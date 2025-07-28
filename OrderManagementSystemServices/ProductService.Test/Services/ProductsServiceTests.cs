using Xunit;
using FakeItEasy;
using ProductService.Services.Implementation;
using ProductService.Data.Repositories.Interfaces;
using ProductService.Data.Entities;
using ProductService.Models;

namespace ProductService.Test.Services
{
    public class ProductsServiceTests
    {
        private readonly IProductsRepository _fakeRepo;
        private readonly ProductsService _service;

        public ProductsServiceTests()
        {
            _fakeRepo = A.Fake<IProductsRepository>();
            _service = new ProductsService(_fakeRepo);
        }

        [Fact]
        public async Task GetProductById_Should_Return_Product()
        {
            // Arrange
            var product = new Products
            {
                Id = 1,
                Name = "Test Product",
                Price = 100.00M,
                AvailableStock = 10
            };
            A.CallTo(() => _fakeRepo.GetProductByIdAsync(1)).Returns(product);

            // Act
            var result = await _service.GetProductById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(product.Id, result.Id);
            Assert.Equal(product.Name, result.Name);
            Assert.Equal(product.Price, result.Price);
            Assert.Equal(product.AvailableStock, result.AvailableStock);
        }

        [Fact]
        public async Task GetProductById_Should_Return_Null_When_Not_Found()
        {
            // Arrange
            A.CallTo(() => _fakeRepo.GetProductByIdAsync(999)).Returns((Products)null);

            // Act
            var result = await _service.GetProductById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateProduct_Should_Return_Created_Product()
        {
            // Arrange
            var createDto = new CreateProductDto
            {
                Name = "New Product",
                Price = 150.00M,
                AvailableStock = 20
            };

            var createdProduct = new Products
            {
                Id = 1,
                Name = createDto.Name,
                Price = createDto.Price,
                AvailableStock = createDto.AvailableStock
            };

            A.CallTo(() => _fakeRepo.GetProductByNameAsync(createDto.Name)).Returns((Products)null);
            A.CallTo(() => _fakeRepo.CreateProductAsync(A<Products>._)).Returns(createdProduct);

            // Act
            var result = await _service.CreateProduct(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createdProduct.Id, result.Id);
            Assert.Equal(createDto.Name, result.Name);
            Assert.Equal(createDto.Price, result.Price);
            Assert.Equal(createDto.AvailableStock, result.AvailableStock);
        }

        [Fact]
        public async Task CreateProduct_Should_Return_Null_When_Name_Exists()
        {
            // Arrange
            var createDto = new CreateProductDto
            {
                Name = "Existing Product",
                Price = 150.00M,
                AvailableStock = 20
            };

            var existingProduct = new Products
            {
                Id = 1,
                Name = createDto.Name,
                Price = 100.00M,
                AvailableStock = 10
            };

            A.CallTo(() => _fakeRepo.GetProductByNameAsync(createDto.Name)).Returns(existingProduct);

            // Act
            var result = await _service.CreateProduct(createDto);

            // Assert
            Assert.Null(result);
            A.CallTo(() => _fakeRepo.CreateProductAsync(A<Products>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task UpdateProduct_Should_Return_Updated_Product()
        {
            // Arrange
            var updateDto = new UpdateProductDto
            {
                Id = 1,
                Name = "Updated Product",
                Price = 200.00M,
                AvailableStock = 15
            };

            var existingProduct = new Products
            {
                Id = updateDto.Id,
                Name = "Original Product",
                Price = 100.00M,
                AvailableStock = 10
            };

            var updatedProduct = new Products
            {
                Id = updateDto.Id,
                Name = updateDto.Name,
                Price = updateDto.Price,
                AvailableStock = updateDto.AvailableStock
            };

            A.CallTo(() => _fakeRepo.GetProductByIdAsync(updateDto.Id)).Returns(existingProduct);
            A.CallTo(() => _fakeRepo.GetProductByNameAsync(updateDto.Name)).Returns((Products)null);
            A.CallTo(() => _fakeRepo.UpdateProductAsync(A<Products>._)).Returns(updatedProduct);

            // Act
            var result = await _service.UpdateProduct(updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updateDto.Id, result.Id);
            Assert.Equal(updateDto.Name, result.Name);
            Assert.Equal(updateDto.Price, result.Price);
            Assert.Equal(updateDto.AvailableStock, result.AvailableStock);
        }

        [Fact]
        public async Task UpdateProduct_Should_Return_Null_When_Product_Not_Found()
        {
            // Arrange
            var updateDto = new UpdateProductDto
            {
                Id = 999,
                Name = "Non-existing Product",
                Price = 200.00M,
                AvailableStock = 15
            };

            A.CallTo(() => _fakeRepo.GetProductByIdAsync(updateDto.Id)).Returns((Products)null);

            // Act
            var result = await _service.UpdateProduct(updateDto);

            // Assert
            Assert.Null(result);
            A.CallTo(() => _fakeRepo.UpdateProductAsync(A<Products>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task UpdateProduct_Should_Return_Null_When_Name_Conflict()
        {
            // Arrange
            var updateDto = new UpdateProductDto
            {
                Id = 1,
                Name = "Conflicting Name",
                Price = 200.00M,
                AvailableStock = 15
            };

            var existingProduct = new Products
            {
                Id = updateDto.Id,
                Name = "Original Product",
                Price = 100.00M,
                AvailableStock = 10
            };

            var conflictingProduct = new Products
            {
                Id = 2,
                Name = updateDto.Name,
                Price = 300.00M,
                AvailableStock = 20
            };

            A.CallTo(() => _fakeRepo.GetProductByIdAsync(updateDto.Id)).Returns(existingProduct);
            A.CallTo(() => _fakeRepo.GetProductByNameAsync(updateDto.Name)).Returns(conflictingProduct);

            // Act
            var result = await _service.UpdateProduct(updateDto);

            // Assert
            Assert.Null(result);
            A.CallTo(() => _fakeRepo.UpdateProductAsync(A<Products>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task GetAllProducts_Should_Return_All_Products()
        {
            // Arrange
            var products = new List<Products>
            {
                new Products { Id = 1, Name = "Product 1", Price = 100.00M, AvailableStock = 10 },
                new Products { Id = 2, Name = "Product 2", Price = 200.00M, AvailableStock = 20 }
            };
            A.CallTo(() => _fakeRepo.GetAllProductsAsync()).Returns(products);

            // Act
            var result = await _service.GetAllProducts();

            // Assert
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.Equal(products[0].Id, resultList[0].Id);
            Assert.Equal(products[1].Id, resultList[1].Id);
        }

        [Fact]
        public async Task GetAllProducts_Should_Return_Empty_List_When_No_Products()
        {
            // Arrange
            A.CallTo(() => _fakeRepo.GetAllProductsAsync()).Returns(new List<Products>());

            // Act
            var result = await _service.GetAllProducts();

            // Assert
            Assert.Empty(result);
        }
    }
} 