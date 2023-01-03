using System;
using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Response
{
    public class CustomerSyncToWeClappResponse
    {
        public CustomerSyncToWeClappResponse()
        {
            Phones = new List<CustomerPhoneDto>();
            Emails = new List<CustomerEmailDto>();
            CustomFields = new List<CustomFieldDto>();
            Notes = new List<CustomerNoteDto>();
            Documents = new List<CustomerAttachmentDto>();
            PlannedActivities = new List<CustomerActivityDto>();
            CompletedActivities = new List<CustomerActivityDto>();
        }
        public bool IsDeleted { get; set; }
        public List<CustomerPhoneDto> Phones { get; set; }
        public List<CustomerEmailDto> Emails { get; set; }
        public List<CustomerNoteDto> Notes { get; set; }
        public List<CustomerAttachmentDto> Documents { get; set; }
        public List<CustomerActivityDto> PlannedActivities { get; set; }
        public List<CustomerActivityDto> CompletedActivities { get; set; }
        public List<CustomFieldDto> CustomFields { get; set; }
        

    }
}
    
