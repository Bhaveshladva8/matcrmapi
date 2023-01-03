using System.Collections.Generic;

namespace matcrm.data.Models.Request
{
    public class GoogleCalendarSelectRequest
    {
        public GoogleCalendarSelectRequest(){
            SelectedIds = new List<long>();
        }
        public List<long> SelectedIds { get; set; }
    }
}