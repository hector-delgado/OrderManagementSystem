using Xunit;
using FakeItEasy;
using CustomerService.Services.Implementation;
using CustomerService.Data.Repositories.Interfaces;
using CustomerService.Data.Entities;

namespace CustomerService.Test.Services
{
    public class CustomersServiceTest
    {
        [Fact]
        public async Task GetCustomerById_Should_Return_Success()
        {
            // Arrange
            var fakeRepo = A.Fake<ICustomersRepository>();
            var dummyCustomer = new Customers { Id = 7, FirstName = "Alice", LastName = "Wonder", Email = "alice@wonder.com" };
            A.CallTo(() => fakeRepo.GetCustomerByIdAsync(7)).Returns(dummyCustomer);
            var service = new CustomersService(fakeRepo);

            // Act
            var result = await service.GetCustomerById(7);

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
            var fakeRepo = A.Fake<ICustomersRepository>();
            A.CallTo(() => fakeRepo.GetCustomerByIdAsync(99)).Returns((Customers)null);
            var service = new CustomersService(fakeRepo);

            // Act
            var result = await service.GetCustomerById(99);

            // Assert
            Assert.Null(result);
        }
    }
}
