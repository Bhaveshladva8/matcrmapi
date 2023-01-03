using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class MateProjectTicketListResponse
    {
        public long Id { get; set; }
        public string TicketNo { get; set; }
        public string Name { get; set; }
        public long? StatusId { get; set; }
        public string Status { get; set; }
        public long ClientId { get; set; }
        public string ClientName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public List<MateProjectTicketUserListResponse> AssignedUsers { get; set; }
    }
    public class MateProjectTicketUserListResponse
    {
        public long? UserId { get; set; }
        public string ProfileURL { get; set; }
    }
}