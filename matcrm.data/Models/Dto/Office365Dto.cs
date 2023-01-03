using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace matcrm.data.Models.Dto
{
    public class Office365Dto
    {

    }

    public class Office365Token
    {
        public string token_type { get; set; }
        public string scope { get; set; }
        public int expires_in { get; set; }
        public int ext_expires_in { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
    }

    public class Office365Thread
    {
        public string odatacontext { get; set; }
        public string odatanextLink { get; set; }
        [JsonProperty("@odata.count")]
        public int count { get; set; }
        public Value[] value { get; set; }
    }

    // public class Value : CommonResponse
    public class Value : CommonResponse
    {
        public string odataetag { get; set; }
        public string id { get; set; }
        public string createdDateTime { get; set; }
        public string lastModifiedDateTime { get; set; }
        public string changeKey { get; set; }
        public object[] categories { get; set; }
        public string receivedDateTime { get; set; }
        public string sentDateTime { get; set; }
        public bool hasAttachments { get; set; }
        public string internetMessageId { get; set; }
        public string subject { get; set; }
        public string bodyPreview { get; set; }
        public string importance { get; set; }
        public string parentFolderId { get; set; }
        public string conversationId { get; set; }
        public bool? isDeliveryReceiptRequested { get; set; }
        public bool isReadReceiptRequested { get; set; }
        public bool isRead { get; set; }
        public bool isDraft { get; set; }
        public string webLink { get; set; }
        public string inferenceClassification { get; set; }
        public Office365Body body { get; set; }
        public Sender sender { get; set; }
        public From from { get; set; }
        public Torecipient[] toRecipients { get; set; }
        public Ccrecipient[] ccRecipients { get; set; }
        public Bccrecipient[] bccRecipients { get; set; }
        public Replyto[] replyTo { get; set; }
        public Flag flag { get; set; }
    }

    public class FileDto
    {
        public long FileId { get; set; }

        [StringLength(10)]
        public string InfoCode { get; set; }

        public long TableRecordId { get; set; }

        [StringLength(200)]
        public string FileName { get; set; }

        [StringLength(10)]
        public string Extention { get; set; }

        [StringLength(100)]
        public string FileType { get; set; }

        public long FileSize { get; set; }

        public string FilePath { get; set; }

        public bool IsPrivate { get; set; }

        public string Tag { get; set; }

        public long FileAlbumId { get; set; }

        public bool IsLibraryItem { get; set; }

        public long CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public Nullable<long> UpdatedBy { get; set; }

        public Nullable<DateTime> UpdatedOn { get; set; }

        public bool IsDeleted { get; set; }

        public Nullable<DateTime> DeletedOn { get; set; }

        public Nullable<long> CommunityId { get; set; }

        public Nullable<long> CommunityTypeId { get; set; }
    }

    public class Office365Body
    {
        public string contentType { get; set; }
        public string content { get; set; }
    }

    public class Sender
    {
        public Emailaddress emailAddress { get; set; }
    }

    public class Emailaddress
    {
        public string name { get; set; }
        public string address { get; set; }
    }

    public class From
    {
        public Emailaddress emailAddress { get; set; }
    }

    public class Flag
    {
        public string flagStatus { get; set; }
    }

    public class Torecipient
    {
        public Torecipient()
        {
            emailAddress = new Emailaddress();
        }
        public Emailaddress emailAddress { get; set; }
    }

    public class Replyto
    {
        public Emailaddress emailAddress { get; set; }
    }

    #region --- Create Message Class

    public class CreateMessage : CommonResponse
    {
        public Office365Message message { get; set; }
        public string saveToSentItems { get; set; }
        public string comment { get; set; }
    }

    public class CreateMailMessage
    {
        public Office365Message message { get; set; }
        public string saveToSentItems { get; set; }
        public string comment { get; set; }
    }

    public class Office365Message
    {
        public Office365Message()
        {
            attachments = new MailAttachments[] { };
            ccRecipients = new Ccrecipient[] { };
            toRecipients = new Torecipient[] { };
            bccRecipients = new Bccrecipient[] { };
            body = new Office365Body();
            subject = "";
        }
        public string subject { get; set; }
        public Office365Body body { get; set; }
        public Torecipient[] toRecipients { get; set; }
        public Ccrecipient[] ccRecipients { get; set; }
        public Bccrecipient[] bccRecipients { get; set; }
        public bool isDeliveryReceiptRequested { get; set; }
        public bool hasAttachments { get; set; }
        public MailAttachments[] attachments { get; set; }
    }

    public class Attachments : CommonResponse
    {
        public string odatatype { get; set; }       // = "#microsoft.graph.fileAttachment",
        public string contentBytes { get; set; }    // = contentBytes,
        public string contentType { get; set; }     // = contentType,
        //public string contentId { get; set; }       // = "thumbsUp",
        public string name { get; set; }            // = "thumbs-up.png"

    }

    public class MailAttachments
    {
        public string odatatype { get; set; }       // = "#microsoft.graph.fileAttachment",
        public string contentBytes { get; set; }    // = contentBytes,
        public string contentType { get; set; }     // = contentType,
        //public string contentId { get; set; }       // = "thumbsUp",
        public string name { get; set; }            // = "thumbs-up.png"

    }

    public class Ccrecipient
    {
        public Ccrecipient()
        {
            emailAddress = new Emailaddress();
        }
        public Emailaddress emailAddress { get; set; }
    }

    public class Bccrecipient
    {
        public Bccrecipient()
        {
            emailAddress = new Emailaddress();
        }
        public Emailaddress emailAddress { get; set; }
    }

    public class Office365Draft : CommonResponse
    {
        public Office365Draft()
        {
            attachments = new Attachments[] { };
            ccRecipients = new Ccrecipient[] { };
            toRecipients = new Torecipient[] { };
            body = new Office365Body();
        }
        public string subject { get; set; }
        public string importance { get; set; }
        public Office365Body body { get; set; }
        public Torecipient[] toRecipients { get; set; }
        public Ccrecipient[] ccRecipients { get; set; }
        public bool hasAttachments { get; set; }
        public Attachments[] attachments { get; set; }
    }

    #endregion


    public class Notifications
    {
        public string changeType { get; set; }
        public string notificationUrl { get; set; }
        public string resource { get; set; }
        public DateTime expirationDateTime { get; set; }
        public string clientState { get; set; }
    }


    //public class AttachmentFiles
    //{
    //    public string odatacontext { get; set; }
    //    public AttachmentValue[] value { get; set; }
    //}

    //public class AttachmentValue
    //{
    //    [JsonProperty("@odata.type")]
    //    public string odatatype { get; set; }
    //    public string id { get; set; }
    //    public DateTime lastModifiedDateTime { get; set; }
    //    public string name { get; set; }
    //    public string contentType { get; set; }
    //    public int size { get; set; }
    //    public bool isInline { get; set; }
    //    public string contentId { get; set; }
    //    public object contentLocation { get; set; }
    //    public string contentBytes { get; set; }
    //}


    public class Office365MailFolders
    {
        public List<MailFolders> value { get; set; }
    }

    public class MailFolders
    {
        public string id { get; set; }
        public string displayName { get; set; }
        public string parentFolderId { get; set; }
        public int childFolderCount { get; set; }
        public int unreadItemCount { get; set; }
        public int totalItemCount { get; set; }
    }

    public class MessageMove
    {
        public string destinationId { get; set; }
    }
}