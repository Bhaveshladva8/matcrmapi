using System.Collections.Generic;

namespace matcrm.data.Models.Dto
{
    public class InviteUserDto
    {
        public List<EmailDto> Emails { get; set; }
        public int? UserId { get; set; }
        public int? TenantId { get; set; }
    }

    public class EmailDto
    {
        public string Email { get; set; }
    }
}