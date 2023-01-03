using matcrm.data.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class RemoveTeamInboxResponse
    {
        public RemoveTeamInboxResponse()
        {
            TeamMateIds = new List<int>();
            TeamInboxAccesses = new List<TeamInboxAccessDto>();
        }
        public bool IsPublic { get; set; }
        public List<TeamInboxAccessDto> TeamInboxAccesses { get; set; }
        public List<int> TeamMateIds { get; set; }
    }
}
