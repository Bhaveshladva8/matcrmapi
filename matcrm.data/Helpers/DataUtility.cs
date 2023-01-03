using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using matcrm.data.Context;

namespace matcrm.data.Helpers
{
    public static class DataUtility
    {
        #region Helper Methods

        /// <summary>
        /// Function that creates an object from the given data row
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        public static T CreateItemFromRow<T>(DataRow row) where T : new()
        {
            // create a new object
            T item = new T();

            // set the item
            SetItemFromRow(item, row);

            // return 
            return item;
        }

        /// <summary>
        /// Function that set item from the given row
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="row"></param>
        public static void SetItemFromRow<T>(T item, DataRow row) where T : new()
        {
            // go through each column
            foreach (DataColumn c in row.Table.Columns)
            {
                // find the property for the column
                PropertyInfo p = item.GetType().GetProperty(c.ColumnName);

                // if exists, set the value
                if (p != null && row[c] != DBNull.Value)
                {
                    p.SetValue(item, row[c], null);
                }
            }
        }

        /// <summary>
        /// function that creates a list of an object from the given data table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tbl"></param>
        /// <returns></returns>
        public static List<T> CreateListFromTable<T>(DataTable tbl) where T : new()
        {
            // define return list
            List<T> lst = new List<T>();

            // go through each row
            foreach (DataRow r in tbl.Rows)
            {
                // add to the list
                lst.Add(CreateItemFromRow<T>(r));
            }

            // return the list
            return lst;
        }

        /// <summary>
        /// Check if table is exist in application
        /// </summary>
        /// <typeparam name="T">Class of data table to check</typeparam>
        /// <param name="db">DB Object</param>
        public static bool CheckTableExistsInApplication<T>(this OneClappContext db) where T : class
        {
            try
            {
                db.Set<T>().Count();
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Get SHA1 hash of given text
        /// </summary>
        public class ShaHash
        {
            public static String GetHash(string text)
            {
                // SHA512 is disposable by inheritance.  
                using (var sha256 = SHA256.Create())
                {
                    // Send a sample text to hash.  
                    var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
                    // Get the hashed string.  
                    return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                }
            }
        }

        /// <summary>
        /// This function copy source object property value to destination object property value
        /// This function copy value for only properties having same name
        /// </summary>
        /// <param name="sourceClassObject">Object of source class</param>
        /// <param name="destinationClassObject">Object of destination class</param>
        public static void CopyObject(object sourceClassObject, object destinationClassObject)
        {
            foreach (PropertyInfo property in sourceClassObject.GetType().GetProperties())
            {
                if (!property.CanRead || (property.GetIndexParameters().Length > 0))
                    continue;

                PropertyInfo propertyInfo = destinationClassObject.GetType().GetProperty(property.Name);
                if ((propertyInfo != null) && (propertyInfo.CanWrite))
                    propertyInfo.SetValue(destinationClassObject, property.GetValue(sourceClassObject, null), null);
            }
        }
        #endregion

        #region Constant
        // Company
        // public const string CompanyName = "beMate";

        public const string GoogleRecapchaVerifyUrl = "https://hcaptcha.com/siteverify?secret={0}&response={1}";
        public const string GoogleRecapchaSiteKey = "6c521222-f628-423e-bc15-ba766649f532";
        public const string GoogleRecapchaSiteSecret = "0xc4f821b4D1fEd53f8Da7d03b135C72c0EF34b017";

        //// Gmail API
        public const string GmailToken = "https://www.googleapis.com/oauth2/v4/token";
        public const string GmailThreads = "https://www.googleapis.com/gmail/v1/users/me/threads?{0}&maxResults={1}&pageToken={2}&q={3}";
        public const string GmailThreadItem = "https://www.googleapis.com/gmail/v1/users/me/threads/{0}";
        public const string GmailMessages = "https://www.googleapis.com/gmail/v1/users/me/messages";
        public const string GmailDraftMessageSend = "https://www.googleapis.com/gmail/v1/users/me/drafts/send";
        public const string GmailDraftList = "https://www.googleapis.com/gmail/v1/users/me/drafts";
        public const string GmailGetDraft = "https://www.googleapis.com/gmail/v1/users/me/drafts/{id}";
        public const string GmailMessageItem = "https://www.googleapis.com/gmail/v1/users/me/messages/{0}";
        public const string GmailModifyLabel = "https://www.googleapis.com/gmail/v1/users/me/messages/{0}/modify";
        public const string GmailMessageSend = "https://www.googleapis.com/gmail/v1/users/me/messages/send";
        public const string GmailAttachment = "https://www.googleapis.com/gmail/v1/users/me/messages/{0}/attachments/{1}";
        public const string GmailGetLabels = "https://www.googleapis.com/gmail/v1/users/me/labels";
        public const string GmailGetLabelCount = "https://www.googleapis.com/gmail/v1/users/me/labels/{0}";
        public const string GmailMessageTrash = "https://gmail.googleapis.com/gmail/v1/users/me/messages/{0}/trash";
        public const string GmailMessageUnTrash = "https://gmail.googleapis.com/gmail/v1/users/me/messages/{0}/untrash";

        //// Gmail System Label
        public const string GmailCATEGORY_PERSONAL = "CATEGORY_PERSONAL";
        public const string GmailCATEGORY_SOCIAL = "CATEGORY_SOCIAL";
        public const string GmailCATEGORY_UPDATES = "CATEGORY_UPDATES";
        public const string GmailCATEGORY_FORUMS = "CATEGORY_FORUMS";
        public const string GmailCHAT = "CHAT";
        public const string GmailSENT = "SENT";
        public const string GmailINBOX = "INBOX";
        public const string GmailTRASH = "TRASH";
        public const string GmailCATEGORY_PROMOTIONS = "CATEGORY_PROMOTIONS";
        public const string GmailDRAFT = "DRAFT";
        public const string GmailSPAM = "SPAM";
        public const string GmailSTARRED = "STARRED";
        public const string GmailUNREAD = "UNREAD";
        public const string GmailIMPORTANT = "IMPORTANT";
        public const string Email = "EMAIL";
        public const string Sms = "SMS";

        // Office 365 Credentials
        public static string Office365ClientId = "";//"ac915294-e037-4d25-abb7-d9fdf95a2068";
        public static string Office365ClientSecret = "";//"hZA041##borhngUYCGC10~|";

        //// Office 365
        public const string Office365Token = "https://login.microsoftonline.com/common/oauth2/v2.0/token";
        public const string Office365Threads = "https://graph.microsoft.com/v1.0/me/mailfolders/{0}/messages?$skip={1}&$top={2}&$select=subject,from,toRecipients,ccRecipients,bccRecipients,receivedDateTime,hasAttachments,conversationId,isRead,bodyPreview&$count=true&$conversationId&$orderby=receivedDateTime%20DESC";
        // public const string Office365Threads = "https://graph.microsoft.com/v1.0/me/mailfolders/{0}/messages?$skip={1}&$top={2}&$select=subject,from,toRecipients,ccRecipients,bccRecipients,receivedDateTime,hasAttachments,conversationId,isRead,bodyPreview&$count=true&$orderby=receivedDateTime DESC";
        public const string Office365AllThreads = "https://graph.microsoft.com/v1.0/me/messages?$skip={0}&$top={1}&$select=subject,from,toRecipients,ccRecipients,bccRecipients,receivedDateTime,hasAttachments,conversationId,isRead,bodyPreview&$count=true&$conversationId&$orderby=receivedDateTime%20DESC";
        public const string Office365ThreadsSearch = "https://graph.microsoft.com/v1.0/me/mailfolders/{0}/messages?$select=subject,from,toRecipients,ccRecipients,bccRecipients,receivedDateTime,hasAttachments,conversationId,isRead,bodyPreview&$count=true&$search={1}";
        public const string Office365ThreadItem = "https://graph.microsoft.com/v1.0/me/messages/{0}";
        public const string Office365ThreadByConversionId = "https://graph.microsoft.com/v1.0/me/messages?$filter=conversationId eq {0}";
        // public const string Office365ThreadByConversionId = "https://graph.microsoft.com/v1.0/me/messages/{0}";
        public const string Office365SendEmail = "https://graph.microsoft.com/v1.0/me/sendMail";
        public const string Office365CreateMessage = "https://graph.microsoft.com/v1.0/me/messages";
        public const string Office365DeleteEmail = "https://graph.microsoft.com/v1.0/me/messages/{0}";
        public const string Office365ForwardEmail = "https://graph.microsoft.com/v1.0/me/messages/{0}/createForward";
        public const string Office365ReplayEmail = "https://graph.microsoft.com/v1.0/me/messages/{0}/createReply";      // createReplyAll
        public const string Office365Notification = "https://graph.microsoft.com/v1.0/subscriptions";
        public const string Office365Attachment = "https://graph.microsoft.com/v1.0/me/messages/{0}/attachments";
        public const string Office365ReadUnread = "https://graph.microsoft.com/v1.0/me/messages/{0}";
        public const string Office365Folders = "https://graph.microsoft.com/v1.0/me/mailFolders";
        public const string Office365MoveEmail = "https://graph.microsoft.com/v1.0/me/messages/{0}/move"; // Post request for mail move one folder to another  { "destinationId": "JunkEmail" }

        //// Office 365 Label
        public const string Office365All = "AllItems";
        public const string Office365Inbox = "Inbox";
        public const string Office365SentItems = "SentItems";
        public const string Office365Drafts = "Drafts";
        public const string Office365DeletedItems = "DeletedItems";
        public const string Office365Outbox = "Outbox";
        public const string Office365Archive = "Archive";
        public const string Office365ConversationHistory = "ConversationHistory";
        public const string Office365JunkEmail = "JunkEmail";

        // List Infocode
        public const string ListContact = "CNTCT";
        public const string ListProperty = "PROTY";
        public const string ListActivity = "ACTVT";
        public const string ActivityInfoCode = "ACTY";
        public const string ListFile = "FILEL";
        public const string SaveFilterListFile = "FILES";
        public const string ListFeedUpload = "UPFIL";
        public const string ListFeedComment = "COMNT";
        public const string ListPreviewUrl = "PRURL";
        public const string ListShareFeed = "SHRFD";
        public const string ListUserTeam = "URTEM";
        public const string ListGroup = "GROUP";
        public const string ListPage = "PAGE";
        public const string ListTeam = "TEAM";
        public const string ListNetwork = "NETWORK";
        public const string InfoCodeLandingPage = "LDPGS";
        public const string ListProduct = "PRODT";
        public const string ListProductCategory = "PRTCT";
        public const string ListWebPage = "WBPAG";
        public const string MarketingPieceTemplate = "MPTMP";
        public const string MarketingPieceDigitalAdd = "DGADS";
        public const string MarketingPieceFlyer = "FLYER";
        public const string MarketingPiecePostCard = "PTCRD";

        // Image Extentions
        public const string ImageExtention = "'JPG','PNG','GIF','JPEG'";

        // Timeline Infocode
        public const string TimelineNote = "TLNOT";
        public const string TimelineActivity = "TLACT";
        public const string TimelineAppointment = "TLAPM";
        public const string TimelineLink = "TLLNK";
        public const string TimelineFile = "TLFIL";
        public const string TimelineAssignUser = "TLASU";
        public const string TimelineCallVerify = "TLCVF";
        public const string TimelineNewItemAdd = "TLIAD";
        public const string TimelineNotification = "TLNTF";

        // Link Notification
        public const string LinkNotificationGroup = "LNGRP";
        public const string LinkNotificationTeam = "LNTEM";
        public const string LinkNotificationPage = "LNPAG";
        public const string LinkNotificationMember = "LNMEM";
        public const string LinkNotificationActivity = "LNACT";
        public const string LinkNotificationLicense = "LNLIC";

        // MemberRequest
        public const string MemberRequestGroup = "LNGRP";
        public const string MemberRequestTeam = "LNTEM";
        public const string MemberRequestPage = "LNPAG";
        public const string MemberRequestLicense = "LNLIC";

        // License Action
        public const string LicenseActionRemove = "RM";
        public const string LicenseActionReAssign = "RA";
        // Azure URL
        public const string AzureFileUrl = "https://mrw.blob.core.windows.net/";

        //// Office365 Calendar API

        public const string AllMSCalendarEvents = "https://graph.microsoft.com/v1.0/me/events"; // Get
        public const string CreateMSCalendarEvent = "https://graph.microsoft.com/v1.0/me/events"; // Post
        public const string UpdateMSCalendarEvent = "https://graph.microsoft.com/v1.0/me/events/{0}"; // Patch // {0}=> eventid
        public const string GetMSCalendarEventById = "https://graph.microsoft.com/v1.0/me/events/{0}"; // Get
        public const string DeleteMSCalendarEventById = "https://graph.microsoft.com/v1.0/me/events/{0}"; // Delete

        public const string AllMSContact = "https://graph.microsoft.com/v1.0/me/contacts"; // Get all contact
        public const string GetUpdateDeleteMSContactById = "https://graph.microsoft.com/v1.0/me/contacts/{0}"; // Get all contact

        public const string AllGoogleContacts = "https://people.googleapis.com/v1/people/me/connections?personFields=names,emailAddresses"; // Get
        // Google calendar apis

        public const string AllGoogleCalendarEvents = "https://www.googleapis.com/calendar/v3/calendars/{0}/events";
        public const string GoogleCalendarUpdateEvent = "https://www.googleapis.com/calendar/v3/calendars/{0}/events/{1}";

        //MS Contacts apis
        public const string AllMSContactEvents = "https://graph.microsoft.com/v1.0/me/contacts"; // Get
        public const string CreateMSContactEvents = "https://graph.microsoft.com/v1.0/me/contacts";//create contact

        //Google contact 
         public const string CreateGoogleContactEvents = "https://people.googleapis.com/v1/people:createContact";//create contact

        //Notification
        public const string NotificationWelcome = "Thanks for signing up with us. Welcome to {0}!";
        #endregion

        public static class FileUploadSettings
        {
            public static string Container { get; set; }

            public static bool Property { get; set; }

            public static bool PropertyResized1 { get; set; }

            public static bool PropertyResized2 { get; set; }

            public static bool PropertyResized3 { get; set; }

            public static bool PropertyImage { get; set; }

            public static bool Contact { get; set; }

            public static bool FileUpload { get; set; }

            public static bool FileUploadResized { get; set; }

            public static bool Page { get; set; }

            public static bool Group { get; set; }

            public static bool Team { get; set; }

            public static bool FeatureAndBug { get; set; }

            public static bool UserProfile { get; set; }
        }

        #region Enum
        public enum VerificationCodeType
        {
            EmailChange = 1,
            SignupEmail = 2,
            PasswordReset = 3,
            SetPassword = 4,
            EmailVerify = 5
        }

        public enum NotificationType
        {
            Notification = 1
        }

        public enum CreditAmount
        {
            InviteUserSuccess = 10
        }

        public enum CreditableInvites
        {
            Value = 10
        }

        public enum OneclappRoles
        {
            Admin = 1,
            TenantAdmin,
            TenantManager,
            TenantUser,
            TenantExternalUser
        }

        public enum OneclappMasterTables
        {
            Customer = 1,
            Ticket,
            TaskStatus,
            User,
            Organization,
            Lead
        }

        public enum OneclappDefaultTenants
        {
            Admin = 1,
            TestIT
        }

        public enum OneClappLabelCategory
        {
            Customer = 1,
            Organization,
            Lead
        }

        #endregion

        #region Format Methods
        /// <summary>
        /// This function convert name into Title case letter
        /// </summary>
        /// <param name="str">Name</param>
        /// <returns></returns>
        public static string ToTitleCase(string str)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo.ToTitleCase(str.ToLower().Trim());
        }

        /// <summary>
        /// This function used to provide current date for the system.
        /// This is mostly used for CreatedOn field.
        /// </summary>
        /// <returns></returns>
        public static DateTime GetCurrentDateTime()
        {
            return System.DateTime.UtcNow;
        }

        /// <summary>
        /// This function used to provide system day of week.
        /// This is mostly used for CreatedOn field.
        /// </summary>
        /// <returns></returns>
        public static string GetSystemDayOfWeek()
        {
            return Convert.ToString(System.DateTime.UtcNow.DayOfWeek);
        }

        /// <summary>
        /// This function used to provide system MONTH
        /// This is mostly used for CreatedOn field.
        /// </summary>
        /// <returns></returns>
        public static string GetSystemMonth()
        {
            return Convert.ToString(System.DateTime.UtcNow.ToString("MMM"));
        }

        public static long ConvertDateTimeToUnixTimeStamp(DateTime datetime)
        {
            return ((DateTimeOffset)datetime).ToUnixTimeMilliseconds();
        }

        public static string Base64UrlEncode(string input)
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(inputBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }
        #endregion
    }
}