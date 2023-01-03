using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    // public class AuthenticationCompleteResponse
    // {
    //    public string code { get; set; }
    //     public string client_id { get; set; }
    //     public string client_secret { get; set; }
    //     public string grant_type { get; set; }
    //     public string redirect_uri { get; set; }
    //     public string access_token { get; set; }
    //     public long? expires_in { get; set; }
    //     public string refresh_token { get; set; }
    //     public string scope { get; set; }
    //     public string token_type { get; set; }
    //     public string id_token { get; set; }
    //     public string error { get; set; }
    //     public string error_description { get; set; }
    //     public string eventId { get; set; }
    //     public string email { get; set; }
    //     // public int? userId { get; set; }
    //     // public string providerApp { get; set; }
    //     public string label { get; set; }
    //     public long? AppSecretId { get; set; }
    //     public string type { get; set; }
    //     public int? top { get; set; }
    //     public int? page { get; set; }
    //     public int? skip { get; set; }
    //     public string nextPageToken { get; set; }
    //     public long? teamInboxId { get; set; }
    //     public string? Color { get; set; }
    //     public bool IsValid { get; set; } = true;
    //     public string ErrorMessage { get; set; } = "";
    // }
    public class AuthenticationCompleteResponse{
        public string email { get; set; }
        public bool IsValid { get; set; } = true;
        public string ErrorMessage { get; set; } = "";
    }
}
