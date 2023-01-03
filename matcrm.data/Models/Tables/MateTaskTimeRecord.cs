using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Tables
{
    [Table("MateTaskTimeRecord", Schema = "AppTask")]
    public class MateTaskTimeRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? TaskId { get; set; }
        [ForeignKey("TaskId")]
        public virtual EmployeeTask EmployeeTask { get; set; }
        public long? MateTimeRecordId { get; set; }
        [ForeignKey("MateTimeRecordId")]
        public virtual MateTimeRecord MateTimeRecord { get; set; }
        
    }
}