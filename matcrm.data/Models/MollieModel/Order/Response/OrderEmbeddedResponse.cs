using System.Collections.Generic;
using matcrm.data.Models.MollieModel.Payment.Response;
using matcrm.data.Models.MollieModel.Refund;
using matcrm.data.Models.MollieModel.Shipment;

namespace matcrm.data.Models.MollieModel.Order.Response {

    public class OrderEmbeddedResponse : IResponseObject {

        public IEnumerable<PaymentResponse> Payments { get; set; }

        public IEnumerable<RefundResponse> Refunds { get; set; }

        public IEnumerable<ShipmentResponse> Shipments { get; set; }
    }
}