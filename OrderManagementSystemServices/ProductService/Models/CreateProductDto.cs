using System.ComponentModel.DataAnnotations;

namespace ProductService.Models
{
    public class CreateProductDto
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Name can only contain letters, numbers, and spaces.")]
        public string Name { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "The available stock must be zero or a positive number.")]
        public int AvailableStock { get; set; }
    }
}
