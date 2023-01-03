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

namespace Employee.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class EmployeeSubTaskController : Controller
    {
        private readonly IEmployeeTaskService _employeeTaskService;
        private readonly IEmployeeSubTaskService _employeeSubTaskService;
        private readonly IEmployeeChildTaskService _employeeChildTaskService;
        private readonly IEmployeeTaskUserSerivce _employeeTaskUserSerivce;
        private readonly IEmployeeSubTaskUserService _employeeSubTaskUserService;
        private readonly IEmployeeChildTaskUserService _employeeChildTaskUserService;
        private readonly IEmployeeTaskTimeRecordService _employeeTaskTimeRecordService;
        private readonly IEmployeeSubTaskTimeRecordService _employeeSubTaskTimeRecordService;
        private readonly IEmployeeChildTaskTimeRecordService _employeeChildTaskTimeRecordService;
        private readonly IWeClappService _weClappService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IEmployeeTaskAttachmentService _employeeTaskAttachmentService;
        private readonly IEmployeeSubTaskAttachmentService _employeeSubTaskAttachmentService;
        private readonly IEmployeeChildTaskAttachmentService _employeeChildTaskAttachmentService;
        private readonly IEmployeeTaskActivityService _employeeTaskActivityService;
        private readonly IEmployeeSubTaskActivityService _employeeSubTaskActivityService;
        private readonly IEmployeeChildTaskActivityService _employeeChildTaskActivityService;
        private readonly IEmployeeTaskCommentService _employeeTaskCommentService;
        private readonly IEmployeeSubTaskCommentService _employeeSubTaskCommentService;
        private readonly IEmployeeChildTaskCommentService _employeeChildTaskCommentService;
        private readonly IUserService _userService;
        private readonly IEmployeeTaskStatusService _employeeTaskStatusService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEmailLogService _emailLogService;
        private readonly IEmailConfigService _emailConfigService;
        private readonly IEmailProviderService _emailProvider;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private readonly OneClappContext _context;
        private readonly IMateSubTaskCommentService _mateSubTaskCommentService;
        private readonly IMateCommentAttachmentService _mateCommentAttachmentService;
        private readonly IMateCommentService _mateCommentService;
        private readonly IMateChildTaskCommentService _mateChildTaskCommentService;
        private readonly IMateTimeRecordService _mateTimeRecordService;
        private readonly IMateSubTaskTimeRecordService _mateSubTaskTimeRecordService;
        private readonly IMateChildTaskTimeRecordService _mateChildTaskTimeRecordService;
        private readonly IStatusService _statusService;

        private IMapper _mapper;
        private SendEmail sendEmail;
        private int UserId = 0;
        private int TenantId = 0;

        public EmployeeSubTaskController(IEmployeeTaskService employeeTaskService,
            IEmployeeSubTaskService employeeSubTaskService,
            IEmployeeChildTaskService employeeChildTaskService,
            IEmployeeTaskUserSerivce employeeTaskUserSerivce,
            IEmployeeSubTaskUserService employeeSubTaskUserService,
            IEmployeeChildTaskUserService employeeChildTaskUserService,
            IEmployeeTaskTimeRecordService employeeTaskTimeRecordService,
            IEmployeeSubTaskTimeRecordService employeeSubTaskTimeRecordService,
            IEmployeeChildTaskTimeRecordService employeeChildTaskTimeRecordService,
            IWeClappService weClappService,
            IHostingEnvironment hostingEnvironment,
            IEmployeeTaskAttachmentService employeeTaskAttachmentService,
            IEmployeeSubTaskAttachmentService employeeSubTaskAttachmentService,
            IEmployeeChildTaskAttachmentService employeeChildTaskAttachmentService,
            IEmployeeTaskActivityService employeeTaskActivityService,
            IEmployeeSubTaskActivityService employeeSubTaskActivityService,
            IEmployeeChildTaskActivityService employeeChildTaskActivityService,
            IUserService userService,
            IEmployeeTaskCommentService employeeTaskCommentService,
            IEmployeeSubTaskCommentService employeeSubTaskCommentService,
            IEmployeeChildTaskCommentService employeeChildTaskCommentService,
            IEmployeeTaskStatusService employeeTaskStatusService,
            IEmailTemplateService emailTemplateService,
            IEmailLogService emailLogService,
            IEmailConfigService emailConfigService,
            IEmailProviderService emailProvider,
            IHubContext<BroadcastHub, IHubClient> hubContext,
            OneClappContext context,
            IMateSubTaskCommentService mateSubTaskCommentService,
            IMateCommentAttachmentService mateCommentAttachmentService,
            IMateCommentService mateCommentService,
            IMateChildTaskCommentService mateChildTaskCommentService,
            IMateTimeRecordService mateTimeRecordService,
            IMateSubTaskTimeRecordService mateSubTaskTimeRecordService,
            IMateChildTaskTimeRecordService mateChildTaskTimeRecordService,
            IStatusService statusService,
            IMapper mapper)
        {
            _employeeTaskService = employeeTaskService;
            _employeeSubTaskService = employeeSubTaskService;
            _employeeChildTaskService = employeeChildTaskService;
            _employeeTaskUserSerivce = employeeTaskUserSerivce;
            _employeeSubTaskUserService = employeeSubTaskUserService;
            _employeeChildTaskUserService = employeeChildTaskUserService;
            _employeeTaskTimeRecordService = employeeTaskTimeRecordService;
            _employeeSubTaskTimeRecordService = employeeSubTaskTimeRecordService;
            _employeeChildTaskTimeRecordService = employeeChildTaskTimeRecordService;
            _weClappService = weClappService;
            _hostingEnvironment = hostingEnvironment;
            _employeeTaskAttachmentService = employeeTaskAttachmentService;
            _employeeSubTaskAttachmentService = employeeSubTaskAttachmentService;
            _employeeChildTaskAttachmentService = employeeChildTaskAttachmentService;
            _employeeTaskActivityService = employeeTaskActivityService;
            _employeeSubTaskActivityService = employeeSubTaskActivityService;
            _employeeChildTaskActivityService = employeeChildTaskActivityService;
            _userService = userService;
            _employeeTaskCommentService = employeeTaskCommentService;
            _employeeSubTaskCommentService = employeeSubTaskCommentService;
            _employeeChildTaskCommentService = employeeChildTaskCommentService;
            _employeeTaskStatusService = employeeTaskStatusService;
            _emailTemplateService = emailTemplateService;
            _emailLogService = emailLogService;
            _emailProvider = emailProvider;
            _emailConfigService = emailConfigService;
            _hubContext = hubContext;
            _mateSubTaskCommentService = mateSubTaskCommentService;
            _mateCommentAttachmentService = mateCommentAttachmentService;
            _mateCommentService = mateCommentService;
            _mateChildTaskCommentService = mateChildTaskCommentService;
            _mateTimeRecordService = mateTimeRecordService;
            _mateSubTaskTimeRecordService = mateSubTaskTimeRecordService;
            _mateChildTaskTimeRecordService = mateChildTaskTimeRecordService;
            _statusService = statusService;
            _context = context;
            _mapper = mapper;
            sendEmail = new SendEmail(emailTemplateService, emailLogService, emailConfigService, emailProvider, mapper);
        }

        // Save Time Record [Task]
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<EmployeeSubTaskAddUpdateResponse>> Add([FromBody] AddUpdateEmployeeSubTaskRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            EmployeeSubTaskDto model = _mapper.Map<EmployeeSubTaskDto>(requestModel);

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
            model.CreatedBy = UserId;
            model.IsActive = true;

            var subTaskResult = await _employeeSubTaskService.CheckInsertOrUpdate(model);

            EmployeeSubTaskActivity employeeSubTaskActivityObj = new EmployeeSubTaskActivity();
            employeeSubTaskActivityObj.UserId = UserId;
            employeeSubTaskActivityObj.EmployeeSubTaskId = subTaskResult.Id;
            employeeSubTaskActivityObj.Activity = Enums.EmployeeSubTaskActivityEnum.Subtask_Created.ToString().Replace("_", " ");

            var AddUpdate = await _employeeSubTaskActivityService.CheckInsertOrUpdate(employeeSubTaskActivityObj);

            if (model.AssignedUsers != null && model.AssignedUsers.Count() > 0)
            {
                foreach (var userObj in model.AssignedUsers)
                {
                    EmployeeSubTaskUserDto employeeSubTaskUserDto = new EmployeeSubTaskUserDto();
                    employeeSubTaskUserDto.EmployeeSubTaskId = subTaskResult.Id;
                    employeeSubTaskUserDto.UserId = UserId;
                    var isExist = _employeeSubTaskUserService.IsExistOrNot(employeeSubTaskUserDto);
                    var employeeSubTaskUserObj = await _employeeSubTaskUserService.CheckInsertOrUpdate(employeeSubTaskUserDto);
                    if (employeeSubTaskUserObj != null)
                    {
                        userObj.Id = employeeSubTaskUserObj.Id;
                    }

                    if (!isExist)
                    {
                        var userAssignDetails = _userService.GetUserById(UserId);
                        if (userAssignDetails != null)
                            await sendEmail.SendEmailEmployeeTaskUserAssignNotification(userAssignDetails.Email, userAssignDetails.FirstName + ' ' + userAssignDetails.LastName, model.Description, TenantId, subTaskResult.Id);
                        EmployeeSubTaskActivity employeeSubTaskActivityObj1 = new EmployeeSubTaskActivity();
                        employeeSubTaskActivityObj1.EmployeeSubTaskId = subTaskResult.Id;
                        employeeSubTaskActivityObj1.UserId = UserId;
                        employeeSubTaskActivityObj1.Activity = Enums.EmployeeSubTaskActivityEnum.Subtask_assigned_to_user.ToString().Replace("_", " ");
                        var AddUpdate1 = await _employeeSubTaskActivityService.CheckInsertOrUpdate(employeeSubTaskActivityObj1);
                    }
                }
                model.AssignedUsers = new List<EmployeeSubTaskUser>();
            }

            if (requestModel.ChildTasks != null && requestModel.ChildTasks.Count > 0)
            {
                foreach (var childtaskItem in requestModel.ChildTasks)
                {
                    EmployeeChildTaskDto employeeChildTaskDtoObj = new EmployeeChildTaskDto();
                    employeeChildTaskDtoObj.Description = childtaskItem.Description;
                    employeeChildTaskDtoObj.EmployeeSubTaskId = subTaskResult.Id;
                    employeeChildTaskDtoObj.IsActive = true;
                    employeeChildTaskDtoObj.CreatedBy = UserId;
                    var childTaskResult = await _employeeChildTaskService.CheckInsertOrUpdate(employeeChildTaskDtoObj);
                    if (childTaskResult != null)
                    {
                        EmployeeChildTaskActivity childTaskActivityObj = new EmployeeChildTaskActivity();
                        childTaskActivityObj.EmployeeChildTaskId = childTaskResult.Id;
                        childTaskActivityObj.UserId = UserId;
                        childTaskActivityObj.Activity = Enums.EmployeeChidTaskActivityEnum.Childtask_Created.ToString().Replace("_", " ");
                        var childTaskAddUpdateActivity = await _employeeChildTaskActivityService.CheckInsertOrUpdate(childTaskActivityObj);
                        if (childtaskItem.AssignedUsers != null && childtaskItem.AssignedUsers.Count > 0)
                        {
                            foreach (var childAssignUser in childtaskItem.AssignedUsers)
                            {
                                EmployeeChildTaskUserDto employeeChildTaskUserDtoObj = new EmployeeChildTaskUserDto();
                                employeeChildTaskUserDtoObj.UserId = childAssignUser.UserId;
                                employeeChildTaskUserDtoObj.EmployeeChildTaskId = childTaskResult.Id;
                                var isExist = _employeeChildTaskUserService.IsExistOrNot(employeeChildTaskUserDtoObj);
                                var childTaskAssignUserResult = await _employeeChildTaskUserService.CheckInsertOrUpdate(employeeChildTaskUserDtoObj);
                                if (childTaskAssignUserResult != null)
                                {
                                    childAssignUser.Id = childTaskAssignUserResult.Id;
                                }
                                if (!isExist)
                                {
                                    if (employeeChildTaskUserDtoObj.UserId != null)
                                    {
                                        var childTaskUserAssignDetails = _userService.GetUserById(employeeChildTaskUserDtoObj.UserId.Value);
                                        if (childTaskUserAssignDetails != null)
                                            await sendEmail.SendEmailEmployeeTaskUserAssignNotification(childTaskUserAssignDetails.Email, childTaskUserAssignDetails.FirstName + ' ' + childTaskUserAssignDetails.LastName, childtaskItem.Description, TenantId,childTaskResult.Id);
                                        EmployeeChildTaskActivity childTaskAssignActivityObj = new EmployeeChildTaskActivity();
                                        childTaskAssignActivityObj.EmployeeChildTaskId = childTaskResult.Id;
                                        childTaskAssignActivityObj.UserId = UserId;
                                        childTaskAssignActivityObj.Activity = Enums.EmployeeChidTaskActivityEnum.Childtask_assigned_to_user.ToString().Replace("_", " ");
                                        var childTaskAssignActivity = await _employeeChildTaskActivityService.CheckInsertOrUpdate(childTaskAssignActivityObj);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            var response = _mapper.Map<EmployeeSubTaskAddUpdateResponse>(model);
            response.AssignedUsers = requestModel.AssignedUsers;
            return new OperationResult<EmployeeSubTaskAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Task saved successfully.", response);
        }

        // Save Time Record [Task]
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<EmployeeSubTaskAddUpdateResponse>> Update([FromBody] AddUpdateEmployeeSubTaskRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            EmployeeSubTaskDto model = _mapper.Map<EmployeeSubTaskDto>(requestModel);

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
            if (model.Id == null)
            {
                model.CreatedBy = UserId;
            }
            else
            {
                model.UpdatedBy = UserId;
            }
            model.IsActive = true;

            var subTaskResult = await _employeeSubTaskService.CheckInsertOrUpdate(model);
            if (subTaskResult != null)
            {
                model.Id = subTaskResult.Id;
            }

            EmployeeSubTaskActivity employeeSubTaskActivityObj = new EmployeeSubTaskActivity();
            employeeSubTaskActivityObj.UserId = UserId;
            if (model.Id == null)
            {
                employeeSubTaskActivityObj.EmployeeSubTaskId = subTaskResult.Id;
                employeeSubTaskActivityObj.Activity = Enums.EmployeeSubTaskActivityEnum.Subtask_Created.ToString().Replace("_", " ");
            }
            else
            {
                employeeSubTaskActivityObj.EmployeeSubTaskId = subTaskResult.Id;
                employeeSubTaskActivityObj.Activity = Enums.EmployeeSubTaskActivityEnum.Subtask_Updated.ToString().Replace("_", " ");
            }
            var AddUpdate = await _employeeSubTaskActivityService.CheckInsertOrUpdate(employeeSubTaskActivityObj);

            if (model.AssignedUsers != null && model.AssignedUsers.Count() > 0)
            {
                foreach (var userObj in model.AssignedUsers)
                {
                    EmployeeSubTaskUserDto employeeSubTaskUserDto = new EmployeeSubTaskUserDto();
                    employeeSubTaskUserDto.EmployeeSubTaskId = subTaskResult.Id;
                    employeeSubTaskUserDto.UserId = UserId;
                    var isExist = _employeeSubTaskUserService.IsExistOrNot(employeeSubTaskUserDto);
                    var employeeSubTaskUserObj = await _employeeSubTaskUserService.CheckInsertOrUpdate(employeeSubTaskUserDto);
                    if (employeeSubTaskUserObj != null)
                    {
                        userObj.Id = employeeSubTaskUserObj.Id;
                    }

                    if (!isExist)
                    {
                        var userAssignDetails = _userService.GetUserById(UserId);
                        if (userAssignDetails != null)
                            await sendEmail.SendEmailEmployeeTaskUserAssignNotification(userAssignDetails.Email, userAssignDetails.FirstName + ' ' + userAssignDetails.LastName, model.Description, TenantId, subTaskResult.Id);
                        EmployeeSubTaskActivity employeeSubTaskActivityObj1 = new EmployeeSubTaskActivity();
                        employeeSubTaskActivityObj1.EmployeeSubTaskId = subTaskResult.Id;
                        employeeSubTaskActivityObj1.UserId = UserId;
                        employeeSubTaskActivityObj1.Activity = Enums.EmployeeSubTaskActivityEnum.Subtask_assigned_to_user.ToString().Replace("_", " ");
                        var AddUpdate1 = await _employeeSubTaskActivityService.CheckInsertOrUpdate(employeeSubTaskActivityObj1);
                    }
                }
                model.AssignedUsers = new List<EmployeeSubTaskUser>();
            }

            if (requestModel.ChildTasks != null && requestModel.ChildTasks.Count > 0)
            {
                foreach (var childtaskItem in requestModel.ChildTasks)
                {
                    EmployeeChildTaskDto employeeChildTaskDtoObj = new EmployeeChildTaskDto();
                    employeeChildTaskDtoObj.Id = childtaskItem.Id;
                    employeeChildTaskDtoObj.Description = childtaskItem.Description;
                    employeeChildTaskDtoObj.EmployeeSubTaskId = subTaskResult.Id;
                    employeeChildTaskDtoObj.IsActive = true;
                    if (childtaskItem.Id != null && childtaskItem.Id > 0)
                    {
                        employeeChildTaskDtoObj.UpdatedBy = UserId;
                    }
                    else
                    {
                        employeeChildTaskDtoObj.CreatedBy = UserId;
                    }
                    var childTaskResult = await _employeeChildTaskService.CheckInsertOrUpdate(employeeChildTaskDtoObj);
                    if (childTaskResult != null)
                    {
                        EmployeeChildTaskActivity childTaskActivityObj = new EmployeeChildTaskActivity();
                        childTaskActivityObj.EmployeeChildTaskId = childTaskResult.Id;
                        childTaskActivityObj.UserId = UserId;
                        if (childtaskItem.Id != null && childtaskItem.Id > 0)
                        {
                            childTaskActivityObj.Activity = Enums.EmployeeChidTaskActivityEnum.Childtask_Updated.ToString().Replace("_", " ");
                        }
                        else
                        {
                            childTaskActivityObj.Activity = Enums.EmployeeChidTaskActivityEnum.Childtask_Created.ToString().Replace("_", " ");
                        }
                        //childTaskActivityObj.Activity = Enums.EmployeeChidTaskActivityEnum.Childtask_Created.ToString().Replace("_", " ");
                        var childTaskAddUpdateActivity = await _employeeChildTaskActivityService.CheckInsertOrUpdate(childTaskActivityObj);
                        if (childtaskItem.AssignedUsers != null && childtaskItem.AssignedUsers.Count > 0)
                        {
                            foreach (var childAssignUser in childtaskItem.AssignedUsers)
                            {
                                EmployeeChildTaskUserDto employeeChildTaskUserDtoObj = new EmployeeChildTaskUserDto();
                                employeeChildTaskUserDtoObj.UserId = childAssignUser.UserId;
                                employeeChildTaskUserDtoObj.EmployeeChildTaskId = childTaskResult.Id;
                                var isExist = _employeeChildTaskUserService.IsExistOrNot(employeeChildTaskUserDtoObj);
                                var childTaskAssignUserResult = await _employeeChildTaskUserService.CheckInsertOrUpdate(employeeChildTaskUserDtoObj);
                                if (childTaskAssignUserResult != null)
                                {
                                    childAssignUser.Id = childTaskAssignUserResult.Id;
                                }
                                if (!isExist)
                                {
                                    if (employeeChildTaskUserDtoObj.UserId != null)
                                    {
                                        var childTaskUserAssignDetails = _userService.GetUserById(employeeChildTaskUserDtoObj.UserId.Value);
                                        if (childTaskUserAssignDetails != null)
                                            await sendEmail.SendEmailEmployeeTaskUserAssignNotification(childTaskUserAssignDetails.Email, childTaskUserAssignDetails.FirstName + ' ' + childTaskUserAssignDetails.LastName, childtaskItem.Description, TenantId,childTaskResult.Id);
                                        EmployeeChildTaskActivity childTaskAssignActivityObj = new EmployeeChildTaskActivity();
                                        childTaskAssignActivityObj.EmployeeChildTaskId = childTaskResult.Id;
                                        childTaskAssignActivityObj.UserId = UserId;
                                        childTaskAssignActivityObj.Activity = Enums.EmployeeChidTaskActivityEnum.Childtask_assigned_to_user.ToString().Replace("_", " ");
                                        var childTaskAssignActivity = await _employeeChildTaskActivityService.CheckInsertOrUpdate(childTaskAssignActivityObj);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            var response = _mapper.Map<EmployeeSubTaskAddUpdateResponse>(model);
            response.AssignedUsers = requestModel.AssignedUsers;
            return new OperationResult<EmployeeSubTaskAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Task saved successfully.", response);
        }

        // Assign task to users
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<EmployeeSubTaskUser>> AssignToUser([FromBody] EmployeeSubTaskUserDto subTaskUser)
        {
            var assignSubTaskUserObj = await _employeeSubTaskUserService.CheckInsertOrUpdate(subTaskUser);
            return new OperationResult<EmployeeSubTaskUser>(true, System.Net.HttpStatusCode.OK, "", assignSubTaskUserObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<EmployeeSubTaskTimeRecordResponse>> TimeRecord([FromBody] AddUpdateEmployeeSubTaskTimeRecordRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            EmployeeSubTaskTimeRecordResponse response = new EmployeeSubTaskTimeRecordResponse();
            EmployeeSubTaskTimeRecordDto employeeSubTaskTimeRecordDto = new EmployeeSubTaskTimeRecordDto();
            employeeSubTaskTimeRecordDto = _mapper.Map<EmployeeSubTaskTimeRecordDto>(requestModel);
            EmployeeSubTaskTimeRecord employeeSubTaskTimeRecordObj = new EmployeeSubTaskTimeRecord();
            if (employeeSubTaskTimeRecordDto.Duration != null && employeeSubTaskTimeRecordDto.SubTaskId != null)
            {
                var subTaskTotalDuration = _employeeSubTaskTimeRecordService.GetTotalEmployeeSubTaskTimeRecord(employeeSubTaskTimeRecordDto.SubTaskId.Value);
                if (subTaskTotalDuration >= 0)
                {
                    employeeSubTaskTimeRecordDto.Duration = employeeSubTaskTimeRecordDto.Duration - subTaskTotalDuration;
                }
                employeeSubTaskTimeRecordDto.UserId = UserId;
                employeeSubTaskTimeRecordObj = await _employeeSubTaskTimeRecordService.CheckInsertOrUpdate(employeeSubTaskTimeRecordDto);
                EmployeeSubTaskActivity employeeSubTaskActivityObj = new EmployeeSubTaskActivity();
                employeeSubTaskActivityObj.EmployeeSubTaskId = employeeSubTaskTimeRecordObj.SubTaskId;
                employeeSubTaskActivityObj.UserId = UserId;
                employeeSubTaskActivityObj.Activity = Enums.EmployeeSubTaskActivityEnum.Subtask_time_record_created.ToString().Replace("_", " ");
                var AddUpdate1 = await _employeeSubTaskActivityService.CheckInsertOrUpdate(employeeSubTaskActivityObj);
                response = _mapper.Map<EmployeeSubTaskTimeRecordResponse>(employeeSubTaskTimeRecordObj);
                return new OperationResult<EmployeeSubTaskTimeRecordResponse>(true, System.Net.HttpStatusCode.OK, "", response);
            }
            else
            {
                var message = "EmployeeSubTaskId can not be null";
                if (requestModel.Duration == null)
                {
                    message = "Duration can not be null";
                }
                return new OperationResult<EmployeeSubTaskTimeRecordResponse>(false, System.Net.HttpStatusCode.OK, message, response);
            }

        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<List<EmployeeSubTaskAttachment>>> UploadFiles([FromForm] EmployeeSubTaskAttachmentRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            List<EmployeeSubTaskAttachment> employeeSubTaskAttachmentList = new List<EmployeeSubTaskAttachment>();

            if (model.FileList == null) throw new Exception("File is null");
            if (model.FileList.Length == 0) throw new Exception("File is empty");

            EmployeeSubTaskActivity employeeSubTaskActivityObj = new EmployeeSubTaskActivity();
            employeeSubTaskActivityObj.EmployeeSubTaskId = model.EmployeeSubTaskId;
            employeeSubTaskActivityObj.UserId = UserId;
            employeeSubTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Document_uploaded.ToString().Replace("_", " ");

            foreach (IFormFile file in model.FileList)
            {
                // full path to file in temp location
                //var dirPath = _hostingEnvironment.WebRootPath + "\\EmployeeSubTaskUpload";
                var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.EmployeeSubTaskUploadDirPath;

                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                var fileName = string.Concat(
                    Path.GetFileNameWithoutExtension(file.FileName + "_" + model.EmployeeSubTaskId),
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
                        return new OperationResult<List<EmployeeSubTaskAttachment>>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                    }
                }

                using (var oStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    await file.CopyToAsync(oStream);
                }

                EmployeeSubTaskAttachmentDto employeeSubTaskAttchmentObj = new EmployeeSubTaskAttachmentDto();
                employeeSubTaskAttchmentObj.Name = fileName;
                employeeSubTaskAttchmentObj.EmployeeSubTaskId = model.EmployeeSubTaskId;
                employeeSubTaskAttchmentObj.UserId = UserId;
                var addedItem = await _employeeSubTaskAttachmentService.CheckInsertOrUpdate(employeeSubTaskAttchmentObj);
                employeeSubTaskAttachmentList.Add(addedItem);
            }
            var AddUpdate1 = await _employeeSubTaskActivityService.CheckInsertOrUpdate(employeeSubTaskActivityObj);

            await _hubContext.Clients.All.OnUploadEmployeeTaskDocumentEventEmit(model.EmployeeSubTaskId);

            return new OperationResult<List<EmployeeSubTaskAttachment>>(true, System.Net.HttpStatusCode.OK, "", employeeSubTaskAttachmentList);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        // [AllowAnonymous]
        [HttpGet("{EmployeeSubTaskId}")]
        public async Task<OperationResult<List<EmployeeSubTaskAttachment>>> Documents(long EmployeeSubTaskId)
        {

            List<EmployeeSubTaskAttachment> employeeSubTaskAttachmentList = new List<EmployeeSubTaskAttachment>();
            employeeSubTaskAttachmentList = _employeeSubTaskAttachmentService.GetAllByEmployeeSubTaskId(EmployeeSubTaskId);

            return new OperationResult<List<EmployeeSubTaskAttachment>>(true, System.Net.HttpStatusCode.OK, "", employeeSubTaskAttachmentList);
        }

        [AllowAnonymous]
        [HttpGet("{SubTaskAttachmentId}")]
        public async Task<OperationResult<string>> Document(long SubTaskAttachmentId)
        {
            var taskAttachmentObj = _employeeSubTaskAttachmentService.GetEmployeeSubTaskAttachmentById(SubTaskAttachmentId);

            //var dirPath = _hostingEnvironment.WebRootPath + "\\EmployeeSubTaskUpload";
            var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.EmployeeSubTaskUploadDirPath;
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
        [HttpGet("{EmployeeSubTaskId}")]
        public async Task<OperationResult<EmployeeSubTaskVM>> Detail(long EmployeeSubTaskId)
        {
            EmployeeSubTaskVM employeeSubTaskVMObj = new EmployeeSubTaskVM();

            // var AllCustomers = _customerService.GetAllCustomer();
            var AllUsers = _userService.GetAll();
            var subTaskObj = _employeeSubTaskService.GetSubTaskById(EmployeeSubTaskId);
            employeeSubTaskVMObj = _mapper.Map<EmployeeSubTaskVM>(subTaskObj);

            var taskTotalDuration = _employeeSubTaskTimeRecordService.GetTotalEmployeeSubTaskTimeRecord(EmployeeSubTaskId);
            //employeeSubTaskVMObj.Duration = taskTotalDuration;

            // For task assign users
            var assignUsers = _employeeSubTaskUserService.GetAssignUsersBySubTask(EmployeeSubTaskId);
            if (assignUsers != null && assignUsers.Count > 0)
            {
                var assignTaskUserVMList = _mapper.Map<List<EmployeeSubTaskUserDto>>(assignUsers);
                if (employeeSubTaskVMObj.AssignedUsers == null)
                {
                    employeeSubTaskVMObj.AssignedUsers = new List<EmployeeSubTaskUserDto>();
                }
                if (assignTaskUserVMList != null && assignTaskUserVMList.Count() > 0)
                {
                    foreach (var assignUser in assignTaskUserVMList)
                    {
                        var customerObj = AllUsers.Where(t => t.Id == assignUser.UserId).FirstOrDefault();
                        if (customerObj != null)
                        {
                            assignUser.Name = customerObj.UserName;
                            // assignUser.LastName = userObj.LastName;
                        }
                    }
                }
                employeeSubTaskVMObj.AssignedUsers = assignTaskUserVMList;
            }

            // For task documents
            var employeeSubTaskDocuments = _employeeSubTaskAttachmentService.GetAllByEmployeeSubTaskId(EmployeeSubTaskId);
            if (employeeSubTaskDocuments != null && employeeSubTaskDocuments.Count > 0)
            {
                if (employeeSubTaskVMObj.Documents == null)
                {
                    employeeSubTaskVMObj.Documents = new List<EmployeeSubTaskAttachment>();
                }
                employeeSubTaskVMObj.Documents = employeeSubTaskDocuments;
            }

            // For mate sub task comments
            var mateSubTaskCommentList = _mateSubTaskCommentService.GetBySubTaskId(EmployeeSubTaskId);
            if (mateSubTaskCommentList != null && mateSubTaskCommentList.Count > 0)
            {
                if (mateSubTaskCommentList != null && mateSubTaskCommentList.Count() > 0)
                {
                    foreach (var subtaskItem in mateSubTaskCommentList)
                    {
                        EmployeeSubTaskMateComment mateSubTaskCommentResponseObj = new EmployeeSubTaskMateComment();
                        if (AllUsers != null && AllUsers.Count() > 0)
                        {
                            var userObj = AllUsers.Where(t => t.Id == subtaskItem.MateComment?.UserId).FirstOrDefault();
                            if (userObj != null)
                            {
                                mateSubTaskCommentResponseObj.UserId = subtaskItem.MateComment?.UserId;
                                string UserName = userObj.FirstName + " " + userObj.LastName;
                                mateSubTaskCommentResponseObj.UserName = UserName;

                            }
                        }
                        mateSubTaskCommentResponseObj.Id = subtaskItem.MateComment.Id;
                        mateSubTaskCommentResponseObj.CreatedOn = subtaskItem.MateComment?.CreatedOn;
                        mateSubTaskCommentResponseObj.Comment = subtaskItem.MateComment?.Comment;
                        mateSubTaskCommentResponseObj.MateReplyCommentId = subtaskItem.MateComment?.MateReplyCommentId;
                        //Attachment
                        if (subtaskItem.MateCommentId != null)
                        {
                            var subTaskAttachments = _mateCommentAttachmentService.GetByMateCommentId(subtaskItem.MateCommentId.Value);
                            if (subTaskAttachments != null && subTaskAttachments.Count > 0)
                            {
                                EmployeeSubTaskMateCommentAttachment mateCommentSubTaskAttachmentObj = new EmployeeSubTaskMateCommentAttachment();
                                var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                                foreach (var taskfile in subTaskAttachments)
                                {
                                    mateCommentSubTaskAttachmentObj.Name = taskfile.Name;
                                    if (taskfile.Name == null)
                                    {
                                        mateCommentSubTaskAttachmentObj.URL = null;
                                    }
                                    else
                                    {
                                        mateCommentSubTaskAttachmentObj.URL = OneClappContext.CurrentURL + "MateComment/Attachment/" + taskfile.Id + "?" + Timestamp;
                                    }
                                    mateSubTaskCommentResponseObj.Attachments.Add(mateCommentSubTaskAttachmentObj);
                                }
                            }
                        }
                        //Attachment
                        employeeSubTaskVMObj.Comments.Add(mateSubTaskCommentResponseObj);
                    }
                }
            }
            // For mate sub task comments

            // For task activities
            var employeeSubTaskActivities = _employeeSubTaskActivityService.GetAllByEmployeeSubTaskId(EmployeeSubTaskId);
            if (employeeSubTaskActivities != null && employeeSubTaskActivities.Count > 0)
            {
                var employeeSubTaskActivityDtoList = _mapper.Map<List<EmployeeSubTaskActivityDto>>(employeeSubTaskActivities);
                if (employeeSubTaskActivityDtoList != null && employeeSubTaskActivityDtoList.Count() > 0)
                {
                    foreach (var EmployeeSubTaskActivityObj in employeeSubTaskActivityDtoList)
                    {
                        var userObjAct = AllUsers.Where(t => t.Id == EmployeeSubTaskActivityObj.UserId).FirstOrDefault();
                        if (userObjAct != null)
                        {
                            EmployeeSubTaskActivityObj.FirstName = userObjAct.FirstName;
                            EmployeeSubTaskActivityObj.LastName = userObjAct.LastName;
                        }
                    }
                }
                if (employeeSubTaskVMObj.Activities == null)
                {
                    employeeSubTaskVMObj.Activities = new List<EmployeeSubTaskActivityDto>();
                }
                employeeSubTaskVMObj.Activities = employeeSubTaskActivityDtoList;
            }

            return new OperationResult<EmployeeSubTaskVM>(true, System.Net.HttpStatusCode.OK, "", employeeSubTaskVMObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete("{EmployeeSubTaskId}")]
        public async Task<OperationResult<RemoveEmployeeSubTaskResponse>> Remove(long EmployeeSubTaskId)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var childTasks = _employeeChildTaskService.GetAllChildTaskBySubTask(EmployeeSubTaskId);

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

                // var childDocuments = await _employeeChildTaskAttachmentService.DeleteAttachmentByChildTaskId(childTaskId);

                // // Remove child task documents from folder

                // foreach (var childTaskDoc in childDocuments)
                // {

                //     //var dirPath = _hostingEnvironment.WebRootPath + "\\ChildTaskUpload";
                //     var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.ChildTaskUploadDirPath;
                //     var filePath = dirPath + "\\" + childTaskDoc.Name;

                //     if (System.IO.File.Exists(filePath))
                //     {
                //         System.IO.File.Delete(Path.Combine(filePath));
                //     }
                // }

                // var childComments = await _employeeChildTaskCommentService.DeleteCommentByChildTaskId(childTaskId);

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

                var childTaskActivities = await _employeeChildTaskActivityService.DeleteByEmployeeChildTaskId(childTaskId);

                var childTaskToDelete = await _employeeChildTaskService.Delete(childTaskId);

                EmployeeChildTaskActivity employeeChildTaskActivityObj = new EmployeeChildTaskActivity();
                employeeChildTaskActivityObj.EmployeeChildTaskId = childTaskId;
                employeeChildTaskActivityObj.UserId = UserId;
                employeeChildTaskActivityObj.Activity = Enums.EmployeeChidTaskActivityEnum.Childtask_Removed.ToString().Replace("_", " ");
                var AddUpdate1 = await _employeeChildTaskActivityService.CheckInsertOrUpdate(employeeChildTaskActivityObj);
            }

            //for comment and document
            var mateSubCommentIdList = _mateSubTaskCommentService.GetBySubTaskId(EmployeeSubTaskId).Select(t => t.MateCommentId).ToList();
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
                            subCommentActivityObj.EmployeeSubTaskId = EmployeeSubTaskId;
                            subCommentActivityObj.UserId = UserId;
                            subCommentActivityObj.Activity = Enums.EmployeeSubTaskActivityEnum.Subtask_comment_removed.ToString().Replace("_", " ");
                            var AddUpdateSubCommentActivity = await _employeeSubTaskActivityService.CheckInsertOrUpdate(subCommentActivityObj);
                        }
                    }
                }
            }
            //for comment and document

            //var subTimeRecords = await _employeeSubTaskTimeRecordService.DeleteTimeRecordBySubTaskId(EmployeeSubTaskId);
            //for delete subtask time record
            var mateSubTaskTimeRecordList = _mateSubTaskTimeRecordService.GetRecordBySubTaskId(EmployeeSubTaskId);
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

            var subTaskUsers = await _employeeSubTaskUserService.DeleteBySubTaskId(EmployeeSubTaskId);

            var subTaskActivities = await _employeeSubTaskActivityService.DeleteByEmployeeSubTaskId(EmployeeSubTaskId);

            var subTaskToDelete = await _employeeSubTaskService.Delete(EmployeeSubTaskId);

            EmployeeSubTaskActivity employeeSubTaskActivityObj = new EmployeeSubTaskActivity();
            employeeSubTaskActivityObj.EmployeeSubTaskId = EmployeeSubTaskId;
            employeeSubTaskActivityObj.UserId = UserId;
            employeeSubTaskActivityObj.Activity = Enums.EmployeeSubTaskActivityEnum.Subtask_Removed.ToString().Replace("_", " ");
            var AddUpdate2 = await _employeeSubTaskActivityService.CheckInsertOrUpdate(employeeSubTaskActivityObj);

            var response = _mapper.Map<RemoveEmployeeSubTaskResponse>(subTaskToDelete);

            return new OperationResult<RemoveEmployeeSubTaskResponse>(true, System.Net.HttpStatusCode.OK, "Task deleted successfully", response);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{EmployeeSubTaskId}")]
        public async Task<OperationResult<List<EmployeeSubTaskHistoryResponse>>> History(long EmployeeSubTaskId)
        {
            List<EmployeeSubTaskActivityDto> employeeSubTaskActivityList = new List<EmployeeSubTaskActivityDto>();
            var AllUsers = _userService.GetAll();
            var activities = _employeeSubTaskActivityService.GetAllByEmployeeSubTaskId(EmployeeSubTaskId);
            employeeSubTaskActivityList = _mapper.Map<List<EmployeeSubTaskActivityDto>>(activities);

            if (employeeSubTaskActivityList != null && employeeSubTaskActivityList.Count() > 0)
            {
                foreach (var item in employeeSubTaskActivityList)
                {
                    if (AllUsers != null)
                    {
                        var userObj = AllUsers.Where(t => t.Id == item.UserId).FirstOrDefault();
                        if (userObj != null)
                        {
                            item.FirstName = userObj.FirstName;
                            item.LastName = userObj.LastName;
                            item.Email = userObj.Email;
                            if (item.FirstName != null)
                            {
                                item.ShortName = item.FirstName.Substring(0, 1);
                            }
                            if (item.LastName != null)
                            {
                                item.ShortName = item.ShortName + item.LastName.Substring(0, 1);
                            }
                        }
                    }
                }
            }

            var response = _mapper.Map<List<EmployeeSubTaskHistoryResponse>>(employeeSubTaskActivityList);
            return new OperationResult<List<EmployeeSubTaskHistoryResponse>>(true, System.Net.HttpStatusCode.OK, "", response);
        }

        // Document delete method - Shakti
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete("{Id}")]
        public async Task<OperationResult<RemoveEmployeeSubTaskAttachmentResponse>> RemoveDocument(long Id)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            var deletedDocument = await _employeeSubTaskAttachmentService.DeleteEmployeeSubTaskAttachmentById(Id);

            await _hubContext.Clients.All.OnUploadEmployeeSubTaskDocumentEventEmit(deletedDocument.EmployeeSubTaskId);

            // Remove task documents from folder
            if (deletedDocument != null)
            {
                //var dirPath = _hostingEnvironment.WebRootPath + "\\EmployeeSubTaskUpload";
                var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.EmployeeSubTaskUploadDirPath;
                var filePath = dirPath + "\\" + deletedDocument.Name;

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(Path.Combine(filePath));
                }
                EmployeeSubTaskActivity employeeSubTaskActivityObj = new EmployeeSubTaskActivity();
                employeeSubTaskActivityObj.EmployeeSubTaskId = deletedDocument.EmployeeSubTaskId;
                employeeSubTaskActivityObj.UserId = UserId;
                employeeSubTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Document_removed.ToString().Replace("_", " ");
                var AddUpdate = await _employeeSubTaskActivityService.CheckInsertOrUpdate(employeeSubTaskActivityObj);
                return new OperationResult<RemoveEmployeeSubTaskAttachmentResponse>(true, System.Net.HttpStatusCode.OK, "Doccument removed successfully", new RemoveEmployeeSubTaskAttachmentResponse());
            }
            else
            {
                return new OperationResult<RemoveEmployeeSubTaskAttachmentResponse>(false, System.Net.HttpStatusCode.OK, "Doccument not found", new RemoveEmployeeSubTaskAttachmentResponse());
            }
        }

        // Assigned task user delete method - Shakti
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete("{SubTaskAssignUserId}")]
        public async Task<OperationResult<EmployeeTaskUserDto>> AssignUser(long SubTaskAssignUserId)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            var assignUserRemove = await _employeeSubTaskUserService.DeleteAssignedSubTaskUser(SubTaskAssignUserId);

            if (assignUserRemove != null)
            {
                EmployeeSubTaskActivity employeeSubTaskActivityObj = new EmployeeSubTaskActivity();
                employeeSubTaskActivityObj.EmployeeSubTaskId = assignUserRemove.EmployeeSubTaskId;
                employeeSubTaskActivityObj.UserId = UserId;
                employeeSubTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Assign_user_removed.ToString().Replace("_", " ");
                var AddUpdate = await _employeeSubTaskActivityService.CheckInsertOrUpdate(employeeSubTaskActivityObj);

                if (assignUserRemove.UserId != null)
                {
                    var removeTaskUserDetails = _userService.GetUserById(assignUserRemove.UserId.Value);
                    var subTaskUserdetails = _employeeSubTaskService.GetSubTaskById(assignUserRemove.EmployeeSubTaskId.Value);
                    if (removeTaskUserDetails != null && subTaskUserdetails != null)
                    {
                        await sendEmail.SendEmailRemoveEmployeeTaskUserAssignNotification(removeTaskUserDetails.Email, removeTaskUserDetails.FirstName + ' ' + removeTaskUserDetails.LastName, subTaskUserdetails.Description, TenantId);
                        return new OperationResult<EmployeeTaskUserDto>(true, System.Net.HttpStatusCode.OK, "User removed successfully from task.");
                    }
                    else
                    {
                        return new OperationResult<EmployeeTaskUserDto>(false, System.Net.HttpStatusCode.OK, "Something went wrong");
                    }
                }
            }
            return new OperationResult<EmployeeTaskUserDto>(false, System.Net.HttpStatusCode.OK, "Remove user not found");
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<List<EmployeeSubTaskListResponse>>> List([FromBody] EmployeeSubTaskListRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            //List<EmployeeTask> employeeTaskList = new List<EmployeeTask>();

            var employeeSubTaskList = _employeeSubTaskService.GetByTaskId(model);
            var AllStatus = _statusService.GetByTenant(TenantId);
            var AllUsers = _userService.GetAll();

            var employeeSubTaskListResponseList = _mapper.Map<List<EmployeeSubTaskListResponse>>(employeeSubTaskList);
            //EmployeeTaskListResponse employeeTaskListResponseObj = new EmployeeTaskListResponse();

            if (employeeSubTaskListResponseList != null && employeeSubTaskListResponseList.Count() > 0)
            {
                foreach (var item in employeeSubTaskListResponseList)
                {
                    var statusObj = AllStatus.Where(t => t.Id == item.StatusId).FirstOrDefault();
                    if (statusObj != null)
                    {
                        item.Status = statusObj.Name;
                    }
                    item.Name = item.Description;
                    item.TaskId = model.TaskId;
                    // var assignTaskUsers = _employeeTaskUserService.GetAssignUsersByEmployeeTask(item.Id);
                    // if (assignTaskUsers != null && assignTaskUsers.Count > 0)
                    // {
                    //     var assignTaskUserVMList = _mapper.Map<List<EmployeeTaskUserRequestResponse>>(assignTaskUsers);
                    //     if (item.AssignedUsers == null)
                    //     {
                    //         item.AssignedUsers = new List<EmployeeTaskUserRequestResponse>();
                    //     }
                    //     foreach (var assignTaskUser in assignTaskUserVMList)
                    //     {
                    //         if (AllUsers != null)
                    //         {
                    //             var userObj2 = AllUsers.Where(t => t.Id == assignTaskUser.UserId).FirstOrDefault();
                    //             if (userObj2 != null)
                    //             {
                    //                 assignTaskUser.AssignUserFirstName = userObj2.FirstName;
                    //                 assignTaskUser.AssignUserLastName = userObj2.LastName;
                    //             }
                    //         }
                    //     }
                    //     item.AssignedUsers = assignTaskUserVMList;
                    // }

                    //task time record
                    var mateSubTaskTimeRecordList = _mateSubTaskTimeRecordService.GetBySubTaskIdAndUserId(item.Id, UserId);
                    var mateSubTaskTimeRecordAscList = mateSubTaskTimeRecordList.OrderBy(t => t.MateTimeRecord.CreatedOn).ToList();
                    var mateSubTaskTimeRecordLast = mateSubTaskTimeRecordAscList.LastOrDefault();
                    long TaskTotalDuration = 0;
                    if (mateSubTaskTimeRecordLast != null && mateSubTaskTimeRecordList.Count > 0)
                    {
                        foreach (var subTaskTimeRecord in mateSubTaskTimeRecordList)
                        {
                            if (subTaskTimeRecord.MateTimeRecord != null)
                            {
                                if (subTaskTimeRecord.MateTimeRecord.Duration != null)
                                {
                                    TaskTotalDuration = TaskTotalDuration + subTaskTimeRecord.MateTimeRecord.Duration.Value;

                                    TimeSpan timeSpan = TimeSpan.FromMinutes(TaskTotalDuration);

                                    item.TotalDuration = timeSpan.ToString(@"hh\:mm"); ;
                                    if (mateSubTaskTimeRecordLast != null)
                                    {
                                        item.Enddate = mateSubTaskTimeRecordLast.MateTimeRecord.CreatedOn;
                                    }

                                }
                            }
                        }
                        item.TimeRecordCount = mateSubTaskTimeRecordList.Count;
                    }
                }

            }
            return new OperationResult<List<EmployeeSubTaskListResponse>>(true, System.Net.HttpStatusCode.OK, "", employeeSubTaskListResponseList);
        }

    }
}