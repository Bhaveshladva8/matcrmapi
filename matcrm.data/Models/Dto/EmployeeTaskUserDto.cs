using System;

namespace matcrm.data.Models.Dto
{
    public class EmployeeTaskUserDto
    {
        public long? Id { get; set; }
        public int? UserId { get; set; }
        public long? EmployeeTaskId { get; set; }
        public int? TenantId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string Name { get; set; }
    }
}