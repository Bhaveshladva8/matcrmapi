using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class DiscussionAssignCustomerRequest
    {
        public long? Id { get; set; }
        public long? CustomerId { get; set; }
        //public int? TenantId { get; set; }
    }
}
