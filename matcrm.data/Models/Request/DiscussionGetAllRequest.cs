using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class DiscussionGetAllRequest
    {
        //public bool IsRead { get; set; }
        public string label { get; set; }
        // public int? page { get; set; }
        // public string Provider { get; set; }
        // public string ProviderApp { get; set; }
        public int? skip { get; set; }
        public int? top { get; set; }
    }
}
