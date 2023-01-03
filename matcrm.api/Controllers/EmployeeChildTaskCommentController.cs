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
using matcrm.data.Models.Response;
using matcrm.data.Models.Tables;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.data.Context;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class EmployeeChildTaskCommentController : Controller
    {
        private readonly IEmployeeChildTaskService _employeeChildTaskService;
        private readonly IEmployeeChildTaskCommentService _employeeChildTaskCommentService;
        private readonly IEmployeeChildTaskActivityService _employeeChildTaskActivityService;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private int UserId = 0;
        private IMapper _mapper;

        public EmployeeChildTaskCommentController(
            IEmployeeChildTaskService employeeChildTaskService,
            IEmployeeChildTaskCommentService employeeChildTaskCommentService,
            IEmployeeChildTaskActivityService employeeChildTaskActivityService,
            IMapper mapper,
            IHubContext<BroadcastHub, IHubClient> hubContext
        )
        {
            _employeeChildTaskService = employeeChildTaskService;
            _employeeChildTaskCommentService = employeeChildTaskCommentService;
            _employeeChildTaskActivityService = employeeChildTaskActivityService;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<AddUpdateEmployeeChildTaskCommentResponse>> Add([FromBody] AddUpdateEmployeeChildTaskCommentRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            EmployeeChildTaskCommentDto employeeChildTaskCommentDto = _mapper.Map<EmployeeChildTaskCommentDto>(requestModel);
            employeeChildTaskCommentDto.Name = requestModel.Comment;
            employeeChildTaskCommentDto.UserId = UserId;
            var employeeChildTaskCommentObj = await _employeeChildTaskCommentService.CheckInsertOrUpdate(employeeChildTaskCommentDto);

            // Log in to ChildTask Activity table 

            EmployeeChildTaskActivity employeeChildTaskActivityObj = new EmployeeChildTaskActivity();
            employeeChildTaskActivityObj.EmployeeChildTaskId = employeeChildTaskCommentObj.EmployeeChildTaskId;
            employeeChildTaskActivityObj.UserId = UserId;

            if (employeeChildTaskCommentDto.Id == null)
            {
                employeeChildTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_comment_created.ToString().Replace("_", " ");
            }
            else
            {
                employeeChildTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_comment_updated.ToString().Replace("_", " ");
            }
            var AddUpdate = await _employeeChildTaskActivityService.CheckInsertOrUpdate(employeeChildTaskActivityObj);
            await _hubContext.Clients.All.OnChildTaskCommentEventEmit(employeeChildTaskCommentObj.EmployeeChildTaskId);
            var response = _mapper.Map<AddUpdateEmployeeChildTaskCommentResponse>(employeeChildTaskCommentObj);
            // response.Comment = employeeChildTaskCommentObj.Name;
            return new OperationResult<AddUpdateEmployeeChildTaskCommentResponse>(true, System.Net.HttpStatusCode.OK, "", response);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<AddUpdateEmployeeChildTaskCommentResponse>> Update([FromBody] AddUpdateEmployeeChildTaskCommentRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            EmployeeChildTaskCommentDto employeeChildTaskCommentDto = _mapper.Map<EmployeeChildTaskCommentDto>(requestModel);
            employeeChildTaskCommentDto.Name = requestModel.Comment;
            employeeChildTaskCommentDto.UserId = UserId;
            var employeeChildTaskCommentObj = await _employeeChildTaskCommentService.CheckInsertOrUpdate(employeeChildTaskCommentDto);

            // Log in to ChildTask Activity table 

            EmployeeChildTaskActivity employeeChildTaskActivityObj = new EmployeeChildTaskActivity();
            employeeChildTaskActivityObj.EmployeeChildTaskId = employeeChildTaskCommentObj.EmployeeChildTaskId;
            employeeChildTaskActivityObj.UserId = UserId;

            if (employeeChildTaskCommentDto.Id == null)
            {
                employeeChildTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_comment_created.ToString().Replace("_", " ");
            }
            else
            {
                employeeChildTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_comment_updated.ToString().Replace("_", " ");
            }
            var AddUpdate = await _employeeChildTaskActivityService.CheckInsertOrUpdate(employeeChildTaskActivityObj);
            await _hubContext.Clients.All.OnChildTaskCommentEventEmit(employeeChildTaskCommentObj.EmployeeChildTaskId);
            var response = _mapper.Map<AddUpdateEmployeeChildTaskCommentResponse>(employeeChildTaskCommentObj);
            // response.Comment = employeeChildTaskCommentObj.Name;
            return new OperationResult<AddUpdateEmployeeChildTaskCommentResponse>(true, System.Net.HttpStatusCode.OK, "", response);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete("{CommentId}")]
        public async Task<OperationResult<EmployeeChildTaskComment>> Remove(long CommentId)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            
                var employeeChildTaskCommentObj = await _employeeChildTaskCommentService.DeleteEmployeeChildTaskComment(CommentId);

                EmployeeChildTaskActivity employeeChildTaskActivityObj = new EmployeeChildTaskActivity();
                employeeChildTaskActivityObj.EmployeeChildTaskId = employeeChildTaskCommentObj.EmployeeChildTaskId;
                employeeChildTaskActivityObj.UserId = UserId;
                employeeChildTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_comment_removed.ToString().Replace("_", " ");

                var AddUpdate = await _employeeChildTaskActivityService.CheckInsertOrUpdate(employeeChildTaskActivityObj);
                await _hubContext.Clients.All.OnEmployeeChildTaskCommentEventEmit(employeeChildTaskCommentObj.EmployeeChildTaskId);

                return new OperationResult<EmployeeChildTaskComment>(true, System.Net.HttpStatusCode.OK, "Comment deleted successfully");
            
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{EmployeeChildTaskId}")]
        public async Task<OperationResult<List<EmployeeChildTaskCommentListResponse>>> List(long EmployeeChildTaskId)
        {
            List<EmployeeChildTaskCommentListResponse> response = new List<EmployeeChildTaskCommentListResponse>();
            List<EmployeeChildTaskComment> employeeChildTaskCommentList = _employeeChildTaskCommentService.GetAllByChildTaskId(EmployeeChildTaskId);
             if (employeeChildTaskCommentList != null && employeeChildTaskCommentList.Count > 0)
            {
             response = _mapper.Map<List<EmployeeChildTaskCommentListResponse>>(employeeChildTaskCommentList);
            }
            return new OperationResult<List<EmployeeChildTaskCommentListResponse>>(true, System.Net.HttpStatusCode.OK, "", response);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<EmployeeChildTaskCommentListResponse>> Detail(long Id)
        {
            var employeeChildTaskCommentObj = _employeeChildTaskCommentService.GetEmployeeChildTaskCommentById(Id);
            var response = _mapper.Map<EmployeeChildTaskCommentListResponse>(employeeChildTaskCommentObj);
            return new OperationResult<EmployeeChildTaskCommentListResponse>(true, System.Net.HttpStatusCode.OK, "", response);
        }

    }
}