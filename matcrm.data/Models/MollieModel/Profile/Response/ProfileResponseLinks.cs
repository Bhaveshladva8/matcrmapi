using matcrm.data.Models.MollieModel.Chargeback;
using matcrm.data.Models.MollieModel.List;
using matcrm.data.Models.MollieModel.Payment.Response;
using matcrm.data.Models.MollieModel.PaymentMethod;
using matcrm.data.Models.MollieModel.Refund;
using matcrm.data.Models.MollieModel.Url;

namespace matcrm.data.Models.MollieModel.Profile.Response {
    public class ProfileResponseLinks {
        public UrlObjectLink<ProfileResponse> Self { get; set; }
        public UrlObjectLink<ListResponse<ChargebackResponse>> Chargebacks { get; set; }
        public UrlObjectLink<ListResponse<PaymentResponse>> Methods { get; set; }
        public UrlObjectLink<ListResponse<PaymentMethodResponse>> Payments { get; set; }
        public UrlObjectLink<ListResponse<RefundResponse>> Refunds { get; set; }
        public UrlLink CheckoutPreviewUrl { get; set; }
        public UrlLink Documentation { get; set; }
    }
}