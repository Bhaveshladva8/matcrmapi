using System.ComponentModel.DataAnnotations;

namespace matcrm.data.Models.ViewModels {
    public class AuthenticateModel {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        public string Tenant { get; set; }
        public int? TenantId { get; set; }
    }
}