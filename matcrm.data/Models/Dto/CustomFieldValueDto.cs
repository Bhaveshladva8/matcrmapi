using System;

namespace matcrm.data.Models.Dto {
    public class CustomFieldValueDto {
        public long? Id { get; set; }
        public long? ModuleId { get; set; }
        public long? FieldId { get; set; }
        public string ControlType { get; set; }
        public long? RecordId { get; set; }
        public string Option { get; set; }
        public long? OptionId { get; set; }
        public string Value { get; set; }
        public long? TenantId { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public long? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}