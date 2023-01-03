using System.Collections.Generic;

namespace matcrm.data.Models.Response
{
    public class GoogleCalendarSelectResponse
    {
        public GoogleCalendarSelectResponse(){
            SelectedIds = new List<long>();
        }
        public List<long> SelectedIds { get; set; }
    }
}