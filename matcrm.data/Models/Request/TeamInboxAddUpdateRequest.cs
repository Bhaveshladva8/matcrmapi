using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class TeamInboxAddUpdateRequest
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public bool IsPublic { get; set; }        
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? IntProviderAppSecretId { get; set; }
        public long? MailBoxTeamId { get; set; }
    }
}
