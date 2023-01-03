using System;
using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Request
{
    public class CustomerAddUpdateRequest
    {
        public CustomerAddUpdateRequest()
        {
            Phones = new List<CustomerPhoneDto>();
            Emails = new List<CustomerEmailDto>();                        
            CustomFields = new List<CustomFieldDto>();         
               
        }
        public long? Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long? SalutationId { get; set; }        
        public long? LabelId { get; set; }
        public long? OrganizationId { get; set; }
        public string OrganizationName { get; set; }        
        public List<CustomerPhoneDto> Phones { get; set; }
        public List<CustomerEmailDto> Emails { get; set; }        
        public List<CustomFieldDto> CustomFields { get; set; }
        
        
        
    }
}