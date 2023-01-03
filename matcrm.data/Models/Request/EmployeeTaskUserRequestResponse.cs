using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class EmployeeTaskUserRequestResponse
    {
        public long? Id { get; set; }
        public int? UserId { get; set; }
        public long? EmployeeTaskId { get; set; }
        public string AssignUserFirstName { get; set; }
        public string AssignUserLastName { get; set; }
    }
}