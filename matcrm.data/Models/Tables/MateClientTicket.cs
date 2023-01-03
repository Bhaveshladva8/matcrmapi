using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Tables
{
    [Table("MateClientTicket", Schema = "AppTask")]
    public class MateClientTicket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? MateTicketId { get; set; }

        [ForeignKey("MateTicketId")]
        public virtual MateTicket MateTicket { get; set; }
        public long? ClientId { get; set; }

        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}