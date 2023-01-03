using matcrm.data.Models.MollieModel.Customer;
using matcrm.data.Models.MollieModel.List;
using matcrm.data.Models.MollieModel.Payment.Response;
using matcrm.data.Models.MollieModel.Profile.Response;
using matcrm.data.Models.MollieModel.Url;

namespace matcrm.data.Models.MollieModel.Subscription {
    public class SubscriptionResponseLinks {
        /// <summary>
        ///     The API resource URL of the subscription itself.
        /// </summary>
        public UrlObjectLink<SubscriptionResponse> Self { get; set; }

        /// <summary>
        /// The API resource URL of the customer the subscription is for.
        /// </summary>
        public UrlObjectLink<CustomerResponse> Customer { get; set; }

        /// <summary>
        /// The API resource URL of the payments that are created by this subscription. Not present 
        /// if no payments yet created.
        /// </summary>
        public UrlObjectLink<ListResponse<PaymentResponse>> Payments { get; set; }

        /// <summary>
        /// The API resource URL of the website profile on which this subscription was created.
        /// </summary>
        public UrlObjectLink<ProfileResponse> Profile { get; set; }

        /// <summary>
        /// The URL to the subscription retrieval endpoint documentation.
        /// </summary>
        public UrlLink Documentation { get; set; }
    }
}