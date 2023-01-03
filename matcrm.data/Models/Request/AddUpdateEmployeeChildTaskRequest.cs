using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class AddUpdateEmployeeChildTaskRequest
    {
        public long? Id { get; set; }
        public long? EmployeeSubTaskId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? SectionId { get; set; }
        public double? Duration { get; set; }
        public long? StatusId { get; set; }
        public string Description { get; set; }
        public List<EmployeeChildTaskUserRequestResponse> AssignedUsers { get; set; }
    }
}