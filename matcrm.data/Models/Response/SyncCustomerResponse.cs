using System;
using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Response
{
    public class SyncCustomerResponse
    {
        public SyncCustomerResponse()
        {            
            CustomFields = new List<CustomFieldDto>();           
        }
        public long? Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Label { get; set; }        
        public long? WeClappCustomerId { get; set; }
        public int? TenantId { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public List<CustomFieldDto> CustomFields { get; set; }
    }
}