using Microsoft.AspNetCore.Http;
using matcrm.data.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class DiscussionAssignCustomerResponse
    {
        public long? Id { get; set; }
        public long? CustomerId { get; set; }
        public DateTime? CreatedOn { get; set; }       
        
    }
}
