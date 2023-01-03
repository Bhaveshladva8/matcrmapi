using System;
using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Response
{
    public class CustomerActivityAddUpdateResponse
    {        
        public CustomerActivityAddUpdateResponse () {
            Members = new List<CustomerActivityMemberDto> ();
        }
        public long? Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public string Address { get; set; }
        public long? CustomerId { get; set; }
        public long? CustomerActivityTypeId { get; set; }
        public long? CustomerActivityAvailabilityId { get; set; }
        public DateTime? ScheduleStartDate { get; set; }
        public DateTime? ScheduleEndDate { get; set; }
        public string EventStartTime { get; set; }
        public string EventEndTime { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public bool IsDone { get; set; }           
        public List<CustomerActivityMemberDto> Members { get; set; }
        
    }
}