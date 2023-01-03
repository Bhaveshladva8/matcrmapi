using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class DeleteCustomEmailRequest
    {
        // public string EventType { get; set; }
        // public bool IsRead { get; set; }        
        // public long MailAssignUserId { get; set; }
        public string SelectedEmail { get; set; }
        public int UserId { get; set; }
        public string label { get; set; }
    }
}
