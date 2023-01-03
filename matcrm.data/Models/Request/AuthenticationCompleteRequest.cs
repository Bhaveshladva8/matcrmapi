using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class AuthenticationCompleteRequest
    {
        public string code { get; set; }
        public string? Color { get; set; }
        public string Provider { get; set; }
        public string ProviderApp { get; set; }
        public string redirect_uri { get; set; }
        public int? UserId { get; set; }
        public long? teamInboxId { get; set; }
        public string SelectedEmail { get; set; }
        // public bool IsValid { get; set; } = true;
        // public string ErrorMessage { get; set; } = "";
        // public string email { get; set; }
    }
}
