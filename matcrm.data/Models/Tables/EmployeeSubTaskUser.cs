using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Tables
{

    [Table("EmployeeSubTaskUser", Schema = "AppTask")]
    public class EmployeeSubTaskUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public int? UserId { get; set; }
        public long? EmployeeSubTaskId { get; set; }
        [ForeignKey("EmployeeSubTaskId")]
        public virtual EmployeeSubTask EmployeeSubTask { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}