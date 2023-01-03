using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Dto
{
    public class OneClappTaskDto
    {
        public long? Id { get; set; }
        public long? WeClappTimeRecordId { get; set; }
        public long? OneClappTaskId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public string Description { get; set; }
        public double? StartAt { get; set; }
        public string Ticket { get; set; }
        public double? Duration { get; set; }
        public string ApiKey { get; set; }
        public string Tenant { get; set; }
        public long? TenantId { get; set; }
        public int? UserId { get; set; }
        public int? StatusId { get; set; }
        public long? SectionId { get; set; }
        public long? Priority { get; set; }
        public long? CurrentPriority { get; set; }
        public long? PreviousPriority { get; set; }
        public long? CurrentSectionId { get; set; }
        public long? PreviousSectionId { get; set; }
        public bool IsSectionChange { get; set; }
        public List<OneClappTaskUser> AssignedUsers { get; set; }
    }
}