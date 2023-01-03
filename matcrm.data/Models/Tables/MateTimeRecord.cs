using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Tables
{
    [Table("MateTimeRecord", Schema = "AppTask")]
    public class MateTimeRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public long? Duration { get; set; }
        [Column(TypeName = "varchar")]
        public string? Comment { get; set; }
        public bool? IsBillable { get; set; }
        public bool IsManual { get; set; }
        public long? ServiceArticleId { get; set; }
        [ForeignKey("ServiceArticleId")]
        public virtual ServiceArticle ServiceArticle { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}