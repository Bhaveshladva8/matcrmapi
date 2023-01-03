using System;
using System.ComponentModel.DataAnnotations;

namespace matcrm.data.Models.Dto
{
    public class CustomTableColumnDto
    {
        public long? Id { get; set; }

        [StringLength(500)]
        public string Name { get; set; }
        public string? Key { get; set; }
        public long? ControlId { get; set; }
        public string ControlType { get; set; }
        public long? MasterTableId { get; set; }
        public long? CustomFieldId { get; set; }
        public long? Priority { get; set; }
        public Boolean IsDefault { get; set; }
        public int? TenantId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string TableName { get; set; }
        public int? UserId { get; set; }
        public Boolean IsHide { get; set; }
    }
}