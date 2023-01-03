using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    [Table("MateChildTaskComment", Schema = "AppTask")]
    public class MateChildTaskComment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? ChildTaskId { get; set; }
        [ForeignKey("ChildTaskId")]
        public virtual EmployeeChildTask EmployeeChildTask { get; set; }
        public long? MateCommentId { get; set; }
        [ForeignKey("MateCommentId")]
        public virtual MateComment MateComment { get; set; }
    }
}