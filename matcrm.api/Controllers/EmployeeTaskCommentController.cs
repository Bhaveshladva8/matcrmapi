using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using matcrm.api.SignalR;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.service.Common;
using matcrm.service.Services;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System;
using matcrm.data.Models.Request;
using AutoMapper;
using matcrm.data.Models.Response;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class EmployeeTaskCommentController : Controller
    {
        private readonly IOneClappTaskService _taskService;
        private readonly IEmployeeTaskCommentService _employeeTaskCommentService;
        private readonly IEmployeeTaskActivityService _EmployeeTaskActivityService;
        private IMapper _mapper;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private int UserId = 0;
        private int TenantId = 0;
        public EmployeeTaskCommentController(
            IOneClappTaskService taskService,
            IEmployeeTaskCommentService employeeTaskCommentService,
            IEmployeeTaskActivityService EmployeeTaskActivityService,
            IMapper mapper,
            IHubContext<BroadcastHub, IHubClient> hubContext
        )
        {
            _taskService = taskService;
            _employeeTaskCommentService = employeeTaskCommentService;
            _EmployeeTaskActivityService = EmployeeTaskActivityService;
            _hubContext = hubContext;
        }

        [Authorize(Roles = "TenantManager,TenantAdmin, TenantUser")]
        [HttpPost]
        public async Task<OperationResult<AddUpdateEmployeeTaskCommentResponse>> Add([FromBody] AddUpdateEmployeeTaskCommentRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            AddUpdateEmployeeTaskCommentResponse response = new AddUpdateEmployeeTaskCommentResponse();
            EmployeeTaskCommentDto employeeTaskCommentDto = _mapper.Map<EmployeeTaskCommentDto>(requestModel);
            employeeTaskCommentDto.UserId = UserId;
            var employeeTaskCommentObj = await _employeeTaskCommentService.CheckInsertOrUpdate(employeeTaskCommentDto);
            if (employeeTaskCommentObj != null)
            {
                EmployeeTaskActivity employeeTaskActivityObj = new EmployeeTaskActivity();
                employeeTaskActivityObj.EmployeeTaskId = employeeTaskCommentDto.EmployeeTaskId;
                employeeTaskActivityObj.UserId = UserId;

                if (requestModel.Id == null)
                {
                    employeeTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_comment_created.ToString().Replace("_", " ");
                }
                else
                {
                    employeeTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_comment_updated.ToString().Replace("_", " ");
                }
                var AddUpdate = await _EmployeeTaskActivityService.CheckInsertOrUpdate(employeeTaskActivityObj);
                await _hubContext.Clients.All.OnEmployeeTaskCommentEventEmit(employeeTaskCommentObj.EmployeeTaskId);
                await _hubContext.Clients.All.OnEmployeeTaskModuleEvent(employeeTaskCommentObj.EmployeeTaskId, TenantId);
                response = _mapper.Map<AddUpdateEmployeeTaskCommentResponse>(employeeTaskActivityObj);
            }

            return new OperationResult<AddUpdateEmployeeTaskCommentResponse>(true, System.Net.HttpStatusCode.OK, "", response);
        }

        [Authorize(Roles = "TenantManager,TenantAdmin, TenantUser")]
        [HttpPut]
        public async Task<OperationResult<AddUpdateEmployeeTaskCommentResponse>> Update([FromBody] AddUpdateEmployeeTaskCommentRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            AddUpdateEmployeeTaskCommentResponse response = new AddUpdateEmployeeTaskCommentResponse();
            EmployeeTaskCommentDto employeeTaskCommentDto = _mapper.Map<EmployeeTaskCommentDto>(requestModel);
            employeeTaskCommentDto.UserId = UserId;
            var employeeTaskCommentObj = await _employeeTaskCommentService.CheckInsertOrUpdate(employeeTaskCommentDto);
            if (employeeTaskCommentObj != null)
            {
                EmployeeTaskActivity employeeTaskActivityObj = new EmployeeTaskActivity();
                employeeTaskActivityObj.EmployeeTaskId = employeeTaskCommentDto.EmployeeTaskId;
                employeeTaskActivityObj.UserId = UserId;

                if (requestModel.Id == null)
                {
                    employeeTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_comment_created.ToString().Replace("_", " ");
                }
                else
                {
                    employeeTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_comment_updated.ToString().Replace("_", " ");
                }
                var AddUpdate = await _EmployeeTaskActivityService.CheckInsertOrUpdate(employeeTaskActivityObj);
                await _hubContext.Clients.All.OnEmployeeTaskCommentEventEmit(employeeTaskCommentObj.EmployeeTaskId);
                await _hubContext.Clients.All.OnEmployeeTaskModuleEvent(employeeTaskCommentObj.EmployeeTaskId, TenantId);
                response = _mapper.Map<AddUpdateEmployeeTaskCommentResponse>(employeeTaskActivityObj);
            }

            return new OperationResult<AddUpdateEmployeeTaskCommentResponse>(true, System.Net.HttpStatusCode.OK, "", response);
        }

        [Authorize(Roles = "TenantManager,TenantAdmin, TenantUser")]
        [HttpDelete("{EmployeeTaskCommentId}")]
        public async Task<OperationResult<EmployeeTaskComment>> Remove(long EmployeeTaskCommentId)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var employeeTaskCommentObj = await _employeeTaskCommentService.DeleteEmployeeTaskComment(EmployeeTaskCommentId);
            if (employeeTaskCommentObj != null)
            {
                EmployeeTaskActivity employeeTaskActivityObj = new EmployeeTaskActivity();
                employeeTaskActivityObj.EmployeeTaskId = employeeTaskCommentObj.EmployeeTaskId;
                employeeTaskActivityObj.UserId = UserId;
                employeeTaskActivityObj.Activity = "Removed the comment";
                var AddUpdate = await _EmployeeTaskActivityService.CheckInsertOrUpdate(employeeTaskActivityObj);
                await _hubContext.Clients.All.OnEmployeeTaskCommentEventEmit(employeeTaskCommentObj.EmployeeTaskId);
                await _hubContext.Clients.All.OnEmployeeTaskModuleEvent(employeeTaskCommentObj.EmployeeTaskId, TenantId);
                return new OperationResult<EmployeeTaskComment>(true, System.Net.HttpStatusCode.OK, "Comment deleted successfully");
            }
            else
            {
                return new OperationResult<EmployeeTaskComment>(false, System.Net.HttpStatusCode.OK, "Something went to wrong");
            }

        }

        [Authorize(Roles = "TenantManager,TenantAdmin, TenantUser")]
        [HttpGet("{EmployeeTaskId}")]
        public async Task<OperationResult<List<AddUpdateEmployeeTaskCommentResponse>>> List(long EmployeeTaskId)
        {
            List<AddUpdateEmployeeTaskCommentResponse> response = new List<AddUpdateEmployeeTaskCommentResponse>();
            List<EmployeeTaskComment> employeeTaskCommentList = _employeeTaskCommentService.GetAllByEmployeeTaskId(EmployeeTaskId);
            if (employeeTaskCommentList != null && employeeTaskCommentList.Count > 0)
            {
                response = _mapper.Map<List<AddUpdateEmployeeTaskCommentResponse>>(employeeTaskCommentList);
            }

            return new OperationResult<List<AddUpdateEmployeeTaskCommentResponse>>(true, System.Net.HttpStatusCode.OK, "", response);
        }

        [Authorize(Roles = "TenantManager,TenantAdmin, TenantUser")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<AddUpdateEmployeeTaskCommentResponse>> Detail(long Id)
        {
            var employeeTaskCommentObj = _employeeTaskCommentService.GetEmployeeTaskCommentById(Id);
            var response = _mapper.Map<AddUpdateEmployeeTaskCommentResponse>(employeeTaskCommentObj);
            return new OperationResult<AddUpdateEmployeeTaskCommentResponse>(true, System.Net.HttpStatusCode.OK, "", response);
        }

    }
}