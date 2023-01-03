namespace matcrm.data.Models.MollieModel.Order.SpecificParameters
{
    public class SepaDirectDebitSpecificParameters : PaymentSpecificParameters
    {
        /// <summary>
        /// Optional - IBAN of the account holder.
        /// </summary>
        public string ConsumerAccount { get; set; }
    }
}
