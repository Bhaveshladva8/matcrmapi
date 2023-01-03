using System;

namespace matcrm.data.Models.Dto
{
    public class MailParticipantDto : CommonResponse
    {
        public long Id { get; set; }
        public string? ThreadId { get; set; }
        public long? IntProviderAppSecretId { get; set; }
        public int? TeamMemberId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}