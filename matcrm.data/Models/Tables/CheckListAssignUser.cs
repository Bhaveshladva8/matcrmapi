using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    public class CheckListAssignUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? CheckListId { get; set; }

        [ForeignKey("CheckListId")]
        public virtual CheckList CheckList { get; set; }
        public int? TenantId { get; set; }

        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }
        public int? AssignUserId { get; set; }
        [ForeignKey("AssignUserId")]
        public virtual User User { get; set; }

        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
         public DateTime? DeletedOn { get; set; }
    }
}