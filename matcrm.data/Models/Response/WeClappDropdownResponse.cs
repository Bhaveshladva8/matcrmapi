using System.Collections.Generic;
using matcrm.data.Models.ViewModels;

namespace matcrm.data.Models.Response
{
    public class WeClappDropdownResponse
    {
        public List<CustomerVM> Customers { get; set; }
        public List<TicketCategory> TicketCategories { get; set; }
        public List<TicketType> TicketTypes { get; set; }
    }
}