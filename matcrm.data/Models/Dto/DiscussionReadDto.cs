using System;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Dto
{
    public class DiscussionReadDto
    {
        public long? Id { get; set; }

        public long? DiscussionId { get; set; }
        public int? ReadBy { get; set; }
        public virtual User User { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}