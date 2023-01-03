using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class MailAssignUserResposne
    {
        //public DateTime? CreatedOn { get; set; }
        public long? Id { get; set; }
        public long? IntProviderAppSecretId { get; set; }
        //public bool IsValid { get; set; } = true;
        //public int UserId { get; set; }
        public int? TeamMemberId { get; set; }
        public string? ThreadId { get; set; }
    }
}
