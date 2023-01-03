using System;
using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Response
{
    public class LeadAddUpdateResponse
    {
        public LeadAddUpdateResponse()
        {            
            Labels = new List<LeadLabelDto>();            
            CustomerDto = new CustomerDto();
            OrganizationDto = new OrganizationDto();
        }
        public long? Id { get; set; }        
        public List<LeadLabelDto> Labels { get; set; }
        public long? CustomerId { get; set; }
        public CustomerDto CustomerDto { get; set; }
        public long? OrganizationId { get; set; }        
        public string Title { get; set; }
        public OrganizationDto OrganizationDto { get; set; }
        public DateTime? CreatedOn { get; set; }
        
    }
}