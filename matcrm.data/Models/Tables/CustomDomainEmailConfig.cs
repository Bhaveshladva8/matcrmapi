using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Tables
{
    public class CustomDomainEmailConfig
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Column(TypeName = "varchar(1000)")]
        public string Name { get; set; }
        [Column(TypeName = "varchar(1000)")]
        public string IMAPHost { get; set; }
        public int? IMAPPort { get; set; }
        [Column(TypeName = "varchar(1000)")]
        public string SMTPHost { get; set; }
        public int? SMTPPort { get; set; }

        [Column(TypeName = "varchar(1000)")]
        public string Email { get; set; }
        [Column(TypeName = "varchar(10000)")]
        public string Password { get; set; }
        public long? IntProviderAppSecretId { get; set; }

        [ForeignKey("IntProviderAppSecretId")]
        public IntProviderAppSecret IntProviderAppSecret { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}