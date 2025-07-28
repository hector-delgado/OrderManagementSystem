
using CustomerService.Data.Entities;
using CustomerService.Data.Repositories.Interfaces;
using CustomerService.Models;

namespace CustomerService.Services.Implementation
{
    public class CustomersService : ICustomersService
    {
        private readonly ICustomersRepository _customersRepository;

        public CustomersService(ICustomersRepository customersRepository)
        {
            _customersRepository = customersRepository;
        }

        public async Task<Customers> GetCustomerById(int id)
        {
            return await _customersRepository.GetCustomerByIdAsync(id);
        }

        public async Task<Customers> CreateCustomer(CreateCustomerDto customer)
        {
            try
            {
                // Check if a customer with the same email already exists
                var existingCustomer = await _customersRepository.GetCustomerByEmailAsync(customer.Email);
                if (existingCustomer != null)
                {
                    return null;
                }

                var entity = new Customers
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email
                };

                return await _customersRepository.CreateCustomerAsync(entity);
            }
            catch (Exception ex)
            {
                // Check for SQL constraint violation (unique email)
                if (ex.InnerException != null && ex.InnerException.Message.Contains("UNIQUE"))
                {
                    return null;
                }
                throw;
            }
        }

        public async Task<IEnumerable<Customers>> GetAllCustomers()
        {
            return await _customersRepository.GetAllCustomersAsync();
        }
    }
} 