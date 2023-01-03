using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class TeamInboxAddUpdateAccessRequest
    {
        public TeamInboxAddUpdateAccessRequest()
        {
            TeamMateIds = new List<int>();            
        }
        public long? Id { get; set; }
        public bool IsPublic { get; set; }
        public List<int> TeamMateIds { get; set; }
        public int tenantId { get; set; }
    }
}
