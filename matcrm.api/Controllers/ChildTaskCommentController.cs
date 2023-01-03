using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using matcrm.api.SignalR;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.service.Common;
using matcrm.service.Services;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ChildTaskCommentController : Controller
    {
        private readonly IOneClappChildTaskService _childTaskService;
        private readonly IChildTaskCommentService _childTaskCommentService;
        private readonly IChildTaskActivityService _childTaskActivityService;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private int UserId = 0;

        public ChildTaskCommentController(
            IOneClappChildTaskService childTaskService,
            IChildTaskCommentService childTaskCommentService,
            IChildTaskActivityService childTaskActivityService,
            IHubContext<BroadcastHub, IHubClient> hubContext
        )
        {
            _childTaskService = childTaskService;
            _childTaskCommentService = childTaskCommentService;
            _childTaskActivityService = childTaskActivityService;
            _hubContext = hubContext;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<ChildTaskComment>> Add([FromBody] ChildTaskCommentDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            model.UserId = UserId;
            var taskCommentObj = _childTaskCommentService.CheckInsertOrUpdate(model);

            // Log in to ChildTask Activity table 

            ChildTaskActivity childTaskActivityObj = new ChildTaskActivity();
            childTaskActivityObj.ChildTaskId = model.ChildTaskId;
            childTaskActivityObj.UserId = UserId;

            if (model.Id == null)
            {
                childTaskActivityObj.Activity = "Added the comment.";
            }
            else
            {
                childTaskActivityObj.Activity = "Updated the comment.";
            }
            var AddUpdate = _childTaskActivityService.CheckInsertOrUpdate(childTaskActivityObj);
            await _hubContext.Clients.All.OnChildTaskCommentEventEmit(model.ChildTaskId);
            return new OperationResult<ChildTaskComment>(true, System.Net.HttpStatusCode.OK, "", taskCommentObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<ChildTaskComment>> Update([FromBody] ChildTaskCommentDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            model.UserId = UserId;
            var taskCommentObj = _childTaskCommentService.CheckInsertOrUpdate(model);

            // Log in to ChildTask Activity table 

            ChildTaskActivity childTaskActivityObj = new ChildTaskActivity();
            childTaskActivityObj.ChildTaskId = model.ChildTaskId;
            childTaskActivityObj.UserId = UserId;

            if (model.Id == null)
            {
                childTaskActivityObj.Activity = "Added the comment.";
            }
            else
            {
                childTaskActivityObj.Activity = "Updated the comment.";
            }
            var AddUpdate = _childTaskActivityService.CheckInsertOrUpdate(childTaskActivityObj);
            await _hubContext.Clients.All.OnChildTaskCommentEventEmit(model.ChildTaskId);
            return new OperationResult<ChildTaskComment>(true, System.Net.HttpStatusCode.OK, "", taskCommentObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete]
        public async Task<OperationResult<ChildTaskComment>> Remove([FromBody] ChildTaskCommentDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            ChildTaskComment childTaskCommentObj = new ChildTaskComment();
            if (model.Id != null)
            {
                childTaskCommentObj = _childTaskCommentService.DeleteChildTaskComment(model.Id.Value);

                ChildTaskActivity childTaskActivityObj = new ChildTaskActivity();
                childTaskActivityObj.ChildTaskId = model.ChildTaskId;
                childTaskActivityObj.UserId = UserId;
                childTaskActivityObj.Activity = "Removed the comment";

                var AddUpdate = _childTaskActivityService.CheckInsertOrUpdate(childTaskActivityObj);
                await _hubContext.Clients.All.OnChildTaskCommentEventEmit(model.ChildTaskId);

                return new OperationResult<ChildTaskComment>(true, System.Net.HttpStatusCode.OK, "Comment deleted successfully", childTaskCommentObj);
            }
            else
            {
                return new OperationResult<ChildTaskComment>(true, System.Net.HttpStatusCode.OK, "Please provide comment id", childTaskCommentObj);
            }
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{ChildTaskId}")]
        public async Task<OperationResult<List<ChildTaskComment>>> List(long ChildTaskId)
        {
            var childTaskCommentList = _childTaskCommentService.GetAllByChildTaskId(ChildTaskId);
            return new OperationResult<List<ChildTaskComment>>(true, System.Net.HttpStatusCode.OK, "", childTaskCommentList);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<ChildTaskComment>> Detail(long Id)
        {
            var childTaskCommentObj = _childTaskCommentService.GetChildTaskCommentById(Id);
            return new OperationResult<ChildTaskComment>(true, System.Net.HttpStatusCode.OK, "", childTaskCommentObj);
        }

    }
}