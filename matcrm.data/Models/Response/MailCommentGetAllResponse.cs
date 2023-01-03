using Microsoft.AspNetCore.Http;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class MailCommentGetAllResponse
    {
        public MailCommentGetAllResponse()
        {
            Attachments = new List<MailCommentAttachmentDto>();
            FileList = new IFormFile[] { };
        }
        public long? Id { get; set; }
        public IFormFile[] FileList { get; set; }
        public List<MailCommentAttachmentDto> Attachments { get; set; }
        public string? Comment { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ShortName { get; set; }        
        public bool IsPinned { get; set; }        
        public int? TeamMemberId { get; set; }
        public string? ThreadId { get; set; }
        public long? MailReplyCommentId { get; set; }
        public virtual MailComment ReplyMailComment { get; set; }
        
    }
}
