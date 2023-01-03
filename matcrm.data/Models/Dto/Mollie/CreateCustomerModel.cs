using System.ComponentModel.DataAnnotations;

namespace matcrm.data.Models.Dto.Mollie
{
    public class CreateCustomerModel
    {
        // [Required]
        public string Name { get; set; }

        // [EmailAddress]
        public string Email { get; set; }
        public string CustomerId { get; set; }
    }
}