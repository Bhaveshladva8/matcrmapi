using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    [Table("MailParticipant", Schema = "AppMail")]
    public class MailParticipant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string? ThreadId { get; set; }
        public long? IntProviderAppSecretId { get; set; }
        [ForeignKey("IntProviderAppSecretId")]
        public virtual IntProviderAppSecret IntProviderAppSecret { get; set; }
        public int? TeamMemberId { get; set; }

        [ForeignKey("TeamMemberId")]
        public virtual User User { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}