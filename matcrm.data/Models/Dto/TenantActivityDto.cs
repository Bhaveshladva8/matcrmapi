using System;

namespace matcrm.data.Models.Dto {
    public class TenantActivityDto {
        public long? Id { get; set; }
        public long? TenantId { get; set; }
        public long? UserId { get; set; }
        public string Activity { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}