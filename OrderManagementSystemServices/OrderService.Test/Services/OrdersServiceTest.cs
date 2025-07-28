using Xunit;
using FakeItEasy;
using OrderService.Services.Implementation;
using OrderService.Data.Repositories.Interfaces;
using OrderService.Data.Entities;
using OrderService.Models;
using OrderService.RabbitMQ;

namespace OrderService.Test.Services
{
    public class OrdersServiceTest
    {
        private readonly IOrdersRepository _fakeRepo;
        private readonly IRabbitMqPublisher _fakePublisher;
        private readonly IRabbitMqProductCheck _fakeProductCheck;
        private readonly OrdersService _service;

        public OrdersServiceTest()
        {
            _fakeRepo = A.Fake<IOrdersRepository>();
            _fakePublisher = A.Fake<IRabbitMqPublisher>();
            _fakeProductCheck = A.Fake<IRabbitMqProductCheck>();
            _service = new OrdersService(_fakeRepo, _fakePublisher, _fakeProductCheck);
        }

        [Fact]
        public async Task GetOrderById_Should_Return_Success()
        {
            // Arrange
            var dummyOrder = new Orders 
            { 
                Id = 7, 
                CustomerId = 1,
                ProductId = 1,
                OrderDate = DateTime.UtcNow,
                Quantity = 2,
                TotalAmount = 100.00M
            };
            A.CallTo(() => _fakeRepo.GetOrderByIdAsync(7)).Returns(dummyOrder);

            // Act
            var result = await _service.GetOrderById(7);

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
            A.CallTo(() => _fakeRepo.GetOrderByIdAsync(99)).Returns((Orders)null);

            // Act
            var result = await _service.GetOrderById(99);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateOrder_Should_Return_Success_When_Stock_Available()
        {
            // Arrange
            var createOrderDto = new CreateOrderDto
            {
                CustomerId = 1,
                ProductId = 1,
                Quantity = 2
            };

            var stockResponse = new ValidateProductResponseDto
            {
                InStock = true,
                totalAmount = 100.00M
            };

            var expectedOrder = new Orders
            {
                Id = 1,
                CustomerId = createOrderDto.CustomerId,
                ProductId = createOrderDto.ProductId,
                Quantity = createOrderDto.Quantity,
                TotalAmount = stockResponse.totalAmount,
                OrderDate = DateTime.UtcNow
            };

            A.CallTo(() => _fakeProductCheck.CheckProductStockAvailabilityAsync(createOrderDto.ProductId, createOrderDto.Quantity))
                .Returns(stockResponse);
            A.CallTo(() => _fakeRepo.AddOrderAsync(A<Orders>._)).Returns(expectedOrder);
            A.CallTo(() => _fakePublisher.SendMessageToQueue(A<string>._, A<string>._)).Returns(true);

            // Act
            var result = await _service.CreateOrder(createOrderDto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsStockAvailable);
            Assert.NotNull(result.Order);
            Assert.Equal(expectedOrder.CustomerId, result.Order.CustomerId);
            Assert.Equal(expectedOrder.ProductId, result.Order.ProductId);
            Assert.Equal(expectedOrder.Quantity, result.Order.Quantity);
            Assert.Equal(expectedOrder.TotalAmount, result.Order.TotalAmount);
        }

        [Fact]
        public async Task CreateOrder_Should_Return_Failure_When_Stock_Not_Available()
        {
            // Arrange
            var createOrderDto = new CreateOrderDto
            {
                CustomerId = 1,
                ProductId = 1,
                Quantity = 2
            };

            var stockResponse = new ValidateProductResponseDto
            {
                InStock = false,
                totalAmount = 0
            };

            A.CallTo(() => _fakeProductCheck.CheckProductStockAvailabilityAsync(createOrderDto.ProductId, createOrderDto.Quantity))
                .Returns(stockResponse);

            // Act
            var result = await _service.CreateOrder(createOrderDto);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsStockAvailable);
            Assert.Null(result.Order);
            Assert.Contains("Stock is not available", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateOrder_Should_Return_Success()
        {
            // Arrange
            var updateOrderDto = new UpdateOrderDto
            {
                Id = 1,
                CustomerId = 1,
                ProductId = 1,
                Quantity = 2,
                TotalAmount = 100.00M,
                OrderDate = DateTime.UtcNow
            };

            var existingOrder = new Orders
            {
                Id = 1,
                CustomerId = 1,
                ProductId = 1,
                Quantity = 1,
                TotalAmount = 50.00M,
                OrderDate = DateTime.UtcNow.AddDays(-1)
            };

            var updatedOrder = new Orders
            {
                Id = updateOrderDto.Id,
                CustomerId = updateOrderDto.CustomerId,
                ProductId = updateOrderDto.ProductId,
                Quantity = updateOrderDto.Quantity,
                TotalAmount = updateOrderDto.TotalAmount,
                OrderDate = updateOrderDto.OrderDate
            };

            A.CallTo(() => _fakeRepo.GetOrderByIdAsync(1)).Returns(existingOrder);
            A.CallTo(() => _fakeRepo.UpdateOrderAsync(1, A<Orders>._)).Returns(updatedOrder);

            // Act
            var result = await _service.UpdateOrder(1, updateOrderDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updateOrderDto.Id, result.Id);
            Assert.Equal(updateOrderDto.CustomerId, result.CustomerId);
            Assert.Equal(updateOrderDto.ProductId, result.ProductId);
            Assert.Equal(updateOrderDto.Quantity, result.Quantity);
            Assert.Equal(updateOrderDto.TotalAmount, result.TotalAmount);
        }

        [Fact]
        public async Task UpdateOrder_Should_Return_Null_When_Order_Not_Found()
        {
            // Arrange
            var updateOrderDto = new UpdateOrderDto
            {
                Id = 99,
                CustomerId = 1,
                ProductId = 1,
                Quantity = 2,
                TotalAmount = 100.00M,
                OrderDate = DateTime.UtcNow
            };

            A.CallTo(() => _fakeRepo.GetOrderByIdAsync(99)).Returns((Orders)null);

            // Act
            var result = await _service.UpdateOrder(99, updateOrderDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteOrder_Should_Return_True_When_Order_Exists()
        {
            // Arrange
            A.CallTo(() => _fakeRepo.DeleteOrderAsync(1)).Returns(true);

            // Act
            var result = await _service.DeleteOrder(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteOrder_Should_Return_False_When_Order_Not_Found()
        {
            // Arrange
            A.CallTo(() => _fakeRepo.DeleteOrderAsync(99)).Returns(false);

            // Act
            var result = await _service.DeleteOrder(99);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetAllOrders_Should_Return_All_Orders()
        {
            // Arrange
            var orders = new List<Orders>
            {
                new Orders { Id = 1, CustomerId = 1, ProductId = 1, Quantity = 1, TotalAmount = 50.00M, OrderDate = DateTime.UtcNow },
                new Orders { Id = 2, CustomerId = 2, ProductId = 2, Quantity = 2, TotalAmount = 100.00M, OrderDate = DateTime.UtcNow }
            };

            A.CallTo(() => _fakeRepo.GetAllOrdersAsync()).Returns(orders);

            // Act
            var result = await _service.GetAllOrders();

            // Assert
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.Equal(orders[0].Id, resultList[0].Id);
            Assert.Equal(orders[1].Id, resultList[1].Id);
        }

        [Fact]
        public async Task GetAllOrders_Should_Return_Empty_List_When_No_Orders()
        {
            // Arrange
            A.CallTo(() => _fakeRepo.GetAllOrdersAsync()).Returns(new List<Orders>());

            // Act
            var result = await _service.GetAllOrders();

            // Assert
            Assert.Empty(result);
        }
    }
} 