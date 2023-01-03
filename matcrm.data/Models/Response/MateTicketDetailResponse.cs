using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class MateTicketDetailResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long? ClientId { get; set; }
        public string ClientName { get; set; }
        public long? StatusId { get; set; }
        public string StatusName { get; set; }
        public string StatusColor { get; set; }
        public long? MatePriorityId { get; set; }
        public string MatePriority { get; set; }
        public string MatePriorityColor { get; set; }
        public long? MateCategoryId { get; set; }
        public string MateCategory { get; set; }
        public long TotalTimeTracked { get; set; }
        public List<MateTicketUserDetailResponse> AssignUsers { get; set; }
    }

    public class MateTicketUserDetailResponse
    {
        public long? Id { get; set; }
        public int? UserId { get; set; }
        public string ProfileURL { get; set; }
    }
}