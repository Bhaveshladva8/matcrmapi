using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class StatusDeleteResponse
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public bool IsDeleted { get; set; }        
    }
}