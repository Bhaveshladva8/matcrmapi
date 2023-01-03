using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class MateTicketListResponse
    {
        public long Id { get; set; }
        public string TicketNo { get; set; }
        public string Name { get; set; }
        public long? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectLogoURL { get; set; }
        public long? StatusId { get; set; }
        public string StatusName { get; set; }
        public string StatusColor { get; set; }
        public long? ClientId { get; set; }
        public string ClientName { get; set; }
        public DateTime? Date { get; set; }
        public long? TaskCount { get; set; }
    }



}