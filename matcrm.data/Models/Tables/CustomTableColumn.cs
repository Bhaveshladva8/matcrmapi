using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    public class CustomTableColumn
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column(TypeName = "varchar(500)")]
        public string? Name { get; set; }
        [Column(TypeName = "varchar(500)")]
        public string? Key { get; set; }
        public long? ControlId { get; set; }

        [ForeignKey("ControlId")]
        public virtual CustomControl CustomControl { get; set; }
        public long? MasterTableId { get; set; }

        [ForeignKey("MasterTableId")]
        public virtual CustomTable CustomTable { get; set; }
        public Boolean IsDefault { get; set; }
        public long? CustomFieldId { get; set; }

        [ForeignKey("CustomFieldId")]
        public virtual CustomField CustomField { get; set; }
        public int? TenantId { get; set; }

        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}