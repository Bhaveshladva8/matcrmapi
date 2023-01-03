using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using matcrm.api.SignalR;
using matcrm.api.Helper;
using matcrm.authentication.jwt;
using matcrm.data.Context;
using matcrm.data.Models.Dto;
using matcrm.data.Models.MollieModel.Payment.Response;
using matcrm.data.Models.Tables;
using matcrm.data.Models.ViewModels;
using matcrm.service.BusinessLogic;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.service.Services.Mollie.Payment;
using matcrm.service.Utility;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;

namespace matcrm.api.Controllers
{
    [Route("[controller]")]
    public class LoginController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly ITenantService _tenantService;
        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly IMollieSubscriptionService _mollieSubscriptionService;
        private readonly IPaymentStorageClient _paymentStorageClient;
        private readonly IMailBoxTeamService _mailBoxTeamService;
        private readonly IMapper _mapper;
        private JwtManager jwtManager;

        public LoginController(
        IUserService userService,
        IRoleService roleService,
        ITenantService tenantService,
        IUserSubscriptionService userSubscriptionService,
        IMollieSubscriptionService mollieSubscriptionService,
        IPaymentStorageClient paymentStorageClient,
        IMailBoxTeamService mailBoxTeamService,
        IMapper mapper)
        {
            _userService = userService;
            _roleService = roleService;
            _tenantService = tenantService;
            _userSubscriptionService = userSubscriptionService;
            _mollieSubscriptionService = mollieSubscriptionService;
            _paymentStorageClient = paymentStorageClient;
            _mailBoxTeamService = mailBoxTeamService;
            _mapper = mapper;
            jwtManager = new JwtManager(userService, tenantService, roleService);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDTO)
        {
            try
            {
                var checkEmail = _userService.GetUserByEmail(loginDTO.Email);
                if (checkEmail == null)
                {
                    return BadRequest("Email not registered.");
                }
                var externalRoleObj = _roleService.GetRole("ExternalUser");

                int? externalRoleId;
                if (externalRoleObj != null)
                {
                    externalRoleId = externalRoleObj.RoleId;

                    if (checkEmail != null && checkEmail.RoleId == externalRoleId && checkEmail.PasswordHash != null && checkEmail.PasswordHash.Length == 0)
                    {
                        return BadRequest("E-Mail or password wrong.");
                    }
                }

                var user = _userService.Authenticate(loginDTO.Email, loginDTO.Password, checkEmail.TenantId);

                string TenantName = "";
                if (user != null)
                {
                    if (user.IsEmailVerified == false)
                    {
                        return BadRequest("Kindly verify your email first.");
                    }
                    UserDto userDto = new UserDto();
                    userDto = _mapper.Map<UserDto>(user);
                    if (user.LastLoggedIn != null)
                    {
                        if (user.TenantId != null)
                        {
                            var tenantObj = _tenantService.GetTenantById(user.TenantId.Value);
                            if (tenantObj != null)
                            {
                                TenantName = tenantObj.TenantName;
                            }
                        }
                    }
                    var updatedUser = await _userService.UpdateUser(user, true);
                    userDto.Tenant = TenantName;

                    // Generate token
                    var RoleName = "";
                    if (userDto.RoleId != null)
                    {
                        var roleObj = _roleService.GetRoleById(userDto.RoleId.Value);
                        if (roleObj != null)
                            RoleName = roleObj.RoleName;
                    }

                    userDto.RoleName = RoleName;


                    var userSubscriptionObj = _userSubscriptionService.GetByUser(userDto.Id);
                    var mollieSubscriptionObj = _mollieSubscriptionService.GetByUser(userDto.Id);


                    // var token = new JwtTokenBuilder()
                    //    .AddClaims(GetClaim(user, TenantName, RoleName, userDto.IsSubscribed))
                    //    .Build();

                    Tokens JwtTokenData = await jwtManager.GenerateJWTTokens(user.Email, userDto.IsSubscribed);
                    return Ok(JwtTokenData.Access_Token);
                    // return Ok(token.Value);
                }
            }
            catch
            {
                return BadRequest
                ("An error occurred in generating the token");
            }
            return Unauthorized();
        }

        #region Private
        private Dictionary<string, string> GetClaim(User userObj, string Tenant, string RoleName, bool IsSubscribed = false)
        {
            var claim = new Dictionary<string, string>();
            claim.Add("Id", userObj.Id.ToString());
            claim.Add("Sid", userObj.Id.ToString());
            if (Tenant != "" || Tenant != null)
            {
                claim.Add("Tenant", Tenant.ToString());
            }
            if (userObj.TenantId != null)
            {
                claim.Add("TenantId", userObj.TenantId.ToString());
            }
            if (userObj.WeClappUserId != null)
            {
                claim.Add("WeClappUserId", userObj.WeClappUserId.ToString());
            }
            if (userObj.WeClappToken != null)
            {
                claim.Add("WeClappToken", userObj.WeClappToken);
            }
            if (RoleName != "")
            {
                claim.Add(ClaimTypes.Role, RoleName);
            }
            if (RoleName != "")
            {
                claim.Add("RoleName", RoleName);
            }
            if (userObj != null && !string.IsNullOrEmpty(userObj.Email))
            {
                claim.Add("Email", userObj.Email);
            }

            claim.Add("IsSubscribed", IsSubscribed.ToString());

            return claim;
        }
        #endregion
    }
}