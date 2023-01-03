using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace matcrm.data.Models.Request
{
    public class MateCommentAddUpdateRequest
    {
        public MateCommentAddUpdateRequest()
        {
            //Attachments = new List<MateCommentAttachmentAddUpdateRequest>();
            FileList = new IFormFile[] { };
        }
        public long? Id { get; set; }
        public string? Comment { get; set; }
        public long? MateReplyCommentId { get; set; }
        public IFormFile[] FileList { get; set; }
        //public List<MateCommentAttachmentAddUpdateRequest> Attachments { get; set; }
        public long? TaskId { get; set; }
        public long? SubTaskId { get; set; }
        public long? ChildTaskId { get; set; }
        public long? MateTicketId { get; set; }
    }
    // public class MateCommentAttachmentAddUpdateRequest
    // {
    //     public long? Id { get; set; }
    //     public string? Name { get; set; }
    //     public long? MateCommentId { get; set; }
    // }
}