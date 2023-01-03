using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables {
    public class Role {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }
        [Column (TypeName = "varchar(150)")]
        public string? RoleName { get; set; }
        public long? TenantId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }

        public long? CreatedBy {get; set;}
        public long? UpdatedBy {get; set;}
        public long? DeletedBy {get; set;}
    }
}