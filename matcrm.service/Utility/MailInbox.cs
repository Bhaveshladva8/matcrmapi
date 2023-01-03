using AutoMapper;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
// using Microsoft.Graph;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using matcrm.data.Context;
using matcrm.data.Helpers;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Dto.CustomEmail;
using matcrm.data.Models.Tables;
using matcrm.service.BusinessLogic;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.service.Services.ERP;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Sockets;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using static matcrm.data.Helpers.DataUtility;

namespace matcrm.service.Utility
{
    public class MailInbox
    {
        #region 'Service Object'
        private readonly OneClappContext _context;
        private readonly IUserService _userService;
        private readonly IIntProviderAppService _intProviderAppService;
        private readonly IIntProviderAppSecretService _intProviderAppSecretService;
        private readonly ICustomerAttachmentService _customerAttachmentService;
        private readonly ICustomDomainEmailConfigService _customDomainEmailConfigService;
        private readonly ITeamInboxService _teamInboxService;
        private readonly IGoogleCalendarService _calendarService;
        private readonly IHostingEnvironment _hostingEnvironment;
        #endregion

        #region 'Global Initialization'
        private static readonly HttpClient client = new HttpClient();
        private InboxThreads inboxThreads = new InboxThreads();
        private IntProviderAppSecret intProviderAppSecretObj = new IntProviderAppSecret();
        private List<NextPageToken> NextPageToken = new List<NextPageToken>();
        private List<InboxThreadItem> inboxThreadItems = new List<InboxThreadItem>();
        private string token;
        private User user;
        private FileUpload fileUpload;
        private MailTokenDto mailTokenDto = new MailTokenDto();
        private IMapper _mapper;
        private string MicrosoftScope;
        #endregion

        #region 'Constructor'
        public MailInbox(OneClappContext context, IUserService userService,
        IHostingEnvironment hostingEnvironment,
         IIntProviderAppService intProviderAppService,
         IIntProviderAppSecretService intProviderAppSecretService,
         ICustomerAttachmentService customerAttachmentService,
         IGoogleCalendarService calendarService,
         ITeamInboxService teamInboxService,
         ICustomDomainEmailConfigService customDomainEmailConfigService,
         IMapper mapper)
        {
            _userService = userService;
            _hostingEnvironment = hostingEnvironment;
            _intProviderAppService = intProviderAppService;
            _intProviderAppSecretService = intProviderAppSecretService;
            _customerAttachmentService = customerAttachmentService;
            _calendarService = calendarService;
            _teamInboxService = teamInboxService;
            _customDomainEmailConfigService = customDomainEmailConfigService;
            _mapper = mapper;
            fileUpload = new FileUpload(hostingEnvironment, customerAttachmentService);
            // System settings
            // systemSettingService.GetSystemSettingListByCodes(("GMTOK,GMTRD,GMTRI,GMLMS,GMMSI,GMLML,GMLSM,GMLGL,GMGLC,GMATH,GMLST,GMUNR,GMCID,GMCSC,OFCID,OFCSC,OFTOK,OFTRD,OFATR,OFTDS,OFTDI,OFTBI,OFSDM,OFCRM,OFDLE,OFFDE,OFRPE,OFATH,OFUNR,OFFOL,OFALL,OFINB,OFSNT,OFDRF,OFDLI,OFJKE,AZFLS").Split(","));
            //MicrosoftScope = "offline_access Mail.ReadWrite Mail.Send User.Read MailboxSettings.Read Calendars.Read Calendars.ReadWrite Calendars.Read.Shared Contacts.Read";
            MicrosoftScope = OneClappContext.MicrosoftUserScopes;
        }
        #endregion

        #region 'Get Function'
        public async Task<InboxThreads> GetThread(int userId, InboxVM model)
        {

            var user = _userService.GetUserById(userId);
            if (user == null)
            {
                inboxThreads.IsValid = false;
                inboxThreads.ErrorMessage = CommonMessage.UnAuthorizedUser;
                // inboxThreads.ErrorMessage = "UnAuthorize";
                return inboxThreads;
            }
            if (string.IsNullOrEmpty(model.Provider) && string.IsNullOrEmpty(model.ProviderApp))
            {
                var customEmailDomainObj = _customDomainEmailConfigService.GetByUserAndEmail(userId, model.SelectedEmail);
                if (customEmailDomainObj != null)
                {
                    inboxThreads.InboxThread = new List<InboxThread>();
                    using (var client = new ImapClient())
                    {
                        var options = SecureSocketOptions.StartTlsWhenAvailable;
                        options = SecureSocketOptions.SslOnConnect;

                        #region Host, Port, UserName, Passwrod with different host

                        string Host = customEmailDomainObj.IMAPHost;
                        int Port = customEmailDomainObj.IMAPPort.Value;
                        string UserName = customEmailDomainObj.Email;
                        string Password = customEmailDomainObj.Password;

                        #endregion
                        //client.Timeout = Convert.ToInt16(TimeSpan.FromMinutes(10));
                        client.CheckCertificateRevocation = false;
                        client.ServerCertificateValidationCallback = (s, c, ch, e) => true;

                        client.Connect(Host, Port, options);

                        client.Authenticate(UserName, Password);

                        //Check IsConnected condition
                        if (client.IsConnected)
                        {
                            // The Inbox folder is always available on all IMAP servers...

                            var personal = client.GetFolder(client.PersonalNamespaces[0]);
                            var folderList = personal.GetSubfolders(false);
                            var isExist = folderList.Where(t => t.Name.ToLower().Contains(model.Label.ToLower())).FirstOrDefault();

                            var folder1 = client.GetFolder(isExist.Name);
                            folder1.Open(FolderAccess.ReadOnly);

                            IList<UniqueId> seenUids = null;
                            IList<UniqueId> notseenUids = null;
                            seenUids = folder1.Search(MailKit.Search.SearchQuery.Seen);
                            notseenUids = folder1.Search(MailKit.Search.SearchQuery.NotSeen);

                            foreach (var seenUuId in seenUids)
                            {
                                var message = folder1.GetMessage(seenUuId);

                                var inboxThreadItem = new InboxThread();
                                if (message.MessageId != null)
                                {
                                    inboxThreadItem.ThreadId = message.MessageId;
                                    inboxThreadItem.messageId = message.MessageId;
                                    inboxThreadItem.Subject = message.Subject;
                                    inboxThreadItem.BodyHtml = new Body();
                                    inboxThreadItem.BodyHtml.contentType = message.HtmlBody;
                                    DateTime messageDate = message.Date.UtcDateTime;
                                    inboxThreadItem.Date = messageDate.ToString("yyyy-MM-dd");
                                    inboxThreadItem.CreatedOn = messageDate;
                                    foreach (var item in message.From.Mailboxes)
                                    {
                                        MailboxAddress fromObj = new MailboxAddress();
                                        fromObj.Address = item.Address;
                                        fromObj.Name = item.Name;
                                        if (string.IsNullOrEmpty(inboxThreadItem.From))
                                        {
                                            inboxThreadItem.From = fromObj.Address;
                                            inboxThreadItem.FromName = fromObj.Name;
                                        }
                                        else
                                        {
                                            inboxThreadItem.From = inboxThreadItem.From + ", " + fromObj.Address;
                                        }
                                    }
                                    foreach (var item in message.To.Mailboxes)
                                    {
                                        MailboxAddress toObj = new MailboxAddress();
                                        toObj.Address = item.Address;
                                        toObj.Name = item.Name;
                                        if (string.IsNullOrEmpty(inboxThreadItem.To))
                                        {
                                            inboxThreadItem.To = toObj.Address;
                                            inboxThreadItem.ToName = toObj.Name;
                                        }
                                        else
                                        {
                                            inboxThreadItem.To = inboxThreadItem.To + ", " + toObj.Address;
                                        }
                                    }
                                    inboxThreadItem.IsRead = true;
                                    inboxThreads.InboxThread.Add(inboxThreadItem);
                                }
                            }

                            foreach (var notSeenUuId in notseenUids)
                            {
                                var message = folder1.GetMessage(notSeenUuId);

                                var inboxThreadItem = new InboxThread();
                                if (message.MessageId != null)
                                {
                                    inboxThreadItem.ThreadId = message.MessageId;
                                    inboxThreadItem.messageId = message.MessageId;
                                    inboxThreadItem.Subject = message.Subject;
                                    inboxThreadItem.BodyHtml = new Body();
                                    inboxThreadItem.BodyHtml.contentType = message.HtmlBody;
                                    DateTime messageDate = message.Date.UtcDateTime;
                                    inboxThreadItem.CreatedOn = messageDate;
                                    inboxThreadItem.Date = messageDate.ToString("yyyy-MM-dd");
                                    //inboxThreadItem.Date = DateTime.UtcNow.ToString();
                                    foreach (var item in message.From.Mailboxes)
                                    {
                                        MailboxAddress fromObj = new MailboxAddress();
                                        fromObj.Address = item.Address;
                                        fromObj.Name = item.Name;
                                        if (string.IsNullOrEmpty(inboxThreadItem.From))
                                        {
                                            inboxThreadItem.From = fromObj.Address;
                                            inboxThreadItem.FromName = fromObj.Name;
                                        }
                                        else
                                        {
                                            inboxThreadItem.From = inboxThreadItem.From + ", " + fromObj.Address;
                                        }
                                    }
                                    foreach (var item in message.To.Mailboxes)
                                    {
                                        MailboxAddress toObj = new MailboxAddress();
                                        toObj.Address = item.Address;
                                        toObj.Name = item.Name;
                                        if (string.IsNullOrEmpty(inboxThreadItem.To))
                                        {
                                            inboxThreadItem.To = toObj.Address;
                                            inboxThreadItem.ToName = toObj.Name;
                                        }
                                        else
                                        {
                                            inboxThreadItem.To = inboxThreadItem.To + ", " + toObj.Address;
                                        }
                                    }
                                    inboxThreadItem.IsRead = false;
                                    inboxThreads.InboxThread.Add(inboxThreadItem);
                                }
                            }

                            inboxThreads.InboxThread = inboxThreads.InboxThread.OrderByDescending(t => t.CreatedOn).ToList();
                            inboxThreads.InboxThread = inboxThreads.InboxThread.Take(model.Top.Value).Skip(model.Skip.Value).ToList();

                            //var draft = client.Inbox.Search(SearchQuery.Draft);

                            ////This is ue to get email ids
                            //var mailIds = client.Inbox.Search(SearchQuery.NotSeen);
                            //var mailIds1 = client.Inbox.Search(SearchQuery.Deleted);

                            // Get the first personal namespace and list the toplevel folders under it.
                            // var personal = client.GetFolder(client.PersonalNamespaces[0]);

                            // Console.WriteLine("Folders", personal.GetSubfolders(false));
                            // var folderList = personal.GetSubfolders(false);
                            // var isExist = folderList.Where(t => t.Name.ToLower().Contains(model.Label.ToLower())).FirstOrDefault();
                            // if (isExist == null)
                            // {
                            //     if (model.Label.ToLower() == "trash")
                            //     {
                            //         model.Label = "delete";
                            //         isExist = folderList.Where(t => t.Name.ToLower().Contains(model.Label.ToLower())).FirstOrDefault();
                            //     }
                            //     else if (model.Label.ToLower() == "spam")
                            //     {
                            //         model.Label = "junk";
                            //         isExist = folderList.Where(t => t.Name.ToLower().Contains(model.Label.ToLower())).FirstOrDefault();
                            //     }
                            // }

                            // foreach (var folder in personal.GetSubfolders(false))
                            // {
                            //     CustomEmailFolder emailFolder = new CustomEmailFolder();
                            //     emailFolder.Name = folder.Name;
                            //     emailFolder.Count = folder.Count;
                            //     // folder.Open(FolderAccess.ReadWrite);
                            //     // var uids = folder.Search(MailKit.Search.SearchQuery.HeaderContains("Message-Id", "78ae5f76f1684065aa334e67441c4140@techavidus.com"));
                            //     // if (uids.Count() > 0)
                            //     //     folder.AddFlags(uids, MessageFlags.Draft, silent: true);
                            //     // if (folder.Name.ToLower() == model.Label.ToLower())
                            //     if (folder.Name.ToLower().Contains(model.Label.ToLower()))
                            //     {
                            //         folder.Open(FolderAccess.ReadOnly);
                            //         int folderss = folder.Count;
                            //         // var notseenIds = folder.Search(MailKit.Search.SearchQuery.NotSeen);

                            //         //  var seenIds = folder.Search(MailKit.Search.SearchQuery.).Select(t => t.);
                            //         //For loog for getting email details
                            //         inboxThreads.count = folder.Count;
                            //         var totalLoop = folder.Count;
                            //         if (folder.Count > 20)
                            //         {
                            //             totalLoop = totalLoop - 20;
                            //         }
                            //         for (int j = folder.Count - 1; j >= 0; j--)
                            //         {
                            //             folder.Open(FolderAccess.ReadOnly);
                            //             var message = folder.GetMessage(j);
                            //             // folder.AddFlags(new int[] { j }, MessageFlags.Deleted, silent: true);
                            //             // var messages = folder.Fetch(notseenIds);
                            //             var objStr = message.ToString();


                            //             var inboxThreadItem = new InboxThread();
                            //             inboxThreadItem.ThreadId = message.MessageId;
                            //             // inboxThreadItem.MessageId = message.MessageId;
                            //             inboxThreadItem.Subject = message.Subject;
                            //             // inboxThreadItem.Snippet = message.HtmlBody;
                            //             inboxThreadItem.BodyHtml = new Body();
                            //             inboxThreadItem.BodyHtml.contentType = message.HtmlBody;
                            //             var messageDate = message.Date.UtcDateTime;
                            //             inboxThreadItem.Date = messageDate.ToString();
                            //             foreach (var item in message.From.Mailboxes)
                            //             {
                            //                 MailboxAddress fromObj = new MailboxAddress();
                            //                 fromObj.Address = item.Address;
                            //                 fromObj.Name = item.Name;
                            //                 if (string.IsNullOrEmpty(inboxThreadItem.From))
                            //                 {
                            //                     inboxThreadItem.From = fromObj.Address;
                            //                     inboxThreadItem.FromName = fromObj.Name;
                            //                 }
                            //                 else
                            //                 {
                            //                     inboxThreadItem.From = inboxThreadItem.From + ", " + fromObj.Address;
                            //                 }
                            //                 // inboxThreadItem.From.Add(fromObj);
                            //             }
                            //             foreach (var item in message.To.Mailboxes)
                            //             {
                            //                 MailboxAddress toObj = new MailboxAddress();
                            //                 toObj.Address = item.Address;
                            //                 toObj.Name = item.Name;
                            //                 if (string.IsNullOrEmpty(inboxThreadItem.To))
                            //                 {
                            //                     inboxThreadItem.To = toObj.Address;
                            //                     inboxThreadItem.ToName = toObj.Name;
                            //                 }
                            //                 else
                            //                 {
                            //                     inboxThreadItem.To = inboxThreadItem.To + ", " + toObj.Address;
                            //                 }
                            //                 // customEmailInbox.To.Add(toObj);
                            //             }

                            //             // foreach (var attachment in message.BodyParts)
                            //             // {
                            //             //     Console.WriteLine(attachment);
                            //             //     // byte[] allBytes = new byte[attachment.ContentStream.Length];
                            //             //     // int bytesRead = attachment.ContentStream.Read(allBytes, 0, (int)attachment.ContentStream.Length);

                            //             //     // string destinationFile = @"C:\Download\" + attachment.Name;

                            //             //     // BinaryWriter writer = new BinaryWriter(new FileStream(destinationFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None));
                            //             //     // writer.Write(allBytes);
                            //             //     // writer.Close();
                            //             // }

                            //             inboxThreads.InboxThread.Add(inboxThreadItem);
                            //         }

                            //         // var test =  JsonConvert.DeserializeObject<CustomEmailInbox>(objStr);
                            //         // listData.Add(customEmailInbox);

                            //         // //if (message.Subject.ToLower() == "test thread")
                            //         // //{
                            //         // list.Add(message);
                            //         // emailFolder.Messages.Add(customEmailInbox);
                            //         inboxThreads.InboxThread = inboxThreads.InboxThread.Take(model.Top.Value).Skip(model.Skip.Value).ToList();
                            //     }
                            //     // }
                            //     // folders.Add(emailFolder);
                            //     //Console.WriteLine("[folder] {0}", folder.Name);
                            // }
                            client.Disconnect(true);
                        }
                    }
                }
                inboxThreads.count = inboxThreads.InboxThread.Count();
                return inboxThreads;
            }

            var task = new List<Task>();
            var intproviderApp = _intProviderAppService.GetIntProviderApp(model.ProviderApp);

            // var intAppSecretObj = _intProviderAppSecretService.GetByUserAndEmail(userId, model.SelectedEmail);
            var intAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUserAndEmail(userId, model.SelectedEmail, intproviderApp.Id);


            if (intproviderApp != null && intAppSecretObj != null)
            {
                mailTokenDto.refresh_token = intAppSecretObj.Refresh_Token;
                mailTokenDto.access_token = intAppSecretObj.Access_Token;
                mailTokenDto.code = model.Code;
                mailTokenDto.ProviderApp = model.Provider;
                mailTokenDto.UserId = userId;
                intProviderAppSecretObj.Refresh_Token = intAppSecretObj.Refresh_Token;
                intProviderAppSecretObj.Access_Token = intAppSecretObj.Access_Token;
                intProviderAppSecretObj.Email = intAppSecretObj.Email;
            }
            // else
            // {
            //     var customEmailDomainObj = _customDomainEmailConfigService.GetByUserAndEmail(userId, model.SelectedEmail);
            //     if (customEmailDomainObj != null)
            //     {
            //         DataTable table = new DataTable();
            //         List<CustomEmailFolder> folders = new List<CustomEmailFolder>();
            //         List<CustomEmailInbox> listData = new List<CustomEmailInbox>();
            //         var list = new List<object>();
            //         using (var client = new ImapClient())
            //         {
            //             var options = SecureSocketOptions.StartTlsWhenAvailable;
            //             options = SecureSocketOptions.SslOnConnect;

            //             #region Host, Port, UserName, Passwrod with different host

            //             string Host = customEmailDomainObj.IMAPHost;
            //             int Port = customEmailDomainObj.IMAPPort.Value;
            //             // string UserName = "yagnik.darji@techavidus.com";
            //             // string Password = "8[)6fUV+'}";
            //             string UserName = customEmailDomainObj.Email;
            //             string Password = customEmailDomainObj.Password;

            //             #endregion

            //             client.CheckCertificateRevocation = false;
            //             client.ServerCertificateValidationCallback = (s, c, ch, e) => true;

            //             client.Connect(Host, Port, options);

            //             //client.AuthenticationMechanisms.Remove("XOAUTH2");

            //             client.Authenticate(UserName, Password);

            //             //Check IsConnected condition
            //             if (client.IsConnected)
            //             {
            //                 // The Inbox folder is always available on all IMAP servers...
            //                 var inbox = client.Inbox;
            //                 inbox.Open(FolderAccess.ReadOnly);

            //                 //var draft = client.Inbox.Search(SearchQuery.Draft);

            //                 ////This is ue to get email ids
            //                 //var mailIds = client.Inbox.Search(SearchQuery.NotSeen);
            //                 //var mailIds1 = client.Inbox.Search(SearchQuery.Deleted);

            //                 // Get the first personal namespace and list the toplevel folders under it.
            //                 var personal = client.GetFolder(client.PersonalNamespaces[0]);
            //                 Console.WriteLine("Folders", personal.GetSubfolders(false));
            //                 foreach (var folder in personal.GetSubfolders(false))
            //                 {
            //                     CustomEmailFolder emailFolder = new CustomEmailFolder();
            //                     emailFolder.Name = folder.Name;
            //                     emailFolder.Count = folder.Count;
            //                     // folder.Open(FolderAccess.ReadWrite);
            //                     // var uids = folder.Search(MailKit.Search.SearchQuery.HeaderContains("Message-Id", "78ae5f76f1684065aa334e67441c4140@techavidus.com"));
            //                     // if (uids.Count() > 0)
            //                     //     folder.AddFlags(uids, MessageFlags.Draft, silent: true);
            //                     if (folder.Name.ToLower() == model.Label.ToLower())
            //                     {

            //                         int folderss = folder.Count;
            //                         // var notseenIds = folder.Search(MailKit.Search.SearchQuery.NotSeen);

            //                         //  var seenIds = folder.Search(MailKit.Search.SearchQuery.).Select(t => t.);
            //                         //For loog for getting email details
            //                         for (int j = folder.Count - 1; j >= folder.Count - 20; j--)
            //                         {
            //                             folder.Open(FolderAccess.ReadOnly);
            //                             var message = folder.GetMessage(j);
            //                             // folder.AddFlags(new int[] { j }, MessageFlags.Deleted, silent: true);
            //                             // var messages = folder.Fetch(notseenIds);
            //                             var objStr = message.ToString();
            //                             // if(notseenIds.Contains())
            //                             CustomEmailInbox customEmailInbox = new CustomEmailInbox();
            //                             customEmailInbox.MessageId = message.MessageId;
            //                             customEmailInbox.Subject = message.Subject;
            //                             customEmailInbox.HtmlBody = message.HtmlBody;
            //                             var date1 = message.Date.UtcDateTime;
            //                             customEmailInbox.Date = date1;
            //                             // customEmailInbox.To = message.To;
            //                             // customEmailInbox.From = message.From;
            //                             customEmailInbox.InReplyTo = message.InReplyTo;
            //                             customEmailInbox.HtmlBody = message.HtmlBody;
            //                             customEmailInbox.Priority = message.Priority.ToString();
            //                             foreach (var item in message.From.Mailboxes)
            //                             {
            //                                 MailboxAddress fromObj = new MailboxAddress();
            //                                 fromObj.Address = item.Address;
            //                                 fromObj.Name = item.Name;
            //                                 customEmailInbox.From.Add(fromObj);
            //                             }
            //                             foreach (var item in message.To.Mailboxes)
            //                             {
            //                                 MailboxAddress toObj = new MailboxAddress();
            //                                 toObj.Address = item.Address;
            //                                 toObj.Name = item.Name;
            //                                 customEmailInbox.To.Add(toObj);
            //                             }
            //                             // customEmailInbox.From = message.From;
            //                             // customEmailInbox.To = message.To;

            //                             // if (message.From != null && message.From.Count() > 0)
            //                             // {
            //                             //     var data = message.From.Cast<InternetAddress>().ToList();
            //                             // }
            //                             // if (message.To != null && message.To.Count() > 0)
            //                             // {
            //                             //     customEmailInbox.To = message.To.Cast<InternetAddress>().ToList();
            //                             // }


            //                             // customEmailInbox.Mailboxes = message.Mailboxes;

            //                             // var test =  JsonConvert.DeserializeObject<CustomEmailInbox>(objStr);
            //                             listData.Add(customEmailInbox);

            //                             //if (message.Subject.ToLower() == "test thread")
            //                             //{
            //                             list.Add(message);
            //                             emailFolder.Messages.Add(customEmailInbox);
            //                         }
            //                     }
            //                     folders.Add(emailFolder);
            //                     //Console.WriteLine("[folder] {0}", folder.Name);
            //                 }
            //             }
            //         }
            //     }
            // }
            // var emailAccount = _emailAccountService.GetEmailAccountById(model.UserEmail.AccountId);
            // userEmail = _userEmailService.GetUserEmailById(model.UserEmail.UserEmailId);

            switch (intproviderApp.Name)
            {
                case "Gmail":
                    await SetGmailToken();
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    GmailThreads gmailThreads = null;
                    // model.Label = "inbox";
                    // model.NextPageToken = "";
                    model.Query = "";
                    var api = string.Format(DataUtility.GmailThreads, LabelBuilder(model.Label), model.Top, model.NextPageToken, model.Query);
                    //var gmailResponse = await client.GetAsync(string.Format(DataUtility.GmailThreads, LabelBuilder(model.Label), 30, model.NextPageToken, model.Query));
                    var gmailResponse = await client.GetAsync(api);
                    if (gmailResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var stream = await gmailResponse.Content.ReadAsStreamAsync();
                        var serializer = new DataContractJsonSerializer(typeof(GmailThreads));
                        gmailThreads = (GmailThreads)serializer.ReadObject(stream);
                    }

                    if (gmailThreads != null && gmailThreads.threads != null && gmailThreads.threads.Count > 0)
                    {
                        // model.UserEmail = userEmail;

                        inboxThreads.InboxThread = new List<InboxThread>();
                        inboxThreads.NextPageToken = gmailThreads.nextPageToken;
                        inboxThreads.count = gmailThreads.resultSizeEstimate;
                        foreach (var thread in gmailThreads.threads)
                            // task.Add(SetInboxThreadDetail(thread.id, model));
                            await SetInboxThreadDetail(thread.id, model);

                        // Task.WaitAll(task.ToArray());
                    }

                    // await GetLabelWithUnReadCount(userId, model);

                    break;

                case "Office 365":
                case "Outlook":

                    await SetOffice365Token();
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    Office365Thread office365Threads = null;
                    string office365Uri = "";
                    // model.Label = "inbox";
                    // All
                    //if (model.Label.ToLower() == "all") office365Uri = string.Format(DataUtility.Office365AllThreads, model.Skip, model.Top);
                    // Label
                    if (model.Label.ToLower() == "all") model.Label = DataUtility.Office365All;// DataUtility.Office365All;
                    else if (model.Label.ToLower() == "inbox") model.Label = DataUtility.Office365Inbox;// DataUtility.Office365Inbox;
                    else if (model.Label.ToLower() == "sent") model.Label = DataUtility.Office365SentItems; //DataUtility.Office365SentItems;
                    else if (model.Label.ToLower() == "draft") model.Label = DataUtility.Office365Drafts; // DataUtility.Office365Drafts;
                    else if (model.Label.ToLower() == "trash") model.Label = DataUtility.Office365DeletedItems;      // DataUtility.Office365DeletedItems;
                    else if (model.Label.ToLower() == "spam") model.Label = DataUtility.Office365JunkEmail;        // DataUtility.Office365JunkEmail;
                    office365Uri = string.Format(DataUtility.Office365Threads, model.Label.Replace(" ", ""), model.Skip, model.Top); //DataUtility.Office365Threads

                    if (model.Query != "" && model.Query != null) { office365Uri = string.Format(DataUtility.Office365ThreadsSearch, model.Label.Replace(" ", ""), model.Query); } //DataUtility.Office365ThreadsSearch

                    // office365Uri = "https://graph.microsoft.com/v1.0/me/mailfolders/Inbox/messages?$select=subject,from,toRecipients,ccRecipients,bccRecipients,receivedDateTime,hasAttachments,conversationId,isRead,bodyPreview&$count=true&$conversationId&$orderby=receivedDateTime DESC";

                    var office365Response = await client.GetAsync(office365Uri);
                    if (office365Response.StatusCode == HttpStatusCode.OK)
                    {
                        //var stream = await office365Response.Content.ReadAsStreamAsync();
                        //var serializer = new DataContractJsonSerializer(typeof(Office365Thread));
                        //office365Threads = (Office365Thread)serializer.ReadObject(stream);

                        var json = JObject.Parse(await office365Response.Content.ReadAsStringAsync());
                        office365Threads = JsonConvert.DeserializeObject<Office365Thread>(json.ToString());
                        // Total Count of Email
                        inboxThreads.count = office365Threads.count;

                        //var json = await client.GetStringAsync(label == "All" ? string.Format(office365Uri, skip, top) : string.Format(office365Uri, label, skip, top));
                        //var odata = JsonConvert.DeserializeObject<OData>(json);
                        //office365Threads.count = odata.count;

                    }

                    if (office365Threads != null && office365Threads.value.Length > 0)
                    {
                        inboxThreads.InboxThread = new List<InboxThread>();
                        // model.UserEmail = userEmail;
                        foreach (var thread in office365Threads.value)
                        {
                            // task.Add(SetOffice365InboxThreadDetail(thread, model));
                            await SetOffice365InboxThreadDetail(thread, model);
                        }

                        // Task.WaitAll(task.ToArray());
                        inboxThreads.count = office365Threads.count;


                        // if (inboxThreads.InboxThread != null)
                        // {
                        //     // Mark email as read if it is unread
                        //     if (inboxThreads.InboxThread.FirstOrDefault() != null) MarkOffice365EmailAsReadUnread(inboxThreads.InboxThread.FirstOrDefault().messageId, false);
                        // }
                    }

                    //await GetLabelWithUnReadCount(userId, model);

                    break;

                default:
                    break;
            }

            if (inboxThreads.InboxThread != null && inboxThreads.InboxThread.Count > 0)
            {
                inboxThreads.InboxThread = inboxThreads.InboxThread.OrderByDescending(x => x.InternalDate).ToList();
                task.Add(SetContactDetailInThreadEmail(model.Label));
                task.Add(SetNetworkMemberDetailInThreadEmail());
            }
            Task.WaitAll(task.ToArray());
            return inboxThreads;
        }

        public async Task<List<InboxThreadItem>> GetThreadByThreadId(int userId, string threadId, InboxVM model)
        {
            user = _userService.GetUserById(userId);
            if (user == null)
            {
                return inboxThreadItems;
            }

            // var emailAccount = _emailAccountService.GetEmailAccountById(model.UserEmail.AccountId);
            // userEmail = _userEmailService.GetUserEmailById(model.UserEmail.UserEmailId);

            if (string.IsNullOrEmpty(model.Provider) && string.IsNullOrEmpty(model.ProviderApp))
            {
                inboxThreadItems = new List<InboxThreadItem>();
                var customEmailDomainObj = _customDomainEmailConfigService.GetByUserAndEmail(userId, model.SelectedEmail);
                if (customEmailDomainObj != null)
                {
                    inboxThreads.InboxThread = new List<InboxThread>();
                    DataTable table = new DataTable();
                    List<CustomEmailFolder> folders = new List<CustomEmailFolder>();
                    List<CustomEmailInbox> listData = new List<CustomEmailInbox>();
                    var list = new List<object>();
                    using (var client = new ImapClient())
                    {
                        var options = SecureSocketOptions.StartTlsWhenAvailable;
                        options = SecureSocketOptions.SslOnConnect;

                        #region Host, Port, UserName, Passwrod with different host

                        string Host = customEmailDomainObj.IMAPHost;
                        int Port = customEmailDomainObj.IMAPPort.Value;
                        // string UserName = "yagnik.darji@techavidus.com";
                        // string Password = "8[)6fUV+'}";
                        string UserName = customEmailDomainObj.Email;
                        string Password = customEmailDomainObj.Password;

                        #endregion

                        client.CheckCertificateRevocation = false;
                        client.ServerCertificateValidationCallback = (s, c, ch, e) => true;

                        client.Connect(Host, Port, options);

                        //client.AuthenticationMechanisms.Remove("XOAUTH2");

                        client.Authenticate(UserName, Password);

                        //Check IsConnected condition
                        if (client.IsConnected)
                        {
                            // The Inbox folder is always available on all IMAP servers...
                            var inbox = client.Inbox;
                            inbox.Open(FolderAccess.ReadOnly);

                            //var draft = client.Inbox.Search(SearchQuery.Draft);

                            ////This is ue to get email ids
                            //var mailIds = client.Inbox.Search(SearchQuery.NotSeen);
                            //var mailIds1 = client.Inbox.Search(SearchQuery.Deleted);

                            // Get the first personal namespace and list the toplevel folders under it.
                            var personal = client.GetFolder(client.PersonalNamespaces[0]);

                            Console.WriteLine("Folders", personal.GetSubfolders(false));
                            var folderList = personal.GetSubfolders(false);
                            // var isExist = folderList.Where(t => t.ToLower().Contains(model.Label.ToLower())).FirstOrDefault();
                            // if (isExist == null)
                            // {
                            //     if (model.Label.ToLower() == "trash")
                            //     {
                            //         model.Label = "delete";
                            //         isExist = folderList.Where(t => t.Name.ToLower().Contains(model.Label.ToLower())).FirstOrDefault();
                            //     }
                            //     else if (model.Label.ToLower() == "spam")
                            //     {
                            //         model.Label = "junk";
                            //         isExist = folderList.Where(t => t.Name.ToLower().Contains(model.Label.ToLower())).FirstOrDefault();
                            //     }
                            // }

                            foreach (var folder in personal.GetSubfolders(false))
                            {
                                CustomEmailFolder emailFolder = new CustomEmailFolder();
                                emailFolder.Name = folder.Name;
                                emailFolder.Count = folder.Count;
                                folder.Open(FolderAccess.ReadOnly);
                                var uids = folder.Search(MailKit.Search.SearchQuery.HeaderContains("Message-Id", threadId));
                                if (uids.Count() == 0)
                                {
                                    uids = folder.Search(MailKit.Search.SearchQuery.HeaderContains("Message-Id", "<" + threadId + ">"));
                                }
                                if (uids.Count() > 0)
                                {
                                    foreach (var uid in uids)
                                    {
                                        var message = folder.GetMessage(uid);
                                        if (message != null)
                                        {
                                            var inboxThreadItem = new InboxThreadItem();
                                            inboxThreadItem.ThreadId = message.MessageId;
                                            inboxThreadItem.MessageId = message.MessageId;
                                            inboxThreadItem.Subject = message.Subject;
                                            inboxThreadItem.Snippet = message.HtmlBody;
                                            inboxThreadItem.BodyHtml = new Body();
                                            inboxThreadItem.BodyHtml.contentType = message.HtmlBody;
                                            inboxThreadItem.BodyHtml.data = message.HtmlBody;
                                            var messageDate = message.Date.UtcDateTime;
                                            inboxThreadItem.CreatedOn = message.Date.UtcDateTime;
                                            inboxThreadItem.Date = messageDate.ToString();
                                            foreach (var item in message.From.Mailboxes)
                                            {
                                                MailboxAddress fromObj = new MailboxAddress();
                                                fromObj.Address = item.Address;
                                                fromObj.Name = item.Name;
                                                if (string.IsNullOrEmpty(inboxThreadItem.From))
                                                {
                                                    inboxThreadItem.From = fromObj.Address;
                                                    inboxThreadItem.FromName = fromObj.Name != "" ? fromObj.Name : fromObj.Address;
                                                }
                                                else
                                                {
                                                    inboxThreadItem.From = inboxThreadItem.From + ", " + fromObj.Address;
                                                }
                                                // inboxThreadItem.From.Add(fromObj);
                                            }
                                            foreach (var item in message.To.Mailboxes)
                                            {
                                                MailboxAddress toObj = new MailboxAddress();
                                                toObj.Address = item.Address;
                                                toObj.Name = item.Name;
                                                if (string.IsNullOrEmpty(inboxThreadItem.To))
                                                {
                                                    inboxThreadItem.To = toObj.Address;
                                                    inboxThreadItem.ToName = toObj.Name != "" ? toObj.Name : toObj.Address;
                                                }
                                                else
                                                {
                                                    inboxThreadItem.To = inboxThreadItem.To + ", " + toObj.Address;
                                                }
                                                // customEmailInbox.To.Add(toObj);
                                            }

                                            foreach (var item in message.Cc.Mailboxes)
                                            {
                                                MailboxAddress CCObj = new MailboxAddress();
                                                CCObj.Address = item.Address;
                                                CCObj.Name = item.Name;
                                                if (string.IsNullOrEmpty(inboxThreadItem.CcEmail))
                                                {
                                                    inboxThreadItem.CcEmail = CCObj.Address;
                                                    inboxThreadItem.CcName = CCObj.Name;
                                                }
                                            }

                                            foreach (var item in message.Bcc.Mailboxes)
                                            {
                                                MailboxAddress BccObj = new MailboxAddress();
                                                BccObj.Address = item.Address;
                                                BccObj.Name = item.Name;
                                                if (string.IsNullOrEmpty(inboxThreadItem.BccEmail))
                                                {
                                                    inboxThreadItem.BccEmail = BccObj.Address;
                                                    inboxThreadItem.BccName = BccObj.Name;
                                                }
                                            }
                                            inboxThreadItems.Add(inboxThreadItem);
                                            break;
                                            // inbox.AddFlags(uid, MessageFlags.Deleted, true);
                                        }
                                    }
                                }

                                if (inboxThreadItems.Count() == 0 && !string.IsNullOrEmpty(model.Label))
                                {

                                    folder.Open(FolderAccess.ReadWrite);
                                    if (folder.Name.ToLower().Contains(model.Label.ToLower()))
                                    {
                                        folder.Open(FolderAccess.ReadOnly);
                                        int folderss = folder.Count;
                                        // var notseenIds = folder.Search(MailKit.Search.SearchQuery.NotSeen);

                                        //  var seenIds = folder.Search(MailKit.Search.SearchQuery.).Select(t => t.);
                                        //For loog for getting email details
                                        inboxThreads.count = folder.Count;
                                        var totalLoop = folder.Count;
                                        // if (folder.Count > 20)
                                        // {
                                        //     totalLoop = totalLoop - 20;
                                        // }
                                        for (int j = folder.Count - 1; j >= 0; j--)
                                        {
                                            folder.Open(FolderAccess.ReadOnly);
                                            var message = folder.GetMessage(j);
                                            // folder.AddFlags(new int[] { j }, MessageFlags.Deleted, silent: true);
                                            // var messages = folder.Fetch(notseenIds);

                                            if (message.MessageId == threadId)
                                            {
                                                var inboxThreadItem = new InboxThreadItem();
                                                inboxThreadItem.ThreadId = message.MessageId;
                                                inboxThreadItem.MessageId = message.MessageId;
                                                inboxThreadItem.Subject = message.Subject;
                                                inboxThreadItem.Snippet = message.HtmlBody;
                                                inboxThreadItem.BodyHtml = new Body();
                                                inboxThreadItem.BodyHtml.contentType = message.HtmlBody;
                                                inboxThreadItem.BodyHtml.data = message.HtmlBody;
                                                var messageDate = message.Date.UtcDateTime;
                                                inboxThreadItem.CreatedOn = message.Date.UtcDateTime;
                                                inboxThreadItem.Date = messageDate.ToString();
                                                foreach (var item in message.From.Mailboxes)
                                                {
                                                    MailboxAddress fromObj = new MailboxAddress();
                                                    fromObj.Address = item.Address;
                                                    fromObj.Name = item.Name;
                                                    if (string.IsNullOrEmpty(inboxThreadItem.From))
                                                    {
                                                        inboxThreadItem.From = fromObj.Address;
                                                        inboxThreadItem.FromName = fromObj.Name;
                                                    }
                                                    else
                                                    {
                                                        inboxThreadItem.From = inboxThreadItem.From + ", " + fromObj.Address;
                                                    }
                                                    // inboxThreadItem.From.Add(fromObj);
                                                }
                                                foreach (var item in message.To.Mailboxes)
                                                {
                                                    MailboxAddress toObj = new MailboxAddress();
                                                    toObj.Address = item.Address;
                                                    toObj.Name = item.Name;
                                                    if (string.IsNullOrEmpty(inboxThreadItem.To))
                                                    {
                                                        inboxThreadItem.To = toObj.Address;
                                                        inboxThreadItem.ToName = toObj.Name;
                                                    }
                                                    else
                                                    {
                                                        inboxThreadItem.To = inboxThreadItem.To + ", " + toObj.Address;
                                                    }
                                                    // customEmailInbox.To.Add(toObj);
                                                }

                                                foreach (var item in message.Cc.Mailboxes)
                                                {
                                                    MailboxAddress CCObj = new MailboxAddress();
                                                    CCObj.Address = item.Address;
                                                    CCObj.Name = item.Name;
                                                    if (string.IsNullOrEmpty(inboxThreadItem.CcEmail))
                                                    {
                                                        inboxThreadItem.CcEmail = CCObj.Address;
                                                        inboxThreadItem.CcName = CCObj.Name;
                                                    }
                                                }

                                                foreach (var item in message.Bcc.Mailboxes)
                                                {
                                                    MailboxAddress BccObj = new MailboxAddress();
                                                    BccObj.Address = item.Address;
                                                    BccObj.Name = item.Name;
                                                    if (string.IsNullOrEmpty(inboxThreadItem.BccEmail))
                                                    {
                                                        inboxThreadItem.BccEmail = BccObj.Address;
                                                        inboxThreadItem.BccName = BccObj.Name;
                                                    }
                                                }

                                                // foreach (var attachment in message.BodyParts)
                                                // {
                                                //     Console.WriteLine(attachment);
                                                //     // byte[] allBytes = new byte[attachment.ContentStream.Length];
                                                //     // int bytesRead = attachment.ContentStream.Read(allBytes, 0, (int)attachment.ContentStream.Length);

                                                //     // string destinationFile = @"C:\Download\" + attachment.Name;

                                                //     // BinaryWriter writer = new BinaryWriter(new FileStream(destinationFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None));
                                                //     // writer.Write(allBytes);
                                                //     // writer.Close();
                                                // }

                                                inboxThreadItems.Add(inboxThreadItem);
                                                break;
                                            }
                                            if (inboxThreadItems.Count() > 0)
                                            {
                                                break;
                                            }
                                        }


                                        // var test =  JsonConvert.DeserializeObject<CustomEmailInbox>(objStr);
                                        // listData.Add(customEmailInbox);

                                        // //if (message.Subject.ToLower() == "test thread")
                                        // //{
                                        // list.Add(message);
                                        // emailFolder.Messages.Add(customEmailInbox);
                                        inboxThreads.InboxThread = inboxThreads.InboxThread.Take(model.Top.Value).Skip(model.Skip.Value).ToList();
                                    }
                                }
                                // folders.Add(emailFolder);
                                //Console.WriteLine("[folder] {0}", folder.Name);
                                if (inboxThreadItems.Count() > 0)
                                {
                                    break;
                                }
                            }
                            client.Disconnect(true);
                        }
                    }
                }
                return inboxThreadItems;
            }

            var intproviderApp = _intProviderAppService.GetIntProviderApp(model.ProviderApp);

            // var intAppSecretObj = _intProviderAppSecretService.GetByUserAndEmail(userId, model.SelectedEmail);
            var intAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUserAndEmail(userId, model.SelectedEmail, intproviderApp.Id);
            var task = new List<Task>();

            if (intproviderApp != null && intAppSecretObj != null)
            {
                mailTokenDto.refresh_token = intAppSecretObj.Refresh_Token;
                mailTokenDto.access_token = intAppSecretObj.Access_Token;
                mailTokenDto.code = model.Code;
                mailTokenDto.ProviderApp = model.Provider;
                mailTokenDto.UserId = userId;
                intProviderAppSecretObj.Refresh_Token = intAppSecretObj.Refresh_Token;
                intProviderAppSecretObj.Access_Token = intAppSecretObj.Access_Token;
                intProviderAppSecretObj.Email = intAppSecretObj.Email;
            }

            switch (intproviderApp.Name)
            {
                case "Gmail":
                    await SetGmailToken();
                    // var task = new List<Task>();
                    inboxThreadItems = new List<InboxThreadItem>();
                    InboxVM objInbox = new InboxVM();
                    // objInbox.UserEmail = userEmail;
                    task.Add(SetInboxThreadDetail(threadId, objInbox, true));
                    Task.WaitAll(task.ToArray());

                    break;

                case "Office 365":
                case "Outlook":

                    await SetOffice365Token();
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    inboxThreadItems = new List<InboxThreadItem>();
                    // var threadResponse = await client.GetAsync(string.Format(DataUtility.Office365ThreadByConversionId, threadId)); //DataUtility.Office365ThreadByConversionId
                    var url = string.Format(DataUtility.Office365ThreadByConversionId, "'" + threadId + "'");
                    // var url2 = "https://graph.microsoft.com/v1.0/me/messages/AQMkADAwATMwMAItNDg3YS0zYzYyLTAwAi0wMAoARgAAAzLJyQDbpCyjSqlr6NglX8N7BwBh1tJcKOcASqMJqUbgSxJVAAACAQkAAABh1tJcKOcASqMJqUbgSxJVAAAAA7NxNAAAAA==";
                    var threadResponse = await client.GetAsync(url);
                    Office365Thread office365Threads = null;
                    if (threadResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var stream = await threadResponse.Content.ReadAsStreamAsync();

                        var serializer = new DataContractJsonSerializer(typeof(MessageConversation));
                        var threadDetail = (MessageConversation)serializer.ReadObject(stream);
                        if (threadDetail != null)
                        {
                            if (threadDetail.value != null)
                            {
                                foreach (var message in threadDetail.value)
                                {
                                    InboxThreadItem objThread = new InboxThreadItem();
                                    objThread.ThreadId = message.conversationId;
                                    //objThread.ThreadId = message.id;
                                    objThread.Subject = message.subject != null ? message.subject : "";
                                    objThread.From = message.from != null ? message.from.emailAddress.address : "";
                                    objThread.FromName = message.from != null ? message.from.emailAddress.name : "";
                                    objThread.FromEmail = message.from != null ? message.from.emailAddress.address : "";

                                    // To Email
                                    string toEmails = "";
                                    string toNames = "";
                                    int i = 0;
                                    if (message.toRecipients.Length > 0)
                                    {
                                        foreach (var item in message.toRecipients)
                                        {
                                            if (i == 0) { toEmails = item.emailAddress.address; toNames = item.emailAddress.name; }
                                            else { toEmails += ", " + item.emailAddress.address; toNames += ", " + item.emailAddress.name; }
                                            i++;
                                        }
                                    }
                                    objThread.To = message.toRecipients.Length > 0 ? toEmails : null;
                                    objThread.ToEmail = message.toRecipients.Length > 0 ? toEmails : null;
                                    objThread.ToName = message.toRecipients.Length > 0 ? toNames : null;

                                    // Cc Email
                                    string ccEmails = "";
                                    string ccNames = "";
                                    i = 0;
                                    if (message.ccRecipients.Length > 0)
                                    {
                                        foreach (var item in message.ccRecipients)
                                        {
                                            if (i == 0) { ccEmails = item.emailAddress.address; ccNames = item.emailAddress.name; }
                                            else { ccEmails += ", " + item.emailAddress.address; ccNames += ", " + item.emailAddress.name; }
                                            i++;
                                        }
                                    }
                                    objThread.CcEmail = message.ccRecipients.Length > 0 ? ccEmails : null;
                                    objThread.CcName = message.ccRecipients.Length > 0 ? ccNames : null;

                                    // Bcc Email
                                    string bccEmails = "";
                                    string bccNames = "";
                                    i = 0;
                                    if (message.bccRecipients.Length > 0)
                                    {
                                        foreach (var item in message.bccRecipients)
                                        {
                                            if (i == 0) { bccEmails = item.emailAddress.address; bccNames = item.emailAddress.name; }
                                            else { bccEmails += ", " + item.emailAddress.address; bccNames += ", " + item.emailAddress.name; }
                                            i++;
                                        }
                                    }
                                    objThread.BccEmail = message.bccRecipients.Length > 0 ? ccEmails : null;
                                    objThread.BccName = message.bccRecipients.Length > 0 ? ccNames : null;

                                    objThread.Date = message.receivedDateTime != null ? message.receivedDateTime : "";
                                    objThread.IsHasAttachment = message.hasAttachments;
                                    objThread.Snippet = message.bodyPreview;
                                    objThread.MessageId = message.id;
                                    objThread.LabelIds = null;
                                    objThread.SizeEstimate = 0;
                                    objThread.MimeType = null;
                                    objThread.Filename = null;
                                    objThread.OfficeBodyHtml = message.body;
                                    objThread.OfficeBodyPlain = message.bodyPreview;
                                    objThread.InternalDate = DataUtility.ConvertDateTimeToUnixTimeStamp(Convert.ToDateTime(message.receivedDateTime));
                                    objThread.CreatedOn = matcrm.service.Common.Common.UnixTimeStampToDateTimeMilliSec(objThread.InternalDate);
                                    objThread.IsOpen = message.isRead ? false : true;
                                    objThread.IsRead = message.isRead;
                                    objThread.Attachments = message.hasAttachments ? await GetOffice365Attachment(message.id) : null;
                                    inboxThreadItems.Add(objThread);

                                    // await MarkOffice365EmailAsReadUnread(message.id, false);
                                }
                            }
                        }
                    }

                    break;

                default:
                    break;
            }

            inboxThreadItems = inboxThreadItems.OrderByDescending(x => x.InternalDate).ToList();
            foreach (var item in inboxThreadItems)
            {
                string Label = model.Label != null ? model.Label : "";
                //var contact = await GetContactResultByFromAddress(Label.ToLower() == DataUtility.Office365SentItems.ToLower() || Label.ToLower() == DataUtility.GmailSENT.ToLower() ? item.To : item.From);
                var contact = await GetContactResultByFromAddress(Label.ToLower() == DataUtility.Office365SentItems.ToLower() || Label.ToLower() == DataUtility.GmailSENT.ToLower() ? item.To : item.From);
                item.Contact = contact;
                item.FromName = contact != null && contact.ContactId > 0 ? (contact.FirstName + " " + contact.LastName).Trim() : item.FromName;
            }

            return inboxThreadItems;
        }

        public async Task<BodyVM> GetAttachment(int userId, BodyVM model)
        {
            user = _userService.GetUserById(userId);
            if (user == null)
            {
                return model;
            }

            // var emailAccount = _emailAccountService.GetEmailAccountById(model.UserEmail.AccountId);
            // userEmail = _userEmailService.GetUserEmailById(model.UserEmail.UserEmailId);
            var intproviderApp = _intProviderAppService.GetIntProviderApp(model.ProviderApp);

            // var intAppSecretObj = _intProviderAppSecretService.GetByUserAndEmail(userId, model.SelectedEmail);
            var intAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUserAndEmail(userId, model.SelectedEmail, intproviderApp.Id);


            if (intproviderApp != null && intAppSecretObj != null)
            {
                mailTokenDto.refresh_token = intAppSecretObj.Refresh_Token;
                mailTokenDto.access_token = intAppSecretObj.Access_Token;
                // mailTokenDto.code = model.Code;
                mailTokenDto.ProviderApp = model.Provider;
                mailTokenDto.UserId = userId;
                intProviderAppSecretObj.Refresh_Token = intAppSecretObj.Refresh_Token;
                intProviderAppSecretObj.Access_Token = intAppSecretObj.Access_Token;
                intProviderAppSecretObj.Email = intAppSecretObj.Email;
            }

            switch (intproviderApp.Name)
            {
                case "Gmail":
                    await SetGmailToken();
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    //var response = await client.GetAsync(string.Format(DataUtility.GmailAttachment, model.MessageId, model.Body.attachmentId));
                    var response = await client.GetAsync(string.Format(DataUtility.GmailAttachment, model.MessageId, model.Body.attachmentId));
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var stream = await response.Content.ReadAsStreamAsync();
                        var serializer = new DataContractJsonSerializer(typeof(Body));
                        var attachment = (Body)serializer.ReadObject(stream);

                        if (attachment == null)
                        {
                            model.IsValid = false;
                            model.ErrorMessage = CommonMessage.ErrorOccurredAttachment;
                            return model;
                        }


                        String attachData = attachment.data.Replace('-', '+');
                        attachData = attachData.Replace('_', '/');
                        model.Body.data = attachment.data;//Utility.DecodeBase64String(attachment.data);
                        byte[] data = Convert.FromBase64String(attachData);
                        model.Body.bytes = data;
                        model.Body.size = attachment.size;
                        model.IsValid = true;
                    }
                    else
                    {
                        model.IsValid = false;
                        model.ErrorMessage = response.StatusCode.ToString();

                        var stream = await response.Content.ReadAsStreamAsync();
                        var serializer = new DataContractJsonSerializer(typeof(GmailError));
                        var GmailError = (GmailError)serializer.ReadObject(stream);

                        if (GmailError != null && !string.IsNullOrEmpty(GmailError.error.message))
                            model.ErrorMessage = GmailError.error.code + "-" + GmailError.error.message;

                        return model;
                    }

                    break;
            }

            return model;
        }

        #region Get Email Contact wise

        public async Task<InboxThreads> GetEmailForContact(int userId, InboxVM model)
        {
            user = _userService.GetUserById(userId);
            if (user == null)
            {
                inboxThreads.IsValid = false;
                inboxThreads.ErrorMessage = CommonMessage.UnAuthorizedUser;
                return inboxThreads;
            }
            // var userAllEmail = _userEmailService.GetAllUserEmailByUserId(userId);

            // if (model.Filterdata != null && model.Filterdata.Count > 0)
            // {
            //     userAllEmail = userAllEmail.Where(a => model.Filterdata.Any(f => a.UserEmailId == f)).ToList();
            // }
            // else if (model.UserEmailId > 0)
            // {
            //     userAllEmail = userAllEmail.FindAll(x => x.UserEmailId == model.UserEmailId).ToList();
            // }

            var task = new List<Task>();
            if (user != null)
            {
                inboxThreads.InboxThread = new List<InboxThread>();
                // inboxThreads.EmailAccount = _emailAccountService.GetAllEmailAccount();
                // foreach (var item in userAllEmail)
                // {
                // var emailAccount = _emailAccountService.GetEmailAccountById(item.AccountId);
                // userEmail = item;

                var intproviderApp = _intProviderAppService.GetIntProviderApp(model.ProviderApp);

                // var intAppSecretObj = _intProviderAppSecretService.GetByUserAndEmail(userId, model.SelectedEmail);
                var intAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUserAndEmail(userId, model.SelectedEmail, intproviderApp.Id);


                if (intproviderApp != null && intAppSecretObj != null)
                {
                    mailTokenDto.refresh_token = intAppSecretObj.Refresh_Token;
                    mailTokenDto.access_token = intAppSecretObj.Access_Token;
                    mailTokenDto.code = model.Code;
                    mailTokenDto.ProviderApp = model.Provider;
                    mailTokenDto.UserId = userId;
                    intProviderAppSecretObj.Refresh_Token = intAppSecretObj.Refresh_Token;
                    intProviderAppSecretObj.Access_Token = intAppSecretObj.Access_Token;
                    intProviderAppSecretObj.Email = intAppSecretObj.Email;
                }

                switch (intproviderApp.Name)
                {
                    case "Gmail":
                        // model.UserEmail = item;
                        model.Label = "";
                        //task.Add(GmailContactEmail(model));
                        await GmailContactEmail(model);
                        break;
                    case "Office 365":
                    case "Outlook":
                        // model.UserEmail = item;
                        model.Label = "all";
                        //task.Add(Office365ContactEmail(model));
                        await Office365ContactEmail(model);
                        break;
                    default:
                        break;
                }
                // }

                Task.WaitAll(task.ToArray());
                if (inboxThreads.InboxThread != null && inboxThreads.InboxThread.Count > 0)
                {
                    inboxThreads.InboxThread = inboxThreads.InboxThread.OrderByDescending(x => x.InternalDate).ToList();
                    task.Add(SetContactDetailInThreadEmail(model.Label));
                    task.Add(SetNetworkMemberDetailInThreadEmail());
                }
                Task.WaitAll(task.ToArray());
            }
            //inboxThreads.count = inboxThreads.InboxThread.Count;
            return inboxThreads;
        }

        public async Task<bool> GmailContactEmail(InboxVM model)
        {
            var task = new List<Task>();
            await SetGmailToken();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            GmailThreads gmailThreads = null;
            //var gmailResponse = await client.GetAsync(string.Format(DataUtility.GmailThreads, LabelBuilder(model.Label), 30, model.NextPageToken, model.Query));
            if (model.NextPageTokens != null)
            {
                string tokens = model.NextPageTokens.Find(x => x.UserEmailId == model.UserEmail.UserEmailId).PageToken;
                model.NextPageToken = tokens;
            }

            model.NextPageToken = "";
            model.Query = "";
            var api = string.Format(DataUtility.GmailThreads, LabelBuilder(model.Label), model.Top, model.NextPageToken, model.Query);

            // var gmailResponse = await client.GetAsync(string.Format(DataUtility.GmailThreads, LabelBuilder(model.Label), model.Top, model.NextPageToken, model.Query));
            var gmailResponse = await client.GetAsync(string.Format(api));
            if (gmailResponse.StatusCode == HttpStatusCode.OK)
            {
                var stream = await gmailResponse.Content.ReadAsStreamAsync();
                var serializer = new DataContractJsonSerializer(typeof(GmailThreads));
                gmailThreads = (GmailThreads)serializer.ReadObject(stream);
            }
            if (gmailThreads != null && gmailThreads.threads != null && gmailThreads.threads.Count > 0)
            {
                inboxThreads.NextPageToken = gmailThreads.nextPageToken;
                NextPageToken objnxt = new NextPageToken();
                objnxt.PageToken = gmailThreads.nextPageToken;
                objnxt.UserEmailId = model.UserEmail.UserEmailId;
                NextPageToken.Add(objnxt);
                inboxThreads.NextPageTokens = NextPageToken;

                inboxThreads.count += gmailThreads.resultSizeEstimate;
                foreach (var thread in gmailThreads.threads)
                    task.Add(SetInboxThreadDetail(thread.id, model));

                Task.WaitAll(task.ToArray());
            }
            return true;
        }

        public async Task<bool> Office365ContactEmail(InboxVM model)
        {
            var task = new List<Task>();
            await SetOffice365Token();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            Office365Thread office365Threads = null;
            string office365Uri = "";

            if (model.Label.ToLower() == "all") model.Label = DataUtility.Office365All; // DataUtility.Office365All;
            else if (model.Label.ToLower() == "inbox") model.Label = DataUtility.Office365Inbox;  // DataUtility.Office365Inbox;
            else if (model.Label.ToLower() == "sent") model.Label = DataUtility.Office365SentItems;   // DataUtility.Office365SentItems;
            else if (model.Label.ToLower() == "draft") model.Label = DataUtility.Office365Drafts;  // DataUtility.Office365Drafts;
            else if (model.Label.ToLower() == "trash") model.Label = DataUtility.Office365DeletedItems;  // DataUtility.Office365DeletedItems;
            else if (model.Label.ToLower() == "spam") model.Label = DataUtility.Office365JunkEmail;   // DataUtility.Office365JunkEmail;
            office365Uri = string.Format(DataUtility.Office365Threads, model.Label.Replace(" ", ""), model.Skip, model.Top); //DataUtility.Office365Threads
            if (model.Query != "" && model.Query != null) { office365Uri = string.Format(DataUtility.Office365ThreadsSearch, model.Label.Replace(" ", ""), "%22" + model.Query + "%22"); }  //DataUtility.Office365ThreadsSearch
            var office365Response = await client.GetAsync(office365Uri);
            if (office365Response.StatusCode == HttpStatusCode.OK)
            {
                var json = JObject.Parse(await office365Response.Content.ReadAsStringAsync());
                office365Threads = JsonConvert.DeserializeObject<Office365Thread>(json.ToString());
            }
            if (office365Threads != null && office365Threads.value.Length > 0)
            {
                if (inboxThreads.InboxThread == null) inboxThreads.InboxThread = new List<InboxThread>();

                foreach (var thread in office365Threads.value)
                    task.Add(SetOffice365InboxThreadDetail(thread, model));

                Task.WaitAll(task.ToArray());
            }
            return true;
        }

        #endregion
        #endregion
        #region 'Set Function'
        public async Task<MailTokenDto> AuthenticationComplete(int userId, MailTokenDto model)
        {

            user = _userService.GetUserById(userId);
            if (user == null)
            {
                model.IsValid = false;
                model.ErrorMessage = CommonMessage.UnAuthorizedUser;
                return model;
            }
            // var userAllEmail = _userEmailService.GetAllUserEmailByUserId(userId);

            // if (model.Filterdata != null && model.Filterdata.Count > 0)
            // {
            //     userAllEmail = userAllEmail.Where(a => model.Filterdata.Any(f => a.UserEmailId == f)).ToList();
            // }
            // else if (model.UserEmailId > 0)
            // {
            //     userAllEmail = userAllEmail.FindAll(x => x.UserEmailId == model.UserEmailId).ToList();
            // }

            var task = new List<Task>();
            // if (user != null)
            // {
            inboxThreads.InboxThread = new List<InboxThread>();
            // inboxThreads.EmailAccount = _emailAccountService.GetAllEmailAccount();
            // foreach (var item in userAllEmail)
            // {
            // var emailAccount = _emailAccountService.GetEmailAccountById(item.AccountId);
            // userEmail = item;

            var intproviderApp = _intProviderAppService.GetIntProviderApp(model.ProviderApp);

            var intAppSecretObj = _intProviderAppSecretService.GetByUserAndEmail(model.UserId, model.SelectedEmail);


            if (model.teamInboxId != null)
            {                
                var isExist = _intProviderAppSecretService.GetActiveSecretByUserAndEmail(userId, model.SelectedEmail, intproviderApp.Id);
                if (isExist != null)
                {
                    model.ErrorMessage = "Email account already synced";
                    model.IsValid = false;
                    return model;
                }
            }

            if (intproviderApp != null && intAppSecretObj != null)
            {
                mailTokenDto.refresh_token = intAppSecretObj.Refresh_Token;
                mailTokenDto.access_token = intAppSecretObj.Access_Token;
                // mailTokenDto.code = model.Code;
                mailTokenDto.ProviderApp = model.Provider;
                mailTokenDto.UserId = userId;
                intProviderAppSecretObj.Refresh_Token = intAppSecretObj.Refresh_Token;
                intProviderAppSecretObj.Access_Token = intAppSecretObj.Access_Token;
                intProviderAppSecretObj.Email = intAppSecretObj.Email;
                model.SelectedEmail = intAppSecretObj.Email;
                model.email = intAppSecretObj.Email;
                if (model.teamInboxId != null)
                {
                    var teamInboxObj = _teamInboxService.GetById(model.teamInboxId.Value);
                    if (teamInboxObj != null)
                    {
                        teamInboxObj.IntProviderAppSecretId = intProviderAppSecretObj.Id;
                        model.SelectedEmail = intProviderAppSecretObj.Email;
                        var teamInboxDto = _mapper.Map<TeamInboxDto>(teamInboxObj);
                        var teamInboxObj1 = await _teamInboxService.CheckInsertOrUpdate(teamInboxDto);
                    }
                }
            }
            UserEmail userEmail = new UserEmail();


            userEmail.UserId = model.UserId;
            userEmail.Email = model.SelectedEmail;
            // userEmail.Code = model.Code;
            // userEmail.AccountId = model.AccountId;
            userEmail.CreatedOn = DataUtility.GetCurrentDateTime();
            // model.UserEmailId = userEmail.UserEmailId;

            // if (userEmail == null || userEmail.UserEmailId <= 0)
            // {
            //     model.IsValid = false;
            //     model.ErrorMessage = CommonMessage.SomethingWentWrong;
            //     return model;
            // }

            switch (intproviderApp.Name)
            {
                case "Gmail":
                    var tokenModel = await GetGmailToken(model, model.redirect_uri);
                    if (!tokenModel.IsValid)
                    {
                        model.IsValid = false;
                        model.ErrorMessage = tokenModel.ErrorMessage;
                        return model;
                    }
                    // if (!isTokenReceived && model.teamInboxId != null)
                    // {
                    //     // model.IsTokenReceived = false;
                    //     model.IsValid = false;
                    //     model.ErrorMessage = "Email account already synced";
                    //     return model;
                    // }
                    // else if (!isTokenReceived)
                    // {
                    //     model.IsValid = false;
                    //     model.ErrorMessage = CommonMessage.ErrorOccuredInTokenGet;
                    //     return model;
                    // }

                    model.SelectedEmail = intProviderAppSecretObj.Email;
                    model.email = intProviderAppSecretObj.Email;

                    break;

                case "Office 365":
                case "Outlook":
                    var isToken365Received = await GetOffice365Token(model, model.redirect_uri);
                    if (!isToken365Received)
                    {
                        // model.IsTokenReceived = false;
                        model.IsValid = false;
                        model.ErrorMessage = CommonMessage.ErrorOccuredInTokenGet;
                        return model;
                    }

                    break;
            }

            model.IsValid = true;
            model.ErrorMessage = CommonMessage.AuthenticationSuccess;
            return model;
        }

        public async Task<CustomDomainEmailConfigDto> CustomDomainAuthentication(int userId, CustomDomainEmailConfigDto model)
        {

            user = _userService.GetUserById(userId);
            if (user == null)
            {
                model.IsValid = false;
                model.ErrorMessage = CommonMessage.UnAuthorizedUser;
                return model;
            }

            // bool isIMAPConnect = false;
            // bool isSMTPConnect = false;

            // using (var imapClient = new ImapClient())
            // {
            //     var options = SecureSocketOptions.StartTlsWhenAvailable;
            //     options = SecureSocketOptions.SslOnConnect;

            //     #region Host, Port, UserName, Passwrod with different host

            //     // string Host = "mail.techavidus.com";
            //     // int Port = 993;
            //     // // string UserName = "yagnik.darji@techavidus.com";
            //     // // string Password = "8[)6fUV+'}";
            //     // string UserName = "shraddha.prajapati@techavidus.com";
            //     // string Password = "SD@TA10";
            //     string Host = model.IMAPHost;
            //     int Port = model.IMAPPort.Value;
            //     string UserName = model.Email;
            //     string Password = model.Password;

            //     #endregion

            //     imapClient.CheckCertificateRevocation = false;
            //     imapClient.ServerCertificateValidationCallback = (s, c, ch, e) => true;

            //     imapClient.Connect(Host, Port, options);

            //     imapClient.Authenticate(UserName, Password);

            //     //Check IsConnected condition
            //     if (imapClient.IsConnected)
            //     {
            //         isIMAPConnect = true;
            //     }
            //     imapClient.Disconnect(true);
            // }

            // using (var client = new TcpClient())
            // {
            //     client.Connect(model.SMTPHost, model.SMTPPort.Value);
            //     var aa = client.Connected;
            //     if (client.Connected)
            //     {
            //         isSMTPConnect = true;
            //     }
            //     client.Close();
            // }
            // if (isSMTPConnect == true && isIMAPConnect == true)
            // {
            Console.WriteLine("Both host connected......");
            IntProviderAppSecretDto intProviderAppSecretDto = new IntProviderAppSecretDto();
            intProviderAppSecretDto.Email = model.Email;
            intProviderAppSecretDto.Color = model.Color;
            intProviderAppSecretDto.CreatedBy = userId;
            var AddUpdate = await _intProviderAppSecretService.CheckInsertOrUpdate(intProviderAppSecretDto, true);
            if (model.TeamInboxId != null)
            {
                TeamInbox teamInboxObj = _teamInboxService.GetById(model.TeamInboxId.Value);
                if (teamInboxObj != null)
                {
                    teamInboxObj.IntProviderAppSecretId = AddUpdate.Id;
                    var teamInboxDto = _mapper.Map<TeamInboxDto>(teamInboxObj);
                    var teamInboxObj1 = await _teamInboxService.CheckInsertOrUpdate(teamInboxDto);
                }
            }
            model.IntProviderAppSecretId = AddUpdate.Id;
            model.CreatedBy = userId;
            var data = await _customDomainEmailConfigService.CheckInsertOrUpdate(model);
            model.Id = data.Id;
            model.IsValid = true;
            model.ErrorMessage = CommonMessage.AuthenticationSuccess;
            return model;
            // }
            // else
            // {
            //     model.IsValid = false;
            //     model.ErrorMessage = CommonMessage.InvalidCredential;
            //     return model;
            // }
        }


        public async Task<CustomDomainEmailConfigDto> TestConnection(CustomDomainEmailConfigDto model)
        {

            bool isIMAPConnect = false;
            bool isSMTPConnect = false;

            using (var imapClient = new ImapClient())
            {
                var options = SecureSocketOptions.StartTlsWhenAvailable;
                options = SecureSocketOptions.SslOnConnect;

                #region Host, Port, UserName, Passwrod with different host

                // string Host = "mail.techavidus.com";
                // int Port = 993;
                // // string UserName = "yagnik.darji@techavidus.com";
                // // string Password = "8[)6fUV+'}";
                // string UserName = "shraddha.prajapati@techavidus.com";
                // string Password = "SD@TA10";
                string Host = model.IMAPHost;
                int Port = model.IMAPPort.Value;
                string UserName = model.Email;
                string Password = model.Password;

                #endregion

                imapClient.CheckCertificateRevocation = false;
                imapClient.ServerCertificateValidationCallback = (s, c, ch, e) => true;

                imapClient.Connect(Host, Port, options);

                imapClient.Authenticate(UserName, Password);

                //Check IsConnected condition
                if (imapClient.IsConnected)
                {
                    isIMAPConnect = true;
                }
                imapClient.Disconnect(true);
            }

            using (var client = new TcpClient())
            {
                client.Connect(model.SMTPHost, model.SMTPPort.Value);
                var aa = client.Connected;
                if (client.Connected)
                {
                    isSMTPConnect = true;
                }
                client.Close();
            }
            if (isSMTPConnect == true && isIMAPConnect == true)
            {
                model.IsValid = true;
                return model;
            }
            else
            {
                model.IsValid = false;
                model.ErrorMessage = CommonMessage.InvalidCredential;
                return model;
            }
        }

        private async Task<MailTokenDto> GetGmailToken(MailTokenDto model, string redirectUri)
        {
            var isTokenReceived = false;
            client.DefaultRequestHeaders.Clear();

            var param = new Dictionary<string, string>();
            param.Add("code", model.code);
            param.Add("client_id", OneClappContext.GoogleCalendarClientId);   //DataUtility.GmailApiClientId
            param.Add("client_secret", OneClappContext.GoogleCalendarSecretKey);   //DataUtility.GmailApiClientSecret
            param.Add("redirect_uri", redirectUri);
            param.Add("grant_type", "authorization_code");

            //var response = await client.PostAsync(DataUtility.GmailToken, new FormUrlEncodedContent(param));
            var response = await client.PostAsync(DataUtility.GmailToken, new FormUrlEncodedContent(param));
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var serializer = new DataContractJsonSerializer(typeof(Token));
                var gmailToken = (Token)serializer.ReadObject(stream);

                if (gmailToken != null && !string.IsNullOrEmpty(gmailToken.access_token))
                {
                    var IntProviderAppObj = _intProviderAppService.GetIntProviderApp(model.ProviderApp);
                    IntProviderAppSecretDto secretDto = new IntProviderAppSecretDto();
                    secretDto.Access_Token = gmailToken.access_token;
                    secretDto.Expires_In = gmailToken.expires_in;
                    secretDto.Refresh_Token = gmailToken.refresh_token;
                    secretDto.Scope = gmailToken.scope;
                    secretDto.Token_Type = gmailToken.token_type;
                    secretDto.Id_Token = gmailToken.id_token;
                    secretDto.CreatedBy = model.UserId;
                    secretDto.IntProviderAppId = IntProviderAppObj.Id;
                    secretDto.Color = model.Color;

                    var tokenInfo = await _calendarService.GetTokenInfo(gmailToken.access_token, OneClappContext.GoogleCalendarSecretKey);
                    if ((tokenInfo != null && (string.IsNullOrEmpty(tokenInfo.error_description))))
                    {
                        secretDto.Email = tokenInfo.email;
                    }

                    // if (model.teamInboxId != null)
                    // {
                    var isExist = _intProviderAppSecretService.GetActiveSecretByUserAndEmail(user.Id, tokenInfo.email, IntProviderAppObj.Id);
                    if (isExist != null)
                    {
                        model.ErrorMessage = "Email account already synced";
                        model.IsValid = false;
                        isTokenReceived = false;
                        return model;
                        // return isTokenReceived;
                    }
                    // }

                    intProviderAppSecretObj = await _intProviderAppSecretService.CheckInsertOrUpdate(secretDto);

                    if (model.teamInboxId != null)
                    {
                        var teamInboxObj = _teamInboxService.GetById(model.teamInboxId.Value);
                        if (teamInboxObj != null)
                        {
                            teamInboxObj.IntProviderAppSecretId = intProviderAppSecretObj.Id;
                            model.SelectedEmail = intProviderAppSecretObj.Email;
                            var teamInboxDto = _mapper.Map<TeamInboxDto>(teamInboxObj);
                            var teamInboxObj1 = await _teamInboxService.CheckInsertOrUpdate(teamInboxDto);
                        }
                    }
                    token = gmailToken.access_token;
                    model.access_token = token;
                    model.refresh_token = gmailToken.refresh_token;
                    model.expires_in = Convert.ToInt64(gmailToken.expires_in);
                    model.scope = gmailToken.scope;
                    var dt = DataUtility.GetCurrentDateTime();
                    //model.ExpireOn = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second + Convert.ToInt32(model.ExpireIn));
                    model.refresh_token = gmailToken.refresh_token;

                    // _userEmailService.CheckInsertOrUpdate(model);
                    isTokenReceived = true;
                    model.IsValid = true;
                }
            }

            // return isTokenReceived;
            return model;
        }

        private async Task SetGmailToken()
        {
            //string reToken = "1/aGnkz31yzmqGcnr2BIHlHjNJ8ViLXrGFX8iYbOFp1CTSehEjdTnLbY8WC-X_Z5VF";

            client.DefaultRequestHeaders.Clear();

            var param = new Dictionary<string, string>();
            param.Add("client_id", OneClappContext.GoogleCalendarClientId);           //DataUtility.GmailApiClientId
            param.Add("client_secret", OneClappContext.GoogleCalendarSecretKey);   //DataUtility.GmailApiClientSecret
            param.Add("refresh_token", intProviderAppSecretObj.Refresh_Token);
            param.Add("grant_type", "refresh_token");

            //var response = await client.PostAsync(DataUtility.GmailToken, new FormUrlEncodedContent(param));
            var response = await client.PostAsync(DataUtility.GmailToken, new FormUrlEncodedContent(param));
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var serializer = new DataContractJsonSerializer(typeof(Token));
                var gmailToken = (Token)serializer.ReadObject(stream);

                if (gmailToken != null) token = gmailToken.access_token;
            }
        }

        // public async Task<ComposeEmail> SendEmailWithReplaceVariables(int userId, ComposeEmail model, IFormFileCollection files)
        public async Task<ComposeEmail> SendEmailWithReplaceVariables(int userId, ComposeEmail model, IFormFile[] files)
        {
            string mailBody = model.Body;
            foreach (var item in model.To)
            {
                var user = _userService.GetUserById(userId);
                // var userDetail = _userService.GetUserDetail(userId, null);
                // var contact = _contactService.GetContactByUserIdAndEmail(userId, item.Contains("<") ? item.Split("<")[1].Trim('>').Trim() : item);

                if (user != null)
                {
                    model.Body = mailBody;
                    model.Body = model.Body.Replace("{{contact_first_name}}", user.FirstName);
                    model.Body = model.Body.Replace("{{contact_last_name}}", user.LastName);
                    model.Body = model.Body.Replace("{{contact_phone}}", user.PhoneNo);
                    model.Body = model.Body.Replace("{{contact_email}}", user.Email);
                    model.Body = model.Body.Replace("{{contact_address}}", user.Address);
                    model.Body = model.Body.Replace("{{system_day_of_week}}", Convert.ToString(DataUtility.GetSystemDayOfWeek()));
                    model.Body = model.Body.Replace("{{system_today}}", Convert.ToString(DataUtility.GetCurrentDateTime()));
                    model.Body = model.Body.Replace("{{system_currentmonth}}", Convert.ToString(DataUtility.GetSystemMonth()));
                    model.Body = model.Body.Replace("{{my_first_name}}", user.FirstName);
                    model.Body = model.Body.Replace("{{my_last_name}}", user.LastName);
                    model.Body = model.Body.Replace("{{my_email}}", user.Email);
                    model.Body = model.Body.Replace("{{my_mobile_phone}}", user.PhoneNo);
                    // model.Body = model.Body.Replace("{{my_title}}", user.AgentTitle);
                    model.Body = model.Body.Replace("{{my_address}}", user.Address);
                    // model.Body = model.Body.Replace("{{my_statedID}}", user.StateName);
                    // model.Body = model.Body.Replace("{{my_NARID}}", user.NarId);
                    // model.Body = model.Body.Replace("{{my_website}}", user.Website);
                    model.Body = model.Body.Replace("{{my_office_phone}}", user.PhoneNo);
                }

                // if (contact != null && contact.ContactId > 0)
                // {
                // int cityId = contact.CityId > 0 ? Convert.ToInt32(contact.CityId) : 0;
                // int stateId = contact.StateId > 0 ? Convert.ToInt32(contact.StateId) : 0;
                //var CountryId = contact.CountryId > 0 ? Convert.ToInt32(contact.CountryId) : 0;
                // model.Body = mailBody;
                // model.Body = model.Body.Replace("{{contact_first_name}}", contact.FirstName);
                // model.Body = model.Body.Replace("{{contact_last_name}}", contact.LastName);
                // model.Body = model.Body.Replace("{{contact_phone}}", contact.Phone);
                // model.Body = model.Body.Replace("{{contact_email}}", contact.Email);
                // model.Body = model.Body.Replace("{{contact_address}}", contact.Address);
                // model.Body = model.Body.Replace("{{contact_zip}}", contact.Zip);
                // model.Body = model.Body.Replace("{{contact_state}}", stateId > 0 ? _stateService.GetStateById(stateId).StateName ?? "" : "");
                // model.Body = model.Body.Replace("{{contact_city}}", cityId > 0 ? _cityService.GetCityById(cityId).CityName ?? "" : "");
                // model.Body = model.Body.Replace("{{system_day_of_week}}", Convert.ToString(DataUtility.GetSystemDayOfWeek()));
                // model.Body = model.Body.Replace("{{system_today}}", Convert.ToString(DataUtility.GetCurrentDateTime()));
                // model.Body = model.Body.Replace("{{system_currentmonth}}", Convert.ToString(DataUtility.GetSystemMonth()));
                // model.Body = model.Body.Replace("{{my_first_name}}", userDetail[0].FirstName);
                // model.Body = model.Body.Replace("{{my_last_name}}", userDetail[0].LastName);
                // model.Body = model.Body.Replace("{{my_email}}", userDetail[0].Email);
                // model.Body = model.Body.Replace("{{my_mobile_phone}}", userDetail[0].MobilePhone);
                // model.Body = model.Body.Replace("{{my_title}}", userDetail[0].AgentTitle);
                // model.Body = model.Body.Replace("{{my_address}}", userDetail[0].Address1);
                // model.Body = model.Body.Replace("{{my_statedID}}", userDetail[0].StateName);
                // model.Body = model.Body.Replace("{{my_NARID}}", userDetail[0].NarId);
                // model.Body = model.Body.Replace("{{my_website}}", userDetail[0].Website);
                // model.Body = model.Body.Replace("{{my_office_phone}}", userDetail[0].MobilePhone);
                //model.Body = model.Body.Replace("{{my_email_signature}}", user.EmailSignature);
                //model.Body = model.Body.Replace('{{my_sms_signature}}',user.           -- NOT in DB
                // }

                var results = await CreateEmailBody(userId, model, files, item.Contains("<") ? item.Split("<")[1].Trim('>').Trim() : item);
            }
            return model;
        }


        public async Task<ComposeEmail> CreateEmailDraft(int userId, ComposeEmail model, IFormFile[] files)
        {
            user = _userService.GetUserById(userId);
            if (user == null)
            {
                model.IsValid = false;
                model.ErrorMessage = CommonMessage.UnAuthorizedUser;
                return model;
            }

            // userEmail = _userEmailService.GetUserEmailById(model.UserEmail.UserEmailId);
            // if (userEmail == null)
            // {
            //     model.IsValid = false;
            //     model.ErrorMessage = CommonMessage.UnAuthorizedUser;
            // }

            // var emailAccount = _emailAccountService.GetEmailAccountById(model.UserEmail.AccountId);

            if (string.IsNullOrEmpty(model.Provider) && string.IsNullOrEmpty(model.ProviderApp))
            {
                var customEmailDomainObj = _customDomainEmailConfigService.GetByUserAndEmail(userId, model.SelectedEmail);
                if (customEmailDomainObj != null)
                {
                    using (var client = new ImapClient())
                    {
                        var options = SecureSocketOptions.StartTlsWhenAvailable;
                        options = SecureSocketOptions.SslOnConnect;

                        #region Host, Port, UserName, Passwrod with different host

                        string Host = customEmailDomainObj.IMAPHost;
                        int Port = customEmailDomainObj.IMAPPort.Value;
                        string UserName = customEmailDomainObj.Email;
                        string Password = customEmailDomainObj.Password;

                        #endregion

                        client.CheckCertificateRevocation = false;
                        client.ServerCertificateValidationCallback = (s, c, ch, e) => true;

                        client.Connect(Host, Port, options);

                        client.Authenticate(UserName, Password);

                        //Check IsConnected condition
                        if (client.IsConnected)
                        {
                            // The Inbox folder is always available on all IMAP servers...

                            var personal = client.GetFolder(client.PersonalNamespaces[0]);
                            var folderList = personal.GetSubfolders(false);
                            var foldeObj = folderList.Where(t => t.Name.ToLower().Contains("draft")).FirstOrDefault();
                            // var drafts = client.GetFolder(SpecialFolder.Drafts);
                            var drafts = client.GetFolder(foldeObj.Name);
                            drafts.Open(FolderAccess.ReadWrite);

                            MailMessage mail = new MailMessage();

                            mail.Subject = model.Subject;
                            mail.Body = model.Body;
                            mail.IsBodyHtml = true;
                            if (model.To.Count > 0 && !model.IsSendMassEmail)
                            {
                                foreach (var ToEmail in model.To)
                                {
                                    mail.To.Add(ToEmail);
                                }
                            }


                            if (model.Cc.Count > 0)
                                mail.CC.Add(string.Join(',', model.Cc).Trim(','));

                            if (model.Bcc.Count > 0)
                                mail.Bcc.Add(string.Join(',', model.Cc).Trim(','));

                            if (files != null)
                            {
                                foreach (var item in files)
                                {
                                    if (item.Length > 0)
                                    {
                                        using (var ms = new MemoryStream())
                                        {
                                            item.CopyTo(ms);
                                            Attachment objAttachment = new Attachment(new MemoryStream(ms.ToArray()), item.FileName);//, string mediaType
                                            if (objAttachment != null)
                                                mail.Attachments.Add(objAttachment);
                                        }
                                    }
                                }
                            }

                            if (model.FileIds != null)
                            {
                                foreach (var item in model.FileIds)
                                {
                                    var file = new FileDto();
                                    var dPath = _hostingEnvironment.WebRootPath + "\\Uploads\\resized";
                                    var fPath = dPath + "\\" + "file-not-found.jpg";
                                    if (item > 0)
                                    {
                                        byte[] imageByte = fileUpload.DownloadFile(item, Convert.ToBoolean(FileUploadSettings.FileUpload), FileUploadSettings.Container, ref file); // FileUploadSettings.FileUpload
                                        Stream streamArray = new MemoryStream(imageByte.ToArray());
                                        Attachment objAttachment = new Attachment(streamArray, file.FileName + "." + file.Extention);
                                        if (objAttachment != null)
                                            mail.Attachments.Add(objAttachment);
                                    }
                                }
                            }

                            if (model.QuickEmailAttachments != null)
                            {
                                Byte[] imageByte = new Byte[0];
                                var dPath = _hostingEnvironment.WebRootPath + "\\AttachEmailPicture";
                                foreach (var item in model.QuickEmailAttachments)
                                {
                                    if (System.IO.File.Exists(dPath + "\\" + item.FileName))
                                    {
                                        imageByte = System.IO.File.ReadAllBytes(dPath + "\\" + item.FileName);
                                        Stream streamArray = new MemoryStream(imageByte.ToArray());
                                        Attachment objAttachment = new Attachment(streamArray, item.OrignalFileName);
                                        if (objAttachment != null)
                                            mail.Attachments.Add(objAttachment);
                                    }
                                }

                            }

                            MimeKit.MimeMessage mimeMessage = MimeKit.MimeMessage.CreateFromMailMessage(mail);
                            drafts.Append(mimeMessage);

                            client.Disconnect(true);
                        }
                        model.IsValid = true;
                        return model;
                    }
                }
                model.IsValid = false;
                model.ErrorMessage = CommonMessage.ErrorOccurredEmailSend;
                return model;
            }

            var intproviderApp = _intProviderAppService.GetIntProviderApp(model.ProviderApp);

            // var intAppSecretObj = _intProviderAppSecretService.GetByUserAndEmail(userId, model.SelectedEmail);
            var intAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUserAndEmail(userId, model.SelectedEmail, intproviderApp.Id);


            if (intproviderApp != null && intAppSecretObj != null)
            {
                mailTokenDto.refresh_token = intAppSecretObj.Refresh_Token;
                mailTokenDto.access_token = intAppSecretObj.Access_Token;
                // mailTokenDto.code = model.Code;
                mailTokenDto.ProviderApp = model.Provider;
                mailTokenDto.UserId = userId;
                intProviderAppSecretObj.Refresh_Token = intAppSecretObj.Refresh_Token;
                intProviderAppSecretObj.Access_Token = intAppSecretObj.Access_Token;
                intProviderAppSecretObj.Email = intAppSecretObj.Email;
            }

            switch (intproviderApp.Name)
            {
                case "Gmail":
                    DraftMessage draftMessage = new DraftMessage();
                    Message message1 = new Message();
                    message1.raw = "";
                    message1.labelIds = new string[] { "DRAFT" };
                    message1.snippet = model.Body;
                    message1.payload = new Payload();
                    message1.payload.headers = new List<Header>();
                    if (!string.IsNullOrEmpty(model.Subject))
                    {
                        Header SubjectHeaderObj = new Header();
                        SubjectHeaderObj.name = "Subject";
                        SubjectHeaderObj.value = model.Subject;
                        message1.payload.headers.Add(SubjectHeaderObj);
                    }
                    if (!string.IsNullOrEmpty(model.SelectedEmail))
                    {
                        Header FromHeaderObj = new Header();
                        FromHeaderObj.name = "From";
                        FromHeaderObj.value = model.SelectedEmail;
                        message1.payload.headers.Add(FromHeaderObj);
                    }
                    if (model.To.Count() > 0)
                    {
                        Header ToHeaderObj = new Header();
                        for (int i = 0; i < model.To.Count(); i++)
                        {
                            var item = model.To[i];
                            if (i == 0)
                            {
                                ToHeaderObj.value = item;
                            }
                            else
                            {
                                ToHeaderObj.value = ToHeaderObj.value + ", " + item;
                            }

                        }
                        ToHeaderObj.name = "To";
                        message1.payload.headers.Add(ToHeaderObj);
                    }
                    // draftMessage.message = message1;
                    // draftMessage.message.raw = "";
                    //         (mimeMessage.ToString());
                    var message = new GmailCompose();
                    MailMessage mail = new MailMessage();

                    mail.Subject = model.Subject;
                    mail.Body = model.Body;
                    mail.IsBodyHtml = true;
                    if (model.To.Count > 0 && !model.IsSendMassEmail)
                    {
                        foreach (var ToEmail in model.To)
                        {
                            mail.To.Add(ToEmail);
                        }
                    }


                    if (model.Cc.Count > 0)
                        mail.CC.Add(string.Join(',', model.Cc).Trim(','));

                    if (model.Bcc.Count > 0)
                        mail.Bcc.Add(string.Join(',', model.Cc).Trim(','));

                    if (files != null)
                    {
                        foreach (var item in files)
                        {
                            if (item.Length > 0)
                            {
                                using (var ms = new MemoryStream())
                                {
                                    item.CopyTo(ms);
                                    Attachment objAttachment = new Attachment(new MemoryStream(ms.ToArray()), item.FileName);//, string mediaType
                                    if (objAttachment != null)
                                        mail.Attachments.Add(objAttachment);
                                }
                            }
                        }
                    }

                    if (model.FileIds != null)
                    {
                        foreach (var item in model.FileIds)
                        {
                            var file = new FileDto();
                            var dPath = _hostingEnvironment.WebRootPath + "\\Uploads\\resized";
                            var fPath = dPath + "\\" + "file-not-found.jpg";
                            if (item > 0)
                            {
                                byte[] imageByte = fileUpload.DownloadFile(item, Convert.ToBoolean(FileUploadSettings.FileUpload), FileUploadSettings.Container, ref file); // FileUploadSettings.FileUpload
                                Stream streamArray = new MemoryStream(imageByte.ToArray());
                                Attachment objAttachment = new Attachment(streamArray, file.FileName + "." + file.Extention);
                                if (objAttachment != null)
                                    mail.Attachments.Add(objAttachment);
                            }
                        }
                    }

                    if (model.QuickEmailAttachments != null)
                    {
                        Byte[] imageByte = new Byte[0];
                        var dPath = _hostingEnvironment.WebRootPath + "\\AttachEmailPicture";
                        foreach (var item in model.QuickEmailAttachments)
                        {
                            if (System.IO.File.Exists(dPath + "\\" + item.FileName))
                            {
                                imageByte = System.IO.File.ReadAllBytes(dPath + "\\" + item.FileName);
                                Stream streamArray = new MemoryStream(imageByte.ToArray());
                                Attachment objAttachment = new Attachment(streamArray, item.OrignalFileName);
                                if (objAttachment != null)
                                    mail.Attachments.Add(objAttachment);
                            }
                        }

                    }

                    MimeKit.MimeMessage mimeMessage = MimeKit.MimeMessage.CreateFromMailMessage(mail);
                    message1.raw = DataUtility.Base64UrlEncode
                       (mimeMessage.ToString());

                    draftMessage.message = message1;


                    var myContent1 = JsonConvert.SerializeObject(draftMessage);
                    var ChangeAttach1 = myContent1.Replace("odatatype", "@odata.type");
                    var buffer1 = Encoding.UTF8.GetBytes(ChangeAttach1);
                    var byteContent1 = new ByteArrayContent(buffer1);
                    byteContent1.Headers.ContentType = new MediaTypeHeaderValue("application/json");


                    await SetGmailToken();
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    //var uri = DataUtility.GmailMessageSend;
                    //if (model.FileIds.Count > 0)
                    //{
                    //    uri = "https://www.googleapis.com/upload/gmail/v1/users/me/messages/send?uploadType=multipart";
                    //    client.DefaultRequestHeaders.Add("ContentType", "multipart/related;");
                    //    client.DefaultRequestHeaders.Add("ContentLength", "2147483647");
                    //}
                    //var response = await client.PostAsync(uri, new StringContent(JsonConvert.SerializeObject(message)));

                    //var response = await client.PostAsync(DataUtility.GmailMessageSend, new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json"));

                    // For recipients receive email as if it was only sent to them
                    var response = new HttpResponseMessage();

                    // if (model.IsSendMassEmail)
                    // {
                    //     mail.To.Add(ToEmail);
                    //     MimeKit.MimeMessage mimeMessage = MimeKit.MimeMessage.CreateFromMailMessage(mail);
                    //     message.raw = DataUtility.Base64UrlEncode
                    //         (mimeMessage.ToString());
                    //     response = await client.PostAsync(DataUtility.GmailDraftList, new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json"));

                    // }
                    // else
                    // {
                    //     MimeKit.MimeMessage mimeMessage = MimeKit.MimeMessage.CreateFromMailMessage(mail);
                    //     message.raw = DataUtility.Base64UrlEncode
                    //         (mimeMessage.ToString());
                    //     response = await client.PostAsync(DataUtility.GmailDraftList, new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json"));
                    // }

                    response = await client.PostAsync(DataUtility.GmailDraftList, byteContent1); // DataUtility.Office3

                    //var response = await client.PostAsync(SystemSettingService.Dictionary["GMLSM"], new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json"));
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var stream = await response.Content.ReadAsStreamAsync();
                        var serializer = new DataContractJsonSerializer(typeof(GmailSent));
                        model.MailSent = (GmailSent)serializer.ReadObject(stream);

                        if (string.IsNullOrEmpty(model.MailSent.id))
                        {
                            model.IsValid = false;
                            model.ErrorMessage = CommonMessage.ErrorOccurredEmailSend;
                            return model;
                        }
                    }
                    else
                    {
                        var stream = await response.Content.ReadAsStreamAsync();
                        var serializer = new DataContractJsonSerializer(typeof(GmailError));
                        model.GmailError = (GmailError)serializer.ReadObject(stream);

                        if (model.GmailError != null && !string.IsNullOrEmpty(model.GmailError.error.message))
                        {
                            model.IsValid = false;
                            model.ErrorMessage = model.GmailError.error.code + "-" + model.GmailError.error.message;
                        }

                        return model;
                    }

                    break;

                case "Office 365":
                case "Outlook":

                    Office365Message office365Message = new Office365Message();
                    Office365Body objBody = new Office365Body();
                    List<Torecipient> objToList = new List<Torecipient>();
                    List<Ccrecipient> objCcList = new List<Ccrecipient>();
                    List<Bccrecipient> objBccList = new List<Bccrecipient>();
                    List<MailAttachments> objAttachmentList = new List<MailAttachments>();
                    CreateMailMessage createMessageBody = new CreateMailMessage();

                    office365Message.subject = model.Subject;
                    objBody.contentType = "Html";
                    objBody.content = model.Body;
                    office365Message.body = objBody;

                    if (model.To.Count() > 0)
                    {
                        Torecipient objTo = new Torecipient();

                        foreach (var ToEmail in model.To)
                        {
                            Emailaddress objEmail = new Emailaddress();
                            objEmail.address = ToEmail;
                            objEmail.name = ToEmail;
                            objTo.emailAddress = objEmail;
                            objToList.Add(objTo);
                        }

                    }
                    office365Message.toRecipients = objToList.ToArray();

                    foreach (var item in model.Cc)
                    {
                        Ccrecipient objCc = new Ccrecipient();
                        Emailaddress objEmail = new Emailaddress();
                        objEmail.address = item.Contains("<") ? item.Split("<")[1].Trim('>').Trim() : item;
                        objEmail.name = item.Contains("<") ? item.Split("<")[0].Trim() : item;
                        objCc.emailAddress = objEmail;

                        objCcList.Add(objCc);
                    }
                    office365Message.ccRecipients = objCcList.ToArray();

                    foreach (var item in model.Bcc)
                    {
                        Bccrecipient objBcc = new Bccrecipient();
                        Emailaddress objEmail = new Emailaddress();
                        objEmail.address = item.Contains("<") ? item.Split("<")[1].Trim('>').Trim() : item;
                        objEmail.name = item.Contains("<") ? item.Split("<")[0].Trim() : item;
                        objBcc.emailAddress = objEmail;

                        objBccList.Add(objBcc);
                    }
                    office365Message.bccRecipients = objBccList.ToArray();

                    //// Attachment
                    if (files != null)
                    {
                        foreach (var item in files)
                        {
                            if (item.Length > 0)
                            {
                                using (var ms = new MemoryStream())
                                {
                                    item.CopyTo(ms);
                                    MailAttachments objAttachments = new MailAttachments();
                                    string base64 = Convert.ToBase64String(ms.ToArray());
                                    objAttachments.odatatype = "#microsoft.graph.fileAttachment";
                                    objAttachments.contentBytes = base64;
                                    //objAttachments.contentType = "";
                                    objAttachments.name = item.FileName;
                                    objAttachmentList.Add(objAttachments);
                                }
                            }
                        }
                    }

                    if (model.FileIds != null)
                    {
                        foreach (var item in model.FileIds)
                        {
                            var file = new FileDto();
                            var dPath = _hostingEnvironment.WebRootPath + "\\Uploads\\resized";
                            var fPath = dPath + "\\" + "file-not-found.jpg";
                            if (item > 0)
                            {
                                var imageByte = fileUpload.DownloadFile(item, Convert.ToBoolean(FileUploadSettings.FileUpload), FileUploadSettings.Container, ref file);    // FileUploadSettings.FileUpload
                                MailAttachments objAttachments = new MailAttachments();
                                string base64 = Convert.ToBase64String(imageByte);
                                objAttachments.odatatype = "#microsoft.graph.fileAttachment";
                                objAttachments.contentBytes = base64;
                                objAttachments.contentType = file.FileType;
                                objAttachments.name = file.FileName + "." + file.Extention;
                                objAttachmentList.Add(objAttachments);
                            }
                        }
                    }

                    if (model.QuickEmailAttachments != null)
                    {
                        Byte[] imageByte = new Byte[0];
                        var dPath = _hostingEnvironment.WebRootPath + "\\AttachEmailPicture";
                        foreach (var item in model.QuickEmailAttachments)
                        {
                            if (System.IO.File.Exists(dPath + "\\" + item.FileName))
                            {
                                imageByte = System.IO.File.ReadAllBytes(dPath + "\\" + item.FileName);
                            }
                            else
                            {
                                imageByte = System.IO.File.ReadAllBytes(dPath + "\\" + "file-not-found.jpg");
                            }

                            MailAttachments objAttachments = new MailAttachments();
                            string base64 = Convert.ToBase64String(imageByte);
                            objAttachments.odatatype = "#microsoft.graph.fileAttachment";
                            objAttachments.contentBytes = base64;
                            //objAttachments.contentType = "image/jpeg";
                            objAttachments.name = item.OrignalFileName;
                            objAttachmentList.Add(objAttachments);
                        }
                    }

                    office365Message.attachments = objAttachmentList.ToArray();

                    //// Create Message
                    //createMessageBody.message = office365Message;
                    //createMessageBody.saveToSentItems = "true";

                    await SetOffice365Token();
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    // For recipients receive email as if it was only sent to them
                    var response365 = new HttpResponseMessage();
                    if (model.IsSendMassEmail)
                    {

                        objToList = new List<Torecipient>();
                        Torecipient objTo = new Torecipient();
                        foreach (var ToEmail in model.To)
                        {
                            Emailaddress objEmail = new Emailaddress();
                            objEmail.address = ToEmail;
                            objEmail.name = ToEmail;
                            objTo.emailAddress = objEmail;
                            objToList.Add(objTo);
                        }
                        // Emailaddress objEmail = new Emailaddress();
                        // objEmail.address = ToEmail;
                        // objEmail.name = ToEmail;
                        // objTo.emailAddress = objEmail;
                        // objToList.Add(objTo);
                        office365Message.toRecipients = objToList.ToArray();

                        // Create Message
                        createMessageBody.message = office365Message;
                        createMessageBody.saveToSentItems = "true";

                        var myContent = JsonConvert.SerializeObject(createMessageBody);
                        var ChangeAttach = myContent.Replace("odatatype", "@odata.type");
                        var buffer = Encoding.UTF8.GetBytes(ChangeAttach);
                        var byteContent = new ByteArrayContent(buffer);
                        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                        response365 = await client.PostAsync(DataUtility.Office365SendEmail, byteContent); // DataUtility.Office365SendEmail
                    }
                    else
                    {
                        // Create Message
                        createMessageBody.message = office365Message;
                        createMessageBody.saveToSentItems = "true";

                        var myContent = JsonConvert.SerializeObject(createMessageBody);
                        var ChangeAttach = myContent.Replace("odatatype", "@odata.type");
                        var buffer = Encoding.UTF8.GetBytes(ChangeAttach);
                        var byteContent = new ByteArrayContent(buffer);
                        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                        response365 = await client.PostAsync(DataUtility.Office365SendEmail, byteContent); // DataUtility.Office365SendEmail
                    }

                    //var response365 = await client.PostAsync(SystemSettingService.Dictionary["OFSDM"], byteContent); // DataUtility.Office365SendEmail
                    if (response365.StatusCode != HttpStatusCode.Accepted)
                    {
                        model.IsValid = false;
                        model.ErrorMessage = CommonMessage.ErrorOccurredEmailSend;
                        return model;
                    }
                    break;
            }

            model.IsValid = true;
            model.ErrorMessage = CommonMessage.EmailSent;

            return model;
        }



        // public async Task<ComposeEmail> CreateEmailBody(int userId, ComposeEmail model, IFormFileCollection files, string ToEmail)
        public async Task<ComposeEmail> CreateEmailBody(int userId, ComposeEmail model, IFormFile[] files, string ToEmail)
        {
            user = _userService.GetUserById(userId);
            if (user == null)
            {
                model.IsValid = false;
                model.ErrorMessage = CommonMessage.UnAuthorizedUser;
                return model;
            }

            // userEmail = _userEmailService.GetUserEmailById(model.UserEmail.UserEmailId);
            // if (userEmail == null)
            // {
            //     model.IsValid = false;
            //     model.ErrorMessage = CommonMessage.UnAuthorizedUser;
            // }

            // var emailAccount = _emailAccountService.GetEmailAccountById(model.UserEmail.AccountId);

            var intproviderApp = _intProviderAppService.GetIntProviderApp(model.ProviderApp);

            // var intAppSecretObj = _intProviderAppSecretService.GetByUserAndEmail(userId, model.SelectedEmail);
            var intAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUserAndEmail(userId, model.SelectedEmail, intproviderApp.Id);


            if (intproviderApp != null && intAppSecretObj != null)
            {
                mailTokenDto.refresh_token = intAppSecretObj.Refresh_Token;
                mailTokenDto.access_token = intAppSecretObj.Access_Token;
                // mailTokenDto.code = model.Code;
                mailTokenDto.ProviderApp = model.Provider;
                mailTokenDto.UserId = userId;
                intProviderAppSecretObj.Refresh_Token = intAppSecretObj.Refresh_Token;
                intProviderAppSecretObj.Access_Token = intAppSecretObj.Access_Token;
                intProviderAppSecretObj.Email = intAppSecretObj.Email;
            }

            switch (intproviderApp.Name)
            {
                case "Gmail":
                    var message = new GmailCompose();
                    MailMessage mail = new MailMessage();

                    mail.Subject = model.Subject;
                    mail.Body = model.Body;
                    mail.IsBodyHtml = true;
                    if (model.To.Count > 0 && !model.IsSendMassEmail)
                        mail.To.Add(ToEmail);

                    if (model.Cc.Count > 0)
                        mail.CC.Add(string.Join(',', model.Cc).Trim(','));

                    if (model.Bcc.Count > 0)
                        mail.Bcc.Add(string.Join(',', model.Cc).Trim(','));

                    if (files != null)
                    {
                        foreach (var item in files)
                        {
                            if (item.Length > 0)
                            {
                                using (var ms = new MemoryStream())
                                {
                                    item.CopyTo(ms);
                                    Attachment objAttachment = new Attachment(new MemoryStream(ms.ToArray()), item.FileName);//, string mediaType
                                    if (objAttachment != null)
                                        mail.Attachments.Add(objAttachment);
                                }
                            }
                        }
                    }

                    if (model.FileIds != null)
                    {
                        foreach (var item in model.FileIds)
                        {
                            var file = new FileDto();
                            var dPath = _hostingEnvironment.WebRootPath + "\\Uploads\\resized";
                            var fPath = dPath + "\\" + "file-not-found.jpg";
                            if (item > 0)
                            {
                                byte[] imageByte = fileUpload.DownloadFile(item, Convert.ToBoolean(FileUploadSettings.FileUpload), FileUploadSettings.Container, ref file); // FileUploadSettings.FileUpload
                                Stream streamArray = new MemoryStream(imageByte.ToArray());
                                Attachment objAttachment = new Attachment(streamArray, file.FileName + "." + file.Extention);
                                if (objAttachment != null)
                                    mail.Attachments.Add(objAttachment);
                            }
                        }
                    }

                    if (model.QuickEmailAttachments != null)
                    {
                        Byte[] imageByte = new Byte[0];
                        var dPath = _hostingEnvironment.WebRootPath + "\\AttachEmailPicture";
                        foreach (var item in model.QuickEmailAttachments)
                        {
                            if (System.IO.File.Exists(dPath + "\\" + item.FileName))
                            {
                                imageByte = System.IO.File.ReadAllBytes(dPath + "\\" + item.FileName);
                                Stream streamArray = new MemoryStream(imageByte.ToArray());
                                Attachment objAttachment = new Attachment(streamArray, item.OrignalFileName);
                                if (objAttachment != null)
                                    mail.Attachments.Add(objAttachment);
                            }
                        }

                    }

                    //MimeKit.MimeMessage mimeMessage = MimeKit.MimeMessage.CreateFromMailMessage(mail);
                    //message.raw = Utility.Base64UrlEncode
                    //    (mimeMessage.ToString());

                    await SetGmailToken();
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    //var uri = DataUtility.GmailMessageSend;
                    //if (model.FileIds.Count > 0)
                    //{
                    //    uri = "https://www.googleapis.com/upload/gmail/v1/users/me/messages/send?uploadType=multipart";
                    //    client.DefaultRequestHeaders.Add("ContentType", "multipart/related;");
                    //    client.DefaultRequestHeaders.Add("ContentLength", "2147483647");
                    //}
                    //var response = await client.PostAsync(uri, new StringContent(JsonConvert.SerializeObject(message)));

                    //var response = await client.PostAsync(DataUtility.GmailMessageSend, new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json"));

                    // For recipients receive email as if it was only sent to them
                    var response = new HttpResponseMessage();
                    if (model.IsSendMassEmail)
                    {
                        mail.To.Add(ToEmail);
                        MimeKit.MimeMessage mimeMessage = MimeKit.MimeMessage.CreateFromMailMessage(mail);
                        message.raw = DataUtility.Base64UrlEncode
                            (mimeMessage.ToString());
                        response = await client.PostAsync(DataUtility.GmailMessageSend, new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json"));

                    }
                    else
                    {
                        MimeKit.MimeMessage mimeMessage = MimeKit.MimeMessage.CreateFromMailMessage(mail);
                        message.raw = DataUtility.Base64UrlEncode
                            (mimeMessage.ToString());
                        response = await client.PostAsync(DataUtility.GmailMessageSend, new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json"));
                    }

                    //var response = await client.PostAsync(SystemSettingService.Dictionary["GMLSM"], new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json"));
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var stream = await response.Content.ReadAsStreamAsync();
                        var serializer = new DataContractJsonSerializer(typeof(GmailSent));
                        model.MailSent = (GmailSent)serializer.ReadObject(stream);

                        if (string.IsNullOrEmpty(model.MailSent.id))
                        {
                            model.IsValid = false;
                            model.ErrorMessage = CommonMessage.ErrorOccurredEmailSend;
                            return model;
                        }
                    }
                    else
                    {
                        var stream = await response.Content.ReadAsStreamAsync();
                        var serializer = new DataContractJsonSerializer(typeof(GmailError));
                        model.GmailError = (GmailError)serializer.ReadObject(stream);

                        if (model.GmailError != null && !string.IsNullOrEmpty(model.GmailError.error.message))
                        {
                            model.IsValid = false;
                            model.ErrorMessage = model.GmailError.error.code + "-" + model.GmailError.error.message;
                        }

                        return model;
                    }

                    break;

                case "Office 365":
                case "Outlook":

                    Office365Message office365Message = new Office365Message();
                    Office365Body objBody = new Office365Body();
                    List<Torecipient> objToList = new List<Torecipient>();
                    List<Ccrecipient> objCcList = new List<Ccrecipient>();
                    List<Bccrecipient> objBccList = new List<Bccrecipient>();
                    List<MailAttachments> objAttachmentList = new List<MailAttachments>();
                    CreateMailMessage createMessageBody = new CreateMailMessage();

                    office365Message.subject = model.Subject;
                    objBody.contentType = "Html";
                    objBody.content = model.Body;
                    office365Message.body = objBody;

                    if (ToEmail.Length > 0)
                    {
                        Torecipient objTo = new Torecipient();
                        Emailaddress objEmail = new Emailaddress();
                        objEmail.address = ToEmail;
                        objEmail.name = ToEmail;
                        objTo.emailAddress = objEmail;
                        objToList.Add(objTo);
                    }
                    office365Message.toRecipients = objToList.ToArray();

                    foreach (var item in model.Cc)
                    {
                        Ccrecipient objCc = new Ccrecipient();
                        Emailaddress objEmail = new Emailaddress();
                        objEmail.address = item.Contains("<") ? item.Split("<")[1].Trim('>').Trim() : item;
                        objEmail.name = item.Contains("<") ? item.Split("<")[0].Trim() : item;
                        objCc.emailAddress = objEmail;

                        objCcList.Add(objCc);
                    }
                    office365Message.ccRecipients = objCcList.ToArray();

                    foreach (var item in model.Bcc)
                    {
                        Bccrecipient objBcc = new Bccrecipient();
                        Emailaddress objEmail = new Emailaddress();
                        objEmail.address = item.Contains("<") ? item.Split("<")[1].Trim('>').Trim() : item;
                        objEmail.name = item.Contains("<") ? item.Split("<")[0].Trim() : item;
                        objBcc.emailAddress = objEmail;

                        objBccList.Add(objBcc);
                    }
                    office365Message.bccRecipients = objBccList.ToArray();

                    //// Attachment
                    if (files != null)
                    {
                        foreach (var item in files)
                        {
                            if (item.Length > 0)
                            {
                                using (var ms = new MemoryStream())
                                {
                                    item.CopyTo(ms);
                                    MailAttachments objAttachments = new MailAttachments();
                                    string base64 = Convert.ToBase64String(ms.ToArray());
                                    objAttachments.odatatype = "#microsoft.graph.fileAttachment";
                                    objAttachments.contentBytes = base64;
                                    //objAttachments.contentType = "";
                                    objAttachments.name = item.FileName;
                                    objAttachmentList.Add(objAttachments);
                                }
                            }
                        }
                    }

                    if (model.FileIds != null)
                    {
                        foreach (var item in model.FileIds)
                        {
                            var file = new FileDto();
                            var dPath = _hostingEnvironment.WebRootPath + "\\Uploads\\resized";
                            var fPath = dPath + "\\" + "file-not-found.jpg";
                            if (item > 0)
                            {
                                var imageByte = fileUpload.DownloadFile(item, Convert.ToBoolean(FileUploadSettings.FileUpload), FileUploadSettings.Container, ref file);    // FileUploadSettings.FileUpload
                                MailAttachments objAttachments = new MailAttachments();
                                string base64 = Convert.ToBase64String(imageByte);
                                objAttachments.odatatype = "#microsoft.graph.fileAttachment";
                                objAttachments.contentBytes = base64;
                                objAttachments.contentType = file.FileType;
                                objAttachments.name = file.FileName + "." + file.Extention;
                                objAttachmentList.Add(objAttachments);
                            }
                        }
                    }

                    if (model.QuickEmailAttachments != null)
                    {
                        Byte[] imageByte = new Byte[0];
                        var dPath = _hostingEnvironment.WebRootPath + "\\AttachEmailPicture";
                        foreach (var item in model.QuickEmailAttachments)
                        {
                            if (System.IO.File.Exists(dPath + "\\" + item.FileName))
                            {
                                imageByte = System.IO.File.ReadAllBytes(dPath + "\\" + item.FileName);
                            }
                            else
                            {
                                imageByte = System.IO.File.ReadAllBytes(dPath + "\\" + "file-not-found.jpg");
                            }

                            MailAttachments objAttachments = new MailAttachments();
                            string base64 = Convert.ToBase64String(imageByte);
                            objAttachments.odatatype = "#microsoft.graph.fileAttachment";
                            objAttachments.contentBytes = base64;
                            //objAttachments.contentType = "image/jpeg";
                            objAttachments.name = item.OrignalFileName;
                            objAttachmentList.Add(objAttachments);
                        }
                    }

                    office365Message.attachments = objAttachmentList.ToArray();

                    //// Create Message
                    //createMessageBody.message = office365Message;
                    //createMessageBody.saveToSentItems = "true";

                    await SetOffice365Token();
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    // For recipients receive email as if it was only sent to them
                    var response365 = new HttpResponseMessage();
                    if (model.IsSendMassEmail)
                    {

                        objToList = new List<Torecipient>();
                        Torecipient objTo = new Torecipient();
                        Emailaddress objEmail = new Emailaddress();
                        objEmail.address = ToEmail;
                        objEmail.name = ToEmail;
                        objTo.emailAddress = objEmail;
                        objToList.Add(objTo);
                        office365Message.toRecipients = objToList.ToArray();

                        // Create Message
                        createMessageBody.message = office365Message;
                        createMessageBody.saveToSentItems = "true";

                        var myContent = JsonConvert.SerializeObject(createMessageBody);
                        var ChangeAttach = myContent.Replace("odatatype", "@odata.type");
                        var buffer = Encoding.UTF8.GetBytes(ChangeAttach);
                        var byteContent = new ByteArrayContent(buffer);
                        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                        response365 = await client.PostAsync(DataUtility.Office365SendEmail, byteContent); // DataUtility.Office365SendEmail
                    }
                    else
                    {
                        // Create Message
                        createMessageBody.message = office365Message;
                        createMessageBody.saveToSentItems = "true";

                        var myContent = JsonConvert.SerializeObject(createMessageBody);
                        var ChangeAttach = myContent.Replace("odatatype", "@odata.type");
                        var buffer = Encoding.UTF8.GetBytes(ChangeAttach);
                        var byteContent = new ByteArrayContent(buffer);
                        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                        response365 = await client.PostAsync(DataUtility.Office365SendEmail, byteContent); // DataUtility.Office365SendEmail
                    }

                    //var response365 = await client.PostAsync(SystemSettingService.Dictionary["OFSDM"], byteContent); // DataUtility.Office365SendEmail
                    if (response365.StatusCode != HttpStatusCode.Accepted)
                    {
                        model.IsValid = false;
                        model.ErrorMessage = CommonMessage.ErrorOccurredEmailSend;
                        return model;
                    }
                    break;
            }

            model.IsValid = true;
            model.ErrorMessage = CommonMessage.EmailSent;

            return model;
        }


        // public async Task<ComposeEmail> SendEmail(int userId, ComposeEmail model, IFormFileCollection files)
        public async Task<ComposeEmail1> SendEmail(int userId, ComposeEmail1 model, IFormFile[] files)
        {
            user = _userService.GetUserById(userId);
            if (user == null)
            {
                model.IsValid = false;
                model.ErrorMessage = CommonMessage.UnAuthorizedUser;
                return model;
            }
            if (string.IsNullOrEmpty(model.Provider) && string.IsNullOrEmpty(model.ProviderApp))
            {
                var customEmailDomainObj = _customDomainEmailConfigService.GetByUserAndEmail(userId, model.SelectedEmail);
                if (customEmailDomainObj != null)
                {
                    var Host = customEmailDomainObj.SMTPHost;
                    var Port = customEmailDomainObj.SMTPPort.Value;
                    var Password = customEmailDomainObj.Password;
                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress(model.SelectedEmail);
                    // mailMessage.To.Add("oneclapp.2021@gmail.com");
                    // mailMessage.To.Add("shraddha.prajapati@techavidus.com");
                    mailMessage.Subject = model.Subject;
                    mailMessage.Body = model.Body;
                    if (model.Cc.Count() > 0)
                    {
                        foreach (var item in model.Cc)
                        {
                            mailMessage.CC.Add(item);
                        }
                    }
                    if (model.Bcc.Count() > 0)
                    {
                        foreach (var item in model.Bcc)
                        {
                            mailMessage.Bcc.Add(item);
                        }
                    }
                    if (model.To.Count() > 0)
                    {
                        foreach (var item in model.To)
                        {
                            mailMessage.To.Add(item);
                        }
                    }
                    // IFormFile[] files = new IFormFile[] { };
                    if (model.FileList != null)
                    {
                        foreach (var item in model.FileList)
                        {
                            if (item.Length > 0)
                            {
                                using (var ms = new MemoryStream())
                                {
                                    item.CopyTo(ms);
                                    Attachment objAttachment = new Attachment(new MemoryStream(ms.ToArray()), item.FileName);//, string mediaType
                                    if (objAttachment != null)
                                        mailMessage.Attachments.Add(objAttachment);
                                }
                            }
                        }
                    }


                    using (SmtpClient client = new SmtpClient())
                    {
                        mailMessage.IsBodyHtml = true;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.UseDefaultCredentials = false;
                        client.Host = Host;
                        client.Port = Port;
                        client.EnableSsl = true;
                        client.Credentials = new NetworkCredential
                        {
                            UserName = model.SelectedEmail,
                            Password = Password
                        };
                        client.Send(mailMessage);
                    }
                    // SmtpClient smtpClient = new SmtpClient(Host);
                    // smtpClient.Port = Port;
                    // smtpClient.Credentials = new NetworkCredential(model.SelectedEmail, Password);
                    // smtpClient.EnableSsl = true;
                    // smtpClient.Send(mailMessage);
                    return model;
                }

            }

            // userEmail = _userEmailService.GetUserEmailById(model.UserEmail.UserEmailId);
            // if (userEmail == null)
            // {
            //     model.IsValid = false;
            //     model.ErrorMessage = CommonMessage.UnAuthorizedUser;
            // }

            // var emailAccount = _emailAccountService.GetEmailAccountById(model.UserEmail.AccountId);
            var intproviderApp = _intProviderAppService.GetIntProviderApp(model.ProviderApp);

            // var intAppSecretObj = _intProviderAppSecretService.GetByUserAndEmail(userId, model.SelectedEmail);
            var intAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUserAndEmail(userId, model.SelectedEmail, intproviderApp.Id);


            if (intproviderApp != null && intAppSecretObj != null)
            {
                mailTokenDto.refresh_token = intAppSecretObj.Refresh_Token;
                mailTokenDto.access_token = intAppSecretObj.Access_Token;
                // mailTokenDto.code = model.Code;
                mailTokenDto.ProviderApp = model.Provider;
                mailTokenDto.UserId = userId;
                intProviderAppSecretObj.Refresh_Token = intAppSecretObj.Refresh_Token;
                intProviderAppSecretObj.Access_Token = intAppSecretObj.Access_Token;
                intProviderAppSecretObj.Email = intAppSecretObj.Email;
            }
            switch (intproviderApp.Name)
            {
                case "Gmail":
                    var message = new GmailCompose();
                    MailMessage mail = new MailMessage();

                    mail.Subject = model.Subject;
                    mail.Body = model.Body;
                    mail.IsBodyHtml = true;
                    if (model.To.Count > 0 && !model.IsSendMassEmail)
                        mail.To.Add(string.Join(',', model.To).Trim(','));

                    if (model.Cc.Count > 0)
                        mail.CC.Add(string.Join(',', model.Cc).Trim(','));

                    if (model.Bcc.Count > 0)
                        mail.Bcc.Add(string.Join(',', model.Cc).Trim(','));

                    if (files != null)
                    {
                        foreach (var item in files)
                        {
                            if (item.Length > 0)
                            {
                                using (var ms = new MemoryStream())
                                {
                                    item.CopyTo(ms);
                                    Attachment objAttachment = new Attachment(new MemoryStream(ms.ToArray()), item.FileName);//, string mediaType
                                    if (objAttachment != null)
                                        mail.Attachments.Add(objAttachment);
                                }
                            }
                        }
                    }

                    if (model.FileIds != null)
                    {
                        foreach (var item in model.FileIds)
                        {
                            var file = new FileDto();
                            var dPath = _hostingEnvironment.WebRootPath + "\\Uploads\\resized";
                            var fPath = dPath + "\\" + "file-not-found.jpg";
                            if (item > 0)
                            {
                                byte[] imageByte = fileUpload.DownloadFile(item, Convert.ToBoolean(FileUploadSettings.FileUpload), FileUploadSettings.Container, ref file); // FileUploadSettings.FileUpload
                                Stream streamArray = new MemoryStream(imageByte.ToArray());
                                Attachment objAttachment = new Attachment(streamArray, file.FileName + "." + file.Extention);
                                if (objAttachment != null)
                                    mail.Attachments.Add(objAttachment);
                            }
                        }
                    }

                    if (model.QuickEmailAttachments != null)
                    {
                        Byte[] imageByte = new Byte[0];
                        var dPath = _hostingEnvironment.WebRootPath + "\\AttachEmailPicture";
                        foreach (var item in model.QuickEmailAttachments)
                        {
                            if (System.IO.File.Exists(dPath + "\\" + item.FileName))
                            {
                                imageByte = System.IO.File.ReadAllBytes(dPath + "\\" + item.FileName);
                                Stream streamArray = new MemoryStream(imageByte.ToArray());
                                Attachment objAttachment = new Attachment(streamArray, item.OrignalFileName);
                                if (objAttachment != null)
                                    mail.Attachments.Add(objAttachment);
                            }
                        }

                    }

                    //MimeKit.MimeMessage mimeMessage = MimeKit.MimeMessage.CreateFromMailMessage(mail);
                    //message.raw = Utility.Base64UrlEncode
                    //    (mimeMessage.ToString());

                    await SetGmailToken();
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    //var uri = DataUtility.GmailMessageSend;
                    //if (model.FileIds.Count > 0)
                    //{
                    //    uri = "https://www.googleapis.com/upload/gmail/v1/users/me/messages/send?uploadType=multipart";
                    //    client.DefaultRequestHeaders.Add("ContentType", "multipart/related;");
                    //    client.DefaultRequestHeaders.Add("ContentLength", "2147483647");
                    //}
                    //var response = await client.PostAsync(uri, new StringContent(JsonConvert.SerializeObject(message)));

                    //var response = await client.PostAsync(DataUtility.GmailMessageSend, new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json"));

                    // For recipients receive email as if it was only sent to them
                    var response = new HttpResponseMessage();
                    if (model.IsSendMassEmail)
                    {
                        int it = 0;
                        foreach (var item in model.To)
                        {
                            if (mail.To.Count > 0) mail.To.RemoveAt(0);
                            mail.To.Add(item);
                            MimeKit.MimeMessage mimeMessage = MimeKit.MimeMessage.CreateFromMailMessage(mail);
                            message.raw = DataUtility.Base64UrlEncode
                                (mimeMessage.ToString());
                            response = await client.PostAsync(DataUtility.GmailMessageSend, new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json"));

                            it++;
                        }
                    }
                    else
                    {
                        MimeKit.MimeMessage mimeMessage = MimeKit.MimeMessage.CreateFromMailMessage(mail);
                        message.raw = DataUtility.Base64UrlEncode
                            (mimeMessage.ToString());
                        response = await client.PostAsync(DataUtility.GmailMessageSend, new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json"));
                    }

                    //var response = await client.PostAsync(SystemSettingService.Dictionary["GMLSM"], new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json"));
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var stream = await response.Content.ReadAsStreamAsync();
                        var serializer = new DataContractJsonSerializer(typeof(GmailSent));
                        model.MailSent = (GmailSent)serializer.ReadObject(stream);

                        if (string.IsNullOrEmpty(model.MailSent.id))
                        {
                            model.IsValid = false;
                            model.ErrorMessage = CommonMessage.ErrorOccurredEmailSend;
                            return model;
                        }
                    }
                    else
                    {
                        var stream = await response.Content.ReadAsStreamAsync();
                        var serializer = new DataContractJsonSerializer(typeof(GmailError));
                        model.GmailError = (GmailError)serializer.ReadObject(stream);

                        if (model.GmailError != null && !string.IsNullOrEmpty(model.GmailError.error.message))
                        {
                            model.IsValid = false;
                            model.ErrorMessage = model.GmailError.error.code + "-" + model.GmailError.error.message;
                        }

                        return model;
                    }

                    break;

                case "Office 365":
                case "Outlook":

                    Office365Message office365Message = new Office365Message();
                    Office365Body objBody = new Office365Body();
                    List<Torecipient> objToList = new List<Torecipient>();
                    List<Ccrecipient> objCcList = new List<Ccrecipient>();
                    List<Bccrecipient> objBccList = new List<Bccrecipient>();
                    List<MailAttachments> objAttachmentList = new List<MailAttachments>();
                    CreateMailMessage createMessageBody = new CreateMailMessage();

                    office365Message.subject = model.Subject;
                    objBody.contentType = "Html";
                    objBody.content = model.Body;
                    office365Message.body = objBody;

                    foreach (var item in model.To)
                    {
                        Torecipient objTo = new Torecipient();
                        Emailaddress objEmail = new Emailaddress();
                        objEmail.address = item.Contains("<") ? item.Split("<")[1].Trim('>').Trim() : item;
                        objEmail.name = item.Contains("<") ? item.Split("<")[0].Trim() : item;
                        objTo.emailAddress = objEmail;
                        objToList.Add(objTo);
                    }
                    office365Message.toRecipients = objToList.ToArray();

                    foreach (var item in model.Cc)
                    {
                        Ccrecipient objCc = new Ccrecipient();
                        Emailaddress objEmail = new Emailaddress();
                        objEmail.address = item.Contains("<") ? item.Split("<")[1].Trim('>').Trim() : item;
                        objEmail.name = item.Contains("<") ? item.Split("<")[0].Trim() : item;
                        objCc.emailAddress = objEmail;

                        objCcList.Add(objCc);
                    }
                    office365Message.ccRecipients = objCcList.ToArray();

                    foreach (var item in model.Bcc)
                    {
                        Bccrecipient objBcc = new Bccrecipient();
                        Emailaddress objEmail = new Emailaddress();
                        objEmail.address = item.Contains("<") ? item.Split("<")[1].Trim('>').Trim() : item;
                        objEmail.name = item.Contains("<") ? item.Split("<")[0].Trim() : item;
                        objBcc.emailAddress = objEmail;

                        objBccList.Add(objBcc);
                    }
                    office365Message.bccRecipients = objBccList.ToArray();

                    //// Attachment
                    if (files != null)
                    {
                        foreach (var item in files)
                        {
                            if (item.Length > 0)
                            {
                                using (var ms = new MemoryStream())
                                {
                                    item.CopyTo(ms);
                                    MailAttachments objAttachments = new MailAttachments();
                                    string base64 = Convert.ToBase64String(ms.ToArray());
                                    objAttachments.odatatype = "#microsoft.graph.fileAttachment";
                                    objAttachments.contentBytes = base64;
                                    //objAttachments.contentType = "";
                                    objAttachments.name = item.FileName;
                                    objAttachmentList.Add(objAttachments);
                                }
                            }
                        }
                    }

                    if (model.FileIds != null)
                    {
                        // foreach (var item in model.FileIds)
                        // {
                        //     var file = new FileDto();
                        //     var dPath = _hostingEnvironment.WebRootPath + "\\Uploads\\resized";
                        //     var fPath = dPath + "\\" + "file-not-found.jpg";
                        //     if (item > 0)
                        //     {
                        //         var imageByte = fileUpload.DownloadFile(item, Convert.ToBoolean(SystemSettingService.Dictionary["AZFLS"]), FileUploadSettings.Container, ref file);    // FileUploadSettings.FileUpload
                        //         Attachments objAttachments = new Attachments();
                        //         string base64 = Convert.ToBase64String(imageByte);
                        //         objAttachments.odatatype = "#microsoft.graph.fileAttachment";
                        //         objAttachments.contentBytes = base64;
                        //         objAttachments.contentType = file.FileType;
                        //         objAttachments.name = file.FileName + "." + file.Extention;
                        //         objAttachmentList.Add(objAttachments);
                        //     }
                        // }
                    }

                    if (model.QuickEmailAttachments != null)
                    {
                        Byte[] imageByte = new Byte[0];
                        var dPath = _hostingEnvironment.WebRootPath + "\\AttachEmailPicture";
                        foreach (var item in model.QuickEmailAttachments)
                        {
                            if (System.IO.File.Exists(dPath + "\\" + item.FileName))
                            {
                                imageByte = System.IO.File.ReadAllBytes(dPath + "\\" + item.FileName);
                            }
                            else
                            {
                                imageByte = System.IO.File.ReadAllBytes(dPath + "\\" + "file-not-found.jpg");
                            }

                            MailAttachments objAttachments = new MailAttachments();
                            string base64 = Convert.ToBase64String(imageByte);
                            objAttachments.odatatype = "#microsoft.graph.fileAttachment";
                            objAttachments.contentBytes = base64;
                            //objAttachments.contentType = "image/jpeg";
                            objAttachments.name = item.OrignalFileName;
                            objAttachmentList.Add(objAttachments);
                        }
                    }

                    office365Message.attachments = objAttachmentList.ToArray();

                    //// Create Message
                    //createMessageBody.message = office365Message;
                    //createMessageBody.saveToSentItems = "true";

                    await SetOffice365Token();
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    // For recipients receive email as if it was only sent to them
                    var response365 = new HttpResponseMessage();
                    if (model.IsSendMassEmail)
                    {
                        foreach (var item in model.To)
                        {
                            objToList = new List<Torecipient>();
                            Torecipient objTo = new Torecipient();
                            Emailaddress objEmail = new Emailaddress();
                            objEmail.address = item.Contains("<") ? item.Split("<")[1].Trim('>').Trim() : item;
                            objEmail.name = item.Contains("<") ? item.Split("<")[0].Trim() : item;
                            objTo.emailAddress = objEmail;
                            objToList.Add(objTo);
                            office365Message.toRecipients = objToList.ToArray();

                            // Create Message
                            createMessageBody.message = office365Message;
                            createMessageBody.saveToSentItems = "true";

                            var myContent = JsonConvert.SerializeObject(createMessageBody);
                            var ChangeAttach = myContent.Replace("odatatype", "@odata.type");
                            var buffer = Encoding.UTF8.GetBytes(ChangeAttach);
                            var byteContent = new ByteArrayContent(buffer);
                            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                            response365 = await client.PostAsync(DataUtility.Office365SendEmail, byteContent); // DataUtility.Office365SendEmail
                        }
                    }
                    else
                    {
                        // Create Message
                        createMessageBody.message = office365Message;
                        createMessageBody.saveToSentItems = "true";

                        var myContent = JsonConvert.SerializeObject(createMessageBody);
                        var ChangeAttach = myContent.Replace("odatatype", "@odata.type");
                        var buffer = Encoding.UTF8.GetBytes(ChangeAttach);
                        var byteContent = new ByteArrayContent(buffer);
                        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                        response365 = await client.PostAsync(DataUtility.Office365SendEmail, byteContent); // DataUtility.Office365SendEmail
                    }

                    //var response365 = await client.PostAsync(SystemSettingService.Dictionary["OFSDM"], byteContent); // DataUtility.Office365SendEmail
                    if (response365.StatusCode != HttpStatusCode.Accepted)
                    {
                        model.IsValid = false;
                        model.ErrorMessage = CommonMessage.ErrorOccurredEmailSend;
                        return model;
                    }
                    break;
            }

            model.IsValid = true;
            model.ErrorMessage = CommonMessage.EmailSent;

            return model;
        }

        public async Task<InboxThreads> DeleteMultipleEmail(int userId, ThreadOperationVM model)
        {
            user = _userService.GetUserById(userId);
            if (user == null)
            {
                inboxThreads.IsValid = false;
                inboxThreads.ErrorMessage = CommonMessage.UnAuthorizedUser;
                return inboxThreads;
            }

            // var emailAccount = _emailAccountService.GetEmailAccountById(model.UserEmail.AccountId);
            // userEmail = _userEmailService.GetUserEmailById(model.UserEmail.UserEmailId);

            var intproviderApp = _intProviderAppService.GetIntProviderApp(model.ProviderApp);

            // var intAppSecretObj = _intProviderAppSecretService.GetByUserAndEmail(userId, model.SelectedEmail);
            var intAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUserAndEmail(userId, model.SelectedEmail, intproviderApp.Id);


            if (intproviderApp != null && intAppSecretObj != null)
            {
                mailTokenDto.refresh_token = intAppSecretObj.Refresh_Token;
                mailTokenDto.access_token = intAppSecretObj.Access_Token;
                // mailTokenDto.code = model.Code;
                mailTokenDto.ProviderApp = model.Provider;
                mailTokenDto.UserId = userId;
                intProviderAppSecretObj.Refresh_Token = intAppSecretObj.Refresh_Token;
                intProviderAppSecretObj.Access_Token = intAppSecretObj.Access_Token;
                intProviderAppSecretObj.Email = intAppSecretObj.Email;
            }

            foreach (var item in model.ThreadId)
            {

                switch (intproviderApp.Name)
                {
                    case "Gmail":

                        await SetGmailToken();
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                        //string gmailUri = DataUtility.GmailThreadItem;
                        //if (threadType == "message") gmailUri = DataUtility.GmailMessageItem;
                        string gmailUri = DataUtility.GmailThreadItem;
                        if (model.ThreadType == "message") gmailUri = DataUtility.GmailMessageItem;

                        var response = await client.DeleteAsync(string.Format(gmailUri, item));

                        if (response.StatusCode == HttpStatusCode.NoContent)
                        {
                            inboxThreads.IsValid = true;
                            inboxThreads.ErrorMessage = CommonMessage.MessageDeletedSuccessMsg;
                        }
                        else
                        {
                            inboxThreads.IsValid = false;
                            inboxThreads.ErrorMessage = CommonMessage.DefaultErrorMessage;
                        }

                        break;

                    case "Office 365":
                    case "Outlook":

                        await SetOffice365Token();
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                        if (model.ThreadType == "thread") await DeleteOffice365Thread(item);
                        else
                        {
                            var officce365response = await client.DeleteAsync(string.Format(DataUtility.Office365DeleteEmail, item)); //DataUtility.Office365DeleteEmail
                            if (officce365response.StatusCode == HttpStatusCode.NoContent)
                            {
                                inboxThreads.IsValid = true;
                                inboxThreads.ErrorMessage = CommonMessage.MessageDeletedSuccessMsg;
                            }
                            else
                            {
                                inboxThreads.IsValid = false;
                                inboxThreads.ErrorMessage = CommonMessage.DefaultErrorMessage;
                            }
                        }

                        break;

                    default:
                        break;
                }
            }


            return inboxThreads;
        }

        public async Task<InboxThreads> DeleteEmail(int userId, string threadId, string threadType, UserEmail model)
        {
            user = _userService.GetUserById(userId);
            if (user == null)
            {
                inboxThreads.IsValid = false;
                inboxThreads.ErrorMessage = CommonMessage.UnAuthorizedUser;
                return inboxThreads;
            }

            // var emailAccount = _emailAccountService.GetEmailAccountById(model.AccountId);
            // userEmail = _userEmailService.GetUserEmailById(model.UserEmailId);

            var intproviderApp = _intProviderAppService.GetIntProviderApp(model.ProviderApp);

            // var intAppSecretObj = _intProviderAppSecretService.GetByUserAndEmail(userId, model.SelectedEmail);
            var intAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUserAndEmail(userId, model.SelectedEmail, intproviderApp.Id);


            if (intproviderApp != null && intAppSecretObj != null)
            {
                mailTokenDto.refresh_token = intAppSecretObj.Refresh_Token;
                mailTokenDto.access_token = intAppSecretObj.Access_Token;
                mailTokenDto.code = model.Code;
                mailTokenDto.ProviderApp = model.Provider;
                mailTokenDto.UserId = userId;
                intProviderAppSecretObj.Refresh_Token = intAppSecretObj.Refresh_Token;
                intProviderAppSecretObj.Access_Token = intAppSecretObj.Access_Token;
                intProviderAppSecretObj.Email = intAppSecretObj.Email;
            }

            switch (intproviderApp.Name)
            {
                case "Gmail":

                    await SetGmailToken();
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    //string gmailUri = DataUtility.GmailThreadItem;
                    //if (threadType == "message") gmailUri = DataUtility.GmailMessageItem;
                    string gmailUri = DataUtility.GmailThreadItem;
                    if (threadType == "message") gmailUri = DataUtility.GmailMessageItem;

                    var response = await client.DeleteAsync(string.Format(gmailUri, threadId));

                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        inboxThreads.IsValid = true;
                        inboxThreads.ErrorMessage = CommonMessage.MessageDeletedSuccessMsg;
                    }
                    else
                    {
                        inboxThreads.IsValid = false;
                        inboxThreads.ErrorMessage = CommonMessage.DefaultErrorMessage;
                    }

                    break;

                case "Office 365":
                case "Outlook":

                    await SetOffice365Token();
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    if (threadType == "thread") await DeleteOffice365Thread(threadId);
                    else
                    {
                        var officce365response = await client.DeleteAsync(string.Format(DataUtility.Office365DeleteEmail, threadId)); //DataUtility.Office365DeleteEmail
                        if (officce365response.StatusCode == HttpStatusCode.NoContent)
                        {
                            inboxThreads.IsValid = true;
                            inboxThreads.ErrorMessage = CommonMessage.MessageDeletedSuccessMsg;
                        }
                        else
                        {
                            inboxThreads.IsValid = false;
                            inboxThreads.ErrorMessage = CommonMessage.DefaultErrorMessage;
                        }
                    }

                    break;

                default:
                    break;
            }

            return inboxThreads;
        }

        public async Task<InboxThreads> DeleteOffice365Thread(string threadId)
        {
            var threadResponse = await client.GetAsync(string.Format(DataUtility.Office365ThreadByConversionId, threadId)); //DataUtility.Office365ThreadByConversionId
            if (threadResponse.StatusCode == HttpStatusCode.OK)
            {
                var stream = await threadResponse.Content.ReadAsStreamAsync();
                var serializer = new DataContractJsonSerializer(typeof(MessageConversation));
                var threadDetail = (MessageConversation)serializer.ReadObject(stream);
                if (threadDetail != null)
                {
                    if (threadDetail.value != null)
                    {
                        foreach (var message in threadDetail.value)
                        {
                            var officce365response = await client.DeleteAsync(string.Format(DataUtility.Office365DeleteEmail, message.id)); //DataUtility.Office365DeleteEmail
                            if (officce365response.StatusCode == HttpStatusCode.NoContent)
                            {
                                inboxThreads.IsValid = true;
                                inboxThreads.ErrorMessage = CommonMessage.MessageDeletedSuccessMsg;
                            }
                            else
                            {
                                inboxThreads.IsValid = false;
                                inboxThreads.ErrorMessage = CommonMessage.DefaultErrorMessage;
                            }
                        }
                    }
                }
            }

            return inboxThreads;
        }

        public async Task<InboxThreads> MarkAsReadUnRead(int userId, string messageId, UserEmail model)
        {
            bool isCompleted = false;
            user = _userService.GetUserById(userId);
            if (user == null)
            {
                inboxThreads.IsValid = false;
                inboxThreads.ErrorMessage = CommonMessage.UnAuthorizedUser;
                return inboxThreads;
            }

            if (string.IsNullOrEmpty(model.Provider) && string.IsNullOrEmpty(model.ProviderApp))
            {
                var customEmailDomainObj = _customDomainEmailConfigService.GetByUserAndEmail(userId, model.SelectedEmail);
                if (customEmailDomainObj != null)
                {
                    inboxThreads.InboxThread = new List<InboxThread>();
                    using (var client = new ImapClient())
                    {
                        var options = SecureSocketOptions.StartTlsWhenAvailable;
                        options = SecureSocketOptions.SslOnConnect;

                        #region Host, Port, UserName, Passwrod with different host

                        string Host = customEmailDomainObj.IMAPHost;
                        int Port = customEmailDomainObj.IMAPPort.Value;
                        // string UserName = "yagnik.darji@techavidus.com";
                        // string Password = "8[)6fUV+'}";
                        string UserName = customEmailDomainObj.Email;
                        string Password = customEmailDomainObj.Password;

                        #endregion

                        client.CheckCertificateRevocation = false;
                        client.ServerCertificateValidationCallback = (s, c, ch, e) => true;

                        client.Connect(Host, Port, options);

                        //client.AuthenticationMechanisms.Remove("XOAUTH2");

                        client.Authenticate(UserName, Password);

                        //Check IsConnected condition
                        if (client.IsConnected)
                        {
                            // The Inbox folder is always available on all IMAP servers...
                            var inbox = client.Inbox;
                            inbox.Open(FolderAccess.ReadOnly);
                            var folder = client.GetFolder(model.Label);
                            var uids = folder.Search(MailKit.Search.SearchQuery.HeaderContains("Message-Id", messageId));
                            if (uids.Count() == 0)
                            {
                                uids = folder.Search(MailKit.Search.SearchQuery.HeaderContains("Message-Id", "<" + messageId + ">"));
                            }
                            foreach (var item in uids)
                            {
                                folder.AddFlags(item, MessageFlags.None, true);
                            }

                            client.Disconnect(true);
                        }
                    }
                }
                // inboxThreads.count = inboxThreads.InboxThread.Count();
                inboxThreads.IsValid = true;
                inboxThreads.ErrorMessage = CommonMessage.MessageMarkAsUnRead;
                return inboxThreads;
            }

            // var emailAccount = _emailAccountService.GetEmailAccountById(model.AccountId);
            // userEmail = _userEmailService.GetUserEmailById(model.UserEmailId);

            var intproviderApp = _intProviderAppService.GetIntProviderApp(model.ProviderApp);

            // var intAppSecretObj = _intProviderAppSecretService.GetByUserAndEmail(userId, model.SelectedEmail);
            var intAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUserAndEmail(userId, model.SelectedEmail, intproviderApp.Id);



            if (intproviderApp != null && intAppSecretObj != null)
            {
                mailTokenDto.refresh_token = intAppSecretObj.Refresh_Token;
                mailTokenDto.access_token = intAppSecretObj.Access_Token;
                // mailTokenDto.code = model.Code;
                mailTokenDto.ProviderApp = model.Provider;
                mailTokenDto.UserId = userId;
                intProviderAppSecretObj.Refresh_Token = intAppSecretObj.Refresh_Token;
                intProviderAppSecretObj.Access_Token = intAppSecretObj.Access_Token;
                intProviderAppSecretObj.Email = intAppSecretObj.Email;
            }

            switch (intproviderApp.Name)
            {
                case "Gmail":
                    // isCompleted = await MarkGmailEmailAsReadUnread(threadId, true);
                    isCompleted = await MarkGmailEmailAsReadUnread(messageId, model.IsRead);
                    break;

                case "Office 365":
                case "Outlook":
                    // isCompleted = await MarkOffice365EmailAsReadUnread(threadId, true);
                    isCompleted = await MarkOffice365EmailAsReadUnread(messageId, model.IsRead);
                    break;

                default:
                    break;
            }
            if (isCompleted && model.IsRead)
            {
                inboxThreads.IsValid = true;
                inboxThreads.ErrorMessage = CommonMessage.MessageMarkAsRead;
            }
            else if (isCompleted && model.IsRead == false)
            {
                inboxThreads.IsValid = true;
                inboxThreads.ErrorMessage = CommonMessage.MessageMarkAsUnRead;
            }
            else
            {
                inboxThreads.IsValid = false;
                inboxThreads.ErrorMessage = CommonMessage.UnAuthorizedUser;
            }

            return inboxThreads;
        }

        public async Task<InboxThreads> MultipleThreadMarkAsReadUnRead(int userId, ThreadOperationVM model)
        {
            bool isCompleted = false;
            user = _userService.GetUserById(userId);
            if (user == null)
            {
                inboxThreads.IsValid = false;
                inboxThreads.ErrorMessage = CommonMessage.UnAuthorizedUser;
                return inboxThreads;
            }

            // var emailAccount = _emailAccountService.GetEmailAccountById(model.UserEmail.AccountId);
            // userEmail = _userEmailService.GetUserEmailById(model.UserEmail.UserEmailId);
            var intproviderApp = _intProviderAppService.GetIntProviderApp(model.ProviderApp);

            // var intAppSecretObj = _intProviderAppSecretService.GetByUserAndEmail(userId, model.SelectedEmail);
            var intAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUserAndEmail(userId, model.SelectedEmail, intproviderApp.Id);


            if (intproviderApp != null && intAppSecretObj != null)
            {
                mailTokenDto.refresh_token = intAppSecretObj.Refresh_Token;
                mailTokenDto.access_token = intAppSecretObj.Access_Token;
                // mailTokenDto.code = model.Code;
                mailTokenDto.ProviderApp = model.Provider;
                mailTokenDto.UserId = userId;
                intProviderAppSecretObj.Refresh_Token = intAppSecretObj.Refresh_Token;
                intProviderAppSecretObj.Access_Token = intAppSecretObj.Access_Token;
                intProviderAppSecretObj.Email = intAppSecretObj.Email;
            }

            foreach (var item in model.ThreadId)
            {
                switch (intproviderApp.Name)
                {
                    case "Gmail":
                        isCompleted = await MarkGmailEmailAsReadUnread(item, model.IsRead);
                        break;

                    case "Office 365":
                    case "Outlook":
                        isCompleted = await MarkOffice365EmailAsReadUnread(item, model.IsRead);
                        break;

                    default:
                        break;
                }
                if (isCompleted)
                {
                    inboxThreads.IsValid = true;
                    inboxThreads.ErrorMessage = CommonMessage.MessageMarkAsUnRead;
                }
                else
                {
                    inboxThreads.IsValid = false;
                    inboxThreads.ErrorMessage = CommonMessage.UnAuthorizedUser;
                }
            }

            return inboxThreads;
        }


        public async Task<InboxThreads> ReadUnReadByThread(string threadId, int userId, UserEmail model)
        {
            bool isCompleted = false;
            user = _userService.GetUserById(userId);
            if (user == null)
            {
                inboxThreads.IsValid = false;
                inboxThreads.ErrorMessage = CommonMessage.UnAuthorizedUser;
                return inboxThreads;
            }
            if (string.IsNullOrEmpty(model.Provider) && string.IsNullOrEmpty(model.ProviderApp))
            {
                var customEmailDomainObj = _customDomainEmailConfigService.GetByUserAndEmail(userId, model.SelectedEmail);
                if (customEmailDomainObj != null)
                {
                    inboxThreads.InboxThread = new List<InboxThread>();
                    using (var client = new ImapClient())
                    {
                        var options = SecureSocketOptions.StartTlsWhenAvailable;
                        options = SecureSocketOptions.SslOnConnect;

                        #region Host, Port, UserName, Passwrod with different host

                        string Host = customEmailDomainObj.IMAPHost;
                        int Port = customEmailDomainObj.IMAPPort.Value;
                        // string UserName = "yagnik.darji@techavidus.com";
                        // string Password = "8[)6fUV+'}";
                        string UserName = customEmailDomainObj.Email;
                        string Password = customEmailDomainObj.Password;

                        #endregion

                        client.CheckCertificateRevocation = false;
                        client.ServerCertificateValidationCallback = (s, c, ch, e) => true;

                        client.Connect(Host, Port, options);

                        //client.AuthenticationMechanisms.Remove("XOAUTH2");

                        client.Authenticate(UserName, Password);

                        //Check IsConnected condition
                        if (client.IsConnected)
                        {
                            // The Inbox folder is always available on all IMAP servers...
                            var folder = client.GetFolder(model.Label);
                            folder.Open(FolderAccess.ReadWrite);
                            var uids = folder.Search(MailKit.Search.SearchQuery.HeaderContains("Message-Id", threadId));
                            if (uids.Count() == 0)
                            {
                                uids = folder.Search(MailKit.Search.SearchQuery.HeaderContains("Message-Id", "<" + threadId + ">"));
                            }
                            foreach (var item in uids)
                            {
                                if (model.IsRead)
                                {
                                    folder.AddFlags(item, MessageFlags.Seen, true);
                                }
                                else
                                {
                                    folder.RemoveFlags(item, MessageFlags.Seen, true);
                                }

                            }

                            client.Disconnect(true);
                        }
                    }
                }
                // inboxThreads.count = inboxThreads.InboxThread.Count();
                inboxThreads.IsValid = true;
                inboxThreads.ErrorMessage = CommonMessage.MessageMarkAsUnRead;
                return inboxThreads;
            }
            var intproviderApp = _intProviderAppService.GetIntProviderApp(model.ProviderApp);
            var intAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUserAndEmail(userId, model.SelectedEmail, intproviderApp.Id);


            if (intproviderApp != null && intAppSecretObj != null)
            {
                mailTokenDto.refresh_token = intAppSecretObj.Refresh_Token;
                mailTokenDto.access_token = intAppSecretObj.Access_Token;
                // mailTokenDto.code = model.Code;
                mailTokenDto.ProviderApp = model.Provider;
                mailTokenDto.UserId = userId;
                intProviderAppSecretObj.Refresh_Token = intAppSecretObj.Refresh_Token;
                intProviderAppSecretObj.Access_Token = intAppSecretObj.Access_Token;
                intProviderAppSecretObj.Email = intAppSecretObj.Email;
            }

            switch (intproviderApp.Name)
            {
                case "Gmail":
                    await SetGmailToken();
                    var task = new List<Task>();
                    inboxThreadItems = new List<InboxThreadItem>();
                    InboxVM objInbox = new InboxVM();
                    // objInbox.UserEmail = userEmail;
                    // await SetInboxThreadDetail(threadId, objInbox, true);
                    task.Add(SetInboxThreadDetail(threadId, objInbox, true));
                    Task.WaitAll(task.ToArray());

                    foreach (var item in inboxThreadItems)
                    {
                        // await MarkAsReadUnRead(userId, item.MessageId,)
                        isCompleted = await MarkGmailEmailAsReadUnread(item.MessageId, model.IsRead);
                    }

                    break;

                case "Office 365":
                case "Outlook":

                    await SetOffice365Token();
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    inboxThreadItems = new List<InboxThreadItem>();
                    var url = string.Format(DataUtility.Office365ThreadByConversionId, "'" + threadId + "'");
                    var threadResponse = await client.GetAsync(url);
                    Office365Thread office365Threads = null;
                    if (threadResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var stream = await threadResponse.Content.ReadAsStreamAsync();

                        var serializer = new DataContractJsonSerializer(typeof(MessageConversation));
                        var threadDetail = (MessageConversation)serializer.ReadObject(stream);
                        if (threadDetail != null)
                        {
                            if (threadDetail.value != null)
                            {
                                foreach (var message in threadDetail.value)
                                {
                                    InboxThreadItem objThread = new InboxThreadItem();
                                    objThread.ThreadId = message.conversationId;
                                    await MarkOffice365EmailAsReadUnread(message.id, model.IsRead);
                                }
                            }
                        }
                    }
                    break;

                default:
                    break;
            }
            inboxThreads.IsValid = true;
            return inboxThreads;
        }

        // #endregion

        // #region 'Gmail Private Function'
        // private async Task<bool> GetGmailToken(UserEmail model, string redirectUri)
        // {
        //     var isTokenReceived = false;
        //     client.DefaultRequestHeaders.Clear();

        //     var param = new Dictionary<string, string>();
        //     param.Add("code", model.Code);
        //     param.Add("client_id", SystemSettingService.Dictionary["GMCID"]);   //DataUtility.GmailApiClientId
        //     param.Add("client_secret", SystemSettingService.Dictionary["GMCSC"]);   //DataUtility.GmailApiClientSecret
        //     param.Add("redirect_uri", redirectUri);
        //     param.Add("grant_type", "authorization_code");

        //     //var response = await client.PostAsync(DataUtility.GmailToken, new FormUrlEncodedContent(param));
        //     var response = await client.PostAsync(SystemSettingService.Dictionary["GMTOK"], new FormUrlEncodedContent(param));
        //     if (response.StatusCode == HttpStatusCode.OK)
        //     {
        //         var stream = await response.Content.ReadAsStreamAsync();
        //         var serializer = new DataContractJsonSerializer(typeof(Token));
        //         var gmailToken = (Token)serializer.ReadObject(stream);

        //         if (gmailToken != null && !string.IsNullOrEmpty(gmailToken.access_token))
        //         {
        //             token = gmailToken.access_token;
        //             model.Access_Token = token;
        //             model.Refresh_Token = gmailToken.refresh_token;
        //             model.ExpireIn = Convert.ToString(gmailToken.expires_in);
        //             model.Scope = gmailToken.scope;
        //             var dt = Utility.GetCurrentDateTime();
        //             //model.ExpireOn = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second + Convert.ToInt32(model.ExpireIn));
        //             model.Refresh_Token = gmailToken.refresh_token;

        //             _userEmailService.CheckInsertOrUpdate(model);
        //             isTokenReceived = true;
        //         }
        //     }

        //     return isTokenReceived;
        // }

        private async Task<bool> SetInboxThreadDetail(string threadId, InboxVM model, bool? isIncludeAllThread = false)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            //var threadResponse = await client.GetAsync(string.Format(DataUtility.GmailThreadItem, threadId));
            var threadResponse = await client.GetAsync(string.Format(DataUtility.GmailThreadItem, threadId));
            if (threadResponse.StatusCode == HttpStatusCode.OK)
            {
                var stream = await threadResponse.Content.ReadAsStreamAsync();
                var serializer = new DataContractJsonSerializer(typeof(ThreadDetail));
                var threadDetail = (ThreadDetail)serializer.ReadObject(stream);

                if (!string.IsNullOrEmpty(threadDetail.id) && threadDetail.messages.Count > 0)
                {
                    if ((bool)isIncludeAllThread)
                    {
                        foreach (var message in threadDetail.messages)
                        {
                            if (message.payload.headers.Count > 0)
                            {
                                var inboxThreadItem = new InboxThreadItem();
                                inboxThreadItem.ThreadId = threadId;
                                inboxThreadItem.Subject = message.payload.headers.Find(x => x.name.ToLower() == "subject").value ?? "";
                                inboxThreadItem.From = message.payload.headers.Find(x => x.name.ToLower() == "from").value ?? "";
                                inboxThreadItem.To = message.payload.headers.Find(x => x.name.ToLower() == "to") != null ? message.payload.headers.Find(x => x.name.ToLower() == "to").value : "";
                                inboxThreadItem.Date = message.payload.headers.Find(x => x.name.ToLower() == "date").value ?? "";
                                inboxThreadItem.Snippet = message.snippet;
                                inboxThreadItem.IsHasAttachment = message.payload.mimeType.Equals("multipart/mixed");
                                inboxThreadItem.MessageId = message.id;
                                inboxThreadItem.LabelIds = message.labelIds;
                                inboxThreadItem.SizeEstimate = message.sizeEstimate;
                                inboxThreadItem.MimeType = message.payload.mimeType;
                                inboxThreadItem.Filename = message.payload.filename;
                                inboxThreadItem.InternalDate = Convert.ToInt64(message.internalDate);
                                inboxThreadItem.CreatedOn = matcrm.service.Common.Common.UnixTimeStampToDateTimeMilliSec(inboxThreadItem.InternalDate);

                                // Get body content and attachments
                                var body = GetBodyAndAttachment(message.payload);
                                inboxThreadItem.BodyHtml = body.htmlBody != null ? body.htmlBody : new Body();
                                inboxThreadItem.BodyPlain = body.plainBody != null ? body.htmlBody : new Body();
                                inboxThreadItem.Attachments = body.attachments != null ? body.attachments : new List<Body>();
                                inboxThreadItem.Attachments = inboxThreadItem.Attachments.Where(t => !string.IsNullOrEmpty(t.attachmentId)).ToList();
                                if (inboxThreadItem.Attachments.Count() > 0)
                                {
                                    foreach (var attachmentItem in inboxThreadItem.Attachments)
                                    {
                                        var bodydata = await GetGmailAttachment(inboxThreadItem.MessageId, attachmentItem.attachmentId);
                                        attachmentItem.contentBytes = bodydata.data;
                                        attachmentItem.bytes = bodydata.bytes;
                                        //    attachmentItem.fileName = bodydata.fileName;
                                        //    attachmentItem.extention = bodydata.extention;
                                    }
                                }

                                // set From and To email address and name
                                var fromEmail = message.payload.headers.Find(x => x.name.ToLower() == "from").value;
                                inboxThreadItem.FromName = fromEmail.Contains("<") ? fromEmail.Split("<")[0].Replace("\"", string.Empty).Trim() : fromEmail;
                                inboxThreadItem.FromEmail = fromEmail.Contains("<") ? fromEmail.Split("<")[1].Trim('>').Replace("\"", string.Empty).Trim() : fromEmail;

                                // To Email
                                if (message.payload.headers.Find(x => x.name.ToLower() == "to") != null)
                                {
                                    var toEmail = message.payload.headers.Find(x => x.name.ToLower() == "to").value;
                                    if (toEmail.Contains(","))
                                        foreach (var item in toEmail.Split(","))
                                            inboxThreadItem.ToName += (item.Contains("<") ? item.Split("<")[0].Replace("\"", string.Empty).Trim() : item) + ", ";
                                    else
                                        inboxThreadItem.ToName = toEmail;

                                    inboxThreadItem.ToName = inboxThreadItem.ToName.Trim().Trim(',');
                                    inboxThreadItem.ToEmail = toEmail;
                                }

                                // Cc Email
                                if (message.payload.headers.Find(x => x.name.ToLower() == "cc") != null)
                                {
                                    var ccEmail = message.payload.headers.Find(x => x.name.ToLower() == "cc").value;
                                    if (ccEmail.Contains(","))
                                        foreach (var item in ccEmail.Split(","))
                                            inboxThreadItem.CcName += (item.Contains("<") ? item.Split("<")[0].Replace("\"", string.Empty).Trim() : item) + ", ";
                                    else
                                        inboxThreadItem.CcName = ccEmail;

                                    inboxThreadItem.CcName = inboxThreadItem.CcName.Trim().Trim(',');
                                    inboxThreadItem.CcEmail = ccEmail;
                                }

                                // Bcc Email
                                if (message.payload.headers.Find(x => x.name.ToLower() == "bcc") != null)
                                {
                                    var bccEmail = message.payload.headers.Find(x => x.name.ToLower() == "bcc").value;
                                    if (bccEmail.Contains(","))
                                        foreach (var item in bccEmail.Split(","))
                                            inboxThreadItem.BccName += (item.Contains("<") ? item.Split("<")[0].Replace("\"", string.Empty).Trim() : item) + ", ";
                                    else
                                        inboxThreadItem.BccName = bccEmail;

                                    inboxThreadItem.BccName = inboxThreadItem.BccName.Trim().Trim(',');
                                    inboxThreadItem.BccEmail = bccEmail;
                                }

                                // Check email is read / unread
                                var unread = Array.Find(message.labelIds, x => x == DataUtility.GmailUNREAD); //DataUtility.GmailUNREAD
                                inboxThreadItem.IsOpen = unread != null && unread.Length > 0 ? false : true;
                                if (string.IsNullOrEmpty(unread))
                                {
                                    inboxThreadItem.IsRead = true;
                                }
                                else
                                {
                                    inboxThreadItem.IsRead = false;
                                }
                                inboxThreadItems.Add(inboxThreadItem);
                            }

                            // Mark email as read if it is unread
                            //if (message.labelIds.Where(x => x == DataUtility.GmailUNREAD).FirstOrDefault() != null) MarkGmailEmailAsReadUnread(message.id);
                            // if (message.labelIds.Where(x => x == DataUtility.GmailUNREAD).FirstOrDefault() != null) MarkGmailEmailAsReadUnread(message.id);
                        }
                    }
                    else
                    {
                        var lastMessageIndex = threadDetail.messages.Count - 1;
                        if (threadDetail.messages[lastMessageIndex].payload.headers.Count > 0)
                        {
                            var inboxThread = new InboxThread();
                            inboxThread.ThreadId = threadId;
                            inboxThread.messageId = threadDetail.messages[lastMessageIndex].id;
                            inboxThread.Subject = threadDetail.messages[lastMessageIndex].payload.headers.Find(x => x.name.ToLower() == "subject") != null ? threadDetail.messages[lastMessageIndex].payload.headers.Find(x => x.name.ToLower() == "subject").value : "";
                            inboxThread.From = threadDetail.messages[lastMessageIndex].payload.headers.Find(x => x.name.ToLower() == "from") != null ? threadDetail.messages[lastMessageIndex].payload.headers.Find(x => x.name.ToLower() == "from").value : "";
                            inboxThread.To = threadDetail.messages[lastMessageIndex].payload.headers.Find(x => x.name.ToLower() == "to") != null ? threadDetail.messages[lastMessageIndex].payload.headers.Find(x => x.name.ToLower() == "to").value : "";
                            inboxThread.Date = threadDetail.messages[lastMessageIndex].payload.headers.Find(x => x.name.ToLower() == "date") != null ? threadDetail.messages[lastMessageIndex].payload.headers.Find(x => x.name.ToLower() == "date").value : "";
                            inboxThread.Snippet = threadDetail.messages[lastMessageIndex].snippet ?? "";
                            inboxThread.IsHasAttachment = threadDetail.messages[lastMessageIndex].payload.mimeType.Equals("multipart/mixed");
                            inboxThread.InternalDate = Convert.ToInt64(threadDetail.messages[lastMessageIndex].internalDate);
                            inboxThread.CreatedOn = matcrm.service.Common.Common.UnixTimeStampToDateTimeMilliSec(inboxThread.InternalDate);
                            // set From and To email address and name
                            var fromEmail = inboxThread.From;
                            inboxThread.FromName = fromEmail.Contains("<") ? fromEmail.Split("<")[0].Replace("\"", string.Empty).Trim() : fromEmail;
                            inboxThread.FromEmail = fromEmail.Contains("<") ? fromEmail.Split("<")[1].Trim('>').Replace("\"", string.Empty).Trim() : fromEmail;

                            // To Email
                            if (threadDetail.messages[lastMessageIndex].payload.headers.Find(x => x.name.ToLower() == "to") != null)
                            {
                                var toEmail = threadDetail.messages[lastMessageIndex].payload.headers.Find(x => x.name.ToLower() == "to").value;
                                if (toEmail.Contains(","))
                                    foreach (var item in toEmail.Split(","))
                                        inboxThread.ToName += (item.Contains("<") ? item.Split("<")[0].Replace("\"", string.Empty).Trim() : item) + ", ";
                                else
                                    inboxThread.ToName = toEmail;

                                inboxThread.ToName = inboxThread.ToName.Trim().Trim(',');
                                inboxThread.ToEmail = toEmail;
                            }

                            // Cc Email
                            if (threadDetail.messages[lastMessageIndex].payload.headers.Find(x => x.name.ToLower() == "cc") != null)
                            {
                                var ccEmail = threadDetail.messages[lastMessageIndex].payload.headers.Find(x => x.name.ToLower() == "cc").value;
                                if (ccEmail.Contains(","))
                                    foreach (var item in ccEmail.Split(","))
                                        inboxThread.CcName += (item.Contains("<") ? item.Split("<")[0].Replace("\"", string.Empty).Trim() : item) + ", ";
                                else
                                    inboxThread.CcName = ccEmail;

                                inboxThread.CcName = inboxThread.CcName.Trim().Trim(',');
                                inboxThread.CcEmail = ccEmail;
                            }

                            // Bcc Email
                            if (threadDetail.messages[lastMessageIndex].payload.headers.Find(x => x.name.ToLower() == "bcc") != null)
                            {
                                var bccEmail = threadDetail.messages[lastMessageIndex].payload.headers.Find(x => x.name.ToLower() == "bcc").value;
                                if (bccEmail.Contains(","))
                                    foreach (var item in bccEmail.Split(","))
                                        inboxThread.BccName += (item.Contains("<") ? item.Split("<")[0].Replace("\"", string.Empty).Trim() : item) + ", ";
                                else
                                    inboxThread.BccName = bccEmail;

                                inboxThread.BccName = inboxThread.BccName.Trim().Trim(',');
                                inboxThread.BccEmail = bccEmail;
                            }

                            // Check email is read / unread
                            var unread = Array.Find(threadDetail.messages[lastMessageIndex].labelIds, x => x == DataUtility.GmailUNREAD); //DataUtility.GmailUNREAD
                            inboxThread.IsOpen = unread != null && unread.Length > 0 ? false : true;
                            if (string.IsNullOrEmpty(unread))
                            {
                                inboxThread.IsRead = true;
                            }
                            else
                            {
                                inboxThread.IsRead = false;
                            }
                            // inboxThread.AccountId = model.UserEmail != null ? model.UserEmail.AccountId : 0;
                            // inboxThread.UserEmailId = model.UserEmail != null ? model.UserEmail.UserEmailId : 0;
                            // inboxThread.AccountAlias = model.UserEmail != null ? model.UserEmail.Alias : "";

                            inboxThreads.InboxThread.Add(inboxThread);
                        }
                    }
                }
            }

            return true;
        }

        private Part GetBodyAndAttachment(Payload payload)
        {
            if (payload.parts == null)
            {
                payload.parts = new List<Part>();
                var partItem = new Part();
                partItem.body = payload.body;
                partItem.mimeType = payload.mimeType;
                payload.parts.Add(partItem);
            }

            var lstAttachment = new List<Part>();
            var part = new Part();
            part.body = new Body();
            part.plainBody = new Body();
            part.htmlBody = new Body();
            part.attachments = new List<Body>();
            var multipartAlternative = new List<Part>();

            // Check Message has attachment
            var isHasAttachment = payload.mimeType.Equals("multipart/mixed");
            if (isHasAttachment)
            {
                // Get attachments
                var attachments = payload.parts.Where(x => !x.mimeType.Contains("multipart/alternative") && !x.mimeType.Contains("text/plain") && !x.mimeType.Contains("text/html")).ToList();
                if (attachments != null && attachments.Count > 0)
                    foreach (var item in attachments)
                    {
                        item.body.mimeType = item.mimeType;
                        var contentType = item.headers.Find(x => x.name.Equals("Content-Type"));
                        if (contentType != null) item.body.contentType = contentType.value;
                        item.body.fileName = item.filename;
                        item.body.extention = item.filename.Contains(".") ? item.filename.Split(".")[1] : "";
                        part.attachments.Add(item.body);
                    }


                // Get body content
                var alternativePart = payload.parts.FirstOrDefault(x => x.mimeType.Contains("multipart/alternative"));
                if (alternativePart != null) multipartAlternative = alternativePart.parts;
            }
            else if (payload.mimeType.Equals("multipart/alternative"))
            {
                multipartAlternative = payload.parts;
            }
            else
            {
                var alternativePart = payload.parts.FirstOrDefault(x => x.mimeType.Contains("multipart/alternative"));
                if (alternativePart != null)
                    multipartAlternative = alternativePart.parts;
                else
                    multipartAlternative = payload.parts;
            }

            // If alternative part not present then file text or html part directly
            if (multipartAlternative == null || multipartAlternative.Count <= 0)
            {
                var plainPart = payload.parts.FirstOrDefault(x => x.mimeType.Contains("text/plain"));
                if (plainPart != null) multipartAlternative.Add(plainPart);

                var htmlPart = payload.parts.FirstOrDefault(x => x.mimeType.Contains("text/html"));
                if (htmlPart != null) multipartAlternative.Add(htmlPart);
            }

            // Get body content
            if (multipartAlternative != null && multipartAlternative.Count > 0)
            {
                var painPart = multipartAlternative.FirstOrDefault(x => x.mimeType.Contains("text/plain"));
                var htmlPart = multipartAlternative.FirstOrDefault(x => x.mimeType.Contains("text/html"));

                if (painPart != null)
                {
                    part.plainBody = painPart.body;
                    part.plainBody.data = ShaHashData.DecodeBase64String(part.plainBody.data);
                }

                if (htmlPart != null)
                {
                    part.htmlBody = htmlPart.body;
                    if (!string.IsNullOrEmpty(part.htmlBody.data))
                        part.htmlBody.data = ShaHashData.DecodeBase64String(part.htmlBody.data);
                }

                if (htmlPart == null)
                    if (!string.IsNullOrEmpty(payload.body.data))
                        part.htmlBody.data = ShaHashData.DecodeBase64String(payload.body.data);
            }

            return part;
        }

        private string LabelBuilder(string label)
        {
            if (string.IsNullOrEmpty(label)) return "";

            var strLabel = "";
            foreach (var item in label.Split(","))
            {
                switch (item.ToLower())
                {
                    case "inbox":
                        strLabel += "&labelIds=" + item.ToUpper();
                        break;

                    case "draft":
                        strLabel += "&labelIds=" + item.ToUpper();
                        break;

                    case "sent":
                        strLabel += "&labelIds=" + item.ToUpper();
                        break;

                    case "spam":
                        strLabel += "&labelIds=" + item.ToUpper();
                        break;

                    case "trash":
                        strLabel += "&labelIds=" + item.ToUpper();
                        break;

                    default:
                        strLabel += "&labelIds=" + item;
                        break;
                }
            }

            return strLabel.Trim('&');
        }

        // private Task<Contact> GetContactByFromAddress(string from)
        // {
        //     return Task.Run(() =>
        //     {
        //         // from = Name <name@provider.com>
        //         var contact = new Contact();
        //         if (string.IsNullOrEmpty(from)) return Task.FromResult(contact);

        //         if (from.Contains("<"))
        //         {
        //             var email = from.Split("<")[1];
        //             email = email != null ? email.Trim('>') : email;

        //             if (!string.IsNullOrEmpty(email))
        //                 contact = _contactService.GetContactByUserIdAndEmail(user.UserId, email);
        //         }

        //         return Task.FromResult(contact);
        //     });
        // }

        private async Task<bool> MarkGmailEmailAsReadUnread(string messageId, bool? isRead = false)
        {
            var isCompleted = false;
            var modifyMessage = new ModifyMessage();
            modifyMessage.addLabelIds = new List<string>();
            modifyMessage.removeLabelIds = new List<string>();

            if ((bool)isRead)
                modifyMessage.removeLabelIds.Add(DataUtility.GmailUNREAD);
            //DataUtility.GmailUNREAD
            else
                modifyMessage.addLabelIds.Add(DataUtility.GmailUNREAD); // DataUtility.GmailUNREAD

            if (token == null) await SetGmailToken();

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            //var response = await client.PostAsync(string.Format(DataUtility.GmailModifyLabel, messageId), new StringContent(JsonConvert.SerializeObject(modifyMessage), Encoding.UTF8, "application/json"));
            var response = await client.PostAsync(string.Format(DataUtility.GmailModifyLabel, messageId), new StringContent(JsonConvert.SerializeObject(modifyMessage), Encoding.UTF8, "application/json"));
            if (response.StatusCode == HttpStatusCode.OK) isCompleted = true;
            return isCompleted;
        }

        public async Task<InboxThreads> GetLabelWithUnReadCount(int userId, InboxVM model)
        {
            user = _userService.GetUserById(userId);
            if (user == null)
            {
                inboxThreads.IsValid = false;
                inboxThreads.ErrorMessage = CommonMessage.UnAuthorizedUser;
                return inboxThreads;
            }

            // var emailAccount = _emailAccountService.GetEmailAccountById(model.UserEmail.AccountId);
            // userEmail = _userEmailService.GetUserEmailById(model.UserEmail.UserEmailId);
            var intproviderApp = _intProviderAppService.GetIntProviderApp(model.ProviderApp);

            // var intAppSecretObj = _intProviderAppSecretService.GetByUserAndEmail(userId, model.SelectedEmail);
            var intAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUserAndEmail(userId, model.SelectedEmail, intproviderApp.Id);


            if (intproviderApp != null && intAppSecretObj != null)
            {
                mailTokenDto.refresh_token = intAppSecretObj.Refresh_Token;
                mailTokenDto.access_token = intAppSecretObj.Access_Token;
                mailTokenDto.code = model.Code;
                mailTokenDto.ProviderApp = model.Provider;
                mailTokenDto.UserId = userId;
                intProviderAppSecretObj.Refresh_Token = intAppSecretObj.Refresh_Token;
                intProviderAppSecretObj.Access_Token = intAppSecretObj.Access_Token;
                intProviderAppSecretObj.Email = intAppSecretObj.Email;
            }
            inboxThreads.Labels = new List<InboxLabel>();
            switch (intproviderApp.Name)
            {
                case "Gmail":
                    string[] labels = new string[] { "INBOX", "SENT", "TRASH", "DRAFT", "SPAM" };
                    //string[] replaceLabels = new string[] { "Inbox", "Sent", "Trash", "Draft", "Spam" };

                    LabelReadUnReadCount gmailThreadsLabels = null;
                    ThreadLabels gmailLabels = null;
                    await SetGmailToken();
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    //var gmaillabels = await client.GetAsync(DataUtility.GmailGetLabels);
                    var gmaillabels = await client.GetAsync(DataUtility.GmailGetLabels);
                    if (gmaillabels.StatusCode == HttpStatusCode.OK)
                    {
                        var streamLabel = await gmaillabels.Content.ReadAsStreamAsync();
                        var serializer = new DataContractJsonSerializer(typeof(ThreadLabels));
                        gmailLabels = (ThreadLabels)serializer.ReadObject(streamLabel);
                    }

                    if (gmailLabels != null)
                    {
                        foreach (var label in gmailLabels.labels)
                        {
                            if (labels.Contains(label.name))
                            {
                                //var gmailResponse = await client.GetAsync(string.Format(DataUtility.GmailGetLabelCount, label.id));
                                var gmailResponse = await client.GetAsync(string.Format(DataUtility.GmailGetLabelCount, label.id));
                                if (gmailResponse.StatusCode == HttpStatusCode.OK)
                                {
                                    var stream = await gmailResponse.Content.ReadAsStreamAsync();
                                    var serializ = new DataContractJsonSerializer(typeof(LabelReadUnReadCount));
                                    gmailThreadsLabels = (LabelReadUnReadCount)serializ.ReadObject(stream);
                                }

                                if (gmailThreadsLabels != null)
                                {
                                    string GmailLabels = "";
                                    if (label.name == "INBOX") { GmailLabels = "Inbox"; }
                                    else if (label.name == "SENT") { GmailLabels = "Sent"; }
                                    else if (label.name == "DRAFT") { GmailLabels = "Draft"; }
                                    else if (label.name == "TRASH") { GmailLabels = "Trash"; }
                                    else if (label.name == "SPAM") { GmailLabels = "Spam"; }

                                    InboxLabel objLabel = new InboxLabel();
                                    objLabel.id = gmailThreadsLabels.id;
                                    objLabel.name = GmailLabels;
                                    objLabel.threadsTotal = gmailThreadsLabels.threadsTotal;
                                    objLabel.threadsUnread = gmailThreadsLabels.threadsUnread;
                                    inboxThreads.Labels.Add(objLabel);
                                }
                            }
                            else
                            {
                                if (label.type == "user")
                                {
                                    //var gmailResponse = await client.GetAsync(string.Format(DataUtility.GmailGetLabelCount, label.id));
                                    var gmailResponse = await client.GetAsync(string.Format(DataUtility.GmailGetLabelCount, label.id));
                                    if (gmailResponse.StatusCode == HttpStatusCode.OK)
                                    {
                                        var stream = await gmailResponse.Content.ReadAsStreamAsync();
                                        var serializ = new DataContractJsonSerializer(typeof(LabelReadUnReadCount));
                                        gmailThreadsLabels = (LabelReadUnReadCount)serializ.ReadObject(stream);
                                    }

                                    if (gmailThreadsLabels != null)
                                    {
                                        InboxLabel objLabel = new InboxLabel();
                                        objLabel.id = gmailThreadsLabels.id;
                                        objLabel.name = label.name;
                                        objLabel.threadsTotal = gmailThreadsLabels.threadsTotal;
                                        objLabel.threadsUnread = gmailThreadsLabels.threadsUnread;
                                        inboxThreads.Labels.Add(objLabel);
                                    }
                                }
                            }

                        }
                    }

                    break;

                case "Office 365":
                case "Outlook":

                    Office365MailFolders office365MailFolders = null;
                    await SetOffice365Token();
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    var office365Response = await client.GetAsync(DataUtility.Office365Folders);    //DataUtility.Office365Folders
                    if (office365Response.StatusCode == HttpStatusCode.OK)
                    {
                        var json = JObject.Parse(await office365Response.Content.ReadAsStringAsync());
                        office365MailFolders = JsonConvert.DeserializeObject<Office365MailFolders>(json.ToString());

                    }
                    if (office365MailFolders != null && office365MailFolders.value != null && office365MailFolders.value.Count > 0)
                    {
                        // System Folder
                        //string[] folders = new string[] { "Inbox", "Sent Items", "Drafts", "Deleted Items", "Junk Email" };
                        string[] systemfolders = new string[] { "Inbox", "Sent Items", "Drafts", "Deleted Items", "Junk Email", "Archive", "Conversation History", "Outbox" };

                        inboxThreads.Labels = new List<InboxLabel>();
                        foreach (var thread in office365MailFolders.value)
                        {
                            if (systemfolders.Contains(thread.displayName))
                            {
                                string label = "";
                                if (thread.displayName == "Inbox") { label = "Inbox"; }
                                else if (thread.displayName == "Sent Items") { label = "Sent"; }
                                else if (thread.displayName == "Drafts") { label = "Draft"; }
                                else if (thread.displayName == "Deleted Items") { label = "Trash"; }
                                else if (thread.displayName == "Junk Email") { label = "Spam"; }
                                if (label != "")
                                {
                                    InboxLabel objLabel = new InboxLabel();
                                    objLabel.id = thread.id;
                                    objLabel.name = label;
                                    objLabel.threadsTotal = thread.totalItemCount;
                                    objLabel.threadsUnread = thread.unreadItemCount;
                                    inboxThreads.Labels.Add(objLabel);
                                }
                            }
                            else
                            {
                                if (!systemfolders.Contains(thread.displayName))
                                {
                                    InboxLabel objLabel = new InboxLabel();
                                    objLabel.id = thread.id;
                                    objLabel.name = thread.displayName;
                                    objLabel.threadsTotal = thread.totalItemCount;
                                    objLabel.threadsUnread = thread.unreadItemCount;
                                    inboxThreads.Labels.Add(objLabel);
                                }
                            }
                        }

                        //// Custom Folder

                        //foreach (var thread in office365MailFolders.value)
                        //{
                        //    if (!systemfolders.Contains(thread.displayName))
                        //    {
                        //        InboxLabel objLabel = new InboxLabel();
                        //        objLabel.id = thread.id;
                        //        objLabel.name = thread.displayName;
                        //        objLabel.threadsTotal = thread.totalItemCount;
                        //        objLabel.threadsUnread = thread.unreadItemCount;
                        //        inboxThreads.Labels.Add(objLabel);
                        //    }
                        //}
                    }

                    break;

                default:
                    break;
            }
            inboxThreads.IsValid = true;
            return inboxThreads;
        }

        private async Task SetContactDetailInThreadEmail(string Label)
        {
            foreach (var item in inboxThreads.InboxThread)
            {
                //var contact = await GetContactResultByFromAddress(Label.ToLower() == DataUtility.Office365SentItems.ToLower() || Label.ToLower() == DataUtility.GmailSENT.ToLower() ? item.To : item.From);
                var contact = await GetContactResultByFromAddress(Label.ToLower() == DataUtility.Office365SentItems.ToLower() || Label.ToLower() == DataUtility.GmailSENT.ToLower() ? item.To : item.From);
                item.Contact = contact;
                item.FromName = contact != null && contact.ContactId > 0 ? (contact.FirstName + " " + contact.LastName).Trim() : item.FromName;
            }
        }

        private async Task SetNetworkMemberDetailInThreadEmail()
        {
            foreach (var item in inboxThreads.InboxThread)
            {
                var member = await GetMemberResultByFromAddress(item.From);
                item.Member = member;
            }
        }

        private Task<UserContactResult> GetContactResultByFromAddress(string from)
        {
            return Task.Run(() =>
            {
                // from = Name <name@provider.com>
                var contactResult = new UserContactResult();
                var email = from;
                if (string.IsNullOrEmpty(from)) return Task.FromResult(contactResult);

                if (from.Contains("<"))
                {
                    email = from.Split("<")[1];
                    email = email != null ? email.Trim('>') : email;
                }

                if (!string.IsNullOrEmpty(email.Trim()))
                {
                    var contactFilter = new ContactFilter()
                    {
                        ContactEmail = email,
                    };

                    // var contact = _contactService.GetUserContact(user.UserId, contactFilter, null);
                    // if (contact != null && contact.Count > 0) contactResult = contact[0];
                }

                return Task.FromResult(contactResult);
            });
        }

        private Task<UserMemberResult> GetMemberResultByFromAddress(string from)
        {
            return Task.Run(() =>
            {
                // from = Name <name@provider.com>
                var memberResult = new UserMemberResult();
                var email = "";
                if (string.IsNullOrEmpty(from)) return Task.FromResult(memberResult);

                if (from.Contains("<"))
                {
                    email = from.Split("<")[1];
                    email = email != null ? email.Trim('>') : email;
                }

                // if (!string.IsNullOrEmpty(email))
                // {
                //     memberResult = _membersService.GetMemberByMemberEmail(user.UserId, email.Trim());
                // }

                return Task.FromResult(memberResult);
            });
        }

        #endregion

        #region 'Office 365 Private Function'
        private async Task<bool> GetOffice365Token(MailTokenDto model, string redirectUri)
        {
            var isTokenReceived = false;
            client.DefaultRequestHeaders.Clear();

            var param = new Dictionary<string, string>();
            param.Add("grant_type", "authorization_code");
            param.Add("code", model.code);
            param.Add("client_id", DataUtility.Office365ClientId);          //DataUtility.Office365ClientId
            param.Add("client_secret", DataUtility.Office365ClientSecret);  //DataUtility.Office365ClientSecret
            param.Add("redirect_uri", redirectUri);
            param.Add("scope", "offline_access Mail.ReadWrite Mail.Send User.Read");

            //var response = await client.PostAsync(DataUtility.Office365Token, new FormUrlEncodedContent(param));
            var response = await client.PostAsync(DataUtility.Office365Token, new FormUrlEncodedContent(param));
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var serializer = new DataContractJsonSerializer(typeof(Token));
                var office365Token = (Token)serializer.ReadObject(stream);

                if (office365Token != null && !string.IsNullOrEmpty(office365Token.access_token))
                {
                    token = office365Token.access_token;
                    model.access_token = token;
                    model.refresh_token = office365Token.refresh_token;
                    // model.expires_in = Convert.ToString(office365Token.expires_in);
                    model.scope = office365Token.scope;
                    var dt = DataUtility.GetCurrentDateTime();
                    //model.ExpireOn = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second + Convert.ToInt32(model.ExpireIn));
                    model.refresh_token = office365Token.refresh_token;

                    // _userEmailService.CheckInsertOrUpdate(model);
                    isTokenReceived = true;
                }
            }

            return isTokenReceived;
        }

        public async Task SetOffice365Token()
        {
            //string refreshToken = "MCQ9f7R6A4wXq7zCTqdstBug3MHrnpmRO06MoNZ1FT5K9YTveFTc0LJEDlcFU88yVhM65uOnKNB3gMGPx9HTum9hMdlyBUVSJ*Eq!S3BWn9E7pN9FIUCIx*tKWXRwueQZmU7Y0CeBagG1HCfL0Lvz599xB9w44EJxHwgB1!JNS!FP5rrplqFmKonMBN69leMR7WkE2wAoPg!vmIARw10MF4v1kd3tZkerO5nkwEAEVVoZJ0HuK95eT7JBpk!x2XIHQZCzz1RkBgD0v*GvO78zxVSRRHivhSwU5VheDYQQtQ00cfSMjoKGaiAiYBVEFk4YLYXfzdkw7tg4mwYweE669nH3GuohiueRD18d2bEjTdcy6Ukeo5NWYa7kYM4Alm7k*S*a!OTRT2deaPOCJ7ijYb08U5gzM3R4PVuBJWR7rE!k";
            //string clientId = "ac915294-e037-4d25-abb7-d9fdf95a2068";
            //string clientSecret = "hZA041##borhngUYCGC10~|";
            //string code = "M5fe8dc9a-36cb-b022-c5fb-5df13470207c";
            //string scope = "offline_access Mail.ReadWrite User.Read";

            client.DefaultRequestHeaders.Clear();

            var param = new Dictionary<string, string>();
            param.Add("client_id", OneClappContext.MicroSoftClientId);  //DataUtility.Office365ClientId
            param.Add("client_secret", OneClappContext.MicroSecretKey);  //DataUtility.Office365ClientSecret
            param.Add("refresh_token", intProviderAppSecretObj.Refresh_Token);
            param.Add("code", mailTokenDto.code);
            param.Add("scope", MicrosoftScope);
            param.Add("grant_type", "refresh_token");

            //var response = await client.PostAsync(DataUtility.Office365Token, new FormUrlEncodedContent(param));
            var response = await client.PostAsync(DataUtility.Office365Token, new FormUrlEncodedContent(param));

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var serializer = new DataContractJsonSerializer(typeof(Office365Token));
                var office365Token = (Office365Token)serializer.ReadObject(stream);

                if (office365Token != null)
                {
                    token = office365Token.access_token;
                }
            }
        }

        private async Task<bool> MarkOffice365EmailAsReadUnread(string messageId, bool? isRead = false)
        {
            var isCompleted = false;

            if (token == null) await SetOffice365Token();

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            IsReadUnRead objIsReadUnRead = new IsReadUnRead();
            if ((bool)isRead)
                objIsReadUnRead.IsRead = true;
            else
                objIsReadUnRead.IsRead = false;
            //objIsReadUnRead.IsRead = (bool)isRead;

            var myContent = JsonConvert.SerializeObject(objIsReadUnRead);
            var buffer = Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = new HttpMethod("PATCH"),
                RequestUri = new Uri(string.Format(DataUtility.Office365ReadUnread, messageId)), //DataUtility.Office365ReadUnread
                Content = byteContent,
            };
            var response = await client.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.OK) isCompleted = true;
            return isCompleted;
        }

        string strOffice365ThreadId = "";
        private async Task<bool> SetOffice365InboxThreadDetail(Value thread, InboxVM model)
        {
            // if (strOffice365ThreadId != thread.conversationId)
            // {
            strOffice365ThreadId = thread.conversationId;

            InboxThread inboxThread = new InboxThread();
            // inboxThread.ThreadId = thread.id;
            // inboxThread.messageId = thread.conversationId;
            inboxThread.ThreadId = thread.conversationId;
            inboxThread.messageId = thread.id;
            inboxThread.Subject = thread.subject != null ? thread.subject : "";
            inboxThread.FromName = thread.from != null ? thread.from.emailAddress.name.ToString() : "";
            inboxThread.From = thread.from != null ? thread.from.emailAddress.address.ToString() : "";
            if (thread.toRecipients != null && thread.toRecipients.Length > 0)
            {
                if (thread.toRecipients[0].emailAddress != null)
                {
                    if (!string.IsNullOrEmpty(thread.toRecipients[0].emailAddress.address))
                    {
                        inboxThread.To = thread.toRecipients[0].emailAddress.address.ToString();
                        inboxThread.ToEmail = thread.toRecipients[0].emailAddress.address.ToString();
                    }
                    else
                    {
                        inboxThread.To = "";
                        inboxThread.ToEmail = "";
                    }

                }
            }
            // inboxThread.To = thread.toRecipients != null && thread.toRecipients.Length > 0 ? thread.toRecipients[0].emailAddress.address.ToString() : "";
            // inboxThread.ToEmail = thread.toRecipients != null && thread.toRecipients.Length > 0 ? thread.toRecipients[0].emailAddress.address : null;
            inboxThread.FromEmail = thread.from != null && thread.from.emailAddress != null ? thread.from.emailAddress.address : "";

            // To Email
            string toEmails = "";
            string toNames = "";
            int i = 0;
            if (thread.toRecipients.Length > 0)
            {
                foreach (var item in thread.toRecipients)
                {
                    if (i == 0) { toEmails = item.emailAddress.address; toNames = item.emailAddress.name; }
                    else { toEmails += ", " + item.emailAddress.address; toNames += ", " + item.emailAddress.name; }
                    i++;
                }
            }
            inboxThread.To = thread.toRecipients.Length > 0 ? toEmails : null;
            inboxThread.ToEmail = thread.toRecipients.Length > 0 ? toEmails : null;
            inboxThread.ToName = thread.toRecipients.Length > 0 ? toNames : null;

            // Cc Email
            string ccEmails = "";
            string ccNames = "";
            i = 0;
            if (thread.ccRecipients.Length > 0)
            {
                foreach (var item in thread.ccRecipients)
                {
                    if (i == 0) { ccEmails = item.emailAddress.address; ccNames = item.emailAddress.name; }
                    else { ccEmails += ", " + item.emailAddress.address; ccNames += ", " + item.emailAddress.name; }
                    i++;
                }
            }
            inboxThread.CcEmail = thread.ccRecipients.Length > 0 ? ccEmails : null;
            inboxThread.CcName = thread.ccRecipients.Length > 0 ? ccNames : null;

            // Bcc Email
            string bccEmails = "";
            string bccNames = "";
            i = 0;
            if (thread.bccRecipients.Length > 0)
            {
                foreach (var item in thread.bccRecipients)
                {
                    if (i == 0) { bccEmails = item.emailAddress.address; bccNames = item.emailAddress.name; }
                    else { bccEmails += ", " + item.emailAddress.address; bccNames += ", " + item.emailAddress.name; }
                    i++;
                }
            }
            inboxThread.BccEmail = thread.bccRecipients.Length > 0 ? ccEmails : null;
            inboxThread.BccName = thread.bccRecipients.Length > 0 ? ccNames : null;

            inboxThread.Date = thread.receivedDateTime != null ? thread.receivedDateTime : "";
            inboxThread.IsHasAttachment = thread.hasAttachments;
            inboxThread.InternalDate = DataUtility.ConvertDateTimeToUnixTimeStamp(Convert.ToDateTime(thread.receivedDateTime));
            inboxThread.CreatedOn = matcrm.service.Common.Common.UnixTimeStampToDateTimeMilliSec(inboxThread.InternalDate);
            inboxThread.IsOpen = thread.isRead;
            inboxThread.IsRead = thread.isRead;
            inboxThread.Snippet = thread.bodyPreview;
            // inboxThread.AccountId = model.UserEmail != null ? model.UserEmail.AccountId : 0;
            // inboxThread.UserEmailId = model.UserEmail != null ? model.UserEmail.UserEmailId : 0;
            // inboxThread.AccountAlias = model.UserEmail != null ? model.UserEmail.Alias : "";
            inboxThreads.InboxThread.Add(inboxThread);

            // }
            return true;
        }

        private async Task<List<Body>> GetOffice365Attachment(string threadId)
        {
            var attactments = await client.GetAsync(string.Format(DataUtility.Office365Attachment, threadId)); //DataUtility.Office365Attachment
            if (attactments.StatusCode == HttpStatusCode.OK)
            {
                AttachmentFiles attachmentFiles = null;
                var json = await attactments.Content.ReadAsStringAsync();
                // var json = JObject.Parse(await attactments.Content.ReadAsStringAsync());
                attachmentFiles = JsonConvert.DeserializeObject<AttachmentFiles>(json.ToString());

                AttachmentFiles InboxThreadItem = new AttachmentFiles();
                List<Body> fileListItem = new List<Body>();
                foreach (var item in attachmentFiles.value)
                {
                    Body objAttach = new Body();
                    objAttach.fileName = item.name;
                    objAttach.contentType = item.contentType;
                    objAttach.size = item.size;
                    objAttach.contentBytes = item.contentBytes;
                    objAttach.bytes = Convert.FromBase64String(item.contentBytes);

                    var ext = item.name != "" ? item.name.Split(".") : null;
                    if (ext != null && ext.Length >= 2)
                    {
                        objAttach.extention = ext != null ? ext[1] : "undefined";
                    }
                    else
                    {
                        objAttach.extention = "undefined";
                    }
                    fileListItem.Add(objAttach);
                }
                return fileListItem;
            }
            else
            {
                return null;
            }
        }

        private async Task<Body> GetGmailAttachment(string messageId, string attachmentId)
        {

            var response = await client.GetAsync(string.Format(DataUtility.GmailAttachment, messageId, attachmentId)); //DataUtility.Office365Attachment
            Body? bodyData = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var serializer = new DataContractJsonSerializer(typeof(Body));
                var attachment = (Body)serializer.ReadObject(stream);
                var contentdata = attachment.data;
                String attachData = attachment.data.Replace('-', '+');
                attachData = attachData.Replace('_', '/');
                bodyData = new Body();
                bodyData.data = attachment.data;//Utility.DecodeBase64String(attachment.data);
                byte[] data = Convert.FromBase64String(attachData);
                bodyData.bytes = data;
                bodyData.contentBytes = attachData;
                bodyData.size = attachment.size;
                bodyData.fileName = attachment.fileName;
                bodyData.contentType = attachment.contentType;

                var ext = !string.IsNullOrEmpty(attachment.fileName) ? attachment.fileName.Split(".") : null;
                if (ext != null && ext.Length >= 2)
                {
                    bodyData.extention = ext != null ? ext[1] : "undefined";
                }
                else
                {
                    bodyData.extention = "undefined";
                }
                return bodyData;
            }
            else
            {
                return null;
            }
        }

        public async Task<Value> Office365ForwardEmail(int userId, string threadId, ComposeEmail1 composeEmail1)
        {
            Value objValue = new Value();

            user = _userService.GetUserById(userId);
            if (user == null)
            {
                objValue.IsValid = false;
                objValue.ErrorMessage = CommonMessage.DefaultErrorMessage;
                return objValue;
            }

            var intproviderApp = _intProviderAppService.GetIntProviderApp(composeEmail1.ProviderApp);

            var intAppSecretObj = _intProviderAppSecretService.GetByUserAndEmail(userId, composeEmail1.SelectedEmail);


            if (intproviderApp != null && intAppSecretObj != null)
            {
                mailTokenDto.refresh_token = intAppSecretObj.Refresh_Token;
                mailTokenDto.access_token = intAppSecretObj.Access_Token;
                // mailTokenDto.code = model.Code;
                mailTokenDto.ProviderApp = composeEmail1.Provider;
                mailTokenDto.UserId = userId;
                intProviderAppSecretObj.Refresh_Token = intAppSecretObj.Refresh_Token;
                intProviderAppSecretObj.Access_Token = intAppSecretObj.Access_Token;
                intProviderAppSecretObj.Email = intAppSecretObj.Email;
            }


            await SetOffice365Token();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var office365ForwardMessage = ConvertOffice365Message(composeEmail1);

            CreateMailMessage createMessageBody = new CreateMailMessage();
            createMessageBody.message = office365ForwardMessage;
            createMessageBody.saveToSentItems = "true";

            var myContent = JsonConvert.SerializeObject(createMessageBody);
            var ChangeAttach = myContent.Replace("odatatype", "@odata.type");
            var buffer = Encoding.UTF8.GetBytes(ChangeAttach);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            // var myContent = JsonConvert.SerializeObject(office365ForwardMessage);
            // var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            // var byteContent = new ByteArrayContent(buffer);
            // byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.PostAsync(string.Format(DataUtility.Office365ForwardEmail, threadId), byteContent); //DataUtility.Office365ForwardEmail
            if (response.StatusCode == HttpStatusCode.Created)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var serializer = new DataContractJsonSerializer(typeof(Value));
                objValue = (Value)serializer.ReadObject(stream);

                objValue.IsValid = true;
                objValue.ErrorMessage = "Success";
            }
            else
            {
                objValue.IsValid = false;
                objValue.ErrorMessage = CommonMessage.DefaultErrorMessage;
            }
            return objValue;
        }

        public async Task<Value> Office365ReplayEmail(int userId, string threadId, ComposeEmail1 composeEmail1)
        {
            Value objValue = new Value();

            user = _userService.GetUserById(userId);
            if (user == null)
            {
                objValue.IsValid = true;
                objValue.ErrorMessage = CommonMessage.DefaultErrorMessage;
                return objValue;
            }

            var intproviderApp = _intProviderAppService.GetIntProviderApp(composeEmail1.ProviderApp);

            var intAppSecretObj = _intProviderAppSecretService.GetByUserAndEmail(userId, composeEmail1.SelectedEmail);


            if (intproviderApp != null && intAppSecretObj != null)
            {
                mailTokenDto.refresh_token = intAppSecretObj.Refresh_Token;
                mailTokenDto.access_token = intAppSecretObj.Access_Token;
                // mailTokenDto.code = model.Code;
                mailTokenDto.ProviderApp = composeEmail1.Provider;
                mailTokenDto.UserId = userId;
                intProviderAppSecretObj.Refresh_Token = intAppSecretObj.Refresh_Token;
                intProviderAppSecretObj.Access_Token = intAppSecretObj.Access_Token;
                intProviderAppSecretObj.Email = intAppSecretObj.Email;
            }

            await SetOffice365Token();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            var office365ForwardMessage = ConvertOffice365Message(composeEmail1);

            // var myContent = JsonConvert.SerializeObject(office365ForwardMessage);
            // var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            // var byteContent = new ByteArrayContent(buffer);
            // byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            CreateMailMessage createMessageBody = new CreateMailMessage();
            createMessageBody.message = office365ForwardMessage;
            createMessageBody.saveToSentItems = "true";

            var myContent = JsonConvert.SerializeObject(createMessageBody);
            var ChangeAttach = myContent.Replace("odatatype", "@odata.type");
            var buffer = Encoding.UTF8.GetBytes(ChangeAttach);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.PostAsync(string.Format(DataUtility.Office365ReplayEmail, threadId), byteContent); //DataUtility.Office365ReplayEmail
            if (response.StatusCode == HttpStatusCode.Created)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var serializer = new DataContractJsonSerializer(typeof(Value));
                objValue = (Value)serializer.ReadObject(stream);

                objValue.IsValid = true;
                objValue.ErrorMessage = "Success";
            }
            else
            {
                objValue.IsValid = false;
                objValue.ErrorMessage = CommonMessage.DefaultErrorMessage;
            }
            return objValue;
        }

        public async Task<MessageSent> Office365Attachments(string threadId, Attachments office365Message)
        {
            MessageSent objValue = new MessageSent();

            user = _userService.GetUserById(office365Message.UserId);
            if (user == null)
            {
                objValue.IsValid = true;
                objValue.ErrorMessage = CommonMessage.DefaultErrorMessage;
                return objValue;
            }

            var intproviderApp = _intProviderAppService.GetIntProviderApp(office365Message.ProviderApp);

            var intAppSecretObj = _intProviderAppSecretService.GetByUserAndEmail(office365Message.UserId, office365Message.SelectedEmail);


            if (intproviderApp != null && intAppSecretObj != null)
            {
                mailTokenDto.refresh_token = intAppSecretObj.Refresh_Token;
                mailTokenDto.access_token = intAppSecretObj.Access_Token;
                // mailTokenDto.code = model.Code;
                mailTokenDto.ProviderApp = office365Message.Provider;
                mailTokenDto.UserId = office365Message.UserId;
                intProviderAppSecretObj.Refresh_Token = intAppSecretObj.Refresh_Token;
                intProviderAppSecretObj.Access_Token = intAppSecretObj.Access_Token;
                intProviderAppSecretObj.Email = intAppSecretObj.Email;
            }

            await SetOffice365Token();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var myContent = JsonConvert.SerializeObject(office365Message);
            var changeAttach = myContent.Replace("odatatype", "@odata.type");
            var buffer = System.Text.Encoding.UTF8.GetBytes(changeAttach);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.PostAsync(string.Format(DataUtility.Office365Attachment, threadId), byteContent); //DataUtility.Office365Attachment
                                                                                                                          // if (response.StatusCode == HttpStatusCode.Accepted)
            if (response.StatusCode == HttpStatusCode.Created)
            {
                objValue.IsValid = true;
                objValue.ErrorMessage = "Success";
            }
            else
            {
                objValue.IsValid = false;
                objValue.ErrorMessage = CommonMessage.DefaultErrorMessage;
            }
            return objValue;
        }

        public async Task<Value> Office365CreateDraft(int userId, ComposeEmail1 composeMessage)
        {
            Value objValue = new Value();

            user = _userService.GetUserById(userId);
            if (user == null)
            {
                return objValue;
            }


            var intproviderApp = _intProviderAppService.GetIntProviderApp(composeMessage.ProviderApp);
            var intAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUserAndEmail(userId, composeMessage.SelectedEmail, intproviderApp.Id);

            // var intAppSecretObj = _intProviderAppSecretService.GetByUserAndEmail(userId, composeMessage.SelectedEmail);


            if (intproviderApp != null && intAppSecretObj != null)
            {
                mailTokenDto.refresh_token = intAppSecretObj.Refresh_Token;
                mailTokenDto.access_token = intAppSecretObj.Access_Token;
                // mailTokenDto.code = model.Code;
                mailTokenDto.ProviderApp = composeMessage.Provider;
                mailTokenDto.UserId = userId;
                intProviderAppSecretObj.Refresh_Token = intAppSecretObj.Refresh_Token;
                intProviderAppSecretObj.Access_Token = intAppSecretObj.Access_Token;
                intProviderAppSecretObj.Email = intAppSecretObj.Email;
            }

            await SetOffice365Token();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            Office365Message office365Message = new Office365Message();
            Office365Body objBody = new Office365Body();
            List<Torecipient> objToList = new List<Torecipient>();
            List<Ccrecipient> objCcList = new List<Ccrecipient>();
            List<Bccrecipient> objBccList = new List<Bccrecipient>();
            List<MailAttachments> objAttachmentList = new List<MailAttachments>();
            CreateMailMessage createMessageBody = new CreateMailMessage();

            office365Message.subject = composeMessage.Subject;
            objBody.contentType = "Html";
            objBody.content = composeMessage.Body;
            office365Message.body = objBody;

            foreach (var item in composeMessage.To)
            {
                Torecipient objTo = new Torecipient();
                Emailaddress objEmail = new Emailaddress();
                objEmail.address = item.Contains("<") ? item.Split("<")[1].Trim('>').Trim() : item;
                objEmail.name = item.Contains("<") ? item.Split("<")[0].Trim() : item;
                objTo.emailAddress = objEmail;
                objToList.Add(objTo);
            }
            office365Message.toRecipients = objToList.ToArray();

            foreach (var item in composeMessage.Cc)
            {
                Ccrecipient objCc = new Ccrecipient();
                Emailaddress objEmail = new Emailaddress();
                objEmail.address = item.Contains("<") ? item.Split("<")[1].Trim('>').Trim() : item;
                objEmail.name = item.Contains("<") ? item.Split("<")[0].Trim() : item;
                objCc.emailAddress = objEmail;

                objCcList.Add(objCc);
            }
            office365Message.ccRecipients = objCcList.ToArray();

            foreach (var item in composeMessage.Bcc)
            {
                Bccrecipient objBcc = new Bccrecipient();
                Emailaddress objEmail = new Emailaddress();
                objEmail.address = item.Contains("<") ? item.Split("<")[1].Trim('>').Trim() : item;
                objEmail.name = item.Contains("<") ? item.Split("<")[0].Trim() : item;
                objBcc.emailAddress = objEmail;

                objBccList.Add(objBcc);
            }
            office365Message.bccRecipients = objBccList.ToArray();

            //// Attachment
            var files = composeMessage.FileList;

            if (files != null)
            {
                foreach (var item in files)
                {
                    if (item.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            item.CopyTo(ms);
                            MailAttachments objAttachments = new MailAttachments();
                            string base64 = Convert.ToBase64String(ms.ToArray());
                            objAttachments.odatatype = "#microsoft.graph.fileAttachment";
                            objAttachments.contentBytes = base64;
                            //objAttachments.contentType = "";
                            objAttachments.name = item.FileName;
                            objAttachmentList.Add(objAttachments);
                        }
                    }
                }
            }
            // if (composeMessage.FileList != null)
            // {
            //     foreach (var item in composeMessage.FileList)
            //     {
            //         if (item.Length > 0)
            //         {
            //             using (var ms = new MemoryStream())
            //             {
            //                 item.CopyTo(ms);
            //                 MailAttachments objAttachments = new MailAttachments();
            //                 string base64 = Convert.ToBase64String(ms.ToArray());
            //                 objAttachments.odatatype = "#microsoft.graph.fileAttachment";
            //                 objAttachments.contentBytes = base64;
            //                 //objAttachments.contentType = "";
            //                 objAttachments.name = item.FileName;
            //                 objAttachmentList.Add(objAttachments);
            //             }
            //         }
            //     }
            // }

            if (composeMessage.FileIds != null)
            {
                // foreach (var item in model.FileIds)
                // {
                //     var file = new FileDto();
                //     var dPath = _hostingEnvironment.WebRootPath + "\\Uploads\\resized";
                //     var fPath = dPath + "\\" + "file-not-found.jpg";
                //     if (item > 0)
                //     {
                //         var imageByte = fileUpload.DownloadFile(item, Convert.ToBoolean(SystemSettingService.Dictionary["AZFLS"]), FileUploadSettings.Container, ref file);    // FileUploadSettings.FileUpload
                //         Attachments objAttachments = new Attachments();
                //         string base64 = Convert.ToBase64String(imageByte);
                //         objAttachments.odatatype = "#microsoft.graph.fileAttachment";
                //         objAttachments.contentBytes = base64;
                //         objAttachments.contentType = file.FileType;
                //         objAttachments.name = file.FileName + "." + file.Extention;
                //         objAttachmentList.Add(objAttachments);
                //     }
                // }
            }

            if (composeMessage.QuickEmailAttachments != null)
            {
                Byte[] imageByte = new Byte[0];
                var dPath = _hostingEnvironment.WebRootPath + "\\AttachEmailPicture";
                foreach (var item in composeMessage.QuickEmailAttachments)
                {
                    if (System.IO.File.Exists(dPath + "\\" + item.FileName))
                    {
                        imageByte = System.IO.File.ReadAllBytes(dPath + "\\" + item.FileName);
                    }
                    else
                    {
                        imageByte = System.IO.File.ReadAllBytes(dPath + "\\" + "file-not-found.jpg");
                    }

                    MailAttachments objAttachments = new MailAttachments();
                    string base64 = Convert.ToBase64String(imageByte);
                    objAttachments.odatatype = "#microsoft.graph.fileAttachment";
                    objAttachments.contentBytes = base64;
                    //objAttachments.contentType = "image/jpeg";
                    objAttachments.name = item.OrignalFileName;
                    objAttachmentList.Add(objAttachments);
                }
            }

            office365Message.attachments = objAttachmentList.ToArray();

            var myContent = JsonConvert.SerializeObject(office365Message);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.PostAsync(DataUtility.Office365CreateMessage, byteContent); //DataUtility.Office365CreateMessage

            if (response.StatusCode == HttpStatusCode.Created)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var serializer = new DataContractJsonSerializer(typeof(Value));
                objValue = (Value)serializer.ReadObject(stream);
                objValue.IsValid = true;
            }
            else
            {
                objValue.IsValid = false;
                objValue.ErrorMessage = CommonMessage.DefaultErrorMessage;
            }
            return objValue;
        }
        public Office365Message ConvertOffice365Message(ComposeEmail1 composeEmail1)
        {
            Office365Message office365Message = new Office365Message();
            Office365Body objBody = new Office365Body();
            List<Torecipient> objToList = new List<Torecipient>();
            List<Ccrecipient> objCcList = new List<Ccrecipient>();
            List<Bccrecipient> objBccList = new List<Bccrecipient>();
            List<MailAttachments> objAttachmentList = new List<MailAttachments>();
            CreateMailMessage createMessageBody = new CreateMailMessage();

            office365Message.subject = composeEmail1.Subject;
            objBody.contentType = "Html";
            objBody.content = composeEmail1.Body;
            office365Message.body = objBody;


            foreach (var item in composeEmail1.To)
            {
                Torecipient objTo = new Torecipient();
                Emailaddress objEmail = new Emailaddress();
                objEmail.address = item.Contains("<") ? item.Split("<")[1].Trim('>').Trim() : item;
                objEmail.name = item.Contains("<") ? item.Split("<")[0].Trim() : item;
                objTo.emailAddress = objEmail;
                objToList.Add(objTo);
            }
            office365Message.toRecipients = objToList.ToArray();

            foreach (var item in composeEmail1.Cc)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    Ccrecipient objCc = new Ccrecipient();
                    Emailaddress objEmail = new Emailaddress();
                    objEmail.address = item.Contains("<") ? item.Split("<")[1].Trim('>').Trim() : item;
                    objEmail.name = item.Contains("<") ? item.Split("<")[0].Trim() : item;
                    objCc.emailAddress = objEmail;
                    objCcList.Add(objCc);
                }
            }
            if (objCcList.Count() > 0)
            {
                office365Message.ccRecipients = objCcList.ToArray();
            }


            foreach (var item in composeEmail1.Bcc)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    Bccrecipient objBcc = new Bccrecipient();
                    Emailaddress objEmail = new Emailaddress();
                    objEmail.address = item.Contains("<") ? item.Split("<")[1].Trim('>').Trim() : item;
                    objEmail.name = item.Contains("<") ? item.Split("<")[0].Trim() : item;
                    objBcc.emailAddress = objEmail;
                    objBccList.Add(objBcc);
                }
            }
            if (objBccList.Count() > 0)
            {
                office365Message.bccRecipients = objBccList.ToArray();
            }
            // office365Message.bccRecipients = objBccList.ToArray();

            //// Attachment
            if (composeEmail1.FileList != null)
            {
                foreach (var item in composeEmail1.FileList)
                {
                    if (item.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            item.CopyTo(ms);
                            MailAttachments objAttachments = new MailAttachments();
                            string base64 = Convert.ToBase64String(ms.ToArray());
                            objAttachments.odatatype = "#microsoft.graph.fileAttachment";
                            objAttachments.contentBytes = base64;
                            //objAttachments.contentType = "";
                            objAttachments.name = item.FileName;
                            objAttachmentList.Add(objAttachments);
                        }
                    }
                }
            }

            if (composeEmail1.FileIds != null)
            {
                // foreach (var item in model.FileIds)
                // {
                //     var file = new FileDto();
                //     var dPath = _hostingEnvironment.WebRootPath + "\\Uploads\\resized";
                //     var fPath = dPath + "\\" + "file-not-found.jpg";
                //     if (item > 0)
                //     {
                //         var imageByte = fileUpload.DownloadFile(item, Convert.ToBoolean(SystemSettingService.Dictionary["AZFLS"]), FileUploadSettings.Container, ref file);    // FileUploadSettings.FileUpload
                //         Attachments objAttachments = new Attachments();
                //         string base64 = Convert.ToBase64String(imageByte);
                //         objAttachments.odatatype = "#microsoft.graph.fileAttachment";
                //         objAttachments.contentBytes = base64;
                //         objAttachments.contentType = file.FileType;
                //         objAttachments.name = file.FileName + "." + file.Extention;
                //         objAttachmentList.Add(objAttachments);
                //     }
                // }
            }

            if (composeEmail1.QuickEmailAttachments != null)
            {
                Byte[] imageByte = new Byte[0];
                var dPath = _hostingEnvironment.WebRootPath + "\\AttachEmailPicture";
                foreach (var item in composeEmail1.QuickEmailAttachments)
                {
                    if (System.IO.File.Exists(dPath + "\\" + item.FileName))
                    {
                        imageByte = System.IO.File.ReadAllBytes(dPath + "\\" + item.FileName);
                    }
                    else
                    {
                        imageByte = System.IO.File.ReadAllBytes(dPath + "\\" + "file-not-found.jpg");
                    }

                    MailAttachments objAttachments = new MailAttachments();
                    string base64 = Convert.ToBase64String(imageByte);
                    objAttachments.odatatype = "#microsoft.graph.fileAttachment";
                    objAttachments.contentBytes = base64;
                    //objAttachments.contentType = "image/jpeg";
                    objAttachments.name = item.OrignalFileName;
                    objAttachmentList.Add(objAttachments);
                }
            }

            office365Message.attachments = objAttachmentList.ToArray();

            return office365Message;

        }

        public async Task<InboxThreads> TrashEmail(int userId, InboxVM model, string messageId)
        {
            var user = _userService.GetUserById(userId);
            if (user == null)
            {
                inboxThreads.IsValid = false;
                inboxThreads.ErrorMessage = CommonMessage.UnAuthorizedUser;
                // inboxThreads.ErrorMessage = "UnAuthorize";
                return inboxThreads;
            }

            var task = new List<Task>();
            var intproviderApp = _intProviderAppService.GetIntProviderApp(model.ProviderApp);
            var intAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUserAndEmail(userId, model.SelectedEmail, intproviderApp.Id);

            if (intproviderApp != null && intAppSecretObj != null)
            {
                mailTokenDto.refresh_token = intAppSecretObj.Refresh_Token;
                mailTokenDto.access_token = intAppSecretObj.Access_Token;
                mailTokenDto.code = model.Code;
                mailTokenDto.ProviderApp = model.Provider;
                mailTokenDto.UserId = userId;
                intProviderAppSecretObj.Refresh_Token = intAppSecretObj.Refresh_Token;
                intProviderAppSecretObj.Access_Token = intAppSecretObj.Access_Token;
                intProviderAppSecretObj.Email = intAppSecretObj.Email;
            }

            switch (intproviderApp.Name)
            {
                case "Gmail":
                    await SetGmailToken();
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    model.Query = "";
                    var api = string.Format(DataUtility.GmailMessageTrash, messageId);
                    //var gmailResponse = await client.GetAsync(string.Format(DataUtility.GmailThreads, LabelBuilder(model.Label), 30, model.NextPageToken, model.Query));
                    var gmailResponse = await client.PostAsync(api, null);
                    if (gmailResponse.StatusCode == HttpStatusCode.OK)
                    {
                        // var stream = await gmailResponse.Content.ReadAsStreamAsync();
                        // var serializer = new DataContractJsonSerializer(typeof(Message));
                        // gmailMessage = (Message)serializer.ReadObject(stream);
                        inboxThreads.IsValid = true;
                        inboxThreads.ErrorMessage = "Message was trashed";
                    }
                    else
                    {
                        inboxThreads.IsValid = false;
                        inboxThreads.ErrorMessage = CommonMessage.DefaultErrorMessage;
                    }

                    break;

                case "Office 365":
                case "Outlook":

                    await SetOffice365Token();
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    Office365Thread office365Threads = null;
                    string office365Uri = "";

                    if (model.Label.ToLower() == "all") model.Label = DataUtility.Office365All;// DataUtility.Office365All;
                    else if (model.Label.ToLower() == "inbox") model.Label = DataUtility.Office365Inbox;// DataUtility.Office365Inbox;
                    else if (model.Label.ToLower() == "sent") model.Label = DataUtility.Office365SentItems; //DataUtility.Office365SentItems;
                    else if (model.Label.ToLower() == "draft") model.Label = DataUtility.Office365Drafts; // DataUtility.Office365Drafts;
                    else if (model.Label.ToLower() == "trash") model.Label = DataUtility.Office365DeletedItems;      // DataUtility.Office365DeletedItems;
                    else if (model.Label.ToLower() == "spam") model.Label = DataUtility.Office365JunkEmail;
                    office365Uri = String.Format(DataUtility.Office365MoveEmail, messageId);

                    MessageMove messageMove = new MessageMove();
                    messageMove.destinationId = DataUtility.Office365DeletedItems;

                    var myContent = JsonConvert.SerializeObject(messageMove);
                    var ChangeAttach = myContent.Replace("odatatype", "@odata.type");
                    var buffer = Encoding.UTF8.GetBytes(ChangeAttach);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var office365Response = await client.PostAsync(office365Uri, byteContent);
                    if (office365Response.StatusCode == HttpStatusCode.Created)
                    {

                        inboxThreads.IsValid = true;
                        inboxThreads.ErrorMessage = "Message was trashed";
                    }
                    else
                    {
                        inboxThreads.IsValid = false;
                        inboxThreads.ErrorMessage = CommonMessage.DefaultErrorMessage;
                    }

                    break;

                default:
                    break;
            }
            return inboxThreads;
        }


        public async Task<InboxThreads> TrashEmailByThread(int userId, InboxVM model, string threadId)
        {

            var user = _userService.GetUserById(userId);
            if (user == null)
            {
                inboxThreads.IsValid = false;
                inboxThreads.ErrorMessage = CommonMessage.UnAuthorizedUser;
                // inboxThreads.ErrorMessage = "UnAuthorize";
                return inboxThreads;
            }

            var task = new List<Task>();
            var intproviderApp = _intProviderAppService.GetIntProviderApp(model.ProviderApp);
            var intAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUserAndEmail(userId, model.SelectedEmail, intproviderApp.Id);

            if (intproviderApp != null && intAppSecretObj != null)
            {
                mailTokenDto.refresh_token = intAppSecretObj.Refresh_Token;
                mailTokenDto.access_token = intAppSecretObj.Access_Token;
                mailTokenDto.code = model.Code;
                mailTokenDto.ProviderApp = model.Provider;
                mailTokenDto.UserId = userId;
                intProviderAppSecretObj.Refresh_Token = intAppSecretObj.Refresh_Token;
                intProviderAppSecretObj.Access_Token = intAppSecretObj.Access_Token;
                intProviderAppSecretObj.Email = intAppSecretObj.Email;
            }

            switch (intproviderApp.Name)
            {
                case "Gmail":
                    await SetGmailToken();
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    inboxThreadItems = new List<InboxThreadItem>();
                    InboxVM objInbox = new InboxVM();
                    // objInbox.UserEmail = userEmail;
                    // await SetInboxThreadDetail(threadId, objInbox, true);
                    task.Add(SetInboxThreadDetail(threadId, objInbox, true));
                    Task.WaitAll(task.ToArray());

                    foreach (var item in inboxThreadItems)
                    {

                        var api = string.Format(DataUtility.GmailMessageTrash, item.MessageId);
                        //var gmailResponse = await client.GetAsync(string.Format(DataUtility.GmailThreads, LabelBuilder(model.Label), 30, model.NextPageToken, model.Query));
                        var gmailResponse = await client.PostAsync(api, null);
                        if (gmailResponse.StatusCode == HttpStatusCode.OK)
                        {
                        }
                        else
                        {
                            inboxThreads.IsValid = false;
                            inboxThreads.ErrorMessage = CommonMessage.DefaultErrorMessage;
                        }
                    }
                    break;

                case "Office 365":
                case "Outlook":

                    await SetOffice365Token();
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    Office365Thread office365Threads = null;
                    string office365Uri = "";
                    await SetOffice365Token();
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    inboxThreadItems = new List<InboxThreadItem>();
                    var url = string.Format(DataUtility.Office365ThreadByConversionId, "'" + threadId + "'");
                    var threadResponse = await client.GetAsync(url);
                    if (threadResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var stream = await threadResponse.Content.ReadAsStreamAsync();

                        var serializer = new DataContractJsonSerializer(typeof(MessageConversation));
                        var threadDetail = (MessageConversation)serializer.ReadObject(stream);
                        if (threadDetail != null)
                        {
                            if (threadDetail.value != null)
                            {
                                foreach (var message in threadDetail.value)
                                {
                                    office365Uri = String.Format(DataUtility.Office365MoveEmail, message.id);

                                    MessageMove messageMove = new MessageMove();
                                    messageMove.destinationId = DataUtility.Office365DeletedItems;

                                    var myContent = JsonConvert.SerializeObject(messageMove);
                                    var ChangeAttach = myContent.Replace("odatatype", "@odata.type");
                                    var buffer = Encoding.UTF8.GetBytes(ChangeAttach);
                                    var byteContent = new ByteArrayContent(buffer);
                                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                                    var office365Response = await client.PostAsync(office365Uri, byteContent);
                                    if (office365Response.StatusCode == HttpStatusCode.Created)
                                    {
                                    }
                                    else
                                    {
                                        inboxThreads.IsValid = false;
                                        inboxThreads.ErrorMessage = CommonMessage.DefaultErrorMessage;
                                    }
                                }
                            }
                        }
                    }
                    break;

                default:
                    break;
            }
            inboxThreads.IsValid = true;
            inboxThreads.ErrorMessage = "";
            return inboxThreads;
        }


        //public async Task<MessageSent> GetOffice365EmailNotification(Notifications office365Notification)
        //{
        //    MessageSent objValue = new MessageSent();

        //    await SetOffice365Token();
        //    client.DefaultRequestHeaders.Clear();
        //    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

        //    var myContent = JsonConvert.SerializeObject(office365Notification);
        //    var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
        //    var byteContent = new ByteArrayContent(buffer);
        //    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        //    var response = await client.PostAsync(DataUtility.Office365Notification, byteContent);
        //    //if (response.StatusCode == HttpStatusCode.Accepted)
        //    //{
        //    objValue.IsValid = true;
        //    objValue.ErrorMessage = CommonMessage.MessageSentSuccessMsg;
        //    //}
        //    //else
        //    //{
        //    //    objValue.IsValid = true;
        //    //    objValue.ErrorMessage = CommonMessage.DefaultErrorMessage;
        //    //}
        //    return objValue;
        //}
        #endregion

        public async Task<ComposeEmail1> SendReply(int userId, ComposeEmail1 model, IFormFile[] files)
        {
            user = _userService.GetUserById(userId);
            if (user == null)
            {
                model.IsValid = false;
                model.ErrorMessage = CommonMessage.UnAuthorizedUser;
                return model;
            }

            var intproviderApp = _intProviderAppService.GetIntProviderApp(model.ProviderApp);

            var intAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUserAndEmail(userId, model.SelectedEmail, intproviderApp.Id);


            if (intproviderApp != null && intAppSecretObj != null)
            {
                mailTokenDto.refresh_token = intAppSecretObj.Refresh_Token;
                mailTokenDto.access_token = intAppSecretObj.Access_Token;
                // mailTokenDto.code = model.Code;
                mailTokenDto.ProviderApp = model.Provider;
                mailTokenDto.UserId = userId;
                intProviderAppSecretObj.Refresh_Token = intAppSecretObj.Refresh_Token;
                intProviderAppSecretObj.Access_Token = intAppSecretObj.Access_Token;
                intProviderAppSecretObj.Email = intAppSecretObj.Email;
            }
            switch (intproviderApp.Name)
            {
                case "Gmail":
                    var message = new GmailCompose();
                    MailMessage mail = new MailMessage();

                    mail.Subject = model.Subject;
                    mail.Body = model.Body;
                    mail.IsBodyHtml = true;
                    if (model.To.Count > 0 && !model.IsSendMassEmail)
                        mail.To.Add(string.Join(',', model.To).Trim(','));

                    if (model.Cc.Count > 0)
                        mail.CC.Add(string.Join(',', model.Cc).Trim(','));

                    if (model.Bcc.Count > 0)
                        mail.Bcc.Add(string.Join(',', model.Cc).Trim(','));

                    if (files != null)
                    {
                        foreach (var item in files)
                        {
                            if (item.Length > 0)
                            {
                                using (var ms = new MemoryStream())
                                {
                                    item.CopyTo(ms);
                                    Attachment objAttachment = new Attachment(new MemoryStream(ms.ToArray()), item.FileName);//, string mediaType
                                    if (objAttachment != null)
                                        mail.Attachments.Add(objAttachment);
                                }
                            }
                        }
                    }

                    if (model.FileIds != null)
                    {
                        foreach (var item in model.FileIds)
                        {
                            var file = new FileDto();
                            var dPath = _hostingEnvironment.WebRootPath + "\\Uploads\\resized";
                            var fPath = dPath + "\\" + "file-not-found.jpg";
                            if (item > 0)
                            {
                                byte[] imageByte = fileUpload.DownloadFile(item, Convert.ToBoolean(FileUploadSettings.FileUpload), FileUploadSettings.Container, ref file); // FileUploadSettings.FileUpload
                                Stream streamArray = new MemoryStream(imageByte.ToArray());
                                Attachment objAttachment = new Attachment(streamArray, file.FileName + "." + file.Extention);
                                if (objAttachment != null)
                                    mail.Attachments.Add(objAttachment);
                            }
                        }
                    }

                    if (model.QuickEmailAttachments != null)
                    {
                        Byte[] imageByte = new Byte[0];
                        var dPath = _hostingEnvironment.WebRootPath + "\\AttachEmailPicture";
                        foreach (var item in model.QuickEmailAttachments)
                        {
                            if (System.IO.File.Exists(dPath + "\\" + item.FileName))
                            {
                                imageByte = System.IO.File.ReadAllBytes(dPath + "\\" + item.FileName);
                                Stream streamArray = new MemoryStream(imageByte.ToArray());
                                Attachment objAttachment = new Attachment(streamArray, item.OrignalFileName);
                                if (objAttachment != null)
                                    mail.Attachments.Add(objAttachment);
                            }
                        }

                    }

                    await SetGmailToken();
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    // For recipients receive email as if it was only sent to them
                    var response = new HttpResponseMessage();
                    if (model.IsSendMassEmail)
                    {
                        int it = 0;
                        foreach (var item in model.To)
                        {
                            if (mail.To.Count > 0) mail.To.RemoveAt(0);
                            mail.To.Add(item);
                            MimeKit.MimeMessage mimeMessage = MimeKit.MimeMessage.CreateFromMailMessage(mail);
                            if (model.mailType == "forward")
                            {
                                if (!mail.Subject.StartsWith("Fw:", StringComparison.OrdinalIgnoreCase))
                                    mimeMessage.Subject = "Fw:" + mail.Subject;
                            }
                            else
                            {
                                if (!mail.Subject.StartsWith("Re:", StringComparison.OrdinalIgnoreCase))
                                    mimeMessage.Subject = "Re:" + mail.Subject;
                                else
                                    mimeMessage.Subject = mail.Subject;
                            }

                            if (!string.IsNullOrEmpty(mimeMessage.MessageId) && model.mailType != "forward")
                            {
                                mimeMessage.InReplyTo = model.messageId;
                                foreach (var id in mimeMessage.References)
                                    mimeMessage.References.Add(id);
                                mimeMessage.References.Add(mimeMessage.MessageId);
                            }
                            message.raw = DataUtility.Base64UrlEncode
                                (mimeMessage.ToString());
                            message.threadId = model.threadId;
                            response = await client.PostAsync(DataUtility.GmailMessageSend, new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json"));

                            it++;
                        }
                    }
                    else
                    {
                        MimeKit.MimeMessage mimeMessage = MimeKit.MimeMessage.CreateFromMailMessage(mail);
                        if (model.mailType == "forward")
                        {
                            if (!mail.Subject.StartsWith("Fw:", StringComparison.OrdinalIgnoreCase))
                                mimeMessage.Subject = "Fw:" + mail.Subject;
                        }
                        else
                        {
                            if (!mail.Subject.StartsWith("Re:", StringComparison.OrdinalIgnoreCase))
                                mimeMessage.Subject = "Re:" + mail.Subject;
                            else
                                mimeMessage.Subject = mail.Subject;
                        }
                        if (!string.IsNullOrEmpty(model.messageId) && model.mailType != "forward")
                        {
                            mimeMessage.InReplyTo = model.messageId;
                            foreach (var id in mimeMessage.References)
                                mimeMessage.References.Add(id);
                            mimeMessage.References.Add(model.messageId);
                        }
                        message.raw = DataUtility.Base64UrlEncode
                            (mimeMessage.ToString());
                        message.threadId = model.threadId;
                        response = await client.PostAsync(DataUtility.GmailMessageSend, new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json"));
                    }

                    //var response = await client.PostAsync(SystemSettingService.Dictionary["GMLSM"], new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json"));
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var stream = await response.Content.ReadAsStreamAsync();
                        var serializer = new DataContractJsonSerializer(typeof(GmailSent));
                        model.MailSent = (GmailSent)serializer.ReadObject(stream);

                        if (string.IsNullOrEmpty(model.MailSent.id))
                        {
                            model.IsValid = false;
                            model.ErrorMessage = CommonMessage.ErrorOccurredEmailSend;
                            return model;
                        }
                    }
                    else
                    {
                        var stream = await response.Content.ReadAsStreamAsync();
                        var serializer = new DataContractJsonSerializer(typeof(GmailError));
                        model.GmailError = (GmailError)serializer.ReadObject(stream);

                        if (model.GmailError != null && !string.IsNullOrEmpty(model.GmailError.error.message))
                        {
                            model.IsValid = false;
                            model.ErrorMessage = model.GmailError.error.code + "-" + model.GmailError.error.message;
                        }

                        return model;
                    }

                    break;

                case "Office 365":
                case "Outlook":

                    await SetOffice365Token();
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    model.messageId = null;
                    var office365ForwardMessage = ConvertOffice365Message(model);

                    CreateMailMessage createMessageBody = new CreateMailMessage();
                    createMessageBody.message = office365ForwardMessage;
                    createMessageBody.saveToSentItems = "true";

                    var myContent = JsonConvert.SerializeObject(createMessageBody);
                    var ChangeAttach = myContent.Replace("odatatype", "@odata.type");
                    var buffer = Encoding.UTF8.GetBytes(ChangeAttach);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    response = await client.PostAsync(string.Format(DataUtility.Office365ReplayEmail, model.messageId), byteContent); //DataUtility.Office365ReplayEmail
                    Value objValue = new Value();
                    if (response.StatusCode == HttpStatusCode.Created)
                    {
                        var stream = await response.Content.ReadAsStreamAsync();
                        var serializer = new DataContractJsonSerializer(typeof(Value));
                        objValue = (Value)serializer.ReadObject(stream);

                        model.IsValid = true;
                        model.ErrorMessage = "Success";
                    }
                    else
                    {
                        model.IsValid = false;
                        model.ErrorMessage = CommonMessage.DefaultErrorMessage;
                    }
                    break;
            }

            model.IsValid = true;
            model.ErrorMessage = CommonMessage.EmailSent;

            return model;
        }

        public async Task<List<CustomEmailFolder>> GetFolders(MailTokenDto model, int userId)
        {
            List<CustomEmailFolder> folders = new List<CustomEmailFolder>();
            var customEmailDomainObj = _customDomainEmailConfigService.GetByUserAndEmail(userId, model.SelectedEmail);
            if (customEmailDomainObj != null)
            {
                // try
                // {
                using (var client = new ImapClient())
                {
                    var options = SecureSocketOptions.StartTlsWhenAvailable;
                    options = SecureSocketOptions.SslOnConnect;

                    #region Host, Port, UserName, Passwrod with different host
                    string Host = customEmailDomainObj.IMAPHost;
                    int Port = customEmailDomainObj.IMAPPort.Value;
                    string UserName = model.SelectedEmail;
                    string Password = customEmailDomainObj.Password;

                    #endregion

                    client.CheckCertificateRevocation = false;
                    client.ServerCertificateValidationCallback = (s, c, ch, e) => true;

                    client.Connect(Host, Port, options);

                    //client.AuthenticationMechanisms.Remove("XOAUTH2");

                    client.Authenticate(UserName, Password);

                    //Check IsConnected condition
                    if (client.IsConnected)
                    {
                        // The Inbox folder is always available on all IMAP servers...
                        var inbox = client.Inbox;
                        inbox.Open(FolderAccess.ReadOnly);

                        // Get the first personal namespace and list the toplevel folders under it.
                        var personal = client.GetFolder(client.PersonalNamespaces[0]);
                        foreach (var folder in personal.GetSubfolders(false))
                        {
                            CustomEmailFolder emailFolder = new CustomEmailFolder();
                            emailFolder.Name = folder.Name;
                            emailFolder.Count = folder.Count;
                            folders.Add(emailFolder);
                        }
                    }
                }
                return folders;
            }
            return folders;
        }
    }
}
