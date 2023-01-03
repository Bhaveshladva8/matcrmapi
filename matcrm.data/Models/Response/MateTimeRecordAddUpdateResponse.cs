using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class MateTimeRecordAddUpdateResponse
    {
        //public long? Id { get; set; }
        //public long? ProjectId { get; set; }
        // public long? TicketId { get; set; }
        // public long? TaskId { get; set; }
        // public long? SubTaskId { get; set; }
        // public long? ChildTaskId { get; set; }        
        // public string? Comment { get; set; }
        // public string? Duration { get; set; }
        // public long? UserId { get; set; }
        // public DateTime? CreatedOn { get; set; }
        // public long? ServiceArticleId { get; set; }
        // public bool? IsBillable { get; set; }
        public long? Id { get; set; }
        public string Type { get; set; }
        public long? ProjectTicketTaskId { get; set; }
        public string Name { get; set; }        
        public string? Comment { get; set; }
        public string? Duration { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? ServiceArticleId { get; set; }
        public bool? IsBillable { get; set; }

    }
}