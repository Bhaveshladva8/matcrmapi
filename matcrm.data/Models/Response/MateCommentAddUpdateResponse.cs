using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class MateCommentAddUpdateResponse
    {        
        public long? Id { get; set; }
        public string? Comment { get; set; }
        public long? MateReplyCommentId { get; set; }        
        public long? TaskId { get; set; }
        public long? SubTaskId { get; set; }
        public long? ChildTaskId { get; set; }
    }
}