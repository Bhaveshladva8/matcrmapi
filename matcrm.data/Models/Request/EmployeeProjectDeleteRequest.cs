using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class EmployeeProjectDeleteRequest
    {
        public long Id { get; set; }
        public bool IsKeepTasks { get; set; }
    }
}