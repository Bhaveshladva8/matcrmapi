using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.data.Context;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;
        private IMapper _mapper;
        private int TenantId = 0;

        public RoleController(IRoleService roleService,
            IMapper mapper)
        {
            _roleService = roleService;
            _mapper = mapper;
        }

        #region  Role

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin")]
        [HttpPost]
        public async Task<OperationResult<RoleDto>> AddUpdate([FromBody] RoleDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            int tenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            if (tenantId != null)
            {
                var roleObj = await _roleService.CheckInsertOrUpdate(model);
                model = _mapper.Map<RoleDto>(roleObj);
                return new OperationResult<RoleDto>(true, System.Net.HttpStatusCode.OK, "Updated successfully", model);
            }
            else
            {
                return new OperationResult<RoleDto>(false, System.Net.HttpStatusCode.OK, "Please provide tenantId.", model);
            }

        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin")]
        [HttpDelete]
        public async Task<OperationResult<RoleDto>> Remove([FromBody] RoleDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            int tenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            if (tenantId != null && model.RoleId != null)
            {
                var roleObj = _roleService.DeleteRoleByTenantAdmin(model);
                model = _mapper.Map<RoleDto>(roleObj);
                return new OperationResult<RoleDto>(true, System.Net.HttpStatusCode.OK, "Role deleted successfully.", model);
            }
            else
            {
                return new OperationResult<RoleDto>(false, System.Net.HttpStatusCode.OK, "Please provide tenant.", model);
            }

        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin")]
        [HttpGet("{TenantId}")]
        public async Task<OperationResult<List<RoleDto>>> List(long? TenantId)
        {
            List<RoleDto> roleDtoList = new List<RoleDto>();
            var roleList = new List<Role>();

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);


            if (TenantId == null)
            {
                roleList = _roleService.GetAllByAdmin();

            }
            else
            {
                roleList = _roleService.GetAllByTenantAdmin(TenantId.Value);
            }

            roleDtoList = _mapper.Map<List<RoleDto>>(roleList);
            return new OperationResult<List<RoleDto>>(true, System.Net.HttpStatusCode.OK, "Found successfully", roleDtoList);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<RoleDto>> Detail(int Id)
        {
            RoleDto roleDtoObj = new RoleDto();

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var roleObj = _roleService.GetRoleByTenant(Id, TenantId);
            roleDtoObj = _mapper.Map<RoleDto>(roleObj);
            return new OperationResult<RoleDto>(true, System.Net.HttpStatusCode.OK, "", roleDtoObj);
        }

        #endregion
    }
}