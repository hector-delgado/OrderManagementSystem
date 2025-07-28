using System.Threading.Tasks;
using OrderService.Data.Entities;

namespace OrderService.Data.Repositories.Interfaces
{
    public interface IOrdersRepository
    {
        Task<Orders> GetOrderByIdAsync(int id);
        
        Task<Orders> AddOrderAsync(Orders order);
        
        Task<Orders> UpdateOrderAsync(int id, Orders order);

        Task<bool> DeleteOrderAsync(int id);

        Task<IEnumerable<Orders>> GetAllOrdersAsync();
    }
} 