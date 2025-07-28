using System.ComponentModel.DataAnnotations;

namespace CustomerService.Models
{
    public class CreateCustomerDto
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z]+(?:\s[a-zA-Z]+)*$", ErrorMessage = "Only letters and single spaces between names are allowed.")]
        public string FirstName { get; set; }
        
        [Required]
        [RegularExpression(@"^[a-zA-Z]+(?:\s[a-zA-Z]+)*$", ErrorMessage = "Only letters and single spaces between names are allowed.")]
        public string LastName { get; set; }

        [Required]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
    }
}
