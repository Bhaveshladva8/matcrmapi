using System;
using System.Collections.Generic;

namespace matcrm.data.Models.ViewModels
{
    public class GoogleCalendarEventVM
    {
        public GoogleCalendarEventVM()
        {
            creator = new EventCreatorOrganizer();
            organizer = new EventCreatorOrganizer();
            start = new EventDate();
            end = new EventDate();
            reminders = new EventReminder();
            attendees = new List<CalendarAttendee>();
        }
        public string kind { get; set; }
        public string etag { get; set; }
        public string id { get; set; }
        public string status { get; set; }
        public string htmlLink { get; set; }
        public string created { get; set; }
        public string summary { get; set; }
        public string colorId { get; set; }
        public string description { get; set; }
        public string iCalUID { get; set; }
        public string eventType { get; set; }
        public string access_token { get; set; }
        public EventCreatorOrganizer creator { get; set; }
        public EventCreatorOrganizer organizer { get; set; }
        public EventDate start { get; set; }
        public EventDate end { get; set; }
        public EventReminder reminders { get; set; }
        public string Token { get; set; }
        public int? UserId { get; set; }
        public int? TenantId { get; set; }
        public string email { get; set; }
        public string[] Recurrence { get; set; }
        public List<CalendarAttendee> attendees { get; set; }
        public string Location { get; set; }
        public string error { get; set; }
        public string error_description { get; set; }
        public long? IntProviderAppSecretId { get; set; }
        public bool isAllDay { get; set; }
        public bool isCancelled { get; set; }
    }

    public class EventCreatorOrganizer
    {
        // public string id { get; set; }
        public string email { get; set; }
        // public string displayName { get; set; }
        // public bool self { get; set; }
    }

    public class EventDate
    {
        // public string id { get; set; }
        public string date { get; set; }
        public string dateTime { get; set; }
        public string timeZone { get; set; }
    }

    public class EventReminder
    {
        public bool useDefault { get; set; }
    }

    public class GoogleCalendarEventPostVM
    {
        public GoogleCalendarEventPostVM()
        {
            creator = new EventCreatorOrganizer();
            attendees = new List<CalendarAttendee>();
        }
        public string summary { get; set; }
        public string description { get; set; }
        public string Location { get; set; }
        public string colorId { get; set; }
        public EventDate start { get; set; }
        public EventDate end { get; set; }
        public string[] Recurrence { get; set; }
        public List<CalendarAttendee> attendees { get; set; }
        public EventCreatorOrganizer creator { get; set; }
    }

    public class GoogleCalendarUser
    {
        public string sub { get; set; }
        public string picture { get; set; }
        public string email { get; set; }
        public bool email_verified { get; set; }
    }

    public class GoogleCalendarTokenInfo
    {
        public string azp { get; set; }
        public string aud { get; set; }
        public string sub { get; set; }
        public string scope { get; set; }
        public string exp { get; set; }
        public string expires_in { get; set; }
        public string email { get; set; }
        public string email_verified { get; set; }
        public string access_type { get; set; }
        public string error { get; set; }
        public string error_description { get; set; }
    }

    public class CalendarAttendee
    {
        public string email { get; set; }
    }
}