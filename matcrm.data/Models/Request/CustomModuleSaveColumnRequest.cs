using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Request
{
    public class CustomModuleSaveColumnRequest
    {
        public CustomModuleSaveColumnRequest(){
            DisplayColumns = new List<CustomTableColumnDto>();
           
        }
        public List<CustomTableColumnDto> DisplayColumns { get; set; }
        public int? UserId { get; set; }
        public int? TenantId { get; set; }
    }
}