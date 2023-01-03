using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Tables
{
    [Table("MateSubTaskComment", Schema = "AppTask")]
    public class MateSubTaskComment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? SubTaskId { get; set; }
        [ForeignKey("SubTaskId")]
        public virtual EmployeeSubTask EmployeeSubTask { get; set; }
        public long? MateCommentId { get; set; }
        [ForeignKey("MateCommentId")]
        public virtual MateComment MateComment { get; set; }
    }
}