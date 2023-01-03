using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class MateTicketAddUpdateRequest
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long? StatusId { get; set; }
        public long? MateCategoryId { get; set; }
        public long? MatePriorityId { get; set; }
        public long? EmployeeProjectId { get; set; }
        public long? ClientId { get; set; }
        public List<MateTicketUserAddUpdateRequest> AssignedUsers { get; set; }
    }

    public class MateTicketUserAddUpdateRequest
    {
        public long? Id { get; set; }
        public int? UserId { get; set; }
    }
}