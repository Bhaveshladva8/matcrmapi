using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class EmployeeTaskAssignClientRequest
    {
        public long EmployeeTaskId { get; set; }
        public int ClientId { get; set; }
    }
}