using System.Collections.Generic;
using matcrm.data.Models.ViewModels.Calendar;

namespace matcrm.data.Models.Response
{
    public class GoogleCalendarDeleteResponse
    {
        public GoogleCalendarDeleteResponse(){
            Events = new List<GoogleMicrosoftCalendarEventVM>();
        }
        public long? Id { get; set; }
        public int? CreatedBy { get; set; }
        public bool IsDefault { get; set; }
        public bool IsSelect { get; set; }
        public bool IsDeleted { get; set; }
        public List<GoogleMicrosoftCalendarEventVM> Events { get; set; }
    }
}