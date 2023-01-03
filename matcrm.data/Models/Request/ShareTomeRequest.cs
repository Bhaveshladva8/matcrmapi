using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class ShareTomeRequest
    {
        public bool IsRead { get; set; }
        public string label { get; set; }
        public int? page { get; set; }
        public string Provider { get; set; }
        public string ProviderApp { get; set; }
        public string SelectedEmail { get; set; }
        public int? skip { get; set; }
        public int? top { get; set; }
        public int tenantId { get; set; }
        public int userId { get; set; }
        public string type { get; set; }
        public string nextPageToken { get; set; }
    }
}
