namespace ProductService.Data.Entities
{
    public class Products
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int AvailableStock { get; set; }
    }
} 