namespace ProductService.Models
{
    public class UpdateProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int AvailableStock { get; set; }
    }
}
