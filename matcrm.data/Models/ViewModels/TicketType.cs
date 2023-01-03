using System.Collections.Generic;

namespace matcrm.data.Models.ViewModels {
    public class TicketType {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PositionNumber { get; set; }
    }

    public class TicketTypeResult {
        public List<TicketType> Result { get; set; }
    }
}