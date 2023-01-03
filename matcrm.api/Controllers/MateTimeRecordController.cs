using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;
using matcrm.data.Models.Tables;
using matcrm.service.Common;
using matcrm.service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using matcrm.api.SignalR;
using Swashbuckle.AspNetCore.Annotations;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class MateTimeRecordController : Controller
    {
        private readonly IMateTimeRecordService _mateTimeRecordService;
        private readonly IMateProjectTimeRecordService _mateProjectTimeRecordService;
        private readonly IMateTaskTimeRecordService _mateTaskTimeRecordService;
        private readonly IContractService _contractService;
        private readonly IEmployeeProjectService _employeeProjectService;
        private readonly IEmployeeTaskService _employeeTaskService;
        private readonly IEmployeeProjectActivityService _employeeProjectActivityService;
        private readonly IEmployeeTaskActivityService _employeeTaskActivityService;
        private readonly IMateSubTaskTimeRecordService _mateSubTaskTimeRecordService;
        private readonly IMateChildTaskTimeRecordService _mateChildTaskTimeRecordService;
        private readonly IEmployeeSubTaskActivityService _employeeSubTaskActivityService;
        private readonly IEmployeeChildTaskActivityService _employeeChildTaskActivityService;
        private readonly IUserService _userService;
        private readonly IEmployeeSubTaskService _employeeSubTaskService;
        private readonly IEmployeeChildTaskService _employeeChildTaskService;
        private readonly IEmployeeClientTaskService _employeeClientTaskService;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private readonly IMateTicketTimeRecordService _mateTicketTimeRecordService;
        private readonly IMateTicketActivityService _mateTicketActivityService;
        private readonly IMateTicketService _mateTicketService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;
        public MateTimeRecordController(IMateTimeRecordService mateTimeRecordService,
        IMateProjectTimeRecordService mateProjectTimeRecordService,
        IMateTaskTimeRecordService mateTaskTimeRecordService,
        IContractService contractService,
        IEmployeeProjectService employeeProjectService,
        IEmployeeProjectActivityService employeeProjectActivityService,
        IEmployeeTaskActivityService employeeTaskActivityService,
        IEmployeeTaskService employeeTaskService,
        IMateSubTaskTimeRecordService mateSubTaskTimeRecordService,
        IMateChildTaskTimeRecordService mateChildTaskTimeRecordService,
        IEmployeeSubTaskActivityService employeeSubTaskActivityService,
        IEmployeeChildTaskActivityService employeeChildTaskActivityService,
        IUserService userService,
        IEmployeeSubTaskService employeeSubTaskService,
        IEmployeeChildTaskService employeeChildTaskService,
        IEmployeeClientTaskService employeeClientTaskService,
        IHubContext<BroadcastHub, IHubClient> hubContext,
        IMateTicketTimeRecordService mateTicketTimeRecordService,
        IMateTicketActivityService mateTicketActivityService,
        IMateTicketService mateTicketService,
        IMapper mapper)
        {
            _mateTimeRecordService = mateTimeRecordService;
            _mateProjectTimeRecordService = mateProjectTimeRecordService;
            _mateTaskTimeRecordService = mateTaskTimeRecordService;
            _contractService = contractService;
            _employeeProjectService = employeeProjectService;
            _employeeTaskService = employeeTaskService;
            _employeeProjectActivityService = employeeProjectActivityService;
            _employeeTaskActivityService = employeeTaskActivityService;
            _mateSubTaskTimeRecordService = mateSubTaskTimeRecordService;
            _mateChildTaskTimeRecordService = mateChildTaskTimeRecordService;
            _employeeSubTaskActivityService = employeeSubTaskActivityService;
            _employeeChildTaskActivityService = employeeChildTaskActivityService;
            _employeeSubTaskService = employeeSubTaskService;
            _employeeChildTaskService = employeeChildTaskService;
            _employeeClientTaskService = employeeClientTaskService;
            _userService = userService;
            _mapper = mapper;
            _hubContext = hubContext;
            _mateTicketTimeRecordService = mateTicketTimeRecordService;
            _mateTicketActivityService = mateTicketActivityService;
            _mateTicketService = mateTicketService;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<MateTimeRecordAddUpdateResponse>> Add([FromBody] MateTimeRecordAddUpdateRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            if (requestmodel.ProjectId != null || requestmodel.TaskId != null || requestmodel.TicketId != null || requestmodel.SubTaskId != null || requestmodel.ChildTaskId != null)
            {
                var model = _mapper.Map<MateTimeRecord>(requestmodel);
                MateTimeRecord mateTimeRecordObj = new MateTimeRecord();
                MateTimeRecordAddUpdateResponse mateTimeRecordAddUpdateResponseObj = new MateTimeRecordAddUpdateResponse();
                model.UserId = UserId;
                if (model.Duration != null)
                {
                    mateTimeRecordObj = await _mateTimeRecordService.CheckInsertOrUpdate(model);

                    if (requestmodel != null && mateTimeRecordObj != null)
                    {
                        //Project time record
                        if (requestmodel.ProjectId != null && requestmodel.ProjectId > 0)
                        {
                            var mateProjectTimeRecordObj = new MateProjectTimeRecord();
                            mateProjectTimeRecordObj.MateTimeRecordId = mateTimeRecordObj.Id;
                            mateProjectTimeRecordObj.ProjectId = requestmodel.ProjectId;
                            var AddUpdateProjectTimeRecordObj = await _mateProjectTimeRecordService.CheckInsertOrUpdate(mateProjectTimeRecordObj);
                            if (AddUpdateProjectTimeRecordObj != null)
                            {
                                EmployeeProjectActivity employeeProjectActivityObj = new EmployeeProjectActivity();
                                employeeProjectActivityObj.ProjectId = requestmodel.ProjectId;
                                employeeProjectActivityObj.UserId = UserId;
                                employeeProjectActivityObj.Activity = Enums.EmployeeProjectActivityEnum.Project_time_record_created.ToString().Replace("_", " ");
                                var AddUpdateProjectActivityObj = await _employeeProjectActivityService.CheckInsertOrUpdate(employeeProjectActivityObj);
                                await _hubContext.Clients.All.OnProjectModuleEvent(requestmodel.ProjectId, TenantId);
                            }
                            mateTimeRecordAddUpdateResponseObj.ProjectTicketTaskId = requestmodel.ProjectId;
                            mateTimeRecordAddUpdateResponseObj.Type = "Project";
                            var projectObj = _employeeProjectService.GetEmployeeProjectById(requestmodel.ProjectId.Value);
                            mateTimeRecordAddUpdateResponseObj.Name = projectObj?.Name;
                        }
                        //Ticket time record
                        if (requestmodel.TicketId != null && requestmodel.TicketId > 0)
                        {
                            var mateTicketTimeRecordObj = new MateTicketTimeRecord();
                            mateTicketTimeRecordObj.MateTimeRecordId = mateTimeRecordObj.Id;
                            mateTicketTimeRecordObj.MateTicketId = requestmodel.TicketId;
                            var AddUpdateTicketTimeRecordObj = await _mateTicketTimeRecordService.CheckInsertOrUpdate(mateTicketTimeRecordObj);
                            if (AddUpdateTicketTimeRecordObj != null)
                            {
                                MateTicketActivity mateTicketActivityObj = new MateTicketActivity();
                                mateTicketActivityObj.MateTicketId = requestmodel.TicketId;
                                mateTicketActivityObj.CreatedBy = UserId;
                                mateTicketActivityObj.Activity = Enums.MateTicketActivityEnum.Ticket_time_record_created.ToString().Replace("_", " ");
                                var AddUpdateTicketActivityObj = await _mateTicketActivityService.CheckInsertOrUpdate(mateTicketActivityObj);
                                await _hubContext.Clients.All.OnMateTicketModuleEvent(requestmodel.TicketId, TenantId);
                            }
                            mateTimeRecordAddUpdateResponseObj.ProjectTicketTaskId = requestmodel.TicketId;
                            mateTimeRecordAddUpdateResponseObj.Type = "Ticket";
                            var ticketObj = _mateTicketService.GetById(requestmodel.TicketId.Value);
                            mateTimeRecordAddUpdateResponseObj.Name = ticketObj?.Name;
                        }
                        //Task time record
                        if (requestmodel.TaskId != null && requestmodel.TaskId > 0)
                        {
                            var mateTaskTimeRecordObj = new MateTaskTimeRecord();
                            mateTaskTimeRecordObj.MateTimeRecordId = mateTimeRecordObj.Id;
                            mateTaskTimeRecordObj.TaskId = requestmodel.TaskId;
                            var AddUpdateTaskTimeRecordObj = await _mateTaskTimeRecordService.CheckInsertOrUpdate(mateTaskTimeRecordObj);
                            if (AddUpdateTaskTimeRecordObj != null)
                            {
                                EmployeeTaskActivity employeeTaskActivityObj = new EmployeeTaskActivity();
                                //employeeTaskActivityObj.ProjectId = requestmodel.ProjectId;
                                employeeTaskActivityObj.EmployeeTaskId = requestmodel.TaskId;
                                employeeTaskActivityObj.UserId = UserId;
                                employeeTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_time_record_created.ToString().Replace("_", " ");
                                var AddUpdateTaskActivityObj = await _employeeTaskActivityService.CheckInsertOrUpdate(employeeTaskActivityObj);
                                await _hubContext.Clients.All.OnEmployeeTaskModuleEvent(requestmodel.TaskId, TenantId);
                            }
                            mateTimeRecordAddUpdateResponseObj.ProjectTicketTaskId = requestmodel.TaskId;
                            mateTimeRecordAddUpdateResponseObj.Type = "Task";
                            var taskObj = _employeeTaskService.GetTaskById(requestmodel.TaskId.Value);
                            mateTimeRecordAddUpdateResponseObj.Name = taskObj?.Name;
                        }
                        //Sub task time record
                        if (requestmodel.SubTaskId != null && requestmodel.SubTaskId > 0)
                        {
                            var mateSubTaskTimeRecordObj = new MateSubTaskTimeRecord();
                            mateSubTaskTimeRecordObj.MateTimeRecordId = mateTimeRecordObj.Id;
                            mateSubTaskTimeRecordObj.SubTaskId = requestmodel.SubTaskId;
                            var AddUpdateSubTaskTimeRecordObj = await _mateSubTaskTimeRecordService.CheckInsertOrUpdate(mateSubTaskTimeRecordObj);
                            if (AddUpdateSubTaskTimeRecordObj != null)
                            {
                                EmployeeSubTaskActivity subTaskActivityObj = new EmployeeSubTaskActivity();
                                subTaskActivityObj.EmployeeSubTaskId = requestmodel.SubTaskId;
                                subTaskActivityObj.UserId = UserId;
                                subTaskActivityObj.Activity = Enums.EmployeeSubTaskActivityEnum.Subtask_time_record_created.ToString().Replace("_", " ");
                                var AddUpdateSubTaskActivityObj = await _employeeSubTaskActivityService.CheckInsertOrUpdate(subTaskActivityObj);
                            }
                            mateTimeRecordAddUpdateResponseObj.ProjectTicketTaskId = requestmodel.TaskId;
                            mateTimeRecordAddUpdateResponseObj.Type = "SubTask";
                            var subTaskObj = _employeeSubTaskService.GetSubTaskById(requestmodel.SubTaskId.Value);
                            mateTimeRecordAddUpdateResponseObj.Name = subTaskObj?.Description;
                        }
                        //Child task time record
                        if (requestmodel.ChildTaskId != null && requestmodel.ChildTaskId > 0)
                        {
                            var mateChildTaskTimeRecordObj = new MateChildTaskTimeRecord();
                            mateChildTaskTimeRecordObj.MateTimeRecordId = mateTimeRecordObj.Id;
                            mateChildTaskTimeRecordObj.ChildTaskId = requestmodel.ChildTaskId;
                            var AddUpdateChidTaskTimeRecordObj = await _mateChildTaskTimeRecordService.CheckInsertOrUpdate(mateChildTaskTimeRecordObj);
                            if (AddUpdateChidTaskTimeRecordObj != null)
                            {
                                EmployeeChildTaskActivity childTaskActivityObj = new EmployeeChildTaskActivity();
                                childTaskActivityObj.EmployeeChildTaskId = requestmodel.ChildTaskId;
                                childTaskActivityObj.UserId = UserId;
                                childTaskActivityObj.Activity = Enums.EmployeeChidTaskActivityEnum.Childtask_time_record_created.ToString().Replace("_", " ");
                                var AddUpdateChildTaskActivityObj = await _employeeChildTaskActivityService.CheckInsertOrUpdate(childTaskActivityObj);
                            }
                            mateTimeRecordAddUpdateResponseObj.ProjectTicketTaskId = requestmodel.ChildTaskId;
                            mateTimeRecordAddUpdateResponseObj.Type = "ChildTask";
                            var childTaskObj = _employeeChildTaskService.GetChildTaskById(requestmodel.ChildTaskId.Value);
                            mateTimeRecordAddUpdateResponseObj.Name = childTaskObj?.Description;
                        }
                        mateTimeRecordAddUpdateResponseObj.Id = mateTimeRecordObj.Id;
                        if (mateTimeRecordObj.Duration != null)
                        {
                            long Duration = mateTimeRecordObj.Duration.Value;
                            TimeSpan timeSpan = TimeSpan.FromMinutes(Duration);
                            mateTimeRecordAddUpdateResponseObj.Duration = timeSpan.ToString(@"hh\:mm");
                        }
                        mateTimeRecordAddUpdateResponseObj.Comment = mateTimeRecordObj.Comment;
                        mateTimeRecordAddUpdateResponseObj.CreatedOn = mateTimeRecordObj.CreatedOn;
                        mateTimeRecordAddUpdateResponseObj.ServiceArticleId = mateTimeRecordObj?.ServiceArticleId;
                        mateTimeRecordAddUpdateResponseObj.IsBillable = mateTimeRecordObj?.IsBillable;
                    }
                }
                return new OperationResult<MateTimeRecordAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Time record added successfully", mateTimeRecordAddUpdateResponseObj);
            }
            else
            {
                return new OperationResult<MateTimeRecordAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Please provide Project or task id or ticketid or subtaskid or childtaskid");
            }
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<MateTimeRecordAddUpdateResponse>> Update([FromBody] MateTimeRecordAddUpdateRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            if (requestmodel.ProjectId != null || requestmodel.TaskId != null || requestmodel.TicketId != null || requestmodel.SubTaskId != null  || requestmodel.ChildTaskId != null)
            {
                var model = _mapper.Map<MateTimeRecord>(requestmodel);
                MateTimeRecord mateTimeRecordObj = new MateTimeRecord();
                MateTimeRecordAddUpdateResponse mateTimeRecordAddUpdateResponseObj = new MateTimeRecordAddUpdateResponse();
                model.UserId = UserId;
                if (model.Duration != null)
                {
                    // if (requestmodel.ProjectId != null)
                    // {
                    //     var IsProjectExitOrNot = _employeeProjectService.GetEmployeeProjectById(requestmodel.ProjectId.Value);
                    //     if (IsProjectExitOrNot == null)
                    //     {
                    //         return new OperationResult<MateTimeRecordAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Project not found");
                    //     }
                    // }
                    // if (requestmodel.TaskId != null)
                    // {
                    //     var IsTaskExitOrNot = _employeeTaskService.GetTaskById(requestmodel.TaskId.Value);
                    //     if (IsTaskExitOrNot == null)
                    //     {
                    //         return new OperationResult<MateTimeRecordAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Task not found");
                    //     }
                    // }

                    mateTimeRecordObj = await _mateTimeRecordService.CheckInsertOrUpdate(model);
                    if (requestmodel != null && mateTimeRecordObj != null)
                    {
                        //Project time record
                        if (requestmodel.ProjectId != null && requestmodel.ProjectId > 0)
                        {
                            var mateProjectTimeRecordObj = new MateProjectTimeRecord();
                            mateProjectTimeRecordObj.MateTimeRecordId = mateTimeRecordObj.Id;
                            mateProjectTimeRecordObj.ProjectId = requestmodel.ProjectId;
                            var AddUpdateProjectTimeRecordObj = await _mateProjectTimeRecordService.CheckInsertOrUpdate(mateProjectTimeRecordObj);
                            if (AddUpdateProjectTimeRecordObj != null)
                            {
                                EmployeeProjectActivity employeeProjectActivityObj = new EmployeeProjectActivity();
                                employeeProjectActivityObj.ProjectId = requestmodel.ProjectId;
                                employeeProjectActivityObj.UserId = UserId;
                                employeeProjectActivityObj.Activity = Enums.EmployeeProjectActivityEnum.Project_time_record_updated.ToString().Replace("_", " ");
                                var AddUpdateProjectActivityObj = await _employeeProjectActivityService.CheckInsertOrUpdate(employeeProjectActivityObj);
                                await _hubContext.Clients.All.OnProjectModuleEvent(requestmodel.ProjectId, TenantId);
                            }
                            mateTimeRecordAddUpdateResponseObj.ProjectTicketTaskId = requestmodel.ProjectId;
                            mateTimeRecordAddUpdateResponseObj.Type = "Project";
                            var projectObj = _employeeProjectService.GetEmployeeProjectById(requestmodel.ProjectId.Value);
                            mateTimeRecordAddUpdateResponseObj.Name = projectObj?.Name;
                        }
                        //Ticket time record
                        if (requestmodel.TicketId != null && requestmodel.TicketId > 0)
                        {
                            var mateTicketTimeRecordObj = new MateTicketTimeRecord();
                            mateTicketTimeRecordObj.MateTimeRecordId = mateTimeRecordObj.Id;
                            mateTicketTimeRecordObj.MateTicketId = requestmodel.TicketId;
                            var AddUpdateTicketTimeRecordObj = await _mateTicketTimeRecordService.CheckInsertOrUpdate(mateTicketTimeRecordObj);
                            if (AddUpdateTicketTimeRecordObj != null)
                            {
                                MateTicketActivity mateTicketActivityObj = new MateTicketActivity();
                                mateTicketActivityObj.MateTicketId = requestmodel.TicketId;
                                mateTicketActivityObj.CreatedBy = UserId;
                                mateTicketActivityObj.Activity = Enums.MateTicketActivityEnum.Ticket_time_record_updated.ToString().Replace("_", " ");
                                var AddUpdateTicketActivityObj = await _mateTicketActivityService.CheckInsertOrUpdate(mateTicketActivityObj);
                                await _hubContext.Clients.All.OnMateTicketModuleEvent(requestmodel.TicketId, TenantId);
                            }
                            mateTimeRecordAddUpdateResponseObj.ProjectTicketTaskId = requestmodel.TicketId;
                            mateTimeRecordAddUpdateResponseObj.Type = "Ticket";
                            var ticketObj = _mateTicketService.GetById(requestmodel.TicketId.Value);
                            mateTimeRecordAddUpdateResponseObj.Name = ticketObj?.Name;
                        }
                        //Task time record
                        if (requestmodel.TaskId != null && requestmodel.TaskId > 0)
                        {
                            var mateTaskTimeRecordObj = new MateTaskTimeRecord();
                            mateTaskTimeRecordObj.MateTimeRecordId = mateTimeRecordObj.Id;
                            mateTaskTimeRecordObj.TaskId = requestmodel.TaskId;
                            var AddUpdateTaskTimeRecordObj = await _mateTaskTimeRecordService.CheckInsertOrUpdate(mateTaskTimeRecordObj);
                            if (AddUpdateTaskTimeRecordObj != null)
                            {
                                EmployeeTaskActivity employeeTaskActivityObj = new EmployeeTaskActivity();
                                //employeeTaskActivityObj.ProjectId = requestmodel.ProjectId;
                                employeeTaskActivityObj.EmployeeTaskId = requestmodel.TaskId;
                                employeeTaskActivityObj.UserId = UserId;
                                employeeTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_time_record_updated.ToString().Replace("_", " ");
                                var AddUpdateTaskActivityObj = await _employeeTaskActivityService.CheckInsertOrUpdate(employeeTaskActivityObj);
                                await _hubContext.Clients.All.OnEmployeeTaskModuleEvent(requestmodel.TaskId, TenantId);
                            }
                            mateTimeRecordAddUpdateResponseObj.ProjectTicketTaskId = requestmodel.TaskId;
                            mateTimeRecordAddUpdateResponseObj.Type = "Task";
                            var taskObj = _employeeTaskService.GetTaskById(requestmodel.TaskId.Value);
                            mateTimeRecordAddUpdateResponseObj.Name = taskObj?.Name;
                        }
                        //Sub task time record
                        if (requestmodel.SubTaskId != null && requestmodel.SubTaskId > 0)
                        {
                            var mateSubTaskTimeRecordObj = new MateSubTaskTimeRecord();
                            mateSubTaskTimeRecordObj.MateTimeRecordId = mateTimeRecordObj.Id;
                            mateSubTaskTimeRecordObj.SubTaskId = requestmodel.SubTaskId;
                            var AddUpdateSubTaskTimeRecordObj = await _mateSubTaskTimeRecordService.CheckInsertOrUpdate(mateSubTaskTimeRecordObj);
                            if (AddUpdateSubTaskTimeRecordObj != null)
                            {
                                EmployeeSubTaskActivity subTaskActivityObj = new EmployeeSubTaskActivity();
                                subTaskActivityObj.EmployeeSubTaskId = requestmodel.SubTaskId;
                                subTaskActivityObj.UserId = UserId;
                                subTaskActivityObj.Activity = Enums.EmployeeSubTaskActivityEnum.Subtask_time_record_created.ToString().Replace("_", " ");
                                var AddUpdateSubTaskActivityObj = await _employeeSubTaskActivityService.CheckInsertOrUpdate(subTaskActivityObj);
                            }
                            mateTimeRecordAddUpdateResponseObj.ProjectTicketTaskId = requestmodel.TaskId;
                            mateTimeRecordAddUpdateResponseObj.Type = "SubTask";
                            var subTaskObj = _employeeSubTaskService.GetSubTaskById(requestmodel.SubTaskId.Value);
                            mateTimeRecordAddUpdateResponseObj.Name = subTaskObj?.Description;
                        }
                        //Child task time record
                        if (requestmodel.ChildTaskId != null && requestmodel.ChildTaskId > 0)
                        {
                            var mateChildTaskTimeRecordObj = new MateChildTaskTimeRecord();
                            mateChildTaskTimeRecordObj.MateTimeRecordId = mateTimeRecordObj.Id;
                            mateChildTaskTimeRecordObj.ChildTaskId = requestmodel.ChildTaskId;
                            var AddUpdateChidTaskTimeRecordObj = await _mateChildTaskTimeRecordService.CheckInsertOrUpdate(mateChildTaskTimeRecordObj);
                            if (AddUpdateChidTaskTimeRecordObj != null)
                            {
                                EmployeeChildTaskActivity childTaskActivityObj = new EmployeeChildTaskActivity();
                                childTaskActivityObj.EmployeeChildTaskId = requestmodel.ChildTaskId;
                                childTaskActivityObj.UserId = UserId;
                                childTaskActivityObj.Activity = Enums.EmployeeChidTaskActivityEnum.Childtask_time_record_created.ToString().Replace("_", " ");
                                var AddUpdateChildTaskActivityObj = await _employeeChildTaskActivityService.CheckInsertOrUpdate(childTaskActivityObj);
                            }
                            mateTimeRecordAddUpdateResponseObj.ProjectTicketTaskId = requestmodel.ChildTaskId;
                            mateTimeRecordAddUpdateResponseObj.Type = "ChildTask";
                            var childTaskObj = _employeeChildTaskService.GetChildTaskById(requestmodel.ChildTaskId.Value);
                            mateTimeRecordAddUpdateResponseObj.Name = childTaskObj?.Description;
                        }
                        mateTimeRecordAddUpdateResponseObj.Id = mateTimeRecordObj.Id;                       
                        if (mateTimeRecordObj.Duration != null)
                        {
                            long Duration = mateTimeRecordObj.Duration.Value;
                            TimeSpan timeSpan = TimeSpan.FromMinutes(Duration);
                            mateTimeRecordAddUpdateResponseObj.Duration = timeSpan.ToString(@"hh\:mm");
                        }
                        mateTimeRecordAddUpdateResponseObj.Comment = mateTimeRecordObj.Comment;                       
                        mateTimeRecordAddUpdateResponseObj.CreatedOn = mateTimeRecordObj.CreatedOn;
                        mateTimeRecordAddUpdateResponseObj.ServiceArticleId = mateTimeRecordObj?.ServiceArticleId;
                        mateTimeRecordAddUpdateResponseObj.IsBillable = mateTimeRecordObj?.IsBillable;
                    }
                }
                //var mateTimeRecordAddUpdateResponseObj = _mapper.Map<MateTimeRecordAddUpdateResponse>(mateTimeRecordObj);
                return new OperationResult<MateTimeRecordAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Time record updated successfully", mateTimeRecordAddUpdateResponseObj);
            }
            else
            {
                return new OperationResult<MateTimeRecordAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Please provide Project or task id or ticketid or subtaskid or childtaskid");
            }
        }

        // [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        // [HttpGet]
        // public async Task<OperationResult<List<MateTimeRecordListResponse>>> List()
        // {
        //     var mateTimeRecordList = _mateTimeRecordService.GetAll();

        //     var mateTimeRecordListResponseList = _mapper.Map<List<MateTimeRecordListResponse>>(mateTimeRecordList);

        //     return new OperationResult<List<MateTimeRecordListResponse>>(true, System.Net.HttpStatusCode.OK, "", mateTimeRecordListResponseList);
        // }

        // [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        // [HttpGet("{Id}")]
        // public async Task<OperationResult<MateTimeRecordDetailResponse>> Detail(int Id)
        // {
        //     var mateTimeRecordObj = _mateTimeRecordService.GetById(Id);

        //     var mateTimeRecordDetailResponseObj = _mapper.Map<MateTimeRecordDetailResponse>(mateTimeRecordObj);

        //     return new OperationResult<MateTimeRecordDetailResponse>(true, System.Net.HttpStatusCode.OK, "", mateTimeRecordDetailResponseObj);
        // }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete("{Id}")]
        public async Task<OperationResult> Remove(long Id)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            if (Id != null && Id > 0)
            {
                var mateTimeRecordObj = await _mateTimeRecordService.DeleteMateTimeRecord(Id);
                if (mateTimeRecordObj != null)
                {
                    //start project record
                    var mateProjectTimeRecordObj = _mateProjectTimeRecordService.GetBymateTimeRecordId(Id);
                    if (mateProjectTimeRecordObj != null)
                    {
                        EmployeeProjectActivity employeeProjectActivityObj = new EmployeeProjectActivity();
                        employeeProjectActivityObj.ProjectId = mateProjectTimeRecordObj.ProjectId;
                        employeeProjectActivityObj.UserId = UserId;
                        employeeProjectActivityObj.Activity = Enums.EmployeeProjectActivityEnum.Project_time_record_removed.ToString().Replace("_", " ");
                        var AddUpdateProjectActivityObj = await _employeeProjectActivityService.CheckInsertOrUpdate(employeeProjectActivityObj);
                        await _hubContext.Clients.All.OnProjectModuleEvent(mateProjectTimeRecordObj.ProjectId, TenantId);
                    }
                    //end project record

                    //start Ticket record
                    var mateTicketTimeRecordObj = _mateTicketTimeRecordService.GetBymateTimeRecordId(Id);
                    if (mateTicketTimeRecordObj != null)
                    {
                        MateTicketActivity mateTicketActivityObj = new MateTicketActivity();
                        mateTicketActivityObj.MateTicketId = mateTicketTimeRecordObj.MateTicketId;
                        mateTicketActivityObj.CreatedBy = UserId;
                        mateTicketActivityObj.Activity = Enums.MateTicketActivityEnum.Ticket_time_record_removed.ToString().Replace("_", " ");
                        var AddUpdateTicketActivityObj = await _mateTicketActivityService.CheckInsertOrUpdate(mateTicketActivityObj);
                        await _hubContext.Clients.All.OnMateTicketModuleEvent(mateTicketTimeRecordObj.MateTicketId, TenantId);
                    }
                    //end Ticket record

                    //start task record
                    var mateTaskTimeRecordObj = _mateTaskTimeRecordService.GetBymateTimeRecordId(Id);
                    if (mateTaskTimeRecordObj != null)
                    {
                        EmployeeTaskActivity employeeTaskActivityObj = new EmployeeTaskActivity();
                        employeeTaskActivityObj.EmployeeTaskId = mateTaskTimeRecordObj.TaskId;
                        employeeTaskActivityObj.UserId = UserId;
                        employeeTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_time_record_removed.ToString().Replace("_", " ");
                        var AddUpdateTaskActivity = await _employeeTaskActivityService.CheckInsertOrUpdate(employeeTaskActivityObj);
                        await _hubContext.Clients.All.OnEmployeeTaskModuleEvent(mateTaskTimeRecordObj.TaskId, TenantId);
                    }
                    //end task record

                    //start subtask record
                    var subTaskTimeRecordObj = _mateSubTaskTimeRecordService.GetBymateTimeRecordId(Id);
                    if (subTaskTimeRecordObj != null)
                    {
                        EmployeeSubTaskActivity subTaskActivityObj = new EmployeeSubTaskActivity();
                        subTaskActivityObj.EmployeeSubTaskId = subTaskTimeRecordObj.SubTaskId;
                        subTaskActivityObj.UserId = UserId;
                        subTaskActivityObj.Activity = Enums.EmployeeSubTaskActivityEnum.Subtask_time_record_removed.ToString().Replace("_", " ");
                        var AddUpdateSubTaskActivity = await _employeeSubTaskActivityService.CheckInsertOrUpdate(subTaskActivityObj);
                    }
                    //end subtask record

                    //start child record
                    var childTaskTimeRecordObj = _mateChildTaskTimeRecordService.GetBymateTimeRecordId(Id);
                    if (childTaskTimeRecordObj != null)
                    {
                        EmployeeChildTaskActivity childTaskActivityObj = new EmployeeChildTaskActivity();
                        childTaskActivityObj.EmployeeChildTaskId = childTaskTimeRecordObj.ChildTaskId;
                        childTaskActivityObj.UserId = UserId;
                        childTaskActivityObj.Activity = Enums.EmployeeChidTaskActivityEnum.Childtask_time_record_removed.ToString().Replace("_", " ");
                        var AddUpdateChildTaskActivity = await _employeeChildTaskActivityService.CheckInsertOrUpdate(childTaskActivityObj);
                    }
                    //end child record
                }
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "", Id);
            }
            else
            {
                return new OperationResult(false, System.Net.HttpStatusCode.OK, "Please provide id", Id);
            }
        }


        // [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        // [HttpPost]
        // public async Task<OperationResult<MateTimeRecordInvoiceResponse>> Invoice([FromBody] MateTimeRecordInvoiceRequest model)
        // {
        //     ClaimsPrincipal user = this.User as ClaimsPrincipal;
        //     TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
        //     MateTimeRecordInvoiceResponse mateTimeRecordInvoiceResponseObj = new MateTimeRecordInvoiceResponse();
        //     var AllUsers = _userService.GetAll();
        //     if (model != null)
        //     {
        //         //start for project                
        //         var employeeProjectList = _employeeProjectService.GetAllByClient(model.ClientId, TenantId);

        //         if (employeeProjectList != null && employeeProjectList.Count() > 0)
        //         {
        //             foreach (var employeeProject in employeeProjectList)
        //             {
        //                 long? projectBillable = 0;
        //                 long? projectNonBillable = 0;

        //                 MateTimeRecordProjectListInvoiceResponse mateTimeRecordProjectListInvoiceResponseObj = new MateTimeRecordProjectListInvoiceResponse();
        //                 mateTimeRecordProjectListInvoiceResponseObj.EmployeeProjectId = employeeProject.Id;
        //                 mateTimeRecordProjectListInvoiceResponseObj.Description = employeeProject.Description;
        //                 //getting time record for project according to start and enddate
        //                 var mateProjectTimeRecordList = _mateProjectTimeRecordService.GetMateProjectTimeRecordByProjectId(employeeProject.Id, model);
        //                 if (mateProjectTimeRecordList != null && mateProjectTimeRecordList.Count() > 0)
        //                 {
        //                     foreach (var projectTimeRecord in mateProjectTimeRecordList)
        //                     {
        //                         TimeRecordInvoiceResponse mateProjectTimeRecordInvoiceResponseObj = new TimeRecordInvoiceResponse();
        //                         if (projectTimeRecord != null && projectTimeRecord.MateTimeRecord != null)
        //                         {
        //                             if (projectTimeRecord.MateTimeRecord.UserId != null)
        //                             {
        //                                 var userObj = AllUsers.Where(t => t.Id == projectTimeRecord.MateTimeRecord.UserId).FirstOrDefault();
        //                                 if (userObj != null)
        //                                 {
        //                                     var UserName = userObj.FirstName + " " + userObj.LastName;
        //                                     if (!String.IsNullOrEmpty(UserName))
        //                                     {
        //                                         mateProjectTimeRecordInvoiceResponseObj.UserName = UserName;
        //                                     }
        //                                     else
        //                                     {
        //                                         mateProjectTimeRecordInvoiceResponseObj.UserName = userObj.Email;
        //                                     }
        //                                 }
        //                             }
        //                             mateProjectTimeRecordInvoiceResponseObj.MateTimeRecordId = projectTimeRecord.MateTimeRecord.Id;
        //                             mateProjectTimeRecordInvoiceResponseObj.Comment = projectTimeRecord.MateTimeRecord.Comment;
        //                             if (projectTimeRecord.MateTimeRecord.CreatedOn != null)
        //                             {
        //                                 DateTime startdate = projectTimeRecord.MateTimeRecord.CreatedOn.Value;
        //                                 mateProjectTimeRecordInvoiceResponseObj.Startdate = startdate.ToString("dd/MM/yyyy");
        //                             }
        //                             mateProjectTimeRecordInvoiceResponseObj.IsBillable = projectTimeRecord.MateTimeRecord.IsBillable;
        //                             mateProjectTimeRecordInvoiceResponseObj.Duration = projectTimeRecord.MateTimeRecord.Duration;
        //                             if (mateProjectTimeRecordInvoiceResponseObj.IsBillable == true)
        //                             {
        //                                 projectBillable = projectBillable + projectTimeRecord.MateTimeRecord.Duration;
        //                             }
        //                             else
        //                             {
        //                                 projectNonBillable = projectNonBillable + projectTimeRecord.MateTimeRecord.Duration;
        //                             }
        //                         }
        //                         mateTimeRecordProjectListInvoiceResponseObj.ProjectTimeRecords.Add(mateProjectTimeRecordInvoiceResponseObj);
        //                     }

        //                 }
        //                 mateTimeRecordProjectListInvoiceResponseObj.TotalBillable = projectBillable.Value;
        //                 mateTimeRecordProjectListInvoiceResponseObj.TotalNonBillable = projectNonBillable.Value;
        //                 mateTimeRecordInvoiceResponseObj.Projects.Add(mateTimeRecordProjectListInvoiceResponseObj);
        //             }

        //         }
        //         //end for project

        //         //start for task
        //         var employeeTaskList = _employeeClientTaskService.GetTaskByClientWithTenant(model.ClientId, TenantId);

        //         if (employeeTaskList != null && employeeTaskList.Count() > 0)
        //         {
        //             foreach (var task in employeeTaskList)
        //             {
        //                 long? taskBillable = 0;
        //                 long? taskNonBillable = 0;
        //                 MateTimeRecordTaskListInvoiceResponse mateTimeRecordTaskListInvoiceResponseObj = new MateTimeRecordTaskListInvoiceResponse();
        //                 mateTimeRecordTaskListInvoiceResponseObj.EmployeeTaskId = task.Id;
        //                 mateTimeRecordTaskListInvoiceResponseObj.Description = task.Description;
        //                 //getting time record for task according to start and enddate
        //                 var mateTaskTimeRecordList = _mateTaskTimeRecordService.GetMateTaskTimeRecordByTaskId(task.Id, model);
        //                 if (mateTaskTimeRecordList != null && mateTaskTimeRecordList.Count() > 0)
        //                 {
        //                     foreach (var taskTimeRecord in mateTaskTimeRecordList)
        //                     {
        //                         TimeRecordInvoiceResponse mateTaskTimeRecordInvoiceResponseObj = new TimeRecordInvoiceResponse();
        //                         if (taskTimeRecord != null && taskTimeRecord.MateTimeRecord != null)
        //                         {
        //                             if (taskTimeRecord.MateTimeRecord.UserId != null)
        //                             {
        //                                 var userObj = AllUsers.Where(t => t.Id == taskTimeRecord.MateTimeRecord.UserId).FirstOrDefault();
        //                                 if (userObj != null)
        //                                 {
        //                                     var UserName = userObj.FirstName + " " + userObj.LastName;
        //                                     if (!String.IsNullOrEmpty(UserName))
        //                                     {
        //                                         mateTaskTimeRecordInvoiceResponseObj.UserName = UserName;
        //                                     }
        //                                     else
        //                                     {
        //                                         mateTaskTimeRecordInvoiceResponseObj.UserName = userObj.Email;
        //                                     }
        //                                 }
        //                             }

        //                             mateTaskTimeRecordInvoiceResponseObj.MateTimeRecordId = taskTimeRecord.MateTimeRecord.Id;
        //                             mateTaskTimeRecordInvoiceResponseObj.Comment = taskTimeRecord.MateTimeRecord.Comment;
        //                             if (taskTimeRecord.MateTimeRecord.CreatedOn != null)
        //                             {
        //                                 DateTime startdate = taskTimeRecord.MateTimeRecord.CreatedOn.Value;
        //                                 mateTaskTimeRecordInvoiceResponseObj.Startdate = startdate.ToString("dd/MM/yyyy");
        //                             }
        //                             mateTaskTimeRecordInvoiceResponseObj.IsBillable = taskTimeRecord.MateTimeRecord.IsBillable;
        //                             mateTaskTimeRecordInvoiceResponseObj.Duration = taskTimeRecord.MateTimeRecord.Duration;
        //                             if (mateTaskTimeRecordInvoiceResponseObj.IsBillable == true)
        //                             {
        //                                 taskBillable = taskBillable + taskTimeRecord.MateTimeRecord.Duration;
        //                             }
        //                             else
        //                             {
        //                                 taskNonBillable = taskNonBillable + taskTimeRecord.MateTimeRecord.Duration;
        //                             }
        //                         }
        //                         mateTimeRecordTaskListInvoiceResponseObj.TaskTimerecords.Add(mateTaskTimeRecordInvoiceResponseObj);
        //                     }
        //                 }
        //                 mateTimeRecordTaskListInvoiceResponseObj.TotalBillable = taskBillable.Value;
        //                 mateTimeRecordTaskListInvoiceResponseObj.TotalNonBillable = taskNonBillable.Value;
        //                 mateTimeRecordInvoiceResponseObj.Tasks.Add(mateTimeRecordTaskListInvoiceResponseObj);

        //                 //start for subtask
        //                 var subTaskList = _employeeSubTaskService.GetAllSubTaskByTask(task.Id);
        //                 if (subTaskList != null && subTaskList.Count > 0)
        //                 {
        //                     foreach (var subtask in subTaskList)
        //                     {
        //                         long? subTaskBillable = 0;
        //                         long? subTaskNonBillable = 0;
        //                         MateTimeRecordSubTaskListInvoiceResponse mateTimeRecordSubTaskListInvoiceResponseObj = new MateTimeRecordSubTaskListInvoiceResponse();
        //                         mateTimeRecordSubTaskListInvoiceResponseObj.SubTaskId = subtask.Id;
        //                         mateTimeRecordSubTaskListInvoiceResponseObj.Description = subtask.Description;
        //                         //getting time record for task according to start and enddate
        //                         var mateSubTaskTimeRecordList = _mateSubTaskTimeRecordService.GetBySubTaskId(subtask.Id, model);
        //                         if (mateSubTaskTimeRecordList != null && mateSubTaskTimeRecordList.Count() > 0)
        //                         {
        //                             foreach (var subTaskRecord in mateSubTaskTimeRecordList)
        //                             {
        //                                 TimeRecordInvoiceResponse subTaskTimeRecordInvoiceResponseObj = new TimeRecordInvoiceResponse();
        //                                 if (subTaskRecord != null && subTaskRecord.MateTimeRecord != null)
        //                                 {
        //                                     if (subTaskRecord.MateTimeRecord.UserId != null)
        //                                     {
        //                                         var userObj = AllUsers.Where(t => t.Id == subTaskRecord.MateTimeRecord.UserId).FirstOrDefault();
        //                                         if (userObj != null)
        //                                         {
        //                                             var UserName = userObj.FirstName + " " + userObj.LastName;
        //                                             if (!String.IsNullOrEmpty(UserName))
        //                                             {
        //                                                 subTaskTimeRecordInvoiceResponseObj.UserName = UserName;
        //                                             }
        //                                             else
        //                                             {
        //                                                 subTaskTimeRecordInvoiceResponseObj.UserName = userObj.Email;
        //                                             }
        //                                         }
        //                                     }

        //                                     subTaskTimeRecordInvoiceResponseObj.MateTimeRecordId = subTaskRecord.MateTimeRecord.Id;
        //                                     subTaskTimeRecordInvoiceResponseObj.Comment = subTaskRecord.MateTimeRecord.Comment;
        //                                     if (subTaskRecord.MateTimeRecord.CreatedOn != null)
        //                                     {
        //                                         DateTime startdate = subTaskRecord.MateTimeRecord.CreatedOn.Value;
        //                                         subTaskTimeRecordInvoiceResponseObj.Startdate = startdate.ToString("dd/MM/yyyy");
        //                                     }
        //                                     subTaskTimeRecordInvoiceResponseObj.IsBillable = subTaskRecord.MateTimeRecord.IsBillable;
        //                                     subTaskTimeRecordInvoiceResponseObj.Duration = subTaskRecord.MateTimeRecord.Duration;
        //                                     if (subTaskTimeRecordInvoiceResponseObj.IsBillable == true)
        //                                     {
        //                                         subTaskBillable = subTaskBillable + subTaskRecord.MateTimeRecord.Duration;
        //                                     }
        //                                     else
        //                                     {
        //                                         subTaskNonBillable = subTaskNonBillable + subTaskRecord.MateTimeRecord.Duration;
        //                                     }
        //                                 }
        //                                 mateTimeRecordSubTaskListInvoiceResponseObj.SubTaskTimerecords.Add(subTaskTimeRecordInvoiceResponseObj);
        //                             }
        //                         }
        //                         mateTimeRecordSubTaskListInvoiceResponseObj.TotalBillable = subTaskBillable.Value;
        //                         mateTimeRecordSubTaskListInvoiceResponseObj.TotalNonBillable = subTaskNonBillable.Value;
        //                         mateTimeRecordInvoiceResponseObj.SubTasks.Add(mateTimeRecordSubTaskListInvoiceResponseObj);
        //                     }

        //                     //start for childtask
        //                     var childTaskList = _employeeChildTaskService.GetAllChildTaskBySubTask(task.Id);
        //                     if (childTaskList != null && childTaskList.Count > 0)
        //                     {
        //                         foreach (var childTask in childTaskList)
        //                         {
        //                             long? childTaskBillable = 0;
        //                             long? childTaskNonBillable = 0;
        //                             MateTimeRecordChildTaskListInvoiceResponse mateTimeRecordChildTaskListInvoiceResponseObj = new MateTimeRecordChildTaskListInvoiceResponse();
        //                             mateTimeRecordChildTaskListInvoiceResponseObj.ChildTaskId = childTask.Id;
        //                             mateTimeRecordChildTaskListInvoiceResponseObj.Description = childTask.Description;
        //                             //getting time record for task according to start and enddate
        //                             var mateChildTaskTimeRecordList = _mateChildTaskTimeRecordService.GetByChildTaskId(childTask.Id, model);
        //                             if (mateChildTaskTimeRecordList != null && mateChildTaskTimeRecordList.Count() > 0)
        //                             {
        //                                 foreach (var childTaskRecord in mateChildTaskTimeRecordList)
        //                                 {
        //                                     TimeRecordInvoiceResponse childTaskTimeRecordInvoiceResponseObj = new TimeRecordInvoiceResponse();
        //                                     if (childTaskRecord != null && childTaskRecord.MateTimeRecord != null)
        //                                     {
        //                                         if (childTaskRecord.MateTimeRecord.UserId != null)
        //                                         {
        //                                             var userObj = AllUsers.Where(t => t.Id == childTaskRecord.MateTimeRecord.UserId).FirstOrDefault();
        //                                             if (userObj != null)
        //                                             {
        //                                                 var UserName = userObj.FirstName + " " + userObj.LastName;
        //                                                 if (!String.IsNullOrEmpty(UserName))
        //                                                 {
        //                                                     childTaskTimeRecordInvoiceResponseObj.UserName = UserName;
        //                                                 }
        //                                                 else
        //                                                 {
        //                                                     childTaskTimeRecordInvoiceResponseObj.UserName = userObj.Email;
        //                                                 }
        //                                             }
        //                                         }

        //                                         childTaskTimeRecordInvoiceResponseObj.MateTimeRecordId = childTaskRecord.MateTimeRecord.Id;
        //                                         childTaskTimeRecordInvoiceResponseObj.Comment = childTaskRecord.MateTimeRecord.Comment;
        //                                         if (childTaskRecord.MateTimeRecord.CreatedOn != null)
        //                                         {
        //                                             DateTime startdate = childTaskRecord.MateTimeRecord.CreatedOn.Value;
        //                                             childTaskTimeRecordInvoiceResponseObj.Startdate = startdate.ToString("dd/MM/yyyy");
        //                                         }
        //                                         childTaskTimeRecordInvoiceResponseObj.IsBillable = childTaskRecord.MateTimeRecord.IsBillable;
        //                                         childTaskTimeRecordInvoiceResponseObj.Duration = childTaskRecord.MateTimeRecord.Duration;
        //                                         if (childTaskTimeRecordInvoiceResponseObj.IsBillable == true)
        //                                         {
        //                                             childTaskBillable = childTaskBillable + childTaskRecord.MateTimeRecord.Duration;
        //                                         }
        //                                         else
        //                                         {
        //                                             childTaskNonBillable = childTaskNonBillable + childTaskRecord.MateTimeRecord.Duration;
        //                                         }
        //                                     }
        //                                     mateTimeRecordChildTaskListInvoiceResponseObj.ChildTaskTimerecords.Add(childTaskTimeRecordInvoiceResponseObj);
        //                                 }
        //                             }
        //                             mateTimeRecordChildTaskListInvoiceResponseObj.TotalBillable = childTaskBillable.Value;
        //                             mateTimeRecordChildTaskListInvoiceResponseObj.TotalNonBillable = childTaskNonBillable.Value;
        //                             mateTimeRecordInvoiceResponseObj.ChildTasks.Add(mateTimeRecordChildTaskListInvoiceResponseObj);
        //                         }
        //                     }
        //                     //end for childtask
        //                 }
        //                 //end for subtask
        //             }
        //         }
        //         //end for task                
        //     }

        //     return new OperationResult<MateTimeRecordInvoiceResponse>(true, System.Net.HttpStatusCode.OK, "", mateTimeRecordInvoiceResponseObj);
        // }


        // [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        // [HttpGet]
        // public async Task<OperationResult<MateTimeRecordListResponse>> List()
        // {
        //     ClaimsPrincipal user = this.User as ClaimsPrincipal;
        //     TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
        //     UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

        //     MateTimeRecordListResponse mateTimeRecordListResponseObj = new MateTimeRecordListResponse();
        //     // for project and it's time record
        //     var employeeProjectList = _employeeProjectService.GetAllByTenant(TenantId);
        //     foreach (var projectitem in employeeProjectList)
        //     {
        //         MateProjectTimeRecordListResponse mateProjectTimeRecordListResponse = new MateProjectTimeRecordListResponse();
        //         mateProjectTimeRecordListResponse.ProjectId = projectitem.Id;
        //         mateProjectTimeRecordListResponse.Name = projectitem.Name;

        //         var mateProjectTimeRecordList = _mateProjectTimeRecordService.GetByProjectIdAndUserId(projectitem.Id, UserId);
        //         var mateProjectTimeRecordAscList = mateProjectTimeRecordList.OrderBy(t => t.MateTimeRecord.CreatedOn).ToList();
        //         var mateProjectTimeRecordLast = mateProjectTimeRecordAscList.LastOrDefault();
        //         long ProjectTotalDuration = 0;
        //         if (mateProjectTimeRecordList != null && mateProjectTimeRecordList.Count > 0)
        //         {
        //             foreach (var projectTimeRecord in mateProjectTimeRecordList)
        //             {
        //                 if (projectTimeRecord.MateTimeRecord != null)
        //                 {
        //                     if (projectTimeRecord.MateTimeRecord.Duration != null)
        //                     {
        //                         ProjectTotalDuration = ProjectTotalDuration + projectTimeRecord.MateTimeRecord.Duration.Value;

        //                         TimeSpan timeSpan = TimeSpan.FromMinutes(ProjectTotalDuration);

        //                         mateProjectTimeRecordListResponse.TotalDuration = timeSpan.ToString(@"hh\:mm"); ;
        //                         if (mateProjectTimeRecordLast != null)
        //                         {
        //                             mateProjectTimeRecordListResponse.Enddate = mateProjectTimeRecordLast.MateTimeRecord.CreatedOn;
        //                         }
        //                         mateProjectTimeRecordListResponse.TotalCount = mateProjectTimeRecordList.Count;
        //                     }
        //                 }
        //             }
        //         }
        //         mateTimeRecordListResponseObj.Projects.Add(mateProjectTimeRecordListResponse);
        //     }

        //     // for task and it's time record
        //     var employeeTaskList = _employeeTaskService.GetAllTaskByTenantWithOutProject(TenantId);

        //     foreach (var taskitem in employeeTaskList)
        //     {
        //         MateTaskTimeRecordListResponse mateTaskTimeRecordListResponse = new MateTaskTimeRecordListResponse();
        //         mateTaskTimeRecordListResponse.TaskId = taskitem.Id;
        //         mateTaskTimeRecordListResponse.Name = taskitem.Description;

        //         var mateTaskTimeRecordList = _mateTaskTimeRecordService.GetByTaskIdAndUserId(taskitem.Id, UserId);
        //         var mateTaskTimeRecordAscList = mateTaskTimeRecordList.OrderBy(t => t.MateTimeRecord.CreatedOn).ToList();
        //         var mateTaskTimeRecordLast = mateTaskTimeRecordAscList.LastOrDefault();
        //         long TaskTotalDuration = 0;
        //         if (mateTaskTimeRecordList != null && mateTaskTimeRecordList.Count > 0)
        //         {
        //             foreach (var taskTimeRecord in mateTaskTimeRecordList)
        //             {
        //                 if (taskTimeRecord.MateTimeRecord != null)
        //                 {
        //                     if (taskTimeRecord.MateTimeRecord.Duration != null)
        //                     {
        //                         TaskTotalDuration = TaskTotalDuration + taskTimeRecord.MateTimeRecord.Duration.Value;

        //                         TimeSpan timeSpan = TimeSpan.FromMinutes(TaskTotalDuration);

        //                         mateTaskTimeRecordListResponse.TotalDuration = timeSpan.ToString(@"hh\:mm"); ;
        //                         if (mateTaskTimeRecordLast != null)
        //                         {
        //                             mateTaskTimeRecordListResponse.Enddate = mateTaskTimeRecordLast.MateTimeRecord.CreatedOn;
        //                         }
        //                         mateTaskTimeRecordListResponse.TotalCount = mateTaskTimeRecordList.Count;
        //                     }
        //                 }
        //             }
        //         }
        //         mateTimeRecordListResponseObj.Tasks.Add(mateTaskTimeRecordListResponse);
        //     }
        //     return new OperationResult<MateTimeRecordListResponse>(true, System.Net.HttpStatusCode.OK, "", mateTimeRecordListResponseObj);
        // }
        [SwaggerOperation(Description = "Use this api for getting time record list as per logged in user")]
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<MateTimeRecordListResponse>>> List()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            List<MateTimeRecordListResponse> mateTimeRecordListResponseList = new List<MateTimeRecordListResponse>();

            //start project time records
            var mateProjectTimeRecordList = _mateProjectTimeRecordService.GetByUserId(UserId);
            if (mateProjectTimeRecordList != null && mateProjectTimeRecordList.Count > 0)
            {
                foreach (var projectTimeRecord in mateProjectTimeRecordList)
                {
                    MateTimeRecordListResponse mateTimeRecordListResponseObj = new MateTimeRecordListResponse();
                    if (projectTimeRecord != null)
                    {
                        mateTimeRecordListResponseObj.ProjectTicketTaskId = projectTimeRecord.ProjectId;
                        mateTimeRecordListResponseObj.Name = projectTimeRecord.EmployeeProject?.Name;
                        mateTimeRecordListResponseObj.Type = "Project";
                        if (projectTimeRecord.MateTimeRecord != null)
                        {
                            mateTimeRecordListResponseObj.Id = projectTimeRecord.MateTimeRecord.Id;
                            mateTimeRecordListResponseObj.Comment = projectTimeRecord.MateTimeRecord.Comment;
                            mateTimeRecordListResponseObj.ServiceArticleId = projectTimeRecord.MateTimeRecord.ServiceArticleId;
                            mateTimeRecordListResponseObj.IsBillable = projectTimeRecord.MateTimeRecord.IsBillable;
                            if (projectTimeRecord.MateTimeRecord.Duration != null)
                            {
                                long ProjectTotalDuration = projectTimeRecord.MateTimeRecord.Duration.Value;
                                TimeSpan timeSpan = TimeSpan.FromMinutes(ProjectTotalDuration);
                                mateTimeRecordListResponseObj.Duration = timeSpan.ToString(@"hh\:mm");
                            }
                            mateTimeRecordListResponseObj.CreatedOn = projectTimeRecord.MateTimeRecord.CreatedOn != null ? projectTimeRecord.MateTimeRecord.CreatedOn.Value : null;
                        }
                    }
                    mateTimeRecordListResponseList.Add(mateTimeRecordListResponseObj);
                }
            }
            //end project time records

            //start Ticke time records
            var mateTicketTimeRecordList = _mateTicketTimeRecordService.GetByUserId(UserId);
            if (mateTicketTimeRecordList != null && mateTicketTimeRecordList.Count > 0)
            {
                foreach (var ticketTimeRecord in mateTicketTimeRecordList)
                {
                    MateTimeRecordListResponse ticketTimeRecordListResponseObj = new MateTimeRecordListResponse();
                    if (ticketTimeRecord != null)
                    {
                        ticketTimeRecordListResponseObj.ProjectTicketTaskId = ticketTimeRecord.MateTicketId;
                        ticketTimeRecordListResponseObj.Name = ticketTimeRecord.MateTicket?.Name;
                        ticketTimeRecordListResponseObj.Type = "Ticket";
                        if (ticketTimeRecord.MateTimeRecord != null)
                        {
                            ticketTimeRecordListResponseObj.Id = ticketTimeRecord.MateTimeRecord.Id;
                            ticketTimeRecordListResponseObj.Comment = ticketTimeRecord.MateTimeRecord.Comment;
                            ticketTimeRecordListResponseObj.ServiceArticleId = ticketTimeRecord.MateTimeRecord.ServiceArticleId;
                            ticketTimeRecordListResponseObj.IsBillable = ticketTimeRecord.MateTimeRecord.IsBillable;
                            if (ticketTimeRecord.MateTimeRecord.Duration != null)
                            {
                                long ProjectTotalDuration = ticketTimeRecord.MateTimeRecord.Duration.Value;
                                TimeSpan timeSpan = TimeSpan.FromMinutes(ProjectTotalDuration);
                                ticketTimeRecordListResponseObj.Duration = timeSpan.ToString(@"hh\:mm");
                            }
                            ticketTimeRecordListResponseObj.CreatedOn = ticketTimeRecord.MateTimeRecord.CreatedOn != null ? ticketTimeRecord.MateTimeRecord.CreatedOn.Value : null;
                        }
                    }
                    mateTimeRecordListResponseList.Add(ticketTimeRecordListResponseObj);
                }
            }
            //end ticket time records

            //start task time records
            var mateTaskTimeRecordList = _mateTaskTimeRecordService.GetByUserId(UserId);
            if (mateTaskTimeRecordList != null && mateTaskTimeRecordList.Count > 0)
            {
                foreach (var taskTimeRecord in mateTaskTimeRecordList)
                {
                    MateTimeRecordListResponse taskTimeRecordListResponseObj = new MateTimeRecordListResponse();
                    if (taskTimeRecord != null)
                    {
                        taskTimeRecordListResponseObj.ProjectTicketTaskId = taskTimeRecord.TaskId;
                        taskTimeRecordListResponseObj.Name = taskTimeRecord.EmployeeTask?.Name;
                        taskTimeRecordListResponseObj.Type = "Task";
                        if (taskTimeRecord.MateTimeRecord != null)
                        {
                            taskTimeRecordListResponseObj.Id = taskTimeRecord.MateTimeRecord.Id;
                            taskTimeRecordListResponseObj.Comment = taskTimeRecord.MateTimeRecord.Comment;
                            taskTimeRecordListResponseObj.ServiceArticleId = taskTimeRecord.MateTimeRecord.ServiceArticleId;
                            taskTimeRecordListResponseObj.IsBillable = taskTimeRecord.MateTimeRecord.IsBillable;
                            if (taskTimeRecord.MateTimeRecord.Duration != null)
                            {
                                long taskTotalDuration = taskTimeRecord.MateTimeRecord.Duration.Value;
                                TimeSpan timeSpan = TimeSpan.FromMinutes(taskTotalDuration);
                                taskTimeRecordListResponseObj.Duration = timeSpan.ToString(@"hh\:mm");
                            }
                            taskTimeRecordListResponseObj.CreatedOn = taskTimeRecord.MateTimeRecord.CreatedOn != null ? taskTimeRecord.MateTimeRecord.CreatedOn.Value : null;
                        }
                    }
                    mateTimeRecordListResponseList.Add(taskTimeRecordListResponseObj);
                }
            }
            //end task time records

            //commented this section for sub task and chid task not in timer module as of now    
            // //start sub task time records
            // var mateSubTaskTimeRecordList = _mateSubTaskTimeRecordService.GetByUserId(UserId);
            // if (mateSubTaskTimeRecordList != null && mateSubTaskTimeRecordList.Count > 0)
            // {
            //     foreach (var subTaskTimeRecord in mateSubTaskTimeRecordList)
            //     {
            //         MateTimeRecordListResponse subTaskTimeRecordListResponseObj = new MateTimeRecordListResponse();
            //         if (subTaskTimeRecord != null)
            //         {
            //             subTaskTimeRecordListResponseObj.ProjectTicketTaskId = subTaskTimeRecord.SubTaskId;
            //             subTaskTimeRecordListResponseObj.Name = subTaskTimeRecord.EmployeeSubTask?.Description;
            //             subTaskTimeRecordListResponseObj.Type = "SubTask";
            //             if (subTaskTimeRecord.MateTimeRecord != null)
            //             {
            //                 subTaskTimeRecordListResponseObj.Id = subTaskTimeRecord.MateTimeRecord.Id;
            //                 subTaskTimeRecordListResponseObj.Comment = subTaskTimeRecord.MateTimeRecord.Comment;
            //                 subTaskTimeRecordListResponseObj.ServiceArticleId = subTaskTimeRecord.MateTimeRecord.ServiceArticleId;
            //                 subTaskTimeRecordListResponseObj.IsBillable = subTaskTimeRecord.MateTimeRecord.IsBillable;
            //                 if (subTaskTimeRecord.MateTimeRecord.Duration != null)
            //                 {
            //                     long taskTotalDuration = subTaskTimeRecord.MateTimeRecord.Duration.Value;
            //                     TimeSpan timeSpan = TimeSpan.FromMinutes(taskTotalDuration);
            //                     subTaskTimeRecordListResponseObj.Duration = timeSpan.ToString(@"hh\:mm");
            //                 }
            //                 subTaskTimeRecordListResponseObj.CreatedOn = subTaskTimeRecord.MateTimeRecord.CreatedOn != null ? subTaskTimeRecord.MateTimeRecord.CreatedOn.Value : null;
            //             }
            //         }
            //         mateTimeRecordListResponseList.Add(subTaskTimeRecordListResponseObj);
            //     }
            // }
            // //end sub task time records

            // //start child task time records
            // var mateChildTaskTimeRecordList = _mateChildTaskTimeRecordService.GetByUserId(UserId);
            // if (mateChildTaskTimeRecordList != null && mateChildTaskTimeRecordList.Count > 0)
            // {
            //     foreach (var childTaskTimeRecord in mateChildTaskTimeRecordList)
            //     {
            //         MateTimeRecordListResponse childTaskTimeRecordListResponseObj = new MateTimeRecordListResponse();
            //         if (childTaskTimeRecord != null)
            //         {
            //             childTaskTimeRecordListResponseObj.ProjectTicketTaskId = childTaskTimeRecord.ChildTaskId;
            //             childTaskTimeRecordListResponseObj.Name = childTaskTimeRecord.EmployeeChildTask?.Description;
            //             childTaskTimeRecordListResponseObj.Type = "ChildTask";
            //             if (childTaskTimeRecord.MateTimeRecord != null)
            //             {
            //                 childTaskTimeRecordListResponseObj.Id = childTaskTimeRecord.MateTimeRecord.Id;
            //                 childTaskTimeRecordListResponseObj.Comment = childTaskTimeRecord.MateTimeRecord.Comment;
            //                 childTaskTimeRecordListResponseObj.ServiceArticleId = childTaskTimeRecord.MateTimeRecord.ServiceArticleId;
            //                 childTaskTimeRecordListResponseObj.IsBillable = childTaskTimeRecord.MateTimeRecord.IsBillable;
            //                 if (childTaskTimeRecord.MateTimeRecord.Duration != null)
            //                 {
            //                     long taskTotalDuration = childTaskTimeRecord.MateTimeRecord.Duration.Value;
            //                     TimeSpan timeSpan = TimeSpan.FromMinutes(taskTotalDuration);
            //                     childTaskTimeRecordListResponseObj.Duration = timeSpan.ToString(@"hh\:mm");
            //                 }
            //                 childTaskTimeRecordListResponseObj.CreatedOn = childTaskTimeRecord.MateTimeRecord.CreatedOn != null ? childTaskTimeRecord.MateTimeRecord.CreatedOn.Value : null;
            //             }
            //         }
            //         mateTimeRecordListResponseList.Add(childTaskTimeRecordListResponseObj);
            //     }
            // }
            // //end child task time records

            return new OperationResult<List<MateTimeRecordListResponse>>(true, System.Net.HttpStatusCode.OK, "", mateTimeRecordListResponseList);
        }


    }
}