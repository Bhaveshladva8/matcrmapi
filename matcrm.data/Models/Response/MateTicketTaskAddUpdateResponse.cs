using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class MateTicketTaskAddUpdateResponse
    {
        public long? Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public long? MateTicketId { get; set; }
    }
}