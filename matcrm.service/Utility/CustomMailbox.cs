using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Context;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Security;
using matcrm.data.Models.Dto.CustomEmail;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Http;
using System.IO;
using matcrm.data.Models.Dto;
using MimeKit;
using MailboxAddress = matcrm.data.Models.Dto.CustomEmail.MailboxAddress;
using matcrm.service.Services;
using MailKit.Search;

namespace matcrm.service.Utility
{
    public class CustomMailbox
    {
        private readonly OneClappContext _context;
        private readonly ICustomDomainEmailConfigService _customDomainEmailConfigService;
        public CustomMailbox(OneClappContext context,
        ICustomDomainEmailConfigService customDomainEmailConfigService)
        {
            _context = context;
            _customDomainEmailConfigService = customDomainEmailConfigService;
        }

        public async Task<List<CustomEmailFolder>> GetThread()
        {
            DataTable table = new DataTable();
            List<CustomEmailFolder> folders = new List<CustomEmailFolder>();
            List<CustomEmailInbox> listData = new List<CustomEmailInbox>();
            var list = new List<object>();
            // try
            // {
            using (var client = new ImapClient())
            {
                var options = SecureSocketOptions.StartTlsWhenAvailable;
                options = SecureSocketOptions.SslOnConnect;

                #region Host, Port, UserName, Passwrod with different host

                string Host = "mail.techavidus.com";
                int Port = 993;
                // string UserName = "yagnik.darji@techavidus.com";
                // string Password = "8[)6fUV+'}";
                string UserName = "shraddha.prajapati@techavidus.com";
                string Password = "SD@TA10";

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
                    var messageIds = new List<string>();
                    messageIds.Add("CA+qzq=tYyRm3e6sa2J1bVFd77HVANVXJ6FO=Bo7jOjrHxkJLFQ@mail.gmail.com");

                    // client.Inbox.Search(SearchQuery.MessageId );

                    //var draft = client.Inbox.Search(SearchQuery.Draft);

                    ////This is ue to get email ids
                    //var mailIds = client.Inbox.Search(SearchQuery.NotSeen);
                    //var mailIds1 = client.Inbox.Search(SearchQuery.Deleted);

                    // Get the first personal namespace and list the toplevel folders under it.
                    var personal = client.GetFolder(client.PersonalNamespaces[0]);
                    Console.WriteLine("Folders", personal.GetSubfolders(false));
                    foreach (var folder in personal.GetSubfolders(false))
                    {
                        CustomEmailFolder emailFolder = new CustomEmailFolder();
                        emailFolder.Name = folder.Name;
                        emailFolder.Count = folder.Count;
                        // folder.Open(FolderAccess.ReadWrite);
                        // var uids = folder.Search(MailKit.Search.SearchQuery.HeaderContains("Message-Id", "78ae5f76f1684065aa334e67441c4140@techavidus.com"));
                        // if (uids.Count() > 0)
                        //     folder.AddFlags(uids, MessageFlags.Draft, silent: true);
                        if (folder.Name == "drafts")
                        {

                            int folderss = folder.Count;
                            // var notseenIds = folder.Search(MailKit.Search.SearchQuery.NotSeen);

                            //  var seenIds = folder.Search(MailKit.Search.SearchQuery.).Select(t => t.);
                            //For loog for getting email details
                            for (int j = folder.Count - 1; j >= 0; j--)
                            {
                                folder.Open(FolderAccess.ReadOnly);
                                var message = folder.GetMessage(j);
                                // folder.AddFlags(new int[] { j }, MessageFlags.Deleted, silent: true);
                                // var messages = folder.Fetch(notseenIds);
                                var objStr = message.ToString();
                                // if(notseenIds.Contains())
                                CustomEmailInbox customEmailInbox = new CustomEmailInbox();
                                customEmailInbox.MessageId = message.MessageId;
                                customEmailInbox.Subject = message.Subject;
                                customEmailInbox.HtmlBody = message.HtmlBody;
                                var date1 = message.Date.UtcDateTime;
                                customEmailInbox.Date = date1;
                                // customEmailInbox.To = message.To;
                                // customEmailInbox.From = message.From;
                                customEmailInbox.InReplyTo = message.InReplyTo;
                                customEmailInbox.HtmlBody = message.HtmlBody;
                                customEmailInbox.Priority = message.Priority.ToString();
                                foreach (var item in message.From.Mailboxes)
                                {
                                    MailboxAddress fromObj = new MailboxAddress();
                                    fromObj.Address = item.Address;
                                    fromObj.Name = item.Name;
                                    customEmailInbox.From.Add(fromObj);
                                }
                                foreach (var item in message.To.Mailboxes)
                                {
                                    MailboxAddress toObj = new MailboxAddress();
                                    toObj.Address = item.Address;
                                    toObj.Name = item.Name;
                                    customEmailInbox.To.Add(toObj);
                                }
                                // customEmailInbox.From = message.From;
                                // customEmailInbox.To = message.To;

                                // if (message.From != null && message.From.Count() > 0)
                                // {
                                //     var data = message.From.Cast<InternetAddress>().ToList();
                                // }
                                // if (message.To != null && message.To.Count() > 0)
                                // {
                                //     customEmailInbox.To = message.To.Cast<InternetAddress>().ToList();
                                // }


                                // customEmailInbox.Mailboxes = message.Mailboxes;

                                // var test =  JsonConvert.DeserializeObject<CustomEmailInbox>(objStr);
                                listData.Add(customEmailInbox);

                                //if (message.Subject.ToLower() == "test thread")
                                //{
                                list.Add(message);
                                emailFolder.Messages.Add(customEmailInbox);
                                //table.Rows.Add(message);
                                //Console.WriteLine("Subject: {0}", message.Subject);
                                //}
                                //Console.WriteLine("Subject: {0}", message.Subject);                                    
                            }
                        }
                        folders.Add(emailFolder);
                        //Console.WriteLine("[folder] {0}", folder.Name);
                    }

                    //int count = inbox.Count;
                    //var recentmessage = inbox.Recent;

                    //for (int i = 0; i < inbox.Count; i++)
                    //{
                    //    var message = inbox.GetMessage(i);
                    //    Console.WriteLine("Subject: {0}", message.Subject);
                    //}
                }
            }
            // var stringdata = Convert.ToString(list);
            // return true;
            // var data = JsonConvert.DeserializeObject<List<CustomEmailInbox>>(stringdata);
            // foreach (var item in folders)
            // {

            // }
            return folders;
            // }
            // catch (System.Exception ex)
            // {

            //     throw ex;
            // }

        }

        public async Task<List<CustomEmailFolder>> GetFolders(ComposeEmail1 model, int userId)
        {
            List<CustomEmailFolder> folders = new List<CustomEmailFolder>();
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
                    string UserName = model.SelectedEmail;
                    string Password = customEmailDomainObj.Password;

                    #endregion

                    client.CheckCertificateRevocation = false;
                    client.ServerCertificateValidationCallback = (s, c, ch, e) => true;

                    client.Connect(Host, Port, options);

                    client.Authenticate(UserName, Password);

                    var uids = client.Inbox.Search(SearchQuery.HeaderContains("Message-Id", ""));
                    client.Inbox.AddFlags(uids, MessageFlags.Deleted, silent: true);

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

        public async Task<bool> SendEmail(ComposeEmail1 model, int userId)
        {
            var customEmailDomainObj = _customDomainEmailConfigService.GetByUserAndEmail(userId, model.SelectedEmail);
            if (customEmailDomainObj != null)
            {
                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                mailMessage.From = new MailAddress("bamate.2022@yahoo.com");
                // mailMessage.From = new MailAddress(model.SelectedEmail);
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
                // mailMessage.CC.Add("bemate.2022@outlook.com");
                // var filepath = @"C:\Users\shraddha\Pictures\test111.jpg";
                IFormFile[] files = new IFormFile[] { };
                if (model.FileList != null)
                {
                    foreach (var item in model.FileList)
                    {
                        if (item.Length > 0)
                        {
                            using (var ms = new MemoryStream())
                            {
                                item.CopyTo(ms);
                                System.Net.Mail.Attachment objAttachment = new System.Net.Mail.Attachment(new MemoryStream(ms.ToArray()), item.FileName);//, string mediaType
                                if (objAttachment != null)
                                    mailMessage.Attachments.Add(objAttachment);
                            }
                        }
                    }
                }
                // SmtpClient smtpClient = new SmtpClient(customEmailDomainObj.SMTPHost);
                // smtpClient.Port = customEmailDomainObj.SMTPPort.Value;
                // smtpClient.Credentials = new NetworkCredential(customEmailDomainObj.Email, customEmailDomainObj.Password);
                // smtpClient.EnableSsl = true;
                // smtpClient.Send(mailMessage);
                SmtpClient smtpClient = new SmtpClient("smtp.mail.yahoo.com");
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential("bamate.2022@yahoo.com", "joeamqzngdolibgj");
                smtpClient.EnableSsl = true;
                smtpClient.Send(mailMessage);
            }
            return true;
        }

        public async Task<bool> SendReply(ComposeEmail1 model, int userId, string messageId)
        {
            var customEmailDomainObj = _customDomainEmailConfigService.GetByUserAndEmail(userId, model.SelectedEmail);
            if (customEmailDomainObj != null)
            {
                System.Net.Mail.MailMessage originalMessage;
                MimeMessage message;
                // using (var imap = new ImapClient())
                // {
                //     var options = SecureSocketOptions.StartTlsWhenAvailable;
                //     options = SecureSocketOptions.SslOnConnect;

                //     #region Host, Port, UserName, Passwrod with different host
                //     string Host = customEmailDomainObj.IMAPHost;
                //     int Port = customEmailDomainObj.IMAPPort.Value;
                //     string UserName = model.SelectedEmail;
                //     string Password = customEmailDomainObj.Password;

                //     #endregion

                //     imap.CheckCertificateRevocation = false;
                //     imap.ServerCertificateValidationCallback = (s, c, ch, e) => true;

                //     imap.Connect(Host, Port, options);

                //     imap.Authenticate(UserName, Password);

                //     // string search = @"SUBJECT " + model.Subject + "";
                //     // var query = SearchQuery.SubjectContains(model.Subject);
                //     // var uuIds = imap.Inbox.Search(query);
                //     // message = imap.Inbox.GetMessage(uuIds[0]);
                //     // imap.GetMessage(uuIds.First());
                //     // imap.Inbox.Fetch(uuIds);
                // }
                model.From = model.SelectedEmail;
                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                mailMessage.From = new MailAddress(model.From);
                // mailMessage.From = new MailAddress(model.SelectedEmail);
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
                // mailMessage.CC.Add("bemate.2022@outlook.com");
                // var filepath = @"C:\Users\shraddha\Pictures\test111.jpg";
                IFormFile[] files = new IFormFile[] { };
                if (model.FileList != null)
                {
                    foreach (var item in model.FileList)
                    {
                        if (item.Length > 0)
                        {
                            using (var ms = new MemoryStream())
                            {
                                item.CopyTo(ms);
                                System.Net.Mail.Attachment objAttachment = new System.Net.Mail.Attachment(new MemoryStream(ms.ToArray()), item.FileName);//, string mediaType
                                if (objAttachment != null)
                                    mailMessage.Attachments.Add(objAttachment);
                            }
                        }
                    }
                }

                //         mailMessage.Headers.Add(
                //    new MimeKit.Header(HeaderId.InReplyTo, model.threadId));
                //         mailMessage.MimeEntity.Headers.Add(
                //             new Header(HeaderId.References, model.threadId));

                // mailMessage.Headers.Add("reply-to", model.threadId);
                mailMessage.Headers.Add("reply-to", model.To[0]);
                mailMessage.Headers.Add("References", messageId);

                // Set subject.
                // mailMessage.Subject = $"Re: {model.Subject}";
                mailMessage.Subject = model.Subject;

                // Set reply message message.
                mailMessage.Body = model.Body;
                // mailMessage.ReplyToList.Add(new MailAddress("oneclapp.2021@gmail.com", "reply-to"));

                // Append Body message text.
                // mailMessage.BodyHtml = "Test reply";


                SmtpClient smtpClient = new SmtpClient(customEmailDomainObj.SMTPHost);
                smtpClient.Port = customEmailDomainObj.SMTPPort.Value;
                smtpClient.Credentials = new NetworkCredential(customEmailDomainObj.Email, customEmailDomainObj.Password);
                smtpClient.EnableSsl = true;
                smtpClient.Send(mailMessage);
                // // SmtpClient smtpClient = new SmtpClient(customEmailDomainObj.SMTPHost);
                // // smtpClient.Port = customEmailDomainObj.SMTPPort.Value;
                // // smtpClient.Credentials = new NetworkCredential(customEmailDomainObj.Email, customEmailDomainObj.Password);
                // // smtpClient.EnableSsl = true;
                // // smtpClient.Send(mailMessage);
                // SmtpClient smtpClient = new SmtpClient("smtp.mail.yahoo.com");
                // smtpClient.Port = 587;
                // smtpClient.Credentials = new NetworkCredential("bamate.2022@yahoo.com", "joeamqzngdolibgj");
                // smtpClient.EnableSsl = true;
                // smtpClient.Send(mailMessage);
            }
            return true;
        }

        public async Task<bool> DeleteMessage(MailTokenDto model, int userId, string messageId)
        {
            List<CustomEmailFolder> folders = new List<CustomEmailFolder>();
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
                    string UserName = model.SelectedEmail;
                    string Password = customEmailDomainObj.Password;

                    #endregion

                    client.CheckCertificateRevocation = false;
                    client.ServerCertificateValidationCallback = (s, c, ch, e) => true;

                    client.Connect(Host, Port, options);

                    client.Authenticate(UserName, Password);
                    if (client.IsConnected)
                    {
                        var personal = client.GetFolder(client.PersonalNamespaces[0]);
                        var customFolders = personal.GetSubfolders(false);
                        foreach (var item in customFolders)
                        {
                            if(item.Name.ToLower() == model.label.ToLower()){
                                model.label = item.Name;
                            }
                        }
                        var inbox = client.GetFolder(model.label);
                        var trash = client.GetFolder(SpecialFolder.Trash);
                        inbox.Open(FolderAccess.ReadWrite);
                        var uids = inbox.Search(SearchQuery.HeaderContains("Message-Id", messageId));

                        foreach (var uid in uids)
                        {
                            var moved = inbox.MoveTo(uid, trash);
                            // var message1 = inbox.GetMessage(uid);
                            // inbox.AddFlags(uid, MessageFlags.Deleted, true);
                        }
                        client.Disconnect(true);
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }

    }
}