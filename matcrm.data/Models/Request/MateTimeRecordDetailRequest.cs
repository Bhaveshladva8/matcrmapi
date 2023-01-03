using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class MateTimeRecordDetailRequest
    {
        public long? ProjectId { get; set; }
        public long? TaskId { get; set; }
        public long? SubTaskId { get; set; }
        public long? ChildTaskId { get; set; }
    }
}