using System;
using System.ComponentModel.DataAnnotations.Schema;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Dto
{
    public class OneClappFormFieldValueDto
    {
        // public long? Id { get; set; }
        // public long? OneClappFormId { get; set; }
        // public long? OneClappFormFieldId { get; set; }
        // public long? CustomFieldId { get; set; }
        // public string ControlType { get; set; }
        // public long? OneClappRequestFormId { get; set; }
        // public long? OptionId { get; set; }
        // [ForeignKey("OptionId")]
        // public virtual CustomControlOption CustomControlOption { get; set; }
        // public string Value { get; set; }
        // public long? TenantId { get; set; }
        // public long? CreatedBy { get; set; }
        // public DateTime? CreatedOn { get; set; }
        // public long? UpdatedBy { get; set; }
        // public long? UpdatedOn { get; set; }
        // public bool IsDeleted { get; set; }
        // public long? DeletedBy { get; set; }
        // public DateTime? DeletedOn { get; set; }

        public long? Id { get; set; }
        public long? OneClappFormId { get; set; }
        public long? OneClappFormFieldId { get; set; }
        public long? CustomFieldId { get; set; }
        public long? OneClappRequestFormId { get; set; }
        public long? OptionId { get; set; }
        [ForeignKey("OptionId")]
        public virtual CustomControlOption CustomControlOption { get; set; }
        public string Value { get; set; }
        public long? TenantId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string ControlType { get; set; }
    }
}