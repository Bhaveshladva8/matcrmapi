using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    [Table("OneClappFormFieldValue", Schema = "AppForm")]
    public class OneClappFormFieldValue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? OneClappFormId { get; set; }
        [ForeignKey("OneClappFormId")]
        public virtual OneClappForm OneClappForm { get; set; }
        public long? OneClappFormFieldId { get; set; }
        [ForeignKey("OneClappFormFieldId")]
        public virtual OneClappFormField OneClappFormField { get; set; }
        public long? CustomFieldId { get; set; }
        [ForeignKey("CustomFieldId")]
        public virtual CustomField CustomField { get; set; }
        public long? CustomTableColumnId { get; set; }
        [ForeignKey("CustomTableColumnId")]
        public virtual CustomTableColumn CustomTableColumn { get; set; }
        public long? OneClappRequestFormId { get; set; }

        [ForeignKey("OneClappRequestFormId")]
        public virtual OneClappRequestForm OneClappRequestForm { get; set; }

        public long? OptionId { get; set; }

        [ForeignKey("OptionId")]
        public virtual CustomControlOption CustomControlOption { get; set; }
        public string? Value { get; set; }
        public long? TenantId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}