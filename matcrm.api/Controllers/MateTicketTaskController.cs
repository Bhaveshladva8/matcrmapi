using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using matcrm.service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Swashbuckle.AspNetCore.Annotations;
using matcrm.service.Common;
using matcrm.data.Models.Response;
using matcrm.data.Models.Request;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using matcrm.data.Context;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.service.BusinessLogic;
using Microsoft.AspNetCore.SignalR;
using matcrm.api.SignalR;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class MateTicketTaskController : Controller
    {
        private readonly IMateTicketTaskService _mateTicketTaskService;
        private readonly IEmployeeTaskService _employeeTaskService;
        private readonly IEmployeeTaskUserSerivce _employeeTaskUserSerivce;
        private readonly IEmployeeTaskActivityService _employeeTaskActivityService;
        private readonly IMateTicketActivityService _mateTicketActivityService;
        private readonly IUserService _userService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEmailLogService _emailLogService;
        private readonly IEmailConfigService _emailConfigService;
        private readonly IEmailProviderService _emailProvider;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private IMapper _mapper;
        private SendEmail sendEmail;
        private int UserId = 0;
        private int TenantId = 0;

        public MateTicketTaskController(IMateTicketTaskService mateTicketTaskService,
        IEmployeeTaskService employeeTaskService,
        IEmployeeTaskUserSerivce employeeTaskUserSerivce,
        IEmployeeTaskActivityService employeeTaskActivityService,
        IMateTicketActivityService mateTicketActivityService,
        IUserService userService,
        IEmailTemplateService emailTemplateService,
        IEmailLogService emailLogService,
        IEmailConfigService emailConfigService,
        IEmailProviderService emailProvider,
        IHubContext<BroadcastHub, IHubClient> hubContext,
        IMapper mapper)
        {
            _mateTicketTaskService = mateTicketTaskService;
            _employeeTaskService = employeeTaskService;
            _employeeTaskUserSerivce = employeeTaskUserSerivce;
            _employeeTaskActivityService = employeeTaskActivityService;
            _mateTicketActivityService = mateTicketActivityService;
            _userService = userService;
            _emailTemplateService = emailTemplateService;
            _emailLogService = emailLogService;
            _emailConfigService = emailConfigService;
            _emailProvider = emailProvider;
            _mapper = mapper;
            _hubContext = hubContext;
            sendEmail = new SendEmail(emailTemplateService, emailLogService, emailConfigService, emailProvider, mapper);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<MateTicketTaskAddUpdateResponse>> Add([FromBody] MateTicketTaskAddUpdateRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            if (requestModel.MateTicketId != null)
            {
                MateTicketTaskAddUpdateResponse taskResponseObj = new MateTicketTaskAddUpdateResponse();
                var model = _mapper.Map<EmployeeTaskDto>(requestModel);
                if (model.Id == null || model.Id == 0)
                {
                    model.CreatedBy = UserId;
                }
                model.IsActive = true;
                model.TenantId = TenantId;
                var employeeTaskObj = await _employeeTaskService.CheckInsertOrUpdate(model);
                if (employeeTaskObj != null)
                {
                    EmployeeTaskActivity employeeTaskActivityObj = new EmployeeTaskActivity();
                    employeeTaskActivityObj.UserId = UserId;
                    employeeTaskActivityObj.EmployeeTaskId = employeeTaskObj.Id;
                    employeeTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_Created.ToString().Replace("_", " ");
                    var AddUpdate = await _employeeTaskActivityService.CheckInsertOrUpdate(employeeTaskActivityObj);

                    if (requestModel.MateTicketId != null)
                    {
                        MateTicketTask mateTicketTaskObj = new MateTicketTask();
                        mateTicketTaskObj.MateTicketId = requestModel.MateTicketId;
                        mateTicketTaskObj.EmployeeTaskId = employeeTaskObj.Id;
                        var AddUpdateTicketTask = await _mateTicketTaskService.CheckInsertOrUpdate(mateTicketTaskObj);
                        if (AddUpdateTicketTask != null)
                        {
                            MateTicketActivity clientTicketActivityObj = new MateTicketActivity();
                            clientTicketActivityObj.MateTicketId = requestModel.MateTicketId;
                            clientTicketActivityObj.EmployeeTaskId = employeeTaskObj.Id;
                            clientTicketActivityObj.CreatedBy = UserId;
                            clientTicketActivityObj.Activity = Enums.MateTicketActivityEnum.Task_added_in_to_ticket.ToString().Replace("_", " ");
                            var AddClientTicketActivity = await _mateTicketActivityService.CheckInsertOrUpdate(clientTicketActivityObj);
                        }
                    }

                    if (requestModel.AssignedUsers != null && requestModel.AssignedUsers.Count() > 0)
                    {
                        foreach (var userObj in requestModel.AssignedUsers)
                        {
                            EmployeeTaskUserDto employeeTaskUserDto = new EmployeeTaskUserDto();
                            employeeTaskUserDto.EmployeeTaskId = employeeTaskObj.Id;
                            employeeTaskUserDto.UserId = userObj.UserId;
                            employeeTaskUserDto.CreatedBy = UserId;
                            var isExist = _employeeTaskUserSerivce.IsExistOrNot(employeeTaskUserDto);
                            var employeeTaskUserObj = await _employeeTaskUserSerivce.CheckInsertOrUpdate(employeeTaskUserDto);
                            if (employeeTaskUserObj != null)
                            {
                                userObj.Id = employeeTaskUserObj.Id;
                            }
                            if (!isExist)
                            {
                                if (employeeTaskUserDto.UserId != null)
                                {
                                    var userAssignDetails = _userService.GetUserById(employeeTaskUserDto.UserId.Value);
                                    if (userAssignDetails != null)
                                        await sendEmail.SendEmailEmployeeTaskUserAssignNotification(userAssignDetails.Email, userAssignDetails.FirstName + ' ' + userAssignDetails.LastName, requestModel.Description, TenantId,employeeTaskObj.Id);
                                    EmployeeTaskActivity employeeTaskActivityObj1 = new EmployeeTaskActivity();
                                    employeeTaskActivityObj1.EmployeeTaskId = employeeTaskObj.Id;
                                    employeeTaskActivityObj1.UserId = UserId;
                                    employeeTaskActivityObj1.Activity = Enums.EmployeeTaskActivityEnum.Task_assigned_to_user.ToString().Replace("_", " ");
                                    var AddUpdate1 = await _employeeTaskActivityService.CheckInsertOrUpdate(employeeTaskActivityObj1);
                                }
                            }
                        }
                        //employeeTaskDto.AssignedUsers = new List<EmployeeTaskUser>();
                    }
                    await _hubContext.Clients.All.OnEmployeeTaskModuleEvent(employeeTaskObj.Id, TenantId);
                    taskResponseObj = _mapper.Map<MateTicketTaskAddUpdateResponse>(employeeTaskObj);
                    taskResponseObj.MateTicketId = requestModel.MateTicketId;
                }
                await _hubContext.Clients.All.OnMateTicketModuleEvent(requestModel.MateTicketId, TenantId);
                return new OperationResult<MateTicketTaskAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Task added successfully", taskResponseObj);
            }
            else
            {
                return new OperationResult<MateTicketTaskAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Please provide ticketId");
            }
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<MateTicketTaskAddUpdateResponse>> Update([FromBody] MateTicketTaskAddUpdateRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            if (requestModel.MateTicketId != null)
            {
                MateTicketTaskAddUpdateResponse taskResponseObj = new MateTicketTaskAddUpdateResponse();
                var model = _mapper.Map<EmployeeTaskDto>(requestModel);
                if (requestModel.Id != null || requestModel.Id > 0)
                {
                    model.Id = requestModel.Id;
                    model.UpdatedBy = UserId;
                }
                model.IsActive = true;
                model.TenantId = TenantId;
                var employeeTaskObj = await _employeeTaskService.CheckInsertOrUpdate(model);
                if (employeeTaskObj != null)
                {
                    EmployeeTaskActivity employeeTaskActivityObj = new EmployeeTaskActivity();
                    employeeTaskActivityObj.UserId = UserId;
                    employeeTaskActivityObj.EmployeeTaskId = employeeTaskObj.Id;
                    employeeTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_Updated.ToString().Replace("_", " ");
                    var AddUpdate = await _employeeTaskActivityService.CheckInsertOrUpdate(employeeTaskActivityObj);

                    if (requestModel.MateTicketId != null)
                    {
                        MateTicketTask mateTicketTaskObj = new MateTicketTask();
                        mateTicketTaskObj.MateTicketId = requestModel.MateTicketId;
                        mateTicketTaskObj.EmployeeTaskId = employeeTaskObj.Id;
                        var AddUpdateTicketTask = await _mateTicketTaskService.CheckInsertOrUpdate(mateTicketTaskObj);
                        if (AddUpdateTicketTask != null)
                        {
                            MateTicketActivity clientTicketActivityObj = new MateTicketActivity();
                            clientTicketActivityObj.MateTicketId = requestModel.MateTicketId;
                            clientTicketActivityObj.EmployeeTaskId = employeeTaskObj.Id;
                            clientTicketActivityObj.CreatedBy = UserId;
                            clientTicketActivityObj.Activity = Enums.MateTicketActivityEnum.Task_updated_in_to_ticket.ToString().Replace("_", " ");
                            var AddClientTicketActivity = await _mateTicketActivityService.CheckInsertOrUpdate(clientTicketActivityObj);
                        }
                    }

                    if (requestModel.AssignedUsers != null && requestModel.AssignedUsers.Count() > 0)
                    {
                        foreach (var userObj in requestModel.AssignedUsers)
                        {
                            EmployeeTaskUserDto employeeTaskUserDto = new EmployeeTaskUserDto();
                            employeeTaskUserDto.EmployeeTaskId = employeeTaskObj.Id;
                            employeeTaskUserDto.UserId = userObj.UserId;
                            employeeTaskUserDto.CreatedBy = UserId;
                            var isExist = _employeeTaskUserSerivce.IsExistOrNot(employeeTaskUserDto);
                            var employeeTaskUserObj = await _employeeTaskUserSerivce.CheckInsertOrUpdate(employeeTaskUserDto);
                            if (employeeTaskUserObj != null)
                            {
                                userObj.Id = employeeTaskUserObj.Id;
                            }
                            if (!isExist)
                            {
                                if (employeeTaskUserDto.UserId != null)
                                {
                                    var userAssignDetails = _userService.GetUserById(employeeTaskUserDto.UserId.Value);
                                    if (userAssignDetails != null)
                                        await sendEmail.SendEmailEmployeeTaskUserAssignNotification(userAssignDetails.Email, userAssignDetails.FirstName + ' ' + userAssignDetails.LastName, requestModel.Description, TenantId,employeeTaskObj.Id);
                                    EmployeeTaskActivity employeeTaskActivityObj1 = new EmployeeTaskActivity();
                                    employeeTaskActivityObj1.EmployeeTaskId = employeeTaskObj.Id;
                                    employeeTaskActivityObj1.UserId = UserId;
                                    employeeTaskActivityObj1.Activity = Enums.EmployeeTaskActivityEnum.Task_assigned_to_user.ToString().Replace("_", " ");
                                    var AddUpdate1 = await _employeeTaskActivityService.CheckInsertOrUpdate(employeeTaskActivityObj1);
                                }
                            }
                        }
                        //employeeTaskDto.AssignedUsers = new List<EmployeeTaskUser>();
                    }
                    await _hubContext.Clients.All.OnEmployeeTaskModuleEvent(employeeTaskObj.Id, TenantId);
                    taskResponseObj = _mapper.Map<MateTicketTaskAddUpdateResponse>(employeeTaskObj);
                    taskResponseObj.MateTicketId = requestModel.MateTicketId;
                }
                await _hubContext.Clients.All.OnMateTicketModuleEvent(requestModel.MateTicketId, TenantId);
                return new OperationResult<MateTicketTaskAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Task added successfully", taskResponseObj);
            }
            else
            {
                return new OperationResult<MateTicketTaskAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Please provide ticketId");
            }
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{TicketId}")]
        [SwaggerOperation(Description = "This api is use for Ticket task list show on ticket detail")]
        public async Task<OperationResult<List<MateTicketTaskListResponse>>> List(long TicketId)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            List<MateTicketTaskListResponse> ticketTaskList = new List<MateTicketTaskListResponse>();
            var mateTicketTaskList = _mateTicketTaskService.GetAllByTicketId(TicketId);
            var employeeTaskList = _employeeTaskUserSerivce.GetByUserId(UserId);
            var TicketTaskList = employeeTaskList.Where(p => mateTicketTaskList.Any(p2 => p2.EmployeeTaskId == p.EmployeeTaskId)).ToList();
            if (TicketTaskList != null && TicketTaskList.Count > 0)
            {
                foreach (var item in TicketTaskList)
                {
                    MateTicketTaskListResponse ticketTaskObj = new MateTicketTaskListResponse();
                    if (item.EmployeeTaskId != null)
                    {
                        var employeeTaskObj = _employeeTaskService.GetTaskById(item.EmployeeTaskId.Value);
                        if (employeeTaskObj != null)
                        {
                            ticketTaskObj.Id = employeeTaskObj.Id;
                            ticketTaskObj.Name = employeeTaskObj.Name;
                            var taskUserList = _employeeTaskUserSerivce.GetAssignUsersByEmployeeTask(item.EmployeeTaskId.Value);
                            List<MateTicketTaskUserListResponse> userList = new List<MateTicketTaskUserListResponse>();
                            if (taskUserList != null && taskUserList.Count > 0)
                            {
                                var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                                foreach (var userItem in taskUserList)
                                {
                                    MateTicketTaskUserListResponse userObj = new MateTicketTaskUserListResponse();
                                    userObj.Id = userItem.Id;
                                    userObj.UserId = userItem.UserId;
                                    if (userItem.UserId != null)
                                    {
                                        if (userItem.User.ProfileImage != null)
                                        {
                                            userObj.ProfileURL = OneClappContext.CurrentURL + "User/ProfileImageView/" + userItem.UserId + "?" + Timestamp;
                                        }
                                    }
                                    userList.Add(userObj);
                                }
                            }
                            ticketTaskObj.AssignUsers = userList;
                        }
                    }
                    ticketTaskList.Add(ticketTaskObj);
                }
            }
            return new OperationResult<List<MateTicketTaskListResponse>>(true, System.Net.HttpStatusCode.OK, "", ticketTaskList);
        }

    }
}