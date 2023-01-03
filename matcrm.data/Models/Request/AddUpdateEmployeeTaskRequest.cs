using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Request
{
    public class AddUpdateEmployeeTaskRequest
    {
        public AddUpdateEmployeeTaskRequest()
        {           
            AssignedUsers = new List<EmployeeTaskUserRequestResponse>();
            SubTasks = new List<AddUpdateTaskEmployeeSubTaskRequest>();
        }

        public long? Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public long? ProjectId { get; set; }
        //public long? SectionId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }        
        public long? StatusId { get; set; }
        //public bool IsActive { get; set; }
        public long? ClientId { get; set; }
        public long? MatePriorityId { get; set; }
        public List<EmployeeTaskUserRequestResponse> AssignedUsers { get; set; }
        public List<AddUpdateTaskEmployeeSubTaskRequest> SubTasks { get; set; }
    }

    public class AddUpdateTaskEmployeeSubTaskRequest
    {
        public AddUpdateTaskEmployeeSubTaskRequest()
        {           
            AssignedUsers = new List<AddUpdateTaskEmployeeSubTaskAssignUserRequest>();
        }
        public long? Id { get; set; }
        public string? Description { get; set; }
        public List<AddUpdateTaskEmployeeSubTaskAssignUserRequest> AssignedUsers { get; set; }
    }

    public class AddUpdateTaskEmployeeSubTaskAssignUserRequest
    {
        public long? Id { get; set; }
        public int? UserId { get; set; }
    }
}