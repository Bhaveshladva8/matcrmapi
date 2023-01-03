using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    [Table("OneClappFormField", Schema = "AppForm")]
    public class OneClappFormField
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? OneClappFormId { get; set; }

        [ForeignKey("OneClappFormId")]
        public virtual OneClappForm OneClappForm { get; set; }
        public long? CustomFieldId { get; set; }

        [ForeignKey("CustomFieldId")]
        public virtual CustomField CustomField { get; set; }
        public long? CustomTableColumnId { get; set; }
        [ForeignKey("CustomTableColumnId")]
        public virtual CustomTableColumn CustomTableColumn { get; set; }
        public long? CustomModuleId { get; set; }
        [ForeignKey("CustomModuleId")]
        public virtual CustomModule CustomModule { get; set; }
        [Column(TypeName = "varchar(1000)")]
        public string? LabelName { get; set; }
        [Column(TypeName = "varchar(1000)")]
        public string? PlaceHolder { get; set; }
        [Column(TypeName = "varchar(1000)")]
        public string? CssClassName { get; set; }

        [Column(TypeName = "jsonb")]
        public object? FormFieldStyle { get; set; }

        [Column(TypeName = "jsonb")]
        public object? TypographyStyle { get; set; }
        public bool IsRequired { get; set; }
        public long? Priority { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}