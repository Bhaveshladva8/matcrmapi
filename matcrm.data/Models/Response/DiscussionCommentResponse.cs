using Microsoft.AspNetCore.Http;
using matcrm.data.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class DiscussionCommentResponse
    {
        public DiscussionCommentResponse()
        {
            Attachments = new List<DiscussionCommentAttachmentDto>();
            FileList = new IFormFile[] { };
        }
        public IFormFile[] FileList { get; set; }
        public List<DiscussionCommentAttachmentDto> Attachments { get; set; }
        public long? Id { get; set; }
        public bool IsPinned { get; set; }
    }
}
