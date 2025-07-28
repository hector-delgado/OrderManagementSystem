using OrderService.Data.Entities;

namespace OrderService.Models
{
    public class CreateOrderResultDto
    {
        public bool IsStockAvailable { get; set; }
        public Orders? Order { get; set; }

        public string ErrorMessage { get; set; }

    }
}
