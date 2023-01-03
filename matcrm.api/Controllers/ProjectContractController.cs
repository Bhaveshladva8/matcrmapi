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

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class ProjectContractController : Controller
    {
        private readonly IProjectContractService _projectContractService;
        private readonly IEmployeeProjectActivityService _employeeProjectActivityService;
        private readonly IContractActivityService _contractActivityService;
        private readonly IContractService _contractService;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;
        public ProjectContractController(IMapper mapper,
        IProjectContractService projectContractService,
        IEmployeeProjectActivityService employeeProjectActivityService,
        IContractActivityService contractActivityService,
        IHubContext<BroadcastHub, IHubClient> hubContext,
        IContractService contractService)
        {
            _mapper = mapper;
            _projectContractService = projectContractService;
            _employeeProjectActivityService = employeeProjectActivityService;
            _contractActivityService = contractActivityService;
            _contractService = contractService;
            _hubContext = hubContext;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<ProjectContractAddUpdateResponse>> Add([FromBody] ProjectContractAddUpdateRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var model = _mapper.Map<ProjectContract>(requestmodel);
            var projectContractObj = await _projectContractService.CheckInsertOrUpdate(model);
            if (projectContractObj != null)
            {
                //project activity
                EmployeeProjectActivity employeeProjectActivityObj = new EmployeeProjectActivity();
                employeeProjectActivityObj.ProjectId = projectContractObj.ProjectId;
                employeeProjectActivityObj.UserId = UserId;
                employeeProjectActivityObj.Activity = Enums.ProjectContractActivityEnum.Project_added_into_contract.ToString().Replace("_", " ");
                var projectActivityObj = await _employeeProjectActivityService.CheckInsertOrUpdate(employeeProjectActivityObj);
                await _hubContext.Clients.All.OnProjectModuleEvent(projectContractObj.ProjectId, TenantId);
                //contract activity
                if (projectContractObj.ContractId != null)
                {
                    var contractObj = _contractService.GetById(projectContractObj.ContractId.Value);
                    ContractActivity contractActivityObj = new ContractActivity();
                    contractActivityObj.ContractId = projectContractObj.ContractId;
                    contractActivityObj.ClientId = contractObj.ClientId;
                    contractActivityObj.Activity = Enums.ProjectContractActivityEnum.Project_added_into_contract.ToString().Replace("_", " ");
                    var contractActivity = await _contractActivityService.CheckInsertOrUpdate(contractActivityObj);
                }
            }

            var ProjectContractAddUpdateResponseObj = _mapper.Map<ProjectContractAddUpdateResponse>(projectContractObj);
            return new OperationResult<ProjectContractAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Project contract added successfully", ProjectContractAddUpdateResponseObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<ProjectContractAddUpdateResponse>> Update([FromBody] ProjectContractAddUpdateRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var model = _mapper.Map<ProjectContract>(requestmodel);
            var projectContractObj = await _projectContractService.CheckInsertOrUpdate(model);
            if (projectContractObj != null)
            {
                //project activity
                EmployeeProjectActivity employeeProjectActivityObj = new EmployeeProjectActivity();
                employeeProjectActivityObj.ProjectId = projectContractObj.ProjectId;
                employeeProjectActivityObj.UserId = UserId;
                employeeProjectActivityObj.Activity = Enums.ProjectContractActivityEnum.Project_added_into_contract.ToString().Replace("_", " ");
                var projectActivityObj = await _employeeProjectActivityService.CheckInsertOrUpdate(employeeProjectActivityObj);
                await _hubContext.Clients.All.OnProjectModuleEvent(projectContractObj.ProjectId, TenantId);
                //contract activity
                if (projectContractObj.ContractId != null)
                {
                    var contractObj = _contractService.GetById(projectContractObj.ContractId.Value);
                    ContractActivity contractActivityObj = new ContractActivity();
                    contractActivityObj.ContractId = projectContractObj.ContractId;
                    contractActivityObj.ClientId = contractObj.ClientId;
                    contractActivityObj.Activity = Enums.ProjectContractActivityEnum.Project_added_into_contract.ToString().Replace("_", " ");
                    var contractActivity = await _contractActivityService.CheckInsertOrUpdate(contractActivityObj);
                }
            }

            var ProjectContractAddUpdateResponseObj = _mapper.Map<ProjectContractAddUpdateResponse>(projectContractObj);
            return new OperationResult<ProjectContractAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Project contract updated successfully", ProjectContractAddUpdateResponseObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete("{Id}")]
        public async Task<OperationResult> Remove(long Id)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            if (Id > 0)
            {
                var projectContractObj = await _projectContractService.Delete(Id);

                if (projectContractObj != null)
                {
                    //project activity
                    EmployeeProjectActivity employeeProjectActivityObj = new EmployeeProjectActivity();
                    employeeProjectActivityObj.ProjectId = projectContractObj.ProjectId;
                    employeeProjectActivityObj.UserId = UserId;
                    employeeProjectActivityObj.Activity = Enums.ProjectContractActivityEnum.Project_removed_from_contract.ToString().Replace("_", " ");
                    var projectActivityObj = await _employeeProjectActivityService.CheckInsertOrUpdate(employeeProjectActivityObj);
                    await _hubContext.Clients.All.OnProjectModuleEvent(projectContractObj.ProjectId, TenantId);
                    //contract activity
                    if (projectContractObj.ContractId != null)
                    {
                        var contractObj = _contractService.GetById(projectContractObj.ContractId.Value);
                        ContractActivity contractActivityObj = new ContractActivity();
                        contractActivityObj.ContractId = projectContractObj.ContractId;
                        contractActivityObj.ClientId = contractObj.ClientId;
                        contractActivityObj.Activity = Enums.ProjectContractActivityEnum.Project_removed_from_contract.ToString().Replace("_", " ");
                        var contractActivity = await _contractActivityService.CheckInsertOrUpdate(contractActivityObj);
                    }
                }

                return new OperationResult(true, System.Net.HttpStatusCode.OK, "", Id);
            }
            else
            {
                return new OperationResult(false, System.Net.HttpStatusCode.OK, "Please provide id", Id);
            }
        }
    }
}