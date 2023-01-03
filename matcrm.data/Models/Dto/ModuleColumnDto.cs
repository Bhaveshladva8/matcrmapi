using System.Collections.Generic;

namespace matcrm.data.Models.Dto {
    public class ModuleColumnDto {
        public ModuleColumnDto(){
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