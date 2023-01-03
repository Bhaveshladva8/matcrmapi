using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Tables
{
    [Table("MateCommentAttachment", Schema = "AppTask")]
    public class MateCommentAttachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? MateCommentId { get; set; }

        [ForeignKey("MateCommentId")]
        public virtual MateComment MateComment { get; set; }
        public string? Name { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}