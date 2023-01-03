using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class MateTicketTaskListResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<MateTicketTaskUserListResponse> AssignUsers { get; set; }
    }
    public class MateTicketTaskUserListResponse
    {
        public long? Id { get; set; }
        public int? UserId { get; set; }
        public string ProfileURL { get; set; }
    }
}