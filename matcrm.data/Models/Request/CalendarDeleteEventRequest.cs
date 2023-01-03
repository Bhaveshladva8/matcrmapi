using System;
using System.Collections.Generic;

namespace matcrm.data.Models.Request
{
    public class CalendarDeleteEventRequest
    {
        
        public string id { get; set; } 
        public DateTime? startDate { get; set; }        
        public string? startTime { get; set; }
        public string? endTime { get; set; }
        public long? IntProviderAppSecretId { get; set; }
        public string summary { get; set; }        
        public string description { get; set; }                
        
    }
}