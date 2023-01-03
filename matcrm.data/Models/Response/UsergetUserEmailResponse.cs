using System;

namespace matcrm.data.Models.Response
{
    public class UsergetUserEmailResponse
    {
        public DateTime? CreatedOn { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public int Id { get; set; }
        public DateTime? LastLoggedIn { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsEmailVerified { get; set; }
        public string? LastName { get; set; }
        public int? RoleId { get; set; }
        public string? TempGuid { get; set; }
        public int? TenantId { get; set; }
        public string? UserName { get; set; }
        public DateTime? VerifiedOn { get; set; }
    }
}