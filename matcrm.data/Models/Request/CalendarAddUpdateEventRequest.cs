using System;
using System.Collections.Generic;
using matcrm.data.Models.ViewModels.Calendar;

namespace matcrm.data.Models.Request
{
    public class CalendarAddUpdateEventRequest
    {
        public CalendarAddUpdateEventRequest(){
            
            start = new CalendarEventDate();
            end = new CalendarEventDate();
            attendee = new List<GoogleMSCalendarAttendee>();
            
        }
        public string id { get; set; }
        public DateTime? startDate { get; set; }
        public string? startTime { get; set; }
        public string? endTime { get; set; }        
        public CalendarEventDate start { get; set; }
        public CalendarEventDate end { get; set; }
        public long? IntProviderAppSecretId { get; set; }
        public string summary { get; set; }
        public string description { get; set; }
        //public int? UserId { get; set; }
        //public int? TenantId { get; set; }
        public bool isAllDay { get; set; }
        public List<GoogleMSCalendarAttendee> attendee { get; set; }
        
    }
}