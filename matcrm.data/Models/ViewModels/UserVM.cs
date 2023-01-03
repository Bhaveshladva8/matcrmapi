using System;
using System.ComponentModel.DataAnnotations;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.ViewModels {
    public class UserVM {
        public int Id { get; set; }
        // [Required]
        public string Tenant { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string Password { get; set; }
        public string TempGuid { get; set; }
        public int? TenantId { get; set; }
        // public DateTime? CreatedDate { get; set; }
        public DateTime? LastLoggedIn { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime? BlockedDate { get; set; }
        public string accessToken { get; set; }
        public string Username { get; set; }
        // [Required]
        public string ApiKey { get; set; }
        public string TenantName { get; set; }
        public int? CreatedBy { get; set; }
        public bool IsAdmin { get; set; }
        public string Address { get; set; }
        public int? RoleId { get; set; }
        public string CustomerId { get; set; }
        public int? WeClappUserId { get; set; }
        public string EmailOTP { get; set; }
        public int? DialCode { get; set; }
        public int? UpdatedBy  { get; set; }
        public int? DeletedBy  { get; set; }
    }

    public class UserVMResult {
        public UserVM Result { get; set; }
    }
}