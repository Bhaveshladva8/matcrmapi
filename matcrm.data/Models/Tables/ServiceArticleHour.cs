using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Tables
{
    [Table("ServiceArticleHour", Schema = "AppServiceArticle")]
    public class ServiceArticleHour
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? UnitPrice { get; set; }
        public long? ServiceArticleId { get; set; }
        [ForeignKey("ServiceArticleId")]
        public virtual ServiceArticle ServiceArticle { get; set; }
    }
}