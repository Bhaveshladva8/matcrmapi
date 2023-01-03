using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Tables
{
    [Table("EmployeeClientTask", Schema = "AppTask")]
    public class EmployeeClientTask
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? EmployeeTaskId { get; set; }

        [ForeignKey("EmployeeTaskId")]
        public virtual EmployeeTask EmployeeTask { get; set; }
        public long? ClientId { get; set; }

        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}