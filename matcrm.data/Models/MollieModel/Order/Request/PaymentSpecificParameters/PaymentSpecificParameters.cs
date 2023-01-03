namespace matcrm.data.Models.MollieModel.Order.Request.PaymentSpecificParameters {
    public class PaymentSpecificParameters {
        public string CustomerId { get; set; }
        /// <summary>
        /// See the matcrm.data.Models.MollieModel.Payment.SequenceType class for a full list of known values.
        /// </summary>
        public string SequenceType { get; set; }
        public string WebhookUrl { get; set; }
    }
}
