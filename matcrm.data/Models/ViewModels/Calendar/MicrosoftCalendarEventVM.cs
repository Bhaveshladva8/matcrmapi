using System;
using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.ViewModels.Calendar
{
    public class MicrosoftCalendarEventVM : CommonResponse
    {
        public MicrosoftCalendarEventVM()
        {
            start = new MSEventDate();
            end = new MSEventDate();
            body = new MSEventBody();
            attendees = new List<MSEventAttendee>();
            location = new MSEventLocation();
            locations = new List<MSEventLocation>();
            organizer = new MSEventOrganizer();
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
        public MSEventBody body { get; set; }
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
        public MSEventDate start { get; set; }
        public MSEventDate end { get; set; }
        public List<MSEventAttendee> attendees { get; set; }
        public MSEventLocation location { get; set; }
        public List<MSEventLocation> locations { get; set; }
        public MSEventOrganizer organizer { get; set; }
        public string Code { get; set; }
        public long? IntProviderAppSecretId { get; set; }
    }

    public class MSCalendarEvent : CommonResponse
    {
        public MSCalendarEvent()
        {
            value = new List<MicrosoftCalendarEventVM>();
            Events = new List<GoogleCalendarEventVM>();
            Items = new List<GoogleCalendarEventVM>();
        }
        public List<MicrosoftCalendarEventVM> value { get; set; }
        public string kind { get; set; }
        public string etag { get; set; }
        public string id { get; set; }
        public string summary { get; set; }
        public string description { get; set; }
        public string timeZone { get; set; }
        public string colorId { get; set; }
        public string backgroundColor { get; set; }
        public string foregroundColor { get; set; }
        public bool selected { get; set; }
        public List<GoogleCalendarEventVM> Events { get; set; }
         public List<GoogleCalendarEventVM> Items { get; set; }

    }

    public class MSEventDate
    {
        public string dateTime { get; set; }
        public string timeZone { get; set; }
    }

    public class MSEventAttendee
    {
        public MSEventAttendee()
        {
            status = new MSEventAttendeeStatus();
            emailAddress = new MSEventEmail();
        }
        public string type { get; set; }
        public MSEventAttendeeStatus status { get; set; }
        public MSEventEmail emailAddress { get; set; }
    }

    public class MSEventAttendeeStatus
    {
        public string response { get; set; }
        public string time { get; set; }
    }

    public class MSEventEmail
    {
        public string name { get; set; }
        public string address { get; set; }
    }

    public class MSEventLocation
    {
        public string displayName { get; set; }
        public string locationType { get; set; }
        public string uniqueId { get; set; }
        public string uniqueIdType { get; set; }
    }
    public class MSEventBody
    {
        public string contentType { get; set; }
        public string content { get; set; }
    }

    public class MSEventOrganizer
    {
        public MSEventOrganizer()
        {
            emailAddress = new MSEventEmail();
        }
        public MSEventEmail emailAddress { get; set; }
    }
}