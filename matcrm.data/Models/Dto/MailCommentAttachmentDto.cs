using System;

namespace matcrm.data.Models.Dto
{
    public class MailCommentAttachmentDto
    {
        public long? Id { get; set; }
        public string? FileName { get; set; }
        public string? Description { get; set; }
        public long? MailCommentId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}