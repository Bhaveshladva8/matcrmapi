using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables {
    [Table("OrganizationNotesComment", Schema = "AppCRM")]
    public class OrganizationNotesComment {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? OrganizationNoteId { get; set; }

        [ForeignKey ("OrganizationNoteId")]
        public virtual OrganizationNote OrganizationNote { get; set; }
        public string? Comment { get; set; }
        public int? CreatedBy { get; set; }

        [ForeignKey ("CreatedBy")]
        public virtual User User { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}