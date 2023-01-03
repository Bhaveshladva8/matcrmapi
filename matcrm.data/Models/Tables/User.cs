using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? TenantId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime? DOB { get; set; }
        public string Address { get; set; }

        [Column(TypeName = "varchar(150)")]
        public string TempGuid { get; set; }
        public int? WeClappUserId { get; set; }
        public string WeClappToken { get; set; }
        public string RefreshToken { get; set; }
        public int? RoleId { get; set; }
        public bool IsEmailVerified { get; set; }
        public DateTime? VerifiedOn { get; set; }

        [Column(TypeName = "varchar(1000)")]
        public string ProfileImage { get; set; }

        public long? OneClappLatestThemeConfigId { get; set; }

        [ForeignKey ("OneClappLatestThemeConfigId")]
        public virtual OneClappLatestThemeConfig OneClappLatestThemeConfig { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public int? DialCode { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? LastLoggedIn { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime? BlockedOn { get; set; }
        public int? BlockedBy { get; set; }
        public bool IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}