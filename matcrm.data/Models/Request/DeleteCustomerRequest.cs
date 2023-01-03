using System;
using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Request
{
    public class DeleteCustomerRequest
    {
        public DeleteCustomerRequest()
        {
            Phones = new List<CustomerPhoneDto>();
            Emails = new List<CustomerEmailDto>();
            CustomFields = new List<CustomFieldDto>();
            Notes = new List<CustomerNoteDto>();
            Documents = new List<CustomerAttachmentDto>();
            PlannedActivities = new List<CustomerActivityDto>();
            CompletedActivities = new List<CustomerActivityDto>();
        }
        public long? Id { get; set; }
        public string FirstName { get; set; } 
        public string Name { get; set; }     
        public bool IsDeleted { get; set; }  
        public int? TenantId { get; set; }
        public List<CustomerActivityDto> CompletedActivities { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public List<CustomFieldDto> CustomFields { get; set; }
        public List<CustomerAttachmentDto> Documents { get; set; }
        public List<CustomerEmailDto> Emails { get; set; }
        public List<CustomerNoteDto> Notes { get; set; }
        public List<CustomerPhoneDto> Phones { get; set; }
        public List<CustomerActivityDto> PlannedActivities { get; set; }
        
    }
}