using System;
using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Request
{
    public class LeadActivityAddUpdateRequest
    {
        public LeadActivityAddUpdateRequest()
        {
            Members = new List<LeadActivityMemberDto>();
        }
        public long? Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public string Address { get; set; }
        public long? LeadId { get; set; }
        public long? LeadActivityTypeId { get; set; }        
        public long? LeadActivityAvailabilityId { get; set; }        
        public DateTime? ScheduleStartDate { get; set; }
        public DateTime? ScheduleEndDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string EventStartTime { get; set; }
        public string EventEndTime { get; set; }
        public bool IsDone { get; set; }
        public List<LeadActivityMemberDto> Members { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ShortName { get; set; }
        public string Email { get; set; }
    }
}