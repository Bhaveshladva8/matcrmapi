using System;
using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Request
{
    public class LeadAddUpdateRequest
    {
        public LeadAddUpdateRequest()
        {            
            CustomFields = new List<CustomFieldDto>();
            Phones = new List<CustomerPhoneDto>();
            Emails = new List<CustomerEmailDto>();
            LabelIds = new List<int>();            
        }
        public long? Id { get; set; }
        public long? CustomerId { get; set; }       
        public string CustomerName { get; set; }              
        public List<int> LabelIds { get; set; } 
        public long? OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string Title { get; set; }        
        public List<CustomFieldDto> CustomFields { get; set; }
        public List<CustomerPhoneDto> Phones { get; set; }
        public List<CustomerEmailDto> Emails { get; set; }
         
    }
}