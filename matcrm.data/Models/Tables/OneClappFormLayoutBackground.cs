using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    [Table("OneClappFormLayoutBackground", Schema = "AppForm")]
    public class OneClappFormLayoutBackground
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column(TypeName = "varchar(1000)")]
        public string? BackgroundImage { get; set; }
        [Column(TypeName = "varchar(1000)")]
        public string? CustomBackgroundImage { get; set; }
        [Column(TypeName = "varchar(1000)")]
        public string? BackgroundColor { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}