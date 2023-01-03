using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Tables
{
    [Table("MateTicketComment", Schema = "AppTask")]
    public class MateTicketComment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? MateTicketId { get; set; }
        [ForeignKey("MateTicketId")]
        public virtual MateTicket MateTicket { get; set; }
        public long? MateCommentId { get; set; }
        [ForeignKey("MateCommentId")]
        public virtual MateComment MateComment { get; set; }
    }
}