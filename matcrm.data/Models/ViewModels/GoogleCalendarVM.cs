using System.Collections.Generic;

namespace matcrm.data.Models.ViewModels {
    public class GoogleCalendarVM {
        public GoogleCalendarVM(){
            Events = new List<GoogleCalendarEventVM>();
            Items = new List<GoogleCalendarEventVM>();
        }
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
}