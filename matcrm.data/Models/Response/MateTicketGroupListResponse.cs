using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class MateTicketGroupListResponse
    {
        public List<MateTicketStatusListResponse> StatusList { get; set; }
        public List<MateProjectTicketGroupTicketListResponse> ProjectTicketList { get; set; }
        // public string TotalDuration { get; set; }
        // public DateTime? Enddate { get; set; }
        // public long TimeRecordCount { get; set; }
    }

    public class MateTicketStatusListResponse
    {
        public long? StatusId { get; set; }
        public string StatusName { get; set; }
        public string StatusColor { get; set; }
        public long TotalCount { get; set; }
    }
    public class MateProjectTicketGroupTicketListResponse
    {
        public long ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectLogoURL { get; set; }
        public long TotalCount { get; set; }
    }
}