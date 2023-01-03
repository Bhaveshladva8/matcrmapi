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
using matcrm.data.Models.Tables;
using matcrm.data.Models.ViewModels;
using matcrm.service.BusinessLogic;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.data.Context;
using matcrm.service.Utility;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class SubTaskController : Controller
    {
        private readonly IOneClappTaskService _taskService;
        private readonly IOneClappSubTaskService _subTaskService;
        private readonly IOneClappChildTaskService _childTaskService;
        private readonly IOneClappTaskUserSerivce _taskUserService;
        private readonly IOneClappSubTaskUserService _subTaskUserService;
        private readonly IOneClappChildTaskUserService _childTaskUserService;
        private readonly ITaskTimeRecordService _taskTimeRecordService;
        private readonly ISubTaskTimeRecordService _subTaskTimeRecordService;
        private readonly IChildTaskTimeRecordService _childTaskTimeRecordService;
        private readonly IWeClappService _weClappService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ITaskAttachmentService _taskAttachmentService;
        private readonly ISubTaskAttachmentService _subTaskAttachmentService;
        private readonly IChildTaskAttachmentService _childTaskAttachmentService;
        private readonly ITaskActivityService _taskActivityService;
        private readonly ISubTaskActivityService _subTaskActivityService;
        private readonly IChildTaskActivityService _childTaskActivityService;
        private readonly ITaskCommentService _taskCommentService;
        private readonly ISubTaskCommentService _subTaskCommentService;
        private readonly IChildTaskCommentService _childTaskCommentService;
        private readonly IUserService _userService;
        private readonly ITaskStatusService _taskStatusService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEmailLogService _emailLogService;
        private readonly IEmailConfigService _emailConfigService;
        private readonly IEmailProviderService _emailProvider;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private IMapper _mapper;
        private readonly OneClappContext _context;
        private SendEmail sendEmail;
        private int UserId = 0;
        private int TenantId = 0;

        public SubTaskController(IOneClappTaskService taskService,
            IOneClappSubTaskService subTaskService,
            IOneClappChildTaskService childTaskService,
            IOneClappTaskUserSerivce taskUserSerivce,
            IOneClappSubTaskUserService subTaskUserSerivce,
            IOneClappChildTaskUserService childTaskUserService,
            ITaskTimeRecordService taskTimeRecordService,
            ISubTaskTimeRecordService subTaskTimeRecordService,
            IChildTaskTimeRecordService childTaskTimeRecordService,
            IWeClappService weClappService,
            IHostingEnvironment hostingEnvironment,
            ITaskAttachmentService taskAttachmentService,
            ISubTaskAttachmentService subTaskAttachmentService,
            IChildTaskAttachmentService childTaskAttachmentService,
            ITaskActivityService taskActivityService,
            ISubTaskActivityService subTaskActivityService,
            IChildTaskActivityService childTaskActivityService,
            IUserService userService,
            ITaskCommentService taskCommentService,
            ISubTaskCommentService subTaskCommentService,
            IChildTaskCommentService childTaskCommentService,
            ITaskStatusService taskStatusService,
            IEmailTemplateService emailTemplateService,
            IEmailLogService emailLogService,
            IEmailConfigService emailConfigService,
            IEmailProviderService emailProvider,
            IHubContext<BroadcastHub, IHubClient> hubContext,
            IMapper mapper,
            OneClappContext context)
        {
            _taskService = taskService;
            _subTaskService = subTaskService;
            _childTaskService = childTaskService;
            _taskUserService = taskUserSerivce;
            _subTaskUserService = subTaskUserSerivce;
            _childTaskUserService = childTaskUserService;
            _taskTimeRecordService = taskTimeRecordService;
            _subTaskTimeRecordService = subTaskTimeRecordService;
            _childTaskTimeRecordService = childTaskTimeRecordService;
            _weClappService = weClappService;
            _hostingEnvironment = hostingEnvironment;
            _taskAttachmentService = taskAttachmentService;
            _subTaskAttachmentService = subTaskAttachmentService;
            _childTaskAttachmentService = childTaskAttachmentService;
            _taskActivityService = taskActivityService;
            _subTaskActivityService = subTaskActivityService;
            _childTaskActivityService = childTaskActivityService;
            _userService = userService;
            _taskCommentService = taskCommentService;
            _subTaskCommentService = subTaskCommentService;
            _childTaskCommentService = childTaskCommentService;
            _taskStatusService = taskStatusService;
            _emailTemplateService = emailTemplateService;
            _emailLogService = emailLogService;
            _emailProvider = emailProvider;
            _emailConfigService = emailConfigService;
            _hubContext = hubContext;
            _mapper = mapper;
            _context = context;
            sendEmail = new SendEmail(emailTemplateService, emailLogService, emailConfigService, emailProvider, mapper);
        }

        // Save Sub task
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<OneClappSubTaskDto>> AddUpdate([FromBody] OneClappSubTaskDto task)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            long weClappSubTaskId = 0;
            matcrm.data.Models.Tables.TaskStatus taskStatusObj = new matcrm.data.Models.Tables.TaskStatus();
            if (UserId != null)
            {
                var userId = UserId;
                var taskStatusList = _taskStatusService.GetStatusByUser(userId);
                if (taskStatusList != null && taskStatusList.Count() > 0)
                {
                    taskStatusObj = taskStatusList.Where(t => t.IsFinalize == true).FirstOrDefault();
                }
            }

            if (!string.IsNullOrEmpty(task.ApiKey))
            {
                if (task.WeClappTimeRecordId == null)
                {
                    PostTimeRecord postTimeRecordObj = new PostTimeRecord()
                    {
                        description = task.Description,
                        startDate = task.StartAt.Value,
                        ticketNumber = task.Ticket,
                        durationSeconds = 1
                        // durationSeconds = task.Duration.Value
                    };

                    var result = await _weClappService.AddJob(task.ApiKey, task.Tenant, postTimeRecordObj);
                    if (result != null)
                    {
                        weClappSubTaskId = Convert.ToInt64(result.id);
                    }
                }
                else
                {
                    weClappSubTaskId = Convert.ToInt64(task.WeClappTimeRecordId);
                }

                if (weClappSubTaskId != 0)
                {
                    OneClappSubTaskDto oneClappSubTaskDto = new OneClappSubTaskDto();
                    oneClappSubTaskDto.WeClappTimeRecordId = Convert.ToInt64(weClappSubTaskId);
                    oneClappSubTaskDto.OneClappTaskId = task.OneClappTaskId;
                    oneClappSubTaskDto.IsActive = true;
                    oneClappSubTaskDto.StatusId = task.StatusId != null ? task.StatusId : oneClappSubTaskDto.StatusId;
                    oneClappSubTaskDto.Description = task.Description != null || task.Description != "" ? task.Description : oneClappSubTaskDto.Description;
                    oneClappSubTaskDto.StartDate = task.StartDate != null ? task.StartDate : oneClappSubTaskDto.StartDate;
                    oneClappSubTaskDto.EndDate = task.EndDate != null ? task.EndDate : oneClappSubTaskDto.EndDate;
                    // subTaskObj.CreatedBy = UserId;
                    oneClappSubTaskDto.CreatedBy = task.CreatedBy != null ? task.CreatedBy : oneClappSubTaskDto.CreatedBy;

                    // For all sub task with completed status then sub task automatic completed status

                    if (task.Id != null)
                    {
                        var subTaskId = task.Id.Value;
                        var childTasks = _childTaskService.GetAllChildTaskBySubTask(subTaskId);
                        if (UserId != null && childTasks != null && childTasks.Count > 0)
                        {
                            if (taskStatusObj != null)
                            {
                                var completedCount = childTasks.Where(t => t.StatusId == taskStatusObj.Id).Count();
                                if (completedCount == childTasks.Count())
                                {
                                    oneClappSubTaskDto.StatusId = taskStatusObj.Id;
                                }
                            }

                        }
                    }
                    var subTaskResult = _subTaskService.CheckInsertOrUpdate(oneClappSubTaskDto);
                    if (subTaskResult != null)
                    {
                        oneClappSubTaskDto.Id = subTaskResult.Id;
                    }

                    // Add logic for if all sub task with finalize status then task status also completed automatically 
                    if (task.StatusId != null && taskStatusObj != null && task.StatusId == taskStatusObj.Id)
                    {
                        if (task.OneClappTaskId != null)
                        {
                            var taskId = task.OneClappTaskId.Value;
                            var subTasks = _subTaskService.GetAllSubTaskByTask(taskId);
                            if (subTasks != null && subTasks.Count() > 0)
                            {
                                var completedSubTaskCount = subTasks.Where(t => t.StatusId == taskStatusObj.Id).Count();

                                if (subTasks.Count() == completedSubTaskCount)
                                {
                                    var taskObj = _taskService.GetTaskById(taskId);
                                    var taskObjDto = _mapper.Map<OneClappTaskDto>(taskObj);
                                    taskObjDto.StatusId = taskStatusObj.Id;
                                    taskObjDto.UserId = UserId;
                                    var addUpdateTask = _taskService.CheckInsertOrUpdate(taskObjDto);

                                    // Start logic task save in to weclapp when task with finalize status (Completed) 
                                    var totalTaskTimeRecord = _taskTimeRecordService.GetTotalTaskTimeRecord(addUpdateTask.Id);
                                    var userObj = _userService.GetUserById(UserId);
                                    TimeRecord timeRecordObj = new TimeRecord();
                                    timeRecordObj.id = addUpdateTask.WeClappTimeRecordId.ToString();
                                    timeRecordObj.ticketNumber = task.Ticket;
                                    timeRecordObj.durationSeconds = totalTaskTimeRecord;
                                    timeRecordObj.description = addUpdateTask.Description;
                                    timeRecordObj.billable = true;
                                    if (userObj != null)
                                    {
                                        timeRecordObj.userId = userObj.WeClappUserId.ToString();
                                    }
                                    timeRecordObj.startDate = task.StartAt != null ? task.StartAt.Value : 0;
                                    var updateTask = _weClappService.UpdateTimeRecord(task.ApiKey, task.Tenant, timeRecordObj);
                                    // End Task
                                }
                            }
                        }
                    }

                    SubTaskActivity subTaskActivityObj = new SubTaskActivity();
                    subTaskActivityObj.UserId = UserId;
                    if (task.Id == null)
                    {
                        subTaskActivityObj.SubTaskId = subTaskResult.Id;
                        subTaskActivityObj.Activity = "Created the task";
                    }
                    else
                    {
                        subTaskActivityObj.SubTaskId = subTaskResult.Id;
                        subTaskActivityObj.Activity = "Updated the task";
                    }
                    var AddUpdate = _subTaskActivityService.CheckInsertOrUpdate(subTaskActivityObj);

                    if (task.AssignedUsers != null && task.AssignedUsers.Count() > 0)
                    {
                        foreach (var userObj in task.AssignedUsers)
                        {
                            OneClappSubTaskUserDto oneClappSubTaskUserDto = new OneClappSubTaskUserDto();
                            oneClappSubTaskUserDto.OneClappSubTaskId = subTaskResult.Id;
                            oneClappSubTaskUserDto.UserId = userObj.UserId;
                            var isExist = _subTaskUserService.IsExistOrNot(oneClappSubTaskUserDto);
                            var subTaskUserObj = _subTaskUserService.CheckInsertOrUpdate(oneClappSubTaskUserDto);
                            if (subTaskUserObj != null)
                            {
                                userObj.Id = subTaskUserObj.Id;
                            }
                            if (!isExist)
                            {
                                if (oneClappSubTaskUserDto.UserId != null)
                                {
                                    var subTaskUserAssignDetails = _userService.GetUserById(oneClappSubTaskUserDto.UserId.Value);
                                    if (subTaskUserAssignDetails != null)
                                        await sendEmail.SendEmailTaskUserAssignNotification(subTaskUserAssignDetails.Email, subTaskUserAssignDetails.FirstName + ' ' + subTaskUserAssignDetails.LastName, task.Description, TenantId);

                                    SubTaskActivity subTaskActivityObj1 = new SubTaskActivity();
                                    subTaskActivityObj1.SubTaskId = subTaskResult.Id;
                                    subTaskActivityObj1.UserId = UserId;
                                    subTaskActivityObj1.Activity = "Assigned user";
                                    var AddUpdate1 = _subTaskActivityService.CheckInsertOrUpdate(subTaskActivityObj1);
                                }
                            }
                        }

                        oneClappSubTaskDto.AssignedUsers = new List<OneClappSubTaskUser>();
                        oneClappSubTaskDto.AssignedUsers = task.AssignedUsers;
                    }

                    return new OperationResult<OneClappSubTaskDto>(true, System.Net.HttpStatusCode.OK, "SubTask saved successfully.", oneClappSubTaskDto);
                }
            }
            OneClappSubTaskDto oneClappSubTaskDto1 = new OneClappSubTaskDto();
            return new OperationResult<OneClappSubTaskDto>(false, System.Net.HttpStatusCode.OK, "Internal error occured.", oneClappSubTaskDto1);
        }

        // Assign task to users
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<OneClappSubTaskUser>> AssignToUser([FromBody] OneClappSubTaskUserDto subTaskUser)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            subTaskUser.UserId = UserId;
            var assignSubTaskUserObj = _subTaskUserService.CheckInsertOrUpdate(subTaskUser);
            return new OperationResult<OneClappSubTaskUser>(true, System.Net.HttpStatusCode.OK, "", assignSubTaskUserObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<SubTaskTimeRecord>> AddUpdateTimeRecord([FromBody] SubTaskTimeRecordDto Model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var subTaskTimeRecord = _subTaskTimeRecordService.CheckInsertOrUpdate(Model);
            SubTaskActivity subTaskActivityObj = new SubTaskActivity();
            subTaskActivityObj.SubTaskId = subTaskTimeRecord.SubTaskId;
            subTaskActivityObj.UserId = UserId;
            subTaskActivityObj.Activity = "Added time record";
            var AddUpdate1 = _subTaskActivityService.CheckInsertOrUpdate(subTaskActivityObj);
            return new OperationResult<SubTaskTimeRecord>(true, System.Net.HttpStatusCode.OK, "", subTaskTimeRecord);
        }

        //[Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<OperationResult<List<SubTaskAttachment>>> UploadFiles([FromForm] SubTaskAttachmentDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            List<SubTaskAttachment> subTaskAttachmentList = new List<SubTaskAttachment>();

            if (model.FileList == null) throw new Exception("File is null");
            if (model.FileList.Length == 0) throw new Exception("File is empty");

            SubTaskActivity subTaskActivityObj = new SubTaskActivity();
            subTaskActivityObj.SubTaskId = model.SubTaskId;
            subTaskActivityObj.UserId = UserId;
            subTaskActivityObj.Activity = "Uploaded document";

            foreach (IFormFile file in model.FileList)
            {
                // full path to file in temp location
                //var dirPath = _hostingEnvironment.WebRootPath + "\\SubTaskUpload";
                var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.SubTaskUploadDirPath;

                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                var fileName = string.Concat(
                    Path.GetFileNameWithoutExtension(file.FileName),
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
                        return new OperationResult<List<SubTaskAttachment>>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                    }
                }
                using (var oStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    await file.CopyToAsync(oStream);
                }

                SubTaskAttachmentDto subTaskAttchmentObj = new SubTaskAttachmentDto();
                subTaskAttchmentObj.Name = fileName;
                subTaskAttchmentObj.SubTaskId = model.SubTaskId;
                subTaskAttchmentObj.UserId = UserId;
                var addedItem = _subTaskAttachmentService.CheckInsertOrUpdate(subTaskAttchmentObj);
                subTaskAttachmentList.Add(addedItem);
            }
            var AddUpdate = _subTaskActivityService.CheckInsertOrUpdate(subTaskActivityObj);
            await _hubContext.Clients.All.OnUploadTaskDocumentEventEmit(model.SubTaskId);

            return new OperationResult<List<SubTaskAttachment>>(true, System.Net.HttpStatusCode.OK, "", subTaskAttachmentList);
        }

        // [AllowAnonymous]
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{SubTaskId}")]
        public async Task<OperationResult<List<SubTaskAttachment>>> Documents(long SubTaskId)
        {

            List<SubTaskAttachment> subTaskAttachmentList = new List<SubTaskAttachment>();
            subTaskAttachmentList = _subTaskAttachmentService.GetAllBySubTaskId(SubTaskId);

            return new OperationResult<List<SubTaskAttachment>>(true, System.Net.HttpStatusCode.OK, "", subTaskAttachmentList);
        }

        //[Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [AllowAnonymous]
        [HttpGet("{SubTaskAttachmentId}")]
        public FileResult Document(long subTaskAttachmentId)
        {

            var SubTaskAttachmentObj = _subTaskAttachmentService.GetSubTaskAttachmentById(subTaskAttachmentId);

            // full path to file in temp location
            //var dirPath = _hostingEnvironment.WebRootPath + "\\SubTaskUpload";
            var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.SubTaskUploadDirPath;
            var filePath = dirPath + "\\" + "default.png";

            if (SubTaskAttachmentObj != null && !string.IsNullOrEmpty(SubTaskAttachmentObj.Name))
            {
                filePath = dirPath + "\\" + SubTaskAttachmentObj.Name;
                Byte[] b = System.IO.File.ReadAllBytes(filePath);
                return File(b, "*");
            }
            return null;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete]
        public async Task<OperationResult<OneClappSubTaskDto>> Remove([FromBody] OneClappSubTaskDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            if (model.Id != null)
            {

                var subTaskId = model.Id.Value;

                var childTasks = _childTaskService.GetAllChildTaskBySubTask(subTaskId);
                if (childTasks != null && childTasks.Count() > 0)
                {
                    foreach (var item in childTasks)
                    {
                        var childTaskId = item.Id;

                        var childDocuments = await _childTaskAttachmentService.DeleteAttachmentByChildTaskId(childTaskId);

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

                        var childComments = _childTaskCommentService.DeleteCommentByChildTaskId(childTaskId);

                        var childTimeRecords = _childTaskTimeRecordService.DeleteTimeRecordByChildTaskId(childTaskId);

                        var childTaskUsers = _childTaskUserService.DeleteByChildTaskId(childTaskId);

                        var childTaskActivities = _childTaskActivityService.DeleteChildTaskActivityByChildTaskId(childTaskId);

                        var childTaskToDelete = _childTaskService.Delete(childTaskId);

                        ChildTaskActivity childTaskActivityObj = new ChildTaskActivity();
                        childTaskActivityObj.ChildTaskId = childTaskId;
                        childTaskActivityObj.UserId = UserId;
                        childTaskActivityObj.Activity = "Removed the task";
                        var AddUpdate1 = _childTaskActivityService.CheckInsertOrUpdate(childTaskActivityObj);
                    }
                }

                var subDocuments = _subTaskAttachmentService.DeleteAttachmentBySubTaskId(subTaskId);

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

                var comments = _subTaskCommentService.DeleteCommentBySubTaskId(subTaskId);

                var timeRecords = _subTaskTimeRecordService.DeleteTimeRecordBySubTaskId(subTaskId);

                var taskUsers = _subTaskUserService.DeleteBySubTaskId(subTaskId);

                // var subTaskActivities = _subTaskActivityService.DeleteSubTaskActivityBySubTaskId (subTaskId);

                var subTaskToDelete = _subTaskService.Delete(subTaskId);

                SubTaskActivity subTaskActivityObj = new SubTaskActivity();
                subTaskActivityObj.SubTaskId = subTaskId;
                subTaskActivityObj.UserId = UserId;
                subTaskActivityObj.Activity = "Removed the task";
                var AddUpdate = _subTaskActivityService.CheckInsertOrUpdate(subTaskActivityObj);

                return new OperationResult<OneClappSubTaskDto>(true, System.Net.HttpStatusCode.OK, "Sub task deleted successfully", model);
            }
            else
            {
                return new OperationResult<OneClappSubTaskDto>(false, System.Net.HttpStatusCode.OK, "Sub task id null", model);
            }
        }

        // Get Sub Task Detail by Id
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{SubTaskId}")]
        public async Task<OperationResult<OneClappSubTaskVM>> Detail(long SubTaskId)
        {
            OneClappSubTaskVM oneClappSubTaskVMObj = new OneClappSubTaskVM();

            var AllUsers = _userService.GetAll();

            var subTaskObj = _subTaskService.GetSubTaskById(SubTaskId);
            oneClappSubTaskVMObj = _mapper.Map<OneClappSubTaskVM>(subTaskObj);

            var subTaskTotalDuration = _subTaskTimeRecordService.GetTotalSubTaskTimeRecord(SubTaskId);
            oneClappSubTaskVMObj.Duration = subTaskTotalDuration;

            // For sub task assign users
            var assignSubTaskUsers = _subTaskUserService.GetAssignUsersBySubTask(SubTaskId);
            if (assignSubTaskUsers != null && assignSubTaskUsers.Count > 0)
            {
                var assignSubTaskUserVMList = _mapper.Map<List<OneClappSubTaskUserDto>>(assignSubTaskUsers);
                if (oneClappSubTaskVMObj.AssignedUsers == null)
                {
                    oneClappSubTaskVMObj.AssignedUsers = new List<OneClappSubTaskUserDto>();
                }
                foreach (var assignSubTaskUser in assignSubTaskUserVMList)
                {
                    if (AllUsers != null && AllUsers.Count() > 0)
                    {
                        var userObj1 = AllUsers.Where(t => t.Id == assignSubTaskUser.UserId).FirstOrDefault();
                        if (userObj1 != null)
                        {
                            assignSubTaskUser.FirstName = userObj1.FirstName;
                            assignSubTaskUser.LastName = userObj1.LastName;
                        }
                    }
                }
                oneClappSubTaskVMObj.AssignedUsers = assignSubTaskUserVMList;
            }

            // For sub task documents
            var subTaskDocuments = _subTaskAttachmentService.GetAllBySubTaskId(SubTaskId);
            if (subTaskDocuments != null && subTaskDocuments.Count > 0)
            {
                if (oneClappSubTaskVMObj.Documents == null)
                {
                    oneClappSubTaskVMObj.Documents = new List<SubTaskAttachment>();
                }
                oneClappSubTaskVMObj.Documents = subTaskDocuments;
            }

            // For sub task commnets
            var subTaskComments = _subTaskCommentService.GetAllBySubTaskId(SubTaskId);
            if (subTaskComments != null && subTaskComments.Count > 0)
            {
                var subTaskCommentsVMList = _mapper.Map<List<SubTaskCommentDto>>(subTaskComments);
                foreach (var subTaskCommentObj in subTaskCommentsVMList)
                {
                    if (AllUsers != null && AllUsers.Count() > 0)
                    {
                        var userObjSubCom = AllUsers.Where(t => t.Id == subTaskCommentObj.UserId).FirstOrDefault();
                        if (userObjSubCom != null)
                        {
                            subTaskCommentObj.FirstName = userObjSubCom.FirstName;
                            subTaskCommentObj.LastName = userObjSubCom.LastName;
                        }
                    }
                }
                if (oneClappSubTaskVMObj.Comments == null)
                {
                    oneClappSubTaskVMObj.Comments = new List<SubTaskCommentDto>();
                }
                oneClappSubTaskVMObj.Comments = subTaskCommentsVMList;
            }

            // For sub task activities
            var subTaskActivities = _subTaskActivityService.GetAllBySubTaskId(SubTaskId);
            if (subTaskActivities != null && subTaskActivities.Count > 0)
            {
                var subTaskActivitiesVMList = _mapper.Map<List<SubTaskActivityDto>>(subTaskActivities);
                foreach (var subTaskActivityObj in subTaskActivitiesVMList)
                {
                    if (AllUsers != null && AllUsers.Count() > 0)
                    {
                        var userObjAct1 = AllUsers.Where(t => t.Id == subTaskActivityObj.UserId).FirstOrDefault();
                        if (userObjAct1 != null)
                        {
                            subTaskActivityObj.FirstName = userObjAct1.FirstName;
                            subTaskActivityObj.LastName = userObjAct1.LastName;
                        }
                    }
                }
                if (oneClappSubTaskVMObj.Activities == null)
                {
                    oneClappSubTaskVMObj.Activities = new List<SubTaskActivityDto>();
                }
                oneClappSubTaskVMObj.Activities = subTaskActivitiesVMList;
            }

            var childTasks = _childTaskService.GetAllChildTaskBySubTask(SubTaskId);
            if (childTasks != null && childTasks.Count() > 0)
            {
                foreach (var childTask in childTasks)
                {
                    if (oneClappSubTaskVMObj.ChildTasks == null)
                    {
                        oneClappSubTaskVMObj.ChildTasks = new List<OneClappChildTaskVM>();
                    }
                    var childTaskVM = _mapper.Map<OneClappChildTaskVM>(childTask);

                    var childTaskTotalDuration = _childTaskTimeRecordService.GetTotalChildTaskTimeRecord(childTask.Id);
                    childTaskVM.Duration = childTaskTotalDuration;

                    // For child task assign users
                    var assignChildTaskUsers = _childTaskUserService.GetAssignUsersByChildTask(childTask.Id);
                    if (assignChildTaskUsers != null && assignChildTaskUsers.Count > 0)
                    {
                        var assignChildTaskUserVMList = _mapper.Map<List<OneClappChildTaskUserDto>>(assignChildTaskUsers);
                        if (childTaskVM.AssignedUsers == null)
                        {
                            childTaskVM.AssignedUsers = new List<OneClappChildTaskUserDto>();
                        }
                        foreach (var assignChildTaskUser in assignChildTaskUserVMList)
                        {
                            if (AllUsers != null && AllUsers.Count() > 0)
                            {
                                var userObj2 = AllUsers.Where(t => t.Id == assignChildTaskUser.UserId).FirstOrDefault();
                                if (userObj2 != null)
                                {
                                    assignChildTaskUser.FirstName = userObj2.FirstName;
                                    assignChildTaskUser.LastName = userObj2.LastName;
                                }
                            }
                        }
                        childTaskVM.AssignedUsers = assignChildTaskUserVMList;
                    }

                    // For child task documents
                    var childTaskDocuments = _childTaskAttachmentService.GetAllByChildTaskId(childTaskVM.Id);
                    if (childTaskDocuments != null && childTaskDocuments.Count > 0)
                    {
                        if (childTaskVM.Documents == null)
                        {
                            childTaskVM.Documents = new List<ChildTaskAttachment>();
                        }
                        childTaskVM.Documents = childTaskDocuments;
                    }

                    // For child task commnets
                    var childTaskComments = _childTaskCommentService.GetAllByChildTaskId(childTaskVM.Id);
                    if (childTaskComments != null && childTaskComments.Count > 0)
                    {
                        var childTaskCommentsVMList = _mapper.Map<List<ChildTaskCommentDto>>(childTaskComments);
                        foreach (var childTaskCommentObj in childTaskCommentsVMList)
                        {
                            if (AllUsers != null && AllUsers.Count() > 0)
                            {
                                var userObjCom2 = AllUsers.Where(t => t.Id == childTaskCommentObj.UserId).FirstOrDefault();
                                if (userObjCom2 != null)
                                {
                                    childTaskCommentObj.FirstName = userObjCom2.FirstName;
                                    childTaskCommentObj.LastName = userObjCom2.LastName;
                                }
                            }
                        }
                        if (childTaskVM.Comments == null)
                        {
                            childTaskVM.Comments = new List<ChildTaskCommentDto>();
                        }
                        childTaskVM.Comments = childTaskCommentsVMList;
                    }

                    // For task activities
                    var childTaskActivities = _childTaskActivityService.GetAllByChildTaskId(childTaskVM.Id);
                    if (childTaskActivities != null && childTaskActivities.Count > 0)
                    {
                        var childTaskActivitiesVMList = _mapper.Map<List<ChildTaskActivityDto>>(childTaskActivities);
                        foreach (var childTaskActivityObj in childTaskActivitiesVMList)
                        {
                            if (AllUsers != null && AllUsers.Count() > 0)
                            {
                                var userObjAct2 = AllUsers.Where(t => t.Id == childTaskActivityObj.UserId).FirstOrDefault();
                                if (userObjAct2 != null)
                                {
                                    childTaskActivityObj.FirstName = userObjAct2.FirstName;
                                    childTaskActivityObj.LastName = userObjAct2.LastName;
                                }
                            }
                        }
                        if (childTaskVM.Activities == null)
                        {
                            childTaskVM.Activities = new List<ChildTaskActivityDto>();
                        }
                        childTaskVM.Activities = childTaskActivitiesVMList;
                    }

                    oneClappSubTaskVMObj.ChildTasks.Add(childTaskVM);
                }
            }
            return new OperationResult<OneClappSubTaskVM>(true, System.Net.HttpStatusCode.OK, "", oneClappSubTaskVMObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{SubTaskId}")]
        public async Task<OperationResult<List<SubTaskActivityDto>>> History(long SubTaskId)
        {
            List<SubTaskActivityDto> subTaskActivityDtoList = new List<SubTaskActivityDto>();
            var AllUsers = _userService.GetAll();
            var activities = _subTaskActivityService.GetAllBySubTaskId(SubTaskId);
            subTaskActivityDtoList = _mapper.Map<List<SubTaskActivityDto>>(activities);
            if (subTaskActivityDtoList != null && subTaskActivityDtoList.Count() > 0)
            {
                foreach (var item in subTaskActivityDtoList)
                {
                    if (AllUsers != null && AllUsers.Count() > 0)
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
            return new OperationResult<List<SubTaskActivityDto>>(true, System.Net.HttpStatusCode.OK, "", subTaskActivityDtoList);
        }

        // Document delete method - Shakti
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete]
        public async Task<OperationResult<SubTaskAttachmentDto>> RemoveDocument([FromBody] SubTaskAttachmentDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            if (model.Id != null)
            {

                var subDocument = _subTaskAttachmentService.DeleteSubTaskAttachmentById(model.Id.Value);

                // Remove task documents from folder
                if (subDocument != null)
                {
                    //var dirPath = _hostingEnvironment.WebRootPath + "\\SubTaskUpload";
                    var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.SubTaskUploadDirPath;
                    var filePath = dirPath + "\\" + subDocument.Name;

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(Path.Combine(filePath));
                    }

                    SubTaskActivity subTaskActivityObj = new SubTaskActivity();
                    subTaskActivityObj.SubTaskId = model.SubTaskId;
                    subTaskActivityObj.UserId = UserId;
                    subTaskActivityObj.Activity = "Removed an attachment";
                    var AddUpdate1 = _subTaskActivityService.CheckInsertOrUpdate(subTaskActivityObj);
                    await _hubContext.Clients.All.OnUploadTaskDocumentEventEmit(model.SubTaskId);

                    return new OperationResult<SubTaskAttachmentDto>(true, System.Net.HttpStatusCode.OK, "Doccument removed successfully");
                }
                else
                {
                    return new OperationResult<SubTaskAttachmentDto>(false, System.Net.HttpStatusCode.OK, "doccument not found");
                }
            }
            else
            {
                return new OperationResult<SubTaskAttachmentDto>(false, System.Net.HttpStatusCode.OK, "Incorrect doccument value");
            }
        }

        // Assigned Sub task user delete method - Shakti
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete]
        public async Task<OperationResult<OneClappSubTaskUserDto>> RemoveAssignUser([FromBody] OneClappSubTaskUserDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            if (model.Id != null)
            {
                var assignUserRemove = _subTaskUserService.DeleteAssignedSubTaskUser(model.Id);

                if (assignUserRemove != null)
                {
                    SubTaskActivity subTaskActivityObj = new SubTaskActivity();
                    subTaskActivityObj.SubTaskId = model.OneClappSubTaskId;
                    subTaskActivityObj.UserId = UserId;
                    subTaskActivityObj.Activity = "Removed assign user";
                    var AddUpdate = _subTaskActivityService.CheckInsertOrUpdate(subTaskActivityObj);

                    var removeSubTaskUserDetails = _userService.GetUserById(assignUserRemove.UserId.Value);
                    var subTaskDetail = _subTaskService.GetSubTaskById(assignUserRemove.OneClappSubTaskId.Value);
                    if (subTaskDetail != null && removeSubTaskUserDetails != null)
                    {
                        await sendEmail.SendEmailRemoveTaskUserAssignNotification(removeSubTaskUserDetails.Email, removeSubTaskUserDetails.FirstName + ' ' + removeSubTaskUserDetails.LastName, subTaskDetail.Description, model.TenantId.Value);
                        return new OperationResult<OneClappSubTaskUserDto>(true, System.Net.HttpStatusCode.OK, "User removed successfully from sub task.");
                    }
                    else
                    {
                        return new OperationResult<OneClappSubTaskUserDto>(false, System.Net.HttpStatusCode.OK, "Something went wrong");
                    }

                }
            }
            else
            {
                return new OperationResult<OneClappSubTaskUserDto>(false, System.Net.HttpStatusCode.OK, "Incorrect user value");
            }
            return new OperationResult<OneClappSubTaskUserDto>(false, System.Net.HttpStatusCode.OK, "Remove user not found");
        }

        [AllowAnonymous]
        [HttpGet("GetSubTaskDocumentById")]
        public async Task<OperationResult<string>> SubTaskAttachmentDto(long subTaskAttachmentId)
        {
            var subTaskAttachmentObj = _subTaskAttachmentService.GetSubTaskAttachmentById(subTaskAttachmentId);

            //var dirPath = _hostingEnvironment.WebRootPath + "\\SubTaskUpload";
            var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.SubTaskUploadDirPath;
            var filePath = dirPath + "\\" + subTaskAttachmentObj.Name;
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

    }
}