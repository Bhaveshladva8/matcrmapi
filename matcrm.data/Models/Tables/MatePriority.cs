using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    [Table("MatePriority", Schema = "AppTask")]
    public class MatePriority
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column(TypeName = "varchar(500)")]
        public string Name { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string Color { get; set; }        
        public long? CustomTableId { get; set; }

        [ForeignKey("CustomTableId")]
        public virtual CustomTable CustomTable { get; set; }        
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