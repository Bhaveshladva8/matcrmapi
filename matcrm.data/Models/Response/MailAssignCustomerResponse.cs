using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class MailAssignCustomerResponse
    {        
        public long? Id { get; set; }
        public long? IntProviderAppSecretId { get; set; }        
        public string? ThreadId { get; set; }        
        public long? CustomerId { get; set; }

    }
}
