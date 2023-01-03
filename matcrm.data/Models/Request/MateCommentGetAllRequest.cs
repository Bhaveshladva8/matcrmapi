using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class MateCommentGetAllRequest
    {
        public long? TaskId { get; set; }
        public long? SubTaskId { get; set; }
        public long? ChildTaskId { get; set; }
        public long? MateTicketId { get; set; }
    }
}