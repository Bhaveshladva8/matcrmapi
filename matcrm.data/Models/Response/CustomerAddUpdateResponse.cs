using System;
using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Response
{
    public class CustomerAddUpdateResponse
    {
        public CustomerAddUpdateResponse()
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
        public long? OrganizationId { get; set; }        
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }       
        public List<CustomerPhoneDto> Phones { get; set; }
        public List<CustomerEmailDto> Emails { get; set; }
        public List<CustomFieldDto> CustomFields { get; set; }
    }
}