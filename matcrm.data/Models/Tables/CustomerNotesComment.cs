using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables {
    [Table("CustomerNotesComment", Schema = "AppCRM")]
    public class CustomerNotesComment {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? CustomerNoteId { get; set; }

        [ForeignKey ("CustomerNoteId")]
        public virtual CustomerNote CustomerNote { get; set; }
        public string? Comment { get; set; }
        public int? CreatedBy { get; set; }

        [ForeignKey ("CreatedBy")]
        public virtual User User { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}