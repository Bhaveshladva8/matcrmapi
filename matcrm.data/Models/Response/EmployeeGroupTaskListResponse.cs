using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class EmployeeGroupTaskListResponse
    {
        // public EmployeeGroupTaskListResponse(){
        //     Tasks = new List<EmployeeGroupTask>();
        // }
        // public long? Id { get; set; }
        // public string Name { get; set; }
        // public long? TotalCount { get; set; }
        // public List<EmployeeGroupTask> Tasks { get; set; }
        public List<EmployeeGroupTask> StatusList { get; set; }

    }

    public class EmployeeGroupTask
    {
        public long? StatusId { get; set; }
        public string StatusName { get; set; }
        public string StatusColor { get; set; }
        public long TotalCount { get; set; }
        // public long? Id { get; set; }
        // public string Name { get; set; }
        // public string Description { get; set; }
        // public long? StatusId { get; set; }
        // public string TotalDuration { get; set; }
        // public DateTime? Enddate { get; set; }
        // public long TimeRecordCount { get; set; }
        // public long PageNo { get; set; }
    }
}