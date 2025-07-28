namespace CustomerService.Services
{
    using CustomerService.Data.Entities;
    using CustomerService.Models;
    public interface ICustomersService
    {
        Task<Customers> GetCustomerById(int id);
        Task<Customers> CreateCustomer(CreateCustomerDto customer);
        Task<IEnumerable<Customers>> GetAllCustomers();
    }
} 