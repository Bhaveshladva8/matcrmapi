using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    [Table("OneClappFormLayout", Schema = "AppForm")]
    public class OneClappFormLayout
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public bool TextDirectionRTL { get; set; }
        public long? LayoutBackgroundId { get; set; }
        [ForeignKey("LayoutBackgroundId")]
        public virtual OneClappFormLayoutBackground OneClappFormLayoutBackground { get; set; }
         public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}