using System;
using System.Collections.Generic;

namespace matcrm.data.Models.Dto
{
    public class LeadDto
    {

        public LeadDto()
        {
            Notes = new List<LeadNoteDto>();
            CustomFields = new List<CustomFieldDto>();
            Phones = new List<CustomerPhoneDto>();
            Emails = new List<CustomerEmailDto>();
            LabelIds = new List<int>();
            Labels = new List<LeadLabelDto>();
            PlannedActivities = new List<LeadActivityDto>();
            CompletedActivities = new List<LeadActivityDto>();
            CustomerDto = new CustomerDto();
            OrganizationDto = new OrganizationDto();
        }

        public long? Id { get; set; }
        public string Title { get; set; }
        public long? DealValue { get; set; }
        public long? CurrencyId { get; set; }
        public long? CustomerId { get; set; }
        public CustomerDto CustomerDto { get; set; }
        public string CustomerName { get; set; }
        public List<int> LabelIds { get; set; }
        public long? OrganizationId { get; set; }
        public OrganizationDto OrganizationDto { get; set; }
        public string OrganizationName { get; set; }
        public long? LeadLabelId { get; set; }
        public int? TenantId { get; set; }
        public long? CreatedBy { get; set; }
        public long? UserId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public List<LeadLabelDto> Labels { get; set; }
        public List<CustomerPhoneDto> Phones { get; set; }
        public List<CustomerEmailDto> Emails { get; set; }
        public List<CustomFieldDto> CustomFields { get; set; }
        public List<LeadNoteDto> Notes { get; set; }
        public List<LeadActivityDto> PlannedActivities { get; set; }
        public List<LeadActivityDto> CompletedActivities { get; set; }
    }
}