using System.Collections.Generic;

namespace matcrm.data.Models.ViewModels
{
    public class TicketInfoVM
    {
        public List<TimeRecord> TimeTecords { get; set; }
        public Ticket Ticket { get; set; }
    }
}