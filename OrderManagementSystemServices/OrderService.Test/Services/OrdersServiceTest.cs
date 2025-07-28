using Xunit;
using FakeItEasy;
using OrderService.Services.Implementation;
using OrderService.Data.Repositories.Interfaces;
using OrderService.Data.Entities;
using OrderService.Models;

namespace OrderService.Test.Services
{
    public class OrdersServiceTest
    {
        [Fact]
        public async Task GetOrderById_Should_Return_Success()
        {
            // Arrange
            var fakeRepo = A.Fake<IOrdersRepository>();
            var dummyOrder = new Orders 
            { 
                Id = 7, 
                CustomerId = 1,
                ProductId = 1,
                OrderDate = DateTime.UtcNow,
                Quantity = 2,
                TotalAmount = 100.00M
            };
            A.CallTo(() => fakeRepo.GetOrderByIdAsync(7)).Returns(dummyOrder);
            var service = new OrdersService(fakeRepo);

            // Act
            var result = await service.GetOrderById(7);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(7, result.Id);
            Assert.Equal(1, result.CustomerId);
            Assert.Equal(1, result.ProductId);
            Assert.Equal(2, result.Quantity);
            Assert.Equal(100.00M, result.TotalAmount);
        }

        [Fact]
        public async Task GetOrderById_Should_Return_Not_Found()
        {
            // Arrange
            var fakeRepo = A.Fake<IOrdersRepository>();
            A.CallTo(() => fakeRepo.GetOrderByIdAsync(99)).Returns((Orders)null);
            var service = new OrdersService(fakeRepo);

            // Act
            var result = await service.GetOrderById(99);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateOrder_Should_Return_Success()
        {
            // Arrange
            var fakeRepo = A.Fake<IOrdersRepository>();
            var createOrderDto = new CreateOrderDto
            {
                CustomerId = 1,
                ProductId = 1,
                Quantity = 2,
                TotalAmount = 100.00M
            };

            var expectedOrder = new Orders
            {
                Id = 1,
                CustomerId = createOrderDto.CustomerId,
                ProductId = createOrderDto.ProductId,
                Quantity = createOrderDto.Quantity,
                TotalAmount = createOrderDto.TotalAmount,
                OrderDate = DateTime.UtcNow
            };

            A.CallTo(() => fakeRepo.AddOrderAsync(A<Orders>._)).Returns(expectedOrder);
            var service = new OrdersService(fakeRepo);

            // Act
            var result = await service.CreateOrder(createOrderDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedOrder.Id, result.Id);
            Assert.Equal(expectedOrder.CustomerId, result.CustomerId);
            Assert.Equal(expectedOrder.ProductId, result.ProductId);
            Assert.Equal(expectedOrder.Quantity, result.Quantity);
            Assert.Equal(expectedOrder.TotalAmount, result.TotalAmount);
        }
    }
} 