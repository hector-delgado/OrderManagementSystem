using CustomerService.Data.Entities;

namespace CustomerService.Data.Repositories.Interfaces
{
    public interface ICustomersRepository
    {
        Task<Customers> GetCustomerByIdAsync(int id);
        Task<Customers> CreateCustomerAsync(Customers customer);
        Task<Customers> GetCustomerByEmailAsync(string email);
        Task<IEnumerable<Customers>> GetAllCustomersAsync();
    }
} 