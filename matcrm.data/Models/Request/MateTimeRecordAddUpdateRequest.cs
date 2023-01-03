using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class MateTimeRecordAddUpdateRequest
    {
         public long? Id { get; set; }
         public long? Duration { get; set; }
         public string Comment { get; set; }
         public bool? IsBillable { get; set; }
         public bool IsManual { get; set; }
         public long? ServiceArticleId { get; set; }
         public long? ProjectId { get; set; }
         public long? TaskId { get; set; }
         public long? SubTaskId { get; set; }
         public long? ChildTaskId { get; set; }
         public long? TicketId { get; set; }
    }
}