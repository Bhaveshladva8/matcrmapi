using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    [Table("OneClappLatestTheme", Schema = "AppTheme")]
    public class OneClappLatestTheme
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

       [Column (TypeName = "varchar(500)")]
        public string? Name { get; set; }

       [Column (TypeName = "varchar(500)")]
        public string? Accent { get; set; }

       [Column (TypeName = "varchar(500)")]
        public string? Primary { get; set; }

        [Column (TypeName = "varchar(500)")]
        public string? Warn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}