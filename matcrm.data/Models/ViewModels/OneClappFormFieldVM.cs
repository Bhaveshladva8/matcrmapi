using System;

namespace matcrm.data.Models.ViewModels
{
    public class OneClappFormFieldVM
    {
        
        public long? Id { get; set; }
        public string Name { get; set; }
        public long? OneClappFormId { get; set; }
        public long? OneClappFormFieldId { get; set; }
        public long? CustomModuleId { get; set; }
        public long? CustomFieldId { get; set; }
        public string LabelName { get; set; }
        public string PlaceHolder { get; set; }
        public string CssClassName { get; set; }
        public bool IsRequired { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}