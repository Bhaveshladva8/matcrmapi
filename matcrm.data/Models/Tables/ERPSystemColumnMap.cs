using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    public class ERPSystemColumnMap
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? UserId { get; set; }
        public long? WeClappUserId { get; set; }
        [ForeignKey("WeClappUserId")]
        public virtual WeClappUser WeClappUser { get; set; }
        public long? ERPSystemId { get; set; }
        [ForeignKey("ERPSystemId")]
        public virtual ERPSystem ERPSystem { get; set; }
        public string? SourceColumnName { get; set; }
        public string? DestinationColumnName { get; set; }
        public long? CustomModuleId { get; set; }
        [ForeignKey("CustomModuleId")]
        public virtual CustomModule CustomModule { get; set; }

        public long? CustomFieldId { get; set; }
        [ForeignKey("CustomFieldId")]
        public virtual CustomField CustomField { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}