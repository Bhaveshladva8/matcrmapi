using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class DiscussionCommentAddUpdateRequest
    {
        public DiscussionCommentAddUpdateRequest()
        {
            
            FileList = new IFormFile[] { };
        }
        public long? Id { get; set; }
        public long? ReplyCommentId { get; set; }
        public string? Comment { get; set; }
        public IFormFile[] FileList { get; set; }
        public long? MailReplyCommentId { get; set; }
        public long? DiscussionId { get; set; }
    }
}
