using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Dto
{
    public class DiscussionCommentDto
    {
        public DiscussionCommentDto (){
            Attachments = new List<DiscussionCommentAttachmentDto>();
            FileList = new IFormFile[] {};
        }
        public long? Id { get; set; }
        public int? TeamMemberId { get; set; }
        public string? Comment { get; set; }
        public long? DiscussionId { get; set; }
        public int? CreatedBy { get; set; }
        public long? ReplyCommentId { get; set; }
        public string SortName { get; set; }
        public virtual DiscussionComment ReplyDiscussionComment { get; set; }
        public bool IsPinned { get; set; }
        public int? PinnedBy { get; set; }
        public virtual User PinnedUser { get; set; }
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
        public List<DiscussionCommentAttachmentDto> Attachments { get; set; }
    }
}