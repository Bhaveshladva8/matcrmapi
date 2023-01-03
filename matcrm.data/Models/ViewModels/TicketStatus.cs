using System.Collections.Generic;

namespace matcrm.data.Models.ViewModels {
    public class TicketStatus {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class TicketStatusResult {
        public List<TicketStatus> Result { get; set; }
    }
}