using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using matcrm.data.Context;
using matcrm.data.Helpers;
using matcrm.data.Models.Dto;

namespace matcrm.service.Utility
{
    public static class MailBoxManager<T>
    {
        //// Office 365
        public const string Office365Token = "https://login.microsoftonline.com/common/oauth2/v2.0/token";
        public const string Office365Threads = "https://graph.microsoft.com/v1.0/me/mailfolders/{0}/messages?$skip={1}&$top={2}&$select=subject,from,toRecipients,ccRecipients,bccRecipients,receivedDateTime,hasAttachments,conversationId,isRead,bodyPreview&$count=true&$conversationId&$orderby=receivedDateTime%20DESC";
        public const string Office365AllThreads = "https://graph.microsoft.com/v1.0/me/messages?$skip={0}&$top={1}&$select=subject,from,toRecipients,ccRecipients,bccRecipients,receivedDateTime,hasAttachments,conversationId,isRead,bodyPreview&$count=true&$conversationId&$orderby=receivedDateTime%20DESC";
        public const string Office365ThreadsSearch = "https://graph.microsoft.com/v1.0/me/mailfolders/{0}/messages?$select=subject,from,toRecipients,ccRecipients,bccRecipients,receivedDateTime,hasAttachments,conversationId,isRead,bodyPreview&$count=true&$search={1}";
        public const string Office365ThreadItem = "https://graph.microsoft.com/v1.0/me/messages/{0}";
        public const string Office365ThreadByConversionId = "https://graph.microsoft.com/v1.0/me/messages?$filter=conversationId%20eq%20%27{0}%27";
        public const string Office365SendEmail = "https://graph.microsoft.com/v1.0/me/sendMail";
        public const string Office365CreateMessage = "https://graph.microsoft.com/v1.0/me/messages";
        public const string Office365DeleteEmail = "https://graph.microsoft.com/v1.0/me/messages/{0}";
        public const string Office365ForwardEmail = "https://graph.microsoft.com/v1.0/me/messages/{0}/createForward";
        public const string Office365ReplayEmail = "https://graph.microsoft.com/v1.0/me/messages/{0}/createReply";      // createReplyAll
        public const string Office365Notification = "https://graph.microsoft.com/v1.0/subscriptions";
        public const string Office365Attachment = "https://graph.microsoft.com/beta/me/messages/{0}/attachments";
        public const string Office365ReadUnread = "https://graph.microsoft.com/v1.0/me/messages/{0}";
        public const string Office365Folders = "https://graph.microsoft.com/v1.0/me/mailFolders";

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

        private static readonly HttpClient httpClient = new HttpClient();
        // private InboxThreads inboxThreads = new InboxThreads();
        // private List<NextPageToken> NextPageToken = new List<NextPageToken>();
        // private List<InboxThreadItem> inboxThreadItems = new List<InboxThreadItem>();

        // #region 'Set Function'
        // public async Task<ManageMyAccountVM.UserEmailVM> AuthenticationComplete(long userId, ManageMyAccountVM.UserEmailVM model)
        // {
        //     data.Models.UserEmail userEmail = new data.Models.UserEmail();
        //     user = _userService.GetUserByUserId(userId);
        //     if (user == null)
        //     {
        //         model.IsValid = false;
        //         model.ErrorMessage = CommonMessage.UnAuthorizedUser;
        //     }

        //     userEmail.UserId = userId;
        //     userEmail.Email = model.Email;
        //     userEmail.Code = model.Code;
        //     userEmail.AccountId = model.AccountId;
        //     userEmail.CreatedOn = Utility.GetCurrentDateTime();
        //     userEmail = _userEmailService.CheckInsertOrUpdate(userEmail);
        //     model.UserEmailId = userEmail.UserEmailId;

        //     if (userEmail == null || userEmail.UserEmailId <= 0)
        //     {
        //         model.IsValid = false;
        //         model.ErrorMessage = CommonMessage.SomethingWentWrong;
        //         return model;
        //     }

        //     switch (model.AccountId)
        //     {
        //         case DataUtility.Gmail:
        //             var isTokenReceived = await GetGmailToken(userEmail, model.RedirectUri);
        //             if (!isTokenReceived)
        //             {
        //                 model.IsTokenReceived = false;
        //                 model.IsValid = false;
        //                 model.ErrorMessage = CommonMessage.ErrorOccuredInTokenGet;
        //                 return model;
        //             }

        //             break;

        //         case DataUtility.Office365:
        //         case DataUtility.Outlook:
        //             var isToken365Received = await GetOffice365Token(userEmail, model.RedirectUri);
        //             if (!isToken365Received)
        //             {
        //                 model.IsTokenReceived = false;
        //                 model.IsValid = false;
        //                 model.ErrorMessage = CommonMessage.ErrorOccuredInTokenGet;
        //                 return model;
        //             }

        //             break;
        //     }

        //     model.IsValid = true;
        //     model.ErrorMessage = CommonMessage.AuthenticationSuccess;
        //     return model;
        // }

        public static async Task<string> PostAcccessTokenAsync(string api, MailTokenDto model)
        {
            string mainUrl = "https://login.microsoftonline.com/common/oauth2/v2.0/token";

            using (HttpClient httpClient = new HttpClient())
            {
                // client.BaseAddress = new Uri (mainUrl);
                // client.DefaultRequestHeaders.Accept.Clear ();
                // client.DefaultRequestHeaders.Accept.Add (new MediaTypeWithQualityHeaderValue ("application/x-www-form-urlencoded"));
                // if (!string.IsNullOrEmpty (apiKey))
                //     client.DefaultRequestHeaders.Add ("AuthenticationToken", apiKey);

                var dict = new Dictionary<string, string>();
                dict.Add("client_id", OneClappContext.MicroSoftClientId);
                dict.Add("client_secret", OneClappContext.MicroSecretKey);
                dict.Add("grant_type", model.grant_type);
                dict.Add("redirect_uri", model.redirect_uri);
                if (model.type == "Refresh_Token")
                {
                    dict.Add("refresh_token", model.refresh_token);
                }
                else
                {
                    dict.Add("code", model.code);
                }
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, Office365Token) { Content = new FormUrlEncodedContent(dict) };

                HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(request);

                // HttpRequestMessage request = new HttpRequestMessage (HttpMethod.Post, api);
                request.Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/x-www-form-urlencoded"); //CONTENT-TYPE header

                // HttpResponseMessage responseMessage = await client.SendAsync (request);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var responseData = httpResponseMessage.Content.ReadAsStringAsync();
                    return responseData.Result;
                }
                else
                {
                    var responseData = httpResponseMessage.Content.ReadAsStringAsync();
                    return responseData.Result;
                }
            }
        }

        public static async Task<string> GetAsync(string api, T model, string apiKey, string token)
        {
            string mainUrl = "https://graph.microsoft.com/v1.0/me";

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(mainUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (!string.IsNullOrEmpty(apiKey))
                    // client.DefaultRequestHeaders.Add ("AuthenticationToken", apiKey);
                    httpClient.DefaultRequestHeaders.Add("Authorization", token);

                HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(api);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var responseData = httpResponseMessage.Content.ReadAsStringAsync();
                    return responseData.Result;
                }
                return string.Empty;
            }
        }

        // public async Task<InboxThreads> GetThread(long userId, InboxVM model, string token)
        // {
        //     // user = _userService.GetUserByUserId(userId);
        //     // if (user == null)
        //     // {
        //     //     inboxThreads.IsValid = false;
        //     //     inboxThreads.ErrorMessage = CommonMessage.UnAuthorizedUser;
        //     //     return inboxThreads;
        //     // }

        //     var task = new List<Task>();
        //     // var emailAccount = _emailAccountService.GetEmailAccountById(model.UserEmail.AccountId);
        //     // userEmail = _userEmailService.GetUserEmailById(model.UserEmail.UserEmailId);
        //     var name = "Outlook";
        //     HttpClient client = new HttpClient();
        //     switch (name)
        //     {
        //         case "Gmail":
        //             // await SetGmailToken();
        //             // client.DefaultRequestHeaders.Clear();
        //             // client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
        //             // GmailThreads gmailThreads = null;
        //             // //var gmailResponse = await client.GetAsync(string.Format(DataUtility.GmailThreads, LabelBuilder(model.Label), 30, model.NextPageToken, model.Query));
        //             // var gmailResponse = await client.GetAsync(string.Format(SystemSettingService.Dictionary["GMTRD"], LabelBuilder(model.Label), 30, model.NextPageToken, model.Query));
        //             // if (gmailResponse.StatusCode == HttpStatusCode.OK)
        //             // {
        //             //     var stream = await gmailResponse.Content.ReadAsStreamAsync();
        //             //     var serializer = new DataContractJsonSerializer(typeof(GmailThreads));
        //             //     gmailThreads = (GmailThreads)serializer.ReadObject(stream);
        //             // }

        //             // if (gmailThreads != null && gmailThreads.threads != null && gmailThreads.threads.Count > 0)
        //             // {
        //             //     model.UserEmail = userEmail;

        //             //     inboxThreads.InboxThread = new List<InboxThread>();
        //             //     inboxThreads.NextPageToken = gmailThreads.nextPageToken;
        //             //     inboxThreads.count = gmailThreads.resultSizeEstimate;
        //             //     foreach (var thread in gmailThreads.threads)
        //             //         task.Add(SetInboxThreadDetail(thread.id, model));
        //             //     //await SetInboxThreadDetail(thread.id);

        //             //     Task.WaitAll(task.ToArray());
        //             // }

        //             //await GetLabelWithUnReadCount(userId, model);

        //             break;

        //         case "Office 365":
        //         case "Outlook":

        //             client.DefaultRequestHeaders.Clear();
        //             client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

        //             Office365Thread office365Threads = null;
        //             string office365Uri = "";

        //             // All
        //             //if (model.Label.ToLower() == "all") office365Uri = string.Format(DataUtility.Office365AllThreads, model.Skip, model.Top);
        //             // Label
        //             if (model.Label.ToLower() == "all") model.Label = Office365All;// DataUtility.Office365All;
        //             else if (model.Label.ToLower() == "inbox") model.Label = Office365Inbox;// DataUtility.Office365Inbox;
        //             else if (model.Label.ToLower() == "sent") model.Label = Office365SentItems; //DataUtility.Office365SentItems;
        //             else if (model.Label.ToLower() == "draft") model.Label = Office365Drafts; // DataUtility.Office365Drafts;
        //             else if (model.Label.ToLower() == "trash") model.Label = Office365DeletedItems;      // DataUtility.Office365DeletedItems;
        //             else if (model.Label.ToLower() == "spam") model.Label = Office365JunkEmail;        // DataUtility.Office365JunkEmail;
        //             office365Uri = string.Format(Office365Threads, model.Label.Replace(" ", ""), model.Skip, model.Top); //DataUtility.Office365Threads

        //             if (model.Query != "" && model.Query != null) { office365Uri = string.Format(Office365ThreadsSearch, model.Label.Replace(" ", ""), model.Query); } //DataUtility.Office365ThreadsSearch

        //             var office365Response = await client.GetAsync(office365Uri);
        //             if (office365Response.StatusCode == HttpStatusCode.OK)
        //             {
        //                 //var stream = await office365Response.Content.ReadAsStreamAsync();
        //                 //var serializer = new DataContractJsonSerializer(typeof(Office365Thread));
        //                 //office365Threads = (Office365Thread)serializer.ReadObject(stream);

        //                 var json = await office365Response.Content.ReadAsStringAsync();
        //                 office365Threads = JsonConvert.DeserializeObject<Office365Thread>(json.ToString());
        //                 // Total Count of Email
        //                 inboxThreads.count = office365Threads.count;

        //                 //var json = await client.GetStringAsync(label == "All" ? string.Format(office365Uri, skip, top) : string.Format(office365Uri, label, skip, top));
        //                 //var odata = JsonConvert.DeserializeObject<OData>(json);
        //                 //office365Threads.count = odata.count;

        //             }

        //             if (office365Threads != null && office365Threads.value.Length > 0)
        //             {
        //                 inboxThreads.InboxThread = new List<InboxThread>();
        //                 // model.UserEmail = userEmail;
        //                 // foreach (var thread in office365Threads.value)
        //                 // {
        //                 //     task.Add(SetOffice365InboxThreadDetail(thread, model));
        //                 // }

        //                 Task.WaitAll(task.ToArray());
        //                 inboxThreads.count = office365Threads.count;


        //                 // if (inboxThreads.InboxThread != null)
        //                 // {
        //                 //     // Mark email as read if it is unread
        //                 //     if (inboxThreads.InboxThread.FirstOrDefault() != null) MarkOffice365EmailAsReadUnread(inboxThreads.InboxThread.FirstOrDefault().messageId, false);
        //                 // }
        //             }

        //             //await GetLabelWithUnReadCount(userId, model);

        //             break;

        //         default:
        //             break;
        //     }

        //     // if (inboxThreads.InboxThread != null && inboxThreads.InboxThread.Count > 0)
        //     // {
        //     //     inboxThreads.InboxThread = inboxThreads.InboxThread.OrderByDescending(x => x.InternalDate).ToList();
        //     //     task.Add(SetContactDetailInThreadEmail(model.Label));
        //     //     task.Add(SetNetworkMemberDetailInThreadEmail());
        //     // }
        //     Task.WaitAll(task.ToArray());
        //     return inboxThreads;
        // }

        // string strOffice365ThreadId = "";
        // private async Task<bool> SetOffice365InboxThreadDetail(Value thread, InboxVM model)
        // {
        //     if (strOffice365ThreadId != thread.conversationId)
        //     {
        //         strOffice365ThreadId = thread.conversationId;

        //         InboxThread inboxThread = new InboxThread();
        //         inboxThread.ThreadId = thread.conversationId;
        //         inboxThread.messageId = thread.id;
        //         inboxThread.Subject = thread.subject != null ? thread.subject : "";
        //         inboxThread.FromName = thread.from != null ? thread.from.emailAddress.name.ToString() : "";
        //         inboxThread.From = thread.from != null ? thread.from.emailAddress.address.ToString() : "";
        //         inboxThread.To = thread.toRecipients.Length > 0 ? thread.toRecipients[0].emailAddress.address.ToString() : "";
        //         inboxThread.ToEmail = thread.toRecipients.Length > 0 ? thread.toRecipients[0].emailAddress.address : null;
        //         inboxThread.FromEmail = thread.from != null ? thread.from.emailAddress.address : "";

        //         // To Email
        //         string toEmails = "";
        //         string toNames = "";
        //         int i = 0;
        //         if (thread.toRecipients.Length > 0)
        //         {
        //             foreach (var item in thread.toRecipients)
        //             {
        //                 if (i == 0) { toEmails = item.emailAddress.address; toNames = item.emailAddress.name; }
        //                 else { toEmails += ", " + item.emailAddress.address; toNames += ", " + item.emailAddress.name; }
        //                 i++;
        //             }
        //         }
        //         inboxThread.To = thread.toRecipients.Length > 0 ? toEmails : null;
        //         inboxThread.ToEmail = thread.toRecipients.Length > 0 ? toEmails : null;
        //         inboxThread.ToName = thread.toRecipients.Length > 0 ? toNames : null;

        //         // Cc Email
        //         string ccEmails = "";
        //         string ccNames = "";
        //         i = 0;
        //         if (thread.ccRecipients.Length > 0)
        //         {
        //             foreach (var item in thread.ccRecipients)
        //             {
        //                 if (i == 0) { ccEmails = item.emailAddress.address; ccNames = item.emailAddress.name; }
        //                 else { ccEmails += ", " + item.emailAddress.address; ccNames += ", " + item.emailAddress.name; }
        //                 i++;
        //             }
        //         }
        //         inboxThread.CcEmail = thread.ccRecipients.Length > 0 ? ccEmails : null;
        //         inboxThread.CcName = thread.ccRecipients.Length > 0 ? ccNames : null;

        //         // Bcc Email
        //         string bccEmails = "";
        //         string bccNames = "";
        //         i = 0;
        //         if (thread.bccRecipients.Length > 0)
        //         {
        //             foreach (var item in thread.bccRecipients)
        //             {
        //                 if (i == 0) { bccEmails = item.emailAddress.address; bccNames = item.emailAddress.name; }
        //                 else { bccEmails += ", " + item.emailAddress.address; bccNames += ", " + item.emailAddress.name; }
        //                 i++;
        //             }
        //         }
        //         inboxThread.BccEmail = thread.bccRecipients.Length > 0 ? ccEmails : null;
        //         inboxThread.BccName = thread.bccRecipients.Length > 0 ? ccNames : null;

        //         inboxThread.Date = thread.receivedDateTime != null ? thread.receivedDateTime : "";
        //         inboxThread.IsHasAttachment = thread.hasAttachments;
        //         inboxThread.InternalDate = DataUtility.ConvertDateTimeToUnixTimeStamp(Convert.ToDateTime(thread.receivedDateTime));
        //         inboxThread.IsOpen = thread.isRead;
        //         inboxThread.Snippet = thread.bodyPreview;
        //         inboxThread.AccountId = model.UserEmail != null ? model.UserEmail.AccountId : 0;
        //         inboxThread.UserEmailId = model.UserEmail != null ? model.UserEmail.UserEmailId : 0;
        //         inboxThread.AccountAlias = model.UserEmail != null ? model.UserEmail.Alias : "";
        //         inboxThreads.InboxThread.Add(inboxThread);

        //     }
        //     return true;
        // }

    }
}
