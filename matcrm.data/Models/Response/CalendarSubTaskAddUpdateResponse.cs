using System;

namespace matcrm.data.Models.Response
{
    public class CalendarSubTaskAddUpdateResponse
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public long? CalendarTaskId { get; set; }
        public DateTime? StartDate { get; set; }
        public string StartTime { get; set; }        
        public bool IsDone { get; set; }       
        
    }
}