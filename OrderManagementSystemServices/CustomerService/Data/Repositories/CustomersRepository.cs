using CustomerService.Data.Context;
using CustomerService.Data.Entities;
using CustomerService.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Data.Repositories
{
    public class CustomersRepository : ICustomersRepository
    {
        private readonly CustomersContext _context;

        public CustomersRepository(CustomersContext context)
        {
            _context = context;
        }  

        public async Task<Customers> GetCustomerByIdAsync(int id)
        {
            return await _context.Customers.FindAsync(id);
        }

        public async Task<Customers> CreateCustomerAsync(Customers customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<Customers> GetCustomerByEmailAsync(string email)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<IEnumerable<Customers>> GetAllCustomersAsync()
        {
            return await _context.Customers.ToListAsync();
        }
    }
}
