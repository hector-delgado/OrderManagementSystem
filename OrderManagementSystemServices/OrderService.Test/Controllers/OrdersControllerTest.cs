using System;
using System.Threading.Tasks;
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
        [Fact]
        public async Task GetOrderById_Should_Return_Success()
        {
            // Arrange
            var fakeService = A.Fake<IOrdersService>();
            var fakeLogger = A.Fake<ILogger<OrdersController>>();
            var dummyOrder = new Orders 
            { 
                Id = 42, 
                CustomerId = 1,
                ProductId = 1,
                OrderDate = DateTime.UtcNow,
                Quantity = 2,
                TotalAmount = 100.00M
            };
            A.CallTo(() => fakeService.GetOrderById(42)).Returns(dummyOrder);
            var controller = new OrdersController(fakeService, fakeLogger);

            // Act
            var result = await controller.GetOrderById(42);

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
            var fakeService = A.Fake<IOrdersService>();
            var fakeLogger = A.Fake<ILogger<OrdersController>>();
            A.CallTo(() => fakeService.GetOrderById(99)).Returns((Orders)null);
            var controller = new OrdersController(fakeService, fakeLogger);

            // Act
            var result = await controller.GetOrderById(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateOrder_Should_Return_Success()
        {
            // Arrange
            var fakeService = A.Fake<IOrdersService>();
            var fakeLogger = A.Fake<ILogger<OrdersController>>();
            
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

            A.CallTo(() => fakeService.CreateOrder(createOrderDto)).Returns(expectedOrder);
            var controller = new OrdersController(fakeService, fakeLogger);

            // Act
            var result = await controller.CreateOrder(createOrderDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedOrder = Assert.IsType<Orders>(createdResult.Value);
            Assert.Equal(expectedOrder.Id, returnedOrder.Id);
            Assert.Equal(expectedOrder.CustomerId, returnedOrder.CustomerId);
            Assert.Equal(expectedOrder.ProductId, returnedOrder.ProductId);
            Assert.Equal(expectedOrder.Quantity, returnedOrder.Quantity);
            Assert.Equal(expectedOrder.TotalAmount, returnedOrder.TotalAmount);
        }
    }
} 