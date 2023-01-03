using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables {
    public class Ticket {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? TicketNumber { get; set; }
        public string? Description { get; set; }
        public long? AssignedUserId { get; set; }
        public string? AssignedUserUsername { get; set; }
        public bool Billable { get; set; }
        public long? ContactId { get; set; }

        public long? CustomerId { get; set; }

        [ForeignKey ("CustomerId")]
        public virtual Customer Customer { get; set; }

        public long? TicketTypeId { get; set; }

        [ForeignKey ("TicketTypeId")]
        public virtual TicketType TicketType { get; set; }

        public long? TicketCategoryId { get; set; }

        [ForeignKey ("TicketCategoryId")]
        public virtual TicketCategory TicketCategory { get; set; }
        public string? CustomerNumber { get; set; }
        public bool DisableEmailTemplates { get; set; }

        [Column (TypeName = "varchar(254)")]
        public string? Email { get; set; }

        [Column (TypeName = "varchar(254)")]
        public string? FirstName { get; set; }

        [Column (TypeName = "varchar(254)")]
        public string? LastName { get; set; }
        public string? Subject { get; set; }
        public long? TicketChannelId { get; set; }

        [ForeignKey ("TicketChannelId")]
        public virtual TicketChannel TicketChannel { get; set; }
        public long? TicketPriorityId { get; set; }

        [ForeignKey ("TicketPriorityId")]
        public virtual TicketPriority TicketPriority { get; set; }
        public long? TicketStatusId { get; set; }

        [ForeignKey ("TicketStatusId")]
        public virtual TicketStatus TicketStatus { get; set; }
        public int? TenantId { get; set; }

        [ForeignKey ("TenantId")]
        public virtual Tenant Tenant { get; set; }
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