using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class ClientAppointmentDetailResponse
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? StatusId { get; set; }
        public string Status { get; set; }
        public long? ClientUserId { get; set; }
        public string ClientUserName { get; set; }
        
    }
}