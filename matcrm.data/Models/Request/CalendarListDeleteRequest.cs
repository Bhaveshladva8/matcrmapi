using System;

namespace matcrm.data.Models.Request
{
    public class CalendarListDeleteRequest
    {
        
        public long? Id { get; set; }
        public string Name { get; set; }
        public int? CreatedBy { get; set; }        
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        
        
    }
}