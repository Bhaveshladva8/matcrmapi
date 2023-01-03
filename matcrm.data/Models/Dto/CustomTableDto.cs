using System.Collections.Generic;

namespace matcrm.data.Models.Dto {
    public class CustomTableDto {
        public CustomTableDto () {
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