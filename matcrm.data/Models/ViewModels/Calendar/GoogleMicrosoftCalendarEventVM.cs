using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.ViewModels.Calendar
{
    public class GoogleMicrosoftCalendarEventVM: CommonResponse
    {
        public GoogleMicrosoftCalendarEventVM(){
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
        public string createdDateTime { get; set; }
        public string lastModifiedDateTime { get; set; }
        public string changeKey { get; set; }
        public string transactionId { get; set; }
        public string reminderMinutesBeforeStart { get; set; }
        public bool isReminderOn { get; set; }
        public bool hasAttachments { get; set; }
        public string subject { get; set; }
        public string bodyPreview { get; set; }
        public CalendarEventBody body { get; set; }
        public string importance { get; set; }
        public string sensitivity { get; set; }
        public bool isAllDay { get; set; }
        public bool isCancelled { get; set; }
        public bool isOrganizer { get; set; }
        public bool responseRequested { get; set; }
        public string showAs { get; set; }
        public string type { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string? startTime { get; set; }
        public string? endTime { get; set; }
        public bool hideAttendees { get; set; }
        public CalendarEventDate start { get; set; }
        public CalendarEventDate end { get; set; }
        public List<GoogleMSCalendarAttendee> attendees { get; set; }
        // public CalendarEventLocation location { get; set; }
        // public List<CalendarEventLocation> locations { get; set; }
        public EventCreatorOrganizerTest organizer { get; set; }
        public string Code { get; set; }
        public long? IntProviderAppSecretId { get; set; }

        public string kind { get; set; }
        public string etag { get; set; }
        public string status { get; set; }
        public string htmlLink { get; set; }
        public string created { get; set; }
        public string summary { get; set; }
        public string colorId { get; set; }
        public string description { get; set; }
        public string iCalUID { get; set; }
        public string eventType { get; set; }
        public string access_token { get; set; }
        public EventCreatorOrganizerTest creator { get; set; }
        public CalendarEventReminder reminders { get; set; }
        public string Token { get; set; }
        public int? UserId { get; set; }
        public int? TenantId { get; set; }
        public string email { get; set; }
        public string[] Recurrence { get; set; }
        public List<GoogleMSCalendarAttendee> attendee { get; set; }
        public string Location { get; set; }
        public string error { get; set; }
        public string error_description { get; set; }
    }

      public class CalendarEventBody
    {
        public string contentType { get; set; }
        public string content { get; set; }
    }

//   public class CalendarEventLocation
//     {
//         public string displayName { get; set; }
//         public string locationType { get; set; }
//         public string uniqueId { get; set; }
//         public string uniqueIdType { get; set; }
//     }
    public class CalendarEventDate
    {
        public string dateTime { get; set; }
        // public string date { get; set; }
        public string timeZone { get; set; }
    }

    public class CalendarEventReminder
    {
        public bool useDefault { get; set; }
    }

    public class EventCreatorOrganizerTest
    {
        public string email { get; set; }
        public string name { get; set; }
        public string address { get; set; }
    }

    public class GoogleMSCalendarAttendee
    {
        public string email { get; set; }
        public string type { get; set; }
        public CalendarEventAttendeeStatus status { get; set; }
        public CalendarEventEmail emailAddress { get; set; }
    }

    public class CalendarEventAttendeeStatus
    {
        public string response { get; set; }
        public string time { get; set; }
    }

    public class CalendarEventEmail
    {
        public string name { get; set; }
        public string address { get; set; }
    }

}