using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Response
{
    public class OneClappFormRemoveResponse
    {
        public OneClappFormRemoveResponse()
        {
            // CustomFields = new List<CustomFieldDto>();
            Fields = new List<OneClappFormFieldDto>();
            FormHeader = new OneClappFormHeaderDto();
            FormLayout = new OneClappFormLayoutDto();
        }
        public long? Id { get; set; }
        public List<OneClappFormFieldDto> Fields { get; set; }
        public OneClappFormHeaderDto FormHeader { get; set; }
        public OneClappFormLayoutDto FormLayout { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsUseCssClass { get; set; }
        public bool IsUsePlaceHolder { get; set; }
    }
}