using System.ComponentModel.DataAnnotations;

namespace OrderService.Models
{
    public class CreateOrderDto
    {
        [Required]
        public int CustomerId { get; set; } // From Customer List in UI

        [Required]
        public int ProductId { get; set; } // From Product List in UI

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
