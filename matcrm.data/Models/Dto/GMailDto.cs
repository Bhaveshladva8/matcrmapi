using System;
using System.Collections.Generic;

namespace matcrm.data.Models.Dto
{
    public class GMailDto
    {

    }

    #region Thread List
    public class GmailThreads
    {
        public List<Thread> threads { get; set; }
        public string nextPageToken { get; set; }
        public int resultSizeEstimate { get; set; }
    }

    public class Thread
    {
        public string id { get; set; }
        public string snippet { get; set; }
        public string historyId { get; set; }
    }
    #endregion

    #region Thread Detail
    public class ThreadDetail
    {
        public string id { get; set; }
        public string historyId { get; set; }
        public List<Message> messages { get; set; }
    }

    public class DraftMessage
    {
        public DraftMessage()
        {
            message = new Message();
        }
        public string id { get; set; }
        public Message message { get; set; }
    }

    public class Message
    {
        public string id { get; set; }
        public string threadId { get; set; }
        public string[] labelIds { get; set; }
        public string snippet { get; set; }
        public string historyId { get; set; }
        public string internalDate { get; set; }
        public Payload payload { get; set; }
        public int sizeEstimate { get; set; }
        public string raw { get; set; }
    }

    public class Payload
    {
        public string partId { get; set; }
        public string mimeType { get; set; }
        public string filename { get; set; }
        public List<Header> headers { get; set; }
        public List<Part> parts { get; set; }
        public Body body { get; set; }
    }

    public class Header
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class Part
    {
        public string partId { get; set; }
        public string mimeType { get; set; }
        public string filename { get; set; }
        public List<BodyHeader> headers { get; set; }
        public Body body { get; set; }
        public List<Part> parts { get; set; }

        // private properties
        public Body plainBody { get; set; }
        public Body htmlBody { get; set; }
        public List<Body> attachments { get; set; }
    }

    public class Body
    {
        public int size { get; set; }
        public string attachmentId { get; set; }
        public string data { get; set; }

        // private properties
        public string mimeType { get; set; }
        public string contentType { get; set; }
        public string fileName { get; set; }
        public string extention { get; set; }
        public Byte[] bytes { get; set; }
        public string contentBytes { get; set; }
    }

    public class BodyHeader
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class BodyVM : CommonResponse
    //  public class BodyVM 
    {
        public string MessageId { get; set; }
        public Body Body { get; set; }
        public UserEmail UserEmail { get; set; }
    }
    #endregion

    #region Token
    public class Token
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string scope { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }
        public string id_token { get; set; }
    }
    #endregion

    #region Modify Message

    public class ModifyMessage
    {
        public List<string> addLabelIds { get; set; }
        public List<string> removeLabelIds { get; set; }
    }

    #endregion

    #region Gmail Labels

    public class ThreadLabels
    {
        public List<MailLabel> labels { get; set; }
    }

    public class MailLabel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string messageListVisibility { get; set; }
        public string labelListVisibility { get; set; }
    }

    // Thread Count
    public class LabelReadUnReadCount
    {
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int messagesTotal { get; set; }
        public int messagesUnread { get; set; }
        public int threadsTotal { get; set; }
        public int threadsUnread { get; set; }
    }


    #endregion
}