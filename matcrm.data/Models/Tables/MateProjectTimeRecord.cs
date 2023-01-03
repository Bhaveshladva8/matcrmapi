using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Tables
{
    [Table("MateProjectTimeRecord", Schema = "AppTask")]
    public class MateProjectTimeRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public virtual EmployeeProject EmployeeProject { get; set; }
        public long? MateTimeRecordId { get; set; }
        [ForeignKey("MateTimeRecordId")]
        public virtual MateTimeRecord MateTimeRecord { get; set; }
    }
}