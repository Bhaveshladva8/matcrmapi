using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Request
{
    public class OneClappRequestFormAddUpdateRequest
    {
        public OneClappRequestFormAddUpdateRequest()
        {
            FormFieldValues = new List<OneClappFormFieldValueDto>();
            
        }
        
        public long? OneClappFormId { get; set; }        
        public int? TenantId { get; set; }        
        public List<OneClappFormFieldValueDto> FormFieldValues { get; set; }        
        
    }
}