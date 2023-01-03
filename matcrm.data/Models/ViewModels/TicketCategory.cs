using System.Collections.Generic;

namespace matcrm.data.Models.ViewModels {
    public class TicketCategory {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class TicketCategoryResult {
        public List<TicketCategory> Result { get; set; }
    }
}