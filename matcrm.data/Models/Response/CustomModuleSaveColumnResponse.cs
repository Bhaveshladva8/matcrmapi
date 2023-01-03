using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Response
{
    public class CustomModuleSaveColumnResponse
    {
        public CustomModuleSaveColumnResponse(){
            DisplayColumns = new List<CustomTableColumnDto>();
            Tables = new List<CustomTableDto>();
            ColumnUsers = new List<TableColumnUserDto>();
        }
        public List<CustomTableColumnDto> DisplayColumns { get; set; }

        public List<TableColumnUserDto> ColumnUsers { get; set; }
        public List<CustomTableDto> Tables { get; set; }
        public int? UserId { get; set; }
        public int? TenantId { get; set; }
    }
}