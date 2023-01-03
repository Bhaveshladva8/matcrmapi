using System;

namespace matcrm.data.Models.Response
{
    public class CalendarSubTaskDeleteResponse
    {
        public long Id { get; set; }
        
        public long? CalendarTaskId { get; set; }
       
        public bool IsDone { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}