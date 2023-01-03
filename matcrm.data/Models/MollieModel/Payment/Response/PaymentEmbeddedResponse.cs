using System.Collections.Generic;
using matcrm.data.Models.MollieModel.Chargeback;
using matcrm.data.Models.MollieModel.Refund;

namespace matcrm.data.Models.MollieModel.Payment.Response {
    public class PaymentEmbeddedResponse : IResponseObject {
        public IEnumerable<RefundResponse> Refunds { get; set; }
        public IEnumerable<ChargebackResponse> Chargebacks { get; set; }
    }
}