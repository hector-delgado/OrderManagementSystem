using OrderService.Models;
using System.Threading.Tasks;

namespace OrderService.RabbitMQ
{
    public interface IRabbitMqProductCheck
    {
        Task<ValidateProductResponseDto> CheckProductStockAvailabilityAsync(int productId, int requestedQuantity);
    }
}
