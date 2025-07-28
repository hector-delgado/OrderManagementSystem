namespace OrderService.Services
{
    using OrderService.Data.Entities;
    using OrderService.Models;

    public interface IOrdersService
    {
        public Task<Orders> GetOrderById(int id);
        
        public Task<CreateOrderResultDto> CreateOrder(CreateOrderDto order);
        
        public Task<Orders> UpdateOrder(int id, UpdateOrderDto order);

        public Task<bool> DeleteOrder(int id);

        public Task<IEnumerable<Orders>> GetAllOrders();
    }
} 