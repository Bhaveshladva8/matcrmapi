using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    [Table("OneClappLatestThemeConfig", Schema = "AppTheme")]
    public class OneClappLatestThemeConfig
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? OneClappLatestThemeId { get; set; }

        [ForeignKey("OneClappLatestThemeId")]
        public virtual OneClappLatestTheme OneClappLatestTheme { get; set; }
        public long? OneClappLatestThemeLayoutId { get; set; }

        [ForeignKey("OneClappLatestThemeLayoutId")]
        public virtual OneClappLatestThemeLayout OneClappLatestThemeLayout { get; set; }
        public long? OneClappLatestThemeSchemeId { get; set; }

        [ForeignKey("OneClappLatestThemeSchemeId")]
        public virtual OneClappLatestThemeScheme OneClappLatestThemeScheme { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}