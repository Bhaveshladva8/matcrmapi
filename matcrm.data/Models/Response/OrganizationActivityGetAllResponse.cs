using System;
using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Response
{
    public class OrganizationActivityGetAllResponse
    {
        public OrganizationActivityGetAllResponse()
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
        public bool IsDone { get; set; }        
        public int? CreatedBy { get; set; }        
        public DateTime? CreatedOn { get; set; }
        public List<OrganizationActivityMemberDto> Members { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ShortName { get; set; }
        public string Email { get; set; }
    }
}