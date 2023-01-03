using System;
using System.ComponentModel.DataAnnotations;

namespace matcrm.data.Models.Dto
{
    public class EmailConfigDto
    {
        public long? Id { get; set; }
        public int? EmailProviderId { get; set; }

        [StringLength (254)]
        public string Email { get; set; }
        public string Password { get; set; }
        public long? TenantId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }

    public class EmailProviderConfigDto
    {
         public long? Id { get; set; }
        public int? EmailProviderId { get; set; }

        [StringLength (254)]
        public string Email { get; set; }
        public string Password { get; set; }
        public long? TenantId { get; set; }
        public bool IsActive { get; set; }

         public string ProviderName { get; set; }
        public string Host { get; set; }
        public int? Port { get; set; }
       
    }
}