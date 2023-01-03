using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.service.Common;
using matcrm.service.Services;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class TenantController : Controller
    {
        // private readonly IAuthService _authService;
        private readonly ITenantService _tenantService;
        private readonly ITenantActivityService _tenantActivityService;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly IUserService _userService;
        public IConfiguration _configuration;
        private int UserId = 0;

        public TenantController(
            // IAuthService authService,
            ITenantService tenantService,
            ITenantActivityService tenantActivityService,
            IUserService userService,
            IConfiguration config)
        {
            // _authService = authService;
            _tenantService = tenantService;
            _tenantActivityService = tenantActivityService;
            _userService = userService;
            _configuration = config;
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        [Authorize(Roles = "Admin, TenantAdmin")]
        [HttpPost]
        public async Task<OperationResult<Tenant>> AddUpdate([FromBody] TenantDto tenant)
        {
            Tenant model = new Tenant();

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            int tenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            
            var existingItem = _tenantService.GetTenant(tenant.TenantName);
            model = await _tenantService.CheckInsertOrUpdate(tenant);
            TenantActivity tenantActivityObj = new TenantActivity();
            tenantActivityObj.UserId = UserId;
            tenantActivityObj.TenantId = tenantId;
            if (existingItem != null)
            {
                tenantActivityObj.Activity = "Updated tenant";
            }
            else
            {
                tenantActivityObj.Activity = "Created tenant";
            }
            var AddUpdate = await _tenantActivityService.CheckInsertOrUpdate(tenantActivityObj);
            return new OperationResult<Tenant>(true, System.Net.HttpStatusCode.OK,"", model);
        }

        [Authorize(Roles = "Admin, TenantAdmin")]
        [HttpPut]
        public async Task<OperationResult<Tenant>> Block([FromBody] TenantDto tenant)
        {
            Tenant model = new Tenant();

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            int tenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            model = await _tenantService.Block(tenant.TenantId);
            TenantActivity tenantActivityObj = new TenantActivity();
            tenantActivityObj.UserId = UserId;
            tenantActivityObj.TenantId = tenantId;
            tenantActivityObj.Activity = "Blocked tenant";
            var AddUpdate = await _tenantActivityService.CheckInsertOrUpdate(tenantActivityObj);
            return new OperationResult<Tenant>(true, System.Net.HttpStatusCode.OK,"", model);
        }

        [Authorize(Roles = "Admin, TenantAdmin")]
        [HttpPut]
        public async Task<OperationResult<Tenant>> Revoke([FromBody] TenantDto tenant)
        {
            Tenant model = new Tenant();

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            int tenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            model = await _tenantService.Revoke(tenantId);
            TenantActivity tenantActivityObj = new TenantActivity();
            tenantActivityObj.UserId = UserId;
            tenantActivityObj.TenantId = tenantId;
            tenantActivityObj.Activity = "Revoked tenant";
            var AddUpdate = await _tenantActivityService.CheckInsertOrUpdate(tenantActivityObj);
            return new OperationResult<Tenant>(true, System.Net.HttpStatusCode.OK,"", model);
        }

        [Authorize(Roles = "Admin, TenantAdmin")]
        [HttpGet]
        public async Task<OperationResult<List<Tenant>>> List()
        {
            List<Tenant> tenantList = new List<Tenant>();
            tenantList = _tenantService.GetAll();
            return new OperationResult<List<Tenant>>(true, System.Net.HttpStatusCode.OK,"", tenantList);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<OperationResult<List<Tenant>>> ListByAdmin()
        {
            List<Tenant> tenantList = new List<Tenant>();
            tenantList = _tenantService.GetAllTenantByAdmin();
            return new OperationResult<List<Tenant>>(true, System.Net.HttpStatusCode.OK,"", tenantList);
        }

        [Authorize(Roles = "Admin, TenantAdmin")]
        [HttpPost]
        public bool IsBloked([FromBody] Tenant tenant)
        {
            bool IsBloked = false;
            IsBloked = _tenantService.IsBlocked(tenant.TenantName);
            return IsBloked;
        }

        [AllowAnonymous]
        // [Authorize]
        [HttpPost]
        public async Task<OperationResult<Tenant>> Detail([FromBody] Tenant tenant)
        {
            Tenant tenantObj = new Tenant();
            var request = HttpContext.Request;
            var ip = request.HttpContext.Connection.RemoteIpAddress;
            var CurrentTenat = Request.Headers["x-tenant-id"].ToString();
            var ua = Request.Headers["User-Agent"].ToString();
            var host = !string.IsNullOrEmpty(Request.Headers["Origin"].ToString()) ? (new Uri(Request.Headers["Origin"].ToString()).Authority ?? "") : "";
            tenantObj = _tenantService.GetTenant(tenant.TenantName);
            if (tenantObj != null)
            {
                return new OperationResult<Tenant>(true, System.Net.HttpStatusCode.OK,"", tenantObj);
            }
            else
            {
                return new OperationResult<Tenant>(false, System.Net.HttpStatusCode.OK,"", tenantObj);
            }
        }

        [Authorize(Roles = "Admin, TenantAdmin")]
        [HttpPut]
        public async Task<OperationResult<User>> BlockUser()
        {
            User userObj = new User();

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            userObj = _userService.Block(UserId);
            if (userObj == null)
                return new OperationResult<User>(false, System.Net.HttpStatusCode.OK,"User not found", userObj);
            else
                return new OperationResult<User>(true, System.Net.HttpStatusCode.OK,"User blocked successfully", userObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin")]
        [HttpPut("{UserId}")]
        public async Task<OperationResult<User>> RevokeUser(int UserId)
        {
            User userObj = new User();

            userObj = _userService.Unblock(UserId);
            if (userObj == null)
                return new OperationResult<User>(false, System.Net.HttpStatusCode.OK,"User not found", userObj);
            else
                return new OperationResult<User>(true, System.Net.HttpStatusCode.OK,"User revoked successfully", userObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin")]
        [HttpDelete]
        public async Task<OperationResult<User>> RemoveUser(int UserId)
        {
            User userObj = new User();

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            userObj = _userService.DeleteUser(UserId);
            if (userObj != null)
                return new OperationResult<User>(true, System.Net.HttpStatusCode.OK,"User deleted successfully", userObj);
            else
                return new OperationResult<User>(false, System.Net.HttpStatusCode.OK,"User not found!", userObj);
        }
    }
}