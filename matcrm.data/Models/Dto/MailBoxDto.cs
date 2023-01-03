using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace matcrm.data.Models.Dto
{

    public class MailTokenDto : CommonResponse
    {
        public string code { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string grant_type { get; set; }
        public string redirect_uri { get; set; }
        public string access_token { get; set; }
        public long? expires_in { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
        public string token_type { get; set; }
        public string id_token { get; set; }
        public string error { get; set; }
        public string error_description { get; set; }
        public string eventId { get; set; }
        public string email { get; set; }
        // public int? userId { get; set; }
        // public string providerApp { get; set; }
        public string label { get; set; }
        public long? AppSecretId { get; set; }
        public string type { get; set; }
        public int? top { get; set; }
        public int? page { get; set; }
        public int? skip { get; set; }
        public string nextPageToken { get; set; }
        public long? teamInboxId { get; set; }
        public string? Color { get; set; }
    }

    public class MailBoxTokenVM
    {
        public string displayName { get; set; }
        public string surname { get; set; }
        public string givenName { get; set; }
        public string id { get; set; }
        public string userPrincipalName { get; set; }
        public string mail { get; set; }
    }
    // public class InboxThreads : CommonResponse
    public class InboxThreads
    {
        public int count { get; set; }
        public List<InboxThread> InboxThread { get; set; }
        public List<InboxThreadAndDiscussion> MailAndDiscussions { get; set; }
        public string NextPageToken { get; set; }
        public List<InboxLabel> Labels { get; set; }
        public List<NextPageToken> NextPageTokens { get; set; }
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        // public List<EmailAccount> EmailAccount { get; set; }
    }
    public class InboxThread
    {
        public string ThreadId { get; set; }
        // public string MessageId { get; set; }

        public string Subject { get; set; }

        public string FromName { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string Date { get; set; }

        public string Snippet { get; set; }

        public bool IsHasAttachment { get; set; }

        public long InternalDate { get; set; }

        public UserContactResult Contact { get; set; }

        public UserMemberResult Member { get; set; }

        public bool IsOpen { get; set; }

        public string FromEmail { get; set; }

        public string ToName { get; set; }

        public string ToEmail { get; set; }

        public string CcName { get; set; }

        public string CcEmail { get; set; }

        public string BccName { get; set; }

        public string BccEmail { get; set; }

        public string messageId { get; set; }

        public int AccountId { get; set; }

        public long UserEmailId { get; set; }

        public string AccountAlias { get; set; }

        public long MailAssignUserId { get; set; }
        public long? CustomerId { get; set; }

        public bool IsChecked { get; set; } = false;
        public bool IsRead { get; set; }
        public bool IsStarred { get; set; }
        public string Importance { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Body BodyHtml { get; set; }
    }

    public class InboxThreadAndDiscussion
    {
        public InboxThreadAndDiscussion()
        {
            FileList = new IFormFile[] { };
        }
        public string ThreadId { get; set; }

        public string Subject { get; set; }

        public string FromName { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string Date { get; set; }

        public string Snippet { get; set; }

        public bool IsHasAttachment { get; set; }

        public long InternalDate { get; set; }

        public UserContactResult Contact { get; set; }

        public UserMemberResult Member { get; set; }

        public bool IsOpen { get; set; }

        public string FromEmail { get; set; }

        public string ToName { get; set; }

        public string ToEmail { get; set; }

        public string CcName { get; set; }

        public string CcEmail { get; set; }

        public string BccName { get; set; }

        public string BccEmail { get; set; }

        public string messageId { get; set; }
        public long? MailAssignUserId { get; set; }
        public long? CustomerId { get; set; }

        public int AccountId { get; set; }

        public long UserEmailId { get; set; }

        public string AccountAlias { get; set; }

        public bool IsChecked { get; set; } = false;
        public bool IsRead { get; set; }
        public bool IsStarred { get; set; }
        public string Importance { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? Id { get; set; }
        public string? Topic { get; set; }
        public string? Note { get; set; }
        public int? AssignUserId { get; set; }
        public List<string>? ToDiscussion { get; set; }
        public List<int>? ToTeamMateIds { get; set; }
        public long? TeamInboxId { get; set; }
        public long? TeamId { get; set; }
        public bool IsArchived { get; set; }
        public bool IsPinned { get; set; }
        public int? PinnedBy { get; set; }
        public IFormFile[] FileList { get; set; }
        public int? CreatedBy { get; set; }
        public int? TenantId { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }

    // public class InboxThreadItem : CommonResponse
    public class InboxThreadItem
    {
        public InboxThreadItem()
        {
            MailComments = new List<MailCommentDto>();
            Participants = new List<MailParticipantDto>();
        }
        public string ThreadId { get; set; }

        public string Subject { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string Date { get; set; }

        public string Snippet { get; set; }

        public bool IsHasAttachment { get; set; }

        public string MessageId { get; set; }

        public string[] LabelIds { get; set; }

        public int SizeEstimate { get; set; }

        public string MimeType { get; set; }

        public string Filename { get; set; }

        public Body BodyPlain { get; set; }

        public Body BodyHtml { get; set; }

        public List<Body> Attachments { get; set; }

        public string OfficeBodyPlain { get; set; }

        public Office365Body OfficeBodyHtml { get; set; }

        public int BodySize { get; set; }

        public long InternalDate { get; set; }

        public UserContactResult Contact { get; set; }

        public UserMemberResult Member { get; set; }

        public string FromName { get; set; }

        public bool IsOpen { get; set; }
        public bool IsRead { get; set; }

        public string FromEmail { get; set; }

        public string ToName { get; set; }

        public string ToEmail { get; set; }

        public string CcName { get; set; }

        public string CcEmail { get; set; }

        public string BccName { get; set; }

        public string BccEmail { get; set; }
        public DateTime? CreatedOn { get; set; }

        public int? AssignUserId { get; set; }

        public AttachmentFiles AttachmentFiles { get; set; }
        public List<MailCommentDto> MailComments { get; set; }
        public List<MailParticipantDto> Participants { get; set; }
        public long? CustomerId { get; set; }
        public CustomerDto customerDto { get; set; }
    }

    public class GmailCompose
    {
        public string raw { get; set; }
        public string threadId { get; set; }
    }

    public class GmailSent
    {
        public string id { get; set; }
        public string threadId { get; set; }
        public string[] labelIds { get; set; }
    }

    public class ComposeEmail : CommonResponse
    // public class ComposeEmail
    {
        public ComposeEmail()
        {
            FileList = new IFormFile[] { };
        }
        public UserEmail? UserEmail { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> To { get; set; }
        public List<string> Cc { get; set; }
        public List<string> Bcc { get; set; }
        public GmailSent MailSent { get; set; }
        public GmailError GmailError { get; set; }
        public List<long> FileIds { get; set; }
        public IFormFile[] FileList { get; set; }
        public List<QuickEmailAttachments> QuickEmailAttachments { get; set; }
        public bool IsSendMassEmail { get; set; }
    }

    public class ComposeEmail1 : CommonResponse
    // public class ComposeEmail
    {
        public ComposeEmail1()
        {
            FileIds = new List<long>();
            To = new List<string>();
            Cc = new List<string>();
            Bcc = new List<string>();
            FileList = new IFormFile[] { };
        }
        // public UserEmail? UserEmail { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> To { get; set; }
        public List<string> Cc { get; set; }
        public List<string> Bcc { get; set; }
        public GmailSent MailSent { get; set; }
        public GmailError GmailError { get; set; }
        public List<long> FileIds { get; set; }
        public IFormFile[] FileList { get; set; }
        public string threadId { get; set; }
        public string id { get; set; }
        public string messageId { get; set; }
        public string mailType { get; set; }
        public List<QuickEmailAttachments> QuickEmailAttachments { get; set; }
        public bool IsSendMassEmail { get; set; } = false;
    }

    public class QuickEmailAttachments
    {
        public string FileName { get; set; }
        public string OrignalFileName { get; set; }
    }


    public class GmailError
    {
        public Error error { get; set; }
    }

    public class Error
    {
        public ErrorDetail[] errors { get; set; }
        public int code { get; set; }
        public string message { get; set; }
    }

    public class ErrorDetail
    {
        public string domain { get; set; }
        public string reason { get; set; }
        public string message { get; set; }
        public string locationType { get; set; }
        public string location { get; set; }
    }


    public class IsReadUnRead
    {
        public bool IsRead { get; set; }
    }

    public class MessageSent
    {
        public bool IsValid { get; set; } = true;

        public string ErrorMessage { get; set; } = "";
    }

    public class AttachmentFiles
    {
        public string odatacontext { get; set; }
        public AttachmentValue[] value { get; set; }
    }

    public class AttachmentValue
    {
        [JsonProperty("@odata.type")]
        public string odatatype { get; set; }
        public string id { get; set; }
        public DateTime lastModifiedDateTime { get; set; }
        public string name { get; set; }
        public string contentType { get; set; }
        public int size { get; set; }
        public bool isInline { get; set; }
        public string contentId { get; set; }
        public object contentLocation { get; set; }
        public string contentBytes { get; set; }
        public string extention { get; set; }
    }

    public class InboxVM : CommonResponse
    {
        public UserEmail UserEmail { get; set; }

        public int? Skip { get; set; }

        public int? Top { get; set; }

        public string NextPageToken { get; set; }

        public string Label { get; set; }

        public string Query { get; set; }

        public long UserEmailId { get; set; }

        public long? MailAssignUserId { get; set; }

        public List<int> Filterdata { get; set; }

        public List<NextPageToken> NextPageTokens { get; set; }
        public int? UserId { get; set; }
        public string Code { get; set; }
        public string EventType { get; set; }
        public long? TeamInboxId { get; set; }
    }

    public class InboxLabel
    {
        public string id { get; set; }
        public string name { get; set; }
        public int threadsTotal { get; set; }
        public int threadsUnread { get; set; }
    }

    public class NextPageToken
    {
        public long UserEmailId { get; set; }
        public string PageToken { get; set; }
    }

    public class IFileAttachment
    {
        // public File File { get; set; }
        public string FileName { get; set; }
        public string Extention { get; set; }
        public string FileSize { get; set; }
        public string FileType { get; set; }
        public string FileSizeFormated { get; set; }
        public string FileBase64 { get; set; }
    }

    public class ThreadOperationVM : CommonResponse
    {
        public List<string> ThreadId { get; set; }

        public string ThreadType { get; set; }

        public bool IsRead { get; set; }

        public UserEmail UserEmail { get; set; }
    }

    public class UserEmail : CommonResponse
    {
        // [Key, Required]
        public long UserEmailId { get; set; }

        public int UserId { get; set; }

        public int AccountId { get; set; }
        public bool IsRead { get; set; }

        [StringLength(200)]
        public string Email { get; set; }

        public string Access_Token { get; set; }

        public string Refresh_Token { get; set; }

        public string Code { get; set; }

        public string Scope { get; set; }

        public string ExpireIn { get; set; }

        public Nullable<bool> IsNotificationWatchEnable { get; set; } = true;

        public Nullable<DateTime> NotificationWatchExpireOn { get; set; }

        public Nullable<DateTime> ExpireOn { get; set; }

        public DateTime CreatedOn { get; set; }

        public Nullable<DateTime> LastUpdatedOn { get; set; }

        public bool IsDeleted { get; set; } = false;

        public Nullable<DateTime> DeletedOn { get; set; }

        public Nullable<bool> IsDefault { get; set; } = false;

        public string Alias { get; set; }
        public string Label { get; set; }
    }

    public class UserMemberResult
    {
        public long UserId { get; set; }

        public long TotalRecords { get; set; }

        public long MembersId { get; set; }

        public long MemberUserId { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string ProfilePicPath { get; set; }

        public string Thumbnail { get; set; }

        public long LinkNotificationId { get; set; }

        public string InfoCode { get; set; }

        public long TableRecordId { get; set; }

        public DateTime SentOn { get; set; }

        public long FromUserId { get; set; }

        public long ToUserId { get; set; }

        public int InviteStatusId { get; set; }

        public string InviteInfoCode { get; set; }

        public string InviteStatus { get; set; }

        public DateTime InvitedOn { get; set; }

        public Nullable<DateTime> LinkedOn { get; set; }

        public string ContactName { get; set; }

        public Nullable<long> ContactId { get; set; }

        public string ProfileSummary { get; set; }

        public bool IsBlock { get; set; }

        public long TotalCount { get; set; }
        public long UserSubscriptionLicenceId { get; set; }
    }


    //  public class Value : CommonResponse

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

    public class MessageConversation
    {
        public string odatacontext { get; set; }
        public List<Value> value { get; set; }
    }


}