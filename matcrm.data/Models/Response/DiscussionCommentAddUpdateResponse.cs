using Microsoft.AspNetCore.Http;
using matcrm.data.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class DiscussionCommentAddUpdateResponse
    {
        public DiscussionCommentAddUpdateResponse()
        {
            Attachments = new List<DiscussionCommentAttachmentDto>();
            FileList = new IFormFile[] { };
        }
        public long? Id { get; set; }
        public List<DiscussionCommentAttachmentDto> Attachments { get; set; }
        public string? Comment { get; set; }        
        public long? DiscussionId { get; set; }
        public IFormFile[] FileList { get; set; }        
        public bool IsPinned { get; set; }
        public int? TeamMemberId { get; set; }
        public long? ReplyCommentId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
