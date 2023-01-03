using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using matcrm.service.Services;
using AutoMapper;
using matcrm.service.Common;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using matcrm.data.Models.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using matcrm.data.Context;
using System.IO;
using matcrm.service.Utility;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.SignalR;
using matcrm.api.SignalR;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class MateCommentController : Controller
    {
        private readonly IMateCommentService _mateCommentService;
        private readonly IMateCommentAttachmentService _mateCommentAttachmentService;
        private readonly IMateTaskCommentService _mateTaskCommentService;
        private readonly IMateSubTaskCommentService _mateSubTaskCommentService;
        private readonly IMateChildTaskCommentService _mateChildTaskCommentService;
        private readonly IEmployeeTaskActivityService _employeeTaskActivityService;
        private readonly IEmployeeSubTaskActivityService _employeeSubTaskActivityService;
        private readonly IEmployeeChildTaskActivityService _employeeChildTaskActivityService;
        private readonly IMateTicketCommentService _mateTicketCommentService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IMateTicketActivityService _mateTicketActivityService;
        private readonly IUserService _userService;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private IMapper _mapper;
        private Common Common;
        private int UserId = 0;
        private int TenantId = 0;
        public MateCommentController(IMateCommentService mateCommentService,
        IMateCommentAttachmentService mateCommentAttachmentService,
        IMateTaskCommentService mateTaskCommentService,
        IMateSubTaskCommentService mateSubTaskCommentService,
        IMateChildTaskCommentService mateChildTaskCommentService,
        IEmployeeTaskActivityService employeeTaskActivityService,
        IEmployeeSubTaskActivityService employeeSubTaskActivityService,
        IEmployeeChildTaskActivityService employeeChildTaskActivityService,
        IHostingEnvironment hostingEnvironment,
        IUserService userService,
        IHubContext<BroadcastHub, IHubClient> hubContext,
        IMateTicketCommentService mateTicketCommentService,
        IMateTicketActivityService mateTicketActivityService,
        IMapper mapper)
        {
            _mateCommentService = mateCommentService;
            _mateCommentAttachmentService = mateCommentAttachmentService;
            _mateTaskCommentService = mateTaskCommentService;
            _mateSubTaskCommentService = mateSubTaskCommentService;
            _mateChildTaskCommentService = mateChildTaskCommentService;
            _employeeTaskActivityService = employeeTaskActivityService;
            _employeeSubTaskActivityService = employeeSubTaskActivityService;
            _employeeChildTaskActivityService = employeeChildTaskActivityService;
            _mateTicketCommentService = mateTicketCommentService;
            _hostingEnvironment = hostingEnvironment;
            _userService = userService;
            _hubContext = hubContext;
            _mapper = mapper;
            Common = new Common();
            _mateTicketActivityService = mateTicketActivityService;
        }

        [HttpPost]
        public async Task<OperationResult<MateCommentAddUpdateResponse>> Add([FromForm] MateCommentAddUpdateRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            if (requestmodel.TaskId != null || requestmodel.SubTaskId != null || requestmodel.ChildTaskId != null)
            {
                var model = _mapper.Map<MateComment>(requestmodel);
                MateCommentAddUpdateResponse mateCommentAddUpdateResponseObj = new MateCommentAddUpdateResponse();
                model.UserId = UserId;
                long Id = 0;
                string Type = "";
                var mateCommentObj = await _mateCommentService.CheckInsertOrUpdate(model);
                if (mateCommentObj != null)
                {
                    //Ticket comment
                    if (requestmodel.MateTicketId != null && requestmodel.MateTicketId > 0)
                    {
                        Id = requestmodel.MateTicketId.Value;
                        Type = "MateTicket";
                        var mateTicketCommentObj = new MateTicketComment();
                        mateTicketCommentObj.MateCommentId = mateCommentObj.Id;
                        mateTicketCommentObj.MateTicketId = requestmodel.MateTicketId;
                        var AddUpdateTicketCommentObj = await _mateTicketCommentService.CheckInsertOrUpdate(mateTicketCommentObj);
                        if (AddUpdateTicketCommentObj != null)
                        {
                            MateTicketActivity ticketActivityObj = new MateTicketActivity();
                            ticketActivityObj.MateTicketId = requestmodel.MateTicketId;
                            ticketActivityObj.CreatedBy = UserId;
                            ticketActivityObj.Activity = Enums.MateTicketActivityEnum.Ticket_comment_created.ToString().Replace("_", " ");
                            var AddUpdateTicketActivityObj = await _mateTicketActivityService.CheckInsertOrUpdate(ticketActivityObj);
                            await _hubContext.Clients.All.OnMateTicketModuleEvent(requestmodel.MateTicketId, TenantId);
                        }
                    }

                    //Task comment
                    if (requestmodel.TaskId != null && requestmodel.TaskId > 0)
                    {
                        Id = requestmodel.TaskId.Value;
                        Type = "Task";
                        var mateTaskCommentObj = new MateTaskComment();
                        mateTaskCommentObj.MateCommentId = mateCommentObj.Id;
                        mateTaskCommentObj.TaskId = requestmodel.TaskId;
                        var AddUpdateMateTaskCommentObj = await _mateTaskCommentService.CheckInsertOrUpdate(mateTaskCommentObj);
                        if (AddUpdateMateTaskCommentObj != null)
                        {
                            EmployeeTaskActivity taskActivityObj = new EmployeeTaskActivity();
                            taskActivityObj.EmployeeTaskId = requestmodel.TaskId;
                            taskActivityObj.UserId = UserId;
                            taskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_comment_created.ToString().Replace("_", " ");
                            var AddUpdateTaskActivityObj = await _employeeTaskActivityService.CheckInsertOrUpdate(taskActivityObj);
                            await _hubContext.Clients.All.OnEmployeeTaskModuleEvent(requestmodel.TaskId, TenantId);
                        }
                    }

                    //Sub task comment                
                    if (requestmodel.SubTaskId != null && requestmodel.SubTaskId > 0)
                    {
                        Id = requestmodel.SubTaskId.Value;
                        Type = "SubTask";
                        var mateSubTaskCommentObj = new MateSubTaskComment();
                        mateSubTaskCommentObj.MateCommentId = mateCommentObj.Id;
                        mateSubTaskCommentObj.SubTaskId = requestmodel.SubTaskId;
                        var AddUpdateMateSubTaskCommentObj = await _mateSubTaskCommentService.CheckInsertOrUpdate(mateSubTaskCommentObj);
                        if (AddUpdateMateSubTaskCommentObj != null)
                        {
                            EmployeeSubTaskActivity subTaskActivityObj = new EmployeeSubTaskActivity();
                            subTaskActivityObj.EmployeeSubTaskId = requestmodel.SubTaskId;
                            subTaskActivityObj.UserId = UserId;
                            subTaskActivityObj.Activity = Enums.EmployeeSubTaskActivityEnum.Subtask_comment_created.ToString().Replace("_", " ");
                            var AddUpdateSubTaskActivityObj = await _employeeSubTaskActivityService.CheckInsertOrUpdate(subTaskActivityObj);
                        }
                    }

                    //Child task comment                
                    if (requestmodel.ChildTaskId != null && requestmodel.ChildTaskId > 0)
                    {
                        Id = requestmodel.ChildTaskId.Value;
                        Type = "ChildTask";
                        var mateChildTaskCommentObj = new MateChildTaskComment();
                        mateChildTaskCommentObj.MateCommentId = mateCommentObj.Id;
                        mateChildTaskCommentObj.ChildTaskId = requestmodel.ChildTaskId;
                        var AddUpdateMateChildTaskCommentObj = await _mateChildTaskCommentService.CheckInsertOrUpdate(mateChildTaskCommentObj);
                        if (AddUpdateMateChildTaskCommentObj != null)
                        {
                            EmployeeChildTaskActivity childTaskActivityObj = new EmployeeChildTaskActivity();
                            childTaskActivityObj.EmployeeChildTaskId = requestmodel.ChildTaskId;
                            childTaskActivityObj.UserId = UserId;
                            childTaskActivityObj.Activity = Enums.EmployeeChidTaskActivityEnum.Childtask_comment_created.ToString().Replace("_", " ");
                            var AddUpdateChildTaskActivityObj = await _employeeChildTaskActivityService.CheckInsertOrUpdate(childTaskActivityObj);
                        }
                    }

                    //mate comment attchament
                    if (requestmodel.FileList != null && requestmodel.FileList.Count() > 0)
                    {
                        foreach (IFormFile file in requestmodel.FileList)
                        {
                            // full path to file in temp location
                            //var dirPath = _hostingEnvironment.WebRootPath + "\\MateCommentUpload";
                            var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.MateCommentUploadDirPath;

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
                                    return new OperationResult<MateCommentAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                                }
                            }

                            using (var oStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                            {
                                await file.CopyToAsync(oStream);
                            }

                            MateCommentAttachment mateCommentAttachmentObj = new MateCommentAttachment();
                            mateCommentAttachmentObj.Name = fileName;
                            mateCommentAttachmentObj.MateCommentId = mateCommentObj.Id;
                            var AddUpdateCommentAttachment = await _mateCommentAttachmentService.CheckInsertOrUpdate(mateCommentAttachmentObj);
                        }
                    }
                    mateCommentAddUpdateResponseObj = _mapper.Map<MateCommentAddUpdateResponse>(mateCommentObj);
                    mateCommentAddUpdateResponseObj.TaskId = requestmodel.TaskId;
                    mateCommentAddUpdateResponseObj.SubTaskId = requestmodel.SubTaskId;
                    mateCommentAddUpdateResponseObj.ChildTaskId = requestmodel.ChildTaskId;
                    await _hubContext.Clients.All.OnMateCommentModuleEvent(Id, Type);

                }
                return new OperationResult<MateCommentAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Added successfully.", mateCommentAddUpdateResponseObj);
            }
            else
            {
                return new OperationResult<MateCommentAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Please provide task id or subtask id or childtask id");
            }
        }

        [HttpPut]
        public async Task<OperationResult<MateCommentAddUpdateResponse>> Update([FromForm] MateCommentAddUpdateRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var model = _mapper.Map<MateComment>(requestmodel);
            MateCommentAddUpdateResponse mateCommentAddUpdateResponseObj = new MateCommentAddUpdateResponse();
            model.UserId = UserId;

            var mateCommentObj = await _mateCommentService.CheckInsertOrUpdate(model);
            if (mateCommentObj != null)
            {
                long Id = 0;
                string Type = "";
                //Ticket
                var mateTicketCommentObj = _mateTicketCommentService.GetByMateCommentId(mateCommentObj.Id);
                if (mateTicketCommentObj != null)
                {
                    if (mateTicketCommentObj.MateTicketId != null)
                    {
                        Id = mateTicketCommentObj.MateTicketId.Value;
                        Type = "MateTicket";
                    }

                }

                var mateTaskCommentObj = _mateTaskCommentService.GetByMateCommentId(mateCommentObj.Id);
                if (mateTaskCommentObj != null)
                {
                    if (mateTaskCommentObj.TaskId != null)
                    {
                        Id = mateTaskCommentObj.TaskId.Value;
                        Type = "Task";
                    }

                }

                var mateSubTaskCommentObj = _mateSubTaskCommentService.GetByMateCommentId(mateCommentObj.Id);
                if (mateSubTaskCommentObj != null)
                {
                    if (mateSubTaskCommentObj.SubTaskId != null)
                    {
                        Id = mateSubTaskCommentObj.SubTaskId.Value;
                        Type = "SubTask";
                    }
                }
                var mateChildTaskCommentObj = _mateChildTaskCommentService.GetByMateCommentId(mateCommentObj.Id);
                if (mateChildTaskCommentObj != null)
                {
                    if (mateChildTaskCommentObj.ChildTaskId != null)
                    {
                        Id = mateChildTaskCommentObj.ChildTaskId.Value;
                        Type = "ChildTask";
                    }
                }
                await _hubContext.Clients.All.OnMateCommentModuleEvent(Id, Type);
            }
            mateCommentAddUpdateResponseObj = _mapper.Map<MateCommentAddUpdateResponse>(mateCommentObj);
            return new OperationResult<MateCommentAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Updated successfully.", mateCommentAddUpdateResponseObj);
        }

        [HttpDelete("{Id}")]
        public async Task<OperationResult> Remove(long Id)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            if (Id != null && Id > 0)
            {
                // var mateCommentAttachmentList = _mateCommentAttachmentService.GetByMateCommentId(Id);
                // if (mateCommentAttachmentList != null && mateCommentAttachmentList.Count() > 0)
                // {
                //     foreach (var mailCommentAttachmentObj in mateCommentAttachmentList)
                //     {
                //         if (mailCommentAttachmentObj != null)
                //         {
                //             var mateCommentAttachments = await _mateCommentAttachmentService.DeleteById(mailCommentAttachmentObj.Id);

                //             var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.MateCommentUploadDirPath;
                //             var filePath = dirPath + "\\" + mailCommentAttachmentObj.Name;

                //             if (System.IO.File.Exists(filePath))
                //             {
                //                 System.IO.File.Delete(Path.Combine(filePath));
                //             }
                //         }
                //     }
                // }

                long actionId = 0;
                string Type = "";

                var Attachments = await _mateCommentAttachmentService.DeleteByMateCommentId(Id);

                foreach (var Doc in Attachments)
                {
                    var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.EmployeeTaskUploadDirPath;
                    var filePath = dirPath + "\\" + Doc.Name;

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(Path.Combine(filePath));
                    }
                }

                var mateCommentObj = await _mateCommentService.DeleteMateComment(Id);
                if (mateCommentObj != null)
                {
                    //start Ticket comment activity
                    var mateTicketCommentObj = _mateTicketCommentService.GetByMateCommentId(Id);
                    if (mateTicketCommentObj != null)
                    {
                        if (mateTicketCommentObj.MateTicketId != null)
                        {
                            actionId = mateTicketCommentObj.MateTicketId.Value;
                            Type = "MateTicket";
                        }
                        MateTicketActivity ticketActivityObj = new MateTicketActivity();
                        ticketActivityObj.MateTicketId = mateTicketCommentObj.MateTicketId;
                        ticketActivityObj.CreatedBy = UserId;
                        ticketActivityObj.Activity = Enums.MateTicketActivityEnum.Ticket_comment_removed.ToString().Replace("_", " ");
                        var AddUpdateTicketActivityObj = await _mateTicketActivityService.CheckInsertOrUpdate(ticketActivityObj);
                        await _hubContext.Clients.All.OnMateTicketModuleEvent(mateTicketCommentObj.MateTicketId, TenantId);
                    }
                    //end Ticket comment activity

                    //start Task comment activity
                    var mateTaskCommentObj = _mateTaskCommentService.GetByMateCommentId(Id);
                    if (mateTaskCommentObj != null)
                    {
                        if (mateTaskCommentObj.TaskId != null)
                        {
                            actionId = mateTaskCommentObj.TaskId.Value;
                            Type = "Task";
                        }
                        EmployeeTaskActivity taskActivityObj = new EmployeeTaskActivity();
                        taskActivityObj.EmployeeTaskId = mateTaskCommentObj.TaskId;
                        taskActivityObj.UserId = UserId;
                        taskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_comment_removed.ToString().Replace("_", " ");
                        var AddUpdateTaskActivityObj = await _employeeTaskActivityService.CheckInsertOrUpdate(taskActivityObj);
                        await _hubContext.Clients.All.OnEmployeeTaskModuleEvent(mateTaskCommentObj.TaskId, TenantId);
                    }
                    //end Task comment activity

                    //start Sub Task comment activity
                    var mateSubTaskCommentObj = _mateSubTaskCommentService.GetByMateCommentId(Id);
                    if (mateSubTaskCommentObj != null)
                    {
                        if (mateSubTaskCommentObj.SubTaskId != null)
                        {
                            actionId = mateSubTaskCommentObj.SubTaskId.Value;
                            Type = "SubTask";
                        }
                        EmployeeSubTaskActivity subTaskActivityObj = new EmployeeSubTaskActivity();
                        subTaskActivityObj.EmployeeSubTaskId = mateSubTaskCommentObj.SubTaskId;
                        subTaskActivityObj.UserId = UserId;
                        subTaskActivityObj.Activity = Enums.EmployeeSubTaskActivityEnum.Subtask_comment_removed.ToString().Replace("_", " ");
                        var AddUpdateSubTaskActivityObj = await _employeeSubTaskActivityService.CheckInsertOrUpdate(subTaskActivityObj);
                    }
                    //end Sub Task comment activity

                    //start Child Task comment activity
                    var mateChildTaskCommentObj = _mateChildTaskCommentService.GetByMateCommentId(Id);
                    if (mateChildTaskCommentObj != null)
                    {
                        if (mateChildTaskCommentObj.ChildTaskId != null)
                        {
                            actionId = mateChildTaskCommentObj.ChildTaskId.Value;
                            Type = "ChildTask";
                        }

                        EmployeeChildTaskActivity childTaskActivityObj = new EmployeeChildTaskActivity();
                        childTaskActivityObj.EmployeeChildTaskId = mateChildTaskCommentObj.ChildTaskId;
                        childTaskActivityObj.UserId = UserId;
                        childTaskActivityObj.Activity = Enums.EmployeeChidTaskActivityEnum.Childtask_comment_removed.ToString().Replace("_", " ");
                        var AddUpdateChildTaskActivityObj = await _employeeChildTaskActivityService.CheckInsertOrUpdate(childTaskActivityObj);
                    }
                    //end Child Task comment activity
                }
                await _hubContext.Clients.All.OnMateCommentModuleEvent(actionId, Type);
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "Deleted successfully", Id);
            }
            else
            {
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "Please provide Id", Id);
            }
        }

        [HttpDelete("{Id}")]
        public async Task<OperationResult> RemoveAttachment(long Id)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            if (Id != null && Id > 0)
            {
                var mateCommentAttachmentObj = await _mateCommentAttachmentService.DeleteById(Id);
                if (mateCommentAttachmentObj != null)
                {
                    //var dirPath = _hostingEnvironment.WebRootPath + "\\MailCommentUpload";
                    var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.MailCommentUploadDirPath;
                    var filePath = dirPath + "\\" + mateCommentAttachmentObj.Name;

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(Path.Combine(filePath));
                    }

                    await _hubContext.Clients.All.OnMateCommentModuleEvent(Id, "Attachment");

                    return new OperationResult(true, System.Net.HttpStatusCode.OK, "Mate comment file deleted successfully", Id);
                }
                else
                {
                    return new OperationResult(false, System.Net.HttpStatusCode.OK, "Document not found", Id);
                }
            }
            else
            {
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "Please provide Id", Id);
            }
        }
        //download attachment
        [AllowAnonymous]
        [SwaggerOperation(Description = "download Attachment from attachment id")]
        [HttpGet("{Id}")]
        public async Task<FileResult> Attachment(long Id)
        {
            var mateCommentAttachmentObj = _mateCommentAttachmentService.GetById(Id);

            var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.MateCommentUploadDirPath;
            var filePath = dirPath + "\\" + "default.png";
            if (mateCommentAttachmentObj != null && !string.IsNullOrEmpty(mateCommentAttachmentObj.Name))
            {
                filePath = dirPath + "\\" + mateCommentAttachmentObj.Name;
                if (System.IO.File.Exists(filePath))
                {
                    var bytes = await System.IO.File.ReadAllBytesAsync(filePath);

                    return File(bytes, Common.GetMimeTypes(mateCommentAttachmentObj.Name), mateCommentAttachmentObj.Name);
                }
            }
            return null;
        }

        [SwaggerOperation(Description = "If getting task record then use taskid as param, subtask record then use as subtaskid, childtask then use as childtaskid")]
        [HttpPost]
        public async Task<OperationResult<List<MateCommentGetAllResponse>>> List([FromBody] MateCommentGetAllRequest model)
        {
            List<MateCommentGetAllResponse> mateCommentGetAllResponseList = new List<MateCommentGetAllResponse>();
            if (model.TaskId != null || model.SubTaskId != null || model.ChildTaskId != null || model.MateTicketId != null)
            {
                var users = _userService.GetAll();
                var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

                //===start for task comment
                if (model.TaskId != null)
                {
                    var mateTaskCommentList = _mateTaskCommentService.GetByTaskId(model.TaskId.Value);
                    if (mateTaskCommentList != null && mateTaskCommentList.Count() > 0)
                    {
                        foreach (var taskItem in mateTaskCommentList)
                        {
                            MateCommentGetAllResponse mateTaskCommentGetAllResponseObj = new MateCommentGetAllResponse();
                            if (users != null && users.Count() > 0)
                            {
                                var userObj = users.Where(t => t.Id == taskItem.MateComment?.UserId).FirstOrDefault();
                                if (userObj != null)
                                {
                                    mateTaskCommentGetAllResponseObj.UserId = taskItem.MateComment?.UserId;
                                    string UserName = userObj.FirstName + " " + userObj.LastName;
                                    mateTaskCommentGetAllResponseObj.UserName = UserName;

                                }
                            }
                            mateTaskCommentGetAllResponseObj.Id = taskItem.MateComment.Id;
                            mateTaskCommentGetAllResponseObj.CreatedOn = taskItem.MateComment?.CreatedOn;
                            mateTaskCommentGetAllResponseObj.Comment = taskItem.MateComment?.Comment;
                            mateTaskCommentGetAllResponseObj.MateReplyCommentId = taskItem.MateComment?.MateReplyCommentId;
                            //Attachment
                            if (taskItem.MateCommentId != null)
                            {
                                var taskAttachments = _mateCommentAttachmentService.GetByMateCommentId(taskItem.MateCommentId.Value);
                                if (taskAttachments != null && taskAttachments.Count > 0)
                                {
                                    MateCommentAttachmentGetAllResponse mateCommentTaskAttachmentObj = new MateCommentAttachmentGetAllResponse();
                                    foreach (var taskfile in taskAttachments)
                                    {
                                        mateCommentTaskAttachmentObj.Id = taskfile.Id;
                                        mateCommentTaskAttachmentObj.Name = taskfile.Name;
                                        if (taskfile.Name == null)
                                        {
                                            mateCommentTaskAttachmentObj.URL = null;
                                        }
                                        else
                                        {
                                            mateCommentTaskAttachmentObj.URL = OneClappContext.CurrentURL + "MateComment/Attachment/" + taskfile.Id + "?" + Timestamp;
                                        }
                                        mateTaskCommentGetAllResponseObj.Attachments.Add(mateCommentTaskAttachmentObj);
                                    }
                                }
                            }
                            //Attachment
                            mateCommentGetAllResponseList.Add(mateTaskCommentGetAllResponseObj);
                        }
                    }
                }
                //===end for task comment

                //===start for sub task comment
                if (model.SubTaskId != null)
                {
                    var mateSubTaskCommentList = _mateSubTaskCommentService.GetBySubTaskId(model.SubTaskId.Value);
                    if (mateSubTaskCommentList != null && mateSubTaskCommentList.Count() > 0)
                    {
                        foreach (var subTaskItem in mateSubTaskCommentList)
                        {
                            MateCommentGetAllResponse mateSubTaskCommentGetAllResponseObj = new MateCommentGetAllResponse();
                            if (users != null && users.Count() > 0)
                            {
                                var userObj = users.Where(t => t.Id == subTaskItem.MateComment?.UserId).FirstOrDefault();
                                if (userObj != null)
                                {
                                    mateSubTaskCommentGetAllResponseObj.UserId = subTaskItem.MateComment?.UserId;
                                    string UserName = userObj.FirstName + " " + userObj.LastName;
                                    mateSubTaskCommentGetAllResponseObj.UserName = UserName;

                                }
                            }
                            mateSubTaskCommentGetAllResponseObj.Id = subTaskItem.MateComment.Id;
                            mateSubTaskCommentGetAllResponseObj.CreatedOn = subTaskItem.MateComment?.CreatedOn;
                            mateSubTaskCommentGetAllResponseObj.Comment = subTaskItem.MateComment?.Comment;
                            mateSubTaskCommentGetAllResponseObj.MateReplyCommentId = subTaskItem.MateComment?.MateReplyCommentId;
                            //Attachment
                            if (subTaskItem.MateCommentId != null)
                            {
                                var subTaskAttachments = _mateCommentAttachmentService.GetByMateCommentId(subTaskItem.MateCommentId.Value);
                                if (subTaskAttachments != null && subTaskAttachments.Count > 0)
                                {
                                    MateCommentAttachmentGetAllResponse mateCommentSubTaskAttachmenObj = new MateCommentAttachmentGetAllResponse();
                                    foreach (var subTaskfile in subTaskAttachments)
                                    {
                                        mateCommentSubTaskAttachmenObj.Id = subTaskfile.Id;
                                        mateCommentSubTaskAttachmenObj.Name = subTaskfile.Name;
                                        if (subTaskfile.Name == null)
                                        {
                                            mateCommentSubTaskAttachmenObj.URL = null;
                                        }
                                        else
                                        {
                                            mateCommentSubTaskAttachmenObj.URL = OneClappContext.CurrentURL + "MateComment/Attachment/" + subTaskfile.Id + "?" + Timestamp;
                                        }
                                        mateSubTaskCommentGetAllResponseObj.Attachments.Add(mateCommentSubTaskAttachmenObj);
                                    }
                                }
                            }
                            //Attachment
                            mateCommentGetAllResponseList.Add(mateSubTaskCommentGetAllResponseObj);
                        }
                    }
                }
                //===end for sub task comment

                //===start for child task comment
                if (model.ChildTaskId != null)
                {
                    var mateChildTaskComments = _mateChildTaskCommentService.GetByChildTaskId(model.ChildTaskId.Value);
                    if (mateChildTaskComments != null && mateChildTaskComments.Count() > 0)
                    {
                        foreach (var childTaskItem in mateChildTaskComments)
                        {
                            MateCommentGetAllResponse mateChildTaskCommentGetAllResponseObj = new MateCommentGetAllResponse();
                            if (users != null && users.Count() > 0)
                            {
                                var userObj = users.Where(t => t.Id == childTaskItem.MateComment?.UserId).FirstOrDefault();
                                if (userObj != null)
                                {
                                    mateChildTaskCommentGetAllResponseObj.UserId = childTaskItem.MateComment?.UserId;
                                    string UserName = userObj.FirstName + " " + userObj.LastName;
                                    mateChildTaskCommentGetAllResponseObj.UserName = UserName;

                                }
                            }
                            mateChildTaskCommentGetAllResponseObj.Id = childTaskItem.MateComment.Id;
                            mateChildTaskCommentGetAllResponseObj.CreatedOn = childTaskItem.MateComment?.CreatedOn;
                            mateChildTaskCommentGetAllResponseObj.Comment = childTaskItem.MateComment?.Comment;
                            mateChildTaskCommentGetAllResponseObj.MateReplyCommentId = childTaskItem.MateComment?.MateReplyCommentId;
                            //Attachment
                            if (childTaskItem.MateCommentId != null)
                            {
                                var childTaskAttachments = _mateCommentAttachmentService.GetByMateCommentId(childTaskItem.MateCommentId.Value);
                                if (childTaskAttachments != null && childTaskAttachments.Count > 0)
                                {
                                    MateCommentAttachmentGetAllResponse mateCommentChildTaskAttachmenObj = new MateCommentAttachmentGetAllResponse();
                                    foreach (var childTaskfile in childTaskAttachments)
                                    {
                                        mateCommentChildTaskAttachmenObj.Id = childTaskfile.Id;
                                        mateCommentChildTaskAttachmenObj.Name = childTaskfile.Name;
                                        if (childTaskfile.Name == null)
                                        {
                                            mateCommentChildTaskAttachmenObj.URL = null;
                                        }
                                        else
                                        {
                                            mateCommentChildTaskAttachmenObj.URL = OneClappContext.CurrentURL + "MateComment/Attachment/" + childTaskfile.Id + "?" + Timestamp;
                                        }
                                        mateChildTaskCommentGetAllResponseObj.Attachments.Add(mateCommentChildTaskAttachmenObj);
                                    }
                                }
                            }
                            //Attachment
                            mateCommentGetAllResponseList.Add(mateChildTaskCommentGetAllResponseObj);
                        }
                    }
                }
                //===end for child task comment

                //===start for ticket comment
                if (model.MateTicketId != null)
                {
                    var mateTicketCommentList = _mateTicketCommentService.GetByTicketId(model.MateTicketId.Value);
                    if (mateTicketCommentList != null && mateTicketCommentList.Count() > 0)
                    {
                        foreach (var ticketItem in mateTicketCommentList)
                        {
                            MateCommentGetAllResponse ticketCommentObj = new MateCommentGetAllResponse();
                            if (users != null && users.Count() > 0)
                            {
                                var userObj = users.Where(t => t.Id == ticketItem.MateComment?.UserId).FirstOrDefault();
                                if (userObj != null)
                                {
                                    ticketCommentObj.UserId = ticketItem.MateComment?.UserId;
                                    string UserName = userObj.FirstName + " " + userObj.LastName;
                                    ticketCommentObj.UserName = UserName;

                                }
                            }
                            ticketCommentObj.Id = ticketItem.MateComment.Id;
                            ticketCommentObj.CreatedOn = ticketItem.MateComment?.CreatedOn;
                            ticketCommentObj.Comment = ticketItem.MateComment?.Comment;
                            ticketCommentObj.MateReplyCommentId = ticketItem.MateComment?.MateReplyCommentId;
                            //Attachment
                            if (ticketItem.MateCommentId != null)
                            {
                                var ticketAttachments = _mateCommentAttachmentService.GetByMateCommentId(ticketItem.MateCommentId.Value);
                                if (ticketAttachments != null && ticketAttachments.Count > 0)
                                {
                                    MateCommentAttachmentGetAllResponse ticketAttachmentObj = new MateCommentAttachmentGetAllResponse();
                                    foreach (var taskfile in ticketAttachments)
                                    {
                                        ticketAttachmentObj.Id = taskfile.Id;
                                        ticketAttachmentObj.Name = taskfile.Name;
                                        if (taskfile.Name == null)
                                        {
                                            ticketAttachmentObj.URL = null;
                                        }
                                        else
                                        {
                                            ticketAttachmentObj.URL = OneClappContext.CurrentURL + "MateComment/Attachment/" + taskfile.Id + "?" + Timestamp;
                                        }
                                        ticketCommentObj.Attachments.Add(ticketAttachmentObj);
                                    }
                                }
                            }
                            //Attachment
                            mateCommentGetAllResponseList.Add(ticketCommentObj);
                        }
                    }
                }
                //===end for ticket comment

                // if (item.Id != null)
                // {
                //     var attachments = _mailCommentAttachmentService.GetAllByMailCommentId(item.Id.Value);
                //     mailCommentObj.Attachments = _mapper.Map<List<MailCommentAttachmentDto>>(attachments);
                // }
            }
            else
            {
                return new OperationResult<List<MateCommentGetAllResponse>>(true, System.Net.HttpStatusCode.OK, "Please provide taskId or subtaskId or childtaskId or mateticketId");
            }
            return new OperationResult<List<MateCommentGetAllResponse>>(true, System.Net.HttpStatusCode.OK, "", mateCommentGetAllResponseList);
        }

    }
}