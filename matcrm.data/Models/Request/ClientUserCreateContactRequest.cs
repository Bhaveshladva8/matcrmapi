using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class ClientUserCreateContactRequest
    {
        public ClientUserCreateContactRequest()
        {
            Email = new List<ClientUserCreateContactemailRequest>();
        }
        public string ContactId { get; set; }
        public long ClientId { get; set; }
        public string Provider { get; set; }
        public string ProviderApp { get; set; }
        public string SelectedEmail { get; set; }        
        public string FirstName { get; set; }
        public string Lastname { get; set; }
        public List<ClientUserCreateContactemailRequest> Email { get; set; }
    }

    public class ClientUserCreateContactemailRequest
    {
        public string Email { get; set; }       
    }
    public class ClientUserMicrosoftContactEmailRequest
    {        
        public string address { get; set; }        
    }
    public class ClientUserGoogleContactEmailRequest
    {   
        public string value { get; set; }
        public string displayName { get; set; }
    }
}