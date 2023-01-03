using System;
using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Request
{
    public class OrganizationActivityAddUpdateRequest
    {
        public OrganizationActivityAddUpdateRequest()
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
        public long? OrganizationActivityAvailabilityId { get; set; }       
        public DateTime? ScheduleStartDate { get; set; }
        public DateTime? ScheduleEndDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string EventStartTime { get; set; }
        public string EventEndTime { get; set; }
        public bool IsDone { get; set; }        
        public List<OrganizationActivityMemberDto> Members { get; set; }
       
    }
}