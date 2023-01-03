using System.Collections.Generic;

namespace matcrm.data.Models.MollieModel.Order {
    public class OrderLineCancellationRequest {
        public IEnumerable<OrderLineDetails> Lines { get; set; }
    }
}
