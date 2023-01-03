using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Dto {
    public class CustomTicketDto {
        public CustomTicketDto () {
            CustomFields = new List<CustomFieldDto> ();
        }
        public long? Id { get; set; }
        public long? TicketNumber { get; set; }
        public long? AssignedUserId { get; set; }
        public string AssignedUserUsername { get; set; }
        public bool Billable { get; set; }
        public long? ContactId { get; set; }
        public long? CustomerId { get; set; }
        public string CustomerNumber { get; set; }
        public bool DisableEmailTemplates { get; set; }

        [StringLength (254)]
        public string Email { get; set; }

        [Column (TypeName = "varchar(n)")]
        public string FirstName { get; set; }

        [Column (TypeName = "varchar(n)")]
        public string LastName { get; set; }
        public string Subject { get; set; }
        
        public string Description { get; set; }
        public long? TicketChannelId { get; set; }
        public string Channel { get; set; }
        public long? TicketPriorityId { get; set; }
        public string Priority { get; set; }
        public long? TicketStatusId { get; set; }
        public string Status { get; set; }
        public long? TicketTypeId { get; set; }
        public string Type { get; set; }
        public long? TicketCategoryId { get; set; }
        public string Category { get; set; }
        public int? TenantId { get; set; }
        public List<CustomFieldDto> CustomFields { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? FinishedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}