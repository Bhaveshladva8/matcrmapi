using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Dto
{
    public class MailCommentDto
    {
        public MailCommentDto(){
            Attachments = new List<MailCommentAttachmentDto>();
            FileList = new IFormFile[] {};
        }
        public long? Id { get; set; }
        public int? TeamMemberId { get; set; }
        public string? ThreadId { get; set; }
        public int? CreatedBy { get; set; }
        public string? Comment { get; set; }
        public string SortName { get; set; }
        public long? MailReplyCommentId { get; set; }
        public virtual MailComment ReplyMailComment { get; set; }
        public bool IsPinned { get; set; }
        public int? PinnedBy { get; set; }
        public virtual User PinnedUser { get; set; }
        public int? TenantId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ShortName { get; set; }
        public string Email { get; set; }
        public IFormFile[] FileList { get; set; }
        public List<MailCommentAttachmentDto> Attachments { get; set; }
    }
}