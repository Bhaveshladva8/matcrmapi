using System.Collections.Generic;

namespace matcrm.data.Models.Dto
{
    public class CustomFieldDto
    {
        public CustomFieldDto()
        {
            CustomFieldValues = new List<CustomFieldValueDto>();
            CustomControlOptions = new List<CustomControlOptionDto>();
        }
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public string Description { get; set; }
        public long? ControlId { get; set; }
        public long? RecordId { get; set; }
        public bool IsRequired { get; set; }
        public bool IsHide { get; set; }
        public string TableName { get; set; }
        public long? CreatedBy { get; set; }
        public long? TenantId { get; set; }
        public string Value { get; set; }
        public List<CustomTableDto> LinkedEntities { get; set; }
        public CustomControlDto CustomControl { get; set; }
        public List<CustomFieldValueDto> CustomFieldValues { get; set; }
        public List<CustomControlOptionDto> CustomControlOptions { get; set; }
        public long? CustomTableColumnId { get; set; }
        public bool IsRecordField { get; set; }
        public string LabelName { get; set; }
        public string PlaceHolder { get; set; }
        public string CssClassName { get; set; }
    }
}