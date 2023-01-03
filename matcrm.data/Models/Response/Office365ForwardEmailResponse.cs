using matcrm.data.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class Office365ForwardEmailResponse
    {
        public bool IsValid { get; set; } = true;
        public string ErrorMessage { get; set; } = "";
        // public Bccrecipient[] bccRecipients { get; set; }
        // public Office365Body body { get; set; }
        // public string bodyPreview { get; set; }
        // public object[] categories { get; set; }
        // public Torecipient[] toRecipients { get; set; }
        // public Ccrecipient[] ccRecipients { get; set; }
        // public string changeKey { get; set; }
        // public string conversationId { get; set; }
        // public string createdDateTime { get; set; }
        
        // public Flag flag { get; set; }
        // public From from { get; set; }
        // public bool hasAttachments { get; set; }
        // public string id { get; set; }
        // public string importance { get; set; }
        // public string inferenceClassification { get; set; }
        // public string internetMessageId { get; set; }
        // public bool? isDeliveryReceiptRequested { get; set; }
        // public bool isRead { get; set; }
        // public bool isDraft { get; set; }
        // public bool isReadReceiptRequested { get; set; }
        
        // public string lastModifiedDateTime { get; set; }
        // public string parentFolderId { get; set; }
        // public string receivedDateTime { get; set; }
        // public Replyto[] replyTo { get; set; }
        // public Sender sender { get; set; }
        // public string sentDateTime { get; set; }
        // public string subject { get; set; }

        // public string webLink { get; set; }
        // public int UserId { get; set; }
    }
}
