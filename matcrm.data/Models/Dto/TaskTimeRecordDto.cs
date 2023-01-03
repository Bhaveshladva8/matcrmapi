using System;

namespace matcrm.data.Models.Dto {
    public class TaskTimeRecordDto {
        public long? Id { get; set; }
        public long? UserId { get; set; }
        public long? Duration { get; set; }
        public long? TaskId { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}