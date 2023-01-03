using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Response
{
    public class CustomModuleGetAllColumnResponse
    {
        public CustomModuleGetAllColumnResponse () {
            Columns = new List<CustomTableColumnDto> ();
            ShowColumns = new List<CustomTableColumnDto> ();
            HideColumns = new List<CustomTableColumnDto> ();
        }
        public long? Id { get; set; }
        public string Name { get; set; }
        public List<CustomTableColumnDto> Columns { get; set; }
        public List<CustomTableColumnDto> ShowColumns { get; set; }
        public List<CustomTableColumnDto> HideColumns { get; set; }
    }
}