using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class EmployeeSubTaskTimeRecordResponse
    {
        public long? Id { get; set; }
        public long? Duration { get; set; }
        public long? SubTaskId { get; set; }
    }
}