using System.Collections.Generic;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Response
{
    public class DashboardNewInfo
    {
        public DashboardNewInfo()
        {
            FormActions = new List<OneClappFormAction>();
        }
        public long Persons { get; set; }
        public long Leads { get; set; }
        public long Organizations { get; set; }
        public long GoogleCalendars { get; set; }
        public List<OneClappFormAction> FormActions { get; set; }
    }
}