using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables {
    public class TableColumnUser {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        
        // In which table you want to sort column
        public long? MasterTableId { get; set; }

        [ForeignKey ("MasterTableId")]
        public virtual CustomTable CustomTable { get; set; }
        public long? TableColumnId { get; set; }

        [ForeignKey ("TableColumnId")]
        public virtual CustomTableColumn CustomTableColumn { get; set; }
        public int? UserId { get; set; }

        [ForeignKey ("UserId")]
        public virtual User User { get; set; }
        public long? Priority { get; set; }
        public Boolean IsHide { get; set; }
        public int? TenantId { get; set; }

        [ForeignKey ("TenantId")]
        public virtual Tenant Tenant { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}