using System;
using System.Collections.Generic;
using matcrm.data.Models.ViewModels.Calendar;

namespace matcrm.data.Models.Response
{
    public class CalendarAddUpdateEventResponse
    {
        public CalendarAddUpdateEventResponse(){
            body = new CalendarEventBody();
            start = new CalendarEventDate();
            end = new CalendarEventDate();
            attendees = new List<GoogleMSCalendarAttendee>();
            attendee = new List<GoogleMSCalendarAttendee>();
            // location = new CalendarEventLocation();
            // locations = new List<CalendarEventLocation>();
            creator = new EventCreatorOrganizerTest();
            organizer = new EventCreatorOrganizerTest();
        }
        public string id { get; set; }        
        public bool isReminderOn { get; set; }
        public bool hasAttachments { get; set; }        
        public CalendarEventBody body { get; set; }        
        public bool isAllDay { get; set; }
        public bool isCancelled { get; set; }
        public bool isOrganizer { get; set; }
        public bool responseRequested { get; set; }
        public DateTime? startDate { get; set; }
        public string? startTime { get; set; }
        public string? endTime { get; set; }
        public bool hideAttendees { get; set; }
        public CalendarEventDate start { get; set; }
        public CalendarEventDate end { get; set; }
        public List<GoogleMSCalendarAttendee> attendees { get; set; }
        public EventCreatorOrganizerTest organizer { get; set; }
        public long? IntProviderAppSecretId { get; set; }
        public string summary { get; set; }
        public string description { get; set; }
        public EventCreatorOrganizerTest creator { get; set; }
        //public int? UserId { get; set; }
        //public int? TenantId { get; set; }
        public List<GoogleMSCalendarAttendee> attendee { get; set; }
        public string Provider { get; set; }
        public string ProviderApp { get; set; }
        public string SelectedEmail { get; set; }
        
    }
}