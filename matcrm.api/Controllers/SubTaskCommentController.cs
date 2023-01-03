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
using matcrm.data.Context;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class SubTaskCommentController : Controller
    {
        private readonly IOneClappSubTaskService _subTaskService;
        private readonly ISubTaskCommentService _subTaskCommentService;
        private readonly ISubTaskActivityService _subTaskActivityService;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private int UserId = 0;

        public SubTaskCommentController(
            IOneClappSubTaskService subTaskService,
            ISubTaskCommentService subTaskCommentService,
            ISubTaskActivityService subTaskActivityService,
            IHubContext<BroadcastHub, IHubClient> hubContext
        )
        {
            _subTaskService = _subTaskService;
            _subTaskCommentService = subTaskCommentService;
            _subTaskActivityService = subTaskActivityService;
            _hubContext = hubContext;
        }

        [Authorize(Roles = "TenantManager,TenantAdmin, TenantUser")]
        [HttpPost]
        public async Task<OperationResult<SubTaskComment>> AddUpdate([FromBody] SubTaskCommentDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            model.UserId = UserId;
            var subTaskCommentObj = _subTaskCommentService.CheckInsertOrUpdate(model);

            // Log in to SubTask Activity table 

            SubTaskActivity subTaskActivityObj = new SubTaskActivity();
            subTaskActivityObj.SubTaskId = model.SubTaskId;
            subTaskActivityObj.UserId = UserId;

            if (model.Id == null)
            {
                subTaskActivityObj.Activity = "Added the comment";
            }
            else
            {
                subTaskActivityObj.Activity = "Updated the comment";
            }
            var AddUpdate = _subTaskActivityService.CheckInsertOrUpdate(subTaskActivityObj);
            await _hubContext.Clients.All.OnSubTaskCommentEventEmit(model.SubTaskId);

            return new OperationResult<SubTaskComment>(true, System.Net.HttpStatusCode.OK, "", subTaskCommentObj);
        }

        [Authorize(Roles = "TenantManager,TenantAdmin, TenantUser")]
        [HttpDelete]
        public async Task<OperationResult<SubTaskComment>> Remove([FromBody] SubTaskCommentDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            if (model.Id != null)
            {
                var taskCommentObj = _subTaskCommentService.DeleteSubTaskComment(model.Id.Value);

                SubTaskActivity subTaskActivityObj = new SubTaskActivity();
                subTaskActivityObj.SubTaskId = model.SubTaskId;
                subTaskActivityObj.UserId = UserId;
                subTaskActivityObj.Activity = "Removed the comment";

                var AddUpdate = _subTaskActivityService.CheckInsertOrUpdate(subTaskActivityObj);

                await _hubContext.Clients.All.OnSubTaskCommentEventEmit(model.SubTaskId);

                return new OperationResult<SubTaskComment>(true, System.Net.HttpStatusCode.OK, "Comment deleted successfully", taskCommentObj);
            }
            else
            {
                return new OperationResult<SubTaskComment>(false, System.Net.HttpStatusCode.OK, "Please provide id");
            }
        }

        [Authorize(Roles = "TenantManager,TenantAdmin, TenantUser")]
        [HttpGet("{SubTaskId}")]
        public async Task<OperationResult<List<SubTaskComment>>> List(long SubTaskId)
        {
            var taskCommentList = _subTaskCommentService.GetAllBySubTaskId(SubTaskId);
            return new OperationResult<List<SubTaskComment>>(true, System.Net.HttpStatusCode.OK, "", taskCommentList);
        }

        [Authorize(Roles = "TenantManager,TenantAdmin, TenantUser")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<SubTaskComment>> Detail(long Id)
        {
            var subTaskCommentObj = _subTaskCommentService.GetSubTaskCommentById(Id);
            return new OperationResult<SubTaskComment>(true, System.Net.HttpStatusCode.OK, "", subTaskCommentObj);
        }

    }
}