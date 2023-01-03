using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class EmployeeChildTaskHistoryResponse
    {
        public string? Email { get; set; }
        public string? ShortName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? Activity { get; set; }
        public long? EmployeeChildTaskId { get; set; }
    }
}