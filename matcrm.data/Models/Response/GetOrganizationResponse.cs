using matcrm.data.Models.Dto;
using System;
using System.Collections.Generic;


namespace matcrm.data.Models.Response
{
    public class GetOrganizationResponse
    {
        public GetOrganizationResponse(){           
            
            Notes = new List<OrganizationNoteDto>();
            Documents = new List<OrganizationAttachmentDto>();
            CustomFields = new List<CustomFieldDto>();
            PlannedActivities = new List<OrganizationActivityDto>();
            CompletedActivities = new List<OrganizationActivityDto>();
           
        }
        public long? Id { get; set; }
        public string Name { get; set; }
        public long? LabelId { get; set; }
        public string Address { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public List<CustomFieldDto> CustomFields { get; set; }
        public List<OrganizationNoteDto> Notes { get; set; }
        public List<OrganizationAttachmentDto> Documents { get; set; }
        public List<OrganizationActivityDto> PlannedActivities { get; set; }
        public List<OrganizationActivityDto> CompletedActivities { get; set; }
    }
}
