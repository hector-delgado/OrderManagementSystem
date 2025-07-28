using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrderService.Controllers;
using OrderService.Services;
using OrderService.Data.Entities;
using OrderService.Models;

namespace OrderService.Test.Controllers
{
    public class OrdersControllerTest
    {
        private readonly IOrdersService _fakeService;
        private readonly ILogger<OrdersController> _fakeLogger;
        private readonly OrdersController _controller;

        public OrdersControllerTest()
        {
            _fakeService = A.Fake<IOrdersService>();
            _fakeLogger = A.Fake<ILogger<OrdersController>>();
            _controller = new OrdersController(_fakeService, _fakeLogger);
        }

        [Fact]
        public async Task GetAllOrders_Should_Return_Success()
        {
            // Arrange
            var orders = new List<Orders>
            {
                new Orders { Id = 1, CustomerId = 1, ProductId = 1, Quantity = 1, TotalAmount = 50.00M, OrderDate = DateTime.UtcNow },
                new Orders { Id = 2, CustomerId = 2, ProductId = 2, Quantity = 2, TotalAmount = 100.00M, OrderDate = DateTime.UtcNow }
            };
            A.CallTo(() => _fakeService.GetAllOrders()).Returns(orders);

            // Act
            var result = await _controller.GetAllOrders();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedOrders = Assert.IsType<List<Orders>>(okResult.Value);
            Assert.Equal(2, returnedOrders.Count);
        }

        [Fact]
        public async Task GetOrderById_Should_Return_Success()
        {
            // Arrange
            var dummyOrder = new Orders 
            { 
                Id = 42, 
                CustomerId = 1,
                ProductId = 1,
                OrderDate = DateTime.UtcNow,
                Quantity = 2,
                TotalAmount = 100.00M
            };
            A.CallTo(() => _fakeService.GetOrderById(42)).Returns(dummyOrder);

            // Act
            var result = await _controller.GetOrderById(42);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedOrder = Assert.IsType<Orders>(okResult.Value);
            Assert.Equal(42, returnedOrder.Id);
            Assert.Equal(1, returnedOrder.CustomerId);
            Assert.Equal(1, returnedOrder.ProductId);
            Assert.Equal(2, returnedOrder.Quantity);
            Assert.Equal(100.00M, returnedOrder.TotalAmount);
        }

        [Fact]
        public async Task GetOrderById_Should_Return_Not_Found()
        {
            // Arrange
            A.CallTo(() => _fakeService.GetOrderById(99)).Returns((Orders)null);

            // Act
            var result = await _controller.GetOrderById(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateOrder_Should_Return_Success()
        {
            // Arrange
            var createOrderDto = new CreateOrderDto
            {
                CustomerId = 1,
                ProductId = 1,
                Quantity = 2
            };

            var expectedOrder = new Orders
            {
                Id = 1,
                CustomerId = createOrderDto.CustomerId,
                ProductId = createOrderDto.ProductId,
                Quantity = createOrderDto.Quantity,
                TotalAmount = 100.00M,
                OrderDate = DateTime.UtcNow
            };

            var createOrderResult = new CreateOrderResultDto
            {
                IsStockAvailable = true,
                Order = expectedOrder,
                ErrorMessage = null
            };

            A.CallTo(() => _fakeService.CreateOrder(createOrderDto)).Returns(createOrderResult);

            // Act
            var result = await _controller.CreateOrder(createOrderDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedOrder = Assert.IsType<Orders>(createdResult.Value);
            Assert.Equal(expectedOrder.Id, returnedOrder.Id);
            Assert.Equal(expectedOrder.CustomerId, returnedOrder.CustomerId);
            Assert.Equal(expectedOrder.ProductId, returnedOrder.ProductId);
            Assert.Equal(expectedOrder.Quantity, returnedOrder.Quantity);
            Assert.Equal(expectedOrder.TotalAmount, returnedOrder.TotalAmount);
        }

        [Fact]
        public async Task CreateOrder_Should_Return_BadRequest_When_Stock_Not_Available()
        {
            // Arrange
            var createOrderDto = new CreateOrderDto
            {
                CustomerId = 1,
                ProductId = 1,
                Quantity = 2
            };

            var createOrderResult = new CreateOrderResultDto
            {
                IsStockAvailable = false,
                Order = null,
                ErrorMessage = "Stock is not available for the requested product."
            };

            A.CallTo(() => _fakeService.CreateOrder(createOrderDto)).Returns(createOrderResult);

            // Act
            var result = await _controller.CreateOrder(createOrderDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(createOrderResult.ErrorMessage, badRequestResult.Value);
        }

        [Fact]
        public async Task CreateOrder_Should_Return_BadRequest_When_Null_Input()
        {
            // Act
            var result = await _controller.CreateOrder(null);

            // Assert
            Assert.IsType<BadRequestResult>(result);
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

            var updatedOrder = new Orders
            {
                Id = updateOrderDto.Id,
                CustomerId = updateOrderDto.CustomerId,
                ProductId = updateOrderDto.ProductId,
                Quantity = updateOrderDto.Quantity,
                TotalAmount = updateOrderDto.TotalAmount,
                OrderDate = updateOrderDto.OrderDate
            };

            A.CallTo(() => _fakeService.UpdateOrder(1, updateOrderDto)).Returns(updatedOrder);

            // Act
            var result = await _controller.UpdateOrder(1, updateOrderDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedOrder = Assert.IsType<Orders>(okResult.Value);
            Assert.Equal(updateOrderDto.Id, returnedOrder.Id);
            Assert.Equal(updateOrderDto.CustomerId, returnedOrder.CustomerId);
            Assert.Equal(updateOrderDto.ProductId, returnedOrder.ProductId);
            Assert.Equal(updateOrderDto.Quantity, returnedOrder.Quantity);
            Assert.Equal(updateOrderDto.TotalAmount, returnedOrder.TotalAmount);
        }

        [Fact]
        public async Task UpdateOrder_Should_Return_NotFound()
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

            A.CallTo(() => _fakeService.UpdateOrder(99, updateOrderDto)).Returns((Orders)null);

            // Act
            var result = await _controller.UpdateOrder(99, updateOrderDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateOrder_Should_Return_BadRequest_When_Null_Input()
        {
            // Act
            var result = await _controller.UpdateOrder(1, null);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteOrder_Should_Return_NoContent_When_Success()
        {
            // Arrange
            A.CallTo(() => _fakeService.DeleteOrder(1)).Returns(true);

            // Act
            var result = await _controller.DeleteOrder(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteOrder_Should_Return_NotFound_When_Order_Does_Not_Exist()
        {
            // Arrange
            A.CallTo(() => _fakeService.DeleteOrder(99)).Returns(false);

            // Act
            var result = await _controller.DeleteOrder(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
} 