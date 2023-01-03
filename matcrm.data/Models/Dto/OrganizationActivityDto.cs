using System;
using System.Collections.Generic;

namespace matcrm.data.Models.Dto
{
    public class OrganizationActivityDto
    {
        public OrganizationActivityDto()
        {
            Members = new List<OrganizationActivityMemberDto>();
        }
        public long? Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public string Address { get; set; }
        public long? OrganizationId { get; set; }
        public long? OrganizationActivityTypeId { get; set; }
        public string OrganizationActivityType { get; set; }
        public long? OrganizationActivityAvailabilityId { get; set; }
        public string OrganizationActivityAvailability { get; set; }
        public DateTime? ScheduleStartDate { get; set; }
        public DateTime? ScheduleEndDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string EventStartTime { get; set; }
        public string EventEndTime { get; set; }
        public bool IsDone { get; set; }
        public int? TenantId { get; set; }
        public int? CreatedBy { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public List<OrganizationActivityMemberDto> Members { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ShortName { get; set; }
        public string Email { get; set; }
    }
}