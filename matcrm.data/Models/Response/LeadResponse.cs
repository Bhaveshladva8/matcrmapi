using System;
using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Response
{
    public class LeadResponse
    {
        public LeadResponse()
        {            
            //Notes = new List<LeadNoteDto>();
            CustomFields = new List<CustomFieldDto>();
            // Phones = new List<CustomerPhoneDto>();
            // Emails = new List<CustomerEmailDto>();            
            Labels = new List<LeadLabelDto>();
            LabelIds = new List<int>();
            // PlannedActivities = new List<LeadActivityDto>();
            // CompletedActivities = new List<LeadActivityDto>();
            
        }
        public long? Id { get; set; }
        public string Title { get; set; }
        public long? CustomerId { get; set; }        
        public long? OrganizationId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public List<LeadLabelDto> Labels { get; set; }        
        public List<CustomFieldDto> CustomFields { get; set; }
        public List<int> LabelIds { get; set; }
        // public List<CustomerPhoneDto> Phones { get; set; }
        // public List<CustomerEmailDto> Emails { get; set; }
        //public List<LeadNoteDto> Notes { get; set; }
        // public List<LeadActivityDto> PlannedActivities { get; set; }
        // public List<LeadActivityDto> CompletedActivities { get; set; }
    }
}