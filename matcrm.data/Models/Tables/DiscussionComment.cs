using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    [Table("DiscussionComment", Schema = "AppMail")]
    public class DiscussionComment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public int? TeamMemberId { get; set; }
        [ForeignKey("TeamMemberId")]
        public virtual User User { get; set; }
        public string? Comment { get; set; }

        public long? DiscussionId { get; set; }

        [ForeignKey("DiscussionId")]
        public Discussion Discussion { get; set; }
        public long? ReplyCommentId { get; set; }

        [ForeignKey("ReplyCommentId")]
        public virtual DiscussionComment ReplyDiscussionComment { get; set; }

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