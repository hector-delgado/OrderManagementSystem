namespace OrderService.Models
{
    public class ValidateProductResponseDto
    {
        public int ProductId { get; set; }
        public int AvailableStock { get; set; }
        public bool InStock { get; set; }
        public decimal totalAmount { get; set; }
    }
}
