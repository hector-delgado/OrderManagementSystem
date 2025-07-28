using Xunit;
using FakeItEasy;
using CustomerService.Services.Implementation;
using CustomerService.Data.Repositories.Interfaces;
using CustomerService.Data.Entities;
using CustomerService.Models;

namespace CustomerService.Test.Services
{
    public class CustomersServiceTest
    {
        private readonly ICustomersRepository _fakeRepo;
        private readonly CustomersService _service;

        public CustomersServiceTest()
        {
            _fakeRepo = A.Fake<ICustomersRepository>();
            _service = new CustomersService(_fakeRepo);
        }

        [Fact]
        public async Task GetCustomerById_Should_Return_Success()
        {
            // Arrange
            var dummyCustomer = new Customers { Id = 7, FirstName = "Alice", LastName = "Wonder", Email = "alice@wonder.com" };
            A.CallTo(() => _fakeRepo.GetCustomerByIdAsync(7)).Returns(dummyCustomer);

            // Act
            var result = await _service.GetCustomerById(7);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(7, result.Id);
            Assert.Equal("Alice", result.FirstName);
            Assert.Equal("Wonder", result.LastName);
            Assert.Equal("alice@wonder.com", result.Email);
        }

        [Fact]
        public async Task GetCustomerById_Should_Return_Not_Found()
        {
            // Arrange
            A.CallTo(() => _fakeRepo.GetCustomerByIdAsync(99)).Returns((Customers)null);

            // Act
            var result = await _service.GetCustomerById(99);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateCustomer_Should_Return_Success()
        {
            // Arrange
            var createDto = new CreateCustomerDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };

            var createdCustomer = new Customers
            {
                Id = 1,
                FirstName = createDto.FirstName,
                LastName = createDto.LastName,
                Email = createDto.Email
            };

            A.CallTo(() => _fakeRepo.GetCustomerByEmailAsync(createDto.Email)).Returns((Customers)null);
            A.CallTo(() => _fakeRepo.CreateCustomerAsync(A<Customers>._)).Returns(createdCustomer);

            // Act
            var result = await _service.CreateCustomer(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(createDto.FirstName, result.FirstName);
            Assert.Equal(createDto.LastName, result.LastName);
            Assert.Equal(createDto.Email, result.Email);
        }

        [Fact]
        public async Task CreateCustomer_Should_Return_Null_When_Email_Exists()
        {
            // Arrange
            var createDto = new CreateCustomerDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "existing@example.com"
            };

            var existingCustomer = new Customers
            {
                Id = 1,
                FirstName = "Jane",
                LastName = "Smith",
                Email = createDto.Email
            };

            A.CallTo(() => _fakeRepo.GetCustomerByEmailAsync(createDto.Email)).Returns(existingCustomer);

            // Act
            var result = await _service.CreateCustomer(createDto);

            // Assert
            Assert.Null(result);
            A.CallTo(() => _fakeRepo.CreateCustomerAsync(A<Customers>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task CreateCustomer_Should_Return_Null_On_Unique_Constraint_Violation()
        {
            // Arrange
            var createDto = new CreateCustomerDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };

            A.CallTo(() => _fakeRepo.GetCustomerByEmailAsync(createDto.Email)).Returns((Customers)null);
            A.CallTo(() => _fakeRepo.CreateCustomerAsync(A<Customers>._))
                .Throws(new Exception("Some error", new Exception("UNIQUE constraint failed")));

            // Act
            var result = await _service.CreateCustomer(createDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateCustomer_Should_Throw_On_Other_Errors()
        {
            // Arrange
            var createDto = new CreateCustomerDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };

            A.CallTo(() => _fakeRepo.GetCustomerByEmailAsync(createDto.Email)).Returns((Customers)null);
            A.CallTo(() => _fakeRepo.CreateCustomerAsync(A<Customers>._))
                .Throws(new Exception("Some other error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.CreateCustomer(createDto));
        }

        [Fact]
        public async Task GetAllCustomers_Should_Return_All_Customers()
        {
            // Arrange
            var customers = new List<Customers>
            {
                new Customers { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com" },
                new Customers { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com" }
            };
            A.CallTo(() => _fakeRepo.GetAllCustomersAsync()).Returns(customers);

            // Act
            var result = await _service.GetAllCustomers();

            // Assert
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.Equal(customers[0].Id, resultList[0].Id);
            Assert.Equal(customers[0].Email, resultList[0].Email);
            Assert.Equal(customers[1].Id, resultList[1].Id);
            Assert.Equal(customers[1].Email, resultList[1].Email);
        }

        [Fact]
        public async Task GetAllCustomers_Should_Return_Empty_List_When_No_Customers()
        {
            // Arrange
            A.CallTo(() => _fakeRepo.GetAllCustomersAsync()).Returns(new List<Customers>());

            // Act
            var result = await _service.GetAllCustomers();

            // Assert
            Assert.Empty(result);
        }
    }
}
