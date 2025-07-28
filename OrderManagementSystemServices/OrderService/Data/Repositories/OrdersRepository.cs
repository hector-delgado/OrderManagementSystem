using Microsoft.EntityFrameworkCore;
using OrderService.Data.Entities;
using OrderService.Data.Repositories.Interfaces;
using OrderService.Data.Context;

namespace OrderService.Data.Repositories
{
    public class OrdersRepository : IOrdersRepository
    {
        private readonly OrdersContext _context;

        public OrdersRepository(OrdersContext context)
        {
            _context = context;
        }

        public async Task<Orders> GetOrderByIdAsync(int id)
        {
            return await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Orders>> GetAllOrdersAsync()
        {
            return await _context.Orders.ToListAsync();
        }

        public async Task<Orders> AddOrderAsync(Orders order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Orders> UpdateOrderAsync(int id, Orders order)
        {
            var existingOrder = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
            if (existingOrder == null)
            {
                return null;
            }
            existingOrder.CustomerId = order.CustomerId;
            existingOrder.ProductId = order.ProductId;
            existingOrder.Quantity = order.Quantity;
            existingOrder.TotalAmount = order.TotalAmount;
            existingOrder.OrderDate = order.OrderDate;
            await _context.SaveChangesAsync();
            return existingOrder;
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return false;
            }
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
