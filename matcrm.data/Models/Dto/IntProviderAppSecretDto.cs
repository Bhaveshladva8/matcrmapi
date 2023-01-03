using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using matcrm.data.Models.Tables;
using matcrm.data.Models.ViewModels.Calendar;

namespace matcrm.data.Models.Dto
{
    public class IntProviderAppSecretDto
    {
        public IntProviderAppSecretDto(){
            Events = new List<GoogleMicrosoftCalendarEventVM>();
        }
        public long? Id { get; set; }
        public string? Access_Token { get; set; }
        public long? Expires_In { get; set; }
        public string Refresh_Token { get; set; }
        public string Scope { get; set; }
        [StringLength(500)]
        public string Token_Type { get; set; }
        public string Id_Token { get; set; }
        public string Token { get; set; }
        public string error_description { get; set; }

        [Column(TypeName = "varchar(n)")]
        public string Email { get; set; }
        public long? IntProviderAppId { get; set; }
        public bool IsDefault { get; set; }
        public bool IsSelect { get; set; }
        public string? Color { get; set; }
        public long? CustomDomainEmailConfigId { get; set; }

        public CustomDomainEmailConfigDto CustomDomainEmailConfigDto { get; set; }
        public DateTime? LastAccessedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string? ProviderName { get; set; }
        public string? IntProviderAppName { get; set; }
        public virtual IntProviderApp IntProviderApp { get; set; }
        public List<GoogleMicrosoftCalendarEventVM> Events { get; set; }
    }

    public class SecretDto{
        public SecretDto(){
            SelectedIds = new List<long>();
        }
        public List<long> SelectedIds { get; set; }
    }
}