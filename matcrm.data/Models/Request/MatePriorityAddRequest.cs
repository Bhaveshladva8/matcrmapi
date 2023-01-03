using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class MatePriorityAddRequest
    {
        public MatePriorityAddRequest()
        {
            CustomTableIds = new List<long>();
        }
        public long? Id { get; set; }        
        public string Name { get; set; }
        public string Color { get; set; }       
        public List<long> CustomTableIds { get; set; }
    }
}