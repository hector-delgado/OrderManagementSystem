using OrderService.Data.Entities;
using OrderService.Data.Repositories.Interfaces;
using OrderService.Models;
using OrderService.RabbitMQ;
using System.Text.Json;

namespace OrderService.Services.Implementation
{
    public class OrdersService : IOrdersService
    {
        private readonly IOrdersRepository _ordersRepository;
        private readonly IRabbitMqPublisher _rabbitMqPublisher;
        private readonly IRabbitMqProductCheck _productCheck;

        private readonly string _loggingQueueName = "loggingservice";
        private readonly string _ProductQueueName = "productservice";
        public OrdersService(IOrdersRepository ordersRepository, IRabbitMqPublisher rabbitMqPublisher, IRabbitMqProductCheck productCheck)
        {
            _ordersRepository = ordersRepository;
            _rabbitMqPublisher = rabbitMqPublisher;
            _productCheck = productCheck;
        }

        public async Task<Orders> GetOrderById(int id)
        {
            return await _ordersRepository.GetOrderByIdAsync(id);
        }

        public async Task<IEnumerable<Orders>> GetAllOrders()
        {
            return await _ordersRepository.GetAllOrdersAsync();
        }

        public async Task<CreateOrderResultDto> CreateOrder(CreateOrderDto order)
        {
            var result = new CreateOrderResultDto();
            // Check product stock availability using RPC
            var isStockAvailable = await _productCheck.CheckProductStockAvailabilityAsync(order.ProductId, order.Quantity);

            if (isStockAvailable.InStock)
            {
                result.IsStockAvailable = true;

                // If stock is available, create the order
                var newOrder = new Orders
                {
                    CustomerId = order.CustomerId,
                    ProductId = order.ProductId,
                    Quantity = order.Quantity,
                    TotalAmount = isStockAvailable.totalAmount,
                    OrderDate = DateTime.UtcNow
                };

                var resultInsertion = await _ordersRepository.AddOrderAsync(newOrder);
                result.Order = newOrder;

                if (resultInsertion != null)
                {
                    SendMessageToLoggingQueue(order);
                }
            }
            else
            {
                result.IsStockAvailable = false;
                result.ErrorMessage = "Stock is not available for the requested product.";
            }

            return result;
        }

        public async Task<Orders> UpdateOrder(int id, UpdateOrderDto order)
        {
            var existingOrder = await _ordersRepository.GetOrderByIdAsync(id);

            if (existingOrder == null)
            {
                return null;
            }

            existingOrder.ProductId = order.ProductId;
            existingOrder.OrderDate = order.OrderDate;


            return await _ordersRepository.UpdateOrderAsync(id, existingOrder);
        }

        public async Task<bool> DeleteOrder(int id)
        {
            return await _ordersRepository.DeleteOrderAsync(id);
        }

        private void SendMessageToLoggingQueue(CreateOrderDto order)
        {
            try
            {
                // Send RabbitMQ message to create order
                var message = new LoggingQueueMessageDto
                {
                    Message = $"Order Created for CustomerId: {order.CustomerId}, ProductId: {order.ProductId}",
                    Level = LogLevel.Information.ToString(),
                    Exception = null
                };

                var messageJson = JsonSerializer.Serialize(message);

                var callWorked = _rabbitMqPublisher.SendMessageToQueue(_loggingQueueName, messageJson);
                System.Diagnostics.Debug.WriteLine($"RabbitMQ call worked: {callWorked}");
            }
            catch (Exception)
            {
                //Log Error
                System.Diagnostics.Debug.WriteLine($"RabbitMQ call did not worked.");
            }
        }
    }
} 