using System;

namespace matcrm.data.Models.Dto {
    public class CustomerPhoneDto {
        public long? Id { get; set; }
        public string PhoneNo { get; set; }
        public long? PhoneNoTypeId { get; set; }
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