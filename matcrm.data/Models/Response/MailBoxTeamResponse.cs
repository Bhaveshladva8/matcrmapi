using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using matcrm.data.Models.Tables;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Response
{
    public class MailBoxTeamResponse
    {
        public MailBoxTeamResponse()
        {
            TeamInboxes = new List<TeamInboxDto>();
        }
        public long Id { get; set; }
        public string? Name { get; set; }
        public int? TenantId { get; set; }
        public long? CreatedBy { get; set; }
        public List<TeamInboxDto> TeamInboxes { get; set; }
        public DateTime? CreatedOn { get; set; }
        
    }
}
