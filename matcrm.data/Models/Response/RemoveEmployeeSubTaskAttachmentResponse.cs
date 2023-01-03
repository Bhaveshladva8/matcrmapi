using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class RemoveEmployeeSubTaskAttachmentResponse
    {
        public long? Id { get; set; }
        public long? EmployeeSubTaskId { get; set; }
        public string? Name { get; set; }
    }
}