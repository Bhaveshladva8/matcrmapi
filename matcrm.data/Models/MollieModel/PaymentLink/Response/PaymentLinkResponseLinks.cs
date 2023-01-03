using matcrm.data.Models.MollieModel.Chargeback;
using matcrm.data.Models.MollieModel.Customer;
using matcrm.data.Models.MollieModel.List;
using matcrm.data.Models.MollieModel.Mandate;
using matcrm.data.Models.MollieModel.Settlement;
using matcrm.data.Models.MollieModel.Subscription;
using matcrm.data.Models.MollieModel.Url;

namespace matcrm.data.Models.MollieModel.PaymentLink.Response {
    public class PaymentLinkResponseLinks {
        /// <summary>
        /// The API resource URL of the payment link itself.
        /// </summary>
        public UrlObjectLink<PaymentLinkResponse> Self { get; set; }

        /// <summary>
        /// Direct link to the payment link.
        /// </summary>
        public UrlLink PaymentLink { get; set; } 

        /// <summary>
        ///The URL to the payment link retrieval endpoint documentation.
        /// </summary>
        public UrlLink Documentation { get; set; }
          
    }
}