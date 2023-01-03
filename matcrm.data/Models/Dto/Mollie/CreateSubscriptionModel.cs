using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace matcrm.data.Models.Dto.Mollie
{
    public class CreateSubscriptionModel
    {
        public CreateSubscriptionModel(){
            Amount = new AmountClass();
        }
        [Required]
        public string CustomerId { get; set; }

        // [Required]
        // [Range(0.01, 1000, ErrorMessage = "Please enter an amount between 0.01 and 1000")]
        // [DecimalPlaces(2)]
        public AmountClass Amount { get; set; }

        // [Required]
        // [StaticStringList(typeof(PaymentCurrency))]
        // public string Currency { get; set; }

        [Range(1, 10)]
        public int? Times { get; set; }

        [Range(1, 20, ErrorMessage = "Please enter a interval number between 1 and 20")]
        [Required]
        [Display(Name = "Interval amount")]
        public int? IntervalAmount { get; set; }

        [Required]
        [Display(Name = "Interval period")]
        public IntervalPeriod IntervalPeriod { get; set; }

        [Required]
        public string Description { get; set; }
        public string startDate { get; set; }
    }

    public class AmountClass
    {
        [Required]
        // [StaticStringList(typeof(Currency))]
        public string Currency { get; set; }
        [Required]
        [Range(0.01, 1000, ErrorMessage = "Please enter an amount between 0.01 and 1000")]
        [DecimalPlaces(2)]
        public decimal value { get; set; }
    }

     public class CreateSubscriptionClass
    {
        public CreateSubscriptionClass(){
            Amount = new MollieAmountClass();
        }
        [Required]
        public string CustomerId { get; set; }

        // [Required]
        // [Range(0.01, 1000, ErrorMessage = "Please enter an amount between 0.01 and 1000")]
        // [DecimalPlaces(2)]
        public MollieAmountClass Amount { get; set; }

        // [Required]
        // [StaticStringList(typeof(PaymentCurrency))]
        // public string Currency { get; set; }

        [Range(1, 10)]
        public int? Times { get; set; }

        [Range(1, 20, ErrorMessage = "Please enter a interval number between 1 and 20")]
        [Required]
        [Display(Name = "Interval amount")]
        public int? IntervalAmount { get; set; }

        [Required]
        [Display(Name = "Interval period")]
        public IntervalPeriod IntervalPeriod { get; set; }

        [Required]
        public string Description { get; set; }
        public string startDate { get; set; }
    }

    public class MollieAmountClass
    {
        [Required]
        // [StaticStringList(typeof(Currency))]
        public string Currency { get; set; }
        [Required]
        public string value { get; set; }
    }

    public enum IntervalPeriod
    {
        Months,
        Weeks,
        Days
    }

    public class DecimalPlacesAttribute : ValidationAttribute
    {
        public int DecimalPlaces { get; }

        public DecimalPlacesAttribute(int decimalPlaces)
        {
            DecimalPlaces = decimalPlaces;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            decimal amount = (decimal)value;
            string text = amount.ToString(CultureInfo.InvariantCulture);
            int dotIndex = text.IndexOf('.');
            var decimals = text.Length - dotIndex - 1;
            var places = DecimalPlaces switch
            {
                0 => "without decimal places",
                1 => "with one decimal place",
                _ => $"with {DecimalPlaces} decimal places"
            };
            return dotIndex < 0 || dotIndex != text.LastIndexOf('.') || decimals != DecimalPlaces
                ? new ValidationResult(ErrorMessage ?? $"Please enter an amount {places}")
                : ValidationResult.Success;
        }
    }
}