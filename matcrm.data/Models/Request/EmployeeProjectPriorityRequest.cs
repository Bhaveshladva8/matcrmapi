using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class EmployeeProjectPriorityRequest
    {
        public long? Id { get; set; }
         public long? CurrentPriority { get; set; }
        public long? PreviousPriority { get; set; }
    }
}