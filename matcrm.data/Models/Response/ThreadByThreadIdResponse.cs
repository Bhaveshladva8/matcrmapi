using matcrm.data.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class ThreadByThreadIdResponse
    {
        public ThreadByThreadIdResponse()
        {
            MailComments = new List<MailCommentDto>();
            Participants = new List<MailParticipantDto>();
        }
        public Body BodyHtml { get; set; }
        public Body BodyPlain { get; set; }
        public int BodySize { get; set; }
        public UserContactResult Contact { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string Date { get; set; }
        public string From { get; set; }
        public string FromName { get; set; }
        public string FromEmail { get; set; }
        public long InternalDate { get; set; }
        public bool IsHasAttachment { get; set; }
        public bool IsOpen { get; set; }
        public bool IsRead { get; set; }
        public string[] LabelIds { get; set; }
        public string MessageId { get; set; }
        public string MimeType { get; set; }
        public int SizeEstimate { get; set; }
        public string Snippet { get; set; }
        public string Subject { get; set; }
        public string ThreadId { get; set; }
        public string To { get; set; }
        public string ToName { get; set; }
        public string ToEmail { get; set; }
        public List<MailCommentDto> MailComments { get; set; }
        public List<MailParticipantDto> Participants { get; set; }
        public long? CustomerId { get; set; }
        public CustomerDto customerDto { get; set; }
        public int? AssignUserId { get; set; }
        public List<Body> Attachments { get; set; }
        public Office365Body OfficeBodyHtml { get; set; }
        public string OfficeBodyPlain { get; set; }

    }
}
