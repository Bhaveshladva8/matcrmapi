using System;
using System.Collections.Generic;

namespace matcrm.data.Models.Dto
{
    // public class CustomerDto {
    //     public long? Id { get; set; }
    //     public string CustomerNumber { get; set; }
    //     public string Company { get; set; }
    //     public string Email { get; set; }
    //     public List<CustomFieldDto> CustomFields { get; set; }
    //     public long? CreatedBy { get; set; }
    //     public bool IsDeleted { get; set; }
    // }

    public class CustomerDto
    {
        public CustomerDto()
        {
            Phones = new List<CustomerPhoneDto>();
            Emails = new List<CustomerEmailDto>();
            CustomFields = new List<CustomFieldDto>();
            Notes = new List<CustomerNoteDto>();
            Documents = new List<CustomerAttachmentDto>();
            PlannedActivities = new List<CustomerActivityDto>();
            CompletedActivities = new List<CustomerActivityDto>();
        }
        public long? Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long? SalutationId { get; set; }
        public string Email { get; set; }
        public string Label { get; set; }
        public string Phone { get; set; }
        public long? LabelId { get; set; }
        public long? OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public long? WeClappCustomerId { get; set; }
        public List<CustomFieldDto> CustomFields { get; set; }
        public int? TenantId { get; set; }
        public int? UserId { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public List<CustomerPhoneDto> Phones { get; set; }
        public List<CustomerEmailDto> Emails { get; set; }
        public List<CustomerNoteDto> Notes { get; set; }
        public List<CustomerAttachmentDto> Documents { get; set; }
        public List<CustomerActivityDto> PlannedActivities { get; set; }
        public List<CustomerActivityDto> CompletedActivities { get; set; }
        public long? CustomFieldId { get; set; }

    }

    // public class SyncCustomerDto
    // {
    //     public SyncCustomerDto()
    //     {
    //         // CustomerList = new List<dynamic>();
    //         CustomerList = new List<KeyValuePair<string, string>>();
    //     }
    //     public int TenantId { get; set; }
    //     public List<KeyValuePair<string, string>> CustomerList { get; set; }
    //     // public List<dynamic> CustomerList { get; set; }
    // }
}