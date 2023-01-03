using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class DiscussionAllTeamMateResponse
    {
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public string Email { get; set; }
        public int Id { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsEmailValid { get; set; } = true;
        public bool IsEmailVerified { get; set; }
        public bool IsSignUp { get; set; }
        public bool IsSubscribed { get; set; }
        public bool IsSuccessSignUp { get; set; }
        public bool IsUserAlreadyExist { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public int? RoleId { get; set; }
        public string TempGuid { get; set; }
        public int? TenantId { get; set; }
    }
}
