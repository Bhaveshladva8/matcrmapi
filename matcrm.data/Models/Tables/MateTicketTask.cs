using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Tables
{
    [Table("MateTicketTask", Schema = "AppTask")]
    public class MateTicketTask
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? MateTicketId { get; set; }

        [ForeignKey("MateTicketId")]
        public virtual MateTicket MateTicket { get; set; }
        public long? EmployeeTaskId { get; set; }

        [ForeignKey("EmployeeTaskId")]
        public virtual EmployeeTask EmployeeTask { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}