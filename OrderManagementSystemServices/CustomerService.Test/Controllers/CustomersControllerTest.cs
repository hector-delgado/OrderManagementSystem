using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CustomerService.Controllers;
using CustomerService.Services;
using CustomerService.Data.Entities;

namespace CustomerService.Test.Controllers
{
    public class CustomersControllerTest
    {
        [Fact]
        public async Task GetCustomerById_Should_Success()
        {
            // Arrange
            var fakeService = A.Fake<ICustomersService>();
            var fakeLogger = A.Fake<ILogger<CustomersController>>();
            var dummyCustomer = new Customers { Id = 42, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com" };
            A.CallTo(() => fakeService.GetCustomerById(42)).Returns(dummyCustomer);
            var controller = new CustomersController(fakeService, fakeLogger);

            // Act
            var result = await controller.GetCustomerById(42);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCustomer = Assert.IsType<Customers>(okResult.Value);
            Assert.Equal(42, returnedCustomer.Id);
            Assert.Equal("Jane", returnedCustomer.FirstName);
            Assert.Equal("Smith", returnedCustomer.LastName);
            Assert.Equal("jane.smith@example.com", returnedCustomer.Email);
        }

        [Fact]
        public async Task GetCustomerById_Should_Return_Not_Found()
        {
            // Arrange
            var fakeService = A.Fake<ICustomersService>();
            var fakeLogger = A.Fake<ILogger<CustomersController>>();
            A.CallTo(() => fakeService.GetCustomerById(99)).Returns((Customers)null);
            var controller = new CustomersController(fakeService, fakeLogger);

            // Act
            var result = await controller.GetCustomerById(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
