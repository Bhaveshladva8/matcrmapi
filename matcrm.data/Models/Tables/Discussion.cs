using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    [Table("Discussion", Schema = "AppMail")]
    public class Discussion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string? Topic { get; set; }
        public string? Note { get; set; }
        public long? TeamInboxId { get; set; }
        [ForeignKey("TeamInboxId")]
        public virtual TeamInbox TeamInbox { get; set; }

        public bool IsArchived { get; set; }
     
        public int? AssignUserId { get; set; }
        [ForeignKey("AssignUserId")]
        public virtual User AssignUser { get; set; }

        public long? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
        public int? CreatedBy { get; set; }
        public int? TenantId { get; set; }

        [ForeignKey("TenantId")]
        public Tenant Tenant { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}