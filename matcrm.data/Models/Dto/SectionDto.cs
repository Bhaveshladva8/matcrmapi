using System;

namespace matcrm.data.Models.Dto
{
    public class SectionDto
    {
        public long? Id { get; set; }
        public string Name { get; set; }
         public long? ProjectId { get; set; }
        public long? TenantId { get; set; }
        public long? TicketNumber { get; set; }
        public long? Priority { get; set; }
        public bool IsKeepTasks { get; set; }
        public long? CurrentPriority { get; set; }
        public long? PreviousPriority { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}