using System;

namespace matcrm.data.Models.Dto {
    public class CustomerEmailDto {
        public long? Id { get; set; }
        public string Email { get; set; }
        public long? EmailTypeId { get; set; }
        public long? CustomerId { get; set; }
        public int? TenantId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}