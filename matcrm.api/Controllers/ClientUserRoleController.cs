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

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ClientUserRoleController : Controller
    {
        private readonly IClientUserRoleService _clientUserRoleService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;

        public ClientUserRoleController(IClientUserRoleService clientUserRoleService,
        IMapper mapper)
        {
            _clientUserRoleService = clientUserRoleService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<OperationResult<ClientUserRoleAddUpdateResponse>> Add([FromBody] ClientUserRoleAddUpdateRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var model = _mapper.Map<ClientUserRole>(requestmodel);
            if (model.Id == 0)
            {
                model.CreatedBy = UserId;
            }
            var clientUserRoleObj = await _clientUserRoleService.CheckInsertOrUpdate(model);
            var responseObj = _mapper.Map<ClientUserRoleAddUpdateResponse>(clientUserRoleObj);
            return new OperationResult<ClientUserRoleAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Added successfully.", responseObj);
        }

        [HttpPut]
        public async Task<OperationResult<ClientUserRoleAddUpdateResponse>> Update([FromBody] ClientUserRoleAddUpdateRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var model = _mapper.Map<ClientUserRole>(requestmodel);
            if (model.Id > 0)
            {
                model.UpdatedBy = UserId;
            }
            var clientUserRoleObj = await _clientUserRoleService.CheckInsertOrUpdate(model);
            var responseObj = _mapper.Map<ClientUserRoleAddUpdateResponse>(clientUserRoleObj);
            return new OperationResult<ClientUserRoleAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Updated successfully.", responseObj);
        }

        [HttpDelete("{Id}")]
        public async Task<OperationResult> Remove(int Id)
        {
            if (Id > 0)
            {
                var clientUserRoleObj = await _clientUserRoleService.DeleteById(Id);
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "Deleted successfully", Id);
            }
            else
            {
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "Please provide Id", Id);
            }
        }

        [HttpGet]
        public async Task<OperationResult<List<ClientUserRoleListResponse>>> List()
        {
            var clientUserRoleList = _clientUserRoleService.GetAll();
            var clientUserRoleListResponses = _mapper.Map<List<ClientUserRoleListResponse>>(clientUserRoleList);
            return new OperationResult<List<ClientUserRoleListResponse>>(true, System.Net.HttpStatusCode.OK, "", clientUserRoleListResponses);
        }
        [HttpGet("{Id}")]
        public async Task<OperationResult<ClientUserRoleDetailResponse>> Detail(long Id)
        {
            var clientUserRoleObj = _clientUserRoleService.GetById(Id);
            var ResponseObj = _mapper.Map<ClientUserRoleDetailResponse>(clientUserRoleObj);
            return new OperationResult<ClientUserRoleDetailResponse>(true, System.Net.HttpStatusCode.OK, "", ResponseObj);
        }

        [HttpGet]
        public async Task<OperationResult<List<ClientUserRoleDropDownListResponse>>> DropDownList()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var clientUserRoleList = _clientUserRoleService.GetByTenant(TenantId);
            var clientUserRoleListResponses = _mapper.Map<List<ClientUserRoleDropDownListResponse>>(clientUserRoleList);
            return new OperationResult<List<ClientUserRoleDropDownListResponse>>(true, System.Net.HttpStatusCode.OK, "", clientUserRoleListResponses);
        }

    }
}