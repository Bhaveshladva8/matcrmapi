using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class ThreadByThreadIdRequest
    {
        public string EventType { get; set; }
        //public bool IsRead { get; set; }
        public string Label { get; set; }
        public long? MailAssignUserId { get; set; }
        public string Provider { get; set; }
        public string ProviderApp { get; set; }
        public string SelectedEmail { get; set; }
        public long? TeamInboxId { get; set; }
        public int? Skip { get; set; }
        public int? Top { get; set; }
        
    }
}
