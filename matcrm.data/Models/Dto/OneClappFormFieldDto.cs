using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Dto
{
    public class OneClappFormFieldDto
    {
        public OneClappFormFieldDto()
        {
            LinkedEntities = new List<CustomTableDto>();
            CustomFieldValues = new List<CustomFieldValueDto>();
            FormFieldValues = new List<OneClappFormFieldValueDto>();
            CustomControlOptions = new List<CustomControlOptionDto>();
        }
        public long? Id { get; set; }
        public string Name { get; set; }
        public long? OneClappFormId { get; set; }
        public long? OneClappFormFieldId { get; set; }
        public long? CustomFieldId { get; set; }
        [ForeignKey("CustomFieldId")]
        public virtual CustomField CustomField { get; set; }
        public long? CustomTableColumnId { get; set; }
        [ForeignKey("CustomTableColumnId")]
        public virtual CustomTableColumn CustomTableColumn { get; set; }
        public long? CustomModuleId { get; set; }
        public string LabelName { get; set; }
        public string PlaceHolder { get; set; }
        public string CssClassName { get; set; }
        public long? Priority { get; set; }
        public bool IsRequired { get; set; }
        [Column(TypeName = "jsonb")]
        public object? FormFieldStyle { get; set; }

        [Column(TypeName = "jsonb")]
        public object? TypographyStyle { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public CustomFieldDto CustomFieldDto { get; set; }
        public List<CustomTableDto> LinkedEntities { get; set; }
        public CustomControlDto CustomControl { get; set; }
        public List<CustomFieldValueDto> CustomFieldValues { get; set; }
        public List<CustomControlOptionDto> CustomControlOptions { get; set; }
        public List<OneClappFormFieldValueDto> FormFieldValues { get; set; }
    }
}