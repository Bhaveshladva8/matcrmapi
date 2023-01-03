using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    [Table("TeamInbox", Schema = "AppMail")]
    public class TeamInbox
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Column(TypeName = "varchar(1000)")]
        public string Name { get; set; }
        public long? IntProviderAppSecretId { get; set; }

        [ForeignKey("IntProviderAppSecretId")]
        public IntProviderAppSecret IntProviderAppSecret { get; set; }

        public long? MailBoxTeamId { get; set; }

        [ForeignKey("MailBoxTeamId")]
        public MailBoxTeam MailBoxTeam { get; set; }
        [Column(TypeName = "varchar(250)")]
        public string Color { get; set; }
        public bool IsPublic { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }

    }
}