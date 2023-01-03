using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Tables
{
    [Table("IntProviderContact", Schema = "AppCRM")]
    public class IntProviderContact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string ContactId { get; set; }
        public long? ClientIntProviderAppSecretId { get; set; }
        [ForeignKey("ClientIntProviderAppSecretId")]
        public virtual ClientIntProviderAppSecret ClientIntProviderAppSecret { get; set; }
        public long? ClientUserId { get; set; }
        [ForeignKey("ClientUserId")]
        public virtual ClientUser ClientUser { get; set; }
        public long? ClientId { get; set; }
        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }
        public int? LoggedInUserId { get; set; }
        [ForeignKey("LoggedInUserId")]
        public virtual User CreatedUser { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        [ForeignKey("UpdatedBy")]
        public virtual User UpdatedUser { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        [Column(TypeName = "jsonb")]
        public object? ContactJson { get; set; }
    }
}