using System;
using System.Collections.Generic;
using matcrm.data.Models.Tables;
using matcrm.data.Models.ViewModels.Calendar;

namespace matcrm.data.Models.Response
{
    public class CalendarGetEventsResponse
    {
        public CalendarGetEventsResponse(){
            Events = new List<GoogleMicrosoftCalendarEventVM>();
        }
        public long? Id { get; set; }
        public string? Access_Token { get; set; }
        public long? Expires_In { get; set; }
        public string Refresh_Token { get; set; }
        public string Scope { get; set; }
        public string Token_Type { get; set; }
        public string Id_Token { get; set; } 
               
        public string Email { get; set; }
        public long? IntProviderAppId { get; set; }
        public bool IsDefault { get; set; }
        public bool IsSelect { get; set; }
        public string? Color { get; set; }
        public string? ProviderName { get; set; }
        public string? IntProviderAppName { get; set; }
        public virtual IntProviderApp IntProviderApp { get; set; }
        public List<GoogleMicrosoftCalendarEventVM> Events { get; set; }
        public string Provider { get; set; }
        public string ProviderApp { get; set; }        
        public string SelectedEmail { get; set; }
    }
}