using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Dto
{
    public class EmployeeSubTaskDto
    {
        public long? Id { get; set; }
        public long? WeClappTimeRecordId { get; set; }
        public long? EmployeeTaskId { get; set; }
        public virtual EmployeeTask EmployeeTask { get; set; }
        public long? StatusId { get; set; }
        public virtual EmployeeTaskStatus EmployeeTaskStatus { get; set; }
        public bool IsActive { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public List<EmployeeSubTaskUser> AssignedUsers { get; set; }
    }
}