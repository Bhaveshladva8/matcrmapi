using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class MateTimeRecordDetailResponse
    {
        public long? Id { get; set; }
        public long? ProjectId { get; set; }
        public long? TaskId { get; set; }
        public long? SubTaskId { get; set; }
        public long? ChildTaskId { get; set; }
        public long? TicketId { get; set; }
        public string? Comment { get; set; }
        public string? Duration { get; set; }
        public DateTime? CreatedOn { get; set; }
        
    }
}