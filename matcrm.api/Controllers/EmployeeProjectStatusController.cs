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
using matcrm.data.Context;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class EmployeeProjectStatusController : Controller
    {

        private readonly IEmployeeProjectStatusService _EmployeeProjectStatusService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;
        public EmployeeProjectStatusController(IEmployeeProjectStatusService EmployeeProjectStatusService,
            IMapper mapper)
        {
            _EmployeeProjectStatusService = EmployeeProjectStatusService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<EmployeeProjectStatusDto>>> List()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            List<EmployeeProjectStatusDto> employeeProjectStatusDtoList = new List<EmployeeProjectStatusDto>();
            var employeeProjectStatusList = _EmployeeProjectStatusService.GetStatusByUser(UserId);
            employeeProjectStatusDtoList = _mapper.Map<List<EmployeeProjectStatusDto>>(employeeProjectStatusList);

            return new OperationResult<List<EmployeeProjectStatusDto>>(true, System.Net.HttpStatusCode.OK,"", employeeProjectStatusDtoList);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<matcrm.data.Models.Tables.EmployeeProjectStatus>>> TenantBased()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var employeeProjectStatusList = _EmployeeProjectStatusService.GetStatusByTenant(TenantId);
            return new OperationResult<List<matcrm.data.Models.Tables.EmployeeProjectStatus>>(true, System.Net.HttpStatusCode.OK,"", employeeProjectStatusList);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<EmployeeProjectStatus>> AddUpdate([FromBody] EmployeeProjectStatusDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            model.UserId = UserId;
            model.TenantId = TenantId;
            var employeeProjectStatusObj = _EmployeeProjectStatusService.CheckInsertOrUpdate(model);

            return new OperationResult<EmployeeProjectStatus>(true, System.Net.HttpStatusCode.OK,"", employeeProjectStatusObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete]
        public async Task<OperationResult<matcrm.data.Models.Tables.EmployeeProjectStatus>> Remove([FromBody] EmployeeProjectStatusDto model)
        {
            var employeeProjectStatusObj = _EmployeeProjectStatusService.DeleteEmployeeProjectStatus(model);
            return new OperationResult<matcrm.data.Models.Tables.EmployeeProjectStatus>(true, System.Net.HttpStatusCode.OK,"", employeeProjectStatusObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{StatusId}")]
        public async Task<OperationResult<matcrm.data.Models.Tables.EmployeeProjectStatus>> Detail(int StatusId)
        {
            matcrm.data.Models.Tables.EmployeeProjectStatus employeeProjectStatusObj = new matcrm.data.Models.Tables.EmployeeProjectStatus();
            employeeProjectStatusObj = _EmployeeProjectStatusService.GetEmployeeProjectStatusById(StatusId);
            return new OperationResult<matcrm.data.Models.Tables.EmployeeProjectStatus>(true, System.Net.HttpStatusCode.OK,"", employeeProjectStatusObj);
        }
    }
}