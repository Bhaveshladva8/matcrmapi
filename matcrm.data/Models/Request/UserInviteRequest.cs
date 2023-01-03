using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Request
{
    public class UserInviteRequest
    {
        public List<EmailDto> Emails { get; set; }
        public int? UserId { get; set; }
        public int? TenantId { get; set; }
    }
}