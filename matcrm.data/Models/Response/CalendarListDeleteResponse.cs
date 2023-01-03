using System;

namespace matcrm.data.Models.Response
{
    public class CalendarListDeleteResponse
    {
        
        public long? Id { get; set; }
        public string Name { get; set; }
        public int? CreatedBy { get; set; }
        
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        
    }
}