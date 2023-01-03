using System;

namespace matcrm.data.Models.Dto {
    public class RoleDto {
        public int? RoleId { get; set; }
        public string RoleName { get; set; }
        public long? TenantId { get; set; }
        public bool IsActive { get; set; }
         public bool IsDefault { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
         public long? CreatedBy {get; set;}
        public long? UpdatedBy {get; set;}
        public long? DeletedBy {get; set;}
        public long? userId {get; set;}
    }
}