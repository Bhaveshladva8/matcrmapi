using System;
using System.Collections.Generic;
using matcrm.data.Models.Tables;
using matcrm.data.Models.ViewModels.Calendar;

namespace matcrm.data.Models.Response
{
    public class GoogleCalendarGetAccountResponse
    {
        public GoogleCalendarGetAccountResponse(){
            Events = new List<GoogleMicrosoftCalendarEventVM>();
        }
        public long? Id { get; set; }
        public string? Access_Token { get; set; }
        public long? Expires_In { get; set; }
        public string Refresh_Token { get; set; }
        public string Scope { get; set; }        
        public string Token_Type { get; set; }        
        public string Email { get; set; }
        public long? IntProviderAppId { get; set; }
        public bool IsDefault { get; set; }
        public bool IsSelect { get; set; }
        public string? Color { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }        
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }        
        public string? IntProviderAppName { get; set; }
        public virtual IntProviderApp IntProviderApp { get; set; }
        public List<GoogleMicrosoftCalendarEventVM> Events { get; set; }
    }
}