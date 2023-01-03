using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Tables
{
    [Table("MateSubTaskTimeRecord", Schema = "AppTask")]
    public class MateSubTaskTimeRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? SubTaskId { get; set; }
        [ForeignKey("SubTaskId")]
        public virtual EmployeeSubTask EmployeeSubTask { get; set; }
        public long? MateTimeRecordId { get; set; }
        [ForeignKey("MateTimeRecordId")]
        public virtual MateTimeRecord MateTimeRecord { get; set; }
    }
}