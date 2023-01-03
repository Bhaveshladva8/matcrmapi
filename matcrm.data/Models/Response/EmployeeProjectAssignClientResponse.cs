using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class EmployeeProjectAssignClientResponse
    {
        public long EmployeeProjectId { get; set; }
        public int ClientId { get; set; }
    }
}