using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
     [Table("MailBoxComment", Schema = "AppMail")]
    public class MailComment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public int? TeamMemberId { get; set; }
        [ForeignKey("TeamMemberId")]
        public virtual User User { get; set; }
        public string? ThreadId { get; set; }
        public string? Comment { get; set; }

        public long? MailReplyCommentId { get; set; }

        [ForeignKey("MailReplyCommentId")]
        public virtual MailComment ReplyMailComment { get; set; }
        public int? TenantId { get; set; }

        [ForeignKey("TenantId")]
        public Tenant Tenant { get; set; }

        public bool IsPinned { get; set; }
        public int? PinnedBy { get; set; }
        [ForeignKey("PinnedBy")]
        public virtual User PinnedUser { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}