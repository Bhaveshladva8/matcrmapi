using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class MateTicketTaskAddUpdateRequest
    {       
        public long? Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }        
        public long? StatusId { get; set; }
        public long? MatePriorityId { get; set; }
        public long? MateTicketId { get; set; }
        public List<EmployeeTaskUserRequestResponse> AssignedUsers { get; set; }       
        
    }
    public class MateTicketTaskUserAddRequest
    {
        public long? Id { get; set; }
        public int? UserId { get; set; }
    }
}