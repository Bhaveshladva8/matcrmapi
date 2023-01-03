using System;

namespace matcrm.data.Models.Dto
{
    public class DiscussionParticipantDto
    {
         public long? Id { get; set; }

        public long? DiscussionId { get; set; }

        public int? TeamMemberId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}