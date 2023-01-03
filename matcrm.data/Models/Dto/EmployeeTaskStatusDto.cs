using System;

namespace matcrm.data.Models.Dto {
    public class EmployeeTaskStatusDto {
        public long? Id { get; set; }
        public string Name { get; set; }
        public int? TenantId { get; set; }
        public int? UserId { get; set; }
        public string Color { get; set; }
        public bool IsFinalize { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}