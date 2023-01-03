using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class MailTokenRequest
    {
        public string code { get; set; }        
        public string grant_type { get; set; }
        public string redirect_uri { get; set; }       
        public string refresh_token { get; set; }            
        public string type { get; set; }        
        public long? teamInboxId { get; set; }
        public string? Color { get; set; }   
        public string Provider { get; set; }
        public string ProviderApp { get; set; }        
        public int UserId { get; set; }
        public long? MailBoxTeamId { get; set; }
    }
}
