using System;
using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Response
{
    public class OneClappFormPositionResponse
    {
        public OneClappFormPositionResponse()
        {
            
            Fields = new List<OneClappFormFieldDto>();
            
            FormHeader = new OneClappFormHeaderDto();
            FormLayout = new OneClappFormLayoutDto();
        }
        public long? Id { get; set; }
        public string Name { get; set; }
        public Guid? FormGuid { get; set; }
        public string FormKey { get; set; }        
        public long? FormActionId { get; set; }
        public bool IsActive { get; set; }        
        public string? SlidingFormPosition { get; set; }       
        public bool IsUsePlaceHolder { get; set; }
        public bool IsUseCssClass { get; set; }
        public object? FormStyle { get; set; }
        public object? LayoutStyle { get; set; }
        public object? HeaderStyle { get; set; }
        public long? FormStyleId { get; set; }        
        public long? FormFieldStyleId { get; set; }
        public long? FormHeaderId { get; set; }
        public OneClappFormHeaderDto FormHeader { get; set; }
        public long? FormLayoutId { get; set; }
        public OneClappFormLayoutDto FormLayout { get; set; }
        public int? TenantId { get; set; }        
        public long? CreatedBy { get; set; }    
        public DateTime? CreatedOn { get; set; }        
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; } 
        public List<OneClappFormFieldDto> Fields { get; set; }
    }
}