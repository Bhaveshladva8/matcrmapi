using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    [Table("EmployeeTaskActivity", Schema = "AppTask")]
    public class EmployeeTaskActivity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? ProjectId { get; set; }
        public long? EmployeeTaskId { get; set; }

        [ForeignKey("EmployeeTaskId")]
        public virtual EmployeeTask EmployeeTask { get; set; }
         public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Column(TypeName = "varchar(1000)")]
        public string Activity { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}