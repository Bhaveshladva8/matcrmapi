using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Tables
{
    [Table("ServiceArticle", Schema = "AppServiceArticle")]
    public class ServiceArticle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Column(TypeName = "varchar(10000)")]
        public string Name { get; set; }
        public string Description { get; set; }
        public long? UnitPrice { get; set; }
        public long? CurrencyId { get; set; }
        [ForeignKey("CurrencyId")]
        public virtual Currency Currency { get; set; }
        public long? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual ServiceArticleCategory ServiceArticleCategory { get; set; }
        public bool IsTaxable { get; set; }
        public long? TaxId { get; set; }
        [ForeignKey("TaxId")]
        public virtual Tax Tax { get; set; }
        public int? CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public virtual User User { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}