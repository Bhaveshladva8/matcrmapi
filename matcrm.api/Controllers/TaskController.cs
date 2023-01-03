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
    public class TaskController : Controller
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
        private readonly IConfiguration _config;
        // private readonly IPdfTemplateService _pdfTemplateService;
        private readonly ITenantConfigService _tenantConfig;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEmailLogService _emailLogService;
        private readonly IEmailConfigService _emailConfigService;
        private readonly IEmailProviderService _emailProvider;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private readonly OneClappContext _context;
        private IMapper _mapper;
        private SendEmail sendEmail;
        private int UserId = 0;
        private int TenantId = 0;
        private Common Common;

        public TaskController(IOneClappTaskService taskService,
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
            IConfiguration config,
            // IPdfTemplateService pdfTemplateService,
            ITenantConfigService tenantConfig,
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
            // _pdfTemplateService = pdfTemplateService;
            _tenantConfig = tenantConfig;
            _emailTemplateService = emailTemplateService;
            _emailLogService = emailLogService;
            _emailProvider = emailProvider;
            _hubContext = hubContext;
            _config = config;
            _mapper = mapper;
            _context = context;
            Common = new Common();
            sendEmail = new SendEmail(emailTemplateService, emailLogService, emailConfigService, emailProvider, mapper);
        }

        // Save Time Record [Task]
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<OneClappTaskDto>> AddUpdate([FromBody] OneClappTaskDto task)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            long weClappTaskId = 0;
            if (!string.IsNullOrEmpty(task.ApiKey))
            {
                if (task.WeClappTimeRecordId == null)
                {
                    // if (task.StartAt == null) {
                    //     task.StartAt = DateTime.UtcNow.Ticks;
                    // }
                    PostTimeRecord timeRecordObj = new PostTimeRecord()
                    {
                        description = task.Description,
                        startDate = task.StartAt.Value,
                        ticketNumber = task.Ticket,
                        // durationSeconds = task.Duration.Value
                        durationSeconds = 1
                    };

                    var result = await _weClappService.AddJob(task.ApiKey, task.Tenant, timeRecordObj);
                    if (result != null)
                    {
                        weClappTaskId = Convert.ToInt64(result.id);
                    }
                }
                else
                {
                    weClappTaskId = Convert.ToInt64(task.WeClappTimeRecordId);
                }

                if (weClappTaskId != 0)
                {

                    OneClappTaskDto oneClappTaskDto = new OneClappTaskDto();
                    oneClappTaskDto.WeClappTimeRecordId = Convert.ToInt64(weClappTaskId);
                    oneClappTaskDto.IsActive = true;
                    oneClappTaskDto.StatusId = task.StatusId;
                    oneClappTaskDto.Description = task.Description;
                    oneClappTaskDto.StartDate = task.StartDate;
                    oneClappTaskDto.EndDate = task.EndDate;
                    oneClappTaskDto.SectionId = task.SectionId;
                    oneClappTaskDto.TenantId = TenantId;
                    oneClappTaskDto.Priority = task.Priority;

                    if (task.Id == null)
                    {
                        oneClappTaskDto.CreatedBy = UserId;
                    }
                    else
                    {
                        oneClappTaskDto.UpdatedBy = UserId;
                    }

                    // For all subtask with completed status then main task automatic completed status
                    if (task.Id != null)
                    {
                        var taskId = task.Id.Value;
                        var subTasks = _subTaskService.GetAllSubTaskByTask(taskId);
                        if (UserId != null && subTasks.Count() > 0)
                        {
                            var userId = UserId;
                            var taskStatusList = _taskStatusService.GetStatusByUser(userId);
                            if (taskStatusList != null)
                            {
                                var finalStatus = taskStatusList.Where(t => t.IsFinalize == true).FirstOrDefault();
                                if (finalStatus != null)
                                {
                                    if (subTasks != null)
                                    {
                                        var completedSubTaskCount = subTasks.Where(t => t.StatusId == finalStatus.Id).Count();
                                        if (subTasks.Count() == completedSubTaskCount)
                                        {
                                            oneClappTaskDto.StatusId = finalStatus.Id;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    var taskResult = _taskService.CheckInsertOrUpdate(oneClappTaskDto);
                    if (taskResult != null)
                    {
                        oneClappTaskDto.Id = taskResult.Id;
                    }

                    TaskActivity taskActivityObj = new TaskActivity();
                    taskActivityObj.UserId = UserId;
                    if (task.Id == null)
                    {
                        taskActivityObj.TaskId = taskResult.Id;
                        taskActivityObj.Activity = "Created Task";
                    }
                    else
                    {
                        taskActivityObj.TaskId = taskResult.Id;
                        taskActivityObj.Activity = "Updated Task";
                    }
                    var AddUpdate = _taskActivityService.CheckInsertOrUpdate(taskActivityObj);

                    if (task.AssignedUsers != null && task.AssignedUsers.Count() > 0)
                    {
                        foreach (var userObj in task.AssignedUsers)
                        {
                            OneClappTaskUserDto oneClappTaskUserDto = new OneClappTaskUserDto();
                            oneClappTaskUserDto.OneClappTaskId = taskResult.Id;
                            oneClappTaskUserDto.UserId = userObj.UserId;
                            var isExist = _taskUserService.IsExistOrNot(oneClappTaskUserDto);
                            var taskUserObj = _taskUserService.CheckInsertOrUpdate(oneClappTaskUserDto);
                            if (taskUserObj != null)
                            {
                                userObj.Id = taskUserObj.Id;
                            }

                            if (!isExist)
                            {
                                if (oneClappTaskUserDto.UserId != null)
                                {
                                    var userAssignDetails = _userService.GetUserById(oneClappTaskUserDto.UserId.Value);
                                    if (userAssignDetails != null)
                                        await sendEmail.SendEmailTaskUserAssignNotification(userAssignDetails.Email, userAssignDetails.FirstName + ' ' + userAssignDetails.LastName, task.Description, TenantId);
                                    TaskActivity taskActivityObj1 = new TaskActivity();
                                    taskActivityObj1.TaskId = taskResult.Id;
                                    taskActivityObj1.UserId = UserId;
                                    taskActivityObj1.Activity = "Assigned the user";
                                    var AddUpdate1 = _taskActivityService.CheckInsertOrUpdate(taskActivityObj1);
                                }
                            }
                        }
                        oneClappTaskDto.AssignedUsers = new List<OneClappTaskUser>();
                        oneClappTaskDto.AssignedUsers = task.AssignedUsers;
                    }
                    return new OperationResult<OneClappTaskDto>(true, System.Net.HttpStatusCode.OK, "Task saved successfully.", oneClappTaskDto);
                }
            }
            OneClappTaskDto postTimeRecord = new OneClappTaskDto();
            return new OperationResult<OneClappTaskDto>(false, System.Net.HttpStatusCode.OK, "Internal error occured.", postTimeRecord);
        }

        // Get All Tasks
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<WeClappTaskVM>> List([FromBody] TicketDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            WeClappTaskVM weClappTaskVMObj = new WeClappTaskVM();

            if (!string.IsNullOrEmpty(model.ApiKey))
            {

                var tasks = _taskService.GetAllActiveByTenant(TenantId);
                if (tasks.Count() == 0)
                {
                    return new OperationResult<WeClappTaskVM>(true, System.Net.HttpStatusCode.OK, "", weClappTaskVMObj);
                }

                List<TimeRecord> timeRecordList = await _weClappService.GetTimeRecords(model.ApiKey, model.Tenant);
                if (timeRecordList != null && timeRecordList.Count() > 0)
                {
                    timeRecordList = timeRecordList.Where(t => t.ticketNumber == model.TicketNumber).ToList();

                    var taskIdList = tasks.Select(t => t.Id).ToList();
                    var subTasks = _subTaskService.GetAllActiveByTaskIds(taskIdList);
                    var subTaskIdList = subTasks.Select(t => t.Id).ToList();
                    var childTasks = _childTaskService.GetAllActiveBySubTaskIds(subTaskIdList);
                    weClappTaskVMObj.Tasks = new List<OneClappTaskVM>();
                    if (tasks != null && tasks.Count() > 0)
                    {
                        foreach (var taskObj in tasks)
                        {
                            var WeClappTaskObject = timeRecordList.Where(t => t.id == taskObj.WeClappTimeRecordId.ToString()).FirstOrDefault();
                            if (WeClappTaskObject != null)
                            {
                                var weClappTaskObj = _mapper.Map<OneClappTaskVM>(taskObj);
                                var taskTotalDuration = _taskTimeRecordService.GetTotalTaskTimeRecord(weClappTaskObj.Id);
                                weClappTaskObj.Duration = taskTotalDuration;
                                var y = 60 * 60 * 1000;
                                var h = taskTotalDuration / y;
                                var m = (taskTotalDuration - (h * y)) / (y / 60);
                                var s = (taskTotalDuration - (h * y) - (m * (y / 60))) / 1000;
                                var mi = taskTotalDuration - (h * y) - (m * (y / 60)) - (s * 1000);

                                weClappTaskObj.Seconds = s;
                                weClappTaskObj.Minutes = m;
                                weClappTaskObj.Hours = h;
                                var assignTaskUsers = _taskUserService.GetAssignUsersByTask(taskObj.Id);
                                if (assignTaskUsers.Count > 0)
                                {
                                    weClappTaskObj.AssignedUsers = new List<OneClappTaskUserDto>();
                                    var assignUsersVMList = _mapper.Map<List<OneClappTaskUserDto>>(assignTaskUsers);
                                    weClappTaskObj.AssignedUsers = assignUsersVMList;
                                }
                                weClappTaskVMObj.Tasks.Add(weClappTaskObj);
                            }
                        }
                    }

                    if (subTasks != null && subTasks.Count() > 0)
                    {
                        foreach (var subtaskObj in subTasks)
                        {
                            var SubTaskObject = timeRecordList.Where(t => t.id == subtaskObj.WeClappTimeRecordId.ToString()).FirstOrDefault();
                            if (SubTaskObject != null)
                            {
                                var weClappSubTaskObj = _mapper.Map<OneClappSubTaskVM>(subtaskObj);
                                if (weClappSubTaskObj != null)
                                {
                                    var taskTotalDuration = _subTaskTimeRecordService.GetTotalSubTaskTimeRecord(weClappSubTaskObj.Id);
                                    weClappSubTaskObj.Duration = taskTotalDuration;
                                    var y = 60 * 60 * 1000;
                                    var h = taskTotalDuration / y;
                                    var m = (taskTotalDuration - (h * y)) / (y / 60);
                                    var s = (taskTotalDuration - (h * y) - (m * (y / 60))) / 1000;
                                    var mi = taskTotalDuration - (h * y) - (m * (y / 60)) - (s * 1000);

                                    weClappSubTaskObj.Seconds = s;
                                    weClappSubTaskObj.Minutes = m;
                                    weClappSubTaskObj.Hours = h;

                                    var taskObj = weClappTaskVMObj.Tasks.Where(t => t.Id == weClappSubTaskObj.OneClappTaskId).FirstOrDefault();
                                    if (taskObj != null)
                                    {
                                        if (taskObj.SubTasks == null)
                                        {
                                            taskObj.SubTasks = new List<OneClappSubTaskVM>();
                                        }
                                        var assignSubTaskUsers = _subTaskUserService.GetAssignUsersBySubTask(subtaskObj.Id);
                                        if (assignSubTaskUsers.Count > 0)
                                        {
                                            weClappSubTaskObj.AssignedUsers = new List<OneClappSubTaskUserDto>();
                                            var assignUsersVMList = _mapper.Map<List<OneClappSubTaskUserDto>>(assignSubTaskUsers);
                                            weClappSubTaskObj.AssignedUsers = assignUsersVMList;
                                        }
                                        taskObj.SubTasks.Add(weClappSubTaskObj);
                                    }
                                }
                            }
                        }
                    }

                    if (childTasks != null && childTasks.Count() > 0)
                    {
                        foreach (var childtaskObj in childTasks)
                        {
                            var ChildTaskObject = timeRecordList.Where(t => t.id == childtaskObj.WeClappTimeRecordId.ToString()).FirstOrDefault();
                            if (ChildTaskObject != null)
                            {
                                var weClappChildTaskObj = _mapper.Map<OneClappChildTaskVM>(childtaskObj);
                                if (weClappChildTaskObj != null)
                                {
                                    var weclappTasks = weClappTaskVMObj.Tasks;
                                    var subTaskObj = new OneClappSubTaskVM();
                                    var subTaskIndex = -1;
                                    foreach (var weclappTaskObj in weclappTasks)
                                    {
                                        if (weclappTaskObj.SubTasks != null)
                                        {
                                            var obj = weclappTaskObj.SubTasks.Where(t => t.Id == weClappChildTaskObj.OneClappSubTaskId).FirstOrDefault();
                                            subTaskIndex = weclappTaskObj.SubTasks.FindIndex(t => t.Id == weClappChildTaskObj.OneClappSubTaskId);
                                            if (obj != null)
                                            {
                                                subTaskObj = obj;
                                            }
                                        }
                                    }

                                    if (weClappTaskVMObj.Tasks != null && weClappTaskVMObj.Tasks.Count() > 0)
                                    {
                                        var taskObj = weClappTaskVMObj.Tasks.Where(t => t.Id == subTaskObj.OneClappTaskId).FirstOrDefault();
                                        if (taskObj != null)
                                        {

                                            if (subTaskIndex > -1)
                                            {
                                                var taskTotalDuration = _childTaskTimeRecordService.GetTotalChildTaskTimeRecord(weClappChildTaskObj.Id);
                                                weClappChildTaskObj.Duration = taskTotalDuration;
                                                var y = 60 * 60 * 1000;
                                                var h = taskTotalDuration / y;
                                                var m = (taskTotalDuration - (h * y)) / (y / 60);
                                                var s = (taskTotalDuration - (h * y) - (m * (y / 60))) / 1000;
                                                var mi = taskTotalDuration - (h * y) - (m * (y / 60)) - (s * 1000);

                                                weClappChildTaskObj.Seconds = s;
                                                weClappChildTaskObj.Minutes = m;
                                                weClappChildTaskObj.Hours = h;
                                                if (taskObj.SubTasks[subTaskIndex].ChildTasks == null)
                                                {
                                                    taskObj.SubTasks[subTaskIndex].ChildTasks = new List<OneClappChildTaskVM>();
                                                }
                                                var assignChildTaskUsers = _childTaskUserService.GetAssignUsersByChildTask(childtaskObj.Id);
                                                if (assignChildTaskUsers.Count > 0)
                                                {
                                                    weClappChildTaskObj.AssignedUsers = new List<OneClappChildTaskUserDto>();
                                                    var assignUsersVMList = _mapper.Map<List<OneClappChildTaskUserDto>>(assignChildTaskUsers);
                                                    weClappChildTaskObj.AssignedUsers = assignUsersVMList;
                                                }
                                                taskObj.SubTasks[subTaskIndex].ChildTasks.Add(weClappChildTaskObj);
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return new OperationResult<WeClappTaskVM>(true, System.Net.HttpStatusCode.OK, "", weClappTaskVMObj);
            }
            return new OperationResult<WeClappTaskVM>(false, System.Net.HttpStatusCode.OK, "Internal error occured.", weClappTaskVMObj);
        }

        // Assign task to users
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<OneClappTaskUser>> AssignToUser([FromBody] OneClappTaskUserDto taskUser)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            taskUser.UserId = UserId;
            var assignTaskUserObj = _taskUserService.CheckInsertOrUpdate(taskUser);
            return new OperationResult<OneClappTaskUser>(true, System.Net.HttpStatusCode.OK, "", assignTaskUserObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<TaskTimeRecord>> AddUpdateTimeRecord([FromBody] TaskTimeRecordDto Model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            var taskTimeRecord = _taskTimeRecordService.CheckInsertOrUpdate(Model);
            TaskActivity taskActivityObj = new TaskActivity();
            taskActivityObj.TaskId = taskTimeRecord.TaskId;
            taskActivityObj.UserId = UserId;
            taskActivityObj.Activity = "Created time record";
            var AddUpdate1 = _taskActivityService.CheckInsertOrUpdate(taskActivityObj);
            return new OperationResult<TaskTimeRecord>(true, System.Net.HttpStatusCode.OK, "", taskTimeRecord);
        }

        // [Authorize (Roles = "TenantManager,TenantAdmin, TenantUser")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<OperationResult<List<TaskAttachment>>> UploadTaskFiles([FromForm] TaskAttachmentDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            List<TaskAttachment> TaskAttachmentList = new List<TaskAttachment>();

            if (model.FileList == null) throw new Exception("File is null");
            if (model.FileList.Length == 0) throw new Exception("File is empty");

            TaskActivity taskActivityObj = new TaskActivity();
            taskActivityObj.TaskId = model.TaskId;
            taskActivityObj.UserId = UserId;
            taskActivityObj.Activity = "Uploaded document.";

            foreach (IFormFile file in model.FileList)
            {
                // full path to file in temp location
                //var dirPath = _hostingEnvironment.WebRootPath + "\\TaskUpload";
                var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.TaskUploadDirPath;

                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                var fileName = string.Concat(
                    Path.GetFileNameWithoutExtension(file.FileName + "_" + model.TaskId),
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
                        return new OperationResult<List<TaskAttachment>>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                    }
                }

                using (var oStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    await file.CopyToAsync(oStream);
                }

                TaskAttachmentDto taskAttachmentDto = new TaskAttachmentDto();
                taskAttachmentDto.Name = fileName;
                taskAttachmentDto.TaskId = model.TaskId;
                taskAttachmentDto.UserId = UserId;
                var addedItem = _taskAttachmentService.CheckInsertOrUpdate(taskAttachmentDto);
                TaskAttachmentList.Add(addedItem);
            }
            var AddUpdate1 = _taskActivityService.CheckInsertOrUpdate(taskActivityObj);

            await _hubContext.Clients.All.OnUploadTaskDocumentEventEmit(model.TaskId);

            return new OperationResult<List<TaskAttachment>>(true, System.Net.HttpStatusCode.OK, "", TaskAttachmentList);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{TaskId}")]
        public async Task<OperationResult<List<TaskAttachment>>> Documents(long TaskId)
        {

            List<TaskAttachment> taskAttachmentList = new List<TaskAttachment>();
            taskAttachmentList = _taskAttachmentService.GetAllByTaskId(TaskId);

            return new OperationResult<List<TaskAttachment>>(true, System.Net.HttpStatusCode.OK, "", taskAttachmentList);
        }

        // [Authorize (Roles = "TenantManager,TenantAdmin, TenantUser")]
        [AllowAnonymous]
        [HttpGet]
        public async Task<FileResult> GetTaskDocumentByIdOld(long taskAttachmentId)
        {

            var taskAttachmentObj = _taskAttachmentService.GetTaskAttachmentById(taskAttachmentId);

            // full path to file in temp location
            //var dirPath = _hostingEnvironment.WebRootPath + "\\TaskUpload";
            var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.TaskUploadDirPath;
            var filePath = dirPath + "\\" + "default.png";

            if (taskAttachmentObj != null && !string.IsNullOrEmpty(taskAttachmentObj.Name))
            {
                filePath = dirPath + "\\" + taskAttachmentObj.Name;
                // Byte[] b = System.IO.File.ReadAllBytes (filePath);
                var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
                return File(bytes, Common.GetMimeTypes(taskAttachmentObj.Name), taskAttachmentObj.Name);
            }
            return null;
        }

        [AllowAnonymous]
        [HttpGet("{TaskAttachmentId}")]
        public async Task<OperationResult<string>> Attachment(long TaskAttachmentId)
        {
            var taskAttachmentObj = _taskAttachmentService.GetTaskAttachmentById(TaskAttachmentId);

            //var dirPath = _hostingEnvironment.WebRootPath + "\\TaskUpload";
            var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.TaskUploadDirPath;
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
        [HttpGet("{TaskId}")]
        public async Task<OperationResult<OneClappTaskVM>> Detail(long TaskId)
        {
            OneClappTaskVM oneClappTaskVMObj = new OneClappTaskVM();

            var AllUsers = _userService.GetAll();
            var taskObj = _taskService.GetTaskById(TaskId);
            oneClappTaskVMObj = _mapper.Map<OneClappTaskVM>(taskObj);

            var taskTotalDuration = _taskTimeRecordService.GetTotalTaskTimeRecord(TaskId);
            oneClappTaskVMObj.Duration = taskTotalDuration;

            // For task assign users
            var assignUsers = _taskUserService.GetAssignUsersByTask(TaskId);
            if (assignUsers != null && assignUsers.Count > 0)
            {
                var assignTaskUserVMList = _mapper.Map<List<OneClappTaskUserDto>>(assignUsers);
                if (oneClappTaskVMObj.AssignedUsers == null)
                {
                    oneClappTaskVMObj.AssignedUsers = new List<OneClappTaskUserDto>();
                }
                if (assignTaskUserVMList != null && assignTaskUserVMList.Count() > 0)
                {
                    foreach (var assignUser in assignTaskUserVMList)
                    {
                        if (AllUsers != null)
                        {
                            var userObj = AllUsers.Where(t => t.Id == assignUser.UserId).FirstOrDefault();
                            if (userObj != null)
                            {
                                assignUser.FirstName = userObj.FirstName;
                                assignUser.LastName = userObj.LastName;
                            }
                        }
                    }
                }
                oneClappTaskVMObj.AssignedUsers = assignTaskUserVMList;
            }

            // For task documents
            var taskDocuments = _taskAttachmentService.GetAllByTaskId(TaskId);
            if (taskDocuments != null && taskDocuments.Count > 0)
            {
                if (oneClappTaskVMObj.Documents == null)
                {
                    oneClappTaskVMObj.Documents = new List<TaskAttachment>();
                }
                oneClappTaskVMObj.Documents = taskDocuments;
            }

            // For task commnets
            var taskComments = _taskCommentService.GetAllByTaskId(TaskId);
            if (taskComments != null && taskComments.Count > 0)
            {
                var taskCommentsVMList = _mapper.Map<List<TaskCommentDto>>(taskComments);
                if (taskCommentsVMList != null && taskCommentsVMList.Count() > 0)
                {
                    foreach (var taskCommentObj in taskCommentsVMList)
                    {
                        if (AllUsers != null)
                        {
                            var userObjCom = AllUsers.Where(t => t.Id == taskCommentObj.UserId).FirstOrDefault();
                            if (userObjCom != null)
                            {
                                taskCommentObj.FirstName = userObjCom.FirstName;
                                taskCommentObj.LastName = userObjCom.LastName;
                            }
                        }
                    }
                }
                if (oneClappTaskVMObj.Comments == null)
                {
                    oneClappTaskVMObj.Comments = new List<TaskCommentDto>();
                }
                oneClappTaskVMObj.Comments = taskCommentsVMList;
            }

            // For task activities
            var taskActivities = _taskActivityService.GetAllByTaskId(TaskId);
            if (taskActivities != null && taskActivities.Count > 0)
            {
                var taskActivitiesVMList = _mapper.Map<List<TaskActivityDto>>(taskActivities);
                foreach (var taskActivityObj in taskActivitiesVMList)
                {
                    if (AllUsers != null)
                    {
                        var userObjAct = AllUsers.Where(t => t.Id == taskActivityObj.UserId).FirstOrDefault();
                        if (userObjAct != null)
                        {
                            taskActivityObj.FirstName = userObjAct.FirstName;
                            taskActivityObj.LastName = userObjAct.LastName;
                        }
                    }
                }
                if (oneClappTaskVMObj.Activities == null)
                {
                    oneClappTaskVMObj.Activities = new List<TaskActivityDto>();
                }
                oneClappTaskVMObj.Activities = taskActivitiesVMList;
            }

            var subTaskList = _subTaskService.GetAllSubTaskByTask(TaskId);
            if (subTaskList != null && subTaskList.Count() > 0)
            {
                foreach (var subTask in subTaskList)
                {
                    var subTaskVM = _mapper.Map<OneClappSubTaskVM>(subTask);
                    if (oneClappTaskVMObj.SubTasks == null)
                    {
                        oneClappTaskVMObj.SubTasks = new List<OneClappSubTaskVM>();
                    }

                    var subTaskTotalDuration = _subTaskTimeRecordService.GetTotalSubTaskTimeRecord(subTask.Id);
                    subTaskVM.Duration = subTaskTotalDuration;

                    // For sub task assign users
                    var assignSubTaskUsers = _subTaskUserService.GetAssignUsersBySubTask(subTask.Id);
                    if (assignSubTaskUsers != null && assignSubTaskUsers.Count > 0)
                    {
                        var assignSubTaskUserVMList = _mapper.Map<List<OneClappSubTaskUserDto>>(assignSubTaskUsers);
                        if (subTaskVM.AssignedUsers == null)
                        {
                            subTaskVM.AssignedUsers = new List<OneClappSubTaskUserDto>();
                        }
                        foreach (var assignSubTaskUser in assignSubTaskUserVMList)
                        {
                            var userObj1 = AllUsers.Where(t => t.Id == assignSubTaskUser.UserId).FirstOrDefault();
                            if (userObj1 != null)
                            {
                                assignSubTaskUser.FirstName = userObj1.FirstName;
                                assignSubTaskUser.LastName = userObj1.LastName;
                            }
                        }
                        subTaskVM.AssignedUsers = assignSubTaskUserVMList;
                    }

                    // For sub task documents
                    var subTaskDocuments = _subTaskAttachmentService.GetAllBySubTaskId(subTask.Id);
                    if (subTaskDocuments != null && subTaskDocuments.Count > 0)
                    {
                        if (subTaskVM.Documents == null)
                        {
                            subTaskVM.Documents = new List<SubTaskAttachment>();
                        }
                        subTaskVM.Documents = subTaskDocuments;
                    }

                    // For sub task commnets
                    var subTaskComments = _subTaskCommentService.GetAllBySubTaskId(subTaskVM.Id);
                    if (subTaskComments != null && subTaskComments.Count > 0)
                    {
                        var subTaskCommentsVMList = _mapper.Map<List<SubTaskCommentDto>>(subTaskComments);
                        foreach (var subTaskCommentObj in subTaskCommentsVMList)
                        {
                            var userObjSubCom = AllUsers.Where(t => t.Id == subTaskCommentObj.UserId).FirstOrDefault();
                            if (userObjSubCom != null)
                            {
                                subTaskCommentObj.FirstName = userObjSubCom.FirstName;
                                subTaskCommentObj.LastName = userObjSubCom.LastName;
                            }
                        }
                        if (subTaskVM.Comments == null)
                        {
                            subTaskVM.Comments = new List<SubTaskCommentDto>();
                        }
                        subTaskVM.Comments = subTaskCommentsVMList;
                    }

                    // For sub task activities
                    var subTaskActivities = _subTaskActivityService.GetAllBySubTaskId(subTaskVM.Id);
                    if (subTaskActivities != null && subTaskActivities.Count > 0)
                    {
                        var subTaskActivitiesVMList = _mapper.Map<List<SubTaskActivityDto>>(subTaskActivities);
                        foreach (var subTaskActivityObj in subTaskActivitiesVMList)
                        {
                            var userObjAct1 = AllUsers.Where(t => t.Id == subTaskActivityObj.UserId).FirstOrDefault();
                            if (userObjAct1 != null)
                            {
                                subTaskActivityObj.FirstName = userObjAct1.FirstName;
                                subTaskActivityObj.LastName = userObjAct1.LastName;
                            }
                        }
                        if (subTaskVM.Activities == null)
                        {
                            subTaskVM.Activities = new List<SubTaskActivityDto>();
                        }
                        subTaskVM.Activities = subTaskActivitiesVMList;
                    }

                    var childTasks = _childTaskService.GetAllChildTaskBySubTask(subTask.Id);
                    if (childTasks != null && childTasks.Count() > 0)
                    {
                        foreach (var childTask in childTasks)
                        {
                            if (subTaskVM.ChildTasks == null)
                            {
                                subTaskVM.ChildTasks = new List<OneClappChildTaskVM>();
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
                                    if (AllUsers != null)
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
                                    if (AllUsers != null)
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
                                    if (AllUsers != null)
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

                            subTaskVM.ChildTasks.Add(childTaskVM);
                        }
                    }
                    oneClappTaskVMObj.SubTasks.Add(subTaskVM);
                }
            }
            return new OperationResult<OneClappTaskVM>(true, System.Net.HttpStatusCode.OK, "", oneClappTaskVMObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete]
        public async Task<OperationResult<OneClappTaskDto>> Remove([FromBody] OneClappTaskDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            if (model.Id != null)
            {
                model.TenantId = TenantId;
                model.UserId = UserId;
                var taskId = model.Id.Value;

                var subTasks = _subTaskService.GetAllSubTaskByTask(taskId);
                if (subTasks != null && subTasks.Count() > 0)
                {
                    foreach (var subTask in subTasks)
                    {
                        var subTaskId = subTask.Id;

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

                                ChildTaskActivity childTaskActivityobj = new ChildTaskActivity();
                                childTaskActivityobj.ChildTaskId = childTaskId;
                                childTaskActivityobj.UserId = UserId;
                                childTaskActivityobj.Activity = "Removed the task";
                                var AddUpdate1 = _childTaskActivityService.CheckInsertOrUpdate(childTaskActivityobj);
                            }
                        }

                        var subDocuments = _subTaskAttachmentService.DeleteAttachmentBySubTaskId(subTaskId);

                        // Remove sub task documents from folder
                        if (subDocuments != null)
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

                        var subComments = _subTaskCommentService.DeleteCommentBySubTaskId(subTaskId);

                        var subTimeRecords = _subTaskTimeRecordService.DeleteTimeRecordBySubTaskId(subTaskId);

                        var subTaskUsers = _subTaskUserService.DeleteBySubTaskId(subTaskId);

                        // var subTaskActivities = _subTaskActivityService.DeleteSubTaskActivityBySubTaskId (subTaskId);

                        var subTaskToDelete = _subTaskService.Delete(subTaskId);

                        SubTaskActivity subTaskActivityObj = new SubTaskActivity();
                        subTaskActivityObj.SubTaskId = subTaskId;
                        subTaskActivityObj.UserId = UserId;
                        subTaskActivityObj.Activity = "Removed the task";
                        var AddUpdate2 = _subTaskActivityService.CheckInsertOrUpdate(subTaskActivityObj);
                    }
                }

                var documents = _taskAttachmentService.DeleteAttachmentByTaskId(taskId);

                // Remove task documents from folder
                if (documents != null && documents.Count() > 0)
                {
                    foreach (var taskDoc in documents)
                    {

                        //var dirPath = _hostingEnvironment.WebRootPath + "\\TaskUpload";
                        var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.TaskUploadDirPath;
                        var filePath = dirPath + "\\" + taskDoc.Name;

                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(Path.Combine(filePath));
                        }
                    }
                }

                var comments = _taskCommentService.DeleteCommentByTaskId(taskId);

                var timeRecords = _taskTimeRecordService.DeleteTimeRecordByTaskId(taskId);

                var taskUsers = _taskUserService.DeleteByTaskId(taskId);

                // var taskActivities = _taskActivityService.DeleteTaskActivityByTaskId (taskId);

                var taskToDelete = _taskService.Delete(taskId);

                TaskActivity taskActivityObj = new TaskActivity();
                taskActivityObj.TaskId = taskId;
                taskActivityObj.UserId = UserId;
                taskActivityObj.Activity = "Removed the task";
                var AddUpdate = _taskActivityService.CheckInsertOrUpdate(taskActivityObj);

                return new OperationResult<OneClappTaskDto>(true, System.Net.HttpStatusCode.OK, "Task deleted successfully", model);
            }
            else
            {
                return new OperationResult<OneClappTaskDto>(false, System.Net.HttpStatusCode.OK, "Task id null", model);
            }
        }
        
        // [Authorize (Roles = "TenantManager,TenantAdmin, TenantUser")]
        // [AllowAnonymous]
        // [HttpPost ("Document")]
        // public async Task<OperationResult<OneClappTaskDto>> Document ([FromBody] OneClappTaskDto finalize) {
        //     // Check if a given license key string is valid.
        //     bool isLicensed = IronPdf.License.IsValidLicense (_config.GetValue<string> ("IronPdf.LicenseKey"));
        //     // Check if IronPdf is licensed sucessfully 
        //     bool is_licensed = IronPdf.License.IsLicensed;

        //     // UserVM currentUser = AuthData.GetAll (this.User);
        //     // if (currentUser == null) return Json (new { success = false, message = "Authentication failed." });

        //     var Renderer = new HtmlToPdf ();

        //     // Save form data to PDF
        //     //var finalPdf = "Leistungsnachweis" + "-Ticket-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + finialize.TicketId + ".pdf";
        //     var finalPdf = "Leistungsnachweis" + "-Ticket-" + finalize.Ticket + ".pdf";

        //     var finalPath = Path.Combine (Directory.GetCurrentDirectory (), @"wwwroot\Output\", finalPdf);

        //     var customCss = Path.Combine (Directory.GetCurrentDirectory (), @"wwwroot\Output\", "CustomCssForPdfTemplate.css");
        //     Renderer.PrintOptions.CustomCssUrl = customCss;
        //     Renderer.PrintOptions.FitToPaperWidth = true;
        //     Renderer.PrintOptions.PrintHtmlBackgrounds = true;
        //     Renderer.PrintOptions.PaperOrientation = PdfPrintOptions.PdfPaperOrientation.Landscape;
        //     Renderer.PrintOptions.PaperSize = PdfPrintOptions.PdfPaperSize.A4;
        //     Renderer.PrintOptions.MarginLeft = 0;
        //     Renderer.PrintOptions.MarginRight = 0;
        //     Renderer.PrintOptions.MarginTop = 0;
        //     Renderer.PrintOptions.MarginBottom = 0;
        //     Renderer.PrintOptions.CssMediaType = PdfPrintOptions.PdfCssMediaType.Screen;
        //     Renderer.PrintOptions.ViewPortWidth = 1280;

        //     var pdfTemplate = _pdfTemplateService.GetPdfTemplateByTenant (1003, "TaskDetail");
        //     var tenantConfig = _tenantConfig.GetConfigByTenant (1003);
        //     var doc = "";

        //     // var doc = "<!DOCTYPE html>" +
        //     //     "<html>" +
        //     //     "<head>" +
        //     //     "<link rel = 'stylesheet' href = 'https://pop.mateit.io/css/style.css'/>" +
        //     //     (string.IsNullOrWhiteSpace (pdfTemplate.Fonts) ? "" : "<link href = 'https://fonts.googleapis.com/css2?family=" + finalize.Fonts + "&amp;display=swap' rel = 'stylesheet' />") +
        //     //     "</head>" +
        //     //     "<body>" +
        //     //     finalize.Doc +
        //     //     "</body>" +
        //     //     "</html>";

        //     if (pdfTemplate != null) {
        //         doc = pdfTemplate.TemplateHtml;
        //     }

        //     // Renderer.PrintOptions.Header = new HtmlHeaderFooter () {
        //     //     Height = 40,
        //     //         HtmlFragment = "<div class='row text-center' style='text-align: center;margin: 0 !important;'>" +
        //     //         "<img alt='logo' src='" + tenantConfig.LogoImage + "' height='150' width='100%'>" +
        //     //         "</div>",
        //     //         DrawDividerLine = false
        //     // };

        //     // Renderer.PrintOptions.Footer = new HtmlHeaderFooter () {
        //     //     Height = 30,
        //     //         HtmlFragment = "<div class='row col-12 text-center' style='text-align: center;margin: 0 !important;'>" +
        //     //         "<img alt='logo' src='" + "" + "' height='100' width='100%'>" +
        //     //         "</div>",
        //     //         DrawDividerLine = false
        //     // };

        //     var row = "<tr>" +
        //         "<td>" + "[@Description]" + "</td>" +
        //         "<td>[@Start]</td>" +
        //         "<td>[@Duration]</td>" +
        //         "<td class='text-right'>[@Date]</td>" +
        //         "<td>[@UserName]</td>" +
        //         "</tr>";

        //     var rowObj = "";

        //     var subTasks = _subTaskService.GetAllSubTaskByTask (finalize.Id.Value);

        //     foreach (var subTaskObj in subTasks) {
        //         var totalDuration = _subTaskTimeRecordService.GetTotalSubTaskTimeRecord (subTaskObj.Id);
        //         var rowTemp = row;
        //         rowTemp = rowTemp.Replace ("[@Description]", subTaskObj.Description).Replace ("[@Start]", subTaskObj.StartDate.ToString ()).Replace ("[@Duration]", totalDuration.ToString ()).Replace ("[@Date]", subTaskObj.EndDate.ToString ()).Replace ("[@UserName]", "Shraddha");
        //         rowObj = rowObj + rowTemp;
        //     }

        //     doc = doc.Replace ("[@Description]", finalize.Description).Replace ("[@Row]", rowObj).Replace ("[@Link]", customCss);

        //     var PDF = Renderer.RenderHtmlAsPdf (doc).SaveAs (finalPath);

        //     // if (!string.IsNullOrEmpty (finalize.ApiKey)) {
        //     // var result = await _weClappService.PostDocument (finalize.ApiKey, finalize.Tenant, finalPdf, finalize.TicketId, PDF.BinaryData);
        //     // if (result)
        //     return new OperationResult<OneClappTaskDto> (true, "Document posted successfully", finalize);
        //     // }
        //     finalize = null;
        //     return new OperationResult<OneClappTaskDto> (false, "Internal error occured.", finalize);
        // }

        [Authorize(Roles = "TenantManager,TenantAdmin, TenantUser")]
        [HttpGet("{TaskId}")]
        public async Task<OperationResult<List<TaskActivityDto>>> History(long TaskId)
        {
            List<TaskActivityDto> taskActivityDtoList = new List<TaskActivityDto>();
            var AllUsers = _userService.GetAll();
            var activities = _taskActivityService.GetAllByTaskId(TaskId);
            taskActivityDtoList = _mapper.Map<List<TaskActivityDto>>(activities);
            if (taskActivityDtoList != null && taskActivityDtoList.Count() > 0)
            {
                foreach (var item in taskActivityDtoList)
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
            return new OperationResult<List<TaskActivityDto>>(true, System.Net.HttpStatusCode.OK, "", taskActivityDtoList);
        }

        // Document delete method - Shakti
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete]
        public async Task<OperationResult<TaskAttachmentDto>> RemoveDoccument([FromBody] TaskAttachmentDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            if (model.Id != null)
            {
                var document = _taskAttachmentService.DeleteTaskAttachmentById(model.Id.Value);

                await _hubContext.Clients.All.OnUploadTaskDocumentEventEmit(model.TaskId);

                // Remove task documents from folder
                if (document != null)
                {
                    //var dirPath = _hostingEnvironment.WebRootPath + "\\TaskUpload";
                    var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.TaskUploadDirPath;
                    var filePath = dirPath + "\\" + document.Name;

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(Path.Combine(filePath));
                    }
                    TaskActivity taskActivityObj = new TaskActivity();
                    taskActivityObj.TaskId = model.TaskId;
                    taskActivityObj.UserId = UserId;
                    taskActivityObj.Activity = "Removed an attachment";
                    var AddUpdate = _taskActivityService.CheckInsertOrUpdate(taskActivityObj);

                    return new OperationResult<TaskAttachmentDto>(true, System.Net.HttpStatusCode.OK, "Doccument removed successfully");
                }
                else
                {
                    return new OperationResult<TaskAttachmentDto>(false, System.Net.HttpStatusCode.OK, "Doccument not found");
                }

            }
            else
            {
                return new OperationResult<TaskAttachmentDto>(false, System.Net.HttpStatusCode.OK, "Incorrect doccument value");
            }

        }

        // Assigned task user delete method - Shakti
        [Authorize(Roles = "TenantManager,TenantAdmin, TenantUser")]
        [HttpDelete]
        public async Task<OperationResult<OneClappTaskUserDto>> RemoveAssignUser([FromBody] OneClappTaskUserDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            if (model.Id != null)
            {
                var assignUserRemove = _taskUserService.DeleteAssignedTaskUser(model.Id);

                if (assignUserRemove != null)
                {

                    TaskActivity taskActivityObj = new TaskActivity();
                    taskActivityObj.TaskId = model.OneClappTaskId;
                    taskActivityObj.UserId = UserId;
                    taskActivityObj.Activity = "Removed assign user";
                    var AddUpdate = _taskActivityService.CheckInsertOrUpdate(taskActivityObj);
                    if (assignUserRemove.UserId != null)
                    {
                        var removeTaskUserDetails = _userService.GetUserById(assignUserRemove.UserId.Value);
                        if (assignUserRemove.OneClappTaskId != null)
                        {
                            var taskUserdetails = _taskService.GetTaskById(assignUserRemove.OneClappTaskId.Value);
                            if (removeTaskUserDetails != null && taskUserdetails != null)
                            {

                                await sendEmail.SendEmailRemoveTaskUserAssignNotification(removeTaskUserDetails.Email, removeTaskUserDetails.FirstName + ' ' + removeTaskUserDetails.LastName, taskUserdetails.Description, model.TenantId.Value);
                                return new OperationResult<OneClappTaskUserDto>(true, System.Net.HttpStatusCode.OK, "User removed successfully from task.");

                            }
                            else
                            {
                                return new OperationResult<OneClappTaskUserDto>(false, System.Net.HttpStatusCode.OK, "Something went wrong");
                            }
                        }
                    }
                }
            }
            else
            {
                return new OperationResult<OneClappTaskUserDto>(false, System.Net.HttpStatusCode.OK, "Incorrect user value");
            }
            return new OperationResult<OneClappTaskUserDto>(false, System.Net.HttpStatusCode.OK, "Remove user not found");
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<OneClappTaskDto>> UpdatePriority([FromBody] OneClappTaskDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            OneClappTaskDto oneClappTaskDtoObj = new OneClappTaskDto();
            TaskActivity taskActivityObj = new TaskActivity();
            if (model.Id != null)
            {
                // start logic for Update Current task with priority
                var taskObj = _taskService.GetTaskById(model.Id.Value);
                taskObj.Priority = model.CurrentPriority;
                taskObj.UpdatedBy = UserId;
                if (model.CurrentSectionId != model.PreviousSectionId)
                {
                    taskObj.SectionId = model.CurrentSectionId;
                    taskActivityObj.Activity = "Section changed and priority set to this task.";
                }
                else
                {
                    taskActivityObj.Activity = "Priority changed for this task. ";
                }
                var taskAddUpdate = _taskService.UpdateTask(taskObj, taskObj.Id);

                taskActivityObj.TaskId = model.Id;
                taskActivityObj.UserId = UserId;

                var AddUpdate = _taskActivityService.CheckInsertOrUpdate(taskActivityObj);
                // End Logic

                // Start logic for without section task move to section Or task with section move in to with out section tasks
                if (model.IsSectionChange == true)
                {
                    var CurrentSectionId = model.CurrentSectionId;
                    var PreviousSectionId = model.PreviousSectionId;

                    if (PreviousSectionId > 0 && model.PreviousPriority >= 0)
                    {
                        var TaskList = _taskService.GetAllTaskBySectionId(PreviousSectionId.Value);
                        if (TaskList != null && TaskList.Count() > 0)
                        {
                            var tasks = TaskList.Where(t => t.Priority > model.PreviousPriority && t.Id != model.Id).ToList();
                            if (tasks != null && tasks.Count() > 0)
                            {
                                foreach (var item in tasks)
                                {
                                    item.Priority = item.Priority - 1;
                                    _taskService.UpdateTask(item, item.Id);
                                }
                            }
                        }
                    }

                    // logic for move task in without sections 
                    if (PreviousSectionId == null && CurrentSectionId != null)
                    {
                        var TaskList = _taskService.GetAllTaskByTenantWithOutSection(TenantId);
                        if (TaskList != null && TaskList.Count() > 0)
                        {
                            var tasks = TaskList.Where(t => t.Priority > model.PreviousPriority && t.Id != model.Id).ToList();
                            if (tasks != null && tasks.Count() > 0)
                            {
                                foreach (var item in tasks)
                                {
                                    item.Priority = item.Priority - 1;
                                    _taskService.UpdateTask(item, item.Id);
                                }
                            }
                        }
                    }
                    // end

                    // start logic for one section task move to other section
                    if (CurrentSectionId == null && PreviousSectionId != null)
                    {
                        var TaskList = _taskService.GetAllTaskByTenantWithOutSection(TenantId);
                        if (TaskList != null && TaskList.Count() > 0)
                        {
                            var tasks = TaskList.Where(t => t.Priority >= model.CurrentPriority && t.Id != model.Id).ToList();
                            if (tasks != null && tasks.Count() > 0)
                            {
                                foreach (var item in tasks)
                                {
                                    item.Priority = item.Priority + 1;
                                    _taskService.UpdateTask(item, item.Id);
                                }
                            }
                        }
                    }

                    if (CurrentSectionId > 0 && model.CurrentPriority >= 0)
                    {
                        var TaskList = _taskService.GetAllTaskBySectionId(CurrentSectionId.Value);
                        if (TaskList != null && TaskList.Count() > 0)
                        {
                            var tasks = TaskList.Where(t => t.Priority >= model.CurrentPriority && t.Id != model.Id).ToList();
                            if (tasks != null && tasks.Count() > 0)
                            {
                                foreach (var item in tasks)
                                {
                                    item.Priority = item.Priority + 1;
                                    _taskService.UpdateTask(item, item.Id);
                                }
                            }
                        }
                    }
                    // end
                }
                else if (model.PreviousSectionId != null && model.CurrentSectionId != null)
                {
                    // start logic for task move in one section
                    if (model.CurrentSectionId == model.PreviousSectionId)
                    {
                        var TaskList = _taskService.GetAllTaskBySectionId(model.CurrentSectionId.Value);

                        if (model.CurrentPriority < TaskList.Count())
                        {
                            if (model.CurrentPriority != model.PreviousPriority)
                            {
                                if (model.PreviousPriority < model.CurrentPriority)
                                {
                                    if (TaskList != null && TaskList.Count() > 0)
                                    {
                                        var tasks = TaskList.Where(t => t.Priority > model.PreviousPriority && t.Priority <= model.CurrentPriority && t.Id != model.Id).ToList();
                                        if (tasks != null && tasks.Count() > 0)
                                        {
                                            foreach (var item in tasks)
                                            {
                                                item.Priority = item.Priority - 1;
                                                _taskService.UpdateTask(item, item.Id);
                                            }
                                        }
                                    }
                                }
                                else if (model.PreviousPriority > model.CurrentPriority)
                                {
                                    if (TaskList != null && TaskList.Count() > 0)
                                    {
                                        var tasks = TaskList.Where(t => t.Priority < model.PreviousPriority && t.Priority >= model.CurrentPriority && t.Id != model.Id).ToList();
                                        if (tasks != null && tasks.Count() > 0)
                                        {
                                            foreach (var item in tasks)
                                            {
                                                item.Priority = item.Priority + 1;
                                                _taskService.UpdateTask(item, item.Id);
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
                    var TaskList = _taskService.GetAllTaskByTenantWithOutSection(TenantId);

                    if (model.CurrentPriority < TaskList.Count())
                    {
                        if (model.CurrentPriority != model.PreviousPriority)
                        {
                            if (model.PreviousPriority < model.CurrentPriority)
                            {
                                if (TaskList != null && TaskList.Count() > 0)
                                {
                                    var tasks = TaskList.Where(t => t.Priority > model.PreviousPriority && t.Priority <= model.CurrentPriority && t.Id != model.Id).ToList();
                                    if (tasks != null && tasks.Count() > 0)
                                    {
                                        foreach (var item in tasks)
                                        {
                                            item.Priority = item.Priority - 1;
                                            _taskService.UpdateTask(item, item.Id);
                                        }
                                    }
                                }
                            }
                            else if (model.PreviousPriority > model.CurrentPriority)
                            {
                                if (TaskList != null && TaskList.Count() > 0)
                                {
                                    var tasks = TaskList.Where(t => t.Priority < model.PreviousPriority && t.Priority >= model.CurrentPriority && t.Id != model.Id).ToList();
                                    if (tasks != null && tasks.Count() > 0)
                                    {
                                        foreach (var item in tasks)
                                        {
                                            item.Priority = item.Priority + 1;
                                            _taskService.UpdateTask(item, item.Id);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // end
                }
            }
            return new OperationResult<OneClappTaskDto>(true, System.Net.HttpStatusCode.OK, "", oneClappTaskDtoObj);
        }

    }
}