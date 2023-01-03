using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace matcrm.data.Models.Response
{
    public class MateCommentGetAllResponse
    {
        public MateCommentGetAllResponse()
        {
            Attachments = new List<MateCommentAttachmentGetAllResponse>();
            //FileList = new IFormFile[] { };
        }
        public long Id { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public DateTime? CreatedOn { get; set; }      
        public string Comment { get; set; }        
        public long? MateReplyCommentId { get; set; }        
        public List<MateCommentAttachmentGetAllResponse> Attachments { get; set; }
    }
    public class MateCommentAttachmentGetAllResponse
    {
        public long Id { get; set; }        
        public string? Name { get; set; }
        public string? URL { get; set; }
    }
}