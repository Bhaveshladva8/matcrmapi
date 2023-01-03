using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    [Table("OneClappRequestForm", Schema = "AppForm")]
    public class OneClappRequestForm
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? OneClappFormId { get; set; }
        public long? OneClappFormStatusId { get; set; }
        [ForeignKey("OneClappFormStatusId")]
        public virtual OneClappFormStatus OneClappFormStatus { get; set; }
        public bool IsVerify { get; set; }
        public int? TenantId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}