using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class EmployeeChildTaskListResponse
    {
        public long Id { get; set; }        
        public long? SubTaskId { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public long? StatusId { get; set; }        
        public string Status { get; set; }
        //public DateTime? CreatedOn { get; set; }
        //public List<EmployeeSubTaskUserRequestResponse> AssignedUsers { get; set; }
        public string TotalDuration { get; set; }
        public DateTime? Enddate { get; set; }
        public long TimeRecordCount { get; set; }
    }
}