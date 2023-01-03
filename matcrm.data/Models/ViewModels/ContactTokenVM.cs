using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.ViewModels
{
    public class ContactTokenVM : CommonResponse
    {
        public string code { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string grant_type { get; set; }
        public string redirect_uri { get; set; }
        public string access_token { get; set; }
        public long? expires_in { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
        public string token_type { get; set; }
        public string id_token { get; set; }
        public string error { get; set; }
        public string error_description { get; set; }
        public string contactId { get; set; }
        public string email { get; set; }
        public long? AppSecretId { get; set; }
        public string type { get; set; }
    }
    public class ClientUserContactTokenVM
    {
        public string displayName { get; set; }
        public string surname { get; set; }
        public string givenName { get; set; }
        public string id { get; set; }
        public string userPrincipalName { get; set; }
        public string email { get; set; }
    }
}