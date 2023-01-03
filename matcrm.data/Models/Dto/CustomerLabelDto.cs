using System;

namespace matcrm.data.Models.Dto {
    public class CustomerLabelDto {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public long? LabelId { get; set; }
        public long? CustomerId { get; set; }

        public long? LabelCategoryId { get; set; }
        public int? TenantId { get; set; }
        public long? UserId { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}