using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Tables
{
    [Table("MateChildTaskTimeRecord", Schema = "AppTask")]
    public class MateChildTaskTimeRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? ChildTaskId { get; set; }
        [ForeignKey("ChildTaskId")]
        public virtual EmployeeChildTask EmployeeChildTask { get; set; }
        public long? MateTimeRecordId { get; set; }
        [ForeignKey("MateTimeRecordId")]
        public virtual MateTimeRecord MateTimeRecord { get; set; }
    }
}