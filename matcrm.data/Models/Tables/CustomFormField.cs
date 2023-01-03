using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables {
    public class CustomFormField {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? CustomFormId { get; set; }

        [ForeignKey ("CustomFormId")]
        public virtual CustomForm CustomForm { get; set; }
        public long? CustomFieldId { get; set; }

        [ForeignKey ("CustomFieldId")]
        public virtual CustomField CustomField { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }

    }
}