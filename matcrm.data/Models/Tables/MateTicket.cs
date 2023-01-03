using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Tables
{
    [Table("MateTicket", Schema = "AppTask")]
    public class MateTicket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long? StatusId { get; set; }

        [ForeignKey("StatusId")]
        public virtual Status Status { get; set; }
        public long? MateCategoryId { get; set; }

        [ForeignKey("MateCategoryId")]
        public virtual MateCategory MateCategory { get; set; }
        public long? MatePriorityId { get; set; }

        [ForeignKey("MatePriorityId")]
        public virtual MatePriority MatePriority { get; set; }
        public string TicketNo { get; set; }
        public int? CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public virtual User CreatedUser { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        [ForeignKey("UpdatedBy")]
        public virtual User UpdatedUser { get; set; }
        public DateTime? UpdatedOn { get; set; }       
        public DateTime? DeletedOn { get; set; }
        
    }
}