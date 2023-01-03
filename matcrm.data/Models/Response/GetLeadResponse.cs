using System;
using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Response
{
    public class GetLeadResponse
    {
        public GetLeadResponse()
        {
            Notes = new List<LeadNoteDto>();
            CustomFields = new List<CustomFieldDto>();            
            Labels = new List<LeadLabelDto>();            
            CustomerDto = new CustomerDto();
            OrganizationDto = new OrganizationDto();
            LabelIds = new List<int>();
            PlannedActivities = new List<LeadActivityDto>();
            CompletedActivities = new List<LeadActivityDto>();
        }
        public long? Id { get; set; }        
        public DateTime? CreatedOn { get; set; }
        public List<LeadLabelDto> Labels { get; set; }
        public List<LeadNoteDto> Notes { get; set; }
        public List<CustomFieldDto> CustomFields { get; set; }
        public CustomerDto CustomerDto { get; set; }
        public long? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public OrganizationDto OrganizationDto { get; set; }
        public long? OrganizationId { get; set; }
        public string OrganizationName { get; set; }        
        public string Title { get; set; }
        public List<int> LabelIds { get; set; }
        public List<LeadActivityDto> PlannedActivities { get; set; }
        public List<LeadActivityDto> CompletedActivities { get; set; }
    }
}