using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables {
    [Table("EmployeeTaskComment", Schema = "AppTask")]
    public class EmployeeTaskComment {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? EmployeeTaskId { get; set; }

        [ForeignKey ("EmployeeTaskId")]
        public virtual EmployeeTask EmployeeTask { get; set; }
         public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public string Comment { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}