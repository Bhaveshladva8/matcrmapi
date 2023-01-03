using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Tables
{
    [Table("MateProjectTicket", Schema = "AppTask")]
    public class MateProjectTicket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? MateTicketId { get; set; }

        [ForeignKey("MateTicketId")]
        public virtual MateTicket MateTicket { get; set; }
        public long? EmployeeProjectId { get; set; }

        [ForeignKey("EmployeeProjectId")]
        public virtual EmployeeProject EmployeeProject { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}