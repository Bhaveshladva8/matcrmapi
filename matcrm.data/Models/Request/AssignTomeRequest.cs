using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class AssignTomeRequest
    {
        public string Provider { get; set; }
        public string ProviderApp { get; set; }
        public int? skip { get; set; }
        public int? top { get; set; }
        public string type { get; set; }
        public string SelectedEmail { get; set; }
        public string nextPageToken { get; set; }
        public string label { get; set; }
        
    }
}
