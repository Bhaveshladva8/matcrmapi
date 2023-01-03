using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class AddUpdateEmployeeTaskTimeRecordRequest
    {
        public long? Duration { get; set; }
        public long? EmployeeTaskId { get; set; }
    }
}