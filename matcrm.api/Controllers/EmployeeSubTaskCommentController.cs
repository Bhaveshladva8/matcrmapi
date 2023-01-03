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
    public class EmployeeSubTaskCommentController : Controller
    {
        private readonly IEmployeeSubTaskService _employeeSubTaskService;
        private readonly IEmployeeSubTaskCommentService _employeeSubTaskCommentService;
        private readonly IEmployeeSubTaskActivityService _employeeSubTaskActivityService;
        private IMapper _mapper;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private int UserId = 0;

        public EmployeeSubTaskCommentController(
            IEmployeeSubTaskService employeeSubTaskService,
            IEmployeeSubTaskCommentService employeeSubTaskCommentService,
            IEmployeeSubTaskActivityService employeeSubTaskActivityService,
            IMapper mapper,
            IHubContext<BroadcastHub, IHubClient> hubContext
        )
        {
            _employeeSubTaskService = employeeSubTaskService;
            _employeeSubTaskCommentService = employeeSubTaskCommentService;
            _employeeSubTaskActivityService = employeeSubTaskActivityService;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<AddUpdateEmployeeSubTaskCommentResponse>> Add([FromBody] AddUpdateEmployeeSubTaskCommentRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            EmployeeSubTaskCommentDto employeeSubTaskCommentDto = _mapper.Map<EmployeeSubTaskCommentDto>(requestModel);
            employeeSubTaskCommentDto.UserId = UserId;
            var employeeSubTaskCommentObj = await _employeeSubTaskCommentService.CheckInsertOrUpdate(employeeSubTaskCommentDto);

            // Log in to SubTask Activity table 

            EmployeeSubTaskActivity employeeSubTaskActivityObj = new EmployeeSubTaskActivity();
            employeeSubTaskActivityObj.EmployeeSubTaskId = employeeSubTaskCommentObj.EmployeeSubTaskId;
            employeeSubTaskActivityObj.UserId = UserId;

            if (requestModel.Id == null)
            {
                employeeSubTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_comment_created.ToString().Replace("_", " ");
            }
            else
            {
                employeeSubTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_comment_updated.ToString().Replace("_", " ");
            }
            var AddUpdate = await _employeeSubTaskActivityService.CheckInsertOrUpdate(employeeSubTaskActivityObj);
            await _hubContext.Clients.All.OnEmployeeSubTaskCommentEventEmit(employeeSubTaskCommentObj.EmployeeSubTaskId);

            var response = _mapper.Map<AddUpdateEmployeeSubTaskCommentResponse>(employeeSubTaskCommentObj);

            return new OperationResult<AddUpdateEmployeeSubTaskCommentResponse>(true, System.Net.HttpStatusCode.OK, "", response);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<AddUpdateEmployeeSubTaskCommentResponse>> Update([FromBody] AddUpdateEmployeeSubTaskCommentRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            EmployeeSubTaskCommentDto employeeSubTaskCommentDto = _mapper.Map<EmployeeSubTaskCommentDto>(requestModel);
            employeeSubTaskCommentDto.UserId = UserId;
            var employeeSubTaskCommentObj = await _employeeSubTaskCommentService.CheckInsertOrUpdate(employeeSubTaskCommentDto);

            // Log in to SubTask Activity table 

            EmployeeSubTaskActivity employeeSubTaskActivityObj = new EmployeeSubTaskActivity();
            employeeSubTaskActivityObj.EmployeeSubTaskId = employeeSubTaskCommentObj.EmployeeSubTaskId;
            employeeSubTaskActivityObj.UserId = UserId;

            if (requestModel.Id == null)
            {
                employeeSubTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_comment_created.ToString().Replace("_", " ");
            }
            else
            {
                employeeSubTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_comment_updated.ToString().Replace("_", " ");
            }
            var AddUpdate = await _employeeSubTaskActivityService.CheckInsertOrUpdate(employeeSubTaskActivityObj);
            await _hubContext.Clients.All.OnEmployeeSubTaskCommentEventEmit(employeeSubTaskCommentObj.EmployeeSubTaskId);

            var response = _mapper.Map<AddUpdateEmployeeSubTaskCommentResponse>(employeeSubTaskCommentObj);

            return new OperationResult<AddUpdateEmployeeSubTaskCommentResponse>(true, System.Net.HttpStatusCode.OK, "", response);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete("{CommentId}")]
        public async Task<OperationResult<EmployeeSubTaskComment>> Remove(long CommentId)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            
                var employeeSubTaskCommentObj = await _employeeSubTaskCommentService.DeleteEmployeeSubTaskComment(CommentId);

                EmployeeSubTaskActivity employeeSubTaskActivityObj = new EmployeeSubTaskActivity();
                employeeSubTaskActivityObj.EmployeeSubTaskId = employeeSubTaskCommentObj.EmployeeSubTaskId;
                employeeSubTaskActivityObj.UserId = UserId;
                employeeSubTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_comment_removed.ToString().Replace("_", " ");

                var AddUpdate = await _employeeSubTaskActivityService.CheckInsertOrUpdate(employeeSubTaskActivityObj);

                await _hubContext.Clients.All.OnEmployeeSubTaskCommentEventEmit(employeeSubTaskCommentObj.EmployeeSubTaskId);

                return new OperationResult<EmployeeSubTaskComment>(true, System.Net.HttpStatusCode.OK, "Comment deleted successfully");
            
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{EmployeeSubTaskId}")]
        public async Task<OperationResult<List<AddUpdateEmployeeSubTaskCommentResponse>>> List(long EmployeeSubTaskId)
        {
            List<AddUpdateEmployeeSubTaskCommentResponse> response = new List<AddUpdateEmployeeSubTaskCommentResponse>();
            List<EmployeeSubTaskComment> employeeSubTaskCommentList = _employeeSubTaskCommentService.GetAllByEmployeeSubTaskId(EmployeeSubTaskId);
            if (employeeSubTaskCommentList != null && employeeSubTaskCommentList.Count > 0)
            {
                response = _mapper.Map<List<AddUpdateEmployeeSubTaskCommentResponse>>(employeeSubTaskCommentList);
            }
            return new OperationResult<List<AddUpdateEmployeeSubTaskCommentResponse>>(true, System.Net.HttpStatusCode.OK, "", response);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<AddUpdateEmployeeSubTaskCommentResponse>> Detail(long Id)
        {
            var employeeSubTaskCommentObj = _employeeSubTaskCommentService.GetEmployeeSubTaskCommentById(Id);
            var response = _mapper.Map<AddUpdateEmployeeSubTaskCommentResponse>(employeeSubTaskCommentObj);
            return new OperationResult<AddUpdateEmployeeSubTaskCommentResponse>(true, System.Net.HttpStatusCode.OK, "", response);
        }

    }
}