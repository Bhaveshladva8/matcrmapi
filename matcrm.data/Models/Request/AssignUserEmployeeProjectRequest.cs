using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class AssignUserEmployeeProjectRequest
    {
        public long EmployeeProjectId { get; set; }
        public List<int> AssignedUsers { get; set; }
    }
}