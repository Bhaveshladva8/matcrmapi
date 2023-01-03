using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class MateTicketTimeRecordListResponse
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string TotalDuration { get; set; }
        public DateTime? Enddate { get; set; }
        public long TimeRecordCount { get; set; }
    }
}