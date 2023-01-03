using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    [Table("TeamInboxAccess", Schema = "AppMail")]
    public class TeamInboxAccess
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? TeamInboxId { get; set; }
        [ForeignKey("TeamInboxId")]
        public virtual TeamInbox TeamInbox { get; set; }
        public int? TeamMateId { get; set; }
        [ForeignKey("TeamMateId")]
        public virtual User TeamMate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}