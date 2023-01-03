using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class MailAssignCustomerRequest
    {
        public string Provider { get; set; }
        public string ProviderApp { get; set; }
        public string SelectedEmail { get; set; }        
        public string? ThreadId { get; set; }
        public long? CustomerId { get; set; }
        public long? IntProviderAppSecretId { get; set; }
        public int? CreatedBy { get; set; }
    }
}
