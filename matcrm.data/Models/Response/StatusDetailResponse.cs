using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class StatusDetailResponse
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Color { get; set; }
        public bool IsDefault { get; set; }
        public long? CustomTableId { get; set; }        
        public long? CreatedBy { get; set; }
    }
}