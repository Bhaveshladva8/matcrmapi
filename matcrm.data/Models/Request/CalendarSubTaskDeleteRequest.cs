using System;

namespace matcrm.data.Models.Request
{
    public class CalendarSubTaskDeleteRequest
    {
        public long? CalendarTaskId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long Id { get; set; }    
        public bool IsDone { get; set; }
        public bool IsDeleted { get; set; }
    }
}