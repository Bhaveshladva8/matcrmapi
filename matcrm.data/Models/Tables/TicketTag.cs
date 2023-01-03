using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables {
    public class TicketTag {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? UserId { get; set; }
        public long? TicketId { get; set; }

        [ForeignKey ("TicketId")]
        public virtual Ticket Ticket { get; set; }
        public long? TagId { get; set; }

        [ForeignKey ("TagId")]
        public virtual Tag Tag { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}