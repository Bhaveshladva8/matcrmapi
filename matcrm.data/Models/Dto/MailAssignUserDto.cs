using System;

namespace matcrm.data.Models.Dto
{
    public class MailAssignUserDto: CommonResponse
    {
        public long? Id { get; set; }
        public int? TeamMemberId { get; set; }
        public long? IntProviderAppSecretId { get; set; }
        public string? ThreadId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}