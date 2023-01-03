using System.ComponentModel.DataAnnotations;
using matcrm.data.Models.MollieModel;
// using Mollie.WebApplicationCoreExample.Framework.Validators;

namespace matcrm.data.Models.Dto.Mollie {
    public class CreatePaymentModel {
        [Required]
        [Range(0.01, 1000, ErrorMessage = "Please enter an amount between 0.01 and 1000")]
        // [DecimalPlaces(2)]
        public decimal Amount { get; set; }

        [Required]
        // [StaticStringList(typeof(Currency))]
        public string Currency { get; set; }

        [Required]
        public string Description { get; set; }
        public string WebhookUrl { get; set; }
         public string CustomerId { get; set; }
         public string SequenceType { get; set; }
    }
}
