using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    [Table("OneClappFormHeader", Schema = "AppForm")]
    public class OneClappFormHeader
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public bool IsEnabled { get; set; }
        [Column(TypeName = "varchar(1000)")]
        public string? BackgroundColor { get; set; }
        [Column(TypeName = "varchar(1000)")]
        public string? HeaderImage { get; set; }
        [Column(TypeName = "varchar(1000)")]
        public string? CustomHeaderImage { get; set; }
        [Column(TypeName = "varchar(1000)")]
        public string? ImageLink { get; set; }
        public long? ImageSize { get; set; }

        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}