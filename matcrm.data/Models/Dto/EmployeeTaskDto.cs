using System;
using System.Collections.Generic;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Dto
{
    public class EmployeeTaskDto
    {
        public long? Id { get; set; }
        public long? StatusId { get; set; }
        public int? TenantId { get; set; }
        public int? UserId { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? Priority { get; set; }
        public long? ProjectId { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        public long? CurrentPriority { get; set; }
        public long? PreviousPriority { get; set; }
        public long? CurrentSectionId { get; set; }
        public long? PreviousSectionId { get; set; }
        public bool IsProjectChange { get; set; }
         public long? SectionId { get; set; }
        public List<EmployeeTaskUser> AssignedUsers { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public long? ClientId { get; set; }
        public long? MatePriorityId { get; set; }
        public string TaskNo { get; set; }

    }
}