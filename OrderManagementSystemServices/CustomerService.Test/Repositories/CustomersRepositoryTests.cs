using CustomerService.Data.Context;
using CustomerService.Data.Entities;
using CustomerService.Data.Repositories;
using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CustomerService.Test.Repositories
{
    public class CustomersRepositoryTests
    {
        private readonly CustomersContext _context;
        private readonly CustomersRepository _repository;
        private readonly DbSet<Customers> _dbSet;

        public CustomersRepositoryTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<CustomersContext>()
                .UseInMemoryDatabase(databaseName: $"CustomersDb_{Guid.NewGuid()}")
                .Options;

            _context = new CustomersContext(options);
            _repository = new CustomersRepository(_context);
        }

        [Fact]
        public async Task GetCustomerByIdAsync_ExistingId_ReturnsCustomer()
        {
            // Arrange
            var customer = new Customers
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetCustomerByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(customer.Id, result.Id);
            Assert.Equal(customer.FirstName, result.FirstName);
            Assert.Equal(customer.LastName, result.LastName);
            Assert.Equal(customer.Email, result.Email);
        }

        [Fact]
        public async Task GetCustomerByIdAsync_NonExistingId_ReturnsNull()
        {
            // Act
            var result = await _repository.GetCustomerByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateCustomerAsync_ValidCustomer_ReturnsCreatedCustomer()
        {
            // Arrange
            var customer = new Customers
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com"
            };

            // Act
            var result = await _repository.CreateCustomerAsync(customer);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(0, result.Id); // Should have an assigned ID
            Assert.Equal(customer.FirstName, result.FirstName);
            Assert.Equal(customer.LastName, result.LastName);
            Assert.Equal(customer.Email, result.Email);

            // Verify it's in the database
            var dbCustomer = await _context.Customers.FindAsync(result.Id);
            Assert.NotNull(dbCustomer);
        }

        [Fact]
        public async Task GetCustomerByEmailAsync_ExistingEmail_ReturnsCustomer()
        {
            // Arrange
            var customer = new Customers
            {
                FirstName = "Alice",
                LastName = "Johnson",
                Email = "alice.johnson@example.com"
            };
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetCustomerByEmailAsync(customer.Email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(customer.Email, result.Email);
            Assert.Equal(customer.FirstName, result.FirstName);
            Assert.Equal(customer.LastName, result.LastName);
        }

        [Fact]
        public async Task GetCustomerByEmailAsync_NonExistingEmail_ReturnsNull()
        {
            // Act
            var result = await _repository.GetCustomerByEmailAsync("nonexistent@example.com");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllCustomersAsync_ReturnsAllCustomers()
        {
            // Arrange
            var customers = new List<Customers>
            {
                new Customers { FirstName = "Bob", LastName = "Wilson", Email = "bob.wilson@example.com" },
                new Customers { FirstName = "Carol", LastName = "Brown", Email = "carol.brown@example.com" }
            };
            await _context.Customers.AddRangeAsync(customers);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllCustomersAsync();

            // Assert
            var customersList = result.ToList();
            Assert.Equal(customers.Count, customersList.Count);
            Assert.All(customers, customer =>
                Assert.Contains(customersList, c =>
                    c.FirstName == customer.FirstName &&
                    c.LastName == customer.LastName &&
                    c.Email == customer.Email));
        }

        [Fact]
        public async Task GetAllCustomersAsync_EmptyDatabase_ReturnsEmptyList()
        {
            // Act
            var result = await _repository.GetAllCustomersAsync();

            // Assert
            Assert.Empty(result);
        }
    }
} 