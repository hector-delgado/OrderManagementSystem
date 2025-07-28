using Xunit;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using ProductService.Controllers;
using ProductService.Services;
using ProductService.Data.Entities;
using ProductService.Models;

namespace ProductService.Test.Controllers
{
    public class ProductsControllerTests
    {
        private readonly IProductsService _fakeService;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _fakeService = A.Fake<IProductsService>();
            _controller = new ProductsController(_fakeService);
        }

        [Fact]
        public async Task GetAllProducts_Should_Return_Ok_With_Products()
        {
            // Arrange
            var products = new List<Products>
            {
                new Products { Id = 1, Name = "Product 1", Price = 100.00M, AvailableStock = 10 },
                new Products { Id = 2, Name = "Product 2", Price = 200.00M, AvailableStock = 20 }
            };
            A.CallTo(() => _fakeService.GetAllProducts()).Returns(products);

            // Act
            var result = await _controller.GetAllProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProducts = Assert.IsType<List<Products>>(okResult.Value);
            Assert.Equal(2, returnedProducts.Count);
        }

        [Fact]
        public async Task GetAllProducts_Should_Return_500_On_Error()
        {
            // Arrange
            A.CallTo(() => _fakeService.GetAllProducts()).Throws(new Exception("Test error"));

            // Act
            var result = await _controller.GetAllProducts();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Test error", statusCodeResult.Value.ToString());
        }

        [Fact]
        public async Task GetProductById_Should_Return_Ok_With_Product()
        {
            // Arrange
            var product = new Products
            {
                Id = 1,
                Name = "Test Product",
                Price = 100.00M,
                AvailableStock = 10
            };
            A.CallTo(() => _fakeService.GetProductById(1)).Returns(product);

            // Act
            var result = await _controller.GetProductById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProduct = Assert.IsType<Products>(okResult.Value);
            Assert.Equal(1, returnedProduct.Id);
            Assert.Equal("Test Product", returnedProduct.Name);
        }

        [Fact]
        public async Task GetProductById_Should_Return_NotFound()
        {
            // Arrange
            A.CallTo(() => _fakeService.GetProductById(999)).Returns((Products)null);

            // Act
            var result = await _controller.GetProductById(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetProductById_Should_Return_500_On_Error()
        {
            // Arrange
            A.CallTo(() => _fakeService.GetProductById(1)).Throws(new Exception("Test error"));

            // Act
            var result = await _controller.GetProductById(1);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Test error", statusCodeResult.Value.ToString());
        }

        [Fact]
        public async Task CreateProduct_Should_Return_CreatedAtAction()
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

            A.CallTo(() => _fakeService.CreateProduct(createDto)).Returns(createdProduct);

            // Act
            var result = await _controller.CreateProduct(createDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(ProductsController.GetProductById), createdAtActionResult.ActionName);
            Assert.Equal(1, createdAtActionResult.RouteValues["id"]);
            var returnedProduct = Assert.IsType<Products>(createdAtActionResult.Value);
            Assert.Equal(createdProduct.Id, returnedProduct.Id);
        }

        [Fact]
        public async Task CreateProduct_Should_Return_BadRequest_When_Null()
        {
            // Act
            var result = await _controller.CreateProduct(null);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task CreateProduct_Should_Return_BadRequest_When_Creation_Fails()
        {
            // Arrange
            var createDto = new CreateProductDto
            {
                Name = "New Product",
                Price = 150.00M,
                AvailableStock = 20
            };

            A.CallTo(() => _fakeService.CreateProduct(createDto)).Returns((Products)null);

            // Act
            var result = await _controller.CreateProduct(createDto);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task CreateProduct_Should_Return_500_On_Error()
        {
            // Arrange
            var createDto = new CreateProductDto
            {
                Name = "New Product",
                Price = 150.00M,
                AvailableStock = 20
            };

            A.CallTo(() => _fakeService.CreateProduct(createDto)).Throws(new Exception("Test error"));

            // Act
            var result = await _controller.CreateProduct(createDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Test error", statusCodeResult.Value.ToString());
        }
    }
} 