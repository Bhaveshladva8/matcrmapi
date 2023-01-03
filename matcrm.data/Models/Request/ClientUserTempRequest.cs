using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class ClientUserTempRequest
    {
        public long ClientId { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Provider { get; set; }
        public string ProviderApp { get; set; }
    }
}