using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class StatusUpdateRequest
    {
        public long? Id { get; set; }        
        public string? Name { get; set; }
        public string? Color { get; set; }       
        public long CustomTableId { get; set; }
    }
}