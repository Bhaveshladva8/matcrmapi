using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables {
    public class EmailLog {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string? Body { get; set; }
        public bool Status { get; set; }

        [Column (TypeName = "varchar(254)")]
        public string? FromEmail { get; set; }

        [Column (TypeName = "varchar(254)")]
        public string? ToEmail { get; set; }
        public string? Subject { get; set; }
        public long? TenantId { get; set; }

        [Column (TypeName = "varchar(5)")]
        public string? TemplateCode { get; set; }
        public string? FromLabel { get; set; }
        public int? Tried { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}