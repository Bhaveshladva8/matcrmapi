using System;

namespace matcrm.data.Models.Dto
{
    public class CalendarSyncActivityDto
    {
        public long? Id { get; set; }
        public string CalendarEventId { get; set; }
        public long? IntProviderAppSecretId { get; set; }
        public long? ActivityId { get; set; }
        public int? TenantId { get; set; }
        public long? ModuleId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}