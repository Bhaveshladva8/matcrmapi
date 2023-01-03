using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeKit;

namespace matcrm.data.Models.Dto.CustomEmail
{
    public class CustomEmailInbox
    {
        public CustomEmailInbox(){
            From = new List<MailboxAddress>();
            To = new List<MailboxAddress>();
        }
        public string HtmlBody { get; set; }
        public string MessageId { get; set; }
        public DateTime Date { get; set; }
        public string Subject { get; set; }
        public string TextBody { get; set; }
        // public string To { get; set; }
        public bool IsAttachment { get; set; }
        public List<MailboxAddress> Mailboxes { get; set; }
        public string InReplyTo { get; set; }
        public string Priority { get; set; }
        public List<MailboxAddress> To { get; set; }
        public List<MailboxAddress> From { get; set; }
    }

    public class MailboxAddress
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class CustomEmailFolder
    {
        public CustomEmailFolder(){
            Messages = new List<CustomEmailInbox>();
        }
        public int? Id { get; set; }
        public string Name { get; set; }
        public long? Count { get; set; }
        public List<CustomEmailInbox> Messages { get; set; }
    }
}