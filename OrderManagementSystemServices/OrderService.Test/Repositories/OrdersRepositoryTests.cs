using OrderService.Data.Context;
using OrderService.Data.Entities;
using OrderService.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace OrderService.Test.Repositories
{
    public class OrdersRepositoryTests
    {
        private readonly OrdersContext _context;
        private readonly OrdersRepository _repository;

        public OrdersRepositoryTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<OrdersContext>()
                .UseInMemoryDatabase(databaseName: $"OrdersDb_{Guid.NewGuid()}")
                .Options;

            _context = new OrdersContext(options);
            _repository = new OrdersRepository(_context);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ExistingId_ReturnsOrder()
        {
            // Arrange
            var order = new Orders
            {
                Id = 1,
                CustomerId = 1,
                ProductId = 1,
                Quantity = 2,
                TotalAmount = 100.00M,
                OrderDate = DateTime.UtcNow
            };
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetOrderByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.Id, result.Id);
            Assert.Equal(order.CustomerId, result.CustomerId);
            Assert.Equal(order.ProductId, result.ProductId);
            Assert.Equal(order.Quantity, result.Quantity);
            Assert.Equal(order.TotalAmount, result.TotalAmount);
            Assert.Equal(order.OrderDate, result.OrderDate);
        }

        [Fact]
        public async Task GetOrderByIdAsync_NonExistingId_ReturnsNull()
        {
            // Act
            var result = await _repository.GetOrderByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllOrdersAsync_ReturnsAllOrders()
        {
            // Arrange
            var orders = new List<Orders>
            {
                new Orders
                {
                    CustomerId = 1,
                    ProductId = 1,
                    Quantity = 2,
                    TotalAmount = 100.00M,
                    OrderDate = DateTime.UtcNow
                },
                new Orders
                {
                    CustomerId = 2,
                    ProductId = 2,
                    Quantity = 1,
                    TotalAmount = 50.00M,
                    OrderDate = DateTime.UtcNow
                }
            };
            await _context.Orders.AddRangeAsync(orders);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllOrdersAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.All(orders, order =>
                Assert.Contains(resultList, r =>
                    r.CustomerId == order.CustomerId &&
                    r.ProductId == order.ProductId &&
                    r.Quantity == order.Quantity &&
                    r.TotalAmount == order.TotalAmount));
        }

        [Fact]
        public async Task GetAllOrdersAsync_EmptyDatabase_ReturnsEmptyList()
        {
            // Act
            var result = await _repository.GetAllOrdersAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddOrderAsync_ValidOrder_ReturnsCreatedOrder()
        {
            // Arrange
            var order = new Orders
            {
                CustomerId = 1,
                ProductId = 1,
                Quantity = 2,
                TotalAmount = 100.00M,
                OrderDate = DateTime.UtcNow
            };

            // Act
            var result = await _repository.AddOrderAsync(order);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(0, result.Id); // Should have an assigned ID
            Assert.Equal(order.CustomerId, result.CustomerId);
            Assert.Equal(order.ProductId, result.ProductId);
            Assert.Equal(order.Quantity, result.Quantity);
            Assert.Equal(order.TotalAmount, result.TotalAmount);

            // Verify it's in the database
            var dbOrder = await _context.Orders.FindAsync(result.Id);
            Assert.NotNull(dbOrder);
        }

        [Fact]
        public async Task UpdateOrderAsync_ExistingOrder_ReturnsUpdatedOrder()
        {
            // Arrange
            var existingOrder = new Orders
            {
                CustomerId = 1,
                ProductId = 1,
                Quantity = 2,
                TotalAmount = 100.00M,
                OrderDate = DateTime.UtcNow.AddDays(-1)
            };
            await _context.Orders.AddAsync(existingOrder);
            await _context.SaveChangesAsync();

            var updatedOrder = new Orders
            {
                Id = existingOrder.Id,
                CustomerId = 2,
                ProductId = 2,
                Quantity = 3,
                TotalAmount = 150.00M,
                OrderDate = DateTime.UtcNow
            };

            // Act
            var result = await _repository.UpdateOrderAsync(existingOrder.Id, updatedOrder);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updatedOrder.CustomerId, result.CustomerId);
            Assert.Equal(updatedOrder.ProductId, result.ProductId);
            Assert.Equal(updatedOrder.Quantity, result.Quantity);
            Assert.Equal(updatedOrder.TotalAmount, result.TotalAmount);
            Assert.Equal(updatedOrder.OrderDate, result.OrderDate);

            // Verify changes are persisted
            var dbOrder = await _context.Orders.FindAsync(existingOrder.Id);
            Assert.NotNull(dbOrder);
            Assert.Equal(updatedOrder.CustomerId, dbOrder.CustomerId);
            Assert.Equal(updatedOrder.ProductId, dbOrder.ProductId);
            Assert.Equal(updatedOrder.Quantity, dbOrder.Quantity);
            Assert.Equal(updatedOrder.TotalAmount, dbOrder.TotalAmount);
            Assert.Equal(updatedOrder.OrderDate, dbOrder.OrderDate);
        }

        [Fact]
        public async Task UpdateOrderAsync_NonExistingOrder_ReturnsNull()
        {
            // Arrange
            var order = new Orders
            {
                Id = 999,
                CustomerId = 1,
                ProductId = 1,
                Quantity = 2,
                TotalAmount = 100.00M,
                OrderDate = DateTime.UtcNow
            };

            // Act
            var result = await _repository.UpdateOrderAsync(999, order);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteOrderAsync_ExistingOrder_ReturnsTrue()
        {
            // Arrange
            var order = new Orders
            {
                CustomerId = 1,
                ProductId = 1,
                Quantity = 2,
                TotalAmount = 100.00M,
                OrderDate = DateTime.UtcNow
            };
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.DeleteOrderAsync(order.Id);

            // Assert
            Assert.True(result);
            var dbOrder = await _context.Orders.FindAsync(order.Id);
            Assert.Null(dbOrder);
        }

        [Fact]
        public async Task DeleteOrderAsync_NonExistingOrder_ReturnsFalse()
        {
            // Act
            var result = await _repository.DeleteOrderAsync(999);

            // Assert
            Assert.False(result);
        }
    }
} 