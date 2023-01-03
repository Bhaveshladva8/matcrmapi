using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Request
{
    public class AddUpdateEmployeeSubTaskRequest
    {
        public AddUpdateEmployeeSubTaskRequest()
        {           
            AssignedUsers = new List<EmployeeSubTaskUserRequestResponse>();
            ChildTasks = new List<AddUpdateSubTaskEmployeeChildTaskRequest>();
        }
        public long? Id { get; set; }
        public long? EmployeeTaskId { get; set; }
        public long? StatusId { get; set; }
        public bool IsActive { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<EmployeeSubTaskUserRequestResponse> AssignedUsers { get; set; }
        public List<AddUpdateSubTaskEmployeeChildTaskRequest> ChildTasks { get; set; }
    }

    public class AddUpdateSubTaskEmployeeChildTaskRequest
    {
        public AddUpdateSubTaskEmployeeChildTaskRequest()
        {           
            AssignedUsers = new List<AddUpdateSubTaskChildAssignUserRequest>();
        }
        public long Id { get; set; }
        public string? Description { get; set; }
        public List<AddUpdateSubTaskChildAssignUserRequest> AssignedUsers { get; set; }
    }

    public class AddUpdateSubTaskChildAssignUserRequest
    {
        public long Id { get; set; }
        public int? UserId { get; set; }
    }
}