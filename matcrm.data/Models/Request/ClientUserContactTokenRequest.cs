using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class ClientUserContactTokenRequest
    {
        public string code { get; set; }        
        public string redirect_uri { get; set; }
        public string refresh_token { get; set; }        
        public string Provider { get; set; }
        public string ProviderApp { get; set; }
        public string? Color { get; set; }
        public long ClientId { get; set; }
        public string Email { get; set; }
    }
}