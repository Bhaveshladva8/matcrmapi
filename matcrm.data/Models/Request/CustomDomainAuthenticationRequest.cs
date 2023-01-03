using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class CustomDomainAuthenticationRequest
    {
        public long? Id { get; set; }
        public string IMAPHost { get; set; }
        public int? IMAPPort { get; set; }
        public string SMTPHost { get; set; }
        public int? SMTPPort { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }        
        public int UserId { get; set; }
        public string Color { get; set; }
        public long? TeamInboxId { get; set; }
        public long? IntProviderAppSecretId { get; set; }
        public bool IsValid { get; set; } = true;
        public string ErrorMessage { get; set; } = "";
        public int? CreatedBy { get; set; }
    }
}
