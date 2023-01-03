using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Tables
{
    [Table("ClientIntProviderAppSecret", Schema = "AppCRM")]
    public class ClientIntProviderAppSecret
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Access_Token { get; set; }
        public long? Expires_In { get; set; }
        public string Refresh_Token { get; set; }
        public string Scope { get; set; }
        public string Token_Type { get; set; }
        public string Id_Token { get; set; }
        public string Email { get; set; }
        public long? IntProviderAppId { get; set; }
        [ForeignKey("IntProviderAppId")]
        public virtual IntProviderApp IntProviderApp { get; set; }
        public long? ClientId { get; set; }
        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }
        public DateTime? LastAccessedOn { get; set; }
        public int? LoggedInUserId { get; set; }
        [ForeignKey("LoggedInUserId")]
        public virtual User CreatedUser { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        [ForeignKey("UpdatedBy")]
        public virtual User UpdatedUser { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}