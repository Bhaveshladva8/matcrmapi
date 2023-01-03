using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    public class EmailTemplate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long EmailTemplateId { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string? TemplateName { get; set; }

        [Column(TypeName = "varchar(5)")]
        public string? TemplateCode { get; set; }
        public string? Description { get; set; }
        public string? TemplateHtml { get; set; }

        public DateTime? CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedOn { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}