using matcrm.data.Models.MollieModel.Chargeback;
using matcrm.data.Models.MollieModel.Customer;
using matcrm.data.Models.MollieModel.Invoice;
using matcrm.data.Models.MollieModel.List;
using matcrm.data.Models.MollieModel.Payment.Response;
using matcrm.data.Models.MollieModel.Profile.Response;
using matcrm.data.Models.MollieModel.Refund;
using matcrm.data.Models.MollieModel.Settlement;
using matcrm.data.Models.MollieModel.Url;

namespace matcrm.data.Models.MollieModel.Organization {
    public class OrganizationResponseLinks {
        /// <summary>
        /// The API resource URL of the organization itself.
        /// </summary>
        public UrlObjectLink<OrganizationResponse> Self { get; set; }

        /// <summary>
        /// The API resource URL where the organization’s chargebacks can be retrieved.
        /// </summary>
        public UrlObjectLink<ListResponse<ChargebackResponse>> Chargebacks { get; set; }

        /// <summary>
        /// The API resource URL where the organization’s customers can be retrieved.
        /// </summary>
        public UrlObjectLink<ListResponse<CustomerResponse>> Customers { get; set; }

        /// <summary>
        /// The API resource URL where the organization’s invoices can be retrieved.
        /// </summary>
        public UrlObjectLink<ListResponse<InvoiceResponse>> Invoices { get; set; }

        /// <summary>
        /// The API resource URL where the organization’s payments can be retrieved.
        /// </summary>
        public UrlObjectLink<ListResponse<PaymentResponse>> Payments { get; set; }

        /// <summary>
        /// The API resource URL where the organization’s profiles can be retrieved.
        /// </summary>
        public UrlObjectLink<ListResponse<ProfileResponse>> Profiles { get; set; }

        /// <summary>
        /// The API resource URL where the organization’s refunds can be retrieved.
        /// </summary>
        public UrlObjectLink<ListResponse<RefundResponse>> Refunds { get; set; }

        /// <summary>
        /// The API resource URL where the organization’s settlements can be retrieved.
        /// </summary>
        public UrlObjectLink<ListResponse<SettlementResponse>> Settlements { get; set; }

        /// <summary>
        /// The URL to the organization dashboard
        /// </summary>
        public UrlLink Dashboard { get; set; }

        /// <summary>
        /// The URL to the payment method retrieval endpoint documentation.
        /// </summary>
        public UrlLink Documentation { get; set; }
    }
}