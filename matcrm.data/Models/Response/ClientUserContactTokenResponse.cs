using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class ClientUserContactTokenResponse
    {
        public ClientUserContactTokenResponse()
        {
            Contacts = new List<ClientUserContactsList>();
            //homePhones = new List<MicrosoftClientContactPhoneVM>();
        }
        public string email { get; set; }
        public bool IsValid { get; set; } = true;
        public string error { get; set; }
        public List<ClientUserContactsList> Contacts{ get; set; }        
    }
}