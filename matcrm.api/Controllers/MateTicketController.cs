using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.api.SignalR;
using matcrm.data.Context;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;
using matcrm.data.Models.Tables;
using matcrm.service.BusinessLogic;
using matcrm.service.Common;
using matcrm.service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class MateTicketController : Controller
    {
        private readonly IMateTicketService _mateTicketService;
        private readonly IMateTicketActivityService _mateTicketActivityService;
        private readonly IMateTicketUserService _mateTicketUserService;
        private readonly IUserService _userService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEmailLogService _emailLogService;
        private readonly IEmailConfigService _emailConfigService;
        private readonly IEmailProviderService _emailProviderService;
        private readonly IMateClientTicketService _mateClientTicketService;
        private readonly IMateProjectTicketService _mateProjectTicketService;
        private readonly IStatusService _statusService;
        private readonly IEmployeeProjectService _employeeProjectService;
        private readonly IClientService _clientService;
        private readonly IMateTicketTimeRecordService _mateTicketTimeRecordService;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private readonly IMateTicketTaskService _mateTicketTaskService;
        private readonly IEmployeeSubTaskService _employeeSubTaskService;
        private readonly IEmployeeChildTaskService _employeeChildTaskService;
        private readonly IEmployeeChildTaskAttachmentService _employeeChildTaskAttachmentService;
        private readonly IEmployeeChildTaskCommentService _employeeChildTaskCommentService;
        private readonly IEmployeeChildTaskTimeRecordService _employeeChildTaskTimeRecordService;
        private readonly IEmployeeChildTaskUserService _employeeChildTaskUserService;
        private readonly IEmployeeChildTaskActivityService _employeeChildTaskActivityService;
        private readonly IEmployeeSubTaskActivityService _employeeSubTaskActivityService;
        private readonly IEmployeeSubTaskAttachmentService _employeeSubTaskAttachmentService;
        private readonly IEmployeeSubTaskCommentService _employeeSubTaskCommentService;
        private readonly IEmployeeSubTaskTimeRecordService _employeeSubTaskTimeRecordService;
        private readonly IEmployeeSubTaskUserService _employeeSubTaskUserService;
        private readonly IEmployeeTaskUserSerivce _employeeTaskUserService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IEmployeeTaskAttachmentService _employeeTaskAttachmentService;
        private readonly IEmployeeTaskCommentService _employeeTaskCommentService;
        private readonly IEmployeeTaskTimeRecordService _employeeTaskTimeRecordService;
        private readonly IEmployeeProjectTaskService _employeeProjectTaskService;
        private readonly IEmployeeClientTaskService _employeeClientTaskService;
        private readonly IEmployeeTaskActivityService _employeeTaskActivityService;
        private readonly IEmployeeTaskService _employeeTaskService;
        private readonly IMateTimeRecordService _mateTimeRecordService;
        private readonly IMateTicketCommentService _mateTicketCommentService;
        private readonly IMateCommentAttachmentService _mateCommentAttachmentService;
        private readonly IMateCommentService _mateCommentService;
        private IMapper _mapper;
        private SendEmail sendEmail;
        private int UserId = 0;
        private int TenantId = 0;
        public MateTicketController(IMateTicketService mateTicketService,
        IMateTicketActivityService mateTicketActivityService,
        IMateTicketUserService mateTicketUserService,
        IEmailTemplateService emailTemplateService,
        IEmailLogService emailLogService,
        IUserService userService,
        IEmailConfigService emailConfigService,
        IEmailProviderService emailProviderService,
        IMateClientTicketService mateClientTicketService,
        IMateProjectTicketService mateProjectTicketService,
        IStatusService statusService,
        IEmployeeProjectService employeeProjectService,
        IClientService clientService,
        IMateTicketTimeRecordService mateTicketTimeRecordService,
        IHubContext<BroadcastHub, IHubClient> hubContext,
        IMateTicketTaskService mateTicketTaskService,
        IEmployeeSubTaskService employeeSubTaskService,
        IEmployeeChildTaskService employeeChildTaskService,
        IEmployeeChildTaskAttachmentService employeeChildTaskAttachmentService,
        IEmployeeChildTaskCommentService employeeChildTaskCommentService,
        IEmployeeChildTaskTimeRecordService employeeChildTaskTimeRecordService,
        IEmployeeChildTaskUserService employeeChildTaskUserService,
        IEmployeeChildTaskActivityService employeeChildTaskActivityService,
        IEmployeeSubTaskActivityService employeeSubTaskActivityService,
        IEmployeeSubTaskAttachmentService employeeSubTaskAttachmentService,
        IEmployeeSubTaskCommentService employeeSubTaskCommentService,
        IEmployeeSubTaskTimeRecordService employeeSubTaskTimeRecordService,
        IEmployeeSubTaskUserService employeeSubTaskUserService,
        IEmployeeTaskUserSerivce employeeTaskUserService,
        IHostingEnvironment hostingEnvironment,
        IEmployeeTaskAttachmentService employeeTaskAttachmentService,
        IEmployeeTaskCommentService employeeTaskCommentService,
        IEmployeeTaskTimeRecordService employeeTaskTimeRecordService,
        IEmployeeProjectTaskService employeeProjectTaskService,
        IEmployeeClientTaskService employeeClientTaskService,
        IEmployeeTaskActivityService employeeTaskActivityService,
        IEmployeeTaskService employeeTaskService,
        IMateTimeRecordService mateTimeRecordService,
        IMateTicketCommentService mateTicketCommentService,
        IMateCommentAttachmentService mateCommentAttachmentService,
        IMateCommentService mateCommentService,
        IMapper mapper)
        {
            _mateTicketService = mateTicketService;
            _mateTicketActivityService = mateTicketActivityService;
            _mateTicketUserService = mateTicketUserService;
            _emailTemplateService = emailTemplateService;
            _emailLogService = emailLogService;
            _userService = userService;
            _emailConfigService = emailConfigService;
            _emailProviderService = emailProviderService;
            _mateClientTicketService = mateClientTicketService;
            _mateProjectTicketService = mateProjectTicketService;
            _statusService = statusService;
            _employeeProjectService = employeeProjectService;
            _clientService = clientService;
            _mateTicketTimeRecordService = mateTicketTimeRecordService;
            _mapper = mapper;
            _hubContext = hubContext;
            _mateTicketTaskService = mateTicketTaskService;
            _employeeSubTaskService = employeeSubTaskService;
            _employeeChildTaskService = employeeChildTaskService;
            _employeeChildTaskAttachmentService = employeeChildTaskAttachmentService;
            _employeeChildTaskCommentService = employeeChildTaskCommentService;
            _employeeChildTaskTimeRecordService = employeeChildTaskTimeRecordService;
            _employeeChildTaskUserService = employeeChildTaskUserService;
            _employeeChildTaskActivityService = employeeChildTaskActivityService;
            _employeeSubTaskActivityService = employeeSubTaskActivityService;
            _employeeSubTaskAttachmentService = employeeSubTaskAttachmentService;
            _employeeSubTaskCommentService = employeeSubTaskCommentService;
            _employeeSubTaskTimeRecordService = employeeSubTaskTimeRecordService;
            _employeeSubTaskUserService = employeeSubTaskUserService;
            _employeeTaskUserService = employeeTaskUserService;
            _hostingEnvironment = hostingEnvironment;
            _employeeTaskAttachmentService = employeeTaskAttachmentService;
            _employeeTaskCommentService = employeeTaskCommentService;
            _employeeTaskTimeRecordService = employeeTaskTimeRecordService;
            _employeeProjectTaskService = employeeProjectTaskService;
            _employeeClientTaskService = employeeClientTaskService;
            _employeeTaskActivityService = employeeTaskActivityService;
            _employeeTaskService = employeeTaskService;
            _mateTimeRecordService = mateTimeRecordService;
            _mateTicketCommentService = mateTicketCommentService;
            _mateCommentAttachmentService = mateCommentAttachmentService;
            _mateCommentService = mateCommentService;
            sendEmail = new SendEmail(emailTemplateService, emailLogService, emailConfigService, emailProviderService, mapper);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<MateTicketAddUpdateResponse>> Add([FromBody] MateTicketAddUpdateRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            // if (requestmodel.ClientId != null)
            // {
            var model = _mapper.Map<MateTicket>(requestmodel);
            if (model.Id == 0)
            {
                model.CreatedBy = UserId;
            }
            Random generator = new Random();
            String r = generator.Next(0, 1000000).ToString("D6");
            model.TicketNo = "#" + r;
            var mateTicketObj = await _mateTicketService.CheckInsertOrUpdate(model);
            if (mateTicketObj != null)
            {
                MateTicketActivity mateTicketActivityObj = new MateTicketActivity();
                mateTicketActivityObj.MateTicketId = mateTicketObj.Id;
                mateTicketActivityObj.CreatedBy = UserId;
                mateTicketActivityObj.Activity = Enums.MateTicketActivityEnum.Ticket_Created.ToString().Replace("_", " ");
                var AddUpdate = await _mateTicketActivityService.CheckInsertOrUpdate(mateTicketActivityObj);

                if (requestmodel.ClientId != null)
                {
                    MateClientTicket mateClientTicketObj = new MateClientTicket();
                    mateClientTicketObj.MateTicketId = mateTicketObj.Id;
                    mateClientTicketObj.ClientId = requestmodel.ClientId;
                    var AddUpdateClientTicket = await _mateClientTicketService.CheckInsertOrUpdate(mateClientTicketObj);
                    if (AddUpdateClientTicket != null)
                    {
                        MateTicketActivity clientTicketActivityObj = new MateTicketActivity();
                        clientTicketActivityObj.MateTicketId = mateTicketObj.Id;
                        clientTicketActivityObj.CreatedBy = UserId;
                        clientTicketActivityObj.Activity = Enums.MateTicketActivityEnum.Ticket_assign_to_Client.ToString().Replace("_", " ");
                        var AddClientTicketActivity = await _mateTicketActivityService.CheckInsertOrUpdate(clientTicketActivityObj);
                    }
                }

                if (requestmodel.EmployeeProjectId != null)
                {
                    MateProjectTicket mateProjectTicketObj = new MateProjectTicket();
                    mateProjectTicketObj.MateTicketId = mateTicketObj.Id;
                    mateProjectTicketObj.EmployeeProjectId = requestmodel.EmployeeProjectId;
                    var AddUpdateProjectTicket = await _mateProjectTicketService.CheckInsertOrUpdate(mateProjectTicketObj);
                    if (AddUpdateProjectTicket != null)
                    {
                        MateTicketActivity ProjectTicketActivityObj = new MateTicketActivity();
                        ProjectTicketActivityObj.MateTicketId = mateTicketObj.Id;
                        ProjectTicketActivityObj.CreatedBy = UserId;
                        ProjectTicketActivityObj.EmployeeProjectId = requestmodel.EmployeeProjectId;
                        ProjectTicketActivityObj.Activity = Enums.MateTicketActivityEnum.Ticket_assign_to_project.ToString().Replace("_", " ");
                        var AddProjectTicketActivity = await _mateTicketActivityService.CheckInsertOrUpdate(ProjectTicketActivityObj);
                    }
                }

                if (requestmodel.AssignedUsers != null && requestmodel.AssignedUsers.Count() > 0)
                {
                    foreach (var userObj in requestmodel.AssignedUsers)
                    {
                        MateTicketUser mateTicketUserObj = new MateTicketUser();
                        mateTicketUserObj.MateTicketId = mateTicketObj.Id;
                        mateTicketUserObj.UserId = userObj.UserId;
                        mateTicketUserObj.CreatedBy = UserId;
                        var isExist = _mateTicketUserService.IsExistOrNot(mateTicketUserObj);
                        var AddUpdateMateTicketUser = await _mateTicketUserService.CheckInsertOrUpdate(mateTicketUserObj);
                        if (AddUpdateMateTicketUser != null)
                        {
                            userObj.Id = AddUpdateMateTicketUser.Id;
                        }
                        if (!isExist)
                        {
                            if (mateTicketUserObj.UserId != null)
                            {
                                var userAssignDetails = _userService.GetUserById(mateTicketUserObj.UserId.Value);
                                if (userAssignDetails != null)
                                    await sendEmail.SendEmailMateTicketUserAssignNotification(userAssignDetails.Email, userAssignDetails.FirstName + ' ' + userAssignDetails.LastName, requestmodel.Description, TenantId, mateTicketObj.Id);
                                MateTicketActivity ticketUserActivityObj = new MateTicketActivity();
                                ticketUserActivityObj.MateTicketId = mateTicketObj.Id;
                                ticketUserActivityObj.CreatedBy = UserId;
                                ticketUserActivityObj.Activity = Enums.MateTicketActivityEnum.Ticket_assigned_to_user.ToString().Replace("_", " ");
                                var AddUpdateTicketUserActivity = await _mateTicketActivityService.CheckInsertOrUpdate(ticketUserActivityObj);
                            }
                        }
                    }
                }
                await _hubContext.Clients.All.OnMateTicketModuleEvent(mateTicketObj.Id, TenantId);
            }
            var responseObj = _mapper.Map<MateTicketAddUpdateResponse>(mateTicketObj);

            return new OperationResult<MateTicketAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Ticket added successfully", responseObj);
            // }
            // else
            // {
            //     return new OperationResult<MateTicketAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Please provide clientId");
            // }
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<MateTicketAddUpdateResponse>> Update([FromBody] MateTicketAddUpdateRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            // if (requestmodel.ClientId != null)
            // {
            var model = _mapper.Map<MateTicket>(requestmodel);
            if (model.Id > 0)
            {
                model.UpdatedBy = UserId;
            }
            Random generator = new Random();
            String r = generator.Next(0, 1000000).ToString("D6");
            model.TicketNo = "#" + r;
            var mateTicketObj = await _mateTicketService.CheckInsertOrUpdate(model);
            if (mateTicketObj != null)
            {
                MateTicketActivity mateTicketActivityObj = new MateTicketActivity();
                mateTicketActivityObj.MateTicketId = mateTicketObj.Id;
                mateTicketActivityObj.CreatedBy = UserId;
                mateTicketActivityObj.Activity = Enums.MateTicketActivityEnum.Ticket_Updated.ToString().Replace("_", " ");
                var AddUpdate = await _mateTicketActivityService.CheckInsertOrUpdate(mateTicketActivityObj);

                //MateClientTicket
                if (requestmodel.ClientId != null)
                {
                    MateClientTicket mateClientTicketObj = new MateClientTicket();
                    mateClientTicketObj.MateTicketId = mateTicketObj.Id;
                    mateClientTicketObj.ClientId = requestmodel.ClientId;
                    var AddUpdateClientTicket = await _mateClientTicketService.CheckInsertOrUpdate(mateClientTicketObj);
                    if (AddUpdateClientTicket != null)
                    {
                        MateTicketActivity clientTicketActivityObj = new MateTicketActivity();
                        clientTicketActivityObj.MateTicketId = mateTicketObj.Id;
                        clientTicketActivityObj.CreatedBy = UserId;
                        clientTicketActivityObj.Activity = Enums.MateTicketActivityEnum.Ticket_assign_to_Client.ToString().Replace("_", " ");
                        var AddClientTicketActivity = await _mateTicketActivityService.CheckInsertOrUpdate(clientTicketActivityObj);
                    }
                }

                //MateProjectTicket
                if (requestmodel.EmployeeProjectId != null)
                {
                    MateProjectTicket mateProjectTicketObj = new MateProjectTicket();
                    mateProjectTicketObj.MateTicketId = mateTicketObj.Id;
                    mateProjectTicketObj.EmployeeProjectId = requestmodel.EmployeeProjectId;
                    var AddUpdateProjectTicket = await _mateProjectTicketService.CheckInsertOrUpdate(mateProjectTicketObj);
                    if (AddUpdateProjectTicket != null)
                    {
                        MateTicketActivity ProjectTicketActivityObj = new MateTicketActivity();
                        ProjectTicketActivityObj.MateTicketId = mateTicketObj.Id;
                        ProjectTicketActivityObj.CreatedBy = UserId;
                        ProjectTicketActivityObj.EmployeeProjectId = requestmodel.EmployeeProjectId;
                        ProjectTicketActivityObj.Activity = Enums.MateTicketActivityEnum.Ticket_assign_to_project.ToString().Replace("_", " ");
                        var AddProjectTicketActivity = await _mateTicketActivityService.CheckInsertOrUpdate(ProjectTicketActivityObj);
                    }
                }

                if (requestmodel.AssignedUsers != null && requestmodel.AssignedUsers.Count() > 0)
                {
                    foreach (var userObj in requestmodel.AssignedUsers)
                    {
                        MateTicketUser mateTicketUserObj = new MateTicketUser();
                        mateTicketUserObj.MateTicketId = mateTicketObj.Id;
                        mateTicketUserObj.UserId = userObj.UserId;
                        mateTicketUserObj.CreatedBy = UserId;
                        var isExist = _mateTicketUserService.IsExistOrNot(mateTicketUserObj);
                        var AddUpdateMateTicketUser = await _mateTicketUserService.CheckInsertOrUpdate(mateTicketUserObj);
                        if (AddUpdateMateTicketUser != null)
                        {
                            userObj.Id = AddUpdateMateTicketUser.Id;
                        }
                        if (!isExist)
                        {
                            if (mateTicketUserObj.UserId != null)
                            {
                                var userAssignDetails = _userService.GetUserById(mateTicketUserObj.UserId.Value);
                                if (userAssignDetails != null)
                                    await sendEmail.SendEmailMateTicketUserAssignNotification(userAssignDetails.Email, userAssignDetails.FirstName + ' ' + userAssignDetails.LastName, requestmodel.Description, TenantId, mateTicketObj.Id);
                                MateTicketActivity ticketUserActivityObj = new MateTicketActivity();
                                ticketUserActivityObj.MateTicketId = mateTicketObj.Id;
                                ticketUserActivityObj.CreatedBy = UserId;
                                ticketUserActivityObj.Activity = Enums.MateTicketActivityEnum.Ticket_assigned_to_user.ToString().Replace("_", " ");
                                var AddUpdateTicketUserActivity = await _mateTicketActivityService.CheckInsertOrUpdate(ticketUserActivityObj);
                            }
                        }
                    }
                }
                await _hubContext.Clients.All.OnMateTicketModuleEvent(mateTicketObj.Id, TenantId);
            }
            var responseObj = _mapper.Map<MateTicketAddUpdateResponse>(mateTicketObj);
            return new OperationResult<MateTicketAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Ticket updated successfully", responseObj);
            // }
            // else
            // {
            //     return new OperationResult<MateTicketAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Please provide clientId");
            // }
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        [SwaggerOperation(Description = "This api is use for Ticket Tab in left panel")]
        public async Task<OperationResult<MateTicketGroupListResponse>> GroupTicketList([FromBody] MateTicketGroupListRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            MateTicketGroupListResponse ticketResponseObj = new MateTicketGroupListResponse();

            var SkipValue = requestModel.PageSize * (requestModel.PageNumber - 1);
            var ticketList = _mateTicketService.GetAllByTenantId(TenantId);
            //ticket by status
            if (ticketList != null && ticketList.Count > 0)
            {
                List<MateTicketStatusListResponse> statusTicketList = new List<MateTicketStatusListResponse>();
                var AllStatus = _statusService.GetByTenant(TenantId);

                statusTicketList.Add(new MateTicketStatusListResponse { StatusId = null, StatusName = "All Tickets", StatusColor = "Yellow", TotalCount = ticketList.Count() });

                var TicketKeyValueData = ticketList.GroupBy(t => t?.StatusId);
                foreach (var TicketData in TicketKeyValueData)
                {
                    MateTicketStatusListResponse statusTicketObj = new MateTicketStatusListResponse();

                    statusTicketObj.TotalCount = TicketData.ToList().Count();
                    if (TicketData.Key != null)
                    {
                        var StatusObj = AllStatus.Where(t => t.Id == TicketData.Key).FirstOrDefault();
                        if (StatusObj != null)
                        {
                            statusTicketObj.StatusId = StatusObj.Id;
                            statusTicketObj.StatusName = StatusObj.Name;
                            statusTicketObj.StatusColor = StatusObj.Color;
                        }
                    }
                    statusTicketList.Add(statusTicketObj);
                }
                if (string.IsNullOrEmpty(requestModel.Type))
                {
                    if (!string.IsNullOrEmpty(requestModel.SearchString))
                    {
                        statusTicketList = statusTicketList.Where(t => t.StatusName != null && t.StatusName.ToLower().Contains(requestModel.SearchString.ToLower())).ToList();
                        statusTicketList = statusTicketList.Skip(SkipValue).Take(requestModel.PageSize).OrderBy(t => t.StatusId).ToList();
                    }
                    else
                    {
                        statusTicketList = statusTicketList.Skip(SkipValue).Take(requestModel.PageSize).OrderBy(t => t.StatusId).ToList();
                    }
                }
                ticketResponseObj.StatusList = statusTicketList;
            }

            //ticket by project
            var mateProjectTicketList = _mateProjectTicketService.GetAllByTenantId(TenantId);
            if (mateProjectTicketList != null && mateProjectTicketList.Count > 0)
            {
                List<MateProjectTicketGroupTicketListResponse> projectTicketList = new List<MateProjectTicketGroupTicketListResponse>();
                var projectList = _employeeProjectService.GetAllByTenant(TenantId);
                var ProjectTicketKeyValueData = mateProjectTicketList.GroupBy(t => t?.EmployeeProjectId);
                foreach (var TicketData1 in ProjectTicketKeyValueData)
                {
                    MateProjectTicketGroupTicketListResponse projectTicketObj = new MateProjectTicketGroupTicketListResponse();

                    if (TicketData1.Key != null)
                    {
                        var projectObj = projectList.Where(t => t.Id == TicketData1.Key).FirstOrDefault();
                        if (projectObj != null)
                        {
                            projectTicketObj.TotalCount = TicketData1.ToList().Count();
                            projectTicketObj.ProjectId = projectObj.Id;
                            projectTicketObj.ProjectName = projectObj.Name;
                            if (projectObj.Logo != null)
                            {
                                var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                                projectTicketObj.ProjectLogoURL = OneClappContext.CurrentURL + "Project/Logo/" + projectObj.Id + "?" + Timestamp;
                            }
                        }
                    }
                    projectTicketList.Add(projectTicketObj);
                }
                if (string.IsNullOrEmpty(requestModel.Type))
                {
                    if (!string.IsNullOrEmpty(requestModel.SearchString))
                    {
                        projectTicketList = projectTicketList.Where(t => t.ProjectName.ToLower().Contains(requestModel.SearchString.ToLower())).ToList();
                        projectTicketList = projectTicketList.Skip(SkipValue).Take(requestModel.PageSize).ToList();
                    }
                    else
                    {
                        projectTicketList = projectTicketList.Skip(SkipValue).Take(requestModel.PageSize).ToList();
                    }
                }
                ticketResponseObj.ProjectTicketList = projectTicketList;
            }
            if (ticketResponseObj != null)
            {
                if (!string.IsNullOrEmpty(requestModel.Type))
                {
                    if (requestModel.Type.ToLower() == "project")
                    {
                        if (!string.IsNullOrEmpty(requestModel.SearchString))
                        {
                            ticketResponseObj.ProjectTicketList = ticketResponseObj.ProjectTicketList.Where(t => t.ProjectName.ToLower().Contains(requestModel.SearchString.ToLower())).ToList();
                            ticketResponseObj.ProjectTicketList = ticketResponseObj.ProjectTicketList.Skip(SkipValue).Take(requestModel.PageSize).ToList();
                        }
                        else
                        {
                            ticketResponseObj.ProjectTicketList = ticketResponseObj.ProjectTicketList.Skip(SkipValue).Take(requestModel.PageSize).ToList();
                        }
                    }
                    if (requestModel.Type.ToLower() == "status")
                    {
                        if (!string.IsNullOrEmpty(requestModel.SearchString))
                        {
                            ticketResponseObj.StatusList = ticketResponseObj.StatusList.Where(t => t.StatusName != null && t.StatusName.ToLower().Contains(requestModel.SearchString.ToLower())).ToList();
                            ticketResponseObj.StatusList = ticketResponseObj.StatusList.Skip(SkipValue).Take(requestModel.PageSize).OrderBy(t => t.StatusId).ToList();
                        }
                        else
                        {
                            ticketResponseObj.StatusList = ticketResponseObj.StatusList.Skip(SkipValue).Take(requestModel.PageSize).OrderBy(t => t.StatusId).ToList();
                        }
                    }
                }
            }
            return new OperationResult<MateTicketGroupListResponse>(true, System.Net.HttpStatusCode.OK, "", ticketResponseObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        [SwaggerOperation(Description = "This api is use for Ticket list when click on left panel ticket list")]
        public async Task<OperationResult<List<MateTicketListResponse>>> List([FromBody] MateTicketListRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            int totalCount = 0;
            List<MateTicketListResponse> ticketResponseList = new List<MateTicketListResponse>();
            var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            var AllStatus = _statusService.GetByTenant(TenantId);
            //start for project ticket
            if (requestModel.ProjectId != null)
            {
                var ticketList = _mateProjectTicketService.GetAllByProjectId(requestModel.ProjectId.Value, TenantId);
                totalCount = ticketList.Count();
                if (ticketList != null && ticketList.Count() > 0)
                {
                    foreach (var item in ticketList)
                    {
                        MateTicketListResponse ticketListObj = new MateTicketListResponse();

                        if (item.MateTicket != null)
                        {
                            if (item.MateTicketId != null)
                            {
                                ticketListObj.Id = item.MateTicketId.Value;
                                var mateTicketTaskList = _mateTicketTaskService.GetAllByTicketId(item.MateTicketId.Value);
                                ticketListObj.TaskCount = mateTicketTaskList.Count();
                            }
                            ticketListObj.TicketNo = item.MateTicket.TicketNo;
                            ticketListObj.Name = item.MateTicket.Name;
                            if (item.EmployeeProjectId != null)
                            {
                                ticketListObj.ProjectId = item.EmployeeProjectId;
                                ticketListObj.ProjectName = item.EmployeeProject?.Name;
                                if (item.EmployeeProject?.Logo != null)
                                {
                                    ticketListObj.ProjectLogoURL = OneClappContext.CurrentURL + "Project/Logo/" + item.EmployeeProjectId + "?" + Timestamp;
                                }
                            }
                            if (item.MateTicket.StatusId != null)
                            {
                                var statusObj = AllStatus.Where(t => t.Id == item.MateTicket?.StatusId).FirstOrDefault();
                                if (statusObj != null)
                                {
                                    ticketListObj.StatusId = statusObj.Id;
                                    ticketListObj.StatusName = statusObj.Name;
                                    ticketListObj.StatusColor = statusObj.Color;
                                }
                            }
                            var mateClientTicketObj = _mateClientTicketService.GetByTicketId(ticketListObj.Id);
                            if (mateClientTicketObj != null && mateClientTicketObj.ClientId != null)
                            {
                                var clientObj = _clientService.GetById(mateClientTicketObj.ClientId.Value);
                                if (clientObj != null)
                                {
                                    ticketListObj.ClientId = clientObj.Id;
                                    ticketListObj.ClientName = clientObj.FirstName + " " + clientObj.LastName;
                                }
                            }
                            ticketListObj.Date = item.MateTicket?.CreatedOn;
                            ticketResponseList.Add(ticketListObj);
                        }
                    }
                }
            }
            //end for project ticket
            //start for status wise ticket
            if (requestModel.StatusId != null)
            {
                List<MateTicket> statusTicketList = new List<MateTicket>();
                statusTicketList = _mateTicketService.GetAllByStatusId(TenantId, requestModel.StatusId.Value);
                totalCount = statusTicketList.Count();
                if (statusTicketList != null && statusTicketList.Count() > 0)
                {
                    foreach (var statusItem in statusTicketList)
                    {
                        MateTicketListResponse statusTicketListObj = new MateTicketListResponse();
                        var mateTicketTaskList = _mateTicketTaskService.GetAllByTicketId(statusItem.Id);
                        statusTicketListObj.TaskCount = mateTicketTaskList.Count();
                        statusTicketListObj.Id = statusItem.Id;
                        statusTicketListObj.TicketNo = statusItem.TicketNo;
                        statusTicketListObj.Name = statusItem.Name;
                        var projectTicketObj = _mateProjectTicketService.GetByTicketId(statusItem.Id);
                        if (projectTicketObj != null)
                        {
                            statusTicketListObj.ProjectId = projectTicketObj.EmployeeProjectId;
                            statusTicketListObj.ProjectName = projectTicketObj.EmployeeProject?.Name;
                            if (projectTicketObj.EmployeeProject?.Logo != null)
                            {
                                statusTicketListObj.ProjectLogoURL = OneClappContext.CurrentURL + "Project/Logo/" + projectTicketObj.EmployeeProjectId + "?" + Timestamp;
                            }
                        }
                        if (statusItem.StatusId != null)
                        {
                            var statusObj = AllStatus.Where(t => t.Id == statusItem.StatusId).FirstOrDefault();
                            if (statusObj != null)
                            {
                                statusTicketListObj.StatusId = statusObj.Id;
                                statusTicketListObj.StatusName = statusObj.Name;
                                statusTicketListObj.StatusColor = statusObj.Color;
                            }
                        }
                        var mateClientTicketObj = _mateClientTicketService.GetByTicketId(statusItem.Id);
                        if (mateClientTicketObj != null && mateClientTicketObj.ClientId != null)
                        {
                            var clientObj = _clientService.GetById(mateClientTicketObj.ClientId.Value);
                            if (clientObj != null)
                            {
                                statusTicketListObj.ClientId = clientObj.Id;
                                statusTicketListObj.ClientName = clientObj.FirstName + " " + clientObj.LastName;
                            }
                        }
                        statusTicketListObj.Date = statusItem.CreatedOn;
                        ticketResponseList.Add(statusTicketListObj);
                    }
                }
            }
            //end for status wise ticket
            //start for All ticket
            if (requestModel.StatusId == null && requestModel.ProjectId == null)
            {
                List<MateTicket> ticketList = new List<MateTicket>();
                ticketList = _mateTicketService.GetAllByTenantId(TenantId);
                totalCount = ticketList.Count();
                if (ticketList != null && ticketList.Count() > 0)
                {
                    foreach (var item in ticketList)
                    {
                        MateTicketListResponse ticketListObj = new MateTicketListResponse();

                        var mateTicketTaskList = _mateTicketTaskService.GetAllByTicketId(item.Id);
                        ticketListObj.TaskCount = mateTicketTaskList.Count();
                        ticketListObj.Id = item.Id;
                        ticketListObj.TicketNo = item.TicketNo;
                        ticketListObj.Name = item.Name;
                        var projectTicketObj = _mateProjectTicketService.GetByTicketId(item.Id);
                        if (projectTicketObj != null)
                        {
                            ticketListObj.ProjectId = projectTicketObj.EmployeeProjectId;
                            ticketListObj.ProjectName = projectTicketObj.EmployeeProject?.Name;
                            if (projectTicketObj.EmployeeProject?.Logo != null)
                            {
                                ticketListObj.ProjectLogoURL = OneClappContext.CurrentURL + "Project/Logo/" + projectTicketObj.EmployeeProjectId + "?" + Timestamp;
                            }
                        }
                        if (item.StatusId != null)
                        {
                            var statusObj = AllStatus.Where(t => t.Id == item.StatusId).FirstOrDefault();
                            if (statusObj != null)
                            {
                                ticketListObj.StatusId = statusObj.Id;
                                ticketListObj.StatusName = statusObj.Name;
                                ticketListObj.StatusColor = statusObj.Color;
                            }
                        }
                        var mateClientTicketObj = _mateClientTicketService.GetByTicketId(item.Id);
                        if (mateClientTicketObj != null && mateClientTicketObj.ClientId != null)
                        {
                            var clientObj = _clientService.GetById(mateClientTicketObj.ClientId.Value);
                            if (clientObj != null)
                            {
                                ticketListObj.ClientId = clientObj.Id;
                                ticketListObj.ClientName = clientObj.FirstName + " " + clientObj.LastName;
                            }
                        }
                        ticketListObj.Date = item.CreatedOn;
                        ticketResponseList.Add(ticketListObj);
                    }
                }
            }
            //end for All ticket
            var SkipValue = requestModel.PageSize * (requestModel.PageNumber - 1);
            if (!string.IsNullOrEmpty(requestModel.SearchString))
            {
                ticketResponseList = ticketResponseList.Where(t => (!string.IsNullOrEmpty(t.Name) && t.Name.ToLower().Contains(requestModel.SearchString.ToLower()))
                                                                || (!string.IsNullOrEmpty(t.ProjectName) && t.ProjectName.ToLower().Contains(requestModel.SearchString.ToLower()))
                                                                || (!string.IsNullOrEmpty(t.StatusName) && t.StatusName.ToLower().Contains(requestModel.SearchString.ToLower()))
                                                                || (!string.IsNullOrEmpty(t.ClientName) && t.ClientName.ToLower().Contains(requestModel.SearchString.ToLower()))).ToList();
                ticketResponseList = ticketResponseList.Skip(SkipValue).Take(requestModel.PageSize).ToList();
            }
            else
            {
                ticketResponseList = ticketResponseList.Skip(SkipValue).Take(requestModel.PageSize).ToList();
            }
            ticketResponseList = ShortTaskByColumn(requestModel.ShortColumnName, requestModel.SortType, ticketResponseList);
            return new OperationResult<List<MateTicketListResponse>>(true, System.Net.HttpStatusCode.OK, "", ticketResponseList, totalCount);
        }
        private List<MateTicketListResponse> ShortTaskByColumn(string ShortColumn, string ShortOrder, List<MateTicketListResponse> ticketList)
        {
            List<MateTicketListResponse> ticketVMList = new List<MateTicketListResponse>();
            ticketVMList = ticketList;
            if (ShortColumn != "" && ShortColumn != null)
            {
                if (ShortColumn.ToLower() == "name")
                {
                    if (ShortOrder.ToLower() == "asc")
                    {
                        ticketVMList = ticketList.OrderBy(t => t.Name).ToList();
                    }
                    else
                    {
                        ticketVMList = ticketList.OrderByDescending(t => t.Name).ToList();
                    }
                }
                if (ShortColumn.ToLower() == "projectname")
                {
                    if (ShortOrder.ToLower() == "asc")
                    {
                        ticketVMList = ticketList.OrderBy(t => t.ProjectName).ToList();
                    }
                    else
                    {
                        ticketVMList = ticketList.OrderByDescending(t => t.ProjectName).ToList();
                    }
                }
                else if (ShortColumn.ToLower() == "date")
                {
                    if (ShortOrder.ToLower() == "asc")
                    {
                        ticketVMList = ticketList.OrderBy(t => t.Date).ToList();
                    }
                    else
                    {
                        ticketVMList = ticketList.OrderByDescending(t => t.Date).ToList();
                    }
                }
                else if (ShortColumn.ToLower() == "statusname")
                {
                    if (ShortOrder.ToLower() == "asc")
                    {
                        ticketVMList = ticketList.OrderBy(t => t?.StatusName).ToList();
                    }
                    else
                    {
                        ticketVMList = ticketList.OrderByDescending(t => t?.StatusName).ToList();
                    }
                }
                else if (ShortColumn.ToLower() == "clientname")
                {
                    if (ShortOrder.ToLower() == "asc")
                    {
                        ticketVMList = ticketList.OrderBy(t => t?.ClientName).ToList();
                    }
                    else
                    {
                        ticketVMList = ticketList.OrderByDescending(t => t?.ClientName).ToList();
                    }
                }
                else
                {
                    ticketVMList = ticketList.OrderByDescending(t => t.Date).ToList();
                }
            }

            return ticketVMList;
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<MateTicketDetailResponse>> Detail(int Id)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var mateTicketObj = _mateTicketService.GetById(Id);
            var ticketResponseObj = _mapper.Map<MateTicketDetailResponse>(mateTicketObj);
            //var clientList = _clientService.GetByTenant(TenantId);
            if (ticketResponseObj != null)
            {
                var mateClientTicketObj = _mateClientTicketService.GetByTicketId(ticketResponseObj.Id);
                if (mateClientTicketObj != null)
                {
                    ticketResponseObj.ClientId = mateClientTicketObj.ClientId;
                    ticketResponseObj.ClientName = mateClientTicketObj.Client?.FirstName + " " + mateClientTicketObj.Client?.LastName;
                }
                if (ticketResponseObj.MatePriorityId != null)
                {
                    ticketResponseObj.MatePriority = mateTicketObj.MatePriority?.Name;
                    ticketResponseObj.MatePriorityColor = mateTicketObj.MatePriority?.Color;
                }
                if (ticketResponseObj.StatusId != null)
                {
                    ticketResponseObj.StatusName = mateTicketObj.Status?.Name;
                    ticketResponseObj.StatusColor = mateTicketObj.Status?.Color;
                }
                if (ticketResponseObj.MateCategoryId != null)
                {
                    ticketResponseObj.MateCategory = mateTicketObj.MateCategory?.Name;
                }
                //Assign User
                var ticketUserList = _mateTicketUserService.GetByTicketId(ticketResponseObj.Id);
                if (ticketUserList != null && ticketUserList.Count > 0)
                {
                    if (ticketResponseObj.AssignUsers == null)
                    {
                        ticketResponseObj.AssignUsers = new List<MateTicketUserDetailResponse>();
                    }
                    var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                    foreach (var assignUser in ticketUserList)
                    {
                        MateTicketUserDetailResponse mateTicketUserObj = new MateTicketUserDetailResponse();
                        mateTicketUserObj.Id = assignUser.Id;
                        mateTicketUserObj.UserId = assignUser.UserId;
                        if (assignUser.User?.ProfileImage != null)
                        {
                            mateTicketUserObj.ProfileURL = OneClappContext.CurrentURL + "User/ProfileImageView/" + assignUser.UserId + "?" + Timestamp;
                        }
                        ticketResponseObj.AssignUsers.Add(mateTicketUserObj);
                    }
                }
                //for total time tracked
                long? Duration = 0;
                var mateTicketTimeRecordList = _mateTicketTimeRecordService.GetByTicketId(ticketResponseObj.Id).Where(t => t.MateTimeRecord != null).ToList();
                if (mateTicketTimeRecordList != null)
                {
                    Duration = mateTicketTimeRecordList.Sum(t => t.MateTimeRecord.Duration);
                    //TimeSpan timeSpan = TimeSpan.FromMinutes(Duration);
                    if (Duration != null)
                    {
                        ticketResponseObj.TotalTimeTracked = Duration.Value;
                    }
                }
            }
            return new OperationResult<MateTicketDetailResponse>(true, System.Net.HttpStatusCode.OK, "", ticketResponseObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<List<MateTicketAcitvityResponse>>> History(long Id)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            List<MateTicketAcitvityResponse> ticketActivityList = new List<MateTicketAcitvityResponse>();
            var AllUsers = _userService.GetAll();
            var activityList = _mateTicketActivityService.GetAllByTicketId(Id);
            ticketActivityList = _mapper.Map<List<MateTicketAcitvityResponse>>(activityList);
            if (ticketActivityList != null && ticketActivityList.Count() > 0)
            {
                foreach (var item in ticketActivityList)
                {
                    var userObj = AllUsers.Where(t => t.Id == item.UserId).FirstOrDefault();
                    if (userObj != null)
                    {
                        item.FirstName = userObj.FirstName;
                        item.LastName = userObj.LastName;
                        item.Email = userObj.Email;

                        var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                        if (userObj.ProfileImage != null)
                        {
                            item.ProfileUrl = OneClappContext.CurrentURL + "User/ProfileImageView/" + userObj.Id + "?" + Timestamp;
                        }
                    }
                }
            }
            return new OperationResult<List<MateTicketAcitvityResponse>>(true, System.Net.HttpStatusCode.OK, "", ticketActivityList);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete]
        public async Task<OperationResult> Remove([FromBody] MateTicketDeleteRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            if (requestmodel.Id > 0)
            {
                var mateTicketId = requestmodel.Id;
                //Client Ticket
                var mateClientTicketObj = await _mateClientTicketService.DeleteByTicketId(mateTicketId);
                if (mateClientTicketObj != null)
                {
                    MateTicketActivity clientTicketActivityObj = new MateTicketActivity();
                    clientTicketActivityObj.MateTicketId = mateTicketId;
                    clientTicketActivityObj.CreatedBy = UserId;
                    clientTicketActivityObj.Activity = Enums.MateTicketActivityEnum.Unassign_client_from_ticket.ToString().Replace("_", " ");
                    var AddUpdateClientTicket = await _mateTicketActivityService.CheckInsertOrUpdate(clientTicketActivityObj);
                }
                //Project Ticket
                var mateProjectTicketObj = await _mateProjectTicketService.DeleteByTicketId(mateTicketId);
                if (mateProjectTicketObj != null)
                {
                    MateTicketActivity projectTicketActivityObj = new MateTicketActivity();
                    projectTicketActivityObj.MateTicketId = mateTicketId;
                    projectTicketActivityObj.EmployeeProjectId = mateProjectTicketObj.EmployeeProjectId;
                    projectTicketActivityObj.CreatedBy = UserId;
                    projectTicketActivityObj.Activity = Enums.MateTicketActivityEnum.Unassign_project_from_ticket.ToString().Replace("_", " ");
                    var AddUpdateProjectTicket = await _mateTicketActivityService.CheckInsertOrUpdate(projectTicketActivityObj);
                }
                //Ticket Task
                var tasks = _mateTicketTaskService.GetAllByTicketId(mateTicketId);
                if (requestmodel.IsKeepTasks == true)
                {
                    if (tasks != null && tasks.Count() > 0)
                    {
                        foreach (var taskObj in tasks)
                        {
                            var mateTicketTaskObj = await _mateTicketTaskService.DeleteByTaskId(taskObj.Id);

                            MateTicketActivity mateTicketTaskActivityObj = new MateTicketActivity();
                            mateTicketTaskActivityObj.EmployeeTaskId = taskObj.EmployeeTaskId;
                            mateTicketTaskActivityObj.MateTicketId = mateTicketId;
                            mateTicketTaskActivityObj.CreatedBy = UserId;
                            mateTicketTaskActivityObj.Activity = Enums.MateTicketActivityEnum.Task_removed_from_ticket.ToString().Replace("_", " ");
                            var AddUpdateTicketTaskActivity = await _mateTicketActivityService.CheckInsertOrUpdate(mateTicketTaskActivityObj);

                        }
                    }
                }
                else
                {
                    if (tasks != null && tasks.Count() > 0)
                    {
                        foreach (var taskObj in tasks)
                        {
                            var employeeTaskId = taskObj.Id;

                            var subTasks = _employeeSubTaskService.GetAllSubTaskByTask(employeeTaskId);

                            if (subTasks != null && subTasks.Count() > 0)
                            {
                                foreach (var subTask in subTasks)
                                {
                                    var subTaskId = subTask.Id;

                                    var childTasks = _employeeChildTaskService.GetAllChildTaskBySubTask(subTaskId);

                                    if (childTasks != null && childTasks.Count() > 0)
                                    {
                                        foreach (var item in childTasks)
                                        {
                                            var childTaskId = item.Id;

                                            var childDocuments = await _employeeChildTaskAttachmentService.DeleteAttachmentByChildTaskId(childTaskId);

                                            // Remove child task documents from folder
                                            if (childDocuments != null && childDocuments.Count() > 0)
                                            {
                                                foreach (var childTaskDoc in childDocuments)
                                                {

                                                    //var dirPath = _hostingEnvironment.WebRootPath + "\\ChildTaskUpload";
                                                    var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.ChildTaskUploadDirPath;
                                                    var filePath = dirPath + "\\" + childTaskDoc.Name;

                                                    if (System.IO.File.Exists(filePath))
                                                    {
                                                        System.IO.File.Delete(Path.Combine(filePath));
                                                    }
                                                }
                                            }

                                            var childComments = await _employeeChildTaskCommentService.DeleteCommentByChildTaskId(childTaskId);

                                            var childTimeRecords = await _employeeChildTaskTimeRecordService.DeleteTimeRecordByEmployeeChildTaskId(childTaskId);

                                            var childTaskUsers = await _employeeChildTaskUserService.DeleteByChildTaskId(childTaskId);

                                            EmployeeChildTaskActivity employeeChildTaskActivityObj = new EmployeeChildTaskActivity();
                                            employeeChildTaskActivityObj.EmployeeChildTaskId = childTaskId;
                                            employeeChildTaskActivityObj.UserId = UserId;
                                            employeeChildTaskActivityObj.Activity = "Removed the task";
                                            var AddUpdate1 = await _employeeChildTaskActivityService.CheckInsertOrUpdate(employeeChildTaskActivityObj);

                                            var childTaskActivities = await _employeeChildTaskActivityService.DeleteByEmployeeChildTaskId(childTaskId);

                                            var childTaskToDelete = await _employeeChildTaskService.Delete(childTaskId);
                                        }
                                    }

                                    var subDocuments = await _employeeSubTaskAttachmentService.DeleteAttachmentByEmployeeSubTaskId(subTaskId);

                                    // Remove sub task documents from folder
                                    if (subDocuments != null && subDocuments.Count() > 0)
                                    {
                                        foreach (var subTaskDoc in subDocuments)
                                        {

                                            //var dirPath = _hostingEnvironment.WebRootPath + "\\SubTaskUpload";
                                            var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.SubTaskUploadDirPath;
                                            var filePath = dirPath + "\\" + subTaskDoc.Name;

                                            if (System.IO.File.Exists(filePath))
                                            {
                                                System.IO.File.Delete(Path.Combine(filePath));
                                            }
                                        }
                                    }

                                    var subComments = await _employeeSubTaskCommentService.DeleteCommentByEmployeeSubTaskId(subTaskId);

                                    var subTimeRecords = await _employeeSubTaskTimeRecordService.DeleteTimeRecordBySubTaskId(subTaskId);

                                    var subTaskUsers = await _employeeSubTaskUserService.DeleteBySubTaskId(subTaskId);

                                    EmployeeSubTaskActivity employeeSubTaskActivityObj = new EmployeeSubTaskActivity();
                                    employeeSubTaskActivityObj.EmployeeSubTaskId = subTaskId;
                                    employeeSubTaskActivityObj.UserId = UserId;
                                    employeeSubTaskActivityObj.Activity = "Removed the task";
                                    var AddUpdate2 = await _employeeSubTaskActivityService.CheckInsertOrUpdate(employeeSubTaskActivityObj);

                                    var subTaskActivities = await _employeeSubTaskActivityService.DeleteByEmployeeSubTaskId(subTaskId);

                                    var subTaskToDelete = await _employeeSubTaskService.Delete(subTaskId);
                                }
                            }

                            var documents = await _employeeTaskAttachmentService.DeleteAttachmentByTaskId(employeeTaskId);

                            // Remove task documents from folder
                            if (documents != null && documents.Count() > 0)
                            {
                                foreach (var taskDoc in documents)
                                {

                                    //var dirPath = _hostingEnvironment.WebRootPath + "\\EmployeeTaskUpload";
                                    var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.EmployeeTaskUploadDirPath;
                                    var filePath = dirPath + "\\" + taskDoc.Name;

                                    if (System.IO.File.Exists(filePath))
                                    {
                                        System.IO.File.Delete(Path.Combine(filePath));
                                    }
                                }
                            }

                            var comments = await _employeeTaskCommentService.DeleteCommentByEmployeeTaskId(employeeTaskId);

                            var timeRecords = await _employeeTaskTimeRecordService.DeleteTimeRecordByTaskId(employeeTaskId);

                            //for EmployeeProjectTask
                            var employeeProjectTaskObj = await _employeeProjectTaskService.DeleteByTaskId(taskObj.Id);

                            EmployeeTaskActivity ProjectTaskActivityObj = new EmployeeTaskActivity();
                            ProjectTaskActivityObj.EmployeeTaskId = taskObj.EmployeeTaskId;
                            ProjectTaskActivityObj.UserId = UserId;
                            ProjectTaskActivityObj.ProjectId = employeeProjectTaskObj.EmployeeProjectId;
                            ProjectTaskActivityObj.Activity = "Removed this task from Project";
                            var AddUpdateProjectTask = await _employeeTaskActivityService.CheckInsertOrUpdate(ProjectTaskActivityObj);

                            //for EmployeeClientTask
                            var employeeClientTaskObj = await _employeeClientTaskService.DeleteByTaskId(employeeTaskId);
                            if (employeeClientTaskObj != null)
                            {
                                EmployeeTaskActivity clientTaskActivityObj = new EmployeeTaskActivity();
                                clientTaskActivityObj.EmployeeTaskId = employeeTaskId;
                                clientTaskActivityObj.UserId = UserId;
                                clientTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Unassign_client_from_task.ToString().Replace("_", " ");
                                var AddUpdateClientTask = await _employeeTaskActivityService.CheckInsertOrUpdate(clientTaskActivityObj);
                            }

                            var taskUsers = await _employeeTaskUserService.DeleteByEmployeeTaskId(employeeTaskId);
                            //ticket task
                            var mateTicketTaskObj = await _mateTicketTaskService.DeleteByTaskId(taskObj.Id);

                            MateTicketActivity mateTicketTaskActivityObj = new MateTicketActivity();
                            mateTicketTaskActivityObj.EmployeeTaskId = taskObj.EmployeeTaskId;
                            mateTicketTaskActivityObj.MateTicketId = mateTicketId;
                            mateTicketTaskActivityObj.CreatedBy = UserId;
                            mateTicketTaskActivityObj.Activity = Enums.MateTicketActivityEnum.Task_removed_from_ticket.ToString().Replace("_", " ");
                            var AddUpdateTicketTaskActivity = await _mateTicketActivityService.CheckInsertOrUpdate(mateTicketTaskActivityObj);

                            var taskToDelete = await _employeeTaskService.Delete(employeeTaskId);

                            EmployeeTaskActivity employeeTaskActivityObj = new EmployeeTaskActivity();
                            employeeTaskActivityObj.EmployeeTaskId = employeeTaskId;
                            employeeTaskActivityObj.UserId = UserId;
                            employeeTaskActivityObj.Activity = "Removed this task";
                            var AddUpdateActivity = await _employeeTaskActivityService.CheckInsertOrUpdate(employeeTaskActivityObj);
                            await _hubContext.Clients.All.OnEmployeeTaskModuleEvent(taskObj.EmployeeTaskId, TenantId);
                        }
                    }
                }
                //for ticket time record
                var mateTicketTimeRecordList = _mateTicketTimeRecordService.GetByTicketId(mateTicketId);
                if (mateTicketTimeRecordList != null && mateTicketTimeRecordList.Count > 0)
                {
                    foreach (var ticketTimeRecord in mateTicketTimeRecordList)
                    {
                        if (ticketTimeRecord != null && ticketTimeRecord.MateTimeRecordId != null)
                        {
                            var mateTimeRecordObj = await _mateTimeRecordService.DeleteMateTimeRecord(ticketTimeRecord.MateTimeRecordId.Value);
                            if (mateTimeRecordObj != null)
                            {
                                MateTicketActivity ticketTimeRecordActivityObj = new MateTicketActivity();
                                ticketTimeRecordActivityObj.MateTicketId = ticketTimeRecord.MateTicketId;
                                ticketTimeRecordActivityObj.CreatedBy = UserId;
                                ticketTimeRecordActivityObj.Activity = Enums.MateTicketActivityEnum.Ticket_time_record_removed.ToString().Replace("_", " ");
                                var AddUpdateTicketTimeRecordObj = await _mateTicketActivityService.CheckInsertOrUpdate(ticketTimeRecordActivityObj);
                            }
                        }
                    }
                }
                //ticket Comment and attchament                
                var mateCommentIdList = _mateTicketCommentService.GetByTicketId(mateTicketId).Select(t => t.MateCommentId).ToList();
                if (mateCommentIdList != null && mateCommentIdList.Count > 0)
                {
                    foreach (var item in mateCommentIdList)
                    {
                        if (item != null)
                        {
                            var mateCommentAttachments = await _mateCommentAttachmentService.DeleteByMateCommentId(item.Value);
                            foreach (var ticketAttachment in mateCommentAttachments)
                            {
                                var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.MateCommentUploadDirPath;
                                var filePath = dirPath + "\\" + ticketAttachment.Name;

                                if (System.IO.File.Exists(filePath))
                                {
                                    System.IO.File.Delete(Path.Combine(filePath));
                                }
                            }
                            var mateCommentObj = await _mateCommentService.DeleteMateComment(item.Value);
                            if (mateCommentObj != null)
                            {
                                MateTicketActivity ticketCommentActivityObj = new MateTicketActivity();
                                ticketCommentActivityObj.MateTicketId = mateTicketId;
                                ticketCommentActivityObj.CreatedBy = UserId;
                                ticketCommentActivityObj.Activity = Enums.MateTicketActivityEnum.Ticket_comment_removed.ToString().Replace("_", " ");
                                var AddUpdateTicketCommentActivity = await _mateTicketActivityService.CheckInsertOrUpdate(ticketCommentActivityObj);
                            }
                        }
                    }
                }
                //task comment and attchament  
                //ticket user
                var ticketUsers = await _mateTicketUserService.DeleteByTicketId(mateTicketId);
                if (ticketUsers != null && ticketUsers.Count > 0)
                {
                    MateTicketActivity ticketUserActivityObj = new MateTicketActivity();
                    ticketUserActivityObj.MateTicketId = mateTicketId;
                    ticketUserActivityObj.CreatedBy = UserId;
                    ticketUserActivityObj.Activity = Enums.MateTicketActivityEnum.User_unassigned_from_ticket.ToString().Replace("_", " ");
                    var AddUpdateTicketUserActivity = await _mateTicketActivityService.CheckInsertOrUpdate(ticketUserActivityObj);
                }

                var ticketObj = await _mateTicketService.DeleteById(mateTicketId);
                if (ticketObj != null)
                {
                    MateTicketActivity ticketActivityObj = new MateTicketActivity();
                    ticketActivityObj.MateTicketId = mateTicketId;
                    ticketActivityObj.CreatedBy = UserId;
                    ticketActivityObj.Activity = Enums.MateTicketActivityEnum.Ticket_removed.ToString().Replace("_", " ");
                    var AddUpdateTicketActivity = await _mateTicketActivityService.CheckInsertOrUpdate(ticketActivityObj);
                }
                await _hubContext.Clients.All.OnMateTicketModuleEvent(mateTicketId, TenantId);
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "", mateTicketId);
            }
            else
            {
                return new OperationResult(false, System.Net.HttpStatusCode.OK, "Please provide id", requestmodel.Id);
            }
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, ExternalUser, TenantUser")]
        [HttpPost]
        public async Task<OperationResult<List<MateTicketAssignUserResponse>>> Assign([FromBody] MateTicketAssignUserRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            List<MateTicketAssignUserResponse> assignUserList = new List<MateTicketAssignUserResponse>();
            //start assign user for ticket
            if (requestModel.AssignedUsers != null && requestModel.AssignedUsers.Count() > 0)
            {
                var existingItem = _mateTicketService.GetById(requestModel.TicketId);

                foreach (var userObj in requestModel.AssignedUsers)
                {
                    MateTicketUser mateTicketUserObj = new MateTicketUser();
                    mateTicketUserObj.MateTicketId = requestModel.TicketId;
                    mateTicketUserObj.UserId = userObj;
                    mateTicketUserObj.CreatedBy = UserId;
                    var isExist = _mateTicketUserService.IsExistOrNot(mateTicketUserObj);
                    var AddUpdatemateTicketUserObj = await _mateTicketUserService.CheckInsertOrUpdate(mateTicketUserObj);

                    if (!isExist)
                    {
                        if (mateTicketUserObj.UserId != null)
                        {
                            var userAssignDetails = _userService.GetUserById(mateTicketUserObj.UserId.Value);
                            if (userAssignDetails != null)
                                await sendEmail.SendEmailMateTicketUserAssignNotification(userAssignDetails.Email, userAssignDetails.FirstName + ' ' + userAssignDetails.LastName, existingItem.Description, TenantId, requestModel.TicketId);
                            MateTicketActivity ticketUserActivityObj = new MateTicketActivity();
                            ticketUserActivityObj.MateTicketId = requestModel.TicketId;
                            ticketUserActivityObj.CreatedBy = UserId;
                            ticketUserActivityObj.Activity = Enums.MateTicketActivityEnum.Ticket_assigned_to_user.ToString().Replace("_", " ");
                            var AddUpdateTicketUserActivity = await _mateTicketActivityService.CheckInsertOrUpdate(ticketUserActivityObj);
                        }
                    }
                }

                var assignUsers = _mateTicketUserService.GetByTicketId(requestModel.TicketId);
                var AllUsers = _userService.GetAll();

                if (assignUsers != null && assignUsers.Count > 0)
                {
                    var assignTicketUserVMList = _mapper.Map<List<MateTicketAssignUserResponse>>(assignUsers);

                    if (assignTicketUserVMList != null && assignTicketUserVMList.Count() > 0)
                    {
                        foreach (var assignUser in assignTicketUserVMList)
                        {
                            if (AllUsers != null)
                            {
                                var userObj = AllUsers.Where(t => t.Id == assignUser.UserId).FirstOrDefault();
                                if (userObj != null)
                                {
                                    assignUser.TicketId = requestModel.TicketId;
                                    assignUser.UserId = userObj.Id;
                                    assignUser.FirstName = userObj.FirstName;
                                    assignUser.LastName = userObj.LastName;
                                    assignUser.Email = userObj.Email;

                                    var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

                                    if (userObj.ProfileImage == null)
                                    {
                                        assignUser.ProfileImageURL = "assets/images/default-profile.jpg";
                                    }
                                    else
                                    {
                                        assignUser.ProfileImageURL = OneClappContext.CurrentURL + "User/ProfileImageView/" + userObj.Id + "?" + Timestamp;
                                    }
                                }
                            }
                        }
                    }
                    assignUserList = assignTicketUserVMList;
                }
                await _hubContext.Clients.All.OnMateTicketModuleEvent(requestModel.TicketId, TenantId);
            }
            //end assign user for project            
            return new OperationResult<List<MateTicketAssignUserResponse>>(true, System.Net.HttpStatusCode.OK, "User assigned successfully", assignUserList);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, ExternalUser, TenantUser")]
        [HttpDelete("{Id}")]
        public async Task<OperationResult> UnAssign(long Id)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            if (Id != null)
            {
                var mateTicketUserObj = await _mateTicketUserService.UnAssign(Id);
                if (mateTicketUserObj != null)
                {
                    if (mateTicketUserObj.MateTicketId != null && mateTicketUserObj.UserId != null)
                    {
                        var userAssignDetails = _userService.GetUserById(mateTicketUserObj.UserId.Value);
                        var existingItem = _mateTicketService.GetById(mateTicketUserObj.MateTicketId.Value);
                        if (userAssignDetails != null)
                            await sendEmail.SendEmailRemoveMateTicketUserAssignNotification(userAssignDetails.Email, userAssignDetails.FirstName + ' ' + userAssignDetails.LastName, existingItem.Name, TenantId);

                        MateTicketActivity mateTicketActivityObj = new MateTicketActivity();
                        mateTicketActivityObj.MateTicketId = mateTicketUserObj.MateTicketId.Value;
                        mateTicketActivityObj.CreatedBy = UserId;
                        mateTicketActivityObj.Activity = Enums.MateTicketActivityEnum.User_unassigned_from_ticket.ToString().Replace("_", " ");
                        var AddUpdateEmployeeProjectActivity = await _mateTicketActivityService.CheckInsertOrUpdate(mateTicketActivityObj);
                        await _hubContext.Clients.All.OnMateTicketModuleEvent(mateTicketUserObj.MateTicketId, TenantId);
                    }
                }
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "User unassigned", Id);
            }
            else
            {
                return new OperationResult(false, System.Net.HttpStatusCode.OK, "Please provide id", Id);
            }
        }

        [HttpPost]
        public async Task<OperationResult<List<MateTicketDropDownListResponse>>> DropDownList([FromBody] MateTicketDropDownListRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            List<MateTicketDropDownListResponse> ticketResponseList = new List<MateTicketDropDownListResponse>();
            var ticketList = _mateTicketUserService.GetByUserId(UserId);
            //All Ticket
            if (requestModel.Type.ToLower() == "ticket" && requestModel.Id == null)
            {
                if (ticketList != null && ticketList.Count > 0)
                {
                    foreach (var item in ticketList)
                    {
                        if (item != null)
                        {
                            MateTicketDropDownListResponse ticketObj = new MateTicketDropDownListResponse();
                            ticketObj = _mapper.Map<MateTicketDropDownListResponse>(item?.MateTicket);
                            ticketResponseList.Add(ticketObj);
                        }
                    }
                }
            }
            //Project ticket
            else if (requestModel.Type.ToLower() == "project" && requestModel.Id != null)
            {
                var MateProjectTicketList = _mateProjectTicketService.GetAllByProjectId(requestModel.Id.Value, TenantId);
                var ProjectTicketList = ticketList.Where(p => MateProjectTicketList.Any(p2 => p2.MateTicketId == p.MateTicketId)).ToList();
                if (ProjectTicketList != null && ProjectTicketList.Count > 0)
                {
                    foreach (var item in ProjectTicketList)
                    {
                        if (item != null)
                        {
                            MateTicketDropDownListResponse projectTicketObj = new MateTicketDropDownListResponse();
                            projectTicketObj = _mapper.Map<MateTicketDropDownListResponse>(item?.MateTicket);
                            ticketResponseList.Add(projectTicketObj);
                        }
                    }
                }
            }
            return new OperationResult<List<MateTicketDropDownListResponse>>(true, System.Net.HttpStatusCode.OK, "", ticketResponseList);
        }

        [HttpGet]
        [SwaggerOperation(Description = "This api is use for Time record List group with ticket")]
        public async Task<OperationResult<List<MateTicketTimeRecordListResponse>>> TimeRecordList()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var ticketList = _mateTicketService.GetAllByTenantId(TenantId);
            List<MateTicketTimeRecordListResponse> ticketResposneList = new List<MateTicketTimeRecordListResponse>();
            if (ticketList != null && ticketList.Count() > 0)
            {
                //time record for ticket
                foreach (var item in ticketList)
                {
                    MateTicketTimeRecordListResponse ticketResponseObj = new MateTicketTimeRecordListResponse();
                    ticketResponseObj = _mapper.Map<MateTicketTimeRecordListResponse>(item);
                    if (item.Id != null)
                    {
                        var mateTicketTimeRecordList = _mateTicketTimeRecordService.GetByTicketIdAndUserId(item.Id, UserId);
                        var mateTicketTimeRecordAscList = mateTicketTimeRecordList.OrderBy(t => t.MateTimeRecord.CreatedOn).ToList();
                        var mateTicketTimeRecordLast = mateTicketTimeRecordAscList.LastOrDefault();
                        long TicketTotalDuration = 0;
                        if (mateTicketTimeRecordList != null && mateTicketTimeRecordList.Count > 0)
                        {
                            foreach (var ticketTimeRecord in mateTicketTimeRecordList)
                            {
                                if (ticketTimeRecord.MateTimeRecord != null)
                                {
                                    if (ticketTimeRecord.MateTimeRecord.Duration != null)
                                    {
                                        TicketTotalDuration = TicketTotalDuration + ticketTimeRecord.MateTimeRecord.Duration.Value;

                                        TimeSpan timeSpan = TimeSpan.FromMinutes(TicketTotalDuration);

                                        ticketResponseObj.TotalDuration = timeSpan.ToString(@"hh\:mm");
                                    }
                                    if (mateTicketTimeRecordLast != null)
                                    {
                                        ticketResponseObj.Enddate = mateTicketTimeRecordLast.MateTimeRecord?.CreatedOn;
                                    }

                                }
                            }
                            ticketResponseObj.TimeRecordCount = mateTicketTimeRecordList.Count;
                        }
                    }
                    ticketResposneList.Add(ticketResponseObj);
                }
                //time record for ticket
            }
            return new OperationResult<List<MateTicketTimeRecordListResponse>>(true, System.Net.HttpStatusCode.OK, "", ticketResposneList);
        }
    }
}