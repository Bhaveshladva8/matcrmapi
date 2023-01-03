using Microsoft.AspNetCore.Http;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class MailCommentAddUpdateRequest
    {
        public MailCommentAddUpdateRequest(){
            Attachments = new List<MailCommentAttachmentDto>();
            FileList = new IFormFile[] {};
        }
        public long? Id { get; set; }
        public int? TeamMemberId { get; set; }
        public string? ThreadId { get; set; }        
        public string? Comment { get; set; }       
        public long? MailReplyCommentId { get; set; }
        //public long? DiscussionId { get; set; }
        public long? ReplyCommentId { get; set; }        
        public IFormFile[] FileList { get; set; }
        public List<MailCommentAttachmentDto> Attachments { get; set; }
    }
}
