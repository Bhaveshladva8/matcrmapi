using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Tables
{

    [Table("EmployeeChildTaskTimeRecord", Schema = "AppTask")]
    public class EmployeeChildTaskTimeRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public long? Duration { get; set; }
        public long? EmployeeChildTaskId { get; set; }
        [ForeignKey("EmployeeChildTaskId")]
        public virtual EmployeeChildTask EmployeeChildTask { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }

    }
}