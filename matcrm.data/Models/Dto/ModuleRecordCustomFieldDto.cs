using System;
using System.ComponentModel.DataAnnotations.Schema;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Dto
{
    public class ModuleRecordCustomFieldDto
    {
        public long? Id { get; set; }
        public long? ModuleFieldId { get; set; }
        [ForeignKey("ModuleFieldId")]
        public virtual ModuleField ModuleField { get; set; }
        public long? RecordId { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}