using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.ViewModels.Calendar
{
    public class CalendarEventVM: CommonResponse
    {
        public CalendarEventVM(){
            MSEvents = new List<MicrosoftCalendarEventVM>();
            GoogleEvents = new List<GoogleCalendarEventVM>();
        }
        public List<MicrosoftCalendarEventVM> MSEvents { get; set; }
        public List<GoogleCalendarEventVM> GoogleEvents { get; set; }
        public string Provider { get; set; }
        public string Email { get; set; }
    }
}