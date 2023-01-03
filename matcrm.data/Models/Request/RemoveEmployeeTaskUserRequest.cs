using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class RemoveEmployeeTaskUserRequest
    {
        public long? Id { get; set; }
        public int? UserId { get; set; }
        public long? EmployeeTaskId { get; set; }
    }
}