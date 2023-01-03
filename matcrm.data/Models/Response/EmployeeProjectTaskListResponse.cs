using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Models.Request;

namespace matcrm.data.Models.Response
{
    public class EmployeeProjectTaskListResponse
    {
        public long Id { get; set; }
        public string TaskNo { get; set; }
        public string Name { get; set; }
        //public long? StatusId { get; set; }
        public string Status { get; set; }
        //public long ClientId { get; set; }
        public string ClientName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public List<EmployeeProjectTaskUserListResponse> AssignedUsers { get; set; }
    }
    public class EmployeeProjectTaskUserListResponse
    {
        public long? UserId { get; set; }
        public string ProfileURL { get; set; }
    }
}