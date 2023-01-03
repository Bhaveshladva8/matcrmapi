using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class MarkAsReadUnReadRequest
    {
        public int? CreatedBy { get; set; }
        public bool IsRead { get; set; }
        public string Label { get; set; }
        public long MailAssignUserId { get; set; }
        public string MessageId { get; set; }
        public int? page { get; set; }
        public string Provider { get; set; }
        public string ProviderApp { get; set; }
        public string SelectedEmail { get; set; }
        public int? skip { get; set; }
        public int? top { get; set; }
        public string type { get; set; }
    }
}
