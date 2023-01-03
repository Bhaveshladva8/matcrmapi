using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Tables
{
    [Table("MateComment", Schema = "AppTask")]
    public class MateComment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public string? Comment { get; set; }
        public long? MateReplyCommentId { get; set; }

        [ForeignKey("MateReplyCommentId")]
        public virtual MateComment ReplyMateComment { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}