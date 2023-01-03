using System;
using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Response
{
    public class OneClappRequestFormRemoveResponse
    {
        public OneClappRequestFormRemoveResponse()
        {
            FormFieldValues = new List<OneClappFormFieldValueDto>();
            FormFields = new List<OneClappFormFieldDto>();
            CreatedCustomer = new CustomerDto();
            CreatedLead = new LeadDto();
            CreatedOrganization = new OrganizationDto();
        }
        public long? Id { get; set; }
        public long? OneClappFormId { get; set; }        
        public bool IsVerify { get; set; }
        public int? TenantId { get; set; }        
        public DateTime? CreatedOn { get; set; }        
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        public List<OneClappFormFieldValueDto> FormFieldValues { get; set; }
        public List<OneClappFormFieldDto> FormFields { get; set; }        
        public CustomerDto CreatedCustomer { get; set; }
        public OrganizationDto CreatedOrganization { get; set; }
        public LeadDto CreatedLead { get; set; }
        public bool IsCustomerCreate { get; set; }
        public bool IsOrganizationCreate { get; set; }
        public bool IsLeadCreate { get; set; }
    }
}