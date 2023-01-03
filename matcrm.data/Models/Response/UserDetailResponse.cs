using System;
using Microsoft.AspNetCore.Http;

namespace matcrm.data.Models.Response
{
    public class UserDetailResponse
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }       
        public string Address { get; set; }        
        public bool IsEmailVerified { get; set; }        
        public DateTime? CreatedOn { get; set; }
        public int? RoleId { get; set; }        
        public long? OneClappLatestThemeConfigId { get; set; }
        public string ProfileImage { get; set; }
        public int? DialCode { get; set; }
        public string? Avatar { get; set; }
        public string RefreshToken { get; set; }
    }
}