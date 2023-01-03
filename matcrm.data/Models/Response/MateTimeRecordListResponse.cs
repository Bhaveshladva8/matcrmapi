using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class MateTimeRecordListResponse
    {
        public long? Id { get; set; }
        public string Type { get; set; }
        public long? ProjectTicketTaskId { get; set; }
        public string Name { get; set; }
        
        // public long? TaskId { get; set; }
        // public long? SubTaskId { get; set; }
        // public long? ChildTaskId { get; set; }
        // public long? TicketId { get; set; }
        public string? Comment { get; set; }
        public string? Duration { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? ServiceArticleId { get; set; }
        public bool? IsBillable { get; set; }
    }

    // public class MateProjectTimeRecordListResponse
    // {   
    //     public long ProjectId { get; set; }    
    //     public string Name { get; set; }
    //     public string TotalDuration { get; set; }
    //     public DateTime? Enddate { get; set; }
    //     public long TotalCount { get; set; }
    // }

    // public class MateTaskTimeRecordListResponse
    // {      
    //     public long TaskId { get; set; }    
    //     public string Name { get; set; }
    //     public string TotalDuration { get; set; }
    //     public DateTime? Enddate { get; set; }
    //     public long TotalCount { get; set; }
        
    // }
}