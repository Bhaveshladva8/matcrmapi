using System;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using matcrm.data.Context;
using matcrm.data.Models.Dto;

namespace matcrm.service.Utility {
    public class EmailManager {
        public static void SendMail (string mTo, string mSubject, string mBody) {
            SendMail (mTo, mSubject, mBody, null);
        }

        public static void SendMail (string mTo, string mSubject, string mBody, Attachment Attachments) //AttachmentCollection
        {
            string mailBody = mBody;

            string mailServer = "smtp.gmail.com";
            string mailUser = "example@gmail.com";
            string mailPassword = "admin@123";
            string mailTo = mTo;
            string mailFrom = "do_not_reply@mailinaotor.com";
            int port = 587;

            if (string.IsNullOrEmpty (mTo))
                mTo = mailTo;

            MailMessage mailMessageObj = new MailMessage ();
            mailMessageObj.To.Add (mTo);
            mailMessageObj.From = new MailAddress (mailFrom, mailFrom);

            mailMessageObj.Subject = mSubject;
            mailMessageObj.IsBodyHtml = true;
            mailMessageObj.Body = mailBody;
            mailMessageObj.BodyEncoding = System.Text.Encoding.UTF8;
            mailMessageObj.SubjectEncoding = System.Text.Encoding.UTF8;
            //rvb 27/11/2016 Comment out for moment, but we may re-instate later if Origin want this feature
            if (Attachments != null) {
                //foreach (var attachment in Attachments)
                //{
                mailMessageObj.Attachments.Add (Attachments);
                //}
            }

            SmtpClient smtp = new SmtpClient ();

            smtp.Host = mailServer;
            smtp.Port = port;
            // smtp.EnableSsl = true;
            System.Net.NetworkCredential basicAuthenticationInfo = new System.Net.NetworkCredential (mailUser, mailPassword);
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = basicAuthenticationInfo;

            smtp.Send (mailMessageObj);
        }

        #region Send Email Async
        public static async Task<bool> SendMailAsync (string mTo, string mSubject, string mBody, EmailProviderConfigDto model) {
            try {
                var success = await Task.Run (() => SendMailAsync (mTo, mSubject, mBody, null, model));
                return success;
            } catch (Exception ex) {
                throw ex;
            }
        }

        public static async Task<bool> SendMailAsyncWithAttachment (string mTo, string mSubject, string mBody, EmailProviderConfigDto model, Attachment Attachments) {
            try {
                var success = await Task.Run (() => SendMailAsync (mTo, mSubject, mBody, Attachments, model));
                return success;
            } catch (Exception ex) {
                throw ex;
            }
        }

        public static async Task SendMailSendGridAsync (string mTo, string mSubject, string mBody, Attachment Attachments) //AttachmentCollection
        {
            MailMessage mailMessageObj = new MailMessage ();

            // To
            mailMessageObj.To.Add (new MailAddress (mTo, "To Name"));

            // From
            mailMessageObj.From = new MailAddress ("no-reply@propertyflow.co.nz", "PropertyFlow");

            // Subject and multipart/alternative Body
            mailMessageObj.Subject = mSubject;
            //string text = "text body";
            string html = mBody;
            //mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
            mailMessageObj.AlternateViews.Add (AlternateView.CreateAlternateViewFromString (html, null, MediaTypeNames.Text.Html));

            // Init SmtpClient and send
            SmtpClient smtpClient = new SmtpClient ("smtp.sendgrid.net", Convert.ToInt32 (587));
            // System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("azure_05d15ceb8d2a3d1d844b4cf64871f933@azure.com", "homebuzz_email_123");
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential ("azure_d15681e31e77cde6dba08a5fffd3ac92@azure.com", "propertylight_123");
            smtpClient.Credentials = credentials;

            smtpClient.Send (mailMessageObj);
        }

        public static async Task<bool> SendMailAsync (string mTo, string mSubject, string mBody, Attachment Attachments, EmailProviderConfigDto model) //AttachmentCollection
        {
            string mailBody = mBody;
            string mailServer = OneClappContext.MailServer;
            string mailTo = mTo;
            // string mailFrom = "shraddha.prof21@gmail.com";
            // string mailPassword = "Shraddha@123";

            string mailFrom = OneClappContext.MailFrom;
            // string mailPassword = "Admin@123";
            string mailPassword = OneClappContext.MailPassword;

            int port = 587;

            if (model != null && model.Email != null) {
                mailServer = model.Host;
                mailFrom = model.Email;
                port = model.Port.Value;
                mailPassword = ShaHashData.DecodePassWord(model.Password);
            }

            // string mailFrom = "mw77767@gmail.com";
            // string mailPassword = "MW@techavidus.com";

            if (string.IsNullOrEmpty (mTo))
                mTo = mailTo;

            MailMessage mailMessageObj = new MailMessage ();
            mailMessageObj.To.Add (mTo);
            mailMessageObj.From = new MailAddress (mailFrom, mailFrom);

            mailMessageObj.Subject = mSubject;
            mailMessageObj.IsBodyHtml = true;
            mailMessageObj.Body = mailBody;
            mailMessageObj.BodyEncoding = System.Text.Encoding.UTF8;
            mailMessageObj.SubjectEncoding = System.Text.Encoding.UTF8;

            //rvb 27/11/2016 Comment out for moment, but we may re-instate later if Origin want this feature
            if (Attachments != null) {
                //foreach (var attachment in Attachments)
                //{
                mailMessageObj.Attachments.Add (Attachments);
                //}
            }

            SmtpClient smtp = new SmtpClient ();

            smtp.Host = mailServer;
            smtp.Port = port;
            smtp.EnableSsl = true;
            System.Net.NetworkCredential basicAuthenticationInfo = new System.Net.NetworkCredential (mailFrom, mailPassword);
            //  System.Net.NetworkCredential basicAuthenticationInfo = new System.Net.NetworkCredential (mailUser, mailPassword);
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = basicAuthenticationInfo;
            try {
                smtp.Send(mailMessageObj);
                return true;
            } catch (Exception ex) {
                return false;
                throw ex;
            }

            Console.WriteLine ("after mail sent");
        }
        #endregion
    }
}