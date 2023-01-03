using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class MateTicketAssignUserRequest
    {
        public long TicketId { get; set; }
        public List<int> AssignedUsers { get; set; }
    }
}