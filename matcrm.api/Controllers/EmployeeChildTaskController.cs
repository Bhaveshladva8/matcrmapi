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

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class EmployeeChildTaskController : Controller
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
        private readonly IMateChildTaskCommentService _mateChildTaskCommentService;
        private readonly IMateCommentAttachmentService _mateCommentAttachmentService;
        private readonly IMateCommentService _mateCommentService;
        private readonly IMateTimeRecordService _mateTimeRecordService;
        private readonly IMateChildTaskTimeRecordService _mateChildTaskTimeRecordService;
        private readonly IStatusService _statusService;
        private IMapper _mapper;
        private SendEmail sendEmail;
        private int UserId = 0;
        private int TenantId = 0;

        public EmployeeChildTaskController(IEmployeeTaskService employeeTaskService,
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
            IMateChildTaskCommentService mateChildTaskCommentService,
            IMateCommentAttachmentService mateCommentAttachmentService,
            IMateCommentService mateCommentService,
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
            _context = context;
            _mateChildTaskCommentService = mateChildTaskCommentService;
            _mateCommentAttachmentService = mateCommentAttachmentService;
            _mateCommentService = mateCommentService;
            _mateChildTaskTimeRecordService = mateChildTaskTimeRecordService;
            _statusService = statusService;
            _mapper = mapper;
            sendEmail = new SendEmail(emailTemplateService, emailLogService, emailConfigService, emailProvider, mapper);
        }

        // Save Child task
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<EmployeeChildTaskAddUpdateResponse>> Add([FromBody] AddUpdateEmployeeChildTaskRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            EmployeeChildTaskAddUpdateResponse response = new EmployeeChildTaskAddUpdateResponse();
            EmployeeTaskStatus employeeTaskStatusObj = new EmployeeTaskStatus();
            EmployeeChildTaskDto employeeChildTaskDto = _mapper.Map<EmployeeChildTaskDto>(requestModel);
            // var userId = UserId;
            // var employeeTaskStatuses = _employeeTaskStatusService.GetStatusByUser(UserId);
            // if (employeeTaskStatuses != null)
            // {
            //     employeeTaskStatusObj = employeeTaskStatuses.Where(t => t.IsFinalize == true).FirstOrDefault();
            // }


            // if (!string.IsNullOrEmpty(task.ApiKey))
            // {
            //     if (task.WeClappTimeRecordId == null)
            //     {
            // PostTimeRecord timeRecord = new PostTimeRecord()
            // {
            //     description = task.Description,
            //     startDate = task.StartAt.Value,
            //     ticketNumber = task.Ticket,
            //     // durationSeconds = task.Duration.Value
            //     durationSeconds = 1
            // };

            // var result = await _weClappService.AddJob(task.ApiKey, task.Tenant, timeRecord);
            // if (result != null)
            // {
            //     weClappChildTaskId = Convert.ToInt64(result.id);
            // }
            // }
            // else
            // {
            //     weClappChildTaskId = Convert.ToInt64(task.WeClappTimeRecordId);
            // }

            // if (weClappChildTaskId != 0)
            // {

            // EmployeeChildTaskDto childTaskObj = new EmployeeChildTaskDto();
            // // childTaskObj.WeClappTimeRecordId = Convert.ToInt64(weClappChildTaskId);
            // childTaskObj.EmployeeSubTaskId = task.EmployeeSubTaskId;
            // childTaskObj.IsActive = true;
            // childTaskObj.StatusId = task.StatusId;
            // childTaskObj.Description = task.Description;
            // childTaskObj.StartDate = task.StartDate;
            // childTaskObj.EndDate = task.EndDate;

            employeeChildTaskDto.CreatedBy = UserId;
            employeeChildTaskDto.IsActive = true;
            var childTaskResult = await _employeeChildTaskService.CheckInsertOrUpdate(employeeChildTaskDto);
            if (childTaskResult != null)
            {
                employeeChildTaskDto.Id = childTaskResult.Id;
            }
            // if (employeeChildTaskDto.EmployeeSubTaskId != null && employeeTaskStatusObj != null && employeeChildTaskDto.StatusId == employeeTaskStatusObj.Id)
            // {
            //     // Add logic for if child task with finalize status then update in to weclapp

            //     var totalTimeRecord = _employeeChildTaskTimeRecordService.GetTotalEmployeeChildTaskTimeRecord(childTaskResult.Id);
            //     var userObj = _userService.GetUserById(UserId);
            //     // TimeRecord timeRecord = new TimeRecord();
            //     // timeRecord.id = task.WeClappTimeRecordId.ToString();
            //     // // timeRecord.ticketNumber = task.Ticket;
            //     // timeRecord.durationSeconds = totaltimeRecord;
            //     // timeRecord.description = childTaskResult.Description;
            //     // timeRecord.billable = true;
            //     // if (userObj != null)
            //     // {
            //     //     timeRecord.userId = userObj.WeClappUserId.ToString();
            //     // }
            //     // timeRecord.startDate = task.StartAt.Value;
            //     // var updateTask = _weClappService.UpdateTimeRecord(task.ApiKey, task.Tenant, timeRecord);

            //     // Add logic for if all child task with completed then sub task status also completed automatically 
            //     var subTaskId = employeeChildTaskDto.EmployeeSubTaskId.Value;
            //     var childTasks = _employeeChildTaskService.GetAllChildTaskBySubTask(subTaskId);
            //     var childTaskCompletedCount = childTasks.Where(t => t.StatusId == employeeTaskStatusObj.Id).Count();

            //     // if (childTasks.Count() == childTaskCompletedCount)
            //     // {
            //     //     var subTaskObj = _employeeSubTaskService.GetSubTaskById(subTaskId);
            //     //     var subTaskObjDto = _mapper.Map<EmployeeSubTaskDto>(subTaskObj);
            //     //     subTaskObjDto.StatusId = finalStatus.Id;
            //     //     var AddUpdateSubTask = _employeeSubTaskService.CheckInsertOrUpdate(subTaskObjDto);

            //     //     var totalSubTaskTimeRecord = _employeeSubTaskTimeRecordService.GetTotalSubTaskTimeRecord(AddUpdateSubTask.Id);
            //     //     TimeRecord subTimeRecord = new TimeRecord();
            //     //     subTimeRecord.id = AddUpdateSubTask.WeClappTimeRecordId.ToString();
            //     //     subTimeRecord.ticketNumber = task.Ticket;
            //     //     subTimeRecord.durationSeconds = totalSubTaskTimeRecord;
            //     //     subTimeRecord.description = AddUpdateSubTask.Description;
            //     //     subTimeRecord.billable = true;
            //     //     if (userObj != null)
            //     //     {
            //     //         subTimeRecord.userId = userObj.WeClappUserId.ToString();
            //     //     }
            //     //     subTimeRecord.startDate = task.StartAt.Value;
            //     //     var updateSubTask = _weClappService.UpdateTimeRecord(task.ApiKey, task.Tenant, subTimeRecord);
            //     // }
            // }

            EmployeeChildTaskActivity employeeChildTaskActivityObj = new EmployeeChildTaskActivity();
            employeeChildTaskActivityObj.UserId = UserId;
            employeeChildTaskActivityObj.EmployeeChildTaskId = childTaskResult.Id;
            employeeChildTaskActivityObj.Activity = Enums.EmployeeChidTaskActivityEnum.Childtask_Created.ToString().Replace("_", " ");

            var AddUpdate = await _employeeChildTaskActivityService.CheckInsertOrUpdate(employeeChildTaskActivityObj);

            if (employeeChildTaskDto.AssignedUsers != null && employeeChildTaskDto.AssignedUsers.Count() > 0)
            {
                foreach (var userObj in employeeChildTaskDto.AssignedUsers)
                {
                    EmployeeChildTaskUserDto employeeChildTaskUserDto = new EmployeeChildTaskUserDto();
                    employeeChildTaskUserDto.EmployeeChildTaskId = childTaskResult.Id;
                    employeeChildTaskUserDto.UserId = userObj.UserId;
                    var isExist = _employeeChildTaskUserService.IsExistOrNot(employeeChildTaskUserDto);
                    var employeeChildTaskUserObj = _employeeChildTaskUserService.CheckInsertOrUpdate(employeeChildTaskUserDto);
                    if (employeeChildTaskUserObj != null)
                    {
                        userObj.Id = employeeChildTaskUserObj.Id;
                    }
                    if (!isExist)
                    {
                        if (employeeChildTaskUserDto.UserId != null)
                        {
                            var childTaskUserAssignDetails = _userService.GetUserById(employeeChildTaskUserDto.UserId.Value);
                            if (childTaskUserAssignDetails != null)
                                await sendEmail.SendEmailEmployeeTaskUserAssignNotification(childTaskUserAssignDetails.Email, childTaskUserAssignDetails.FirstName + ' ' + childTaskUserAssignDetails.LastName, employeeChildTaskDto.Description, TenantId, childTaskResult.Id);
                            EmployeeChildTaskActivity employeeChildTaskActivityObj1 = new EmployeeChildTaskActivity();
                            employeeChildTaskActivityObj1.EmployeeChildTaskId = childTaskResult.Id;
                            employeeChildTaskActivityObj1.UserId = UserId;
                            employeeChildTaskActivityObj1.Activity = Enums.EmployeeChidTaskActivityEnum.Childtask_assigned_to_user.ToString().Replace("_", " ");
                            var AddUpdate1 = await _employeeChildTaskActivityService.CheckInsertOrUpdate(employeeChildTaskActivityObj1);
                        }
                    }
                    // var AddUpdate1 = awaiy _employeeChildTaskActivityService.CheckInsertOrUpdate (EmployeeChildTaskActivityObj1);
                }
                employeeChildTaskDto.AssignedUsers = new List<EmployeeChildTaskUser>();
            }
            response = _mapper.Map<EmployeeChildTaskAddUpdateResponse>(employeeChildTaskDto);
            return new OperationResult<EmployeeChildTaskAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Child task saved successfully.", response);
        }

        // Save Child task
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<EmployeeChildTaskAddUpdateResponse>> Update([FromBody] AddUpdateEmployeeChildTaskRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            EmployeeChildTaskAddUpdateResponse response = new EmployeeChildTaskAddUpdateResponse();
            EmployeeTaskStatus employeeTaskStatusObj = new EmployeeTaskStatus();
            EmployeeChildTaskDto employeeChildTaskDto = _mapper.Map<EmployeeChildTaskDto>(requestModel);
            var userId = UserId;
            var employeeTaskStatuses = _employeeTaskStatusService.GetStatusByUser(UserId);
            if (employeeTaskStatuses != null)
            {
                employeeTaskStatusObj = employeeTaskStatuses.Where(t => t.IsFinalize == true).FirstOrDefault();
            }

            if (employeeChildTaskDto.Id == null)
            {
                employeeChildTaskDto.CreatedBy = UserId;
            }
            else
            {
                employeeChildTaskDto.UpdatedBy = UserId;
            }
            employeeChildTaskDto.IsActive = true;
            var childTaskResult = _employeeChildTaskService.CheckInsertOrUpdate(employeeChildTaskDto);
            if (childTaskResult != null)
            {
                employeeChildTaskDto.Id = childTaskResult.Id;
            }
            if (employeeChildTaskDto.EmployeeSubTaskId != null && employeeTaskStatusObj != null && employeeChildTaskDto.StatusId == employeeTaskStatusObj.Id)
            {
                // Add logic for if child task with finalize status then update in to weclapp

                var totalTimeRecord = _employeeChildTaskTimeRecordService.GetTotalEmployeeChildTaskTimeRecord(childTaskResult.Id);
                var userObj = _userService.GetUserById(UserId);

                // Add logic for if all child task with completed then sub task status also completed automatically 
                var subTaskId = employeeChildTaskDto.EmployeeSubTaskId.Value;
                var childTasks = _employeeChildTaskService.GetAllChildTaskBySubTask(subTaskId);
                var childTaskCompletedCount = childTasks.Where(t => t.StatusId == employeeTaskStatusObj.Id).Count();

            }

            EmployeeChildTaskActivity employeeChildTaskActivityObj = new EmployeeChildTaskActivity();
            employeeChildTaskActivityObj.UserId = UserId;
            employeeChildTaskActivityObj.EmployeeChildTaskId = childTaskResult.Id;
            employeeChildTaskActivityObj.Activity = Enums.EmployeeChidTaskActivityEnum.Childtask_Updated.ToString().Replace("_", " ");

            var AddUpdate = await _employeeChildTaskActivityService.CheckInsertOrUpdate(employeeChildTaskActivityObj);

            if (employeeChildTaskDto.AssignedUsers != null && employeeChildTaskDto.AssignedUsers.Count() > 0)
            {
                foreach (var userObj in employeeChildTaskDto.AssignedUsers)
                {
                    EmployeeChildTaskUserDto employeeChildTaskUserDto = new EmployeeChildTaskUserDto();
                    employeeChildTaskUserDto.EmployeeChildTaskId = childTaskResult.Id;
                    employeeChildTaskUserDto.UserId = userObj.UserId;
                    var isExist = _employeeChildTaskUserService.IsExistOrNot(employeeChildTaskUserDto);
                    var employeeChildTaskUserObj = _employeeChildTaskUserService.CheckInsertOrUpdate(employeeChildTaskUserDto);
                    if (employeeChildTaskUserObj != null)
                    {
                        userObj.Id = employeeChildTaskUserObj.Id;
                    }
                    if (!isExist)
                    {
                        if (employeeChildTaskUserDto.UserId != null)
                        {
                            var childTaskUserAssignDetails = _userService.GetUserById(employeeChildTaskUserDto.UserId.Value);
                            if (childTaskUserAssignDetails != null)
                                await sendEmail.SendEmailEmployeeTaskUserAssignNotification(childTaskUserAssignDetails.Email, childTaskUserAssignDetails.FirstName + ' ' + childTaskUserAssignDetails.LastName, employeeChildTaskDto.Description, TenantId, childTaskResult.Id);
                            EmployeeChildTaskActivity employeeChildTaskActivityObj1 = new EmployeeChildTaskActivity();
                            employeeChildTaskActivityObj1.EmployeeChildTaskId = childTaskResult.Id;
                            employeeChildTaskActivityObj1.UserId = UserId;
                            employeeChildTaskActivityObj1.Activity = Enums.EmployeeChidTaskActivityEnum.Childtask_assigned_to_user.ToString().Replace("_", " ");
                            var AddUpdate1 = await _employeeChildTaskActivityService.CheckInsertOrUpdate(employeeChildTaskActivityObj1);
                        }
                    }
                    // var AddUpdate1 = awaiy _employeeChildTaskActivityService.CheckInsertOrUpdate (EmployeeChildTaskActivityObj1);
                }
                employeeChildTaskDto.AssignedUsers = new List<EmployeeChildTaskUser>();
            }
            response = _mapper.Map<EmployeeChildTaskAddUpdateResponse>(employeeChildTaskDto);
            return new OperationResult<EmployeeChildTaskAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Child task saved successfully.", response);
        }

        // Assign child task to users
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<EmployeeChildTaskUser>> AssignToUser([FromBody] EmployeeChildTaskUserDto childTaskUser)
        {
            var assignChildTaskUserObj = await _employeeChildTaskUserService.CheckInsertOrUpdate(childTaskUser);
            return new OperationResult<EmployeeChildTaskUser>(true, System.Net.HttpStatusCode.OK, "", assignChildTaskUserObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<AddUpdateEmployeeChildTaskTimeRecordResponse>> TimeRecord([FromBody] AddUpdateEmployeeChildTaskTimeRecordRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            EmployeeChildTaskTimeRecordDto employeeChildTaskTimeRecordDto = _mapper.Map<EmployeeChildTaskTimeRecordDto>(requestModel);
            AddUpdateEmployeeChildTaskTimeRecordResponse response = new AddUpdateEmployeeChildTaskTimeRecordResponse();
            if (employeeChildTaskTimeRecordDto.Duration != null && employeeChildTaskTimeRecordDto.EmployeeChildTaskId != null)
            {
                var childTaskTotalDuration = _employeeChildTaskTimeRecordService.GetTotalEmployeeChildTaskTimeRecord(employeeChildTaskTimeRecordDto.EmployeeChildTaskId.Value);
                if (childTaskTotalDuration >= 0)
                {
                    employeeChildTaskTimeRecordDto.Duration = employeeChildTaskTimeRecordDto.Duration - childTaskTotalDuration;
                }
                employeeChildTaskTimeRecordDto.UserId = UserId;
                var employeeChildTaskTimeRecordObj = await _employeeChildTaskTimeRecordService.CheckInsertOrUpdate(employeeChildTaskTimeRecordDto);
                EmployeeChildTaskActivity employeeChildTaskActivityObj = new EmployeeChildTaskActivity();
                employeeChildTaskActivityObj.EmployeeChildTaskId = employeeChildTaskTimeRecordObj.EmployeeChildTaskId;
                employeeChildTaskActivityObj.UserId = UserId;
                employeeChildTaskActivityObj.Activity = Enums.EmployeeChidTaskActivityEnum.Childtask_time_record_created.ToString().Replace("_", " ");
                var AddUpdate1 = await _employeeChildTaskActivityService.CheckInsertOrUpdate(employeeChildTaskActivityObj);

                response = _mapper.Map<AddUpdateEmployeeChildTaskTimeRecordResponse>(employeeChildTaskTimeRecordObj);
                return new OperationResult<AddUpdateEmployeeChildTaskTimeRecordResponse>(true, System.Net.HttpStatusCode.OK, "", response);
            }
            else
            {
                var message = "EmployeeChildTaskId can not be null";
                if (requestModel.Duration == null)
                {
                    message = "Duration can not be null";
                }
                return new OperationResult<AddUpdateEmployeeChildTaskTimeRecordResponse>(false, System.Net.HttpStatusCode.OK, message, response);
            }
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<EmployeeChildTaskAttachment>> UploadFiles([FromForm] EmployeeChildTaskUploadFileRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            List<EmployeeChildTaskAttachment> employeeChildTaskAttachmentsList = new List<EmployeeChildTaskAttachment>();

            if (model.FileList == null) throw new Exception("File is null");
            if (model.FileList.Length == 0) throw new Exception("File is empty");

            EmployeeChildTaskActivity employeeChildTaskActivityObj = new EmployeeChildTaskActivity();
            employeeChildTaskActivityObj.EmployeeChildTaskId = model.EmployeeChildTaskId;
            employeeChildTaskActivityObj.UserId = UserId;
            employeeChildTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Document_uploaded.ToString().Replace("_", " ");

            foreach (IFormFile file in model.FileList)
            {
                // full path to file in temp location
                //var dirPath = _hostingEnvironment.WebRootPath + "\\EmployeeChildTaskUpload";
                var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.EmployeeChildTaskUploadDirPath;

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
                        return new OperationResult<EmployeeChildTaskAttachment>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                    }
                }
                using (var oStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    await file.CopyToAsync(oStream);
                }

                EmployeeChildTaskAttachmentDto employeeChildTaskAttachmentDto = new EmployeeChildTaskAttachmentDto();
                employeeChildTaskAttachmentDto.Name = fileName;
                employeeChildTaskAttachmentDto.EmployeeChildTaskId = model.EmployeeChildTaskId;
                employeeChildTaskAttachmentDto.UserId = UserId;
                var addedItem = _employeeChildTaskAttachmentService.CheckInsertOrUpdate(employeeChildTaskAttachmentDto);
                employeeChildTaskAttachmentsList.Add(addedItem);
            }
            var AddUpdate = await _employeeChildTaskActivityService.CheckInsertOrUpdate(employeeChildTaskActivityObj);
            await _hubContext.Clients.All.OnUploadEmployeeTaskDocumentEventEmit(model.EmployeeChildTaskId);

            return new OperationResult<EmployeeChildTaskAttachment>(true, System.Net.HttpStatusCode.OK, "");
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{EmployeeChildTaskId}")]
        public async Task<OperationResult<List<EmployeeChildTaskAttachmentListResponse>>> Documents(long EmployeeChildTaskId)
        {

            List<EmployeeChildTaskAttachment> employeeChildTaskAttachmentList = new List<EmployeeChildTaskAttachment>();
            employeeChildTaskAttachmentList = _employeeChildTaskAttachmentService.GetAllByChildTaskId(EmployeeChildTaskId);
            var response = _mapper.Map<List<EmployeeChildTaskAttachmentListResponse>>(employeeChildTaskAttachmentList);
            return new OperationResult<List<EmployeeChildTaskAttachmentListResponse>>(true, System.Net.HttpStatusCode.OK, "", response);
        }

        // [Authorize (Roles = "TenantManager,TenantAdmin, TenantUser")]
        [AllowAnonymous]
        [HttpGet("{ChildTaskAttachmentId}")]
        public FileResult TaskDocument(long ChildTaskAttachmentId)
        {

            var employeeChildTaskAttachmentObj = _employeeChildTaskAttachmentService.GetEmployeeChildTaskAttachmentById(ChildTaskAttachmentId);

            // full path to file in temp location
            //var dirPath = _hostingEnvironment.WebRootPath + "\\EmployeeChildTaskUpload";
            var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.EmployeeChildTaskUploadDirPath;
            var filePath = dirPath + "\\" + "default.png";

            if (employeeChildTaskAttachmentObj != null && !string.IsNullOrEmpty(employeeChildTaskAttachmentObj.Name))
            {
                filePath = dirPath + "\\" + employeeChildTaskAttachmentObj.Name;
                Byte[] b = System.IO.File.ReadAllBytes(filePath);
                return File(b, "*");
            }
            return null;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete("{ChildTaskId}")]
        public async Task<OperationResult<EmployeeChildTaskDto>> Remove(long ChildTaskId)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            //for comment and document
            var mateChildCommentIdList = _mateChildTaskCommentService.GetByChildTaskId(ChildTaskId).Select(t => t.MateCommentId).ToList();
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
                            childCommentActivityObj.EmployeeChildTaskId = ChildTaskId;
                            childCommentActivityObj.UserId = UserId;
                            childCommentActivityObj.Activity = Enums.EmployeeChidTaskActivityEnum.Childtask_comment_removed.ToString().Replace("_", " ");
                            var AddUpdateChildCommentActivity = await _employeeChildTaskActivityService.CheckInsertOrUpdate(childCommentActivityObj);
                        }
                    }
                }
            }
            //for comment and document

            //var timeRecords = await _employeeChildTaskTimeRecordService.DeleteTimeRecordByEmployeeChildTaskId(ChildTaskId);
            //for delete childtask time record
            var mateChlidTaskTimeRecordList = _mateChildTaskTimeRecordService.GetRecordByChildTaskId(ChildTaskId);
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

            var taskUsers = await _employeeChildTaskUserService.DeleteByChildTaskId(ChildTaskId);

            EmployeeChildTaskActivity employeeChildTaskActivityObj = new EmployeeChildTaskActivity();
            employeeChildTaskActivityObj.EmployeeChildTaskId = ChildTaskId;
            employeeChildTaskActivityObj.UserId = UserId;
            employeeChildTaskActivityObj.Activity = Enums.EmployeeChidTaskActivityEnum.Childtask_Removed.ToString().Replace("_", " ");
            var AddUpdate = await _employeeChildTaskActivityService.CheckInsertOrUpdate(employeeChildTaskActivityObj);
            var childTaskToDelete = await _employeeChildTaskService.Delete(ChildTaskId);

            var childTaskActivities = await _employeeChildTaskActivityService.DeleteByEmployeeChildTaskId(ChildTaskId);

            return new OperationResult<EmployeeChildTaskDto>(true, System.Net.HttpStatusCode.OK, "Child task deleted successfully");

        }

        // Get Child Task Detail by Id
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{ChildTaskId}")]
        public async Task<OperationResult<EmployeeChildTaskVM>> Detail(long ChildTaskId)
        {
            EmployeeChildTaskVM employeeChildTaskVMObj = new EmployeeChildTaskVM();

            var AllUsers = _userService.GetAll();

            var childTaskObj = _employeeChildTaskService.GetChildTaskById(ChildTaskId);
            employeeChildTaskVMObj = _mapper.Map<EmployeeChildTaskVM>(childTaskObj);

            var childTaskTotalDuration = _employeeChildTaskTimeRecordService.GetTotalEmployeeChildTaskTimeRecord(ChildTaskId);
            //employeeChildTaskVMObj.Duration = childTaskTotalDuration;

            // For child task assign users
            var assignChildTaskUsers = _employeeChildTaskUserService.GetAssignUsersByChildTask(ChildTaskId);
            if (assignChildTaskUsers != null && assignChildTaskUsers.Count > 0)
            {
                var assignChildTaskUserVMList = _mapper.Map<List<EmployeeChildTaskUserDto>>(assignChildTaskUsers);
                if (employeeChildTaskVMObj.AssignedUsers == null)
                {
                    employeeChildTaskVMObj.AssignedUsers = new List<EmployeeChildTaskUserDto>();
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
                employeeChildTaskVMObj.AssignedUsers = assignChildTaskUserVMList;
            }

            // For child task documents
            var childTaskDocuments = _employeeChildTaskAttachmentService.GetAllByChildTaskId(employeeChildTaskVMObj.Id);
            if (childTaskDocuments != null && childTaskDocuments.Count > 0)
            {
                if (employeeChildTaskVMObj.Documents == null)
                {
                    employeeChildTaskVMObj.Documents = new List<EmployeeChildTaskAttachment>();
                }
                employeeChildTaskVMObj.Documents = childTaskDocuments;
            }

            // For mate child task comments
            var mateChildTaskComments = _mateChildTaskCommentService.GetByChildTaskId(ChildTaskId);
            if (mateChildTaskComments != null && mateChildTaskComments.Count > 0)
            {
                if (mateChildTaskComments != null && mateChildTaskComments.Count() > 0)
                {
                    foreach (var childtaskItem in mateChildTaskComments)
                    {
                        EmployeeChildTaskMateComment mateChildTaskCommentResponseObj = new EmployeeChildTaskMateComment();
                        if (AllUsers != null && AllUsers.Count() > 0)
                        {
                            var userObj = AllUsers.Where(t => t.Id == childtaskItem.MateComment?.UserId).FirstOrDefault();
                            if (userObj != null)
                            {
                                mateChildTaskCommentResponseObj.UserId = childtaskItem.MateComment?.UserId;
                                string UserName = userObj.FirstName + " " + userObj.LastName;
                                mateChildTaskCommentResponseObj.UserName = UserName;

                            }
                        }
                        mateChildTaskCommentResponseObj.Id = childtaskItem.MateComment.Id;
                        mateChildTaskCommentResponseObj.CreatedOn = childtaskItem.MateComment?.CreatedOn;
                        mateChildTaskCommentResponseObj.Comment = childtaskItem.MateComment?.Comment;
                        mateChildTaskCommentResponseObj.MateReplyCommentId = childtaskItem.MateComment?.MateReplyCommentId;
                        //Attachment
                        if (childtaskItem.MateCommentId != null)
                        {
                            var childTaskAttachments = _mateCommentAttachmentService.GetByMateCommentId(childtaskItem.MateCommentId.Value);
                            if (childTaskAttachments != null && childTaskAttachments.Count > 0)
                            {
                                EmployeeChildTaskMateCommentAttachment mateCommentChildTaskAttachmentObj = new EmployeeChildTaskMateCommentAttachment();
                                var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                                foreach (var taskfile in childTaskAttachments)
                                {
                                    mateCommentChildTaskAttachmentObj.Name = taskfile.Name;
                                    if (taskfile.Name == null)
                                    {
                                        mateCommentChildTaskAttachmentObj.URL = null;
                                    }
                                    else
                                    {
                                        mateCommentChildTaskAttachmentObj.URL = OneClappContext.CurrentURL + "MateComment/Attachment/" + taskfile.Id + "?" + Timestamp;
                                    }
                                    mateChildTaskCommentResponseObj.Attachments.Add(mateCommentChildTaskAttachmentObj);
                                }
                            }
                        }
                        //Attachment
                        employeeChildTaskVMObj.Comments.Add(mateChildTaskCommentResponseObj);
                    }
                }
            }
            // For mate child task comments

            // For task activities
            var childTaskActivities = _employeeChildTaskActivityService.GetAllByEmployeeChildTaskId(employeeChildTaskVMObj.Id);
            if (childTaskActivities != null && childTaskActivities.Count > 0)
            {
                var childTaskActivitiesVMList = _mapper.Map<List<EmployeeChildTaskActivityDto>>(childTaskActivities);
                if (childTaskActivitiesVMList != null && childTaskActivitiesVMList.Count() > 0)
                {
                    foreach (var EmployeeChildTaskActivityObj in childTaskActivitiesVMList)
                    {
                        var userObjAct2 = AllUsers.Where(t => t.Id == EmployeeChildTaskActivityObj.UserId).FirstOrDefault();
                        if (userObjAct2 != null)
                        {
                            EmployeeChildTaskActivityObj.FirstName = userObjAct2.FirstName;
                            EmployeeChildTaskActivityObj.LastName = userObjAct2.LastName;
                        }
                    }
                }
                if (employeeChildTaskVMObj.Activities == null)
                {
                    employeeChildTaskVMObj.Activities = new List<EmployeeChildTaskActivityDto>();
                }
                employeeChildTaskVMObj.Activities = childTaskActivitiesVMList;
            }
            return new OperationResult<EmployeeChildTaskVM>(true, System.Net.HttpStatusCode.OK, "", employeeChildTaskVMObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{EmployeeChildTaskId}")]
        public async Task<OperationResult<List<EmployeeChildTaskHistoryResponse>>> History(long EmployeeChildTaskId)
        {
            List<EmployeeChildTaskActivityDto> employeeChildTaskActivityDtoList = new List<EmployeeChildTaskActivityDto>();
            var AllUsers = _userService.GetAll();
            var activities = _employeeChildTaskActivityService.GetAllByEmployeeChildTaskId(EmployeeChildTaskId);
            employeeChildTaskActivityDtoList = _mapper.Map<List<EmployeeChildTaskActivityDto>>(activities);

            if (employeeChildTaskActivityDtoList != null && employeeChildTaskActivityDtoList.Count() > 0)
            {
                foreach (var item in employeeChildTaskActivityDtoList)
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
            var response = _mapper.Map<List<EmployeeChildTaskHistoryResponse>>(employeeChildTaskActivityDtoList);
            return new OperationResult<List<EmployeeChildTaskHistoryResponse>>(true, System.Net.HttpStatusCode.OK, "", response);
        }

        // Document delete method - Shakti
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete("{EmployeeChildTaskAttachmentId}")]
        public async Task<OperationResult<EmployeeChildTaskAttachmentDto>> RemoveDocument(long EmployeeChildTaskAttachmentId)
        {
            var employeeChildTaskAttachment = await _employeeChildTaskAttachmentService.DeleteEmployeeChildTaskAttachmentById(EmployeeChildTaskAttachmentId);

            // Remove task documents from folder
            if (employeeChildTaskAttachment != null)
            {
                //var dirPath = _hostingEnvironment.WebRootPath + "\\EmployeeChildTaskUpload";
                var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.EmployeeChildTaskUploadDirPath;
                var filePath = dirPath + "\\" + employeeChildTaskAttachment.Name;

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(Path.Combine(filePath));
                }
                EmployeeChildTaskActivity employeeChildTaskActivityObj = new EmployeeChildTaskActivity();
                employeeChildTaskActivityObj.EmployeeChildTaskId = employeeChildTaskAttachment.EmployeeChildTaskId;
                employeeChildTaskActivityObj.UserId = employeeChildTaskAttachment.UserId;
                employeeChildTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Document_removed.ToString().Replace("_", " ");

                var AddUpdate = await _employeeChildTaskActivityService.CheckInsertOrUpdate(employeeChildTaskActivityObj);
                await _hubContext.Clients.All.OnUploadTaskDocumentEventEmit(employeeChildTaskAttachment.EmployeeChildTaskId);
                return new OperationResult<EmployeeChildTaskAttachmentDto>(true, System.Net.HttpStatusCode.OK, "Document removed successfully");
            }
            else
            {
                return new OperationResult<EmployeeChildTaskAttachmentDto>(false, System.Net.HttpStatusCode.OK, "Document not found");
            }
        }

        // Assigned Child task user delete method - Shakti
        [Authorize(Roles = "TenantManager,TenantAdmin, TenantUser")]
        [HttpDelete("{AssignUserId}")]
        public async Task<OperationResult<EmployeeChildTaskUserDto>> AssignUser(long AssignUserId)
        {
            var assignUserRemove = await _employeeChildTaskUserService.DeleteAssignedChildTaskUser(AssignUserId);

            if (assignUserRemove != null)
            {
                EmployeeChildTaskActivity employeeChildTaskActivityObj = new EmployeeChildTaskActivity();
                employeeChildTaskActivityObj.EmployeeChildTaskId = assignUserRemove.EmployeeChildTaskId;
                employeeChildTaskActivityObj.UserId = assignUserRemove.UserId;
                employeeChildTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Assign_user_removed.ToString().Replace("_", " ");
                var AddUpdate = await _employeeChildTaskActivityService.CheckInsertOrUpdate(employeeChildTaskActivityObj);
                if (assignUserRemove.UserId != null)
                {
                    var removeChildTaskUserDetails = _userService.GetUserById(assignUserRemove.UserId.Value);
                    if (assignUserRemove.EmployeeChildTaskId != null)
                    {
                        var childTaskDetail = _employeeChildTaskService.GetChildTaskById(assignUserRemove.EmployeeChildTaskId.Value);
                        if (childTaskDetail != null && removeChildTaskUserDetails != null)
                        {

                            await sendEmail.SendEmailRemoveTaskUserAssignNotification(removeChildTaskUserDetails.Email, removeChildTaskUserDetails.FirstName + ' ' + removeChildTaskUserDetails.LastName, childTaskDetail.Description, TenantId);
                            return new OperationResult<EmployeeChildTaskUserDto>(true, System.Net.HttpStatusCode.OK, "User removed successfully from child task.");

                        }
                        else
                        {
                            return new OperationResult<EmployeeChildTaskUserDto>(false, System.Net.HttpStatusCode.OK, "Something went wrong");
                        }
                    }
                }
            }

            return new OperationResult<EmployeeChildTaskUserDto>(false, System.Net.HttpStatusCode.OK, "Remove user not found");
        }


        [AllowAnonymous]
        [HttpGet("{ChildTaskAttachmentId}")]
        public async Task<OperationResult<string>> Document(long ChildTaskAttachmentId)
        {
            var employeeChildTaskAttachmentObj = _employeeChildTaskAttachmentService.GetEmployeeChildTaskAttachmentById(ChildTaskAttachmentId);

            //var dirPath = _hostingEnvironment.WebRootPath + "\\EmployeeChildTaskUpload";
            var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.EmployeeChildTaskUploadDirPath;
            var filePath = dirPath + "\\" + employeeChildTaskAttachmentObj.Name;
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

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<List<EmployeeChildTaskListResponse>>> List([FromBody] EmployeeChildTaskListRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            //List<EmployeeTask> employeeTaskList = new List<EmployeeTask>();

            var employeeChildTaskList = _employeeChildTaskService.GetBySubTaskId(model);
            var AllStatus = _statusService.GetByTenant(TenantId);
            var AllUsers = _userService.GetAll();

            var employeeChildTaskListResponseList = _mapper.Map<List<EmployeeChildTaskListResponse>>(employeeChildTaskList);
            //EmployeeTaskListResponse employeeTaskListResponseObj = new EmployeeTaskListResponse();

            if (employeeChildTaskListResponseList != null && employeeChildTaskListResponseList.Count() > 0)
            {
                foreach (var item in employeeChildTaskListResponseList)
                {
                    var statusObj = AllStatus.Where(t => t.Id == item.StatusId).FirstOrDefault();
                    if (statusObj != null)
                    {
                        item.Status = statusObj.Name;
                    }
                    item.Name = item.Description;
                    item.SubTaskId = model.SubTaskId;
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
                    var mateChildTaskTimeRecordList = _mateChildTaskTimeRecordService.GetByChildTaskIdAndUserId(item.Id, UserId);
                    var mateChildTaskTimeRecordAscList = mateChildTaskTimeRecordList.OrderBy(t => t.MateTimeRecord.CreatedOn).ToList();
                    var mateChildTaskTimeRecordLast = mateChildTaskTimeRecordAscList.LastOrDefault();
                    long TaskTotalDuration = 0;
                    if (mateChildTaskTimeRecordList != null && mateChildTaskTimeRecordList.Count > 0)
                    {
                        foreach (var childTaskTimeRecord in mateChildTaskTimeRecordList)
                        {
                            if (childTaskTimeRecord.MateTimeRecord != null)
                            {
                                if (childTaskTimeRecord.MateTimeRecord.Duration != null)
                                {
                                    TaskTotalDuration = TaskTotalDuration + childTaskTimeRecord.MateTimeRecord.Duration.Value;

                                    TimeSpan timeSpan = TimeSpan.FromMinutes(TaskTotalDuration);

                                    item.TotalDuration = timeSpan.ToString(@"hh\:mm"); ;
                                    if (mateChildTaskTimeRecordLast != null)
                                    {
                                        item.Enddate = mateChildTaskTimeRecordLast.MateTimeRecord.CreatedOn;
                                    }

                                }
                            }
                        }
                        item.TimeRecordCount = mateChildTaskTimeRecordList.Count;
                    }
                }
            }
            return new OperationResult<List<EmployeeChildTaskListResponse>>(true, System.Net.HttpStatusCode.OK, "", employeeChildTaskListResponseList);
        }

    }
}