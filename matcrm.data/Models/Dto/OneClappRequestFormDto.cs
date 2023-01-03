using System;
using System.Collections.Generic;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Dto
{
    public class OneClappRequestFormDto
    {
        public OneClappRequestFormDto()
        {
            FormFieldValues = new List<OneClappFormFieldValueDto>();
            FormFields = new List<OneClappFormFieldDto>();
            CreatedCustomer = new CustomerDto();
            CreatedLead = new LeadDto();
            CreatedOrganization = new OrganizationDto();
        }
        public long? Id { get; set; }
        public long? OneClappFormId { get; set; }
        public long? OneClappFormStatusId { get; set; }
        public string SubmitFormCreatedName { get; set; }
        public bool IsVerify { get; set; }
        public int? TenantId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        public List<OneClappFormFieldValueDto> FormFieldValues { get; set; }
        public List<OneClappFormFieldDto> FormFields { get; set; }
        public CustomControl CustomControl { get; set; }
        public CustomerDto CreatedCustomer { get; set; }
        public OrganizationDto CreatedOrganization { get; set; }
        public LeadDto CreatedLead { get; set; }
        public bool IsCustomerCreate { get; set; }
        public bool IsOrganizationCreate { get; set; }
        public bool IsLeadCreate { get; set; }
    }
}