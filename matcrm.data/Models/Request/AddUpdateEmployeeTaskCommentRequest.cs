using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class AddUpdateEmployeeTaskCommentRequest
    {
        public long? Id { get; set; }
        public long? EmployeeTaskId { get; set; }
        public string? Comment { get; set; }
    }
}