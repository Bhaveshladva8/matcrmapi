using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using matcrm.api.SignalR;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Request;
using matcrm.data.Models.Tables;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.data.Context;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class TaskCommentController : Controller
    {
        private readonly IOneClappTaskService _taskService;
        private readonly ITaskCommentService _taskCommentService;
        private readonly ITaskActivityService _taskActivityService;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private IMapper _mapper;
        private int UserId = 0;

        public TaskCommentController(
            IOneClappTaskService taskService,
            ITaskCommentService taskCommentService,
            ITaskActivityService taskActivityService,
            IMapper mapper,
            IHubContext<BroadcastHub, IHubClient> hubContext
        )
        {
            _taskService = taskService;
            _taskCommentService = taskCommentService;
            _taskActivityService = taskActivityService;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<TaskComment>> Add([FromBody] AddUpdateTaskCommentRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TaskCommentDto taskCommentDto = _mapper.Map<TaskCommentDto>(requestModel);
            taskCommentDto.UserId = UserId;
            var taskCommentObj = _taskCommentService.CheckInsertOrUpdate(taskCommentDto);
            // Log in to SubTask Activity table 

            TaskActivity taskActivityObj = new TaskActivity();
            taskActivityObj.TaskId = taskCommentDto.TaskId;
            taskActivityObj.UserId = UserId;

            if (taskCommentDto.Id == null)
            {
                taskActivityObj.Activity = "Created the comment";
            }
            else
            {
                taskActivityObj.Activity = "Updated the comment";
            }
            var AddUpdate = _taskActivityService.CheckInsertOrUpdate(taskActivityObj);
            await _hubContext.Clients.All.OnTaskCommentEventEmit(taskCommentDto.TaskId);
            return new OperationResult<TaskComment>(true, System.Net.HttpStatusCode.OK, "", taskCommentObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<TaskComment>> Update([FromBody] AddUpdateTaskCommentRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TaskCommentDto taskCommentDto = _mapper.Map<TaskCommentDto>(requestModel);
            taskCommentDto.UserId = UserId;
            var taskCommentObj = _taskCommentService.CheckInsertOrUpdate(taskCommentDto);
            // Log in to SubTask Activity table 

            TaskActivity taskActivityObj = new TaskActivity();
            taskActivityObj.TaskId = taskCommentDto.TaskId;
            taskActivityObj.UserId = UserId;

            if (taskCommentDto.Id == null)
            {
                // taskActivityObj.Activity = "Created the comment";
                taskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_comment_created.ToString().Replace("_", " ");
            }
            else
            {
                // taskActivityObj.Activity = "Updated the comment";
                taskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_comment_updated.ToString().Replace("_", " ");
            }
            var AddUpdate = _taskActivityService.CheckInsertOrUpdate(taskActivityObj);
            await _hubContext.Clients.All.OnTaskCommentEventEmit(taskCommentDto.TaskId);
            return new OperationResult<TaskComment>(true, System.Net.HttpStatusCode.OK, "", taskCommentObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete("{CommentId}")]
        public async Task<OperationResult<TaskComment>> Remove(long CommentId)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            var taskCommentObj = await _taskCommentService.DeleteTaskComment(CommentId);
            if (taskCommentObj != null)
            {
                TaskActivity taskActivityObj = new TaskActivity();
                taskActivityObj.TaskId = taskCommentObj.TaskId;
                taskActivityObj.UserId = UserId;
                taskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_comment_removed.ToString().Replace("_", " ");
                var AddUpdate = _taskActivityService.CheckInsertOrUpdate(taskActivityObj);
                await _hubContext.Clients.All.OnTaskCommentEventEmit(taskCommentObj.TaskId);

                return new OperationResult<TaskComment>(true, System.Net.HttpStatusCode.OK, "Comment deleted successfully", taskCommentObj);
            }
            else
            {
                return new OperationResult<TaskComment>(false, System.Net.HttpStatusCode.OK, "Something went to wrong", taskCommentObj);
            }

        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{TaskId}")]
        public async Task<OperationResult<List<TaskComment>>> BasedOnTask(long TaskId)
        {
            var taskCommentList = _taskCommentService.GetAllByTaskId(TaskId);
            return new OperationResult<List<TaskComment>>(true, System.Net.HttpStatusCode.OK, "", taskCommentList);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<TaskComment>> Detail(long Id)
        {
            var taskCommentObj = _taskCommentService.GetTaskCommenttById(Id);
            return new OperationResult<TaskComment>(true, System.Net.HttpStatusCode.OK, "", taskCommentObj);
        }

    }
}