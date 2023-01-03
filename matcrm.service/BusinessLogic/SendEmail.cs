using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data.Context;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.service.Services;
using matcrm.service.Utility;

namespace matcrm.service.BusinessLogic
{
    public class SendEmail
    {
        #region Object Initialization
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEmailLogService _emailLogService;
        private readonly IEmailConfigService _emailConfigService;
        private readonly IEmailProviderService _emailProvider;
        private IMapper _mapper;
        #endregion

        #region Constructor
        public SendEmail(
            IEmailTemplateService emailTemplateService,
            IEmailLogService emailLogService,
            IEmailConfigService emailConfigService,
            IEmailProviderService emailProvider,
            IMapper mapper
        )
        {
            _emailTemplateService = emailTemplateService;
            _emailLogService = emailLogService;
            _emailConfigService = emailConfigService;
            _emailProvider = emailProvider;
            _mapper = mapper;
        }

        public async Task SendSignUpEmail(string email, string TempGuid, string receiverName, string verificationCode, long? tenantId, string AccessToken)
        {
            // Send Signup email
            var subject = OneClappContext.ProjectName + " - Thanks for signing up with us!";
            // var body = _emailTemplateService.GetEmailTemplateByCode ("SIGUP").TemplateHtml;
            var body = "<div style='border:1px solid #dedede; box-shadow:0px 0px 5px #dddddd; padding:10px'>  <p>Welcome</p>    <p>Thank you for using " + OneClappContext.ProjectName + ".</p>    <p>To complete the identification process, please verify your email address.</p>    <p><a href='[@VerifyUrl]' target='_blank'>Click here to verify your email address</a></p>    <p>Verification Code: [@VerificationCode]</p>    <p><strong>As a verified " + OneClappContext.ProjectName + "  <p>The " + OneClappContext.ProjectName + " team</p>  </div>";
            body = body.Replace("[@CompanyName]", OneClappContext.ProjectName).Replace("[@ToName]", receiverName).Replace("[@VerifyUrl]", OneClappContext.AppURL + "verification/true/" + TempGuid).Replace("[@UserName]", email).Replace("[@VerificationCode]", verificationCode);
            // body = body.Replace("[@CompanyName]", OneClappContext.ProjectName).Replace("[@ToName]", receiverName).Replace("[@VerifyUrl]", OneClappContext.AppURL + "verify/" + AccessToken).Replace("[@UserName]", email).Replace("[@VerificationCode]", verificationCode);
            try
            {
                // var IsSuccess = await EmailManager.SendMailAsync (email, subject, body);
                var IsSuccess = false;
                if (tenantId != null)
                {
                    var emailConfig = GetEmailConfig(tenantId.Value);
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, emailConfig);
                }
                else
                {
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, null);
                }
                await AddUpdateEmailLog(body, subject, email, IsSuccess, "SIGN", tenantId);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task SendNewSignUpEmail(string email, string TempGuid, string receiverName, string verificationCode, long? tenantId, string AccessToken)
        {
            // Send Signup email
            var subject = OneClappContext.ProjectName + " - Thanks for signing up with us!";
            // var body = _emailTemplateService.GetEmailTemplateByCode ("SIGUP").TemplateHtml;
            var body = "<div style='border:1px solid #dedede; box-shadow:0px 0px 5px #dddddd; padding:10px'>  <p>Welcome</p>    <p>Thank you for using " + OneClappContext.ProjectName + ".</p>    <p>To complete the identification process, please verify your email address.</p>    <p><a href='[@VerifyUrl]' target='_blank'>Click here to verify your email address</a></p>    <p>Verification Code: [@VerificationCode]</p>    <p><strong>As a verified " + OneClappContext.ProjectName + "  <p>The " + OneClappContext.ProjectName + " team</p>  </div>";
            body = body.Replace("[@CompanyName]", OneClappContext.ProjectName).Replace("[@ToName]", receiverName).Replace("[@VerifyUrl]", OneClappContext.AppURL + "verify/" + AccessToken).Replace("[@UserName]", email).Replace("[@VerificationCode]", verificationCode);
            // body = body.Replace("[@CompanyName]", OneClappContext.ProjectName).Replace("[@ToName]", receiverName).Replace("[@VerifyUrl]", OneClappContext.AppURL + "verify/" + AccessToken).Replace("[@UserName]", email).Replace("[@VerificationCode]", verificationCode);
            try
            {
                // var IsSuccess = await EmailManager.SendMailAsync (email, subject, body);
                var IsSuccess = false;
                if (tenantId != null)
                {
                    var emailConfig = GetEmailConfig(tenantId.Value);
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, emailConfig);
                }
                else
                {
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, null);
                }
                await AddUpdateEmailLog(body, subject, email, IsSuccess, "SIGN", tenantId);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task InviteUserVerifyEmail(string email, string TempGuid, string receiverName, string verificationCode, long? tenantId)
        {
            // Send Signup email
            var subject = OneClappContext.ProjectName + " - Account Password Set";
            var body = "<div style='border:1px solid #dedede;box-shadow: 0px 0px 5px #dddddd;padding:10px;'>  <p>Dear [@ToName],<br /> Your password changed. Verify your account url : <a href='[@VerifyUrl]' target='_blank'>Click here.</a>   Verification Code: [@VerificationCode]</p>    <p><br />  Regards,</p>    <p>[@CompanyName]</p>    <p>THIS IS AN AUTOMATED EMAIL, PLEASE DO NOT REPLY</p>  </div>";
            // body = body.Replace ("[@CompanyName]", OneClappContext.ProjectName).Replace ("[@ToName]", receiverName).Replace ("[@VerifyUrl]", OneClappContext.AppURL + "pages/auth/reset-password/" + userid + "/" + guid).Replace ("[@VerificationCode]", verificationCode);
            body = body.Replace("[@CompanyName]", OneClappContext.ProjectName).Replace("[@ToName]", receiverName).Replace("[@VerifyUrl]", OneClappContext.AppURL + "verification/true/" + TempGuid).Replace("[@VerificationCode]", verificationCode);
            try
            {
                // var IsSuccess = await EmailManager.SendMailAsync (email, subject, body);
                var IsSuccess = false;
                if (tenantId != null)
                {
                    var emailConfig = GetEmailConfig(tenantId.Value);
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, emailConfig);
                }
                else
                {
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, null);
                }
                await AddUpdateEmailLog(body, subject, email, IsSuccess, "INVTE", tenantId);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task SendPasswordResetEmail(string email, string userid, string guid, string receiverName, string verificationCode, long? tenantId)
        {
            // Send Signup email
            var subject = OneClappContext.ProjectName + " - Account Password Reset";
            // var body = _emailTemplateService.GetEmailTemplateByCode ("PSWRT").TemplateHtml;
            var body = "<div style='border:1px solid #dedede;box-shadow: 0px 0px 5px #dddddd;padding:10px;'>  <p>Dear [@ToName],<br />  Verify your url: <a href='[@VerifyUrl]' target='_blank'>Click here.</a>   Verification Code: [@VerificationCode]</p>    <p><br />  Regards,</p>    <p>[@CompanyName]</p>    <p>THIS IS AN AUTOMATED EMAIL, PLEASE DO NOT REPLY</p>  </div>";
            // body = body.Replace ("[@CompanyName]", OneClappContext.ProjectName).Replace ("[@ToName]", receiverName).Replace ("[@VerifyUrl]", OneClappContext.AppURL + "pages/auth/reset-password/" + userid + "/" + guid).Replace ("[@VerificationCode]", verificationCode);
            body = body.Replace("[@CompanyName]", OneClappContext.ProjectName).Replace("[@ToName]", receiverName).Replace("[@VerifyUrl]", OneClappContext.AppURL + "verification/false/" + guid).Replace("[@VerificationCode]", verificationCode);
            try
            {
                // var IsSuccess = await EmailManager.SendMailAsync (email, subject, body);
                var IsSuccess = false;
                if (tenantId != null)
                {
                    var emailConfig = GetEmailConfig(tenantId.Value);
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, emailConfig);
                }
                else
                {
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, null);
                }
                await AddUpdateEmailLog(body, subject, email, IsSuccess, "REPWD", tenantId);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task SendSetPasswordEmail(string email, string userid, string guid, string receiverName, long? tenantId, string emailSubject)
        {
            // Send Signup email
            var subject = OneClappContext.ProjectName + " - " + emailSubject;
            // var body = _emailTemplateService.GetEmailTemplateByCode ("PSWRT").TemplateHtml;
            var body = "<div style='border:1px solid #dedede;box-shadow: 0px 0px 5px #dddddd;padding:10px;'>  <p>Dear [@ToName],<br />  Set your password url : <a href='[@VerifyUrl]' target='_blank'>Click here.</a> </p>    <p><br />  Regards,</p>    <p>[@CompanyName]</p>    <p>THIS IS AN AUTOMATED EMAIL, PLEASE DO NOT REPLY</p>  </div>";
            // body = body.Replace ("[@CompanyName]", OneClappContext.ProjectName).Replace ("[@ToName]", receiverName).Replace ("[@VerifyUrl]", OneClappContext.AppURL + "pages/auth/reset-password/" + userid + "/" + guid).Replace ("[@VerificationCode]", verificationCode);
            body = body.Replace("[@CompanyName]", OneClappContext.ProjectName).Replace("[@ToName]", receiverName).Replace("[@VerifyUrl]", OneClappContext.AppURL + "set-password/" + userid + "/" + guid);
            try
            {
                // var IsSuccess = await EmailManager.SendMailAsync (email, subject, body);
                var IsSuccess = false;
                if (tenantId != null)
                {
                    var emailConfig = GetEmailConfig(tenantId.Value);
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, emailConfig);
                }
                else
                {
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, null);
                }
                await AddUpdateEmailLog(body, subject, email, IsSuccess, "SEPWD", tenantId);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task ChangePasswordEmail(string email, string userid, string guid, string receiverName, long? tenantId)
        {
            // Send Signup email
            var subject = OneClappContext.ProjectName + " - Account Password Changed";
            // var body = _emailTemplateService.GetEmailTemplateByCode ("PSWRT").TemplateHtml;
            var body = "<div style='border:1px solid #dedede;box-shadow: 0px 0px 5px #dddddd;padding:10px;'>  <p>Dear [@ToName],<br />  Your account password changed     <p><br />  Regards,</p>    <p>[@CompanyName]</p>    <p>THIS IS AN AUTOMATED EMAIL, PLEASE DO NOT REPLY</p>  </div>";
            // body = body.Replace ("[@CompanyName]", OneClappContext.ProjectName).Replace ("[@ToName]", receiverName).Replace ("[@VerifyUrl]", OneClappContext.AppURL + "pages/auth/reset-password/" + userid + "/" + guid).Replace ("[@VerificationCode]", verificationCode);
            body = body.Replace("[@CompanyName]", OneClappContext.ProjectName).Replace("[@ToName]", receiverName);
            try
            {
                // var IsSuccess = await EmailManager.SendMailAsync (email, subject, body);
                var IsSuccess = false;
                if (tenantId != null)
                {
                    var emailConfig = GetEmailConfig(tenantId.Value);
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, emailConfig);
                }
                else
                {
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, null);
                }
                await AddUpdateEmailLog(body, subject, email, IsSuccess, "PWD", tenantId);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task SendEmailVerifiedNotificationByAdmin(string email, string receiverName, long? tenantId)
        {
            var subject = OneClappContext.ProjectName + " - Email Verified For Your Account";
            // var body = _emailTemplateService.GetEmailTemplateByCode ("EVERN").TemplateHtml;
            var body = "Your email verified by Admin";
            body = body.Replace("[@CompanyName]", OneClappContext.ProjectName).Replace("[@ToName]", receiverName).Replace("[@Email]", email);

            try
            {
                // var IsSuccess = await EmailManager.SendMailAsync (email, subject, body);
                var IsSuccess = false;
                if (tenantId != null)
                {
                    var emailConfig = GetEmailConfig(tenantId.Value);
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, emailConfig);
                }
                else
                {
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, null);
                }

                await AddUpdateEmailLog(body, subject, email, IsSuccess, "VERIF", tenantId);

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task SendEmailVerifiedNotification(string email, string receiverName, long? tenantId)
        {
            var subject = OneClappContext.ProjectName + " - Email Verified For Your Account";
            // var body = _emailTemplateService.GetEmailTemplateByCode ("EVERN").TemplateHtml;
            var body = "Your email verified";
            body = body.Replace("[@CompanyName]", OneClappContext.ProjectName).Replace("[@ToName]", receiverName).Replace("[@Email]", email);

            try
            {
                // var IsSuccess = await EmailManager.SendMailAsync (email, subject, body);
                var IsSuccess = false;
                if (tenantId != null)
                {
                    var emailConfig = GetEmailConfig(tenantId.Value);
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, emailConfig);
                }
                else
                {
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, null);
                }

                await AddUpdateEmailLog(body, subject, email, IsSuccess, "VERIF", tenantId);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task SendEmailTaskUserAssignNotification(string email, string receiverName, string taskname, long? tenantId)
        {
            var subject = OneClappContext.ProjectName + " - Task Assigned";
            // var body = _emailTemplateService.GetEmailTemplateByCode ("EVERN").TemplateHtml;
            var body = "<div style='border:1px solid #dedede;box-shadow: 0px 0px 5px #dddddd;padding:10px;'> <p>Dear [@ToName],<br />  [@taskname]  assigned to you. </p> <p><br />  Regards,</p> <p>[@CompanyName]</p> <p>THIS IS AN AUTOMATED EMAIL, PLEASE DO NOT REPLY</p>  </div>";
            //var body = taskname + " Assigned to you.";
            body = body.Replace("[@CompanyName]", OneClappContext.ProjectName).Replace("[@taskname]", taskname).Replace("[@ToName]", receiverName).Replace("[@Email]", email);

            try
            {

                var IsSuccess = false;
                if (tenantId != null)
                {
                    var emailConfig = GetEmailConfig(tenantId.Value);
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, emailConfig);
                }
                else
                {
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, null);
                }

                await AddUpdateEmailLog(body, subject, email, IsSuccess, "ATASK", tenantId);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task SendEmailEmployeeTaskUserAssignNotification(string email, string receiverName, string taskname, long? tenantId, long? TaskId)
        {
            var subject = "Employee" + " - Task Assigned";
            // var body = _emailTemplateService.GetEmailTemplateByCode ("EVERN").TemplateHtml;
            string mailUrl = OneClappContext.TaskMailUrl + "" + TaskId;
            var body = "<div style='border:1px solid #dedede;box-shadow: 0px 0px 5px #dddddd;padding:10px;'> <p>Dear [@ToName],<br />  [@taskname]  assigned to you. <a href='[@mailUrl]'>Click Here</a></p> <p><br />  Regards,</p> <p>[@CompanyName]</p> <p>THIS IS AN AUTOMATED EMAIL, PLEASE DO NOT REPLY</p>  </div>";
            //var body = taskname + " Assigned to you.";
            body = body.Replace("[@CompanyName]", OneClappContext.ProjectName).Replace("[@taskname]", taskname).Replace("[@ToName]", receiverName).Replace("[@mailUrl]", mailUrl).Replace("[@Email]", email);

            try
            {

                var IsSuccess = false;
                if (tenantId != null)
                {
                    var emailConfig = GetEmailConfig(tenantId.Value);
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, emailConfig);
                }
                else
                {
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, null);
                }

                await AddUpdateEmailLog(body, subject, email, IsSuccess, "ATASK", tenantId);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task SendEmailMateTicketUserAssignNotification(string email, string receiverName, string ticketname, long? tenantId, long? TicketId)
        {
            var subject = "Employee" + " - Ticket Assigned";
            // var body = _emailTemplateService.GetEmailTemplateByCode ("EVERN").TemplateHtml;
            string mailUrl = OneClappContext.TicketMailUrl + "" + TicketId;
            var body = "<div style='border:1px solid #dedede;box-shadow: 0px 0px 5px #dddddd;padding:10px;'> <p>Dear [@ToName],<br />  [@ticketname]  assigned to you. <a href='[@mailUrl]'>Click Here</a></p> <p><br />  Regards,</p> <p>[@CompanyName]</p> <p>THIS IS AN AUTOMATED EMAIL, PLEASE DO NOT REPLY</p>  </div>";
            //var body = taskname + " Assigned to you.";
            body = body.Replace("[@CompanyName]", OneClappContext.ProjectName).Replace("[@ticketname]", ticketname).Replace("[@mailUrl]", mailUrl).Replace("[@ToName]", receiverName).Replace("[@Email]", email);

            try
            {

                var IsSuccess = false;
                if (tenantId != null)
                {
                    var emailConfig = GetEmailConfig(tenantId.Value);
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, emailConfig);
                }
                else
                {
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, null);
                }

                await AddUpdateEmailLog(body, subject, email, IsSuccess, "ATASK", tenantId);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task SendEmailRemoveMateTicketUserAssignNotification(string email, string receiverName, string ticketname, long? tenantId)
        {
            var subject = "Employee" + " - Ticket User Removed";

            var body = "<div style='border:1px solid #dedede;box-shadow: 0px 0px 5px #dddddd;padding:10px;'> <p>Dear [@ToName],<br />  Removed from [@ticketname]  </p> <p><br />  Regards,</p> <p>[@CompanyName]</p> <p>THIS IS AN AUTOMATED EMAIL, PLEASE DO NOT REPLY</p>  </div>";

            body = body.Replace("[@CompanyName]", OneClappContext.ProjectName).Replace("[@ticketname]", ticketname).Replace("[@ToName]", receiverName).Replace("[@Email]", email);

            try
            {
                var IsSuccess = false;
                if (tenantId != null)
                {
                    var emailConfig = GetEmailConfig(tenantId.Value);
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, emailConfig);
                }
                else
                {
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, null);
                }
                await AddUpdateEmailLog(body, subject, email, IsSuccess, "RPASK", tenantId);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task SendEmailRemoveTaskUserAssignNotification(string email, string receiverName, string taskname, long? tenantId)
        {
            var subject = OneClappContext.ProjectName + " - Task User Removed";
            // var body = _emailTemplateService.GetEmailTemplateByCode ("EVERN").TemplateHtml;
            var body = "<div style='border:1px solid #dedede;box-shadow: 0px 0px 5px #dddddd;padding:10px;'> <p>Dear [@ToName],<br />  Removed from [@taskname]  </p> <p><br />  Regards,</p> <p>[@CompanyName]</p> <p>THIS IS AN AUTOMATED EMAIL, PLEASE DO NOT REPLY</p>  </div>";
            //var body = taskname + " Assigned to you.";
            body = body.Replace("[@CompanyName]", OneClappContext.ProjectName).Replace("[@taskname]", taskname).Replace("[@ToName]", receiverName).Replace("[@Email]", email);

            try
            {
                var IsSuccess = false;
                if (tenantId != null)
                {
                    var emailConfig = GetEmailConfig(tenantId.Value);
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, emailConfig);
                }
                else
                {
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, null);
                }
                await AddUpdateEmailLog(body, subject, email, IsSuccess, "RTASK", tenantId);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task SendEmailRemoveEmployeeTaskUserAssignNotification(string email, string receiverName, string taskname, long? tenantId)
        {
            var subject = "Employee" + " - Task User Removed";
            // var body = _emailTemplateService.GetEmailTemplateByCode ("EVERN").TemplateHtml;
            var body = "<div style='border:1px solid #dedede;box-shadow: 0px 0px 5px #dddddd;padding:10px;'> <p>Dear [@ToName],<br />  Removed from [@taskname]  </p> <p><br />  Regards,</p> <p>[@CompanyName]</p> <p>THIS IS AN AUTOMATED EMAIL, PLEASE DO NOT REPLY</p>  </div>";
            //var body = taskname + " Assigned to you.";
            body = body.Replace("[@CompanyName]", OneClappContext.ProjectName).Replace("[@taskname]", taskname).Replace("[@ToName]", receiverName).Replace("[@Email]", email);

            try
            {
                var IsSuccess = false;
                if (tenantId != null)
                {
                    var emailConfig = GetEmailConfig(tenantId.Value);
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, emailConfig);
                }
                else
                {
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, null);
                }
                await AddUpdateEmailLog(body, subject, email, IsSuccess, "RTASK", tenantId);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task ResetPassWordForNewUser(string email, string userid, string guid, string receiverName, string verificationCode, long? tenantId)
        {
            // Send Signup email
            var subject = OneClappContext.ProjectName + " - Reset Account Password";
            // var body = _emailTemplateService.GetEmailTemplateByCode ("PSWRT").TemplateHtml;
            // var body = "<div style='border:1px solid #dedede;box-shadow: 0px 0px 5px #dddddd;padding:10px;'>  <p>Dear [@ToName],<br />  Your account password changed     <p><br />  Regards,</p>    <p>[@CompanyName]</p>    <p>THIS IS AN AUTOMATED EMAIL, PLEASE DO NOT REPLY</p>  </div>";
            var body = "<div style='border:1px solid #dedede;box-shadow: 0px 0px 5px #dddddd;padding:10px;'>  <p>Dear [@ToName],<br />  Verify your url: <a href='[@VerifyUrl]' target='_blank'>Click here.</a>   Verification Code: [@VerificationCode]</p>    <p><br />  Regards,</p>    <p>[@CompanyName]</p>    <p>THIS IS AN AUTOMATED EMAIL, PLEASE DO NOT REPLY</p>  </div>";
            body = body.Replace("[@CompanyName]", OneClappContext.ProjectName).Replace("[@ToName]", receiverName).Replace("[@VerifyUrl]", OneClappContext.AppURL + "pages/auth/reset-password/" + userid + "/" + guid).Replace("[@VerificationCode]", verificationCode);
            // body = body.Replace ("[@CompanyName]", OneClappContext.ProjectName).Replace ("[@ToName]", receiverName);
            try
            {
                // var IsSuccess = await EmailManager.SendMailAsync (email, subject, body);
                var IsSuccess = false;
                if (tenantId != null)
                {
                    var emailConfig = GetEmailConfig(tenantId.Value);
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, emailConfig);
                }
                else
                {
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, null);
                }
                await AddUpdateEmailLog(body, subject, email, IsSuccess, "PWD", tenantId.Value);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task SendEmailForUserSubscription(string email, string receiverName, long? tenantId)
        {
            var subject = OneClappContext.ProjectName + " - Thanks for subscribe with us!";
            // var body = _emailTemplateService.GetEmailTemplateByCode ("EVERN").TemplateHtml;
            var body = "<div style='border:1px solid #dedede;box-shadow: 0px 0px 5px #dddddd;padding:10px;'> <p>Dear [@ToName],<br /> Thanks for subscription plan.  </p> <p><br />  Regards,</p> <p>[@CompanyName]</p> <p>THIS IS AN AUTOMATED EMAIL, PLEASE DO NOT REPLY</p>  </div>";
            //var body = taskname + " Assigned to you.";
            body = body.Replace("[@CompanyName]", OneClappContext.ProjectName).Replace("[@ToName]", receiverName).Replace("[@Email]", email);

            try
            {
                var IsSuccess = false;
                if (tenantId != null)
                {
                    var emailConfig = GetEmailConfig(tenantId.Value);
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, emailConfig);
                }
                else
                {
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, null);
                }
                await AddUpdateEmailLog(body, subject, email, IsSuccess, "RTASK", tenantId);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task SendEmailForExpireNotification(string email, string receiverName, long? tenantId)
        {
            var subject = OneClappContext.ProjectName + " - Expire your subscription!";
            // var body = _emailTemplateService.GetEmailTemplateByCode ("EVERN").TemplateHtml;
            var body = "<div style='border:1px solid #dedede;box-shadow: 0px 0px 5px #dddddd;padding:10px;'> <p>Dear [@ToName],<br /> Expire your subscription plan soon. Please visit site and continue your subscription plan.  </p> <p><br />  Regards,</p> <p>[@CompanyName]</p> <p>THIS IS AN AUTOMATED EMAIL, PLEASE DO NOT REPLY</p>  </div>";
            //var body = taskname + " Assigned to you.";
            body = body.Replace("[@CompanyName]", OneClappContext.ProjectName).Replace("[@ToName]", receiverName).Replace("[@Email]", email);

            try
            {
                var IsSuccess = false;
                if (tenantId != null)
                {
                    var emailConfig = GetEmailConfig(tenantId.Value);
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, emailConfig);
                }
                else
                {
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, null);
                }
                await AddUpdateEmailLog(body, subject, email, IsSuccess, "RTASK", tenantId);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task SendEmailForRemoveUserSubscription(string email, string receiverName, long? tenantId)
        {
            var subject = OneClappContext.ProjectName + " - Remove subscription plan";
            // var body = _emailTemplateService.GetEmailTemplateByCode ("EVERN").TemplateHtml;
            var body = "<div style='border:1px solid #dedede;box-shadow: 0px 0px 5px #dddddd;padding:10px;'> <p>Dear [@ToName],<br /> Removed subscription plan.  </p> <p><br />  Regards,</p> <p>[@CompanyName]</p> <p>THIS IS AN AUTOMATED EMAIL, PLEASE DO NOT REPLY</p>  </div>";
            //var body = taskname + " Assigned to you.";
            body = body.Replace("[@CompanyName]", OneClappContext.ProjectName).Replace("[@ToName]", receiverName).Replace("[@Email]", email);

            try
            {
                var IsSuccess = false;
                if (tenantId != null)
                {
                    var emailConfig = GetEmailConfig(tenantId.Value);
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, emailConfig);
                }
                else
                {
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, null);
                }
                await AddUpdateEmailLog(body, subject, email, IsSuccess, "RTASK", tenantId);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        public async Task<EmailLog> AddUpdateEmailLog(string body, string subject, string toEmail, bool status, string templateCode, long? TenantId)
        {
            EmailLogDto emailLogDto = new EmailLogDto();
            emailLogDto.Body = body;
            emailLogDto.Subject = subject;
            emailLogDto.ToEmail = toEmail;
            emailLogDto.Status = status;
            emailLogDto.FromEmail = "shraddha.prof21@gmail.com";
            emailLogDto.FromLabel = "Email Verification";
            emailLogDto.TenantId = TenantId;
            // emailLogDto.TemplateCode = "Test";
            emailLogDto.TemplateCode = templateCode;
            emailLogDto.Tried = 1;
            var AddUpdateEmailLog = await _emailLogService.CheckInsertOrUpdate(emailLogDto);
            return AddUpdateEmailLog;
        }

        //Resend All Filed Email
        public async Task<List<EmailLog>> ResendFailedEmails()
        {

            var FailedEmailList = _emailLogService.GetAllFailEmail();
            foreach (var emailLog in FailedEmailList)
            {
                var IsSuccess = false;
                if (emailLog.TenantId != null)
                {
                    var emailConfig = GetEmailConfig(emailLog.TenantId.Value);
                    IsSuccess = await EmailManager.SendMailAsync(emailLog.ToEmail, emailLog.Subject, emailLog.Body, emailConfig);
                }
                else
                {
                    IsSuccess = await EmailManager.SendMailAsync(emailLog.ToEmail, emailLog.Subject, emailLog.Body, null);
                }

                emailLog.Status = IsSuccess;
                emailLog.Tried = emailLog.Tried + 1;
                var emailLogDto = _mapper.Map<EmailLogDto>(emailLog);
                await _emailLogService.CheckInsertOrUpdate(emailLogDto);
            }

            return FailedEmailList;
        }

        public async Task<EmailLogDto> ResendEmail(long Id)
        {
            EmailLogDto emailLogDto = new EmailLogDto();
            var emailLog = _emailLogService.GetEmailLogById(Id);
            if (emailLog != null)
            {

                var IsSuccess = false;
                if (emailLog.TenantId != null)
                {
                    var emailConfig = GetEmailConfig(emailLog.TenantId.Value);
                    IsSuccess = await EmailManager.SendMailAsync(emailLog.ToEmail, emailLog.Subject, emailLog.Body, emailConfig);
                }
                else
                {
                    IsSuccess = await EmailManager.SendMailAsync(emailLog.ToEmail, emailLog.Subject, emailLog.Body, null);
                }
                // var IsSuccess = await EmailManager.SendMailAsync (emailLog.ToEmail, emailLog.Subject, emailLog.Body);
                emailLog.Status = IsSuccess;
                emailLog.Tried = emailLog.Tried + 1;
                emailLogDto = _mapper.Map<EmailLogDto>(emailLog);
                emailLog = await _emailLogService.CheckInsertOrUpdate(emailLogDto);
            }

            return emailLogDto;
        }

        private EmailProviderConfigDto GetEmailConfig(long tenantId)
        {

            EmailProviderConfigDto emailProviderConfigDto = new EmailProviderConfigDto();
            var config = _emailConfigService.GetEmailConfigByTenant(tenantId);

            if (config != null)
            {
                emailProviderConfigDto = _mapper.Map<EmailProviderConfigDto>(config);
                var emailProviderObj = _emailProvider.GetEmailProviderById(emailProviderConfigDto.EmailProviderId.Value);
                if (emailProviderObj != null)
                {
                    emailProviderConfigDto.Port = emailProviderObj.Port;
                    emailProviderConfigDto.Host = emailProviderObj.Host;
                    emailProviderConfigDto.ProviderName = emailProviderObj.ProviderName;
                }

            }
            return emailProviderConfigDto;
        }

        public async Task CustomerActivityInviteMember(string email, string receiverName, CustomerActivityDto model)
        {
            // Send Signup email
            var subject = OneClappContext.ProjectName + " - Invite customer";
            var body = "<div style='border:1px solid #dedede;box-shadow: 0px 0px 5px #dddddd;padding:10px;'>  <p>Dear [@ToName],<br /> <p> Event name:" + model.Title + "</p> <br /> <p>Time: " + model.ScheduleStartDate + "," + model.StartTime + "->" + model.EndTime + "</p><br />  <p><br />  Regards,</p> <p>[@CompanyName]</p>    <p>THIS IS AN AUTOMATED EMAIL, PLEASE DO NOT REPLY</p>  </div>";
            body = body.Replace("[@CompanyName]", OneClappContext.ProjectName).Replace("[@ToName]", receiverName);
            // body = body.Replace ("[@CompanyName]", OneClappContext.ProjectName).Replace ("[@ToName]", receiverName);
            try
            {
                var IsSuccess = false;
                if (model.TenantId != null)
                {
                    if (model.StartTime != null || model.StartTime != "")
                    {
                        model.ScheduleStartDate = getStartEndTime(model.ScheduleStartDate.Value, model.StartTime);
                    }

                    if (model.EndTime != null || model.EndTime != "")
                    {
                        model.ScheduleEndDate = getStartEndTime(model.ScheduleEndDate.Value, model.EndTime);
                    }
                    var attach = GenerateICSFile(model.Title, model.ScheduleStartDate.Value, model.ScheduleEndDate.Value, model.Address, model.Description, model.Note);
                    var emailConfig = GetEmailConfig(model.TenantId.Value);
                    IsSuccess = await EmailManager.SendMailAsyncWithAttachment(email, subject, body, emailConfig, attach);
                }
                else
                {
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, null);
                }
                await AddUpdateEmailLog(body, subject, email, IsSuccess, "PWD", model.TenantId.Value);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task LeadActivityInviteMember(string email, string receiverName, LeadActivityDto model)
        {
            // Send Signup email
            var subject = OneClappContext.ProjectName + " - Invite lead";
            var body = "<div style='border:1px solid #dedede;box-shadow: 0px 0px 5px #dddddd;padding:10px;'>  <p>Dear [@ToName],<br /> <p> Event name:" + model.Title + "</p> <br /> <p>Time: " + model.ScheduleStartDate + "," + model.StartTime + "->" + model.EndTime + "</p><br />  <p><br />  Regards,</p> <p>[@CompanyName]</p>    <p>THIS IS AN AUTOMATED EMAIL, PLEASE DO NOT REPLY</p>  </div>";
            body = body.Replace("[@CompanyName]", OneClappContext.ProjectName).Replace("[@ToName]", receiverName);
            // body = body.Replace ("[@CompanyName]", OneClappContext.ProjectName).Replace ("[@ToName]", receiverName);
            try
            {
                // var IsSuccess = await EmailManager.SendMailAsync (email, subject, body);
                var IsSuccess = false;
                if (model.TenantId != null)
                {
                    if (model.StartTime != null && model.StartTime != "")
                        model.ScheduleStartDate = getStartEndTime(model.ScheduleStartDate.Value, model.StartTime);

                    if (model.EndTime != null && model.EndTime != "")
                        model.ScheduleEndDate = getStartEndTime(model.ScheduleEndDate.Value, model.EndTime);

                    var attach = GenerateICSFile(model.Title, model.ScheduleStartDate.Value, model.ScheduleEndDate.Value, model.Address, model.Description, model.Note);
                    var emailConfig = GetEmailConfig(model.TenantId.Value);
                    IsSuccess = await EmailManager.SendMailAsyncWithAttachment(email, subject, body, emailConfig, attach);
                }
                else
                {
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, null);
                }
                await AddUpdateEmailLog(body, subject, email, IsSuccess, "PWD", model.TenantId.Value);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task OrganizationActivityInviteMember(string email, string receiverName, OrganizationActivityDto model)
        {
            // Send Signup email
            var subject = OneClappContext.ProjectName + " - Invite organization";
            var body = "<div style='border:1px solid #dedede;box-shadow: 0px 0px 5px #dddddd;padding:10px;'>  <p>Dear [@ToName],<br /> <p> Event name:" + model.Title + "</p> <br /> <p>Time: " + model.ScheduleStartDate + "," + model.StartTime + "->" + model.EndTime + "</p><br />  <p><br />  Regards,</p> <p>[@CompanyName]</p>    <p>THIS IS AN AUTOMATED EMAIL, PLEASE DO NOT REPLY</p>  </div>";
            body = body.Replace("[@CompanyName]", OneClappContext.ProjectName).Replace("[@ToName]", receiverName);
            // body = body.Replace ("[@CompanyName]", OneClappContext.ProjectName).Replace ("[@ToName]", receiverName);
            try
            {
                // var IsSuccess = await EmailManager.SendMailAsync (email, subject, body);
                var IsSuccess = false;
                if (model.TenantId != null)
                {
                    if (model.StartTime != null || model.StartTime != "")
                        model.ScheduleStartDate = getStartEndTime(model.ScheduleStartDate.Value, model.StartTime);

                    if (model.EndTime != null || model.EndTime != "")
                        model.ScheduleEndDate = getStartEndTime(model.ScheduleEndDate.Value, model.EndTime);

                    var attach = GenerateICSFile(model.Title, model.ScheduleStartDate.Value, model.ScheduleEndDate.Value, model.Address, model.Description, model.Note);
                    var emailConfig = GetEmailConfig(model.TenantId.Value);
                    IsSuccess = await EmailManager.SendMailAsyncWithAttachment(email, subject, body, emailConfig, attach);
                }
                else
                {
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, null);
                }
                await AddUpdateEmailLog(body, subject, email, IsSuccess, "PWD", model.TenantId.Value);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DateTime getStartEndTime(DateTime Date, string Time)
        {

            if (Time.Contains("PM") || Time.Contains("pm"))
            {
                string[] timeData;
                if (Time.Contains("PM"))
                {
                    timeData = Time.Split("PM");
                }
                else
                {
                    timeData = Time.Split("pm");
                }
                if (timeData.Length > 0)
                {
                    var time = timeData[0].Trim();
                    var hourMinData = time.Split(":");
                    int hour1;
                    if (hourMinData[0] != "12")
                    {
                        hour1 = Convert.ToInt16(hourMinData[0]) + 12;
                    }
                    else
                    {
                        hour1 = Convert.ToInt16(hourMinData[0]);
                    }
                    // var hour1 = Convert.ToInt16 (hourMinData[0]) + 12;
                    var minute = Convert.ToInt16(hourMinData[1]);
                    var hour2 = hour1.ToString();
                    Date = Date.AddHours(hour1).AddMinutes(minute);
                }
            }
            if (Time.Contains("AM") || Time.Contains("am"))
            {
                string[] timeData;
                if (Time.Contains("AM"))
                {
                    timeData = Time.Split("AM");
                }
                else
                {
                    timeData = Time.Split("am");
                }
                if (timeData.Length > 0)
                {
                    var time = timeData[0].Trim();
                    var hourMinData = time.Split(":");
                    var hour1 = Convert.ToInt16(hourMinData[0]);
                    var minute = Convert.ToInt16(hourMinData[1]);
                    var hour2 = hour1.ToString();
                    Date = Date.AddHours(hour1).AddMinutes(minute);
                }
            }
            return Date;
        }

        public Attachment GenerateICSFile(string Title, DateTime ScheduleStartDate, DateTime ScheduleEndDate, string Address, string Description, string Note)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("BEGIN:VCALENDAR");
            str.AppendLine("CALSCALE:GREGORIAN");
            str.AppendLine("METHOD:PUBLISH");
            str.AppendLine("PRODID:-//" + Title);
            str.AppendLine("VERSION:2.0");
            str.AppendLine("BEGIN:VEVENT");
            // str.AppendLine (string.Format ("DTSTART:{0:yyyyMMddTHHmmssZ}", DateTime.Now.AddMinutes (+330)));
            str.AppendLine(string.Format("DTSTART:{0:yyyyMMddTHHmmss}", ScheduleStartDate));
            // str.AppendLine (string.Format ("DTSTAMP:{0:yyyyMMddTHHmm}", DateTime.UtcNow));
            // str.AppendLine (string.Format ("DTEND:{0:yyyyMMddTHHmmssZ}", DateTime.Now.AddMinutes (+660)));
            str.AppendLine(string.Format("DTEND:{0:yyyyMMddTHHmmss}", ScheduleEndDate));
            str.AppendLine("LOCATION: " + Address);
            str.AppendLine(string.Format("UID:{0}", Guid.NewGuid()));
            str.AppendLine(string.Format("DESCRIPTION:{0}", Description));
            str.AppendLine(string.Format("X-ALT-DESC;FMTTYPE=text/html:{0}", "Test"));
            // str.AppendLine (string.Format ("SUMMARY:{0}", Note));
            str.AppendLine(string.Format("SUMMARY:{0}", Title));
            // str.AppendLine (string.Format ("ORGANIZER:MAILTO:{0}", "shraddha.prof21@gmail.com"));

            str.AppendLine(string.Format("ATTENDEE;CN=\"{0}\";RSVP=TRUE:mailto:{1}", OneClappContext.ProjectName, OneClappContext.ProjectName));

            str.AppendLine("BEGIN:VALARM");
            str.AppendLine("TRIGGER:-PT15M");
            str.AppendLine("ACTION:DISPLAY");
            str.AppendLine("DESCRIPTION:Reminder");
            str.AppendLine("END:VALARM");
            str.AppendLine("END:VEVENT");
            str.AppendLine("END:VCALENDAR");

            byte[] byteArray = Encoding.ASCII.GetBytes(str.ToString());
            MemoryStream stream = new MemoryStream(byteArray);

            Attachment attach = new Attachment(stream, Title + ".ics");
            return attach;
        }

        public async Task SendEmailEmployeeProjectUserAssignNotification(string email, string receiverName, string projectname, long? tenantId, long? projectId)
        {
            var subject = "Employee" + " - Project Assigned";
            // var body = _emailTemplateService.GetEmailTemplateByCode ("EVERN").TemplateHtml;
            string mailUrl = OneClappContext.ProjectMailUrl + "" + projectId;
            var body = "<div style='border:1px solid #dedede;box-shadow: 0px 0px 5px #dddddd;padding:10px;'> <p>Dear [@ToName],<br />  [@projectname]  assigned to you.<a href='[@mailUrl]'>Click Here</a> </p> <p><br />  Regards,</p> <p>[@CompanyName]</p> <p>THIS IS AN AUTOMATED EMAIL, PLEASE DO NOT REPLY</p>  </div>";
            //var body = taskname + " Assigned to you.";
            body = body.Replace("[@CompanyName]", OneClappContext.ProjectName).Replace("[@projectname]", projectname).Replace("[@ToName]", receiverName).Replace("[@mailUrl]", mailUrl).Replace("[@Email]", email);

            try
            {

                var IsSuccess = false;
                if (tenantId != null)
                {
                    var emailConfig = GetEmailConfig(tenantId.Value);
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, emailConfig);
                }
                else
                {
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, null);
                }

                await AddUpdateEmailLog(body, subject, email, IsSuccess, "APASK", tenantId);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task SendEmailRemoveEmployeeProjectUserAssignNotification(string email, string receiverName, string projectname, long? tenantId)
        {
            var subject = "Employee" + " - Project User Removed";

            var body = "<div style='border:1px solid #dedede;box-shadow: 0px 0px 5px #dddddd;padding:10px;'> <p>Dear [@ToName],<br />  Removed from [@projectname]  </p> <p><br />  Regards,</p> <p>[@CompanyName]</p> <p>THIS IS AN AUTOMATED EMAIL, PLEASE DO NOT REPLY</p>  </div>";

            body = body.Replace("[@CompanyName]", OneClappContext.ProjectName).Replace("[@projectname]", projectname).Replace("[@ToName]", receiverName).Replace("[@Email]", email);

            try
            {
                var IsSuccess = false;
                if (tenantId != null)
                {
                    var emailConfig = GetEmailConfig(tenantId.Value);
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, emailConfig);
                }
                else
                {
                    IsSuccess = await EmailManager.SendMailAsync(email, subject, body, null);
                }
                await AddUpdateEmailLog(body, subject, email, IsSuccess, "RPASK", tenantId);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task SendEmailContractBasedInvoiceNotification(string email, string ClientName, string contractName, DateTime? StartDate, DateTime? EndDate, long? FinalInvoiceAmount, long? Discount)
        {
            // Send Signup email
            var subject = contractName + " - Invoice Notification";

            var body = "<div style='border:1px solid #dedede;box-shadow: 0px 0px 5px #dddddd;padding:10px;'> <p>Dear [@ClientName],<br /><p>Contract = [@contractName]<br />  StartDate = [@StartDate]<br />  EndDate = [@EndDate]<br />  Total price = [@FinalInvoiceAmount]<br />  Discount = [@Discount]</p>    <p><br />  Regards,</p>    <p>[@CompanyName]</p>    <p>THIS IS AN AUTOMATED EMAIL, PLEASE DO NOT REPLY</p>  </div>";
            body = body.Replace("[@CompanyName]", OneClappContext.ProjectName).Replace("[@ClientName]", ClientName).Replace("[@contractName]", contractName).Replace("[@StartDate]", StartDate.ToString()).Replace("[@EndDate]", EndDate.ToString()).Replace("[@FinalInvoiceAmount]", FinalInvoiceAmount.ToString()).Replace("[@Discount]", Discount.ToString());

            try
            {
                var IsSuccess = false;
                IsSuccess = await EmailManager.SendMailAsync(email, subject, body, null);
                await AddUpdateEmailLog(body, subject, email, IsSuccess, "CONI", null);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task SendEmailClientBasedInvoiceNotification(string email, string ClientName, DateTime? StartDate, DateTime? EndDate, long? FinalInvoiceAmount)
        {
            // Send Signup email
            var subject = ClientName + " - Invoice Notification";

            var body = "<div style='border:1px solid #dedede;box-shadow: 0px 0px 5px #dddddd;padding:10px;'> <p>Dear [@ClientName],<br />StartDate = [@StartDate]<br />  EndDate = [@EndDate]<br />  Total price = [@FinalInvoiceAmount]</p>    <p><br />  Regards,</p>    <p>[@CompanyName]</p>    <p>THIS IS AN AUTOMATED EMAIL, PLEASE DO NOT REPLY</p>  </div>";
            body = body.Replace("[@CompanyName]", OneClappContext.ProjectName).Replace("[@ClientName]", ClientName).Replace("[@StartDate]", StartDate.ToString()).Replace("[@EndDate]", EndDate.ToString()).Replace("[@FinalInvoiceAmount]", FinalInvoiceAmount.ToString());

            try
            {
                var IsSuccess = false;
                IsSuccess = await EmailManager.SendMailAsync(email, subject, body, null);
                await AddUpdateEmailLog(body, subject, email, IsSuccess, "CONI", null);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}