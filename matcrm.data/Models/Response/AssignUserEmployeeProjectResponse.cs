using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Models.Request;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Response
{
    public class AssignUserEmployeeProjectResponse
    {       
        public List<EmployeeProjectUserRequestResponse> AssignedUsers { get; set; }
    }
}