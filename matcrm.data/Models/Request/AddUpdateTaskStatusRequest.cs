using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Request
{
    public class AddUpdateTaskStatusRequest
    {
        public AddUpdateTaskStatusRequest()
        {
            CustomFields = new List<CustomFieldDto>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public List<CustomFieldDto> CustomFields { get; set; }

    }
}