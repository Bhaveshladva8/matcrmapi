using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class AssignToCustomerRequest
    {
        public string type { get; set; }
        public int? top { get; set; }        
        public int? skip { get; set; }
    }
}
