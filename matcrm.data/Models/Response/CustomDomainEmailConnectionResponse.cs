using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class CustomDomainEmailConnectionResponse
    {
        public long? Id { get; set; }
        public string Name { get; set; }               
        public string Email { get; set; }
        public string IMAPHost { get; set; }
        public int? IMAPPort { get; set; }
        public string Password { get; set; }
        public string SMTPHost { get; set; }
        public int? SMTPPort { get; set; }
        public int UserId { get; set; }
    }
}
