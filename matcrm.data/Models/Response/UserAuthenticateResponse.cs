using System;
using Microsoft.AspNetCore.Http;

namespace matcrm.data.Models.Response
{
    public class UserAuthenticateResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? TenantId { get; set; }
        public string Tenant { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        // public string Password { get; set; }
        // public byte[] PasswordHash { get; set; }
        // public byte[] PasswordSalt { get; set; }
        public DateTime? DOB { get; set; }
        public string Address { get; set; }
        public int? WeClappUserId { get; set; }
        public string WeClappToken { get; set; }
        public bool IsEmailVerified { get; set; }
        public DateTime? VerifiedOn { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? LastLoggedIn { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime? BlockedOn { get; set; }
        public int? RoleId { get; set; }
        public string RoleName { get; set; }
        public long? OneClappLatestThemeConfigId { get; set; }
        public int? BlockedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public bool IsSuccessSignUp { get; set; }
        public bool IsEmailValid { get; set; } = true;
        public bool IsUserAlreadyExist { get; set; }
        public string ErrorMessage { get; set; }
        public string EmailOTP { get; set; }
        public string TempGuid { get; set; }
        public string ShortName { get; set; }
        public bool IsSignUp { get; set; }
        public string ProfileImage { get; set; }
        public IFormFile File { get; set; }
        public int? DialCode { get; set; }
        public int? UpdatedBy { get; set; }
        public int? DeletedBy { get; set; }
        public bool IsSubscribed { get; set; }
        public long? MailBoxTeamId { get; set; }
        public string? Avatar { get; set; }
        public bool IsExistPW { get; set; }

    }
}