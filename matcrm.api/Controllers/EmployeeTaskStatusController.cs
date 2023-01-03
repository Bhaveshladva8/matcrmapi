using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.service.BusinessLogic;
using matcrm.service.Common;
using matcrm.service.Services;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class EmployeeTaskStatusController : Controller
    {

        private readonly IEmployeeTaskStatusService _employeeTaskStatusService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;
        public EmployeeTaskStatusController(IEmployeeTaskStatusService employeeTaskStatusService,
            IMapper mapper)
        {
            _employeeTaskStatusService = employeeTaskStatusService;
            _mapper = mapper;
        }

        // Get list based on loggedin user
        // [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        // [HttpGet]
        // public async Task<OperationResult<List<EmployeeTaskStatusDto>>> List()
        // {
        //     ClaimsPrincipal user = this.User as ClaimsPrincipal;
        //     UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
        //     TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
        //     List<EmployeeTaskStatusDto> employeeTaskStatusDtoList = new List<EmployeeTaskStatusDto>();
        //     var employeeTaskStatusList = _employeeTaskStatusService.GetStatusByUser(UserId);
        //     employeeTaskStatusDtoList = _mapper.Map<List<EmployeeTaskStatusDto>>(employeeTaskStatusList);

        //     return new OperationResult<List<EmployeeTaskStatusDto>>(true, System.Net.HttpStatusCode.OK, "", employeeTaskStatusDtoList);
        // }

        // // get list based on loggedin tenant
        // [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        // [HttpGet]
        // public async Task<OperationResult<List<matcrm.data.Models.Tables.EmployeeProjectStatus>>> TenantBased()
        // {
        //     ClaimsPrincipal user = this.User as ClaimsPrincipal;
        //     TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
        //     List<matcrm.data.Models.Tables.EmployeeTaskStatus> employeeTaskStatusList = _employeeTaskStatusService.GetStatusByTenant(TenantId);
        //     return new OperationResult<List<matcrm.data.Models.Tables.EmployeeProjectStatus>>(true, System.Net.HttpStatusCode.OK, "", employeeTaskStatusList);
        // }

        // [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        // [HttpPost]
        // public async Task<OperationResult<matcrm.data.Models.Tables.EmployeeProjectStatus>> AddUpdate([FromBody] EmployeeTaskStatusDto model)
        // {
        //     ClaimsPrincipal user = this.User as ClaimsPrincipal;
        //     UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
        //     TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
        //     model.UserId = UserId;
        //     model.TenantId = TenantId;
        //     matcrm.data.Models.Tables.EmployeeTaskStatus employeeTaskStatusObj = _employeeTaskStatusService.CheckInsertOrUpdate(model);

        //     return new OperationResult<matcrm.data.Models.Tables.EmployeeProjectStatus>(true, System.Net.HttpStatusCode.OK, "", employeeTaskStatusObj);
        // }

        // [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        // [HttpDelete]
        // public async Task<OperationResult<matcrm.data.Models.Tables.EmployeeProjectStatus>> Remove([FromBody] EmployeeTaskStatusDto model)
        // {
        //     matcrm.data.Models.Tables.EmployeeTaskStatus employeeTaskStatusObj = _employeeTaskStatusService.DeleteEmployeeTaskStatus(model);
        //     return new OperationResult<matcrm.data.Models.Tables.EmployeeProjectStatus>(true, System.Net.HttpStatusCode.OK, "", employeeTaskStatusObj);
        // }

        // [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        // [HttpGet("{StatusId}")]
        // public async Task<OperationResult<matcrm.data.Models.Tables.EmployeeProjectStatus>> Detail(int StatusId)
        // {
        //     matcrm.data.Models.Tables.EmployeeTaskStatus employeeTaskStatusObj = new matcrm.data.Models.Tables.EmployeeTaskStatus();
        //     employeeTaskStatusObj = _employeeTaskStatusService.GetEmployeeTaskStatusById(StatusId);
        //     return new OperationResult<matcrm.data.Models.Tables.EmployeeProjectStatus>(true, System.Net.HttpStatusCode.OK, "", employeeTaskStatusObj);
        // }
    }
}