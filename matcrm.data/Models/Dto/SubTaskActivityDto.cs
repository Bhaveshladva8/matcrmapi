using System;

namespace matcrm.data.Models.Dto {
    public class SubTaskActivityDto {
        public long? Id { get; set; }
        public long? SubTaskId { get; set; }
        public long? UserId { get; set; }
        public string Activity { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ShortName { get; set; }
    }
}