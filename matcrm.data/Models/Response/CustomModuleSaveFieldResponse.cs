using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Response
{
    public class CustomModuleSaveFieldResponse
    {
        public CustomModuleSaveFieldResponse()
        {
            CustomFieldValues = new List<CustomFieldValueDto>();
            CustomControlOptions = new List<CustomControlOptionDto>();
        }
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long? ControlId { get; set; }       
        public bool IsRequired { get; set; }
        public bool IsHide { get; set; }
        public List<CustomTableDto> LinkedEntities { get; set; }
        public CustomControlDto CustomControl { get; set; }
        public List<CustomFieldValueDto> CustomFieldValues { get; set; }
       public List<CustomControlOptionDto> CustomControlOptions { get; set; }        
        public bool IsRecordField { get; set; }        
    }
}