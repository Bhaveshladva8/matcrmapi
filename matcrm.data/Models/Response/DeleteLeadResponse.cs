using System;
using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Response
{
    public class DeleteLeadResponse
    {
        public DeleteLeadResponse()
        {
           
            CustomFields = new List<CustomFieldDto>();            
            Labels = new List<LeadLabelDto>();            
            CustomerDto = new CustomerDto();
            OrganizationDto = new OrganizationDto();
        }
        public long? Id { get; set; }
        public DateTime? CreatedOn { get; set; }
        public List<CustomFieldDto> CustomFields { get; set; }
        public CustomerDto CustomerDto { get; set; }
        public long? CustomerId { get; set; }
        public bool IsDeleted { get; set; }
        public List<LeadLabelDto> Labels { get; set; }
        public OrganizationDto OrganizationDto { get; set; }
        public int? TenantId { get; set; }
        public string Title { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}