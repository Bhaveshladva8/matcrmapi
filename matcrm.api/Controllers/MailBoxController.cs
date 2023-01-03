using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using matcrm.api.SignalR;
using matcrm.data.Context;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Dto.CustomEmail;
using matcrm.data.Models.Tables;
using matcrm.data.Models.ViewModels;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.service.Services.ERP;
using matcrm.service.Utility;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class MailBoxController : Controller
    {
        private readonly IMailBoxService _mailBoxService;
        private readonly IUserService _userService;
        private readonly IIntProviderService _intProviderService;
        private readonly IIntProviderAppService _intProviderAppService;
        private readonly IIntProviderAppSecretService _intProviderAppSecretService;
        private readonly ICustomerAttachmentService _customerAttachmentService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IGoogleCalendarService _calendarService;
        private readonly IMailBoxTeamService _mailBoxTeamService;
        private readonly ITeamInboxService _teamInboxService;
        private readonly IDiscussionService _discussionService;
        private readonly IDiscussionParticipantService _discussionParticipantService;
        private readonly IMailAssignUserService _mailAssignUserService;
        private readonly IMailParticipantService _mailParticipantService;
        private readonly IMailCommentService _mailCommentService;
        private readonly IMailCommentAttachmentService _mailCommentAttachmentService;
        private readonly ITeamInboxAccessService _teamInboxAccessService;
        private readonly IMailAssignCustomerService _mailAssignCustomerService;
        private readonly ICalendarSyncActivityService _calendarSyncActivityService;
        private readonly IDiscussionReadService _discussionReadService;
        private readonly ICustomDomainEmailConfigService _customDomainEmailConfigService;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;

        private MailInbox mailInbox;
        private CustomMailbox customMailbox;
        private IMapper _mapper;
        private int UserId = 0;

        private string MicroSoftClientId, MicroSoftSecret, MicroSoftScope, MicroSoftApiKey, MicroSoftTenantId;
        private string GoogleCalendarClientId, GoogleCalendarSecret, GoogleCalendarScope, GoogleCalendarApiKey;
        public MailBoxController(IMailBoxService mailBoxService,
        IUserService userService,
        IIntProviderService intProviderService,
        IIntProviderAppService intProviderAppService,
        IIntProviderAppSecretService intProviderAppSecretService,
        ICustomerAttachmentService customerAttachmentService,
        IGoogleCalendarService calendarService,
        IMailBoxTeamService mailBoxTeamService,
        IMailAssignUserService mailAssignUserService,
        IMailAssignCustomerService mailAssignCustomerService,
        IMailParticipantService mailParticipantService,
        ICustomDomainEmailConfigService customDomainEmailConfigService,
        ITeamInboxService teamInboxService,
        IHostingEnvironment hostingEnvironment,
        IDiscussionService discussionService,
        IMailCommentService mailCommentService,
        IMailCommentAttachmentService mailCommentAttachmentService,
        IDiscussionParticipantService discussionParticipantService,
        ITeamInboxAccessService teamInboxAccessService,
        ICalendarSyncActivityService calendarSyncActivityService,
        IDiscussionReadService discussionReadService,
        IHubContext<BroadcastHub, IHubClient> hubContext,
        OneClappContext context,
        IMapper mapper)
        {
            _mailBoxService = mailBoxService;
            _intProviderService = intProviderService;
            _intProviderAppService = intProviderAppService;
            _intProviderAppSecretService = intProviderAppSecretService;
            _mapper = mapper;
            _userService = userService;
            _customerAttachmentService = customerAttachmentService;
            _hostingEnvironment = hostingEnvironment;
            _calendarService = calendarService;
            _mailBoxTeamService = mailBoxTeamService;
            _mailAssignUserService = mailAssignUserService;
            _mailParticipantService = mailParticipantService;
            _teamInboxService = teamInboxService;
            _discussionService = discussionService;
            _mailCommentService = mailCommentService;
            _mailCommentAttachmentService = mailCommentAttachmentService;
            _discussionParticipantService = discussionParticipantService;
            _mailAssignCustomerService = mailAssignCustomerService;
            _teamInboxAccessService = teamInboxAccessService;
            _calendarSyncActivityService = calendarSyncActivityService;
            _discussionReadService = discussionReadService;
            _customDomainEmailConfigService = customDomainEmailConfigService;
            _hubContext = hubContext;
            MicroSoftClientId = OneClappContext.MicroSoftClientId;
            MicroSoftSecret = OneClappContext.MicroSecretKey;
            MicroSoftTenantId = OneClappContext.MicroSoftTenantId;
            GoogleCalendarClientId = OneClappContext.GoogleCalendarClientId;
            GoogleCalendarSecret = OneClappContext.GoogleCalendarSecretKey;
            GoogleCalendarApiKey = OneClappContext.GoogleCalendarApiKey;
            GoogleCalendarScope = OneClappContext.GoogleScopes;
            mailInbox = new MailInbox(context, userService, hostingEnvironment, intProviderAppService, intProviderAppSecretService, customerAttachmentService, calendarService, teamInboxService, customDomainEmailConfigService, mapper);
            customMailbox = new CustomMailbox(context, customDomainEmailConfigService);
        }

        [Authorize]
        [HttpPost]
        public async Task<OperationResult<AuthenticationCompleteResponse>> GoogleToken([FromBody] AuthenticationCompleteRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            // userId = 6;
            model.UserId = UserId;
            if (UserId > 0)
            {
                var requestmailToken = _mapper.Map<MailTokenDto>(model);
                var result = await mailInbox.AuthenticationComplete(UserId, requestmailToken);
                var responseMailToken = _mapper.Map<AuthenticationCompleteResponse>(result);
                return new OperationResult<AuthenticationCompleteResponse>(responseMailToken.IsValid, System.Net.HttpStatusCode.OK, responseMailToken.ErrorMessage, responseMailToken);
            }
            else
            {
                return new OperationResult<AuthenticationCompleteResponse>(false, System.Net.HttpStatusCode.OK, CommonMessage.UnAuthorizedUser);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<OperationResult<CustomDomainAuthenticationResponse>> CustomDomainAuthentication([FromBody] CustomDomainAuthenticationRequest model)
        {
            IntProviderAppSecretDto intProviderAppSecretDto = new IntProviderAppSecretDto();
            CustomDomainAuthenticationResponse intProviderAppSecretresponse = new CustomDomainAuthenticationResponse();

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            // userId = 6;
            var custommodel = _mapper.Map<CustomDomainEmailConfigDto>(model);
            custommodel.UserId = UserId;
            if (UserId > 0)
            {
                var result = await mailInbox.CustomDomainAuthentication(UserId, custommodel);
                if (result.IntProviderAppSecretId != null)
                {
                    var intProviderAppSecretObj = _intProviderAppSecretService.GetIntProviderAppSecretById(result.IntProviderAppSecretId.Value);
                    intProviderAppSecretDto = _mapper.Map<IntProviderAppSecretDto>(intProviderAppSecretObj);
                    intProviderAppSecretresponse = _mapper.Map<CustomDomainAuthenticationResponse>(intProviderAppSecretDto);
                    intProviderAppSecretresponse.CustomDomainEmailConfigDto = result;
                    intProviderAppSecretresponse.CustomDomainEmailConfigId = result.Id;
                }
                return new OperationResult<CustomDomainAuthenticationResponse>(result.IsValid, System.Net.HttpStatusCode.OK, result.ErrorMessage, intProviderAppSecretresponse);
            }
            else
                return new OperationResult<CustomDomainAuthenticationResponse>(false, System.Net.HttpStatusCode.OK, CommonMessage.UnAuthorizedUser);
        }

        [Authorize]
        [HttpPost]
        public async Task<OperationResult<CustomDomainEmailConnectionResponse>> TestConnection([FromBody] CustomDomainEmailConnectionRequest model)
        {

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var CustomDomainEmailmodel = _mapper.Map<CustomDomainEmailConfigDto>(model);
            CustomDomainEmailmodel.UserId = UserId;
            if (UserId > 0)
            {
                var result = await mailInbox.TestConnection(CustomDomainEmailmodel);
                var responseresult = _mapper.Map<CustomDomainEmailConnectionResponse>(result);
                return new OperationResult<CustomDomainEmailConnectionResponse>(result.IsValid, System.Net.HttpStatusCode.OK, result.ErrorMessage, responseresult);
            }
            else
            {
                return new OperationResult<CustomDomainEmailConnectionResponse>(false, System.Net.HttpStatusCode.OK, CommonMessage.UnAuthorizedUser);
            }
        }

        [HttpPost]
        public async Task<OperationResult<MailTokenResponse>> MicrosoftToken([FromBody] MailTokenRequest model)
        {
            MailTokenResponse calendarTokenObjresponse = new MailTokenResponse();
            MailTokenDto mailTokenDto = new MailTokenDto();
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            // var googleCalendarKey = micro;

            if (!string.IsNullOrEmpty(MicroSoftClientId))
            {
                model.grant_type = "authorization_code";
                model.type = "Access_Token";
                var tokenmodel = _mapper.Map<MailTokenDto>(model);
                var customers = await _mailBoxService.GetAccessToken(tokenmodel);
                if (customers != null)
                {
                    mailTokenDto = customers;
                }

                if (mailTokenDto.error == null || mailTokenDto.error == "")
                {
                    if (UserId != null)
                    {
                        var IntProviderObj = _intProviderService.GetIntProvider(tokenmodel.Provider);
                        var IntProviderAppObj = _intProviderAppService.GetIntProviderApp(tokenmodel.ProviderApp);
                        if (IntProviderObj != null)
                        {
                            // var IntProviderAppObj = _intProviderAppService.GetIntProviderAppByProviderId(IntProviderObj.Id, "Outlook");
                            IntProviderAppSecretDto intProviderAppSecretDto = new IntProviderAppSecretDto();
                            intProviderAppSecretDto.Access_Token = mailTokenDto.access_token;
                            intProviderAppSecretDto.Expires_In = mailTokenDto.expires_in;
                            intProviderAppSecretDto.Refresh_Token = mailTokenDto.refresh_token;
                            intProviderAppSecretDto.Scope = mailTokenDto.scope;
                            intProviderAppSecretDto.Token_Type = mailTokenDto.token_type;
                            intProviderAppSecretDto.Id_Token = mailTokenDto.id_token;
                            //intProviderAppSecretDto.CreatedBy = mailTokenDto.UserId;
                            intProviderAppSecretDto.IntProviderAppId = IntProviderAppObj.Id;
                            intProviderAppSecretDto.Color = tokenmodel.Color;
                            intProviderAppSecretDto.CreatedBy = UserId;

                            MailBoxTokenVM mailBoxTokenVMObj = await _mailBoxService.GetTokenInfo("Bearer " + mailTokenDto.access_token.ToString(), OneClappContext.MicroSoftClientId);
                            if (mailBoxTokenVMObj != null)
                            {
                                intProviderAppSecretDto.Email = mailBoxTokenVMObj.userPrincipalName;
                            }

                            var isExist = _intProviderAppSecretService.GetActiveSecretByUserAndEmail(UserId, intProviderAppSecretDto.Email, IntProviderAppObj.Id);
                            if (isExist != null)
                            {
                                var responsemodel = _mapper.Map<MailTokenResponse>(model);
                                return new OperationResult<MailTokenResponse>(false, System.Net.HttpStatusCode.OK, "Email account already synced", responsemodel);
                            }
                            // var isExpire = Common.IsTokenExpired(calendarTokenObj.access_token.ToString());
                            // if (isExpire == false)
                            // {
                            // var jwt = calendarTokenObj.access_token;
                            // var handler = new JwtSecurityTokenHandler();
                            // var token = handler.ReadJwtToken(jwt) as JwtSecurityToken;
                            // var jti = token.Claims.First(claim => claim.Type == "exp").Value;
                            // secretDto.Email = token.Claims.First(claim => claim.Type == "email").Value;

                            // }
                            // if (googleCalendarUserInfo != null)
                            // {
                            //     secretDto.Email = googleCalendarUserInfo.email;
                            // }
                            var intProviderAppSecretObj = await _intProviderAppSecretService.CheckInsertOrUpdate(intProviderAppSecretDto);

                            if (intProviderAppSecretObj != null && tokenmodel.teamInboxId != null)
                            {
                                if (tokenmodel.teamInboxId != null)
                                {
                                    TeamInbox teamInboxObj = _teamInboxService.GetById(tokenmodel.teamInboxId.Value);
                                    if (teamInboxObj != null)
                                    {
                                        teamInboxObj.IntProviderAppSecretId = intProviderAppSecretObj.Id;
                                        var teamInboxDto = _mapper.Map<TeamInboxDto>(teamInboxObj);
                                        var teamInboxObj1 = await _teamInboxService.CheckInsertOrUpdate(teamInboxDto);
                                    }
                                }
                                else
                                {
                                    TeamInboxDto teamInboxDto = new TeamInboxDto();
                                    teamInboxDto.IntProviderAppSecretId = intProviderAppSecretObj.Id;
                                    teamInboxDto.MailBoxTeamId = tokenmodel.MailBoxTeamId;
                                    teamInboxDto.CreatedBy = UserId;
                                    var AddUpdateTeamInboxObj = _teamInboxService.CheckInsertOrUpdate(teamInboxDto);
                                }
                                mailTokenDto.SelectedEmail = intProviderAppSecretObj.Email;
                                mailTokenDto.email = intProviderAppSecretObj.Email;
                            }
                        }
                    }
                    calendarTokenObjresponse = _mapper.Map<MailTokenResponse>(mailTokenDto);
                    return new OperationResult<MailTokenResponse>(true, System.Net.HttpStatusCode.OK, "", calendarTokenObjresponse);

                }
                else
                {
                    calendarTokenObjresponse = _mapper.Map<MailTokenResponse>(mailTokenDto);
                    return new OperationResult<MailTokenResponse>(false, System.Net.HttpStatusCode.OK, calendarTokenObjresponse.error, calendarTokenObjresponse);
                }

            }
            else
            {
                calendarTokenObjresponse = _mapper.Map<MailTokenResponse>(mailTokenDto);
                return new OperationResult<MailTokenResponse>(false, System.Net.HttpStatusCode.OK, "", calendarTokenObjresponse);
            }
        }

        [HttpPost("{ThreadId}")]
        public async Task<OperationResult<List<ThreadByThreadIdResponse>>> ThreadDetail(string ThreadId, [FromBody] ThreadByThreadIdRequest model)
        {

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var CreatedBy = UserId;
            //InboxVM inboxvmmodel = new InboxVM();
            var inboxvmmodel = _mapper.Map<InboxVM>(model);
            if (!string.IsNullOrEmpty(inboxvmmodel.EventType))
            {
                if (inboxvmmodel.EventType == "assignuser")
                {
                    var data = _mailAssignUserService.GetByUserThread(UserId, ThreadId);
                    if (data != null && data.IntProviderAppSecret != null)
                    {
                        if (data.IntProviderAppSecret.CreatedBy != null)
                        {
                            CreatedBy = data.IntProviderAppSecret.CreatedBy.Value;
                        }
                        inboxvmmodel.SelectedEmail = data.IntProviderAppSecret.Email;
                        if (data.IntProviderAppSecret.IntProviderApp != null)
                        {
                            inboxvmmodel.ProviderApp = data.IntProviderAppSecret.IntProviderApp.Name;
                            if (data.IntProviderAppSecret.IntProviderApp.IntProvider != null)
                            {
                                inboxvmmodel.Provider = data.IntProviderAppSecret.IntProviderApp.IntProvider.Name;
                            }
                        }
                        else
                        {
                            inboxvmmodel.Provider = null;
                            inboxvmmodel.ProviderApp = null;
                        }

                    }
                }
                else if (inboxvmmodel.EventType == "shareduser")
                {
                    var data = _mailParticipantService.GetByUserThread(UserId, ThreadId);
                    if (data != null && data.IntProviderAppSecret != null)
                    {
                        if (data.IntProviderAppSecret.CreatedBy != null)
                        {
                            CreatedBy = data.IntProviderAppSecret.CreatedBy.Value;
                        }
                        inboxvmmodel.SelectedEmail = data.IntProviderAppSecret.Email;
                        if (data.IntProviderAppSecret.IntProviderApp != null)
                        {
                            inboxvmmodel.ProviderApp = data.IntProviderAppSecret.IntProviderApp.Name;
                            if (data.IntProviderAppSecret.IntProviderApp.IntProvider != null)
                            {
                                inboxvmmodel.Provider = data.IntProviderAppSecret.IntProviderApp.IntProvider.Name;
                            }
                        }
                        else
                        {
                            inboxvmmodel.Provider = null;
                            inboxvmmodel.ProviderApp = null;
                        }
                    }
                }
            }
            if (inboxvmmodel.TeamInboxId != null)
            {
                var teamInboxObj = _teamInboxService.GetById(inboxvmmodel.TeamInboxId.Value);
                if (teamInboxObj != null)
                {
                    if (teamInboxObj.CreatedBy != null)
                    {
                        CreatedBy = teamInboxObj.CreatedBy.Value;
                    }
                    if (teamInboxObj.IntProviderAppSecret != null)
                    {
                        inboxvmmodel.SelectedEmail = teamInboxObj.IntProviderAppSecret.Email;
                        if (teamInboxObj.IntProviderAppSecret.IntProviderApp != null)
                        {
                            inboxvmmodel.ProviderApp = teamInboxObj.IntProviderAppSecret.IntProviderApp.Name;
                            if (teamInboxObj.IntProviderAppSecret.IntProviderApp.IntProvider != null)
                            {
                                inboxvmmodel.Provider = teamInboxObj.IntProviderAppSecret.IntProviderApp.IntProvider.Name;
                            }
                        }
                        else
                        {
                            inboxvmmodel.Provider = null;
                            inboxvmmodel.ProviderApp = null;
                        }
                    }
                }
            }
            // userId = 6;
            if (UserId > 0)
            {
                InboxVM inboxVMObj = new InboxVM();
                inboxVMObj.UserId = CreatedBy;
                inboxVMObj.Provider = inboxvmmodel.Provider;
                inboxVMObj.Label = inboxvmmodel.Label;
                if (string.IsNullOrEmpty(inboxvmmodel.SelectedEmail))
                {
                    if (inboxvmmodel.MailAssignUserId != null)
                    {
                        var mailAssignUserObj = _mailAssignUserService.GetById(inboxvmmodel.MailAssignUserId.Value);
                        if (mailAssignUserObj.IntProviderAppSecret != null)
                        {
                            inboxVMObj.SelectedEmail = mailAssignUserObj.IntProviderAppSecret.Email;
                            if (mailAssignUserObj.IntProviderAppSecret.IntProviderApp != null)
                            {
                                inboxVMObj.ProviderApp = mailAssignUserObj.IntProviderAppSecret.IntProviderApp.Name;
                            }
                        }
                    }
                }
                else
                {
                    inboxVMObj.ProviderApp = inboxvmmodel.ProviderApp;
                    inboxVMObj.SelectedEmail = inboxvmmodel.SelectedEmail;
                }

                // inboxVM.Skip = model.Skip;
                if (inboxvmmodel.Skip != null)
                {
                    inboxVMObj.Skip = inboxvmmodel.Skip;
                }
                else
                {
                    inboxVMObj.Skip = 0;
                }
                if (inboxvmmodel.Top != null)
                {
                    inboxVMObj.Top = inboxvmmodel.Top;
                }
                else
                {
                    inboxVMObj.Top = 20;
                }

                var result = await mailInbox.GetThreadByThreadId(CreatedBy, ThreadId, inboxVMObj);
                // foreach (var threadItem in result)
                // {
                //     threadItem.CreatedOn = Common.UnixTimeStampToDateTimeMilliSec(threadItem.InternalDate);
                //     if (threadItem.Attachments != null && threadItem.Attachments.Count() > 0)
                //     {
                //         foreach (var attachmentItem in threadItem.Attachments)
                //         {
                //             if (!string.IsNullOrEmpty(attachmentItem.contentBytes))
                //             {
                //                 var dirPath = _hostingEnvironment.WebRootPath + "\\Mail\\";

                //                 if (!Directory.Exists(dirPath))
                //                 {
                //                     Directory.CreateDirectory(dirPath);
                //                 }
                //                 // _hostingEnvironment.WebRootPath + "\\ProfileImageUpload\\Layout620220128_151730.jpg"
                //                 System.IO.File.WriteAllBytes(dirPath + attachmentItem.fileName, attachmentItem.bytes);
                //                 // System.IO.File.WriteAllBytes(dirPath + attachmentItem.fileName, Convert.FromBase64String(attachmentItem.contentBytes));
                //             }

                //         }
                //     }
                // }
                var participants = _mailParticipantService.GetAllByThread(ThreadId);
                var participantDtoList = _mapper.Map<List<MailParticipantDto>>(participants);
                var customerAssignObj = _mailAssignCustomerService.GetMailAssignCustomerByThread(ThreadId);
                result = result.OrderByDescending(t => t.CreatedOn).ToList();
                var mailAssignUserObj1 = _mailAssignUserService.GetAllMailAssignUserByThread(ThreadId);
                if (result != null && result.Count() > 0)
                {
                    foreach (var threadItem in result)
                    {
                        if (mailAssignUserObj1 != null)
                        {
                            threadItem.AssignUserId = mailAssignUserObj1.TeamMemberId;
                        }
                        threadItem.Participants = participantDtoList;
                        if (customerAssignObj != null)
                        {
                            threadItem.CustomerId = customerAssignObj.CustomerId;
                            if (customerAssignObj.Customer != null)
                            {
                                threadItem.customerDto = _mapper.Map<CustomerDto>(customerAssignObj.Customer);
                            }
                        }
                    }
                }

                var commentList = _mailCommentService.GetAllByThread(ThreadId);
                var commentDtoList = _mapper.Map<List<MailCommentDto>>(commentList);
                if (commentDtoList != null && commentDtoList.Count() > 0)
                {
                    foreach (var mailCommentItem in commentDtoList)
                    {
                        if (mailCommentItem.Id != null)
                        {
                            var mailAttachments = _mailCommentAttachmentService.GetAllByMailCommentId(mailCommentItem.Id.Value);
                            mailCommentItem.Attachments = _mapper.Map<List<MailCommentAttachmentDto>>(mailAttachments);
                        }
                    }
                }
                var threadresponse = _mapper.Map<List<ThreadByThreadIdResponse>>(result);
                return new OperationResult<List<ThreadByThreadIdResponse>>(true, System.Net.HttpStatusCode.OK, "", threadresponse);

            }
            else
                return new OperationResult<List<ThreadByThreadIdResponse>>(false, System.Net.HttpStatusCode.OK, CommonMessage.UnAuthorizedUser);
        }

        [HttpPost]
        public async Task<OperationResult<BodyVM>> Attachment(string label, [FromBody] BodyVM model)
        {

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            if (UserId > 0)
            {
                var result = await mailInbox.GetAttachment(UserId, model);
                return new OperationResult<BodyVM>(result.IsValid, System.Net.HttpStatusCode.OK, result.ErrorMessage, result);
            }
            else
                return new OperationResult<BodyVM>(false, System.Net.HttpStatusCode.OK, CommonMessage.UnAuthorizedUser);

        }

        /// <summary>
        /// This API used to get Gmail thread of User
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<OperationResult<InboxThreads>> LabelWithUnReadCount([FromBody] InboxVM model)
        {

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            if (UserId > 0)
            {
                var result = await mailInbox.GetLabelWithUnReadCount(UserId, model);
                return new OperationResult<InboxThreads>(result.IsValid, System.Net.HttpStatusCode.OK, result.ErrorMessage, result);
            }
            else
                return new OperationResult<InboxThreads>(false, System.Net.HttpStatusCode.OK, CommonMessage.UnAuthorizedUser);

        }

        /// <summary>
        /// This API used to delete email thread
        /// </summary>
        /// <param name="threadId"></param>
        /// <param name="model"></param>
        /// <returns></returns>

        [HttpDelete("{threadId}/{threadType}")]
        public async Task<OperationResult<InboxThreads>> RemoveEmail(string threadId, string threadType, [FromBody] UserEmail model)
        {

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            if (model.UserId > 0)
            {
                var result = await mailInbox.DeleteEmail(UserId, threadId, threadType, model);
                await _hubContext.Clients.All.OnMailModuleEvent(null, "mail", "delete", threadId);
                return new OperationResult<InboxThreads>(result.IsValid, System.Net.HttpStatusCode.OK, result.ErrorMessage, result);
            }
            else
                return new OperationResult<InboxThreads>(false, System.Net.HttpStatusCode.OK, CommonMessage.UnAuthorizedUser);

        }


        [HttpDelete("{MessageId}")]
        public async Task<OperationResult<DeleteCustomEmailResponse>> RemoveCustomEmail(string MessageId, [FromBody] DeleteCustomEmailRequest model)
        {

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var requestmodel = _mapper.Map<MailTokenDto>(model);
            if (requestmodel.UserId > 0)
            {
                var result = await customMailbox.DeleteMessage(requestmodel, UserId, MessageId);

                var responsemodel = _mapper.Map<DeleteCustomEmailResponse>(model);
                return new OperationResult<DeleteCustomEmailResponse>(result, System.Net.HttpStatusCode.OK, "", responsemodel);
            }
            else
                return new OperationResult<DeleteCustomEmailResponse>(false, System.Net.HttpStatusCode.OK, CommonMessage.UnAuthorizedUser);

        }

        /// <summary>
        /// This API used to delete email thread
        /// </summary>
        /// <param name="threadId"></param>
        /// <param name="model"></param>
        /// <returns></returns>

        [HttpDelete]
        public async Task<OperationResult<InboxThreads>> RemoveEmails([FromBody] ThreadOperationVM model)
        {

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            if (model.UserId > 0)
            {
                var result = await mailInbox.DeleteMultipleEmail(UserId, model);
                return new OperationResult<InboxThreads>(result.IsValid, System.Net.HttpStatusCode.OK, result.ErrorMessage, result);
            }
            else
                return new OperationResult<InboxThreads>(false, System.Net.HttpStatusCode.OK, CommonMessage.UnAuthorizedUser);

        }

        /// <summary>
        /// This API used to read/unread email thread
        /// </summary>
        /// <param name="threadId"></param>
        /// <param name="model"></param>
        /// <returns></returns>

        [HttpPost("{MessageId}")]
        public async Task<OperationResult<MarkAsReadUnReadResponse>> MarkAsReadUnRead(string MessageId, [FromBody] MarkAsReadUnReadRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var requestmodel = _mapper.Map<UserEmail>(model);
            if (UserId > 0)
            {
                var result = await mailInbox.MarkAsReadUnRead(UserId, MessageId, requestmodel);
                var responseResult = _mapper.Map<MarkAsReadUnReadResponse>(result);
                return new OperationResult<MarkAsReadUnReadResponse>(result.IsValid, System.Net.HttpStatusCode.OK, result.ErrorMessage, responseResult);
            }
            else
                return new OperationResult<MarkAsReadUnReadResponse>(false, System.Net.HttpStatusCode.OK, CommonMessage.UnAuthorizedUser);
        }

        /// <summary>
        /// This API used to read/unread multiple email thread
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [HttpPost]
        public async Task<OperationResult<InboxThreads>> MultipleThreadMarkAsReadUnRead([FromBody] ThreadOperationVM model)
        {

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            if (UserId > 0)
            {
                var result = await mailInbox.MultipleThreadMarkAsReadUnRead(UserId, model);
                return new OperationResult<InboxThreads>(result.IsValid, System.Net.HttpStatusCode.OK, result.ErrorMessage, result);
            }
            else
                return new OperationResult<InboxThreads>(false, System.Net.HttpStatusCode.OK, CommonMessage.UnAuthorizedUser);

        }

        [HttpPost("{ThreadId}")]
        public async Task<OperationResult<ReadUnReadByThreadResponse>> ReadUnReadByThread(string ThreadId, [FromBody] ReadUnReadByThreadRequest model)
        {

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var requestmodel = _mapper.Map<UserEmail>(model);
            if (UserId > 0)
            {
                InboxVM inboxVMObj = new InboxVM();
                inboxVMObj.Provider = requestmodel.Provider;
                inboxVMObj.ProviderApp = requestmodel.ProviderApp;
                inboxVMObj.SelectedEmail = requestmodel.SelectedEmail;
                inboxVMObj.UserId = UserId;
                var result = await mailInbox.ReadUnReadByThread(ThreadId, UserId, requestmodel);
                if (requestmodel.IsRead)
                {
                    await _hubContext.Clients.All.OnMailModuleEvent(null, "mail", "Read", ThreadId);
                }
                else
                {
                    await _hubContext.Clients.All.OnMailModuleEvent(null, "mail", "Unread", ThreadId);
                }
                var threadresult = _mapper.Map<ReadUnReadByThreadResponse>(result);
                return new OperationResult<ReadUnReadByThreadResponse>(threadresult.IsValid, System.Net.HttpStatusCode.OK, threadresult.ErrorMessage, threadresult);

            }
            else
                return new OperationResult<ReadUnReadByThreadResponse>(false, System.Net.HttpStatusCode.OK, CommonMessage.UnAuthorizedUser);
        }


        /// <summary>
        /// This API used to forward email thread
        /// </summary>
        /// <param name="model"></param>
        /// <param name="threadId"></param>
        /// <returns></returns>

        [HttpPost("{MessageId}")]
        public async Task<OperationResult<Office365ForwardEmailResponse>> Office365ForwardEmail(string MessageId, [FromForm] Office365ForwardEmailRequest composeEmail1)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var requestmodel = _mapper.Map<ComposeEmail1>(composeEmail1);
            if (UserId > 0)
            {
                if (requestmodel.FileList != null && requestmodel.FileList.Count() > 0)
                {
                    foreach (IFormFile file in requestmodel.FileList)
                    {
                        if (OneClappContext.ClamAVServerIsLive)
                        {
                            ScanDocument scanDocumentObj = new ScanDocument();
                            bool fileStatus = await scanDocumentObj.ScanDocumentWithClam(file);
                            if (fileStatus)
                            {
                                 return new OperationResult<Office365ForwardEmailResponse>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                            }
                        }
                    }
                }
                var result = await mailInbox.Office365ForwardEmail(UserId, MessageId, requestmodel);
                var reponseresult = _mapper.Map<Office365ForwardEmailResponse>(result);
                await _hubContext.Clients.All.OnMailModuleEvent(null, "mail", "reply", requestmodel.threadId);
                return new OperationResult<Office365ForwardEmailResponse>(result.IsValid, System.Net.HttpStatusCode.OK, result.ErrorMessage, reponseresult);
            }
            else
                return new OperationResult<Office365ForwardEmailResponse>(false, System.Net.HttpStatusCode.OK, CommonMessage.UnAuthorizedUser);

        }

        /// <summary>
        /// This API used to reply email thread
        /// </summary>
        /// <param name="model"></param>
        /// <param name="threadId"></param>
        /// <returns></returns>

        [HttpPost("{MessageId}")]
        public async Task<OperationResult<Office365ReplayEmailResponse>> Office365ReplayEmail(string MessageId, [FromForm] Office365ReplayEmailRequest composeEmail1)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var requestmodel = _mapper.Map<ComposeEmail1>(composeEmail1);
            if (UserId > 0)
            {
                if (requestmodel.FileList != null && requestmodel.FileList.Count() > 0)
                {
                    foreach (IFormFile file in requestmodel.FileList)
                    {
                        if (OneClappContext.ClamAVServerIsLive)
                        {
                            ScanDocument scanDocumentObj = new ScanDocument();
                            bool fileStatus = await scanDocumentObj.ScanDocumentWithClam(file);
                            if (fileStatus)
                            {
                                return new OperationResult<Office365ReplayEmailResponse>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                            }
                        }
                    }
                }
                var result = await mailInbox.Office365ReplayEmail(UserId, MessageId, requestmodel);
                var responseresult = _mapper.Map<Office365ReplayEmailResponse>(result);
                await _hubContext.Clients.All.OnMailModuleEvent(null, "mail", "reply", requestmodel.threadId);
                return new OperationResult<Office365ReplayEmailResponse>(result.IsValid, System.Net.HttpStatusCode.OK, result.ErrorMessage, responseresult);
            }
            else
                return new OperationResult<Office365ReplayEmailResponse>(false, System.Net.HttpStatusCode.OK, CommonMessage.UnAuthorizedUser);
        }

        [HttpPost("{MessageId}")]
        public async Task<OperationResult<Value>> ReplayEmail(string MessageId, [FromForm] ComposeEmail1 composeEmail1)
        {

            Value valueObj = new Value();
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            if (UserId > 0)
            {
                var result = await customMailbox.SendReply(composeEmail1, UserId, MessageId);
                await _hubContext.Clients.All.OnMailModuleEvent(null, "mail", "reply", composeEmail1.threadId);
                return new OperationResult<Value>(result, System.Net.HttpStatusCode.OK, "", valueObj);
            }
            else
                return new OperationResult<Value>(false, System.Net.HttpStatusCode.OK, CommonMessage.UnAuthorizedUser);

        }

        [HttpPost("{MessageId}")]
        public async Task<OperationResult<GmailReplyEmailResponse>> GmailReplyEmail(string MessageId, [FromForm] GmailReplyEmailRequest composeEmail1)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var requestmodel = _mapper.Map<ComposeEmail1>(composeEmail1);
            requestmodel.messageId = MessageId;
            if (UserId > 0)
            {
                if (requestmodel.FileList != null && requestmodel.FileList.Count() > 0)
                {
                    foreach (IFormFile file in requestmodel.FileList)
                    {
                        if (OneClappContext.ClamAVServerIsLive)
                        {
                            ScanDocument scanDocumentObj = new ScanDocument();
                            bool fileStatus = await scanDocumentObj.ScanDocumentWithClam(file);
                            if (fileStatus)
                            {
                                return new OperationResult<GmailReplyEmailResponse>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                            }
                        }
                    }
                }
                var result = await mailInbox.SendReply(UserId, requestmodel, requestmodel.FileList);
                var responseresult = _mapper.Map<GmailReplyEmailResponse>(result);
                await _hubContext.Clients.All.OnMailModuleEvent(null, "mail", "reply", requestmodel.threadId);
                return new OperationResult<GmailReplyEmailResponse>(result.IsValid, System.Net.HttpStatusCode.OK, result.ErrorMessage, responseresult);
            }
            else
                return new OperationResult<GmailReplyEmailResponse>(false, System.Net.HttpStatusCode.OK, CommonMessage.UnAuthorizedUser);
        }

        /// <summary>
        /// This API used to send email with attachment
        /// </summary>
        /// <param name="composeMessage"></param>
        /// <returns></returns>

        [HttpPost]
        public async Task<OperationResult<SendEmailResponse>> SendEmail([FromForm] SendEmailRequest composeMessage)
        {

            // var file = Request.Form.Files;
            // composeMessage = JsonConvert.DeserializeObject<ComposeEmail>(
            //        Convert.ToString(HttpContext.Request.Form["model"])
            //    );

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var requestmodel = _mapper.Map<ComposeEmail1>(composeMessage);
            if (UserId > 0)
            {
                if (requestmodel.FileList != null && requestmodel.FileList.Count() > 0)
                {
                    foreach (IFormFile file in requestmodel.FileList)
                    {
                        if (OneClappContext.ClamAVServerIsLive)
                        {
                            ScanDocument scanDocumentObj = new ScanDocument();
                            bool fileStatus = await scanDocumentObj.ScanDocumentWithClam(file);
                            if (fileStatus)
                            {
                                return new OperationResult<SendEmailResponse>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                            }
                        }
                    }
                }
                // var result = await mailInbox.SendEmail(composeMessage.UserId, composeMessage, file);
                var result = await mailInbox.SendEmail(UserId, requestmodel, requestmodel.FileList);
                var threadId = "";
                var responseresult = _mapper.Map<SendEmailResponse>(result);
                
                if (result != null)
                {
                    threadId = result.threadId;
                }
                await _hubContext.Clients.All.OnMailModuleEvent(UserId, "mail", "addupdate", threadId);
                return new OperationResult<SendEmailResponse>(result.IsValid, System.Net.HttpStatusCode.OK, result.ErrorMessage, responseresult);
            }
            else
                return new OperationResult<SendEmailResponse>(false, System.Net.HttpStatusCode.OK, CommonMessage.UnAuthorizedUser);

        }

        [HttpPost("{MessageId}")]
        // public async Task<OperationResult<ComposeEmail>> SendEmail([FromForm]ComposeEmail composeMessage, IFormFile[] FileList)
        public async Task<OperationResult<SendReplyResponse>> SendReply(string MessageId, [FromForm] SendReplyRequest composeMessage)
        {

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var requestmodel = _mapper.Map<ComposeEmail1>(composeMessage);
            if (UserId > 0)
            {
                if (requestmodel.FileList != null && requestmodel.FileList.Count() > 0)
                {
                    foreach (IFormFile file in requestmodel.FileList)
                    {
                        if (OneClappContext.ClamAVServerIsLive)
                        {
                            ScanDocument scanDocumentObj = new ScanDocument();
                            bool fileStatus = await scanDocumentObj.ScanDocumentWithClam(file);
                            if (fileStatus)
                            {
                                return new OperationResult<SendReplyResponse>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                            }
                        }
                    }
                }
                // var result = await mailInbox.SendEmail(composeMessage.UserId, composeMessage, file);
                var result = await customMailbox.SendReply(requestmodel, UserId, MessageId);
                var composeMessageresult = _mapper.Map<SendReplyResponse>(composeMessage);
                return new OperationResult<SendReplyResponse>(requestmodel.IsValid, System.Net.HttpStatusCode.OK, "", composeMessageresult);
            }
            else
                return new OperationResult<SendReplyResponse>(false, System.Net.HttpStatusCode.OK, CommonMessage.UnAuthorizedUser);

        }

        /// <summary>
        /// Send Email with replacement perameter 
        /// </summary>
        /// <param name="composeMessage"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<OperationResult<CreateGmailDraftResponse>> CreateDraft([FromForm] CreateGmailDraftRequest composeEmail1)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var requestmodel = _mapper.Map<ComposeEmail1>(composeEmail1);
            if (UserId > 0)
            {
                if (requestmodel.FileList != null && requestmodel.FileList.Count() > 0)
                {
                    foreach (IFormFile file in requestmodel.FileList)
                    {
                        if (OneClappContext.ClamAVServerIsLive)
                        {
                            ScanDocument scanDocumentObj = new ScanDocument();
                            bool fileStatus = await scanDocumentObj.ScanDocumentWithClam(file);
                            if (fileStatus)
                            {
                                return new OperationResult<CreateGmailDraftResponse>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                            }
                        }
                    }
                }
                var ComposeEmailObj = _mapper.Map<ComposeEmail>(requestmodel);
                var result = await mailInbox.CreateEmailDraft(UserId, ComposeEmailObj, requestmodel.FileList);
                var resposneresult = _mapper.Map<CreateGmailDraftResponse>(result);
                await _hubContext.Clients.All.OnMailModuleEvent(UserId, "mail", "addupdate", "");
                return new OperationResult<CreateGmailDraftResponse>(result.IsValid, System.Net.HttpStatusCode.OK, result.ErrorMessage, resposneresult);
            }
            else
                return new OperationResult<CreateGmailDraftResponse>(false, System.Net.HttpStatusCode.OK, CommonMessage.UnAuthorizedUser);

        }

        /// <summary>
        /// Create a draft
        /// </summary>
        /// <param name="office365Draft"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<OperationResult<Office365CreateDraftResponse>> Office365CreateDraft([FromForm] Office365CreateDraftRequest composeMessage)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var requestmodel = _mapper.Map<ComposeEmail1>(composeMessage);
            if (UserId > 0)
            {
                if (requestmodel.FileList != null && requestmodel.FileList.Count() > 0)
                {
                    foreach (IFormFile file in requestmodel.FileList)
                    {
                        if (OneClappContext.ClamAVServerIsLive)
                        {
                            ScanDocument scanDocumentObj = new ScanDocument();
                            bool fileStatus = await scanDocumentObj.ScanDocumentWithClam(file);
                            if (fileStatus)
                            {
                                return new OperationResult<Office365CreateDraftResponse>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                            }
                        }
                    }
                }
                var result = await mailInbox.Office365CreateDraft(UserId, requestmodel);
                var responseresult = _mapper.Map<Office365CreateDraftResponse>(result);
                await _hubContext.Clients.All.OnMailModuleEvent(UserId, "mail", "addupdate", "");
                return new OperationResult<Office365CreateDraftResponse>(result.IsValid, System.Net.HttpStatusCode.OK, result.ErrorMessage, responseresult);
            }
            else
                return new OperationResult<Office365CreateDraftResponse>(false, System.Net.HttpStatusCode.OK, CommonMessage.UnAuthorizedUser);
        }

        [HttpPost("{threadId}")]
        public async Task<OperationResult<MessageSent>> Office365Attachments(string threadId, [FromBody] Attachments office365Attachments)
        {

            var filePath = _hostingEnvironment.WebRootPath + "\\ProfileImageUpload\\Layout620220128_151730.jpg";
            byte[] contentBytes = System.IO.File.ReadAllBytes(filePath);
            string base64 = Convert.ToBase64String(contentBytes);

            office365Attachments.odatatype = "#microsoft.graph.fileAttachment";
            // office365Attachments.odatatype = "#microsoft.graph.itemAttachment";
            office365Attachments.contentBytes = base64;
            office365Attachments.contentType = "image/jpeg";
            office365Attachments.name = "test";

            var result = await mailInbox.Office365Attachments(threadId, office365Attachments);
            return new OperationResult<MessageSent>(result.IsValid, System.Net.HttpStatusCode.OK, result.ErrorMessage, result);

        }


        /// <summary>
        /// This API used to get email threads
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<OperationResult<ThreadsResponse>> ThreadList([FromBody] ThreadsRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            ThreadsResponse threadsResponse = new ThreadsResponse();
            MailTokenDto mailTokenDto = new MailTokenDto();
            var requestmodel = _mapper.Map<MailTokenDto>(model);
            var TeamInboxCreatedBy = UserId;
            if (requestmodel.teamInboxId != null)
            {
                var teamInboxObj = _teamInboxService.GetById(requestmodel.teamInboxId.Value);
                if (teamInboxObj != null)
                {
                    if (teamInboxObj.CreatedBy != null)
                    {
                        TeamInboxCreatedBy = teamInboxObj.CreatedBy.Value;
                    }
                    if (teamInboxObj.IntProviderAppSecret != null)
                    {
                        requestmodel.SelectedEmail = teamInboxObj.IntProviderAppSecret.Email;
                        if (teamInboxObj.IntProviderAppSecret.IntProviderApp != null)
                        {
                            requestmodel.ProviderApp = teamInboxObj.IntProviderAppSecret.IntProviderApp.Name;
                            if (teamInboxObj.IntProviderAppSecret.IntProviderApp.IntProvider != null)
                            {
                                requestmodel.Provider = teamInboxObj.IntProviderAppSecret.IntProviderApp.IntProvider.Name;
                            }
                        }
                    }
                }
            }
            if (UserId > 0 && !string.IsNullOrEmpty(requestmodel.SelectedEmail))
            {
                InboxVM inboxVMObj = new InboxVM();
                inboxVMObj.Provider = requestmodel.Provider;
                inboxVMObj.ProviderApp = requestmodel.ProviderApp;
                inboxVMObj.SelectedEmail = requestmodel.SelectedEmail;
                if (requestmodel.teamInboxId != null)
                {
                    inboxVMObj.UserId = TeamInboxCreatedBy;
                }
                else
                {
                    inboxVMObj.UserId = UserId;
                }
                if (requestmodel.top != null)
                {
                    inboxVMObj.Top = requestmodel.top.Value;
                }
                else
                {
                    inboxVMObj.Top = 20;
                }

                if (requestmodel.skip != null)
                {
                    inboxVMObj.Skip = requestmodel.skip.Value;
                }
                else
                {
                    inboxVMObj.Skip = 0;
                }
                // inboxVM.Top = 20;
                inboxVMObj.Label = requestmodel.label;
                inboxVMObj.NextPageToken = requestmodel.nextPageToken;
                var result = await mailInbox.GetThread(TeamInboxCreatedBy, inboxVMObj);
                // if(result != null && )
                // foreach (var threadItem in result.InboxThread)
                // {
                //     threadItem.CreatedOn = Common.UnixTimeStampToDateTimeMilliSec(threadItem.InternalDate);
                // }

                // var discussionList = _discussionService.GetByUser(userId);

                // List<InboxThreadAndDiscussion> discussions = _mapper.Map<List<InboxThreadAndDiscussion>>(discussionList);
                List<InboxThreadAndDiscussion> inboxThreadAndDiscussionList = _mapper.Map<List<InboxThreadAndDiscussion>>(result.InboxThread);

                // List<InboxThreadAndDiscussion> mailAndDiscussions = discussions.Concat(threads).Where(t => t.CreatedOn != null).ToList();

                if (requestmodel.skip == null)
                {
                    requestmodel.skip = 0;
                }
                if (requestmodel.top == null)
                {
                    requestmodel.top = 20;
                }

                // mailAndDiscussions = mailAndDiscussions.OrderByDescending(t => t.CreatedOn.Value).Skip(model.skip.Value).Take(model.top.Value).ToList();
                // result.MailAndDiscussions = mailAndDiscussions;

                // List<dynamic> objectList = result.InboxThread.Cast<dynamic>()
                //   .Concat(discussionList)
                //   .ToList();

                // var data = Sort<InboxThread>(objectList, "CreatedOn");

                //             List<object> objectList = result.InboxThread.Cast<object>()
                // .Concat(discussionList).Take(model.top.Value).Skip(model.skip.Value).OrderByDescending(item => item.GetReflectedPropertyValue("CreatedOn"))
                // .ToList().OrderByDescending(t => (t.GetType().GetProperty("CreatedOn")));
                threadsResponse = _mapper.Map<ThreadsResponse>(result);
                return new OperationResult<ThreadsResponse>(true, System.Net.HttpStatusCode.OK, result.ErrorMessage, threadsResponse);
            }
            else
                return new OperationResult<ThreadsResponse>(false, System.Net.HttpStatusCode.OK, "Unauthorize");
        }


        [HttpPost]
        public async Task<OperationResult<InboxThreads>> ThreadDiscussionList([FromBody] MailTokenDto model)
        {
            InboxThreads inboxThreadsObj = new InboxThreads();
            inboxThreadsObj.InboxThread = new List<InboxThread>();
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            List<Discussion> discussionList = new List<Discussion>();

            if (model.type.ToLower() == "discussion")
            {
                discussionList = _discussionService.GetByUser(UserId);
                // discussionDtos = _mapper.Map<List<DiscussionDto>>(discussionList);
            }
            else if (model.type.ToLower() == "mail")
            {
                MailTokenDto mailTokenDto = new MailTokenDto();
                if (UserId > 0 && !string.IsNullOrEmpty(model.SelectedEmail))
                {
                    InboxVM inboxVMObj = new InboxVM();
                    inboxVMObj.Provider = model.Provider;
                    inboxVMObj.ProviderApp = model.ProviderApp;
                    inboxVMObj.SelectedEmail = model.SelectedEmail;
                    inboxVMObj.UserId = UserId;
                    if (model.top != null)
                    {
                        inboxVMObj.Top = model.top.Value;
                    }
                    else
                    {
                        inboxVMObj.Top = 20;
                    }

                    if (model.skip != null)
                    {
                        inboxVMObj.Skip = model.skip.Value;
                    }
                    else
                    {
                        inboxVMObj.Skip = 0;
                    }
                    // inboxVM.Top = 20;
                    inboxVMObj.Label = model.label;
                    inboxVMObj.NextPageToken = model.nextPageToken;
                    inboxThreadsObj = await mailInbox.GetThread(UserId, inboxVMObj);
                }


                // userId = 6;

                //             List<object> objectList = result.InboxThread.Cast<object>()
                // .Concat(discussionList)
                // .ToList();

            }
            else if (string.IsNullOrEmpty(model.type))
            {
                var intProviderList = _intProviderService.GetAll();
                var intProviderAppList = _intProviderAppSecretService.GetAll();
                var assignMails = _mailAssignUserService.GetAllByTeamMember(UserId);
                if (assignMails != null && assignMails.Count() > 0)
                {
                    foreach (var assignMailObj in assignMails)
                    {
                        if (assignMailObj.IntProviderAppSecret != null && assignMailObj.IntProviderAppSecret.IntProviderApp != null)
                        {
                            var intProviderAppSecretObj = assignMailObj.IntProviderAppSecret;
                            var intProviderAppObj = assignMailObj.IntProviderAppSecret.IntProviderApp;
                            var intProviderObj = intProviderList.Where(t => t.Id == intProviderAppObj.IntProviderId).FirstOrDefault();

                            InboxVM inboxVMObj = new InboxVM();
                            inboxVMObj.UserId = UserId;
                            if (intProviderObj != null)
                            {
                                inboxVMObj.Provider = intProviderObj.Name;
                            }

                            inboxVMObj.ProviderApp = intProviderAppObj.Name;
                            inboxVMObj.SelectedEmail = intProviderAppSecretObj.Email;
                            inboxVMObj.Skip = 0;
                            inboxVMObj.Top = 20;

                            var result = await mailInbox.GetThreadByThreadId(UserId, assignMailObj.ThreadId, inboxVMObj);
                            if (result != null && result.Count() > 0)
                            {
                                var inboxThread = _mapper.Map<InboxThread>(result[0]);
                                inboxThreadsObj.InboxThread.Add(inboxThread);
                            }
                        }
                    }
                }
                discussionList = _discussionService.GetByAssignUser(UserId);

            }

            if (inboxThreadsObj.InboxThread != null && inboxThreadsObj.InboxThread.Count() > 0)
            {
                foreach (var threadItem in inboxThreadsObj.InboxThread)
                {
                    threadItem.CreatedOn = Common.UnixTimeStampToDateTimeMilliSec(threadItem.InternalDate);
                }
            }

            // var discussionList = _discussionService.GetByUser(userId);

            List<InboxThreadAndDiscussion> discussionsList = _mapper.Map<List<InboxThreadAndDiscussion>>(discussionList);
            List<InboxThreadAndDiscussion> threadList = _mapper.Map<List<InboxThreadAndDiscussion>>(inboxThreadsObj.InboxThread);

            List<InboxThreadAndDiscussion> mailAndDiscussionsList = discussionsList.Concat(threadList).Where(t => t.CreatedOn != null).ToList();

            if (model.skip == null)
            {
                model.skip = 0;
            }
            if (model.top == null)
            {
                model.top = 20;
            }

            mailAndDiscussionsList = mailAndDiscussionsList.OrderByDescending(t => t.CreatedOn.Value).Skip(model.skip.Value).Take(model.top.Value).ToList();
            inboxThreadsObj.MailAndDiscussions = mailAndDiscussionsList;
            inboxThreadsObj.count = mailAndDiscussionsList.Count();
            return new OperationResult<InboxThreads>(true, System.Net.HttpStatusCode.OK, inboxThreadsObj.ErrorMessage, inboxThreadsObj);

        }

        [HttpPost]
        public async Task<OperationResult<MailAssignUserResposne>> AssignUser([FromBody] MailAssignUserRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var requestmodel = _mapper.Map<MailAssignUserDto>(model);
            var responsemodel = _mapper.Map<MailAssignUserResposne>(model);

            if (string.IsNullOrEmpty(requestmodel.ThreadId))
            {
                return new OperationResult<MailAssignUserResposne>(false, System.Net.HttpStatusCode.OK, "Please provide email thread", responsemodel);
            }
            else if (requestmodel.TeamMemberId == null)
            {
                return new OperationResult<MailAssignUserResposne>(false, System.Net.HttpStatusCode.OK, "Please provide team member", responsemodel);
            }
            else if (string.IsNullOrEmpty(requestmodel.SelectedEmail))
            {
                return new OperationResult<MailAssignUserResposne>(false, System.Net.HttpStatusCode.OK, "Please provide selected email", responsemodel);
            }
            else if (!string.IsNullOrEmpty(requestmodel.ProviderApp))
            {
                // return new OperationResult<MailAssignUserDto>(false, "Please provide app name", model);
                var intProviderAppObj = _intProviderAppService.GetIntProviderApp(requestmodel.ProviderApp);
                var intProviderAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUserAndEmail(UserId, requestmodel.SelectedEmail, intProviderAppObj.Id);

                if (intProviderAppSecretObj != null && intProviderAppObj != null)
                {
                    requestmodel.IntProviderAppSecretId = intProviderAppSecretObj.Id;
                }
            }
            else if (string.IsNullOrEmpty(requestmodel.ProviderApp))
            {
                var customEmailDomainObj = _customDomainEmailConfigService.GetByUserAndEmail(UserId, requestmodel.SelectedEmail);
                if (customEmailDomainObj != null)
                {
                    requestmodel.IntProviderAppSecretId = customEmailDomainObj.IntProviderAppSecretId;
                }
                else
                {
                    return new OperationResult<MailAssignUserResposne>(false, System.Net.HttpStatusCode.OK, "Please provide app name", responsemodel);
                }
            }
            requestmodel.UpdatedBy = UserId;
            var mailAssignUserObj = await _mailAssignUserService.CheckInsertOrUpdate(requestmodel);
            var mailAssignUserDto = _mapper.Map<MailAssignUserDto>(mailAssignUserObj);
            var mailAssignUserresponse = _mapper.Map<MailAssignUserResposne>(mailAssignUserDto);
            await _hubContext.Clients.All.OnMailModuleEvent(requestmodel.TeamMemberId, "mail", "assignuser", "");

            return new OperationResult<MailAssignUserResposne>(true, System.Net.HttpStatusCode.OK, "Mail assigned sucessessfully.", mailAssignUserresponse);
        }


        [HttpDelete]
        public async Task<OperationResult<MailUnAssignUserResponse>> UnAssign([FromBody] MailUnAssignUserRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var requestmodel = _mapper.Map<MailAssignUserDto>(model);
            var resposemodel = _mapper.Map<MailUnAssignUserResponse>(model);
            if (string.IsNullOrEmpty(requestmodel.ThreadId))
            {
                return new OperationResult<MailUnAssignUserResponse>(false, System.Net.HttpStatusCode.OK, "Please provide email thread", resposemodel);
            }

            var mailAssignUserObj = _mailAssignUserService.DeleteByThread(requestmodel.ThreadId);
            await _hubContext.Clients.All.OnMailModuleEvent(mailAssignUserObj.TeamMemberId, "mail", "assignuser", "");
            return new OperationResult<MailUnAssignUserResponse>(true, System.Net.HttpStatusCode.OK, "UnAssigned", resposemodel);
        }

        [HttpPost]
        public async Task<OperationResult<MailAssignCustomerResponse>> AssignCustomer([FromBody] MailAssignCustomerRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var requestmodel = _mapper.Map<MailAssignCustomerDto>(model);
            var responsemodel = _mapper.Map<MailAssignCustomerResponse>(model);
            if (string.IsNullOrEmpty(requestmodel.ThreadId))
            {
                return new OperationResult<MailAssignCustomerResponse>(false, System.Net.HttpStatusCode.OK, "Please provide email thread", responsemodel);
            }
            else if (string.IsNullOrEmpty(requestmodel.SelectedEmail))
            {
                return new OperationResult<MailAssignCustomerResponse>(false, System.Net.HttpStatusCode.OK, "Please provide selected email", responsemodel);
            }
            else if (requestmodel.CustomerId == null)
            {
                return new OperationResult<MailAssignCustomerResponse>(false, System.Net.HttpStatusCode.OK, "Please provide customer", responsemodel);
            }
            else if (!string.IsNullOrEmpty(requestmodel.ProviderApp))
            {
                var intProviderAppObj = _intProviderAppService.GetIntProviderApp(requestmodel.ProviderApp);
                var intProviderAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUserAndEmail(UserId, requestmodel.SelectedEmail, intProviderAppObj.Id);

                if (intProviderAppSecretObj != null && intProviderAppObj != null)
                {
                    requestmodel.IntProviderAppSecretId = intProviderAppSecretObj.Id;
                }
            }
            else if (string.IsNullOrEmpty(requestmodel.ProviderApp))
            {
                var customEmailDomainObj = _customDomainEmailConfigService.GetByUserAndEmail(UserId, requestmodel.SelectedEmail);
                if (customEmailDomainObj != null)
                {
                    requestmodel.IntProviderAppSecretId = customEmailDomainObj.IntProviderAppSecretId;
                }
                else
                {
                    return new OperationResult<MailAssignCustomerResponse>(false, System.Net.HttpStatusCode.OK, "Please provide app name", responsemodel);
                }
            }

            requestmodel.CreatedBy = UserId;
            var mailAssignCustomerObj = await _mailAssignCustomerService.CheckInsertOrUpdate(requestmodel);
            var mailAssignCustomerDto = _mapper.Map<MailAssignCustomerDto>(mailAssignCustomerObj);
            var mailAssignCustomerreponse = _mapper.Map<MailAssignCustomerResponse>(mailAssignCustomerDto);
            // await _hubContext.Clients.All.OnMailModuleEvent(model.CustomerId, "mail", "assignuser", "");

            return new OperationResult<MailAssignCustomerResponse>(true, "Mail assigned to customer sucessessfully.", mailAssignCustomerreponse);
        }


        [HttpDelete]
        public async Task<OperationResult<MailUnAssignCustomerResponse>> UnAssignCustomer([FromBody] MailUnAssignCustomerRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            var requestmodel = _mapper.Map<MailAssignCustomerDto>(model);
            var responsemodel = _mapper.Map<MailUnAssignCustomerResponse>(model);

            if (string.IsNullOrEmpty(requestmodel.ThreadId))
            {
                return new OperationResult<MailUnAssignCustomerResponse>(false, System.Net.HttpStatusCode.OK, "Please provide email thread", responsemodel);
            }

            var mailAssignCustomerObj = _mailAssignCustomerService.DeleteByThread(requestmodel.ThreadId);
            // await _hubContext.Clients.All.OnMailModuleEvent(mailAssignUserObj.CustomerId, "mail", "unassign", "");
            return new OperationResult<MailUnAssignCustomerResponse>(true, System.Net.HttpStatusCode.OK, "UnAssigned", responsemodel);
        }

        [HttpPost]
        public async Task<OperationResult<MailParticipantResponse>> Participate([FromBody] MailParticipantRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var requestmodel = _mapper.Map<MailParticipantDto>(model);
            var responsemodel = _mapper.Map<MailParticipantResponse>(model);
            if (string.IsNullOrEmpty(requestmodel.ThreadId))
            {
                return new OperationResult<MailParticipantResponse>(false, System.Net.HttpStatusCode.OK, "Please provide email thread", responsemodel);
            }
            else if (requestmodel.TeamMemberId == null)
            {
                return new OperationResult<MailParticipantResponse>(false, System.Net.HttpStatusCode.OK, "Please provide team member", responsemodel);
            }
            else if (string.IsNullOrEmpty(requestmodel.SelectedEmail))
            {
                return new OperationResult<MailParticipantResponse>(false, System.Net.HttpStatusCode.OK, "Please provide selected email", responsemodel);
            }
            else if (!string.IsNullOrEmpty(requestmodel.ProviderApp))
            {
                // return new OperationResult<MailParticipantDto>(false, "Please provide app name", model);
                var intProviderAppObj = _intProviderAppService.GetIntProviderApp(requestmodel.ProviderApp);
                var intProviderAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUserAndEmail(UserId, requestmodel.SelectedEmail, intProviderAppObj.Id);

                if (intProviderAppSecretObj != null && intProviderAppObj != null)
                {
                    requestmodel.IntProviderAppSecretId = intProviderAppSecretObj.Id;
                }
            }
            else if (!string.IsNullOrEmpty(requestmodel.ProviderApp))
            {
                var customEmailDomainObj = _customDomainEmailConfigService.GetByUserAndEmail(UserId, requestmodel.SelectedEmail);
                if (customEmailDomainObj != null)
                {
                    requestmodel.IntProviderAppSecretId = customEmailDomainObj.IntProviderAppSecretId;
                }
                else
                {
                    return new OperationResult<MailParticipantResponse>(false, System.Net.HttpStatusCode.OK, "Please provide app name", responsemodel);
                }
            }

            requestmodel.CreatedBy = UserId;
            var mailAssignUserObj = await _mailParticipantService.CheckInsertOrUpdate(requestmodel);
            var mailParticipantDto = _mapper.Map<MailParticipantDto>(mailAssignUserObj);
            var mailparticipantresponse = _mapper.Map<MailParticipantResponse>(mailParticipantDto);
            await _hubContext.Clients.All.OnMailModuleEvent(mailAssignUserObj.TeamMemberId, "mail", "shareduser", mailAssignUserObj.ThreadId);
            return new OperationResult<MailParticipantResponse>(true, System.Net.HttpStatusCode.OK, "Mail invited sucessessfully.", mailparticipantresponse);
        }

        [Authorize]
        [HttpGet]
        public async Task<OperationResult<UserMailInboxDto>> InboxList()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            UserMailInboxDto userMailInboxDto = new UserMailInboxDto();

            var userObj = _userService.GetUserById(UserId);

            var CreatedBy = UserId;
            if (userObj.CreatedBy != null)
            {
                CreatedBy = userObj.CreatedBy.Value;
            }

            var mailBoxTeamObj = _mailBoxTeamService.GetByUser(CreatedBy);
            List<long> teamInboxSecretIdList = new List<long>();

            if (mailBoxTeamObj != null)
            {
                var teamInboxes = _teamInboxService.GetByTeam(mailBoxTeamObj.Id);
                userMailInboxDto.MailTeam = _mapper.Map<MailBoxTeamDto>(mailBoxTeamObj);
                userMailInboxDto.UserId = UserId;

                if (teamInboxes != null && teamInboxes.Count() > 0)
                {
                    foreach (var teamInboxItem in teamInboxes)
                    {
                        var teamInboxDto = _mapper.Map<TeamInboxDto>(teamInboxItem);
                        teamInboxDto.IntProviderAppSecretDto = _mapper.Map<IntProviderAppSecretDto>(teamInboxItem.IntProviderAppSecret);
                        if (teamInboxItem.IntProviderAppSecret.IntProviderApp != null)
                        {
                            teamInboxDto.IntProviderAppSecretDto.IntProviderAppName = teamInboxItem.IntProviderAppSecret.IntProviderApp.Name;
                            if (teamInboxItem.IntProviderAppSecret.IntProviderApp.IntProvider != null)
                            {
                                teamInboxDto.IntProviderAppSecretDto.ProviderName = teamInboxItem.IntProviderAppSecret.IntProviderApp.IntProvider.Name;
                            }
                        }
                        userMailInboxDto.MailTeam.TeamInboxes.Add(teamInboxDto);
                        teamInboxSecretIdList.Add(teamInboxItem.IntProviderAppSecret.Id);
                    }
                }
            }

            var personalInboxes = _intProviderAppSecretService.GetAllByUser(UserId);
            var personalIntProviderAppSecretIds = personalInboxes.Select(t => t.Id).ToList();
            if (personalIntProviderAppSecretIds != null && personalIntProviderAppSecretIds.Count() > 0)
            {
                var filteredIntProviderAppSecretIds = personalIntProviderAppSecretIds.Where(p => teamInboxSecretIdList.All(p2 => p2 != p)).ToList();
                if (personalInboxes != null && personalInboxes.Count() > 0)
                {
                    foreach (var item in personalInboxes)
                    {
                        if (filteredIntProviderAppSecretIds.Contains(item.Id))
                        {
                            var intProviderAppSecretDto = _mapper.Map<IntProviderAppSecretDto>(item);
                            if (item.IntProviderApp != null)
                            {
                                intProviderAppSecretDto.IntProviderAppName = item.IntProviderApp.Name;
                                if (item.IntProviderApp.IntProvider != null)
                                {
                                    intProviderAppSecretDto.ProviderName = item.IntProviderApp.IntProvider.Name;
                                }
                            }

                            userMailInboxDto.Inboxes.Add(intProviderAppSecretDto);
                        }
                    }
                }
            }

            var intProviderAppSecrets = _intProviderAppSecretService.GetAllByUser(UserId);

            return new OperationResult<UserMailInboxDto>(true, System.Net.HttpStatusCode.OK, "", userMailInboxDto);

        }

        /// <summary>
        /// This API used to trash message
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("{MessageId}")]
        public async Task<OperationResult<TrashResponse>> Trash(string MessageId, [FromBody] TrashRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            MailTokenDto mailTokenDto = new MailTokenDto();
            var requestmodel = _mapper.Map<MailTokenDto>(model);
            if (UserId > 0 && !string.IsNullOrEmpty(requestmodel.SelectedEmail))
            {
                InboxVM inboxVMObj = new InboxVM();
                inboxVMObj.Provider = requestmodel.Provider;
                inboxVMObj.ProviderApp = requestmodel.ProviderApp;
                inboxVMObj.SelectedEmail = requestmodel.SelectedEmail;
                inboxVMObj.UserId = UserId;
                inboxVMObj.Label = requestmodel.label;
                var result = await mailInbox.TrashEmail(UserId, inboxVMObj, MessageId);
                var responseresult = _mapper.Map<TrashResponse>(result);
                return new OperationResult<TrashResponse>(result.IsValid, System.Net.HttpStatusCode.OK, result.ErrorMessage, responseresult);
            }
            else
                return new OperationResult<TrashResponse>(false, System.Net.HttpStatusCode.OK, "Unauthorize");
        }


        /// <summary>
        /// This API used to trash message
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("{ThreadId}")]
        public async Task<OperationResult<TrashEmailByThreadResponse>> TrashEmailByThread(string ThreadId, [FromBody] TrashEmailByThreadRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            MailTokenDto mailTokenDto = new MailTokenDto();
            var requestmodel = _mapper.Map<MailTokenDto>(model);
            if (UserId > 0 && !string.IsNullOrEmpty(requestmodel.SelectedEmail))
            {
                InboxVM inboxVMObj = new InboxVM();
                inboxVMObj.Provider = requestmodel.Provider;
                inboxVMObj.ProviderApp = requestmodel.ProviderApp;
                inboxVMObj.SelectedEmail = requestmodel.SelectedEmail;
                inboxVMObj.UserId = UserId;
                inboxVMObj.Label = requestmodel.label;
                var result = await mailInbox.TrashEmailByThread(UserId, inboxVMObj, ThreadId);
                var responseresult = _mapper.Map<TrashEmailByThreadResponse>(result);
                return new OperationResult<TrashEmailByThreadResponse>(result.IsValid, System.Net.HttpStatusCode.OK, result.ErrorMessage, responseresult);
            }
            else
                return new OperationResult<TrashEmailByThreadResponse>(false, System.Net.HttpStatusCode.OK, "Unauthorize");
        }

        [Authorize]
        [HttpPost]
        public async Task<OperationResult<AssignTomeResponse>> AssignTome([FromBody] AssignTomeRequest model)
        {
            var requestmodel = _mapper.Map<MailTokenDto>(model);
            if (requestmodel.skip == null)
            {
                requestmodel.skip = 0;
            }
            if (requestmodel.top == null)
            {
                requestmodel.top = 20;
            }
            requestmodel.type = "";
            InboxThreads inboxThreadsObj = new InboxThreads();
            inboxThreadsObj.InboxThread = new List<InboxThread>();
            inboxThreadsObj.MailAndDiscussions = new List<InboxThreadAndDiscussion>();
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            List<Discussion> discussionList = new List<Discussion>();

            if (!string.IsNullOrEmpty(requestmodel.type) && requestmodel.type.ToLower() == "discussion")
            {
                discussionList = _discussionService.GetByUser(UserId);
            }
            else if (!string.IsNullOrEmpty(requestmodel.type) && requestmodel.type.ToLower() == "mail")
            {
                MailTokenDto mailTokenDto = new MailTokenDto();
                if (UserId > 0 && !string.IsNullOrEmpty(requestmodel.SelectedEmail))
                {
                    InboxVM inboxVMObj = new InboxVM();
                    inboxVMObj.Provider = requestmodel.Provider;
                    inboxVMObj.ProviderApp = requestmodel.ProviderApp;
                    inboxVMObj.SelectedEmail = requestmodel.SelectedEmail;
                    inboxVMObj.UserId = UserId;
                    if (requestmodel.top != null)
                    {
                        inboxVMObj.Top = requestmodel.top.Value;
                    }
                    else
                    {
                        inboxVMObj.Top = 20;
                    }

                    if (requestmodel.skip != null)
                    {
                        inboxVMObj.Skip = requestmodel.skip.Value;
                    }
                    else
                    {
                        inboxVMObj.Skip = 0;
                    }
                    // inboxVM.Top = 20;
                    inboxVMObj.Label = requestmodel.label;
                    inboxVMObj.NextPageToken = requestmodel.nextPageToken;
                    inboxThreadsObj = await mailInbox.GetThread(UserId, inboxVMObj);
                }

            }
            else if (string.IsNullOrEmpty(requestmodel.type))
            {
                var intProviderList = _intProviderService.GetAll();
                var intProviderAppList = _intProviderAppSecretService.GetAll();
                var assignMails = _mailAssignUserService.GetAllByTeamMember(UserId);
                if (assignMails != null && assignMails.Count() > 0)
                {
                    foreach (var assignMailObj in assignMails)
                    {
                        if (assignMailObj.IntProviderAppSecret != null)
                        {
                            var intProviderAppSecretObj = assignMailObj.IntProviderAppSecret;
                            IntProviderApp? intProviderAppObj = null;
                            IntProvider? intProviderObj = null;
                            if (assignMailObj.IntProviderAppSecret.IntProviderApp != null)
                            {
                                intProviderAppObj = assignMailObj.IntProviderAppSecret.IntProviderApp;
                                intProviderObj = intProviderList.Where(t => t.Id == intProviderAppObj.IntProviderId).FirstOrDefault();
                            }
                            InboxVM inboxVMObj = new InboxVM();
                            inboxVMObj.UserId = intProviderAppSecretObj.CreatedBy;
                            if (intProviderObj != null)
                            {
                                inboxVMObj.Provider = intProviderObj.Name;
                            }
                            if (intProviderAppObj != null)
                            {
                                inboxVMObj.ProviderApp = intProviderAppObj.Name;
                            }
                            inboxVMObj.SelectedEmail = intProviderAppSecretObj.Email;
                            inboxVMObj.Skip = 0;
                            inboxVMObj.Top = 20;
                            var SelectedMailUserId = UserId;
                            if (assignMailObj.IntProviderAppSecret != null && assignMailObj.IntProviderAppSecret.CreatedBy != null)
                            {
                                SelectedMailUserId = assignMailObj.IntProviderAppSecret.CreatedBy.Value;
                            }
                            var result = await mailInbox.GetThreadByThreadId(SelectedMailUserId, assignMailObj.ThreadId, inboxVMObj);

                            var customerAssignObj = _mailAssignCustomerService.GetMailAssignCustomerByThread(assignMailObj.ThreadId);
                            if (result != null && result.Count() > 0)
                            {
                                var inboxThread = _mapper.Map<InboxThread>(result[0]);
                                inboxThread.MailAssignUserId = assignMailObj.Id;
                                if (customerAssignObj != null)
                                {
                                    inboxThread.CustomerId = customerAssignObj.CustomerId;
                                }
                                inboxThreadsObj.InboxThread.Add(inboxThread);
                            }
                        }
                    }
                }
                discussionList = _discussionService.GetByAssignUser(UserId);
            }

            if (inboxThreadsObj.InboxThread != null && inboxThreadsObj.InboxThread.Count() > 0)
            {
                foreach (var threadItem in inboxThreadsObj.InboxThread)
                {
                    threadItem.CreatedOn = Common.UnixTimeStampToDateTimeMilliSec(threadItem.InternalDate);
                }
            }

            // var discussionList = _discussionService.GetByUser(userId);

            List<InboxThreadAndDiscussion> discussionsList = _mapper.Map<List<InboxThreadAndDiscussion>>(discussionList);
            List<InboxThreadAndDiscussion> threadList = _mapper.Map<List<InboxThreadAndDiscussion>>(inboxThreadsObj.InboxThread);

            if (discussionList != null && discussionList.Count() > 0)
            {
                var discussionReadList = _discussionReadService.GetAll();
                if (discussionsList != null && discussionsList.Count() > 0)
                {
                    foreach (var discussionObj in discussionsList)
                    {
                        if (discussionReadList != null && discussionReadList.Count() > 0)
                        {
                            var discussionReadObj = discussionReadList.Where(t => t.ReadBy == UserId && t.DiscussionId == discussionObj.Id).FirstOrDefault();
                            if (discussionReadObj != null)
                            {
                                discussionObj.IsRead = true;
                            }
                        }
                    }
                }
            }
            List<InboxThreadAndDiscussion> mailAndDiscussionsList = discussionsList.Concat(threadList).Where(t => t.CreatedOn != null).ToList();

            if (requestmodel.skip == null)
            {
                requestmodel.skip = 0;
            }
            if (requestmodel.top == null)
            {
                requestmodel.top = 20;
            }
            inboxThreadsObj.InboxThread = null;
            mailAndDiscussionsList = mailAndDiscussionsList.OrderByDescending(t => t.CreatedOn.Value).Skip(requestmodel.skip.Value).Take(requestmodel.top.Value).ToList();
            inboxThreadsObj.MailAndDiscussions = mailAndDiscussionsList;
            inboxThreadsObj.count = mailAndDiscussionsList.Count();
            var assigntomeresponse = _mapper.Map<AssignTomeResponse>(inboxThreadsObj);
            return new OperationResult<AssignTomeResponse>(true, System.Net.HttpStatusCode.OK, assigntomeresponse.ErrorMessage, assigntomeresponse);
        }

        [Authorize]
        [HttpPost("{CustomerId}")]
        public async Task<OperationResult<AssignToCustomerResponse>> AssignToCustomer(long CustomerId, [FromBody] AssignToCustomerRequest model)
        {
            var requestmodel = _mapper.Map<MailTokenDto>(model);
            if (requestmodel.skip == null)
            {
                requestmodel.skip = 0;
            }
            if (requestmodel.top == null)
            {
                requestmodel.top = 20;
            }
            InboxThreads inboxThreadsObj = new InboxThreads();
            inboxThreadsObj.InboxThread = new List<InboxThread>();
            inboxThreadsObj.MailAndDiscussions = new List<InboxThreadAndDiscussion>();
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            List<Discussion> discussionList = new List<Discussion>();

            if (!string.IsNullOrEmpty(requestmodel.type) && requestmodel.type.ToLower() == "discussion")
            {
                discussionList = _discussionService.GetByAssignCustomer(CustomerId);
            }
            else if (!string.IsNullOrEmpty(requestmodel.type) && requestmodel.type.ToLower() == "mail")
            {
                var intProviderList = _intProviderService.GetAll();
                var intProviderAppList = _intProviderAppSecretService.GetAll();
                var assignCustomerMails = _mailAssignCustomerService.GetAllByCustomer(CustomerId);
                if (assignCustomerMails != null && assignCustomerMails.Count() > 0)
                {
                    foreach (var assignMailObj in assignCustomerMails)
                    {
                        if (assignMailObj.IntProviderAppSecret != null)
                        {
                            var intProviderAppSecretObj = assignMailObj.IntProviderAppSecret;
                            IntProviderApp? intProviderAppObj = null;
                            IntProvider? intProviderObj = null;
                            if (assignMailObj.IntProviderAppSecret.IntProviderApp != null)
                            {
                                intProviderAppObj = assignMailObj.IntProviderAppSecret.IntProviderApp;
                                intProviderObj = intProviderList.Where(t => t.Id == intProviderAppObj.IntProviderId).FirstOrDefault();
                            }

                            InboxVM inboxVMObj = new InboxVM();
                            inboxVMObj.UserId = intProviderAppSecretObj.CreatedBy;
                            if (intProviderObj != null)
                            {
                                inboxVMObj.Provider = intProviderObj.Name;
                            }
                            if (intProviderAppObj != null)
                            {
                                inboxVMObj.ProviderApp = intProviderAppObj.Name;
                            }

                            inboxVMObj.SelectedEmail = intProviderAppSecretObj.Email;
                            inboxVMObj.Skip = 0;
                            inboxVMObj.Top = 20;
                            var SelectedMailUserId = UserId;
                            if (assignMailObj.IntProviderAppSecret != null && assignMailObj.IntProviderAppSecret.CreatedBy != null)
                            {
                                SelectedMailUserId = assignMailObj.IntProviderAppSecret.CreatedBy.Value;
                            }
                            var result = await mailInbox.GetThreadByThreadId(SelectedMailUserId, assignMailObj.ThreadId, inboxVMObj);
                            if (result != null && result.Count() > 0)
                            {
                                var inboxThread = _mapper.Map<InboxThread>(result[0]);
                                inboxThread.MailAssignUserId = assignMailObj.Id;
                                inboxThreadsObj.InboxThread.Add(inboxThread);
                            }
                        }
                    }
                }

            }
            else if (string.IsNullOrEmpty(requestmodel.type))
            {
                var intProviderList = _intProviderService.GetAll();
                var intProviderAppList = _intProviderAppSecretService.GetAll();
                var assignCustomerMails = _mailAssignCustomerService.GetAllByCustomer(CustomerId);
                if (assignCustomerMails != null && assignCustomerMails.Count() > 0)
                {
                    foreach (var assignMailObj in assignCustomerMails)
                    {
                        if (assignMailObj.IntProviderAppSecret != null)
                        {
                            var intProviderAppSecretObj = assignMailObj.IntProviderAppSecret;
                            IntProviderApp? intProviderAppObj = null;
                            IntProvider? intProviderObj = null;
                            if (assignMailObj.IntProviderAppSecret.IntProviderApp != null)
                            {
                                intProviderAppObj = assignMailObj.IntProviderAppSecret.IntProviderApp;
                                intProviderObj = intProviderList.Where(t => t.Id == intProviderAppObj.IntProviderId).FirstOrDefault();
                            }

                            InboxVM inboxVMObj = new InboxVM();
                            inboxVMObj.UserId = intProviderAppSecretObj.CreatedBy;
                            if (intProviderObj != null)
                            {
                                inboxVMObj.Provider = intProviderObj.Name;
                            }
                            if (intProviderAppObj != null)
                            {
                                inboxVMObj.ProviderApp = intProviderAppObj.Name;
                            }
                            inboxVMObj.SelectedEmail = intProviderAppSecretObj.Email;
                            inboxVMObj.Skip = 0;
                            inboxVMObj.Top = 20;
                            var SelectedMailUserId = UserId;
                            if (assignMailObj.IntProviderAppSecret != null && assignMailObj.IntProviderAppSecret.CreatedBy != null)
                            {
                                SelectedMailUserId = assignMailObj.IntProviderAppSecret.CreatedBy.Value;
                            }
                            var result = await mailInbox.GetThreadByThreadId(SelectedMailUserId, assignMailObj.ThreadId, inboxVMObj);
                            if (result != null && result.Count() > 0)
                            {
                                var inboxThread = _mapper.Map<InboxThread>(result[0]);
                                inboxThread.MailAssignUserId = assignMailObj.Id;
                                inboxThreadsObj.InboxThread.Add(inboxThread);
                            }
                        }
                    }
                }
                discussionList = _discussionService.GetByAssignCustomer(CustomerId);
            }

            foreach (var threadItem in inboxThreadsObj.InboxThread)
            {
                threadItem.CreatedOn = Common.UnixTimeStampToDateTimeMilliSec(threadItem.InternalDate);
            }

            // var discussionList = _discussionService.GetByUser(userId);

            List<InboxThreadAndDiscussion> discussionsList = _mapper.Map<List<InboxThreadAndDiscussion>>(discussionList);
            List<InboxThreadAndDiscussion> threadsList = _mapper.Map<List<InboxThreadAndDiscussion>>(inboxThreadsObj.InboxThread);

            List<InboxThreadAndDiscussion> mailAndDiscussionsList = discussionsList.Concat(threadsList).Where(t => t.CreatedOn != null).ToList();

            if (requestmodel.skip == null)
            {
                requestmodel.skip = 0;
            }
            if (requestmodel.top == null)
            {
                requestmodel.top = 20;
            }
            inboxThreadsObj.InboxThread = null;
            mailAndDiscussionsList = mailAndDiscussionsList.OrderByDescending(t => t.CreatedOn.Value).Skip(requestmodel.skip.Value).Take(requestmodel.top.Value).ToList();
            inboxThreadsObj.MailAndDiscussions = mailAndDiscussionsList;
            inboxThreadsObj.count = mailAndDiscussionsList.Count();

            var responseresult = _mapper.Map<AssignToCustomerResponse>(inboxThreadsObj);
            return new OperationResult<AssignToCustomerResponse>(true, System.Net.HttpStatusCode.OK, responseresult.ErrorMessage, responseresult);
        }


        [Authorize]
        [HttpPost]
        public async Task<OperationResult<ShareTomeResponse>> ShareTome([FromBody] ShareTomeRequest model)
        {
            // model.page = 1;
            if (model.skip == null)
            {
                model.skip = 0;
            }
            if (model.top == null)
            {
                model.top = 20;
            }
            model.type = "";
            InboxThreads inboxThreadsObj = new InboxThreads();
            inboxThreadsObj.InboxThread = new List<InboxThread>();
            inboxThreadsObj.MailAndDiscussions = new List<InboxThreadAndDiscussion>();
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            List<Discussion> discussionList = new List<Discussion>();
            if (string.IsNullOrEmpty(model.type))
            {
                var intProviderList = _intProviderService.GetAll();
                var intProviderAppList = _intProviderAppSecretService.GetAll();
                var mailParticipants = _mailParticipantService.GetAllByTeamMember(UserId);
                if (mailParticipants != null && mailParticipants.Count() > 0)
                {
                    foreach (var mailParticipantObj in mailParticipants)
                    {
                        if (mailParticipantObj.IntProviderAppSecret != null)
                        {
                            var intProviderAppSecretObj = mailParticipantObj.IntProviderAppSecret;
                            IntProviderApp? intProviderAppObj = null;
                            IntProvider? intProviderObj = null;
                            if (mailParticipantObj.IntProviderAppSecret.IntProviderApp != null)
                            {
                                intProviderAppObj = mailParticipantObj.IntProviderAppSecret.IntProviderApp;
                                intProviderObj = intProviderList.Where(t => t.Id == intProviderAppObj.IntProviderId).FirstOrDefault();
                            }


                            InboxVM inboxVMObj = new InboxVM();
                            inboxVMObj.UserId = intProviderAppSecretObj.CreatedBy;
                            if (intProviderObj != null)
                            {
                                inboxVMObj.Provider = intProviderObj.Name;
                            }
                            if (intProviderAppObj != null)
                            {
                                inboxVMObj.ProviderApp = intProviderAppObj.Name;
                            }
                            inboxVMObj.SelectedEmail = intProviderAppSecretObj.Email;
                            inboxVMObj.Skip = 0;
                            inboxVMObj.Top = 20;

                            var SelectedMailUserId = UserId;
                            if (mailParticipantObj.IntProviderAppSecret != null && mailParticipantObj.IntProviderAppSecret.CreatedBy != null)
                            {
                                SelectedMailUserId = mailParticipantObj.IntProviderAppSecret.CreatedBy.Value;
                            }

                            var result = await mailInbox.GetThreadByThreadId(SelectedMailUserId, mailParticipantObj.ThreadId, inboxVMObj);
                            var customerAssignObj = _mailAssignCustomerService.GetMailAssignCustomerByThread(mailParticipantObj.ThreadId);
                            if (result != null && result.Count() > 0)
                            {
                                var inboxThread = _mapper.Map<InboxThread>(result[0]);
                                if (customerAssignObj != null)
                                {
                                    inboxThread.CustomerId = customerAssignObj.CustomerId;
                                }
                                inboxThreadsObj.InboxThread.Add(inboxThread);
                            }
                        }
                    }
                }
                // discussionList = _discussionService.GetByAssignUser(userId);
                var discussionParticipants = _discussionParticipantService.GetAllByTeamMate(UserId);
                if (discussionParticipants != null && discussionParticipants.Count() > 0)
                {
                    foreach (var discussionParticipantObj in discussionParticipants)
                    {
                        discussionList.Add(discussionParticipantObj.Discussion);
                    }
                }
                if (inboxThreadsObj.InboxThread != null && inboxThreadsObj.InboxThread.Count() > 0)
                {
                    foreach (var threadItem in inboxThreadsObj.InboxThread)
                    {
                        threadItem.CreatedOn = Common.UnixTimeStampToDateTimeMilliSec(threadItem.InternalDate);
                    }
                }
            }
            else if (!string.IsNullOrEmpty(model.type) && model.type.ToLower() == "discussion")
            {
                discussionList = _discussionService.GetByUser(UserId);
                // discussionDtos = _mapper.Map<List<DiscussionDto>>(discussionList);
            }
            else if (!string.IsNullOrEmpty(model.type) && model.type.ToLower() == "mail")
            {
                MailTokenDto mailTokenDto = new MailTokenDto();
                if (UserId > 0 && !string.IsNullOrEmpty(model.SelectedEmail))
                {
                    InboxVM inboxVMObj = new InboxVM();
                    inboxVMObj.Provider = model.Provider;
                    inboxVMObj.ProviderApp = model.ProviderApp;
                    inboxVMObj.SelectedEmail = model.SelectedEmail;
                    inboxVMObj.UserId = UserId;
                    if (model.top != null)
                    {
                        inboxVMObj.Top = model.top.Value;
                    }
                    else
                    {
                        inboxVMObj.Top = 20;
                    }

                    if (model.skip != null)
                    {
                        inboxVMObj.Skip = model.skip.Value;
                    }
                    else
                    {
                        inboxVMObj.Skip = 0;
                    }
                    // inboxVM.Top = 20;
                    inboxVMObj.Label = model.label;
                    inboxVMObj.NextPageToken = model.nextPageToken;
                    inboxThreadsObj = await mailInbox.GetThread(UserId, inboxVMObj);
                }
            }

            List<InboxThreadAndDiscussion> discussionsList = _mapper.Map<List<InboxThreadAndDiscussion>>(discussionList);
            List<InboxThreadAndDiscussion> threadsList = _mapper.Map<List<InboxThreadAndDiscussion>>(inboxThreadsObj.InboxThread);

            if (discussionList != null && discussionList.Count() > 0)
            {
                var discussionReadList = _discussionReadService.GetAll();
                if (discussionsList != null && discussionsList.Count() > 0)
                {
                    foreach (var discussionObj in discussionsList)
                    {
                        if (discussionReadList != null)
                        {
                            var discussionReadObj = discussionReadList.Where(t => t.ReadBy == UserId && t.DiscussionId == discussionObj.Id).FirstOrDefault();
                            if (discussionReadObj != null)
                            {
                                discussionObj.IsRead = true;
                            }
                        }
                    }
                }
            }

            List<InboxThreadAndDiscussion> mailAndDiscussionsList = discussionsList.Concat(threadsList).Where(t => t.CreatedOn != null).ToList();

            if (model.skip == null)
            {
                model.skip = 0;
            }
            if (model.top == null)
            {
                model.top = 20;
            }
            inboxThreadsObj.InboxThread = null;
            if (model.skip != null && model.top != null)
            {
                mailAndDiscussionsList = mailAndDiscussionsList.OrderByDescending(t => t.CreatedOn.Value).Skip(model.skip.Value).Take(model.top.Value).ToList();
            }
            inboxThreadsObj.MailAndDiscussions = mailAndDiscussionsList;
            inboxThreadsObj.count = mailAndDiscussionsList.Count();

            var sharetoresponse = _mapper.Map<ShareTomeResponse>(inboxThreadsObj);
            return new OperationResult<ShareTomeResponse>(true, System.Net.HttpStatusCode.OK, inboxThreadsObj.ErrorMessage, sharetoresponse);
        }


        #region TeamInbox apis start

        [HttpPost]
        public async Task<OperationResult<TeamInboxAddUpdateResponse>> TeamInbox([FromBody] TeamInboxAddUpdateRequest model)
        {
            var isAddRecord = true;
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var requestmodel = _mapper.Map<TeamInboxDto>(model);
            if (requestmodel.Id == null)
            {
                requestmodel.CreatedBy = UserId;
            }
            else
            {
                requestmodel.UpdatedBy = UserId;
            }

            if (requestmodel.Id != null && requestmodel.Id.Value > 0)
            {
                isAddRecord = false;
            }

            var teamInboxObj = await _teamInboxService.CheckInsertOrUpdate(requestmodel);
            requestmodel = _mapper.Map<TeamInboxDto>(teamInboxObj);
            var responsemodel = _mapper.Map<TeamInboxAddUpdateResponse>(requestmodel);
            if (teamInboxObj != null)
            {
                TeamInboxAccessDto teamInboxAccessDto = new TeamInboxAccessDto();
                teamInboxAccessDto.TeamInboxId = teamInboxObj.Id;
                teamInboxAccessDto.TeamMateId = UserId;
                teamInboxAccessDto.CreatedBy = UserId;
                TeamInboxAccess teamInboxAccessObj = await _teamInboxAccessService.CheckInsertOrUpdate(teamInboxAccessDto);
            }

            if (isAddRecord == true)
            {
                return new OperationResult<TeamInboxAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Added successfully", responsemodel);
            }
            else
            {
                return new OperationResult<TeamInboxAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Updated successfully", responsemodel);
            }

        }

        [HttpDelete("{TeamInBoxId}")]
        public async Task<OperationResult<RemoveTeamInboxResponse>> RemoveTeamInbox(long TeamInBoxId)
        {
            TeamInboxDto teamInboxDto = new TeamInboxDto();
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var teamInboxObj = _teamInboxService.GetById(TeamInBoxId);
            var responseresult = _mapper.Map<RemoveTeamInboxResponse>(teamInboxDto);
            if (teamInboxObj != null)
            {
                var teamInboxAccesses = await _teamInboxAccessService.DeleteByTeam(TeamInBoxId);

                IntProviderAppSecret? intProviderAppSecret;
                if (teamInboxObj.IntProviderAppSecret != null)
                {
                    intProviderAppSecret = teamInboxObj.IntProviderAppSecret;
                    var intProviderAppSecretId = intProviderAppSecret.Id;

                    var mailAssigns = await _mailAssignUserService.DeleteBySecretId(intProviderAppSecretId);
                    var mailParticipants = await _mailParticipantService.DeleteBySecretId(intProviderAppSecretId);
                    var mailAssignCustomers = await _mailAssignCustomerService.DeleteBySecretId(intProviderAppSecretId);

                    var calendarSyncActivities = _calendarSyncActivityService.GetByAppSecret(intProviderAppSecretId);
                    //  var calendarSyncData = _calendarSyncActivityService.GetCalendarSyncActivity(syncActivityDto);
                    if (calendarSyncActivities != null && calendarSyncActivities.Count() > 0)
                    {
                        foreach (var calendarSyncData in calendarSyncActivities)
                        {
                            if (calendarSyncData != null)
                            {
                                GoogleCalendarEventVM googleCalendarEventVMObj = new GoogleCalendarEventVM();
                                googleCalendarEventVMObj.id = calendarSyncData.CalendarEventId;
                                if (intProviderAppSecret.CreatedBy != null)
                                {
                                    var intProviderAppSecretDto = await CheckAccessToken(intProviderAppSecret.CreatedBy.Value);
                                    if (intProviderAppSecretDto != null && string.IsNullOrEmpty(intProviderAppSecretDto.error_description))
                                    {
                                        var customers = await _calendarService.DeleteEvent(GoogleCalendarApiKey, googleCalendarEventVMObj, intProviderAppSecretDto.Email, intProviderAppSecretDto.Token);
                                        var deleted1 = await _calendarSyncActivityService.DeleteCalendarSyncActivity(calendarSyncData.Id);
                                    }
                                }
                            }
                        }
                    }

                    var secretObj = await _intProviderAppSecretService.DeleteIntProviderAppSecret(intProviderAppSecretId);
                }
                var deleted = await _teamInboxService.DeleteTeamInbox(TeamInBoxId);
                return new OperationResult<RemoveTeamInboxResponse>(true, System.Net.HttpStatusCode.OK, "", responseresult);
            }
            return new OperationResult<RemoveTeamInboxResponse>(false, System.Net.HttpStatusCode.OK, "Team inbox not found", responseresult);
        }

        [HttpGet]
        public async Task<OperationResult<MailBoxTeamResponse>> Detail()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            MailBoxTeamDto mailBoxTeamDto = new MailBoxTeamDto();
            MailBoxTeamResponse responseMailBoxTeam = new MailBoxTeamResponse();
            User userObj = _userService.GetUserById(UserId);


            var createdBy = UserId;
            if (userObj.CreatedBy != null)
            {
                createdBy = userObj.CreatedBy.Value;
            }
            MailBoxTeam mailBoxTeamObj = _mailBoxTeamService.GetByUser(createdBy);

            if (mailBoxTeamObj != null)
            {
                mailBoxTeamDto = _mapper.Map<MailBoxTeamDto>(mailBoxTeamObj);
                responseMailBoxTeam = _mapper.Map<MailBoxTeamResponse>(mailBoxTeamObj);
            }
            return new OperationResult<MailBoxTeamResponse>(true, System.Net.HttpStatusCode.OK, "", responseMailBoxTeam);
        }

        [HttpGet]
        public async Task<OperationResult<List<TeamInboxResponse>>> TeamInboxes()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            var tenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var userList = _userService.GetAllUsersByTenantAdmin(tenantId);
            List<TeamInboxDto> teamInboxDtoList = new List<TeamInboxDto>();
            List<TeamInboxResponse> teamInboxResponseList = new List<TeamInboxResponse>();
            if (userList != null && userList.Count() > 0)
            {
                foreach (var userObj in userList)
                {
                    var teamInboxList = _teamInboxService.GetByUser(userObj.Id);
                    if (teamInboxList != null && teamInboxList.Count() > 0)
                    {
                        foreach (var teamInboxObj in teamInboxList)
                        {
                            TeamInboxDto teamInboxDto = new TeamInboxDto();
                            teamInboxDto = _mapper.Map<TeamInboxDto>(teamInboxObj);
                            if (teamInboxObj.IntProviderAppSecret != null)
                            {
                                teamInboxDto.IntProviderAppSecretDto = _mapper.Map<IntProviderAppSecretDto>(teamInboxObj.IntProviderAppSecret);
                                teamInboxDto.SelectedEmail = teamInboxObj.IntProviderAppSecret.Email;
                                if (teamInboxObj.IntProviderAppSecret.IntProviderApp != null)
                                {
                                    teamInboxDto.IntProviderAppName = teamInboxObj.IntProviderAppSecret.IntProviderApp.Name;
                                    if (teamInboxObj.IntProviderAppSecret.IntProviderApp.IntProvider != null)
                                    {
                                        teamInboxDto.ProviderName = teamInboxObj.IntProviderAppSecret.IntProviderApp.IntProvider.Name;
                                    }
                                }
                                else
                                {
                                    var customDomainEmailConfigObj = _customDomainEmailConfigService.GetByUserAndEmail(UserId, teamInboxDto.SelectedEmail);
                                    if (customDomainEmailConfigObj != null)
                                    {
                                        teamInboxDto.IntProviderAppSecretDto.CustomDomainEmailConfigId = customDomainEmailConfigObj.Id;
                                        teamInboxDto.IntProviderAppSecretDto.CustomDomainEmailConfigDto = _mapper.Map<CustomDomainEmailConfigDto>(customDomainEmailConfigObj);
                                    }
                                }
                            }
                            if (teamInboxObj.IsPublic == true)
                            {
                                teamInboxDtoList.Add(teamInboxDto);
                            }
                            else
                            {
                                var teamAccesses = _teamInboxAccessService.GetByTeamInbox(teamInboxObj.Id);
                                if (teamAccesses != null && teamAccesses.Count() > 0)
                                {
                                    var isExist = teamAccesses.Where(t => t.TeamMateId == UserId).FirstOrDefault();
                                    if (isExist != null)
                                    {
                                        teamInboxDto.TeamInboxAccesses = _mapper.Map<List<TeamInboxAccessDto>>(teamAccesses);
                                        teamInboxDtoList.Add(teamInboxDto);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // var teamInboxes = _teamInboxService.GetByUser(userId);

            // var teamInboxesAccessList = _teamInboxAccessService.GetByTeamMate(userId);


            // // teamInboxDtos = _mapper.Map<List<TeamInboxDto>>(teamInboxes);
            // foreach (var teamInboxObj in teamInboxes)
            // {
            //     TeamInboxDto teamInboxDto = new TeamInboxDto();
            //     teamInboxDto = _mapper.Map<TeamInboxDto>(teamInboxObj);
            //     if (teamInboxObj.IntProviderAppSecret != null)
            //     {
            //         teamInboxDto.IntProviderAppSecretDto = _mapper.Map<IntProviderAppSecretDto>(teamInboxObj.IntProviderAppSecret);
            //         teamInboxDto.SelectedEmail = teamInboxObj.IntProviderAppSecret.Email;
            //         if (teamInboxObj.IntProviderAppSecret.IntProviderApp != null)
            //         {
            //             teamInboxDto.IntProviderAppName = teamInboxObj.IntProviderAppSecret.IntProviderApp.Name;
            //             if (teamInboxObj.IntProviderAppSecret.IntProviderApp.IntProvider != null)
            //             {
            //                 teamInboxDto.ProviderName = teamInboxObj.IntProviderAppSecret.IntProviderApp.IntProvider.Name;
            //             }
            //         }
            //         else
            //         {
            //             var customDomainEmailConfigObj = _customDomainEmailConfigService.GetByUserAndEmail(userId, teamInboxDto.SelectedEmail);
            //             if (customDomainEmailConfigObj != null)
            //             {
            //                 teamInboxDto.IntProviderAppSecretDto.CustomDomainEmailConfigId = customDomainEmailConfigObj.Id;
            //                 teamInboxDto.IntProviderAppSecretDto.CustomDomainEmailConfigDto = _mapper.Map<CustomDomainEmailConfigDto>(customDomainEmailConfigObj);
            //             }
            //         }
            //     }
            //     var teamAccesses = _teamInboxAccessService.GetByTeamInbox(teamInboxObj.Id);
            //     if (teamAccesses.Count() > 0)
            //     {
            //         teamInboxDto.TeamInboxAccesses = _mapper.Map<List<TeamInboxAccessDto>>(teamAccesses);
            //     }
            //     teamInboxDtos.Add(teamInboxDto);
            // }

            // foreach (var teamInboxesAccessObj in teamInboxesAccessList)
            // {
            //     if (teamInboxesAccessObj.TeamInbox != null)
            //     {
            //         TeamInbox? isCreatedByMeObj = teamInboxes.Where(t => t.Id == teamInboxesAccessObj.TeamInboxId).FirstOrDefault();
            //         if (isCreatedByMeObj == null)
            //         {
            //             IntProvider? intProviderObj = null;
            //             IntProviderApp? intProviderAppObj = null;

            //             if (teamInboxesAccessObj.TeamInbox.IntProviderAppSecret != null && teamInboxesAccessObj.TeamInbox.IntProviderAppSecret.IntProviderApp != null)
            //             {
            //                 intProviderAppObj = teamInboxesAccessObj.TeamInbox.IntProviderAppSecret.IntProviderApp;

            //                 if (teamInboxesAccessObj.TeamInbox.IntProviderAppSecret.IntProviderApp.IntProvider != null)
            //                 {
            //                     intProviderObj = teamInboxesAccessObj.TeamInbox.IntProviderAppSecret.IntProviderApp.IntProvider;
            //                 }
            //             }

            //             var teamInboxDto = _mapper.Map<TeamInboxDto>(teamInboxesAccessObj.TeamInbox);
            //             if (teamInboxDto.IntProviderAppSecretId != null)
            //             {
            //                 var intProviderAppSecretDto = _mapper.Map<IntProviderAppSecretDto>(teamInboxesAccessObj.TeamInbox.IntProviderAppSecret);
            //                 teamInboxDto.IntProviderAppSecretDto = intProviderAppSecretDto;
            //                 teamInboxDto.SelectedEmail = intProviderAppSecretDto.Email;
            //                 if (intProviderAppObj != null)
            //                 {
            //                     teamInboxDto.IntProviderAppName = intProviderAppObj.Name;
            //                 }

            //                 if (intProviderObj != null)
            //                 {
            //                     teamInboxDto.ProviderName = intProviderObj.Name;
            //                 }
            //             }
            //             var teamInboxAccess = teamInboxesAccessList.Where(t => t.TeamInboxId == teamInboxesAccessObj.TeamInbox.Id).ToList();
            //             if (teamInboxAccess.Count() > 0)
            //             {
            //                 teamInboxDto.TeamInboxAccesses = _mapper.Map<List<TeamInboxAccessDto>>(teamInboxAccess);
            //                 teamInboxDtos.Add(teamInboxDto);
            //             }
            //         }
            //     }
            // }
            teamInboxResponseList = _mapper.Map<List<TeamInboxResponse>>(teamInboxDtoList);
            return new OperationResult<List<TeamInboxResponse>>(true, System.Net.HttpStatusCode.OK, "", teamInboxResponseList);
        }

        [HttpGet("{TeamInboxId}")]
        public async Task<OperationResult<TeamInboxDto>> TeamInboxDetail(long TeamInboxId)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            TeamInboxDto teamInboxDto = new TeamInboxDto();
            var teamInboxObj = _teamInboxService.GetById(TeamInboxId);

            teamInboxDto = _mapper.Map<TeamInboxDto>(teamInboxObj);
            if (teamInboxObj.IntProviderAppSecret != null)
            {
                teamInboxDto.SelectedEmail = teamInboxObj.IntProviderAppSecret.Email;
            }

            return new OperationResult<TeamInboxDto>(true, System.Net.HttpStatusCode.OK, "", teamInboxDto);
        }

        [HttpPost]
        public async Task<OperationResult<TeamInboxAddUpdateAccessResponse>> TeamInboxAccess([FromBody] TeamInboxAddUpdateAccessRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var responsemodel = _mapper.Map<TeamInboxAddUpdateAccessResponse>(model);
            if (model.Id != null)
            {
                var teamInboxObj = _teamInboxService.GetById(model.Id.Value);
                var teamInboxDto = _mapper.Map<TeamInboxDto>(teamInboxObj);
                teamInboxDto.IsPublic = model.IsPublic;
                var AddUpdateTeamInboxObj = await _teamInboxService.CheckInsertOrUpdate(teamInboxDto);

                if (model.IsPublic == false)
                {
                    foreach (var iteam in model.TeamMateIds)
                    {
                        TeamInboxAccessDto teamInboxAccessDto = new TeamInboxAccessDto();
                        teamInboxAccessDto.TeamInboxId = model.Id;
                        teamInboxAccessDto.TeamMateId = iteam;
                        teamInboxAccessDto.CreatedBy = UserId;
                        var AddUpdateTeamInboxAccessObj = await _teamInboxAccessService.CheckInsertOrUpdate(teamInboxAccessDto);
                    }

                    var teamInboxMateList = _teamInboxAccessService.GetByTeamInbox(model.Id.Value);

                    var existTeamMateIdList = teamInboxMateList.Select(t => t.TeamMateId.Value).ToList();

                    var NotExistTeamMateList = teamInboxMateList.Where(p => !model.TeamMateIds.Contains(p.TeamMateId.Value)).ToList();

                    for (int i = NotExistTeamMateList.Count() - 1; i >= 0; i--)
                    {
                        var NotExistTeamMateObj = NotExistTeamMateList[i];
                        var deletedItem = await _teamInboxAccessService.DeleteTeamInboxAccess(NotExistTeamMateObj.Id);
                    }
                }
                return new OperationResult<TeamInboxAddUpdateAccessResponse>(true, System.Net.HttpStatusCode.OK, "", responsemodel);
            }
            else
            {
                return new OperationResult<TeamInboxAddUpdateAccessResponse>(false, System.Net.HttpStatusCode.OK, "Please provide team inbox id.", responsemodel);
            }
        }

        #endregion

        private async Task<IntProviderAppSecretDto> CheckAccessToken(int userId)
        {
            string Token = "";
            var intProviderAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUser(userId);
            var intProviderAppSecretDto = _mapper.Map<IntProviderAppSecretDto>(intProviderAppSecretObj);
            if (intProviderAppSecretObj != null)
            {

                var tokenInfo = await _calendarService.GetTokenInfo(intProviderAppSecretObj.Access_Token, GoogleCalendarSecret);
                if ((tokenInfo == null) || (tokenInfo != null && (!string.IsNullOrEmpty(tokenInfo.error_description))))
                {
                    GoogleCalendarTokenVM googleCalendarTokenVMObj = new GoogleCalendarTokenVM();
                    googleCalendarTokenVMObj.refresh_token = intProviderAppSecretObj.Refresh_Token;
                    googleCalendarTokenVMObj.client_id = GoogleCalendarClientId;
                    googleCalendarTokenVMObj.client_secret = GoogleCalendarSecret;
                    googleCalendarTokenVMObj.grant_type = "refresh_token";
                    googleCalendarTokenVMObj.scope = GoogleCalendarScope;
                    var accessTokenObj = await _calendarService.GetRefreshToken(GoogleCalendarSecret, googleCalendarTokenVMObj);
                    if (string.IsNullOrEmpty(accessTokenObj.error_description))
                    {
                        Token = "Bearer " + accessTokenObj.access_token;
                        intProviderAppSecretDto.Access_Token = accessTokenObj.access_token;
                        intProviderAppSecretDto.Id_Token = accessTokenObj.id_token;
                        intProviderAppSecretDto.LastAccessedOn = DateTime.UtcNow;
                        intProviderAppSecretDto.Token = Token;
                    }
                    else
                    {
                        intProviderAppSecretDto.error_description = accessTokenObj.error_description;
                        return intProviderAppSecretDto;
                    }

                }
                else
                {
                    Token = "Bearer " + intProviderAppSecretObj.Access_Token;
                    intProviderAppSecretDto.Token = Token;
                    intProviderAppSecretDto.LastAccessedOn = DateTime.UtcNow;
                }
                var AddUpdate = await _intProviderAppSecretService.CheckInsertOrUpdate(intProviderAppSecretDto);
            }
            return intProviderAppSecretDto;
            // return Token;
        }

        [HttpGet]
        public async Task<OperationResult<List<CustomEmailFolder>>> GetMessages()
        {
            var customEmailFolderList = await customMailbox.GetThread();
            // var list = JsonSerializer.Deserialize<List<dynamic>>(data);
            return new OperationResult<List<CustomEmailFolder>>(true, System.Net.HttpStatusCode.OK, "", customEmailFolderList);
        }

        [Authorize]
        [HttpPost]
        public async Task<OperationResult<List<CustomEmailFolderResponse>>> Folders([FromBody] CustomEmailFolderRequest model)
        {
            List<CustomEmailFolder> customEmailFolderList = new List<CustomEmailFolder>();
            List<CustomEmailFolderResponse> customEmailFolderResponseList = new List<CustomEmailFolderResponse>();
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var requestmodel = _mapper.Map<MailTokenDto>(model);
            if (UserId > 0 && !string.IsNullOrEmpty(requestmodel.SelectedEmail))
            {
                customEmailFolderList = await mailInbox.GetFolders(requestmodel, UserId);
                customEmailFolderResponseList = _mapper.Map<List<CustomEmailFolderResponse>>(customEmailFolderList);
                return new OperationResult<List<CustomEmailFolderResponse>>(true, System.Net.HttpStatusCode.OK, "", customEmailFolderResponseList);
            }
            customEmailFolderResponseList = _mapper.Map<List<CustomEmailFolderResponse>>(customEmailFolderList);
            return new OperationResult<List<CustomEmailFolderResponse>>(false, System.Net.HttpStatusCode.OK, CommonMessage.DefaultErrorMessage, customEmailFolderResponseList);
        }

    }

}