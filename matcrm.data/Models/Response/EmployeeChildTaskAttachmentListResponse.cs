using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class EmployeeChildTaskAttachmentListResponse
    {
        public long? Id { get; set; }
        public long? EmployeeChildTaskId { get; set; }
        public int? UserId { get; set; }
        public string? Name { get; set; }
    }
}