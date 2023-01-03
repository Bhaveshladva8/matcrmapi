using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables {
    public class TenantConfig {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? TenantId { get; set; }
        public long? UserId { get; set; }

        [Column (TypeName = "varchar(1500)")]
        public string? LogoImage { get; set; }
        public bool IsUploadLogoImg { get; set; }

        [Column (TypeName = "varchar(1500)")]
        public string? BackgroundImage { get; set; }
        public bool IsUploadBgImg { get; set; }

        [Column (TypeName = "varchar(1000)")]
        public string? Font { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}