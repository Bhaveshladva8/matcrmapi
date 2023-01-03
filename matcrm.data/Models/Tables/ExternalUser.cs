using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    public class ExternalUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? UserId { get; set; }
        [Column(TypeName = "varchar(500)")]
        public string? FirstName { get; set; }
        [Column(TypeName = "varchar(500)")]
        public string? LastName { get; set; }
        public string? Id_Token { get; set; }

        [Column(TypeName = "varchar(254)")]
        public string? Email { get; set; }
        [Column(TypeName = "varchar(500)")]
        public string? Token_Type { get; set; }
        public long? IntProviderId { get; set; }
        [ForeignKey("IntProviderId")]
        public virtual IntProvider IntProvider { get; set; }
        public DateTime? ExpiredOn { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }

    }
}