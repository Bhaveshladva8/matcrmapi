using System;

namespace matcrm.data.Models.Dto {
    public class ChildTaskActivityDto {
        public long? Id { get; set; }
        public long? ChildTaskId { get; set; }
        public long? UserId { get; set; }
        public string Activity { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ShortName { get; set; }
    }
}