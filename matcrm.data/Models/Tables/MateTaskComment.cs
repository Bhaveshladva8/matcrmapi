using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Tables
{
    [Table("MateTaskComment", Schema = "AppTask")]
    public class MateTaskComment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? TaskId { get; set; }
        [ForeignKey("TaskId")]
        public virtual EmployeeTask EmployeeTask { get; set; }
        public long? MateCommentId { get; set; }
        [ForeignKey("MateCommentId")]
        public virtual MateComment MateComment { get; set; }
    }
}