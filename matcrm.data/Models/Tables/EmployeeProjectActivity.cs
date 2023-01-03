using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    [Table("EmployeeProjectActivity", Schema = "AppTask")]
    public class EmployeeProjectActivity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public virtual EmployeeProject Project { get; set; }
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Column(TypeName = "varchar(1000)")]
        public string Activity { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}