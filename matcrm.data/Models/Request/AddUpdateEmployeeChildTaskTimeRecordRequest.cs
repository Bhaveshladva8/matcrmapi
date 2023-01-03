using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class AddUpdateEmployeeChildTaskTimeRecordRequest
    {
        public long? Duration { get; set; }
        public long? EmployeeChildTaskId { get; set; }
    }
}