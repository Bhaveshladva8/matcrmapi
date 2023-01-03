using System;
using System.Collections.Generic;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Dto
{
    public class MailBoxTeamDto
    {
        public MailBoxTeamDto(){
            TeamInboxes = new List<TeamInboxDto>();
        }
        public long Id { get; set; }
        public string? Name { get; set; }
        public int? TenantId { get; set; }
        public long? CreatedBy { get; set; }
        public List<TeamInboxDto> TeamInboxes { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}