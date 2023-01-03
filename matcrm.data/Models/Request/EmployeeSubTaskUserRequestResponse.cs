using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class EmployeeSubTaskUserRequestResponse
    {
        public long Id { get; set; }
        public int? UserId { get; set; }
        public long? EmployeeSubTaskId { get; set; }
         public string AssignUserFirtName { get; set; }
        public string AssignUserLastName { get; set; }
    }
}