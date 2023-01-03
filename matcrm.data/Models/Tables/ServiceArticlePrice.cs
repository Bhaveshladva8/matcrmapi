using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Tables
{
    [Table("ServiceArticlePrice", Schema = "AppServiceArticle")]
    public class ServiceArticlePrice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? ClientId { get; set; }
        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }
        public long? ServiceArticleId { get; set; }
        [ForeignKey("ServiceArticleId")]
        public virtual ServiceArticle ServiceArticle { get; set; }
        public long? Price { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsBillable { get; set; }
        public int? LoggedInUserId { get; set; }
        [ForeignKey("LoggedInUserId")]
        public virtual User CreatedUser { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}