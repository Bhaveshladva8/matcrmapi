using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;
using matcrm.api.SignalR;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;
using matcrm.data.Models.Tables;
using matcrm.data.Models.ViewModels;
using matcrm.service.BusinessLogic;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.data.Context;
using matcrm.service.Utility;
using Swashbuckle.AspNetCore.Annotations;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class EmployeeTaskController : Controller
    {
        private readonly IEmployeeTaskService _employeeTaskService;
        private readonly IEmployeeTaskUserSerivce _employeeTaskUserService;
        private readonly IEmployeeTaskTimeRecordService _employeeTaskTimeRecordService;
        private readonly IEmployeeSubTaskService _employeeSubTaskService;
        private readonly IEmployeeChildTaskService _employeeChildTaskService;
        private readonly IEmployeeTaskUserSerivce _employeeTaskUserSerivce;
        private readonly IEmployeeSubTaskUserService _employeeSubTaskUserService;
        private readonly IEmployeeChildTaskUserService _employeeChildTaskUserService;
        private readonly IEmployeeSubTaskTimeRecordService _employeeSubTaskTimeRecordService;
        private readonly IEmployeeChildTaskTimeRecordService _employeeChildTaskTimeRecordService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IEmployeeTaskAttachmentService _employeeTaskAttachmentService;
        private readonly IEmployeeTaskActivityService _employeeTaskActivityService;
        private readonly IEmployeeTaskCommentService _employeeTaskCommentService;
        private readonly IUserService _userService;
        private readonly ICustomerService _customerService;
        private readonly IEmployeeTaskStatusService _employeeTaskStatusService;
        private readonly IConfiguration _config;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEmailLogService _emailLogService;
        private readonly IEmailConfigService _emailConfigService;
        private readonly IEmailProviderService _emailProvider;
        private readonly IEmployeeSubTaskAttachmentService _employeeSubTaskAttachmentService;
        private readonly IEmployeeChildTaskAttachmentService _employeeChildTaskAttachmentService;
        private readonly IEmployeeSubTaskActivityService _employeeSubTaskActivityService;
        private readonly IEmployeeChildTaskActivityService _employeeChildTaskActivityService;
        private readonly IEmployeeSubTaskCommentService _employeeSubTaskCommentService;
        private readonly IEmployeeChildTaskCommentService _employeeChildTaskCommentService;
        private readonly IStatusService _statusService;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private readonly OneClappContext _context;
        private readonly IMateTaskTimeRecordService _mateTaskTimeRecordService;
        private readonly IMateTimeRecordService _mateTimeRecordService;
        private readonly IMateTaskCommentService _mateTaskCommentService;
        private readonly IMateCommentAttachmentService _mateCommentAttachmentService;
        private readonly IMateCommentService _mateCommentService;
        private readonly IMateSubTaskCommentService _mateSubTaskCommentService;
        private readonly IMateChildTaskCommentService _mateChildTaskCommentService;
        private readonly IMateChildTaskTimeRecordService _mateChildTaskTimeRecordService;
        private readonly IMateSubTaskTimeRecordService _mateSubTaskTimeRecordService;
        private readonly IClientService _clientService;
        private readonly IEmployeeProjectTaskService _employeeProjectTaskService;
        private readonly IEmployeeClientTaskService _employeeClientTaskService;
        private readonly IMateTicketTaskService _mateTicketTaskService;
        private readonly IMateTicketActivityService _mateTicketActivityService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;

        private SendEmail sendEmail;

        public EmployeeTaskController(IEmployeeTaskService employeeTaskService,
            IEmployeeTaskUserSerivce employeeTaskUserService,
            IEmployeeTaskTimeRecordService employeeTaskTimeRecordService,
            IHostingEnvironment hostingEnvironment,
            IEmployeeTaskAttachmentService employeeTaskAttachmentService,
            IEmployeeTaskActivityService employeeTaskActivityService,
            IUserService userService,
            ICustomerService customerService,
            IEmployeeTaskCommentService employeeTaskCommentService,
            IEmployeeTaskStatusService employeeTaskStatusService,
            IConfiguration config,
            // IPdfTemplateService pdfTemplateService,
            ITenantConfigService tenantConfig,
            IEmailTemplateService emailTemplateService,
            IEmailLogService emailLogService,
            IEmailConfigService emailConfigService,
            IEmailProviderService emailProvider,
            IEmployeeSubTaskService employeeSubTaskService,
            IEmployeeChildTaskService employeeChildTaskService,
            IEmployeeTaskUserSerivce employeeTaskUserSerivce,
            IEmployeeSubTaskUserService employeeSubTaskUserService,
            IEmployeeChildTaskUserService employeeChildTaskUserService,
            IEmployeeSubTaskTimeRecordService employeeSubTaskTimeRecordService,
            IEmployeeChildTaskTimeRecordService employeeChildTaskTimeRecordService,
            IWeClappService weClappService,
            IEmployeeSubTaskAttachmentService employeeSubTaskAttachmentService,
            IEmployeeChildTaskAttachmentService employeeChildTaskAttachmentService,
            IEmployeeSubTaskActivityService employeeSubTaskActivityService,
            IEmployeeChildTaskActivityService employeeChildTaskActivityService,
            IEmployeeSubTaskCommentService employeeSubTaskCommentService,
            IEmployeeChildTaskCommentService employeeChildTaskCommentService,
            IHubContext<BroadcastHub, IHubClient> hubContext,
            OneClappContext context,
            IMapper mapper,
            IStatusService statusService,
            IMateTaskTimeRecordService mateTaskTimeRecordService,
            IMateTimeRecordService mateTimeRecordService,
            IMateTaskCommentService mateTaskCommentService,
            IMateCommentAttachmentService mateCommentAttachmentService,
            IMateCommentService mateCommentService,
            IMateSubTaskCommentService mateSubTaskCommentService,
            IMateChildTaskCommentService mateChildTaskCommentService,
            IMateChildTaskTimeRecordService mateChildTaskTimeRecordService,
            IMateSubTaskTimeRecordService mateSubTaskTimeRecordService,
            IClientService clientService,
            IEmployeeProjectTaskService employeeProjectTaskService,
            IEmployeeClientTaskService employeeClientTaskService,
            IMateTicketTaskService mateTicketTaskService,
            IMateTicketActivityService mateTicketActivityService)
        {
            _employeeTaskService = employeeTaskService;
            _employeeTaskUserService = employeeTaskUserService;
            _employeeTaskTimeRecordService = employeeTaskTimeRecordService;
            _hostingEnvironment = hostingEnvironment;
            _employeeTaskAttachmentService = employeeTaskAttachmentService;
            _employeeTaskActivityService = employeeTaskActivityService;
            _userService = userService;
            _customerService = customerService;
            _employeeTaskCommentService = employeeTaskCommentService;
            _employeeTaskStatusService = employeeTaskStatusService;
            _emailTemplateService = emailTemplateService;
            _emailLogService = emailLogService;
            _emailProvider = emailProvider;
            _employeeTaskService = employeeTaskService;
            _employeeSubTaskService = employeeSubTaskService;
            _employeeChildTaskService = employeeChildTaskService;
            _employeeTaskUserSerivce = employeeTaskUserSerivce;
            _employeeSubTaskUserService = employeeSubTaskUserService;
            _employeeChildTaskUserService = employeeChildTaskUserService;
            _employeeTaskTimeRecordService = employeeTaskTimeRecordService;
            _employeeSubTaskTimeRecordService = employeeSubTaskTimeRecordService;
            _employeeChildTaskTimeRecordService = employeeChildTaskTimeRecordService;
            _hostingEnvironment = hostingEnvironment;
            _employeeSubTaskAttachmentService = employeeSubTaskAttachmentService;
            _employeeChildTaskAttachmentService = employeeChildTaskAttachmentService;
            _employeeSubTaskActivityService = employeeSubTaskActivityService;
            _employeeChildTaskActivityService = employeeChildTaskActivityService;
            _employeeSubTaskCommentService = employeeSubTaskCommentService;
            _employeeChildTaskCommentService = employeeChildTaskCommentService;
            _employeeTaskStatusService = employeeTaskStatusService;
            _hubContext = hubContext;
            _context = context;
            _config = config;
            _mapper = mapper;
            _statusService = statusService;
            _mateTaskTimeRecordService = mateTaskTimeRecordService;
            _mateTimeRecordService = mateTimeRecordService;
            _mateTaskCommentService = mateTaskCommentService;
            _mateCommentAttachmentService = mateCommentAttachmentService;
            _mateCommentService = mateCommentService;
            _mateSubTaskCommentService = mateSubTaskCommentService;
            _mateChildTaskCommentService = mateChildTaskCommentService;
            _mateChildTaskTimeRecordService = mateChildTaskTimeRecordService;
            _mateSubTaskTimeRecordService = mateSubTaskTimeRecordService;
            _clientService = clientService;
            _employeeProjectTaskService = employeeProjectTaskService;
            _employeeClientTaskService = employeeClientTaskService;
            _mateTicketTaskService = mateTicketTaskService;
            _mateTicketActivityService = mateTicketActivityService;
            sendEmail = new SendEmail(emailTemplateService, emailLogService, emailConfigService, emailProvider, mapper);
        }

        // Save Time Record [Task]
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<EmployeeTaskAddUpdateResponse>> Add([FromBody] AddUpdateEmployeeTaskRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            // if (requestModel.ClientId != null && requestModel.ClientId > 0)
            // {
            var employeeTaskDto = _mapper.Map<EmployeeTaskDto>(requestModel);
            employeeTaskDto.IsActive = true;
            employeeTaskDto.TenantId = TenantId;

            if (requestModel.Id == null || requestModel.Id == 0)
            {
                employeeTaskDto.CreatedBy = UserId;
            }
            Random generator = new Random();
            String r = generator.Next(0, 1000000).ToString("D6");
            employeeTaskDto.TaskNo = "#" + r;

            // For all subtask with completed status then main task automatic completed status
            // if (employeeTask.Id != null) {
            //     var taskId = employeeTask.Id.Value;
            //     var subTasks = _subTaskService.GetAllSubTaskByTask (taskId);
            //     if (employeeTask.UserId != null && subTasks.Count () > 0) {
            //         var userId = employeeTask.UserId.Value;
            //         var employeeTaskStatusList = _employeeTaskService.GetTaskByUser (userId);
            //         var finalStatus = employeeTaskStatusList.Where (t => t.IsFinalize == true).FirstOrDefault ();
            //         if (finalStatus != null) {
            //             var completedSubTaskCount = subTasks.Where (t => t.StatusId == finalStatus.Id).Count ();
            //             if (subTasks.Count () == completedSubTaskCount) {
            //                 taskObj.StatusId = finalStatus.Id;
            //             }
            //         }
            //     }
            // }

            var taskResult = await _employeeTaskService.CheckInsertOrUpdate(employeeTaskDto);
            if (taskResult != null)
            {
                employeeTaskDto.Id = taskResult.Id;
            }
            EmployeeTaskActivity employeeTaskActivityObj = new EmployeeTaskActivity();
            employeeTaskActivityObj.UserId = UserId;
            employeeTaskActivityObj.EmployeeTaskId = taskResult.Id;
            employeeTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_Created.ToString().Replace("_", " ");
            var AddUpdate = await _employeeTaskActivityService.CheckInsertOrUpdate(employeeTaskActivityObj);

            if (requestModel.ClientId != null)
            {
                EmployeeClientTask employeeClientTaskObj = new EmployeeClientTask();
                employeeClientTaskObj.ClientId = requestModel.ClientId;
                employeeClientTaskObj.EmployeeTaskId = employeeTaskDto.Id;
                var AddUpdateClientTask = await _employeeClientTaskService.CheckInsertOrUpdate(employeeClientTaskObj);
                if (AddUpdateClientTask != null)
                {
                    EmployeeTaskActivity clientTaskActivityObj = new EmployeeTaskActivity();
                    clientTaskActivityObj.UserId = UserId;
                    clientTaskActivityObj.EmployeeTaskId = taskResult.Id;
                    clientTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_assign_to_Client.ToString().Replace("_", " ");
                    var AddUpdateClientTaskActivity = await _employeeTaskActivityService.CheckInsertOrUpdate(clientTaskActivityObj);
                }
            }

            if (requestModel.ProjectId != null)
            {
                EmployeeProjectTask employeeProjectTaskObj = new EmployeeProjectTask();
                employeeProjectTaskObj.EmployeeProjectId = requestModel.ProjectId;
                employeeProjectTaskObj.EmployeeTaskId = employeeTaskDto.Id;
                var AddUpdateProjectTask = await _employeeProjectTaskService.CheckInsertOrUpdate(employeeProjectTaskObj);

                if (AddUpdateProjectTask != null)
                {
                    EmployeeTaskActivity projectTaskActivityObj = new EmployeeTaskActivity();
                    projectTaskActivityObj.UserId = UserId;
                    projectTaskActivityObj.ProjectId = AddUpdateProjectTask.EmployeeProjectId;
                    projectTaskActivityObj.EmployeeTaskId = taskResult.Id;
                    projectTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_assign_to_project.ToString().Replace("_", " ");
                    var AddUpdateProjectTaskActivity = await _employeeTaskActivityService.CheckInsertOrUpdate(projectTaskActivityObj);
                }
            }

            if (requestModel.AssignedUsers != null && requestModel.AssignedUsers.Count() > 0)
            {
                foreach (var userObj in requestModel.AssignedUsers)
                {
                    EmployeeTaskUserDto employeeTaskUserDto = new EmployeeTaskUserDto();
                    employeeTaskUserDto.EmployeeTaskId = taskResult.Id;
                    employeeTaskUserDto.UserId = userObj.UserId;
                    employeeTaskUserDto.CreatedBy = UserId;
                    var isExist = _employeeTaskUserService.IsExistOrNot(employeeTaskUserDto);
                    var employeeTaskUserObj = await _employeeTaskUserService.CheckInsertOrUpdate(employeeTaskUserDto);
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
                                await sendEmail.SendEmailEmployeeTaskUserAssignNotification(userAssignDetails.Email, userAssignDetails.FirstName + ' ' + userAssignDetails.LastName, requestModel.Description, TenantId, taskResult.Id);
                            EmployeeTaskActivity employeeTaskActivityObj1 = new EmployeeTaskActivity();
                            employeeTaskActivityObj1.EmployeeTaskId = taskResult.Id;
                            employeeTaskActivityObj1.UserId = UserId;
                            employeeTaskActivityObj1.Activity = Enums.EmployeeTaskActivityEnum.Task_assigned_to_user.ToString().Replace("_", " ");
                            var AddUpdate1 = await _employeeTaskActivityService.CheckInsertOrUpdate(employeeTaskActivityObj1);
                        }
                    }
                }
                employeeTaskDto.AssignedUsers = new List<EmployeeTaskUser>();
            }

            if (requestModel.SubTasks != null && requestModel.SubTasks.Count > 0)
            {
                foreach (var subtaskItem in requestModel.SubTasks)
                {
                    EmployeeSubTaskDto employeeSubTaskDtoObj = new EmployeeSubTaskDto();
                    employeeSubTaskDtoObj.Description = subtaskItem.Description;
                    employeeSubTaskDtoObj.EmployeeTaskId = taskResult.Id;
                    employeeSubTaskDtoObj.IsActive = true;
                    employeeSubTaskDtoObj.CreatedBy = UserId;
                    var subTaskResult = await _employeeSubTaskService.CheckInsertOrUpdate(employeeSubTaskDtoObj);
                    if (subTaskResult != null)
                    {
                        EmployeeSubTaskActivity subTaskActivityObj = new EmployeeSubTaskActivity();
                        subTaskActivityObj.EmployeeSubTaskId = subTaskResult.Id;
                        subTaskActivityObj.UserId = UserId;
                        subTaskActivityObj.Activity = Enums.EmployeeSubTaskActivityEnum.Subtask_Created.ToString().Replace("_", " ");
                        var subTaskAddUpdateActivity = await _employeeSubTaskActivityService.CheckInsertOrUpdate(subTaskActivityObj);
                        if (subtaskItem.AssignedUsers != null && subtaskItem.AssignedUsers.Count > 0)
                        {
                            foreach (var subAssignUser in subtaskItem.AssignedUsers)
                            {
                                EmployeeSubTaskUserDto employeeSubTaskUserDtoObj = new EmployeeSubTaskUserDto();
                                employeeSubTaskUserDtoObj.UserId = subAssignUser.UserId;
                                employeeSubTaskUserDtoObj.EmployeeSubTaskId = subTaskResult.Id;
                                var isExist = _employeeSubTaskUserService.IsExistOrNot(employeeSubTaskUserDtoObj);
                                var subTaskAssignUserResult = await _employeeSubTaskUserService.CheckInsertOrUpdate(employeeSubTaskUserDtoObj);
                                if (subTaskAssignUserResult != null)
                                {
                                    subAssignUser.Id = subTaskAssignUserResult.Id;
                                }
                                if (!isExist)
                                {
                                    if (employeeSubTaskUserDtoObj.UserId != null)
                                    {
                                        var userAssignDetails = _userService.GetUserById(employeeSubTaskUserDtoObj.UserId.Value);
                                        if (userAssignDetails != null)
                                            await sendEmail.SendEmailEmployeeTaskUserAssignNotification(userAssignDetails.Email, userAssignDetails.FirstName + ' ' + userAssignDetails.LastName, subtaskItem.Description, TenantId, subTaskResult.Id);
                                        EmployeeSubTaskActivity subTaskAssignActivityObj = new EmployeeSubTaskActivity();
                                        subTaskAssignActivityObj.EmployeeSubTaskId = subTaskResult.Id;
                                        subTaskAssignActivityObj.UserId = UserId;
                                        subTaskAssignActivityObj.Activity = Enums.EmployeeSubTaskActivityEnum.Subtask_assigned_to_user.ToString().Replace("_", " ");
                                        var subTaskAssignActivity = await _employeeSubTaskActivityService.CheckInsertOrUpdate(subTaskAssignActivityObj);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            var employeeTaskAddUpdateResponse = _mapper.Map<EmployeeTaskAddUpdateResponse>(employeeTaskDto);
            employeeTaskAddUpdateResponse.AssignedUsers = requestModel.AssignedUsers;
            await _hubContext.Clients.All.OnEmployeeTaskModuleEvent(employeeTaskAddUpdateResponse.Id, TenantId);
            return new OperationResult<EmployeeTaskAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Task saved successfully.", employeeTaskAddUpdateResponse);
            // }
            // else
            // {
            //     return new OperationResult<EmployeeTaskAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Please provide ClientId");
            // }
        }

        // Save Time Record [Task]
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<EmployeeTaskAddUpdateResponse>> Update([FromBody] AddUpdateEmployeeTaskRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            // if (requestModel.ClientId != null && requestModel.ClientId > 0)
            // {
            EmployeeTaskAddUpdateResponse employeeTaskAddUpdateResponse = new EmployeeTaskAddUpdateResponse();
            var employeeTaskDto = _mapper.Map<EmployeeTaskDto>(requestModel);
            employeeTaskDto.IsActive = true;
            employeeTaskDto.TenantId = TenantId;
            if (requestModel.Id != null || requestModel.Id > 0)
            {
                employeeTaskDto.Id = requestModel.Id;
                employeeTaskDto.UpdatedBy = UserId;
            }
            Random generator = new Random();
            String r = generator.Next(0, 1000000).ToString("D6");
            employeeTaskDto.TaskNo = "#" + r;

            // For all subtask with completed status then main task automatic completed status
            // if (employeeTask.Id != null) {
            //     var taskId = employeeTask.Id.Value;
            //     var subTasks = _subTaskService.GetAllSubTaskByTask (taskId);
            //     if (employeeTask.UserId != null && subTasks.Count () > 0) {
            //         var userId = employeeTask.UserId.Value;
            //         var employeeTaskStatusList = _employeeTaskService.GetTaskByUser (userId);
            //         var finalStatus = employeeTaskStatusList.Where (t => t.IsFinalize == true).FirstOrDefault ();
            //         if (finalStatus != null) {
            //             var completedSubTaskCount = subTasks.Where (t => t.StatusId == finalStatus.Id).Count ();
            //             if (subTasks.Count () == completedSubTaskCount) {
            //                 taskObj.StatusId = finalStatus.Id;
            //             }
            //         }
            //     }
            // }

            var taskResult = await _employeeTaskService.CheckInsertOrUpdate(employeeTaskDto);
            if (taskResult != null)
            {
                employeeTaskDto.Id = taskResult.Id;
            }

            EmployeeTaskActivity employeeTaskActivityObj = new EmployeeTaskActivity();
            employeeTaskActivityObj.UserId = UserId;
            employeeTaskActivityObj.EmployeeTaskId = taskResult.Id;
            employeeTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_Updated.ToString().Replace("_", " ");
            var AddUpdate = await _employeeTaskActivityService.CheckInsertOrUpdate(employeeTaskActivityObj);

            if (requestModel.ClientId != null)
            {
                EmployeeClientTask employeeClientTaskObj = new EmployeeClientTask();
                employeeClientTaskObj.ClientId = requestModel.ClientId;
                employeeClientTaskObj.EmployeeTaskId = employeeTaskDto.Id;
                var AddUpdateClientTask = await _employeeClientTaskService.CheckInsertOrUpdate(employeeClientTaskObj);
                if (AddUpdateClientTask != null)
                {
                    EmployeeTaskActivity clientTaskActivityObj = new EmployeeTaskActivity();
                    clientTaskActivityObj.UserId = UserId;
                    clientTaskActivityObj.EmployeeTaskId = taskResult.Id;
                    clientTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_assign_to_Client.ToString().Replace("_", " ");
                    var AddUpdateClientTaskActivity = await _employeeTaskActivityService.CheckInsertOrUpdate(clientTaskActivityObj);
                }
            }

            if (requestModel.ProjectId != null)
            {
                EmployeeProjectTask employeeProjectTaskObj = new EmployeeProjectTask();
                employeeProjectTaskObj.EmployeeProjectId = requestModel.ProjectId;
                employeeProjectTaskObj.EmployeeTaskId = employeeTaskDto.Id;
                var AddUpdateProjectTask = await _employeeProjectTaskService.CheckInsertOrUpdate(employeeProjectTaskObj);

                if (AddUpdateProjectTask != null)
                {
                    EmployeeTaskActivity projectTaskActivityObj = new EmployeeTaskActivity();
                    projectTaskActivityObj.UserId = UserId;
                    projectTaskActivityObj.ProjectId = AddUpdateProjectTask.EmployeeProjectId;
                    projectTaskActivityObj.EmployeeTaskId = taskResult.Id;
                    projectTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_assign_to_project.ToString().Replace("_", " ");
                    var AddUpdateProjectTaskActivity = await _employeeTaskActivityService.CheckInsertOrUpdate(projectTaskActivityObj);
                }
            }

            if (requestModel.AssignedUsers != null && requestModel.AssignedUsers.Count() > 0)
            {
                foreach (var userObj in requestModel.AssignedUsers)
                {
                    EmployeeTaskUserDto employeeTaskUserDto = new EmployeeTaskUserDto();
                    employeeTaskUserDto.EmployeeTaskId = taskResult.Id;
                    employeeTaskUserDto.UserId = userObj.UserId;
                    employeeTaskUserDto.CreatedBy = UserId;
                    var isExist = _employeeTaskUserService.IsExistOrNot(employeeTaskUserDto);
                    var employeeTaskUserObj = await _employeeTaskUserService.CheckInsertOrUpdate(employeeTaskUserDto);
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
                                await sendEmail.SendEmailEmployeeTaskUserAssignNotification(userAssignDetails.Email, userAssignDetails.FirstName + ' ' + userAssignDetails.LastName, requestModel.Description, TenantId, taskResult.Id);
                            EmployeeTaskActivity employeeTaskActivityObj1 = new EmployeeTaskActivity();
                            employeeTaskActivityObj1.EmployeeTaskId = taskResult.Id;
                            employeeTaskActivityObj1.UserId = UserId;
                            //employeeTaskActivityObj1.ProjectId = taskResult.ProjectId;
                            // employeeTaskActivityObj1.Activity = "Assigned the user";
                            employeeTaskActivityObj1.Activity = Enums.EmployeeTaskActivityEnum.Task_assigned_to_user.ToString().Replace("_", " ");
                            var AddUpdate1 = await _employeeTaskActivityService.CheckInsertOrUpdate(employeeTaskActivityObj1);
                        }
                    }
                }
                employeeTaskDto.AssignedUsers = new List<EmployeeTaskUser>();
            }

            if (requestModel.SubTasks != null && requestModel.SubTasks.Count > 0)
            {
                foreach (var subtaskItem in requestModel.SubTasks)
                {
                    EmployeeSubTaskDto employeeSubTaskDtoObj = new EmployeeSubTaskDto();
                    employeeSubTaskDtoObj.Id = subtaskItem.Id;
                    employeeSubTaskDtoObj.Description = subtaskItem.Description;
                    employeeSubTaskDtoObj.EmployeeTaskId = taskResult.Id;
                    employeeSubTaskDtoObj.IsActive = true;
                    if (subtaskItem.Id != null && subtaskItem.Id > 0)
                    {
                        employeeSubTaskDtoObj.UpdatedBy = UserId;
                    }
                    else
                    {
                        employeeSubTaskDtoObj.CreatedBy = UserId;
                    }

                    var subTaskResult = await _employeeSubTaskService.CheckInsertOrUpdate(employeeSubTaskDtoObj);
                    if (subTaskResult != null)
                    {
                        EmployeeSubTaskActivity subTaskActivityObj = new EmployeeSubTaskActivity();
                        subTaskActivityObj.EmployeeSubTaskId = subTaskResult.Id;
                        subTaskActivityObj.UserId = UserId;
                        if (subtaskItem.Id != null && subtaskItem.Id > 0)
                        {
                            subTaskActivityObj.Activity = Enums.EmployeeSubTaskActivityEnum.Subtask_Updated.ToString().Replace("_", " ");
                        }
                        else
                        {
                            subTaskActivityObj.Activity = Enums.EmployeeSubTaskActivityEnum.Subtask_Created.ToString().Replace("_", " ");
                        }
                        var subTaskAddUpdateActivity = await _employeeSubTaskActivityService.CheckInsertOrUpdate(subTaskActivityObj);
                        if (subtaskItem.AssignedUsers != null && subtaskItem.AssignedUsers.Count > 0)
                        {
                            foreach (var subAssignUser in subtaskItem.AssignedUsers)
                            {
                                EmployeeSubTaskUserDto employeeSubTaskUserDtoObj = new EmployeeSubTaskUserDto();
                                employeeSubTaskUserDtoObj.UserId = subAssignUser.UserId;
                                employeeSubTaskUserDtoObj.EmployeeSubTaskId = subTaskResult.Id;
                                var isExist = _employeeSubTaskUserService.IsExistOrNot(employeeSubTaskUserDtoObj);
                                var subTaskAssignUserResult = await _employeeSubTaskUserService.CheckInsertOrUpdate(employeeSubTaskUserDtoObj);
                                if (subTaskAssignUserResult != null)
                                {
                                    subAssignUser.Id = subTaskAssignUserResult.Id;
                                }
                                if (!isExist)
                                {
                                    var userAssignDetails = _userService.GetUserById(UserId);
                                    if (userAssignDetails != null)
                                        await sendEmail.SendEmailEmployeeTaskUserAssignNotification(userAssignDetails.Email, userAssignDetails.FirstName + ' ' + userAssignDetails.LastName, subtaskItem.Description, TenantId, subTaskResult.Id);
                                    EmployeeSubTaskActivity subTaskAssignActivityObj = new EmployeeSubTaskActivity();
                                    subTaskAssignActivityObj.EmployeeSubTaskId = subTaskResult.Id;
                                    subTaskAssignActivityObj.UserId = UserId;
                                    subTaskAssignActivityObj.Activity = Enums.EmployeeSubTaskActivityEnum.Subtask_assigned_to_user.ToString().Replace("_", " ");
                                    var subTaskAssignActivity = await _employeeSubTaskActivityService.CheckInsertOrUpdate(subTaskAssignActivityObj);
                                }
                            }
                        }
                    }
                }
            }
            employeeTaskAddUpdateResponse = _mapper.Map<EmployeeTaskAddUpdateResponse>(employeeTaskDto);
            employeeTaskAddUpdateResponse.AssignedUsers = requestModel.AssignedUsers;
            //await _hubContext.Clients.All.OnEmployeeTaskEventEmit(taskResult.Id);
            await _hubContext.Clients.All.OnEmployeeTaskModuleEvent(employeeTaskAddUpdateResponse.Id, TenantId);
            return new OperationResult<EmployeeTaskAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Task updated successfully.", employeeTaskAddUpdateResponse);
            // }
            // else
            // {
            //     return new OperationResult<EmployeeTaskAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Please provide ClientId");
            // }
        }

        // [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        // [HttpPost]
        // public async Task<OperationResult<List<EmployeeTaskListResponse>>> List([FromBody] EmployeeTaskListRequest model)
        // {
        //     ClaimsPrincipal user = this.User as ClaimsPrincipal;
        //     UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
        //     TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

        //     List<EmployeeTask> employeeTaskList = new List<EmployeeTask>();

        //     employeeTaskList = _employeeTaskService.GetAllTaskByTenant(TenantId, model);
        //     var AllStatus = _statusService.GetAll();
        //     var AllUsers = _userService.GetAll();

        //     var employeeTaskListResponseList = _mapper.Map<List<EmployeeTaskListResponse>>(employeeTaskList);
        //     //EmployeeTaskListResponse employeeTaskListResponseObj = new EmployeeTaskListResponse();

        //     if (employeeTaskListResponseList != null && employeeTaskListResponseList.Count() > 0)
        //     {
        //         foreach (var item in employeeTaskListResponseList)
        //         {
        //             var statusObj = AllStatus.Where(t => t.Id == item.StatusId).FirstOrDefault();
        //             if (statusObj != null)
        //             {
        //                 item.Status = statusObj.Name;
        //             }
        //             var taskObj = employeeTaskList.Where(t => t.Id == item.Id).FirstOrDefault();
        //             if (taskObj != null)
        //             {
        //                 var overDueStatusObj = AllStatus.Where(t => t.Name.ToLower() == "overdue").FirstOrDefault();
        //                 if (taskObj.EndDate != null && taskObj.EndDate < DateTime.UtcNow)
        //                 {
        //                     item.StatusId = overDueStatusObj?.Id;
        //                     item.Status = overDueStatusObj?.Name;
        //                 }
        //             }
        //             //item.Name = item.Name;
        //             var assignTaskUsers = _employeeTaskUserService.GetAssignUsersByEmployeeTask(item.Id);
        //             if (assignTaskUsers != null && assignTaskUsers.Count > 0)
        //             {
        //                 var assignTaskUserVMList = _mapper.Map<List<EmployeeTaskUserRequestResponse>>(assignTaskUsers);
        //                 if (item.AssignedUsers == null)
        //                 {
        //                     item.AssignedUsers = new List<EmployeeTaskUserRequestResponse>();
        //                 }
        //                 foreach (var assignTaskUser in assignTaskUserVMList)
        //                 {
        //                     if (AllUsers != null)
        //                     {
        //                         var userObj2 = AllUsers.Where(t => t.Id == assignTaskUser.UserId).FirstOrDefault();
        //                         if (userObj2 != null)
        //                         {
        //                             assignTaskUser.AssignUserFirstName = userObj2.FirstName;
        //                             assignTaskUser.AssignUserLastName = userObj2.LastName;
        //                         }
        //                     }
        //                 }
        //                 item.AssignedUsers = assignTaskUserVMList;
        //             }

        //             //task time record
        //             var mateTaskTimeRecordList = _mateTaskTimeRecordService.GetByTaskIdAndUserId(item.Id, UserId);
        //             var mateTaskTimeRecordAscList = mateTaskTimeRecordList.OrderBy(t => t.MateTimeRecord.CreatedOn).ToList();
        //             var mateTaskTimeRecordLast = mateTaskTimeRecordAscList.LastOrDefault();
        //             long TaskTotalDuration = 0;
        //             if (mateTaskTimeRecordList != null && mateTaskTimeRecordList.Count > 0)
        //             {
        //                 foreach (var taskTimeRecord in mateTaskTimeRecordList)
        //                 {
        //                     if (taskTimeRecord.MateTimeRecord != null)
        //                     {
        //                         if (taskTimeRecord.MateTimeRecord.Duration != null)
        //                         {
        //                             TaskTotalDuration = TaskTotalDuration + taskTimeRecord.MateTimeRecord.Duration.Value;

        //                             TimeSpan timeSpan = TimeSpan.FromMinutes(TaskTotalDuration);

        //                             item.TotalDuration = timeSpan.ToString(@"hh\:mm"); ;
        //                             if (mateTaskTimeRecordLast != null)
        //                             {
        //                                 item.Enddate = mateTaskTimeRecordLast.MateTimeRecord.CreatedOn;
        //                             }

        //                         }
        //                     }
        //                 }
        //                 item.TimeRecordCount = mateTaskTimeRecordList.Count;
        //             }
        //         }

        //     }
        //     return new OperationResult<List<EmployeeTaskListResponse>>(true, System.Net.HttpStatusCode.OK, "", employeeTaskListResponseList);
        // }

        // [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        // [HttpPost]
        // public async Task<OperationResult<List<EmployeeGroupTaskListResponse>>> GroupTaskList([FromBody] EmployeeGroupTaskListRequest model)
        // {
        //     ClaimsPrincipal user = this.User as ClaimsPrincipal;
        //     UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
        //     TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
        //     List<EmployeeGroupTaskListResponse> employeeGroupTaskListResponses = new List<EmployeeGroupTaskListResponse>();

        //     List<EmployeeTask> employeeTaskList = new List<EmployeeTask>();
        //     EmployeeTaskListRequest requestModel = _mapper.Map<EmployeeTaskListRequest>(model);
        //     employeeTaskList = _employeeTaskService.GetByTenantWithoutProject(TenantId);
        //     var AllStatus = _statusService.GetByTenant(TenantId);
        //     var AllUsers = _userService.GetAll();
        //     if (requestModel.PageNumber > 1)
        //     {
        //         if (!string.IsNullOrEmpty(model.GroupBy))
        //         {
        //             employeeTaskList = employeeTaskList.Where(t => t?.StatusId == Convert.ToInt32(model.GroupBy)).ToList();
        //         }
        //     }
        //     var TaskKeyValueData = employeeTaskList.GroupBy(t => t?.StatusId);

        //     foreach (var TaskData in TaskKeyValueData)
        //     {
        //         EmployeeGroupTaskListResponse taskResponseObj = new EmployeeGroupTaskListResponse();
        //         var SkipValue = requestModel.PageSize * (requestModel.PageNumber - 1);
        //         taskResponseObj.TotalCount = TaskData.ToList().Count();

        //         if (TaskData.Key != null)
        //         {
        //             var StatusObj = AllStatus.Where(t => t.Id == TaskData.Key).FirstOrDefault();
        //             if (StatusObj != null)
        //             {
        //                 taskResponseObj.Id = StatusObj.Id;
        //                 taskResponseObj.Name = StatusObj.Name;
        //             }
        //             if (!string.IsNullOrEmpty(requestModel.SearchString))
        //             {
        //                 var TaskList = TaskData.Where(t => t.Description.ToLower().Contains(requestModel.SearchString.ToLower()) || t.Name.ToLower().Contains(requestModel.SearchString.ToLower())).ToList();
        //                 taskResponseObj.TotalCount = TaskList.Count();
        //                 var Tasks = TaskList.Skip(SkipValue).Take(requestModel.PageSize).ToList();
        //                 taskResponseObj.Tasks = _mapper.Map<List<EmployeeGroupTask>>(Tasks.ToList());
        //             }
        //             else
        //             {
        //                 taskResponseObj.TotalCount = TaskData.ToList().Count();
        //                 var Tasks = TaskData.Skip(SkipValue).Take(requestModel.PageSize).ToList();
        //                 taskResponseObj.Tasks = _mapper.Map<List<EmployeeGroupTask>>(Tasks);
        //             }

        //             foreach (var TaskObj in taskResponseObj.Tasks)
        //             {
        //                 if (TaskObj.Id != null)
        //                 {
        //                     var mateTaskTimeRecordList = _mateTaskTimeRecordService.GetByTaskIdAndUserId(TaskObj.Id.Value, UserId);
        //                     var mateTaskTimeRecordAscList = mateTaskTimeRecordList.OrderBy(t => t.MateTimeRecord.CreatedOn).ToList();
        //                     long ProjectTotalDuration = 0;
        //                     if (mateTaskTimeRecordList != null && mateTaskTimeRecordList.Count > 0)
        //                     {
        //                         foreach (var taskTimeRecord in mateTaskTimeRecordList)
        //                         {
        //                             if (taskTimeRecord.MateTimeRecord != null)
        //                             {
        //                                 if (taskTimeRecord.MateTimeRecord.Duration != null)
        //                                 {
        //                                     ProjectTotalDuration = ProjectTotalDuration + taskTimeRecord.MateTimeRecord.Duration.Value;

        //                                     TimeSpan timeSpan = TimeSpan.FromMinutes(ProjectTotalDuration);

        //                                     TaskObj.TotalDuration = timeSpan.ToString(@"hh\:mm");
        //                                 }
        //                             }
        //                         }
        //                         TaskObj.TimeRecordCount = mateTaskTimeRecordList.Count;
        //                     }
        //                 }
        //                 TaskObj.PageNo = requestModel.PageNumber;
        //             }
        //             employeeGroupTaskListResponses.Add(taskResponseObj);
        //         }
        //     }

        //     var employeeTaskListResponseList = _mapper.Map<List<EmployeeTaskListResponse>>(employeeTaskList);
        //     return new OperationResult<List<EmployeeGroupTaskListResponse>>(true, System.Net.HttpStatusCode.OK, "", employeeGroupTaskListResponses);
        // }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<EmployeeGroupTaskListResponse>> GroupTaskList([FromBody] EmployeeGroupTaskListRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            EmployeeGroupTaskListResponse employeeGroupTaskResponseObj = new EmployeeGroupTaskListResponse();

            var SkipValue = requestModel.PageSize * (requestModel.PageNumber - 1);
            var employeeTaskList = _employeeTaskService.GetByTenantWithoutProject(TenantId);

            // if (requestModel.PageNumber > 1)
            // {
            //     //if (!string.IsNullOrEmpty(model.CategoryId))
            //     if (requestModel.StatusId != null && requestModel.StatusId > 0)
            //     {
            //         employeeTaskList = employeeTaskList.Where(t => t?.StatusId == requestModel.StatusId).ToList();
            //     }
            // }
            int totalCount = 0;
            if (employeeTaskList != null && employeeTaskList.Count > 0)
            {
                List<EmployeeGroupTask> statusTaskList = new List<EmployeeGroupTask>();
                var AllStatus = _statusService.GetAll();
                var AllUsers = _userService.GetAll();
                statusTaskList.Add(new EmployeeGroupTask { StatusId = null, StatusName = "All Tasks", StatusColor = "Yellow", TotalCount = employeeTaskList.Count() });
                var TaskKeyValueData = employeeTaskList.GroupBy(t => t?.StatusId);
                foreach (var TaskData in TaskKeyValueData)
                {
                    EmployeeGroupTask taskResponseObj = new EmployeeGroupTask();
                    taskResponseObj.TotalCount = TaskData.ToList().Count();
                    if (TaskData.Key != null)
                    {
                        var StatusObj = AllStatus.Where(t => t.Id == TaskData.Key).FirstOrDefault();
                        if (StatusObj != null)
                        {
                            taskResponseObj.StatusId = StatusObj.Id;
                            taskResponseObj.StatusName = StatusObj.Name;
                            taskResponseObj.StatusColor = StatusObj.Color;
                        }
                        statusTaskList.Add(taskResponseObj);
                    }
                }
                totalCount = statusTaskList.Count();
                if (!string.IsNullOrEmpty(requestModel.SearchString))
                {
                    statusTaskList = statusTaskList.Where(t => t.StatusName != null && t.StatusName.ToLower().Contains(requestModel.SearchString.ToLower())).ToList();
                    statusTaskList = statusTaskList.Skip(SkipValue).Take(requestModel.PageSize).OrderBy(t => t.StatusId).ToList();
                }
                else
                {
                    statusTaskList = statusTaskList.Skip(SkipValue).Take(requestModel.PageSize).OrderBy(t => t.StatusId).ToList();
                }
                employeeGroupTaskResponseObj.StatusList = statusTaskList;
            }

            //var employeeTaskListResponseList = _mapper.Map<List<EmployeeTaskListResponse>>(employeeTaskList);
            return new OperationResult<EmployeeGroupTaskListResponse>(true, System.Net.HttpStatusCode.OK, "", employeeGroupTaskResponseObj, totalCount);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<List<EmployeeTaskListResponse>>> List([FromBody] EmployeeTaskListRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            //List<EmployeeTask> employeeTaskList = new List<EmployeeTask>();

            //employeeTaskList = _employeeTaskService.GetAllTaskByTenant(TenantId, model);
            var AllStatus = _statusService.GetAll();
            var AllUsers = _userService.GetAll();

            int totalCount = 0;
            List<EmployeeTaskListResponse> taskResponseList = new List<EmployeeTaskListResponse>();
            var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();


            //start for status wise ticket
            if (requestModel.StatusId != null)
            {
                List<EmployeeTask> statusTaskList = new List<EmployeeTask>();
                statusTaskList = _employeeTaskService.GetAllByStatusId(TenantId, requestModel.StatusId.Value);
                totalCount = statusTaskList.Count();
                if (statusTaskList != null && statusTaskList.Count() > 0)
                {
                    foreach (var statusItem in statusTaskList)
                    {
                        EmployeeTaskListResponse statusTaskListObj = new EmployeeTaskListResponse();
                        statusTaskListObj.Id = statusItem.Id;
                        statusTaskListObj.TaskNo = statusItem.TaskNo;
                        statusTaskListObj.Name = statusItem.Name;
                        var projectTicketObj = _employeeProjectTaskService.GetByTaskId(statusItem.Id);
                        if (projectTicketObj != null)
                        {
                            statusTaskListObj.ProjectId = projectTicketObj.EmployeeProjectId;
                            statusTaskListObj.ProjectName = projectTicketObj.EmployeeProject?.Name;
                            if (projectTicketObj.EmployeeProject?.Logo != null)
                            {
                                statusTaskListObj.ProjectLogoURL = OneClappContext.CurrentURL + "Project/Logo/" + projectTicketObj.EmployeeProjectId + "?" + Timestamp;
                            }
                        }
                        if (statusItem.StatusId != null)
                        {
                            var statusObj = AllStatus.Where(t => t.Id == statusItem.StatusId).FirstOrDefault();
                            if (statusObj != null)
                            {
                                statusTaskListObj.StatusId = statusObj.Id;
                                statusTaskListObj.StatusName = statusObj.Name;
                                statusTaskListObj.StatusColor = statusObj.Color;
                            }
                        }
                        if (statusItem.EndDate != null && statusItem.EndDate < DateTime.UtcNow)
                        {
                            var overdueStatusObj = AllStatus.Where(t => t.Name.ToLower() == "overdue").FirstOrDefault();
                            statusTaskListObj.StatusId = overdueStatusObj?.Id;
                            statusTaskListObj.StatusName = overdueStatusObj?.Name;
                            statusTaskListObj.StatusColor = overdueStatusObj?.Color;

                        }
                        var employeeClientTaskObj = _employeeClientTaskService.GetByTaskId(statusItem.Id);
                        if (employeeClientTaskObj != null && employeeClientTaskObj.ClientId != null)
                        {
                            var clientObj = _clientService.GetById(employeeClientTaskObj.ClientId.Value);
                            if (clientObj != null)
                            {
                                statusTaskListObj.ClientId = clientObj.Id;
                                statusTaskListObj.ClientName = clientObj.FirstName + " " + clientObj.LastName;
                            }
                        }
                        statusTaskListObj.Date = statusItem.CreatedOn;
                        //Assign Users
                        var assignTaskUsers = _employeeTaskUserService.GetAssignUsersByEmployeeTask(statusItem.Id);
                        if (assignTaskUsers != null && assignTaskUsers.Count > 0)
                        {
                            var assignTaskUserVMList = _mapper.Map<List<EmployeeTaskUserRequestResponse>>(assignTaskUsers);
                            if (statusTaskListObj.AssignedUsers == null)
                            {
                                statusTaskListObj.AssignedUsers = new List<EmployeeTaskUserRequestResponse>();
                            }
                            foreach (var assignTaskUser in assignTaskUserVMList)
                            {
                                if (AllUsers != null)
                                {
                                    var userObj2 = AllUsers.Where(t => t.Id == assignTaskUser.UserId).FirstOrDefault();
                                    if (userObj2 != null)
                                    {
                                        assignTaskUser.AssignUserFirstName = userObj2.FirstName;
                                        assignTaskUser.AssignUserLastName = userObj2.LastName;
                                    }
                                }
                            }
                            statusTaskListObj.AssignedUsers = assignTaskUserVMList;
                        }
                        taskResponseList.Add(statusTaskListObj);
                    }
                }
            }
            //end for status wise ticket
            //start for All ticket
            if (requestModel.StatusId == null)
            {
                List<EmployeeTask> taskList = new List<EmployeeTask>();
                taskList = _employeeTaskService.GetByTenantWithoutProject(TenantId);
                totalCount = taskList.Count();
                if (taskList != null && taskList.Count() > 0)
                {
                    foreach (var item in taskList)
                    {
                        EmployeeTaskListResponse taskListObj = new EmployeeTaskListResponse();
                        taskListObj.Id = item.Id;
                        taskListObj.TaskNo = item.TaskNo;
                        taskListObj.Name = item.Name;
                        var projectTicketObj = _employeeProjectTaskService.GetByTaskId(item.Id);
                        if (projectTicketObj != null)
                        {
                            taskListObj.ProjectId = projectTicketObj.EmployeeProjectId;
                            taskListObj.ProjectName = projectTicketObj.EmployeeProject?.Name;
                            if (projectTicketObj.EmployeeProject?.Logo != null)
                            {
                                taskListObj.ProjectLogoURL = OneClappContext.CurrentURL + "Project/Logo/" + projectTicketObj.EmployeeProjectId + "?" + Timestamp;
                            }
                        }
                        if (item.StatusId != null)
                        {
                            var statusObj = AllStatus.Where(t => t.Id == item.StatusId).FirstOrDefault();
                            if (statusObj != null)
                            {
                                taskListObj.StatusId = statusObj.Id;
                                taskListObj.StatusName = statusObj.Name;
                                taskListObj.StatusColor = statusObj.Color;
                            }
                        }
                        if (item.EndDate != null && item.EndDate < DateTime.UtcNow)
                        {
                            var overdueStatusObj = AllStatus.Where(t => t.Name.ToLower() == "overdue").FirstOrDefault();
                            taskListObj.StatusId = overdueStatusObj?.Id;
                            taskListObj.StatusName = overdueStatusObj?.Name;
                            taskListObj.StatusColor = overdueStatusObj?.Color;

                        }
                        var employeeClientTaskObj = _employeeClientTaskService.GetByTaskId(item.Id);
                        if (employeeClientTaskObj != null && employeeClientTaskObj.ClientId != null)
                        {
                            var clientObj = _clientService.GetById(employeeClientTaskObj.ClientId.Value);
                            if (clientObj != null)
                            {
                                taskListObj.ClientId = clientObj.Id;
                                taskListObj.ClientName = clientObj.FirstName + " " + clientObj.LastName;
                            }
                        }
                        taskListObj.Date = item.CreatedOn;
                        //Assign Users
                        var assignTaskUsers = _employeeTaskUserService.GetAssignUsersByEmployeeTask(item.Id);
                        if (assignTaskUsers != null && assignTaskUsers.Count > 0)
                        {
                            var assignTaskUserVMList = _mapper.Map<List<EmployeeTaskUserRequestResponse>>(assignTaskUsers);
                            if (taskListObj.AssignedUsers == null)
                            {
                                taskListObj.AssignedUsers = new List<EmployeeTaskUserRequestResponse>();
                            }
                            foreach (var assignTaskUser in assignTaskUserVMList)
                            {
                                if (AllUsers != null)
                                {
                                    var userObj2 = AllUsers.Where(t => t.Id == assignTaskUser.UserId).FirstOrDefault();
                                    if (userObj2 != null)
                                    {
                                        assignTaskUser.AssignUserFirstName = userObj2.FirstName;
                                        assignTaskUser.AssignUserLastName = userObj2.LastName;
                                    }
                                }
                            }
                            taskListObj.AssignedUsers = assignTaskUserVMList;
                        }
                        taskResponseList.Add(taskListObj);
                    }
                }
            }
            //end for All ticket
            var SkipValue = requestModel.PageSize * (requestModel.PageNumber - 1);
            if (!string.IsNullOrEmpty(requestModel.SearchString))
            {
                taskResponseList = taskResponseList.Where(t => (!string.IsNullOrEmpty(t.Name) && t.Name.ToLower().Contains(requestModel.SearchString.ToLower()))
                                                                || (!string.IsNullOrEmpty(t.ProjectName) && t.ProjectName.ToLower().Contains(requestModel.SearchString.ToLower()))
                                                                || (!string.IsNullOrEmpty(t.StatusName) && t.StatusName.ToLower().Contains(requestModel.SearchString.ToLower()))
                                                                || (!string.IsNullOrEmpty(t.ClientName) && t.ClientName.ToLower().Contains(requestModel.SearchString.ToLower()))).ToList();
                taskResponseList = taskResponseList.Skip(SkipValue).Take(requestModel.PageSize).ToList();
            }
            else
            {
                taskResponseList = taskResponseList.Skip(SkipValue).Take(requestModel.PageSize).ToList();
            }
            taskResponseList = ShortTaskByColumn(requestModel.ShortColumnName, requestModel.SortType, taskResponseList);
            return new OperationResult<List<EmployeeTaskListResponse>>(true, System.Net.HttpStatusCode.OK, "", taskResponseList, totalCount);
        }

        private List<EmployeeTaskListResponse> ShortTaskByColumn(string ShortColumn, string ShortOrder, List<EmployeeTaskListResponse> taskList)
        {
            List<EmployeeTaskListResponse> taskVMList = new List<EmployeeTaskListResponse>();
            taskVMList = taskList;
            if (ShortColumn != "" && ShortColumn != null)
            {
                if (ShortColumn.ToLower() == "name")
                {
                    if (ShortOrder.ToLower() == "asc")
                    {
                        taskVMList = taskList.OrderBy(t => t.Name).ToList();
                    }
                    else
                    {
                        taskVMList = taskList.OrderByDescending(t => t.Name).ToList();
                    }
                }
                if (ShortColumn.ToLower() == "projectname")
                {
                    if (ShortOrder.ToLower() == "asc")
                    {
                        taskVMList = taskList.OrderBy(t => t.ProjectName).ToList();
                    }
                    else
                    {
                        taskVMList = taskList.OrderByDescending(t => t.ProjectName).ToList();
                    }
                }
                else if (ShortColumn.ToLower() == "date")
                {
                    if (ShortOrder.ToLower() == "asc")
                    {
                        taskVMList = taskList.OrderBy(t => t.Date).ToList();
                    }
                    else
                    {
                        taskVMList = taskList.OrderByDescending(t => t.Date).ToList();
                    }
                }
                else if (ShortColumn.ToLower() == "statusname")
                {
                    if (ShortOrder.ToLower() == "asc")
                    {
                        taskVMList = taskList.OrderBy(t => t?.StatusName).ToList();
                    }
                    else
                    {
                        taskVMList = taskList.OrderByDescending(t => t?.StatusName).ToList();
                    }
                }
                else if (ShortColumn.ToLower() == "clientname")
                {
                    if (ShortOrder.ToLower() == "asc")
                    {
                        taskVMList = taskList.OrderBy(t => t?.ClientName).ToList();
                    }
                    else
                    {
                        taskVMList = taskList.OrderByDescending(t => t?.ClientName).ToList();
                    }
                }
                else
                {
                    taskVMList = taskList.OrderByDescending(t => t.Date).ToList();
                }
            }
            return taskVMList;
        }

        // Get All Tasks
        // [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        // [HttpGet("{SearchString}")]
        // public async Task<OperationResult<EmployeeTaskListVM>> List(string SearchString)
        // {
        //     ClaimsPrincipal user = this.User as ClaimsPrincipal;
        //     UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
        //     TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

        //     EmployeeTaskListVM employeeTaskListVMObj = new EmployeeTaskListVM();
        //     EmployeeTaskDto employeeTaskDto = new EmployeeTaskDto();

        //     //var tasks = _employeeTaskService.GetAllActiveByTenant(TenantId);
        //     var tasks = _employeeTaskService.GetAllActiveTaskByTenant(TenantId, SearchString, employeeTaskDto);
        //     if (tasks.Count() == 0)
        //     {
        //         return new OperationResult<EmployeeTaskListVM>(true, System.Net.HttpStatusCode.OK, "", employeeTaskListVMObj);
        //     }

        //     var taskIdList = tasks.Select(t => t.Id).ToList();
        //     employeeTaskListVMObj.Tasks = new List<EmployeeTaskVM>();

        //     if (tasks != null && tasks.Count() > 0)
        //     {
        //         foreach (var taskObj in tasks)
        //         {
        //             var employeeTaskVMObj = _mapper.Map<EmployeeTaskVM>(taskObj);
        //             if (employeeTaskVMObj.Id != null)
        //             {
        //                 var taskTotalDuration = _employeeTaskTimeRecordService.GetTotalEmployeeTaskTimeRecord(employeeTaskVMObj.Id.Value);
        //                 employeeTaskVMObj.Duration = taskTotalDuration;

        //                 var y = 60 * 60 * 1000;
        //                 var h = taskTotalDuration / y;
        //                 var m = (taskTotalDuration - (h * y)) / (y / 60);
        //                 var s = (taskTotalDuration - (h * y) - (m * (y / 60))) / 1000;
        //                 var mi = taskTotalDuration - (h * y) - (m * (y / 60)) - (s * 1000);

        //                 employeeTaskVMObj.Seconds = s;
        //                 employeeTaskVMObj.Minutes = m;
        //                 employeeTaskVMObj.Hours = h;
        //             }
        //             var assignTaskUsers = _employeeTaskUserService.GetAssignUsersByEmployeeTask(taskObj.Id);
        //             if (assignTaskUsers.Count > 0)
        //             {
        //                 employeeTaskVMObj.AssignedUsers = new List<EmployeeTaskUserDto>();
        //                 var assignUsersVMList = _mapper.Map<List<EmployeeTaskUserDto>>(assignTaskUsers);
        //                 employeeTaskVMObj.AssignedUsers = assignUsersVMList;
        //             }
        //             employeeTaskListVMObj.Tasks.Add(employeeTaskVMObj);
        //         }
        //     }

        //     return new OperationResult<EmployeeTaskListVM>(true, System.Net.HttpStatusCode.OK, "", employeeTaskListVMObj);
        // }

        // Assign task to users
        [Authorize(Roles = "Manager,TenantAdmin")]
        [HttpPost]
        public async Task<OperationResult<EmployeeTaskUser>> AssignToCustomer([FromBody] EmployeeTaskUserDto employeeTaskUser)
        {
            var assignEmployeeTaskUserObj = await _employeeTaskUserService.CheckInsertOrUpdate(employeeTaskUser);
            return new OperationResult<EmployeeTaskUser>(true, System.Net.HttpStatusCode.OK, "", assignEmployeeTaskUserObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<EmployeeTaskTimeRecordResponse>> TimeRecord([FromBody] AddUpdateEmployeeTaskTimeRecordRequest Model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            EmployeeTaskTimeRecord employeeTaskTimeRecordObj = new EmployeeTaskTimeRecord();
            EmployeeTaskTimeRecordResponse employeeTaskTimeRecordResponse = new EmployeeTaskTimeRecordResponse();
            if (Model.Duration != null && Model.EmployeeTaskId != null)
            {
                var taskTotalDuration = _employeeTaskTimeRecordService.GetTotalEmployeeTaskTimeRecord(Model.EmployeeTaskId.Value);
                if (taskTotalDuration >= 0)
                {
                    Model.Duration = Model.Duration - taskTotalDuration;
                }
                var employeeTaskTimeRecordDto = _mapper.Map<EmployeeTaskTimeRecordDto>(Model);
                employeeTaskTimeRecordDto.UserId = UserId;
                employeeTaskTimeRecordObj = await _employeeTaskTimeRecordService.CheckInsertOrUpdate(employeeTaskTimeRecordDto);
                EmployeeTaskActivity employeeTaskActivityObj = new EmployeeTaskActivity();
                employeeTaskActivityObj.EmployeeTaskId = employeeTaskTimeRecordObj.EmployeeTaskId;
                employeeTaskActivityObj.UserId = UserId;
                employeeTaskActivityObj.Activity = "Created time record";
                employeeTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Time_record_added.ToString().Replace("_", " ");
                var AddUpdate1 = await _employeeTaskActivityService.CheckInsertOrUpdate(employeeTaskActivityObj);

                employeeTaskTimeRecordResponse = _mapper.Map<EmployeeTaskTimeRecordResponse>(employeeTaskTimeRecordObj);
                await _hubContext.Clients.All.OnEmployeeTaskModuleEvent(employeeTaskTimeRecordObj.EmployeeTaskId, TenantId);
                return new OperationResult<EmployeeTaskTimeRecordResponse>(true, System.Net.HttpStatusCode.OK, "", employeeTaskTimeRecordResponse);
            }
            else
            {
                var message = "EmployeeTaskId can not be null";
                if (Model.Duration == null)
                {
                    message = "Duration can not be null";
                }
                return new OperationResult<EmployeeTaskTimeRecordResponse>(false, System.Net.HttpStatusCode.OK, message, employeeTaskTimeRecordResponse);
            }

        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        // [AllowAnonymous]
        [HttpPost]
        public async Task<OperationResult<List<EmployeeTaskAttachment>>> UploadFiles([FromForm] EmployeeTaskAttachmentRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            List<EmployeeTaskAttachment> employeeTaskAttachmentList = new List<EmployeeTaskAttachment>();

            if (model.FileList == null) throw new Exception("File is null");
            if (model.FileList.Length == 0) throw new Exception("File is empty");

            EmployeeTaskActivity employeeTaskActivityObj = new EmployeeTaskActivity();
            employeeTaskActivityObj.EmployeeTaskId = model.EmployeeTaskId;
            employeeTaskActivityObj.UserId = UserId;
            employeeTaskActivityObj.Activity = "Uploaded document.";

            foreach (IFormFile file in model.FileList)
            {
                // full path to file in temp location
                //var dirPath = _hostingEnvironment.WebRootPath + "\\EmployeeTaskUpload";
                var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.EmployeeTaskUploadDirPath;

                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                var fileName = string.Concat(
                    Path.GetFileNameWithoutExtension(file.FileName + "_" + model.EmployeeTaskId),
                    DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                    Path.GetExtension(file.FileName)
                );
                var filePath = dirPath + "\\" + fileName;

                if (OneClappContext.ClamAVServerIsLive)
                {
                    ScanDocument scanDocumentObj = new ScanDocument();
                    bool fileStatus = await scanDocumentObj.ScanDocumentWithClam(file);
                    if (fileStatus)
                    {
                        return new OperationResult<List<EmployeeTaskAttachment>>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                    }
                }

                using (var oStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    await file.CopyToAsync(oStream);
                }

                EmployeeTaskAttachmentDto employeeTaskAttachmentDto = new EmployeeTaskAttachmentDto();
                employeeTaskAttachmentDto.Name = fileName;
                employeeTaskAttachmentDto.EmployeeTaskId = model.EmployeeTaskId;
                employeeTaskAttachmentDto.UserId = UserId;
                var addedItem = await _employeeTaskAttachmentService.CheckInsertOrUpdate(employeeTaskAttachmentDto);
                employeeTaskAttachmentList.Add(addedItem);
            }
            var AddUpdate1 = await _employeeTaskActivityService.CheckInsertOrUpdate(employeeTaskActivityObj);

            await _hubContext.Clients.All.OnUploadEmployeeTaskDocumentEventEmit(model.EmployeeTaskId);

            return new OperationResult<List<EmployeeTaskAttachment>>(true, System.Net.HttpStatusCode.OK, "", employeeTaskAttachmentList);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        // [AllowAnonymous]
        [HttpGet("{EmployeeTaskId}")]
        public async Task<OperationResult<List<EmployeeTaskAttachment>>> Documents(long EmployeeTaskId)
        {

            List<EmployeeTaskAttachment> employeeTaskAttachmentList = new List<EmployeeTaskAttachment>();
            employeeTaskAttachmentList = _employeeTaskAttachmentService.GetAllByEmployeeTaskId(EmployeeTaskId);

            return new OperationResult<List<EmployeeTaskAttachment>>(true, System.Net.HttpStatusCode.OK, "", employeeTaskAttachmentList);
        }

        [AllowAnonymous]
        [HttpGet("{TaskAttachmentId}")]
        public async Task<OperationResult<string>> Document(long TaskAttachmentId)
        {
            var taskAttachmentObj = _employeeTaskAttachmentService.GetEmployeeTaskAttachmentById(TaskAttachmentId);

            //var dirPath = _hostingEnvironment.WebRootPath + "\\EmployeeTaskUpload";
            var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.EmployeeTaskUploadDirPath;
            var filePath = dirPath + "\\" + taskAttachmentObj.Name;
            Byte[] newBytes = System.IO.File.ReadAllBytes(filePath);
            String file = Convert.ToBase64String(newBytes);
            if (file != "")
            {
                return new OperationResult<string>(true, System.Net.HttpStatusCode.OK, "File received successfully", file);
            }
            else
            {
                return new OperationResult<string>(false, System.Net.HttpStatusCode.OK, "Issue in downloading file.");
            }

        }

        // Get Task Detail by Id
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{EmployeeTaskId}")]
        public async Task<OperationResult<EmployeeTaskDetailResponse>> Detail(long EmployeeTaskId)
        {
            EmployeeTaskDetailResponse employeeTaskObj = new EmployeeTaskDetailResponse();

            var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

            var AllClients = _clientService.GetAll();
            var AllUsers = _userService.GetAll();
            var taskObj = _employeeTaskService.GetTaskById(EmployeeTaskId);
            if (taskObj != null)
            {
                var employeeClientTaskObj = _employeeClientTaskService.GetByTaskId(taskObj.Id);

                employeeTaskObj = _mapper.Map<EmployeeTaskDetailResponse>(taskObj);
                employeeTaskObj.ClientId = employeeClientTaskObj?.ClientId;
                //for client
                var clientObj = AllClients.Where(t => t.Id == employeeTaskObj.ClientId).FirstOrDefault();
                if (clientObj != null)
                {
                    if (clientObj.FirstName != null && clientObj.LastName != null)
                    {
                        employeeTaskObj.ClientName = clientObj.FirstName + " " + clientObj.LastName;
                    }
                }
                if (employeeTaskObj.StatusId != null)
                {
                    employeeTaskObj.StatusName = taskObj.Status.Name;
                }
                if (employeeTaskObj.MatePriorityId != null)
                {
                    employeeTaskObj.MatePriorityName = taskObj.MatePriority?.Name;
                }

                //for subtasks            
                var subTasks = _employeeSubTaskService.GetAllSubTaskByTask(EmployeeTaskId);
                foreach (var item in subTasks)
                {
                    long SubtaskId = item.Id;
                    EmployeeTaskDetailSubTask employeeSubTaskObj = new EmployeeTaskDetailSubTask();
                    employeeSubTaskObj = _mapper.Map<EmployeeTaskDetailSubTask>(item);
                    var subTaskAssignUsers = _employeeSubTaskUserService.GetAssignUsersBySubTask(SubtaskId);
                    if (subTaskAssignUsers != null && subTaskAssignUsers.Count > 0)
                    {
                        //List<EmployeeTaskDetailSubTaskUser> assignSubTaskUserVMList = new List<EmployeeTaskDetailSubTaskUser>();

                        if (employeeSubTaskObj.AssignedUsers == null)
                        {
                            employeeSubTaskObj.AssignedUsers = new List<EmployeeTaskDetailSubTaskUser>();
                        }

                        foreach (var assignSubTaskUserObj in subTaskAssignUsers)
                        {
                            EmployeeTaskDetailSubTaskUser subTaskUserObj = new EmployeeTaskDetailSubTaskUser();
                            subTaskUserObj.Id = assignSubTaskUserObj.Id;
                            if (AllUsers != null)
                            {
                                var userObj = AllUsers.Where(t => t.Id == assignSubTaskUserObj.UserId).FirstOrDefault();
                                if (userObj != null)
                                {
                                    subTaskUserObj.UserId = assignSubTaskUserObj.UserId;
                                    if (userObj.ProfileImage != null)
                                    {
                                        subTaskUserObj.ProfileURL = OneClappContext.CurrentURL + "User/ProfileImageView/" + subTaskUserObj.UserId + "?" + Timestamp;
                                    }
                                    else
                                    {
                                        subTaskUserObj.ProfileURL = null;
                                    }
                                }
                            }
                            employeeSubTaskObj.AssignedUsers.Add(subTaskUserObj);
                        }
                    }
                    employeeTaskObj.SubTasks.Add(employeeSubTaskObj);
                }

                // For task assign users
                var assignUsers = _employeeTaskUserService.GetAssignUsersByEmployeeTask(EmployeeTaskId);
                if (assignUsers != null && assignUsers.Count > 0)
                {
                    //var assignTaskUserVMList = _mapper.Map<List<EmployeeTaskUserDetailResponse>>(assignUsers);
                    List<EmployeeTaskUserDetailResponse> assignTaskUserVMList = new List<EmployeeTaskUserDetailResponse>();
                    if (employeeTaskObj.AssignedUsers == null)
                    {
                        employeeTaskObj.AssignedUsers = new List<EmployeeTaskUserDetailResponse>();
                    }
                    if (assignUsers != null && assignUsers.Count() > 0)
                    {
                        foreach (var assignUserObj in assignUsers)
                        {
                            EmployeeTaskUserDetailResponse taskUserObj = new EmployeeTaskUserDetailResponse();
                            taskUserObj.Id = assignUserObj.Id;
                            if (AllUsers != null)
                            {
                                var userObj = AllUsers.Where(t => t.Id == assignUserObj.UserId).FirstOrDefault();
                                if (userObj != null)
                                {
                                    taskUserObj.UserId = assignUserObj.UserId;
                                    if (userObj.ProfileImage != null)
                                    {
                                        taskUserObj.ProfileURL = OneClappContext.CurrentURL + "User/ProfileImageView/" + assignUserObj.UserId + "?" + Timestamp;
                                    }
                                    else
                                    {
                                        taskUserObj.ProfileURL = null;
                                    }
                                }
                            }
                            assignTaskUserVMList.Add(taskUserObj);
                        }
                    }
                    employeeTaskObj.AssignedUsers = assignTaskUserVMList;

                }

                // For mate task comments
                var mateTaskCommentList = _mateTaskCommentService.GetByTaskId(EmployeeTaskId);
                if (mateTaskCommentList != null && mateTaskCommentList.Count > 0)
                {
                    if (mateTaskCommentList != null && mateTaskCommentList.Count() > 0)
                    {
                        foreach (var taskItem in mateTaskCommentList)
                        {
                            EmployeeTaskDetailMateComment mateTaskCommentResponseObj = new EmployeeTaskDetailMateComment();
                            if (AllUsers != null && AllUsers.Count() > 0)
                            {
                                var userObj = AllUsers.Where(t => t.Id == taskItem.MateComment?.UserId).FirstOrDefault();
                                if (userObj != null)
                                {
                                    mateTaskCommentResponseObj.UserId = taskItem.MateComment?.UserId;
                                    string UserName = userObj.FirstName + " " + userObj.LastName;
                                    mateTaskCommentResponseObj.UserName = UserName;

                                }
                            }
                            mateTaskCommentResponseObj.Id = taskItem.MateComment.Id;
                            mateTaskCommentResponseObj.CreatedOn = taskItem.MateComment?.CreatedOn;
                            mateTaskCommentResponseObj.Comment = taskItem.MateComment?.Comment;
                            mateTaskCommentResponseObj.MateReplyCommentId = taskItem.MateComment?.MateReplyCommentId;
                            //Attachment
                            if (taskItem.MateCommentId != null)
                            {
                                var taskAttachments = _mateCommentAttachmentService.GetByMateCommentId(taskItem.MateCommentId.Value);
                                if (taskAttachments != null && taskAttachments.Count > 0)
                                {
                                    EmployeeTaskDetailMateCommentAttachment mateCommentTaskAttachmentObj = new EmployeeTaskDetailMateCommentAttachment();
                                    //var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                                    foreach (var taskfile in taskAttachments)
                                    {
                                        mateCommentTaskAttachmentObj.Name = taskfile.Name;
                                        if (taskfile.Name == null)
                                        {
                                            mateCommentTaskAttachmentObj.URL = null;
                                        }
                                        else
                                        {
                                            mateCommentTaskAttachmentObj.URL = OneClappContext.CurrentURL + "MateComment/Attachment/" + taskfile.Id + "?" + Timestamp;
                                        }
                                        mateTaskCommentResponseObj.Attachments.Add(mateCommentTaskAttachmentObj);
                                    }
                                }
                            }
                            //Attachment
                            employeeTaskObj.Comments.Add(mateTaskCommentResponseObj);
                        }
                    }
                }
                // For mate task comments

                // For task activities
                var employeeTaskActivities = _employeeTaskActivityService.GetAllByEmployeeTaskId(EmployeeTaskId);
                if (employeeTaskActivities != null && employeeTaskActivities.Count > 0)
                {
                    var employeeTaskActivitiesVMList = _mapper.Map<List<EmployeeTaskDetailActivityResponse>>(employeeTaskActivities);
                    if (employeeTaskActivitiesVMList != null && employeeTaskActivitiesVMList.Count() > 0)
                    {
                        foreach (var employeeTaskActivityObj in employeeTaskActivitiesVMList)
                        {
                            if (AllUsers != null)
                            {
                                var userObjAct = AllUsers.Where(t => t.Id == employeeTaskActivityObj.UserId).FirstOrDefault();
                                if (userObjAct != null)
                                {
                                    employeeTaskActivityObj.UserName = userObjAct.FirstName + " " + userObjAct.LastName;
                                    if (userObjAct.ProfileImage != null)
                                    {
                                        employeeTaskActivityObj.ProfileURL = OneClappContext.CurrentURL + "User/ProfileImageView/" + employeeTaskActivityObj.UserId + "?" + Timestamp;
                                    }
                                }
                            }
                        }
                    }
                    if (employeeTaskObj.Activities == null)
                    {
                        employeeTaskObj.Activities = new List<EmployeeTaskDetailActivityResponse>();
                    }
                    employeeTaskObj.Activities = employeeTaskActivitiesVMList;
                }
            }

            return new OperationResult<EmployeeTaskDetailResponse>(true, System.Net.HttpStatusCode.OK, "", employeeTaskObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete("{EmployeeTaskId}")]
        public async Task<OperationResult<EmployeeTaskDeleteResponse>> Remove(long EmployeeTaskId)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            EmployeeTaskDto model = new EmployeeTaskDto();

            var employeeTaskId = EmployeeTaskId;

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

                            //for comment and document
                            var mateChildCommentIdList = _mateChildTaskCommentService.GetByChildTaskId(childTaskId).Select(t => t.MateCommentId).ToList();
                            if (mateChildCommentIdList != null && mateChildCommentIdList.Count > 0)
                            {
                                foreach (var childitem in mateChildCommentIdList)
                                {
                                    if (childitem != null)
                                    {
                                        var childAttachments = await _mateCommentAttachmentService.DeleteByMateCommentId(childitem.Value);
                                        foreach (var childDoc in childAttachments)
                                        {
                                            var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.EmployeeTaskUploadDirPath;
                                            var filePath = dirPath + "\\" + childDoc.Name;

                                            if (System.IO.File.Exists(filePath))
                                            {
                                                System.IO.File.Delete(Path.Combine(filePath));
                                            }
                                        }
                                        var mateCommentObj = await _mateCommentService.DeleteMateComment(childitem.Value);
                                        if (mateCommentObj != null)
                                        {
                                            EmployeeChildTaskActivity childCommentActivityObj = new EmployeeChildTaskActivity();
                                            childCommentActivityObj.EmployeeChildTaskId = childTaskId;
                                            childCommentActivityObj.UserId = UserId;
                                            childCommentActivityObj.Activity = Enums.EmployeeChidTaskActivityEnum.Childtask_comment_removed.ToString().Replace("_", " ");
                                            var AddUpdateChildCommentActivity = await _employeeChildTaskActivityService.CheckInsertOrUpdate(childCommentActivityObj);
                                        }
                                    }
                                }
                            }
                            //for comment and document


                            //var childComments = await _employeeChildTaskCommentService.DeleteCommentByChildTaskId(childTaskId);

                            //var childTimeRecords = await _employeeChildTaskTimeRecordService.DeleteTimeRecordByEmployeeChildTaskId(childTaskId);

                            //for delete childtask time record
                            var mateChlidTaskTimeRecordList = _mateChildTaskTimeRecordService.GetRecordByChildTaskId(childTaskId);
                            if (mateChlidTaskTimeRecordList != null && mateChlidTaskTimeRecordList.Count > 0)
                            {
                                foreach (var childTimeRecord in mateChlidTaskTimeRecordList)
                                {
                                    if (childTimeRecord != null && childTimeRecord.MateTimeRecordId != null)
                                    {
                                        var mateTimeRecordObj = await _mateTimeRecordService.DeleteMateTimeRecord(childTimeRecord.MateTimeRecordId.Value);
                                        //childtask time record activity
                                        if (mateTimeRecordObj != null)
                                        {
                                            EmployeeChildTaskActivity childTaskTimeRecordActivityObj = new EmployeeChildTaskActivity();
                                            childTaskTimeRecordActivityObj.EmployeeChildTaskId = childTimeRecord.ChildTaskId;
                                            childTaskTimeRecordActivityObj.UserId = UserId;
                                            childTaskTimeRecordActivityObj.Activity = Enums.EmployeeChidTaskActivityEnum.Childtask_time_record_removed.ToString().Replace("_", " ");
                                            var AddUpdateChildTaskTimeRecordActivity = await _employeeChildTaskActivityService.CheckInsertOrUpdate(childTaskTimeRecordActivityObj);
                                        }
                                        //childtask time record activity
                                    }
                                }
                            }
                            //for delete childtask record

                            var childTaskUsers = await _employeeChildTaskUserService.DeleteByChildTaskId(childTaskId);

                            //var childTaskActivities = await _employeeChildTaskActivityService.DeleteByEmployeeChildTaskId(childTaskId);

                            var childTaskToDelete = await _employeeChildTaskService.Delete(childTaskId);

                            EmployeeChildTaskActivity employeeChildTaskActivityObj = new EmployeeChildTaskActivity();
                            employeeChildTaskActivityObj.EmployeeChildTaskId = childTaskId;
                            employeeChildTaskActivityObj.UserId = UserId;
                            employeeChildTaskActivityObj.Activity = Enums.EmployeeChidTaskActivityEnum.Childtask_Removed.ToString().Replace("_", " ");
                            var AddUpdate1 = await _employeeChildTaskActivityService.CheckInsertOrUpdate(employeeChildTaskActivityObj);
                        }
                    }

                    //for comment and document
                    var mateSubCommentIdList = _mateSubTaskCommentService.GetBySubTaskId(subTaskId).Select(t => t.MateCommentId).ToList();
                    if (mateSubCommentIdList != null && mateSubCommentIdList.Count > 0)
                    {
                        foreach (var subitem in mateSubCommentIdList)
                        {
                            if (subitem != null)
                            {
                                var SubAttachments = await _mateCommentAttachmentService.DeleteByMateCommentId(subitem.Value);
                                foreach (var subDoc in SubAttachments)
                                {
                                    var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.EmployeeTaskUploadDirPath;
                                    var filePath = dirPath + "\\" + subDoc.Name;

                                    if (System.IO.File.Exists(filePath))
                                    {
                                        System.IO.File.Delete(Path.Combine(filePath));
                                    }
                                }
                                var mateCommentObj = await _mateCommentService.DeleteMateComment(subitem.Value);
                                if (mateCommentObj != null)
                                {
                                    EmployeeSubTaskActivity subCommentActivityObj = new EmployeeSubTaskActivity();
                                    subCommentActivityObj.EmployeeSubTaskId = subTaskId;
                                    subCommentActivityObj.UserId = UserId;
                                    subCommentActivityObj.Activity = Enums.EmployeeSubTaskActivityEnum.Subtask_comment_removed.ToString().Replace("_", " ");
                                    var AddUpdateSubCommentActivity = await _employeeSubTaskActivityService.CheckInsertOrUpdate(subCommentActivityObj);
                                }
                            }
                        }
                    }
                    //for comment and document

                    //var subComments = await _employeeSubTaskCommentService.DeleteCommentByEmployeeSubTaskId(subTaskId);

                    //var subTimeRecords = await _employeeSubTaskTimeRecordService.DeleteTimeRecordBySubTaskId(subTaskId);

                    //for delete subtask time record
                    var mateSubTaskTimeRecordList = _mateSubTaskTimeRecordService.GetRecordBySubTaskId(subTaskId);
                    if (mateSubTaskTimeRecordList != null && mateSubTaskTimeRecordList.Count > 0)
                    {
                        foreach (var subTimeRecord in mateSubTaskTimeRecordList)
                        {
                            if (subTimeRecord != null && subTimeRecord.MateTimeRecordId != null)
                            {
                                var mateTimeRecordObj = await _mateTimeRecordService.DeleteMateTimeRecord(subTimeRecord.MateTimeRecordId.Value);
                                //subtask time record activity
                                if (mateTimeRecordObj != null)
                                {
                                    EmployeeSubTaskActivity subTaskTimeRecordActivityObj = new EmployeeSubTaskActivity();
                                    subTaskTimeRecordActivityObj.EmployeeSubTaskId = subTimeRecord.SubTaskId;
                                    subTaskTimeRecordActivityObj.UserId = UserId;
                                    subTaskTimeRecordActivityObj.Activity = Enums.EmployeeSubTaskActivityEnum.Subtask_time_record_removed.ToString().Replace("_", " ");
                                    var AddUpdateSubTaskTimeRecordActivity = await _employeeSubTaskActivityService.CheckInsertOrUpdate(subTaskTimeRecordActivityObj);
                                }
                                //subtask time record activity
                            }
                        }
                    }
                    //for delete subtask record


                    var subTaskUsers = await _employeeSubTaskUserService.DeleteBySubTaskId(subTaskId);

                    var subTaskToDelete = await _employeeSubTaskService.Delete(subTaskId);
                    EmployeeSubTaskActivity employeeSubTaskActivityObj = new EmployeeSubTaskActivity();
                    employeeSubTaskActivityObj.EmployeeSubTaskId = subTaskId;
                    employeeSubTaskActivityObj.UserId = UserId;
                    employeeSubTaskActivityObj.Activity = Enums.EmployeeSubTaskActivityEnum.Subtask_Removed.ToString().Replace("_", " ");
                    var AddUpdate2 = await _employeeSubTaskActivityService.CheckInsertOrUpdate(employeeSubTaskActivityObj);

                }
            }

            //task comment and attchament
            var mateCommentIdList = _mateTaskCommentService.GetByTaskId(employeeTaskId).Select(t => t.MateCommentId).ToList();
            if (mateCommentIdList != null && mateCommentIdList.Count > 0)
            {
                foreach (var item in mateCommentIdList)
                {
                    if (item != null)
                    {
                        var mateCommentAttachments = await _mateCommentAttachmentService.DeleteByMateCommentId(item.Value);
                        foreach (var taskDoc in mateCommentAttachments)
                        {
                            var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.MateCommentUploadDirPath;
                            var filePath = dirPath + "\\" + taskDoc.Name;

                            if (System.IO.File.Exists(filePath))
                            {
                                System.IO.File.Delete(Path.Combine(filePath));
                            }
                        }
                        var mateCommentObj = await _mateCommentService.DeleteMateComment(item.Value);
                        if (mateCommentObj != null)
                        {
                            EmployeeTaskActivity taskCommentActivityObj = new EmployeeTaskActivity();
                            taskCommentActivityObj.EmployeeTaskId = employeeTaskId;
                            taskCommentActivityObj.UserId = UserId;
                            taskCommentActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_comment_removed.ToString().Replace("_", " ");
                            var AddUpdateActivity = await _employeeTaskActivityService.CheckInsertOrUpdate(taskCommentActivityObj);
                        }
                    }
                }
            }
            //task comment and attchament            

            var taskUsers = await _employeeTaskUserService.DeleteByEmployeeTaskId(employeeTaskId);

            //for delete task time record
            var mateTaskTimeRecordList = _mateTaskTimeRecordService.GetByTaskId(employeeTaskId);
            if (mateTaskTimeRecordList != null && mateTaskTimeRecordList.Count > 0)
            {
                foreach (var taskTimeRecord in mateTaskTimeRecordList)
                {
                    if (taskTimeRecord != null && taskTimeRecord.MateTimeRecordId != null)
                    {
                        var mateTimeRecordObj = await _mateTimeRecordService.DeleteMateTimeRecord(taskTimeRecord.MateTimeRecordId.Value);
                        //task time record activity
                        if (mateTimeRecordObj != null)
                        {
                            EmployeeTaskActivity taskTimeRecordActivityObj = new EmployeeTaskActivity();
                            taskTimeRecordActivityObj.EmployeeTaskId = taskTimeRecord.TaskId;
                            taskTimeRecordActivityObj.UserId = UserId;
                            taskTimeRecordActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_time_record_removed.ToString().Replace("_", " ");
                            var AddUpdateActivity = await _employeeTaskActivityService.CheckInsertOrUpdate(taskTimeRecordActivityObj);
                        }
                        //task time record activity
                    }
                }
            }
            //for delete task time record 

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

            //for EmployeeProjectTask
            var employeeProjectTaskObj = await _employeeProjectTaskService.DeleteByTaskId(employeeTaskId);
            if (employeeProjectTaskObj != null)
            {
                EmployeeTaskActivity projectTaskActivityObj = new EmployeeTaskActivity();
                projectTaskActivityObj.EmployeeTaskId = employeeTaskId;
                projectTaskActivityObj.UserId = UserId;
                projectTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Unassign_project_from_task.ToString().Replace("_", " ");
                var AddUpdateProjectTask = await _employeeTaskActivityService.CheckInsertOrUpdate(projectTaskActivityObj);
            }

            //Ticket task
            var mateTicketTaskObj = await _mateTicketTaskService.DeleteByTaskId(employeeTaskId);
            if (mateTicketTaskObj != null)
            {
                MateTicketActivity mateTicketTaskActivityObj = new MateTicketActivity();
                mateTicketTaskActivityObj.EmployeeTaskId = employeeTaskId;
                mateTicketTaskActivityObj.MateTicketId = mateTicketTaskObj.MateTicketId;
                mateTicketTaskActivityObj.CreatedBy = UserId;
                mateTicketTaskActivityObj.Activity = Enums.MateTicketActivityEnum.Task_removed_from_ticket.ToString().Replace("_", " ");
                var AddUpdateTicketTaskActivity = await _mateTicketActivityService.CheckInsertOrUpdate(mateTicketTaskActivityObj);
                await _hubContext.Clients.All.OnMateTicketModuleEvent(mateTicketTaskObj.MateTicketId, TenantId);
            }

            var taskToDelete = await _employeeTaskService.Delete(employeeTaskId);

            EmployeeTaskActivity employeeTaskActivityObj = new EmployeeTaskActivity();
            employeeTaskActivityObj.EmployeeTaskId = employeeTaskId;
            employeeTaskActivityObj.UserId = UserId;
            employeeTaskActivityObj.Activity = "Removed the task";
            var AddUpdate = await _employeeTaskActivityService.CheckInsertOrUpdate(employeeTaskActivityObj);

            var responsemodel = _mapper.Map<EmployeeTaskDeleteResponse>(model);
            await _hubContext.Clients.All.OnEmployeeTaskModuleEvent(employeeTaskId, TenantId);
            return new OperationResult<EmployeeTaskDeleteResponse>(true, System.Net.HttpStatusCode.OK, "Task deleted successfully", responsemodel);

        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{EmployeeTaskId}")]
        public async Task<OperationResult<List<EmployeeTaskHistoryResponse>>> History(long EmployeeTaskId)
        {
            List<EmployeeTaskHistoryResponse> activityList = new List<EmployeeTaskHistoryResponse>();
            var AllUsers = _userService.GetAll();
            var activities = _employeeTaskActivityService.GetAllByEmployeeTaskId(EmployeeTaskId);
            if (activities != null && activities.Count() > 0)
            {
                foreach (var item in activities)
                {
                    EmployeeTaskHistoryResponse activityObj = new EmployeeTaskHistoryResponse();
                    var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                    activityObj.Id = item.Id;
                    activityObj.Activity = item.Activity;
                    activityObj.UserId = item.UserId;
                    var userObj = AllUsers.Where(t => t.Id == item.UserId).FirstOrDefault();
                    if (userObj != null)
                    {
                        if (userObj.FirstName != null && userObj.LastName != null)
                        {
                            activityObj.UserName = userObj.FirstName + " " + userObj.LastName;
                        }
                        else
                        {
                            activityObj.UserName = userObj.Email;
                        }
                        if (userObj.ProfileImage != null)
                        {
                            activityObj.ProfileURL = OneClappContext.CurrentURL + "User/ProfileImageView/" + item.UserId + "?" + Timestamp;
                        }
                    }
                    activityList.Add(activityObj);
                }
            }
            return new OperationResult<List<EmployeeTaskHistoryResponse>>(true, System.Net.HttpStatusCode.OK, "", activityList);
        }

        // Document delete method - Shakti
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete("{Id}")]
        public async Task<OperationResult<EmployeeTaskAttachmentDto>> RemoveDocument(long Id)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var document = _employeeTaskAttachmentService.DeleteEmployeeTaskAttachmentById(Id);

            await _hubContext.Clients.All.OnUploadEmployeeTaskDocumentEventEmit(document.EmployeeTaskId);

            // Remove task documents from folder
            if (document != null)
            {
                //var dirPath = _hostingEnvironment.WebRootPath + "\\EmployeeTaskUpload";
                var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.EmployeeTaskUploadDirPath;
                var filePath = dirPath + "\\" + document.Name;

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(Path.Combine(filePath));
                }
                EmployeeTaskActivity employeeTaskActivityObj = new EmployeeTaskActivity();
                employeeTaskActivityObj.EmployeeTaskId = document.EmployeeTaskId;
                employeeTaskActivityObj.UserId = UserId;
                employeeTaskActivityObj.Activity = "Removed an attachment";
                var AddUpdate = await _employeeTaskActivityService.CheckInsertOrUpdate(employeeTaskActivityObj);
                await _hubContext.Clients.All.OnEmployeeTaskModuleEvent(document.EmployeeTaskId, TenantId);
                return new OperationResult<EmployeeTaskAttachmentDto>(true, System.Net.HttpStatusCode.OK, "Doccument removed successfully");
            }
            else
            {
                return new OperationResult<EmployeeTaskAttachmentDto>(false, System.Net.HttpStatusCode.OK, "Doccument not found");
            }

        }

        // Assigned task user delete method - Shakti
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete("{Id}")]
        public async Task<OperationResult> UnAssignUser(long Id)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            if (Id != null)
            {
                var employeeTaskUserObj = await _employeeTaskUserService.DeleteAssignedEmployeeTaskUser(Id);
                if (employeeTaskUserObj != null)
                {
                    if (employeeTaskUserObj.EmployeeTaskId != null && employeeTaskUserObj.UserId != null)
                    {
                        var userAssignDetails = _userService.GetUserById(employeeTaskUserObj.UserId.Value);
                        var existingItem = _employeeTaskService.GetTaskById(employeeTaskUserObj.EmployeeTaskId.Value);
                        if (userAssignDetails != null)
                            await sendEmail.SendEmailRemoveEmployeeTaskUserAssignNotification(userAssignDetails.Email, userAssignDetails.FirstName + ' ' + userAssignDetails.LastName, existingItem.Description, TenantId);
                        EmployeeTaskActivity employeeTaskActivityObj = new EmployeeTaskActivity();
                        employeeTaskActivityObj.EmployeeTaskId = employeeTaskUserObj.EmployeeTaskId;
                        employeeTaskActivityObj.UserId = UserId;
                        employeeTaskActivityObj.Activity = "Unassign user";
                        var AddUpdate = await _employeeTaskActivityService.CheckInsertOrUpdate(employeeTaskActivityObj);
                        await _hubContext.Clients.All.OnEmployeeTaskModuleEvent(employeeTaskUserObj.EmployeeTaskId, TenantId);
                    }
                }
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "User unassigned", Id);
            }
            else
            {
                return new OperationResult(false, System.Net.HttpStatusCode.OK, "Please provide id", UserId);
            }
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<UpdateEmployeeTaskPriorityRequest>> Priority([FromBody] UpdateEmployeeTaskPriorityRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            EmployeeTaskDto employeeTaskDto = new EmployeeTaskDto();
            EmployeeTaskActivity employeeTaskActivityObj = new EmployeeTaskActivity();
            long? ProjectId = null;
            if (model.Id != null)
            {
                // start logic for Update Current task with priority
                var taskObj = _employeeTaskService.GetTaskById(model.Id.Value);
                //ProjectId = taskObj.ProjectId;
                taskObj.Priority = model.CurrentPriority;
                taskObj.UpdatedBy = UserId;
                if (model.CurrentSectionId != model.PreviousSectionId)
                {
                    taskObj.SectionId = model.CurrentSectionId;
                    employeeTaskActivityObj.Activity = "Project changed and priority set to this task.";
                }
                else
                {
                    employeeTaskActivityObj.Activity = "Priority changed for this task. ";
                }
                var taskAddUpdate = await _employeeTaskService.UpdateTask(taskObj, taskObj.Id);

                employeeTaskActivityObj.EmployeeTaskId = model.Id;
                employeeTaskActivityObj.UserId = UserId;

                var AddUpdate = await _employeeTaskActivityService.CheckInsertOrUpdate(employeeTaskActivityObj);
                await _hubContext.Clients.All.OnEmployeeTaskModuleEvent(taskObj.Id, TenantId);
                // End Logic

                // Start logic for without section task move to section Or task with section move in to with out section tasks
                if (model.IsSectionChange == true)
                {
                    var CurrentSectionId = model.CurrentSectionId;
                    var PreviousSectionId = model.PreviousSectionId;

                    if (PreviousSectionId > 0 && model.PreviousPriority >= 0)
                    {
                        List<EmployeeProjectTask> taskList = new List<EmployeeProjectTask>();
                        if (ProjectId != null)
                        {
                            taskList = _employeeProjectTaskService.GetAllTaskByProjectId(ProjectId.Value);
                        }

                        if (taskList != null)
                        {
                            var tasks = taskList.Where(t => t.EmployeeTask.Priority > model.PreviousPriority && t.Id != model.Id).ToList();
                            if (tasks != null && tasks.Count() > 0)
                            {
                                foreach (var item in tasks)
                                {
                                    item.EmployeeTask.Priority = item.EmployeeTask.Priority - 1;
                                    await _employeeTaskService.UpdateTask(item.EmployeeTask, item.Id);
                                }
                            }
                        }
                    }

                    // logic for move task in without sections 
                    if (PreviousSectionId == null && CurrentSectionId != null)
                    {
                        List<EmployeeProjectTask> TaskList = new List<EmployeeProjectTask>();
                        if (ProjectId != null)
                        {
                            TaskList = _employeeProjectTaskService.GetAllTaskByTenant(ProjectId.Value, TenantId);
                        }
                        if (TaskList != null)
                        {
                            var tasks = TaskList.Where(t => t.EmployeeTask.Priority > model.PreviousPriority && t.Id != model.Id).ToList();
                            if (tasks != null)
                            {
                                foreach (var item in tasks)
                                {
                                    item.EmployeeTask.Priority = item.EmployeeTask.Priority - 1;
                                    await _employeeTaskService.UpdateTask(item.EmployeeTask, item.Id);
                                }
                            }
                        }
                    }
                    // end

                    // start logic for one section task move to other section
                    if (CurrentSectionId == null && PreviousSectionId != null)
                    {
                        var taskList = _employeeProjectTaskService.GetAllTaskByTenant(ProjectId.Value, TenantId);
                        if (taskList != null)
                        {
                            var tasks = taskList.Where(t => t.EmployeeTask.Priority >= model.CurrentPriority && t.Id != model.Id).ToList();
                            if (tasks != null && tasks.Count() > 0)
                            {
                                foreach (var item in tasks)
                                {
                                    item.EmployeeTask.Priority = item.EmployeeTask.Priority + 1;
                                    await _employeeTaskService.UpdateTask(item.EmployeeTask, item.Id);
                                }
                            }
                        }
                    }

                    if (CurrentSectionId > 0 && model.CurrentPriority >= 0)
                    {
                        List<EmployeeProjectTask> taskList = new List<EmployeeProjectTask>();
                        if (ProjectId != null)
                        {
                            taskList = _employeeProjectTaskService.GetAllTaskByProjectId(ProjectId.Value);
                        }
                        if (taskList != null)
                        {
                            var tasks = taskList.Where(t => t.EmployeeTask.Priority >= model.CurrentPriority && t.Id != model.Id).ToList();
                            if (tasks != null && tasks.Count() > 0)
                            {
                                foreach (var item in tasks)
                                {
                                    item.EmployeeTask.Priority = item.EmployeeTask.Priority + 1;
                                    await _employeeTaskService.UpdateTask(item.EmployeeTask, item.Id);
                                }
                            }
                        }
                    }
                    // end
                }
                else if (model.PreviousSectionId != null && model.CurrentSectionId != null)
                {
                    // start logic for task move in one section
                    if (model.CurrentSectionId != null && (model.CurrentSectionId == model.PreviousSectionId))
                    {
                        var taskList = _employeeTaskService.GetAllTaskBySection(model.CurrentSectionId.Value);

                        if (model.CurrentPriority < taskList.Count())
                        {
                            if (model.CurrentPriority != model.PreviousPriority)
                            {
                                if (model.PreviousPriority < model.CurrentPriority)
                                {
                                    if (taskList != null)
                                    {
                                        var tasks = taskList.Where(t => t.Priority > model.PreviousPriority && t.Priority <= model.CurrentPriority && t.Id != model.Id).ToList();
                                        if (tasks != null && tasks.Count() > 0)
                                        {
                                            foreach (var item in tasks)
                                            {
                                                item.Priority = item.Priority - 1;
                                                await _employeeTaskService.UpdateTask(item, item.Id);
                                            }
                                        }
                                    }
                                }
                                else if (model.PreviousPriority > model.CurrentPriority)
                                {
                                    if (taskList != null)
                                    {
                                        var tasks = taskList.Where(t => t.Priority < model.PreviousPriority && t.Priority >= model.CurrentPriority && t.Id != model.Id).ToList();
                                        if (tasks != null && tasks.Count() > 0)
                                        {
                                            foreach (var item in tasks)
                                            {
                                                item.Priority = item.Priority + 1;
                                                await _employeeTaskService.UpdateTask(item, item.Id);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    // start logic for task move in with out section list
                    List<EmployeeProjectTask> taskList = new List<EmployeeProjectTask>();
                    if (ProjectId != null)
                    {
                        taskList = _employeeProjectTaskService.GetAllTaskByTenant(ProjectId.Value, TenantId);
                    }

                    if (model.CurrentPriority < taskList.Count())
                    {
                        if (model.CurrentPriority != model.PreviousPriority)
                        {
                            if (model.PreviousPriority < model.CurrentPriority)
                            {
                                if (taskList != null)
                                {
                                    var tasks = taskList.Where(t => t.EmployeeTask.Priority > model.PreviousPriority && t.EmployeeTask.Priority <= model.CurrentPriority && t.Id != model.Id).ToList();
                                    if (tasks != null && tasks.Count() > 0)
                                    {
                                        foreach (var item in tasks)
                                        {
                                            item.EmployeeTask.Priority = item.EmployeeTask.Priority - 1;
                                            await _employeeTaskService.UpdateTask(item.EmployeeTask, item.Id);
                                        }
                                    }
                                }
                            }
                            else if (model.PreviousPriority > model.CurrentPriority)
                            {
                                if (taskList != null)
                                {
                                    var tasks = taskList.Where(t => t.EmployeeTask.Priority < model.PreviousPriority && t.EmployeeTask.Priority >= model.CurrentPriority && t.Id != model.Id).ToList();
                                    if (tasks != null && tasks.Count() > 0)
                                    {
                                        foreach (var item in tasks)
                                        {
                                            item.EmployeeTask.Priority = item.EmployeeTask.Priority + 1;
                                            await _employeeTaskService.UpdateTask(item.EmployeeTask, item.Id);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // end
                }
            }
            return new OperationResult<UpdateEmployeeTaskPriorityRequest>(true, System.Net.HttpStatusCode.OK, "", model);
        }

        //Get task time record details
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{EmployeeTaskId}")]

        public async Task<OperationResult<List<EmployeeTaskTimeRecordDetailResponse>>> TaskTimeRecord(long EmployeeTaskId)
        {
            var mateTaskTimeRecordList = _mateTaskTimeRecordService.GetTimeRecordByTaskId(EmployeeTaskId);
            List<EmployeeTaskTimeRecordDetailResponse> employeeTaskTimeRecordDetailResponseList = new List<EmployeeTaskTimeRecordDetailResponse>();

            if (mateTaskTimeRecordList != null && mateTaskTimeRecordList.Count > 0)
            {
                foreach (var taskTimeRecord in mateTaskTimeRecordList)
                {
                    if (taskTimeRecord != null && taskTimeRecord.MateTimeRecord != null)
                    {
                        EmployeeTaskTimeRecordDetailResponse employeeTaskTimeRecordDetailResponseObj = new EmployeeTaskTimeRecordDetailResponse();
                        employeeTaskTimeRecordDetailResponseObj.Id = taskTimeRecord.MateTimeRecord.Id;
                        employeeTaskTimeRecordDetailResponseObj.Comment = taskTimeRecord.MateTimeRecord.Comment;
                        employeeTaskTimeRecordDetailResponseObj.CreatedOn = taskTimeRecord.MateTimeRecord.CreatedOn;
                        employeeTaskTimeRecordDetailResponseObj.Duration = taskTimeRecord.MateTimeRecord.Duration;
                        employeeTaskTimeRecordDetailResponseObj.IsBillable = taskTimeRecord.MateTimeRecord.IsBillable;
                        employeeTaskTimeRecordDetailResponseList.Add(employeeTaskTimeRecordDetailResponseObj);
                    }

                }
            }

            return new OperationResult<List<EmployeeTaskTimeRecordDetailResponse>>(true, System.Net.HttpStatusCode.OK, "", employeeTaskTimeRecordDetailResponseList);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, ExternalUser, TenantUser")]
        [HttpPost]
        public async Task<OperationResult<EmployeeTaskAssignClientResponse>> AssignClient([FromBody] EmployeeTaskAssignClientRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            EmployeeClientTask employeeTaskDto = new EmployeeClientTask();
            employeeTaskDto.EmployeeTaskId = requestModel.EmployeeTaskId;
            //employeeTaskDto.TenantId = TenantId;
            employeeTaskDto.ClientId = requestModel.ClientId;
            //employeeTaskDto.UpdatedBy = UserId;

            var employeeClientTask = await _employeeClientTaskService.CheckInsertOrUpdate(employeeTaskDto);
            if (employeeTaskDto != null)
            {
                EmployeeTaskActivity employeeTaskActivityObj = new EmployeeTaskActivity()
                {
                    EmployeeTaskId = requestModel.EmployeeTaskId,
                    UserId = UserId,
                    Activity = Enums.EmployeeTaskActivityEnum.Task_assign_to_Client.ToString().Replace("_", " ")
                };
                var AddUpdateTaskActivity = await _employeeTaskActivityService.CheckInsertOrUpdate(employeeTaskActivityObj);
            }
            var employeeTaskAssignClientResponse = _mapper.Map<EmployeeTaskAssignClientResponse>(employeeClientTask);
            if (employeeClientTask != null && employeeClientTask.EmployeeTaskId != null)
            {
                employeeTaskAssignClientResponse.EmployeeTaskId = employeeClientTask.EmployeeTaskId.Value;
            }
            await _hubContext.Clients.All.OnEmployeeTaskModuleEvent(requestModel.ClientId, TenantId);
            return new OperationResult<EmployeeTaskAssignClientResponse>(true, System.Net.HttpStatusCode.OK, "Client assigned successfully", employeeTaskAssignClientResponse);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, ExternalUser, TenantUser")]
        [HttpPost("{EmployeeTaskId}")]
        public async Task<OperationResult> UnAssignClient(long EmployeeTaskId)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            // EmployeeTaskDto employeeTaskDto = new EmployeeTaskDto();
            // employeeTaskDto.Id = EmployeeTaskId;
            // employeeTaskDto.TenantId = TenantId;
            // employeeTaskDto.ClientId = null;

            var employeeClientTaskObj = await _employeeClientTaskService.DeleteByTaskId(EmployeeTaskId);

            if (employeeClientTaskObj != null)
            {
                EmployeeTaskActivity employeeTaskActivityObj = new EmployeeTaskActivity()
                {
                    EmployeeTaskId = EmployeeTaskId,
                    UserId = UserId,
                    Activity = Enums.EmployeeTaskActivityEnum.Unassign_client_from_task.ToString().Replace("_", " ")
                };
                var AddUpdateTaskActivity = await _employeeTaskActivityService.CheckInsertOrUpdate(employeeTaskActivityObj);
            }
            await _hubContext.Clients.All.OnEmployeeTaskModuleEvent(EmployeeTaskId, TenantId);
            return new OperationResult(true, System.Net.HttpStatusCode.OK, "Client unassigned", EmployeeTaskId);
        }

        // [HttpGet("{EmployeeProjectId}")]
        // public async Task<OperationResult<List<EmployeeTaskDropDownListResponse>>> DropDownList(long EmployeeProjectId)
        // {
        //     ClaimsPrincipal user = this.User as ClaimsPrincipal;
        //     TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
        //     List<EmployeeTaskDropDownListResponse> taskList = new List<EmployeeTaskDropDownListResponse>();
        //     var employeeProjectTaskList = _employeeProjectTaskService.GetAllTaskByTenant(EmployeeProjectId, TenantId);
        //     if (employeeProjectTaskList != null && employeeProjectTaskList.Count > 0)
        //     {
        //         foreach (var item in employeeProjectTaskList)
        //         {
        //             if (item.EmployeeTaskId != null)
        //             {
        //                 var employeeTaskObj = _employeeTaskService.GetTaskById(item.EmployeeTaskId.Value);
        //                 var dropdownObj = _mapper.Map<EmployeeTaskDropDownListResponse>(employeeTaskObj);
        //                 taskList.Add(dropdownObj);
        //             }
        //         }
        //     }
        //     //dropdownList = _mapper.Map<List<MateTicketDropDownListResponse>>(mateTicketList);
        //     return new OperationResult<List<EmployeeTaskDropDownListResponse>>(true, System.Net.HttpStatusCode.OK, "", taskList);
        // }
        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, ExternalUser, TenantUser")]
        [HttpPost]
        [SwaggerOperation(Description = "api used for get list for assign task as per logged in user and project,ticket")]
        public async Task<OperationResult<List<EmployeeTaskDropDownListResponse>>> DropDownList([FromBody] EmployeeTaskDropDownListRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            List<EmployeeTaskDropDownListResponse> taskResponseList = new List<EmployeeTaskDropDownListResponse>();
            var taskList = _employeeTaskUserSerivce.GetByUserId(UserId);
            //All task
            if (requestModel.Type.ToLower() == "task" && requestModel.Id == null)
            {
                if (taskList != null && taskList.Count > 0)
                {
                    foreach (var item in taskList)
                    {
                        if (item != null)
                        {
                            EmployeeTaskDropDownListResponse taskObj = new EmployeeTaskDropDownListResponse();
                            taskObj = _mapper.Map<EmployeeTaskDropDownListResponse>(item?.EmployeeTask);
                            taskResponseList.Add(taskObj);
                        }
                    }
                }
            }
            //Project task
            else if (requestModel.Type.ToLower() == "project" && requestModel.Id != null)
            {
                var employeeProjectTaskList = _employeeProjectTaskService.GetAllTaskByTenant(requestModel.Id.Value, TenantId);
                var ProjectTaskList = taskList.Where(p => employeeProjectTaskList.Any(p2 => p2.EmployeeTaskId == p.EmployeeTaskId)).ToList();
                if (ProjectTaskList != null && ProjectTaskList.Count > 0)
                {
                    foreach (var item in ProjectTaskList)
                    {
                        if (item != null)
                        {
                            EmployeeTaskDropDownListResponse projectTaskObj = new EmployeeTaskDropDownListResponse();
                            projectTaskObj = _mapper.Map<EmployeeTaskDropDownListResponse>(item?.EmployeeTask);
                            taskResponseList.Add(projectTaskObj);
                        }
                    }
                }
            }
            //Ticket task
            else if (requestModel.Type.ToLower() == "ticket" && requestModel.Id != null)
            {
                var MateTicketTaskList = _mateTicketTaskService.GetAllByTicketId(requestModel.Id.Value);
                var ticketTaskList = taskList.Where(p => MateTicketTaskList.Any(p2 => p2.EmployeeTaskId == p.EmployeeTaskId)).ToList();
                if (ticketTaskList != null && ticketTaskList.Count > 0)
                {
                    foreach (var item in ticketTaskList)
                    {
                        if (item != null)
                        {
                            EmployeeTaskDropDownListResponse ticketTaskObj = new EmployeeTaskDropDownListResponse();
                            ticketTaskObj = _mapper.Map<EmployeeTaskDropDownListResponse>(item?.EmployeeTask);
                            taskResponseList.Add(ticketTaskObj);
                        }
                    }
                }
            }
            return new OperationResult<List<EmployeeTaskDropDownListResponse>>(true, System.Net.HttpStatusCode.OK, "", taskResponseList);
        }
    }
}