using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Tables
{
    [Table("MateTicketActivity", Schema = "AppTask")]
    public class MateTicketActivity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Activity { get; set; }
        public long? MateTicketId { get; set; }

        [ForeignKey("MateTicketId")]
        public virtual MateTicket MateTicket { get; set; }
        public long? EmployeeProjectId { get; set; }

        [ForeignKey("EmployeeProjectId")]
        public virtual EmployeeProject EmployeeProject { get; set; }
        public long? EmployeeTaskId { get; set; }

        [ForeignKey("EmployeeTaskId")]
        public virtual EmployeeTask EmployeeTask { get; set; }        
        public int? CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public virtual User CreatedUser { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}