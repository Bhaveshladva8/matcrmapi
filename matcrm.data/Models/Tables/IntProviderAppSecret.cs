using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    public class IntProviderAppSecret
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string? Access_Token { get; set; }
        public long? Expires_In { get; set; }
        public string? Refresh_Token { get; set; }
        public string? Scope { get; set; }
        [Column(TypeName = "varchar(500)")]
        public string? Token_Type { get; set; }
        public string? Id_Token { get; set; }

        [Column(TypeName = "varchar(254)")]
        public string? Email { get; set; }

        public long? IntProviderAppId { get; set; }
        [ForeignKey("IntProviderAppId")]
        public virtual IntProviderApp IntProviderApp { get; set; }
        public bool IsDefault { get; set; }
        public bool IsSelect { get; set; }
        [Column(TypeName = "varchar(500)")]
        public string? Color { get; set; }
        public DateTime? LastAccessedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}