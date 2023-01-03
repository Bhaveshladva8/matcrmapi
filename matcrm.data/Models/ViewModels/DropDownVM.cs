using System.Collections.Generic;

namespace matcrm.data.Models.ViewModels
{
    public class DropDownVM
    {
        public List<CustomerVM> Customers { get; set; }
        public List<TicketCategory> TicketCategories { get; set; }
        public List<TicketType> TicketTypes { get; set; }
    }
}