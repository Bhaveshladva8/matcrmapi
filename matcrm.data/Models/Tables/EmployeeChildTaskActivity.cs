using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Tables
{

    [Table("EmployeeChildTaskActivity", Schema = "AppTask")]
    public class EmployeeChildTaskActivity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? EmployeeChildTaskId { get; set; }

        [ForeignKey("EmployeeChildTaskId")]
        public virtual EmployeeChildTask EmployeeChildTask { get; set; }
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Column(TypeName = "varchar(1000)")]
        public string? Activity { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}