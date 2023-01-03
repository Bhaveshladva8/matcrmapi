using System;

namespace matcrm.data.Models.Dto {
    public class SubTaskTimeRecordDto {
        public long? Id { get; set; }
        public long? UserId { get; set; }
        public long? Duration { get; set; }
        public long? SubTaskId { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}