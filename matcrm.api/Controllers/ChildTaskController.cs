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
    public class ChildTaskController : Controller
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
        private readonly OneClappContext _context;
        private IMapper _mapper;
        private SendEmail sendEmail;
        private int UserId = 0;
        private int TenantId = 0;

        public ChildTaskController(IOneClappTaskService taskService,
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
            OneClappContext context,
            IMapper mapper)
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
            _context = context;
            _mapper = mapper;
            sendEmail = new SendEmail(emailTemplateService, emailLogService, emailConfigService, emailProvider, mapper);
        }

        // Save Child task
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<OneClappChildTaskDto>> Add([FromBody] OneClappChildTaskDto task)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            long weClappChildTaskId = 0;
            matcrm.data.Models.Tables.TaskStatus taskStatusObj = new matcrm.data.Models.Tables.TaskStatus();
            if (UserId != null)
            {
                var userId = UserId;
                var taskStatuses = _taskStatusService.GetStatusByUser(userId);
                if (taskStatuses != null)
                {
                    taskStatusObj = taskStatuses.Where(t => t.IsFinalize == true).FirstOrDefault();
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
                        // durationSeconds = task.Duration.Value
                        durationSeconds = 1
                    };

                    var result = await _weClappService.AddJob(task.ApiKey, task.Tenant, postTimeRecordObj);
                    if (result != null)
                    {
                        weClappChildTaskId = Convert.ToInt64(result.id);
                    }
                }
                else
                {
                    weClappChildTaskId = Convert.ToInt64(task.WeClappTimeRecordId);
                }

                if (weClappChildTaskId != 0)
                {
                    OneClappChildTaskDto oneClappChildTaskObj = new OneClappChildTaskDto();
                    oneClappChildTaskObj.WeClappTimeRecordId = Convert.ToInt64(weClappChildTaskId);
                    oneClappChildTaskObj.OneClappSubTaskId = task.OneClappSubTaskId;
                    oneClappChildTaskObj.IsActive = true;
                    oneClappChildTaskObj.StatusId = task.StatusId;
                    oneClappChildTaskObj.Description = task.Description;
                    oneClappChildTaskObj.StartDate = task.StartDate;
                    oneClappChildTaskObj.EndDate = task.EndDate;
                    oneClappChildTaskObj.TenantId = TenantId;
                    if (task.Id == null)
                    {
                        oneClappChildTaskObj.CreatedBy = UserId;
                    }
                    else
                    {
                        oneClappChildTaskObj.UpdatedBy = UserId;
                    }
                    var childTaskResult = _childTaskService.CheckInsertOrUpdate(oneClappChildTaskObj);
                    if (childTaskResult != null)
                    {
                        oneClappChildTaskObj.Id = childTaskResult.Id;
                    }
                    if (task.StatusId != null && taskStatusObj != null && task.StatusId == taskStatusObj.Id)
                    {
                        // Add logic for if child task with finalize status then update in to weclapp

                        var totaltimeRecord = _childTaskTimeRecordService.GetTotalChildTaskTimeRecord(childTaskResult.Id);
                        var userObj = _userService.GetUserById(UserId);
                        TimeRecord timeRecordObj = new TimeRecord();
                        timeRecordObj.id = task.WeClappTimeRecordId.ToString();
                        timeRecordObj.ticketNumber = task.Ticket;
                        timeRecordObj.durationSeconds = totaltimeRecord;
                        timeRecordObj.description = childTaskResult.Description;
                        timeRecordObj.billable = true;
                        if (userObj != null)
                        {
                            timeRecordObj.userId = userObj.WeClappUserId.ToString();
                        }
                        timeRecordObj.startDate = task.StartAt.Value;
                        if (timeRecordObj != null)
                        {
                            var updateTask = _weClappService.UpdateTimeRecord(task.ApiKey, task.Tenant, timeRecordObj);
                        }

                        // Add logic for if all child task with completed then sub task status also completed automatically 
                        var subTaskId = task.OneClappSubTaskId.Value;
                        var childTasks = _childTaskService.GetAllChildTaskBySubTask(subTaskId);
                        var childTaskCompletedCount = childTasks.Where(t => t.StatusId == taskStatusObj.Id).Count();

                        if (childTasks.Count() == childTaskCompletedCount)
                        {
                            var subTaskObj = _subTaskService.GetSubTaskById(subTaskId);
                            var subTaskObjDto = _mapper.Map<OneClappSubTaskDto>(subTaskObj);
                            subTaskObjDto.StatusId = taskStatusObj.Id;
                            var AddUpdateSubTask = _subTaskService.CheckInsertOrUpdate(subTaskObjDto);

                            var totalSubTaskTimeRecord = _subTaskTimeRecordService.GetTotalSubTaskTimeRecord(AddUpdateSubTask.Id);
                            TimeRecord timeRecordObj1 = new TimeRecord();
                            timeRecordObj1.id = AddUpdateSubTask.WeClappTimeRecordId.ToString();
                            timeRecordObj1.ticketNumber = task.Ticket;
                            timeRecordObj1.durationSeconds = totalSubTaskTimeRecord;
                            timeRecordObj1.description = AddUpdateSubTask.Description;
                            timeRecordObj1.billable = true;
                            if (userObj != null)
                            {
                                timeRecordObj1.userId = userObj.WeClappUserId.ToString();
                            }
                            timeRecordObj1.startDate = task.StartAt.Value;
                            var updateSubTask = _weClappService.UpdateTimeRecord(task.ApiKey, task.Tenant, timeRecordObj1);
                        }
                    }

                    ChildTaskActivity childTaskActivityObj = new ChildTaskActivity();
                    childTaskActivityObj.UserId = UserId;
                    childTaskActivityObj.ChildTaskId = childTaskResult.Id;
                    if (task.Id == null)
                    {
                        childTaskActivityObj.Activity = "Created the task";
                    }
                    else
                    {
                        childTaskActivityObj.Activity = "Updated the task";
                    }
                    var AddUpdate = _childTaskActivityService.CheckInsertOrUpdate(childTaskActivityObj);

                    if (task.AssignedUsers != null && task.AssignedUsers.Count() > 0)
                    {
                        foreach (var userObj in task.AssignedUsers)
                        {
                            OneClappChildTaskUserDto oneClappChildTaskUserDto = new OneClappChildTaskUserDto();
                            oneClappChildTaskUserDto.OneClappChildTaskId = childTaskResult.Id;
                            oneClappChildTaskUserDto.UserId = userObj.UserId;
                            oneClappChildTaskUserDto.TenantId = TenantId;
                            var isExist = _childTaskUserService.IsExistOrNot(oneClappChildTaskUserDto);
                            var childTaskUserObj = _childTaskUserService.CheckInsertOrUpdate(oneClappChildTaskUserDto);
                            if (childTaskUserObj != null)
                            {
                                userObj.Id = childTaskUserObj.Id;
                            }
                            if (!isExist)
                            {
                                var childTaskUserAssignDetails = _userService.GetUserById(childTaskUserObj.UserId.Value);
                                if (childTaskUserAssignDetails != null)
                                    await sendEmail.SendEmailTaskUserAssignNotification(childTaskUserAssignDetails.Email, childTaskUserAssignDetails.FirstName + ' ' + childTaskUserAssignDetails.LastName, task.Description, TenantId);
                                ChildTaskActivity childTaskActivityObj1 = new ChildTaskActivity();
                                childTaskActivityObj1.ChildTaskId = childTaskResult.Id;
                                childTaskActivityObj1.UserId = UserId;
                                childTaskActivityObj1.Activity = "Assigned the user";
                                var AddUpdate1 = _childTaskActivityService.CheckInsertOrUpdate(childTaskActivityObj1);
                            }
                            // var AddUpdate1 = _childTaskActivityService.CheckInsertOrUpdate (childTaskActivityObj1);
                        }
                        oneClappChildTaskObj.AssignedUsers = new List<OneClappChildTaskUser>();
                        oneClappChildTaskObj.AssignedUsers = task.AssignedUsers;
                    }

                    return new OperationResult<OneClappChildTaskDto>(true, System.Net.HttpStatusCode.OK, "Child task saved successfully.", oneClappChildTaskObj);
                }
            }
            OneClappChildTaskDto oneClappChildTaskDto = new OneClappChildTaskDto();
            return new OperationResult<OneClappChildTaskDto>(false, System.Net.HttpStatusCode.OK, "Internal error occured.", oneClappChildTaskDto);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<OneClappChildTaskDto>> Update([FromBody] OneClappChildTaskDto task)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            long weClappChildTaskId = 0;
            matcrm.data.Models.Tables.TaskStatus taskStatusObj = new matcrm.data.Models.Tables.TaskStatus();
            if (UserId != null)
            {
                var userId = UserId;
                var taskStatuses = _taskStatusService.GetStatusByUser(userId);
                if (taskStatuses != null)
                {
                    taskStatusObj = taskStatuses.Where(t => t.IsFinalize == true).FirstOrDefault();
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
                        // durationSeconds = task.Duration.Value
                        durationSeconds = 1
                    };

                    var result = await _weClappService.AddJob(task.ApiKey, task.Tenant, postTimeRecordObj);
                    if (result != null)
                    {
                        weClappChildTaskId = Convert.ToInt64(result.id);
                    }
                }
                else
                {
                    weClappChildTaskId = Convert.ToInt64(task.WeClappTimeRecordId);
                }

                if (weClappChildTaskId != 0)
                {
                    OneClappChildTaskDto oneClappChildTaskObj = new OneClappChildTaskDto();
                    oneClappChildTaskObj.WeClappTimeRecordId = Convert.ToInt64(weClappChildTaskId);
                    oneClappChildTaskObj.OneClappSubTaskId = task.OneClappSubTaskId;
                    oneClappChildTaskObj.IsActive = true;
                    oneClappChildTaskObj.StatusId = task.StatusId;
                    oneClappChildTaskObj.Description = task.Description;
                    oneClappChildTaskObj.StartDate = task.StartDate;
                    oneClappChildTaskObj.EndDate = task.EndDate;
                    oneClappChildTaskObj.TenantId = TenantId;
                    if (task.Id == null)
                    {
                        oneClappChildTaskObj.CreatedBy = UserId;
                    }
                    else
                    {
                        oneClappChildTaskObj.UpdatedBy = UserId;
                    }
                    var childTaskResult = _childTaskService.CheckInsertOrUpdate(oneClappChildTaskObj);
                    if (childTaskResult != null)
                    {
                        oneClappChildTaskObj.Id = childTaskResult.Id;
                    }
                    if (task.StatusId != null && taskStatusObj != null && task.StatusId == taskStatusObj.Id)
                    {
                        // Add logic for if child task with finalize status then update in to weclapp

                        var totaltimeRecord = _childTaskTimeRecordService.GetTotalChildTaskTimeRecord(childTaskResult.Id);
                        var userObj = _userService.GetUserById(UserId);
                        TimeRecord timeRecordObj = new TimeRecord();
                        timeRecordObj.id = task.WeClappTimeRecordId.ToString();
                        timeRecordObj.ticketNumber = task.Ticket;
                        timeRecordObj.durationSeconds = totaltimeRecord;
                        timeRecordObj.description = childTaskResult.Description;
                        timeRecordObj.billable = true;
                        if (userObj != null)
                        {
                            timeRecordObj.userId = userObj.WeClappUserId.ToString();
                        }
                        timeRecordObj.startDate = task.StartAt.Value;
                        if (timeRecordObj != null)
                        {
                            var updateTask = _weClappService.UpdateTimeRecord(task.ApiKey, task.Tenant, timeRecordObj);
                        }

                        // Add logic for if all child task with completed then sub task status also completed automatically 
                        var subTaskId = task.OneClappSubTaskId.Value;
                        var childTasks = _childTaskService.GetAllChildTaskBySubTask(subTaskId);
                        var childTaskCompletedCount = childTasks.Where(t => t.StatusId == taskStatusObj.Id).Count();

                        if (childTasks.Count() == childTaskCompletedCount)
                        {
                            var subTaskObj = _subTaskService.GetSubTaskById(subTaskId);
                            var subTaskObjDto = _mapper.Map<OneClappSubTaskDto>(subTaskObj);
                            subTaskObjDto.StatusId = taskStatusObj.Id;
                            var AddUpdateSubTask = _subTaskService.CheckInsertOrUpdate(subTaskObjDto);

                            var totalSubTaskTimeRecord = _subTaskTimeRecordService.GetTotalSubTaskTimeRecord(AddUpdateSubTask.Id);
                            TimeRecord timeRecordObj1 = new TimeRecord();
                            timeRecordObj1.id = AddUpdateSubTask.WeClappTimeRecordId.ToString();
                            timeRecordObj1.ticketNumber = task.Ticket;
                            timeRecordObj1.durationSeconds = totalSubTaskTimeRecord;
                            timeRecordObj1.description = AddUpdateSubTask.Description;
                            timeRecordObj1.billable = true;
                            if (userObj != null)
                            {
                                timeRecordObj1.userId = userObj.WeClappUserId.ToString();
                            }
                            timeRecordObj1.startDate = task.StartAt.Value;
                            var updateSubTask = _weClappService.UpdateTimeRecord(task.ApiKey, task.Tenant, timeRecordObj1);
                        }
                    }

                    ChildTaskActivity childTaskActivityObj = new ChildTaskActivity();
                    childTaskActivityObj.UserId = UserId;
                    childTaskActivityObj.ChildTaskId = childTaskResult.Id;
                    if (task.Id == null)
                    {
                        childTaskActivityObj.Activity = "Created the task";
                    }
                    else
                    {
                        childTaskActivityObj.Activity = "Updated the task";
                    }
                    var AddUpdate = _childTaskActivityService.CheckInsertOrUpdate(childTaskActivityObj);

                    if (task.AssignedUsers != null && task.AssignedUsers.Count() > 0)
                    {
                        foreach (var userObj in task.AssignedUsers)
                        {
                            OneClappChildTaskUserDto oneClappChildTaskUserDto = new OneClappChildTaskUserDto();
                            oneClappChildTaskUserDto.OneClappChildTaskId = childTaskResult.Id;
                            oneClappChildTaskUserDto.UserId = userObj.UserId;
                            oneClappChildTaskUserDto.TenantId = TenantId;
                            var isExist = _childTaskUserService.IsExistOrNot(oneClappChildTaskUserDto);
                            var childTaskUserObj = _childTaskUserService.CheckInsertOrUpdate(oneClappChildTaskUserDto);
                            if (childTaskUserObj != null)
                            {
                                userObj.Id = childTaskUserObj.Id;
                            }
                            if (!isExist)
                            {
                                var childTaskUserAssignDetails = _userService.GetUserById(childTaskUserObj.UserId.Value);
                                if (childTaskUserAssignDetails != null)
                                    await sendEmail.SendEmailTaskUserAssignNotification(childTaskUserAssignDetails.Email, childTaskUserAssignDetails.FirstName + ' ' + childTaskUserAssignDetails.LastName, task.Description, TenantId);
                                ChildTaskActivity childTaskActivityObj1 = new ChildTaskActivity();
                                childTaskActivityObj1.ChildTaskId = childTaskResult.Id;
                                childTaskActivityObj1.UserId = UserId;
                                childTaskActivityObj1.Activity = "Assigned the user";
                                var AddUpdate1 = _childTaskActivityService.CheckInsertOrUpdate(childTaskActivityObj1);
                            }
                            // var AddUpdate1 = _childTaskActivityService.CheckInsertOrUpdate (childTaskActivityObj1);
                        }
                        oneClappChildTaskObj.AssignedUsers = new List<OneClappChildTaskUser>();
                        oneClappChildTaskObj.AssignedUsers = task.AssignedUsers;
                    }

                    return new OperationResult<OneClappChildTaskDto>(true, System.Net.HttpStatusCode.OK, "Child task saved successfully.", oneClappChildTaskObj);
                }
            }
            OneClappChildTaskDto oneClappChildTaskDto = new OneClappChildTaskDto();
            return new OperationResult<OneClappChildTaskDto>(false, System.Net.HttpStatusCode.OK, "Internal error occured.", oneClappChildTaskDto);
        }

        // Assign child task to users
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<OneClappChildTaskUser>> AssignToUser([FromBody] OneClappChildTaskUserDto childTaskUser)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            childTaskUser.UserId = UserId;
            childTaskUser.TenantId = TenantId;
            var oneClappChildTaskUserObj = _childTaskUserService.CheckInsertOrUpdate(childTaskUser);
            return new OperationResult<OneClappChildTaskUser>(true, System.Net.HttpStatusCode.OK, "", oneClappChildTaskUserObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<ChildTaskTimeRecord>> AddUpdateTimeRecord([FromBody] ChildTaskTimeRecordDto Model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            Model.UserId = UserId;
            var childTaskTimeRecord = _childTaskTimeRecordService.CheckInsertOrUpdate(Model);
            ChildTaskActivity childTaskActivityObj = new ChildTaskActivity();
            childTaskActivityObj.ChildTaskId = childTaskTimeRecord.ChildTaskId;
            childTaskActivityObj.UserId = UserId;
            childTaskActivityObj.Activity = "Added time record";
            var AddUpdate1 = _childTaskActivityService.CheckInsertOrUpdate(childTaskActivityObj);
            return new OperationResult<ChildTaskTimeRecord>(true, System.Net.HttpStatusCode.OK, "", childTaskTimeRecord);
        }

        //    [Authorize (Roles = "TenantManager,TenantAdmin, TenantUser")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<OperationResult<List<ChildTaskAttachment>>> UploadFiles([FromForm] ChildTaskAttachmentDto model)
        {

            List<ChildTaskAttachment> childTaskAttachmentList = new List<ChildTaskAttachment>();

            if (model.FileList == null) throw new Exception("File is null");
            if (model.FileList.Length == 0) throw new Exception("File is empty");

            ChildTaskActivity childTaskActivityObj = new ChildTaskActivity();
            childTaskActivityObj.ChildTaskId = model.ChildTaskId;
            childTaskActivityObj.UserId = model.UserId;
            childTaskActivityObj.Activity = "Uploaded document";
            if (model.FileList != null)
            {
                foreach (IFormFile file in model.FileList)
                {
                    // full path to file in temp location
                    //var dirPath = _hostingEnvironment.WebRootPath + "\\ChildTaskUpload";
                    var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.ChildTaskUploadDirPath;

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
                            return new OperationResult<List<ChildTaskAttachment>>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                        }
                    }

                    using (var oStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        await file.CopyToAsync(oStream);
                    }

                    ChildTaskAttachmentDto childTaskAttchmentObj = new ChildTaskAttachmentDto();
                    childTaskAttchmentObj.Name = fileName;
                    childTaskAttchmentObj.ChildTaskId = model.ChildTaskId;
                    childTaskAttchmentObj.UserId = model.UserId;
                    var addedItem = _childTaskAttachmentService.CheckInsertOrUpdate(childTaskAttchmentObj);
                    childTaskAttachmentList.Add(addedItem);
                }
            }
            var AddUpdate = _childTaskActivityService.CheckInsertOrUpdate(childTaskActivityObj);
            await _hubContext.Clients.All.OnUploadTaskDocumentEventEmit(model.ChildTaskId);

            return new OperationResult<List<ChildTaskAttachment>>(true, System.Net.HttpStatusCode.OK, "", childTaskAttachmentList);
        }

        // [AllowAnonymous]
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{ChildTaskId}")]
        public async Task<OperationResult<List<ChildTaskAttachment>>> Documents(long ChildTaskId)
        {

            List<ChildTaskAttachment> childTaskAttachmentList = new List<ChildTaskAttachment>();
            childTaskAttachmentList = _childTaskAttachmentService.GetAllByChildTaskId(ChildTaskId);

            return new OperationResult<List<ChildTaskAttachment>>(true, System.Net.HttpStatusCode.OK, "", childTaskAttachmentList);
        }

        // [Authorize (Roles = "TenantManager,TenantAdmin, TenantUser")]
        [AllowAnonymous]
        [HttpGet("{ChildTaskAttachmentId}")]
        public FileResult Document(long ChildTaskAttachmentId)
        {

            var ChildTaskAttachmentObj = _childTaskAttachmentService.GetChildTaskAttachmentById(ChildTaskAttachmentId);

            // full path to file in temp location
            //var dirPath = _hostingEnvironment.WebRootPath + "\\SubTaskUpload";
            var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.SubTaskUploadDirPath;
            var filePath = dirPath + "\\" + "default.png";

            if (ChildTaskAttachmentObj != null && !string.IsNullOrEmpty(ChildTaskAttachmentObj.Name))
            {
                filePath = dirPath + "\\" + ChildTaskAttachmentObj.Name;
                Byte[] b = System.IO.File.ReadAllBytes(filePath);
                return File(b, "*");
            }
            return null;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete]
        public async Task<OperationResult<OneClappChildTaskDto>> Remove([FromBody] OneClappChildTaskDto model)
        {
            if (model.Id != null)
            {
                ClaimsPrincipal user = this.User as ClaimsPrincipal;
                UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

                var childTaskId = model.Id.Value;

                var documents = await _childTaskAttachmentService.DeleteAttachmentByChildTaskId(childTaskId);

                // Remove child task documents from folder

                foreach (var childTaskDoc in documents)
                {

                    //var dirPath = _hostingEnvironment.WebRootPath + "\\ChildTaskUpload";
                    var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.ChildTaskUploadDirPath;
                    var filePath = dirPath + "\\" + childTaskDoc.Name;

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(Path.Combine(filePath));
                    }
                }

                var comments = _childTaskCommentService.DeleteCommentByChildTaskId(childTaskId);

                var timeRecords = _childTaskTimeRecordService.DeleteTimeRecordByChildTaskId(childTaskId);

                var taskUsers = _childTaskUserService.DeleteByChildTaskId(childTaskId);

                var childTaskActivities = _childTaskActivityService.DeleteChildTaskActivityByChildTaskId(childTaskId);

                var childTaskToDelete = _childTaskService.Delete(childTaskId);

                ChildTaskActivity childTaskActivityObj = new ChildTaskActivity();
                childTaskActivityObj.ChildTaskId = childTaskId;
                childTaskActivityObj.UserId = UserId;
                childTaskActivityObj.Activity = "Removed the task";
                var AddUpdate = _childTaskActivityService.CheckInsertOrUpdate(childTaskActivityObj);

                return new OperationResult<OneClappChildTaskDto>(true, System.Net.HttpStatusCode.OK, "Child task deleted successfully", model);
            }
            else
            {
                return new OperationResult<OneClappChildTaskDto>(false, System.Net.HttpStatusCode.OK, "Child task id null", model);
            }
        }

        // Get Child Task Detail by Id
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{ChildTaskId}")]
        public async Task<OperationResult<OneClappChildTaskVM>> Detail(long ChildTaskId)
        {
            OneClappChildTaskVM oneClappChildTaskVMObj = new OneClappChildTaskVM();

            var AllUsers = _userService.GetAll();

            var childTaskObj = _childTaskService.GetChildTaskById(ChildTaskId);
            oneClappChildTaskVMObj = _mapper.Map<OneClappChildTaskVM>(childTaskObj);

            var childTaskTotalDuration = _childTaskTimeRecordService.GetTotalChildTaskTimeRecord(ChildTaskId);
            oneClappChildTaskVMObj.Duration = childTaskTotalDuration;

            // For child task assign users
            var assignChildTaskUsers = _childTaskUserService.GetAssignUsersByChildTask(ChildTaskId);
            if (assignChildTaskUsers != null && assignChildTaskUsers.Count > 0)
            {
                var oneClappChildTaskUserDtoList = _mapper.Map<List<OneClappChildTaskUserDto>>(assignChildTaskUsers);
                if (oneClappChildTaskVMObj.AssignedUsers == null)
                {
                    oneClappChildTaskVMObj.AssignedUsers = new List<OneClappChildTaskUserDto>();
                }
                if (oneClappChildTaskUserDtoList != null && oneClappChildTaskUserDtoList.Count() > 0)
                {
                    foreach (var assignChildTaskUser in oneClappChildTaskUserDtoList)
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
                }
                oneClappChildTaskVMObj.AssignedUsers = oneClappChildTaskUserDtoList;
            }

            // For child task documents
            var childTaskDocuments = _childTaskAttachmentService.GetAllByChildTaskId(oneClappChildTaskVMObj.Id);
            if (childTaskDocuments != null && childTaskDocuments.Count > 0)
            {
                if (oneClappChildTaskVMObj.Documents == null)
                {
                    oneClappChildTaskVMObj.Documents = new List<ChildTaskAttachment>();
                }
                oneClappChildTaskVMObj.Documents = childTaskDocuments;
            }

            // For child task commnets
            var childTaskComments = _childTaskCommentService.GetAllByChildTaskId(oneClappChildTaskVMObj.Id);
            if (childTaskComments != null && childTaskComments.Count > 0)
            {
                var childTaskCommentDtoList = _mapper.Map<List<ChildTaskCommentDto>>(childTaskComments);
                if (childTaskCommentDtoList != null && childTaskCommentDtoList.Count() > 0)
                {
                    foreach (var childTaskCommentObj in childTaskCommentDtoList)
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
                }
                if (oneClappChildTaskVMObj.Comments == null)
                {
                    oneClappChildTaskVMObj.Comments = new List<ChildTaskCommentDto>();
                }
                oneClappChildTaskVMObj.Comments = childTaskCommentDtoList;
            }

            // For task activities
            var childTaskActivities = _childTaskActivityService.GetAllByChildTaskId(oneClappChildTaskVMObj.Id);
            if (childTaskActivities != null && childTaskActivities.Count > 0)
            {
                var childTaskActivityDtoList = _mapper.Map<List<ChildTaskActivityDto>>(childTaskActivities);
                if (childTaskActivityDtoList != null && childTaskActivityDtoList.Count() > 0)
                {
                    foreach (var childTaskActivityObj in childTaskActivityDtoList)
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
                }
                if (oneClappChildTaskVMObj.Activities == null)
                {
                    oneClappChildTaskVMObj.Activities = new List<ChildTaskActivityDto>();
                }
                oneClappChildTaskVMObj.Activities = childTaskActivityDtoList;
            }
            return new OperationResult<OneClappChildTaskVM>(true, System.Net.HttpStatusCode.OK, "", oneClappChildTaskVMObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{ChildTaskId}")]
        public async Task<OperationResult<List<ChildTaskActivityDto>>> History(long ChildTaskId)
        {
            List<ChildTaskActivityDto> childTaskActivityDtoList = new List<ChildTaskActivityDto>();
            var AllUsers = _userService.GetAll();
            var activities = _childTaskActivityService.GetAllByChildTaskId(ChildTaskId);
            childTaskActivityDtoList = _mapper.Map<List<ChildTaskActivityDto>>(activities);
            if (childTaskActivityDtoList != null)
            {
                foreach (var item in childTaskActivityDtoList)
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
            return new OperationResult<List<ChildTaskActivityDto>>(true, System.Net.HttpStatusCode.OK, "", childTaskActivityDtoList);
        }

        // Document delete method - Shakti
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete("{AttachmentId}")]
        public async Task<OperationResult<ChildTaskAttachmentDto>> RemoveDocument(long AttachmentId)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            var childDocument = await _childTaskAttachmentService.DeleteChildTaskAttachmentById(AttachmentId);

            // Remove task documents from folder
            if (childDocument != null)
            {
                //var dirPath = _hostingEnvironment.WebRootPath + "\\ChildTaskUpload";
                var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.ChildTaskUploadDirPath;
                var filePath = dirPath + "\\" + childDocument.Name;

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(Path.Combine(filePath));
                }
                ChildTaskActivity childTaskActivityObj = new ChildTaskActivity();
                childTaskActivityObj.ChildTaskId = childDocument.ChildTaskId;
                childTaskActivityObj.UserId = UserId;
                childTaskActivityObj.Activity = "Removed an attachment";
                var AddUpdate = _childTaskActivityService.CheckInsertOrUpdate(childTaskActivityObj);
                await _hubContext.Clients.All.OnUploadTaskDocumentEventEmit(childDocument.ChildTaskId);
                return new OperationResult<ChildTaskAttachmentDto>(true, System.Net.HttpStatusCode.OK, "Doccument removed successfully");
            }
            else
            {
                return new OperationResult<ChildTaskAttachmentDto>(false, System.Net.HttpStatusCode.OK, "Doccument not found");
            }
        }

        // Assigned Child task user delete method - Shakti
        [Authorize(Roles = "TenantManager,TenantAdmin, TenantUser")]
        [HttpDelete("{AssignUserId}")]
        public async Task<OperationResult<OneClappChildTaskUserDto>> RemoveAssignUser(long AssignUserId)
        {

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var assignUserRemove = await _childTaskUserService.DeleteAssignedChildTaskUser(AssignUserId);

            if (assignUserRemove != null)
            {
                ChildTaskActivity childTaskActivityObj = new ChildTaskActivity();
                childTaskActivityObj.ChildTaskId = assignUserRemove.OneClappChildTaskId;
                childTaskActivityObj.UserId = UserId;
                childTaskActivityObj.Activity = "Removed assign user";
                var AddUpdate = _childTaskActivityService.CheckInsertOrUpdate(childTaskActivityObj);

                var removeChildTaskUserDetails = _userService.GetUserById(assignUserRemove.UserId.Value);
                var childTaskDetail = _childTaskService.GetChildTaskById(assignUserRemove.OneClappChildTaskId.Value);
                if (childTaskDetail != null && removeChildTaskUserDetails != null)
                {
                    await sendEmail.SendEmailRemoveTaskUserAssignNotification(removeChildTaskUserDetails.Email, removeChildTaskUserDetails.FirstName + ' ' + removeChildTaskUserDetails.LastName, childTaskDetail.Description, TenantId);
                    return new OperationResult<OneClappChildTaskUserDto>(true, System.Net.HttpStatusCode.OK, "User removed successfully from child task.");
                }
                else
                {
                    return new OperationResult<OneClappChildTaskUserDto>(false, System.Net.HttpStatusCode.OK, "Something went wrong");
                }
            }

            return new OperationResult<OneClappChildTaskUserDto>(false, System.Net.HttpStatusCode.OK, "Remove user not found");
        }

        // [AllowAnonymous]
        // [HttpGet("{ChildTaskAttachmentId}")]
        // public async Task<OperationResult<string>> Document(long childTaskAttachmentId)
        // {
        //     var childTaskAttachmentObj = _childTaskAttachmentService.GetChildTaskAttachmentById(childTaskAttachmentId);

        //     var dirPath = _hostingEnvironment.WebRootPath + "\\ChildTaskUpload";
        //     String file = "";
        //     if (childTaskAttachmentObj != null && !string.IsNullOrEmpty(childTaskAttachmentObj.Name))
        //     {
        //         var filePath = dirPath + "\\" + childTaskAttachmentObj.Name;
        //         Byte[] newBytes = System.IO.File.ReadAllBytes(filePath);
        //         file = Convert.ToBase64String(newBytes);
        //     }
        //     if (file != "")
        //     {
        //         return new OperationResult<string>(true, System.Net.HttpStatusCode.OK, "File received successfully", file);
        //     }
        //     else
        //     {
        //         return new OperationResult<string>(false, System.Net.HttpStatusCode.OK, "Issue in downloading file.");
        //     }

        // }

    }
}