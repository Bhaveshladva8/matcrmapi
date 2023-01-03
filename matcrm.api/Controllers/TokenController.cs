using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using matcrm.authentication.jwt;
using matcrm.data.Models.Tables;
using matcrm.data.Models.ViewModels;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.api.Helper;
using matcrm.data.Models.Request;

namespace matcrm.api.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class TokenController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITenantService _tenantService;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly IUserService _userService;
        private JwtManager jwtManager;
        public IConfiguration _configuration;
        private readonly IRoleService _roleService;

        public TokenController(IAuthService authService, ITenantService tenantService,
            IUserService userService,
            IRoleService roleService,
            IConfiguration config)
        {
            _authService = authService;
            _tenantService = tenantService;
            _userService = userService;
            _configuration = config;
            _roleService = roleService;
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
            jwtManager = new JwtManager(userService, tenantService, roleService);
        }

        [HttpPost]
        public async Task<OperationResult<TenantVM>> LoginTest([FromBody] UserVM user)
        {
            var TenantVMObj = new TenantVM();
            var authUser = await _authService.Login(user);
            if (authUser != null)
            {
                user.Id = authUser.Id;
                user.IsAdmin = authUser.IsAdmin;
                TenantVMObj.Id = user.Id;
                TenantVMObj.Username = user.Username;
                TenantVMObj.IsAdmin = user.IsAdmin;
                TenantVMObj.ApiKey = user.ApiKey;
                TenantVMObj.IsAdmin = true;
                TenantVMObj.IsSuccess = true;

                var userObj = _userService.GetUserByWeclappUserId(authUser.Id);

                // Generate token
                // var token = new JwtTokenBuilder()
                //     .AddClaims(GetClaim(userObj, user.Tenant))
                //     .Build();
                // TenantVMObj.accessToken = token.Value;

                Tokens JwtTokenData = await jwtManager.GenerateJWTTokens(user.Email, true);

                if (JwtTokenData != null)
                {
                    TenantVMObj.accessToken = JwtTokenData.Access_Token;
                }
            }
            else
            {
                TenantVMObj.IsSuccess = false;
            }
            return new OperationResult<TenantVM>(true, System.Net.HttpStatusCode.OK, "", TenantVMObj);
        }

        [HttpPost]
        public async Task<OperationResult<UserVM>> Login([FromBody] UserVM user)
        {
            UserVM userVMObj = new UserVM();

            userVMObj = await _authService.Login(user);

            if (userVMObj != null)
            {
                user.Id = userVMObj.Id;
                user.IsAdmin = userVMObj.IsAdmin;
                var success = Log(user);
                var userObj = _userService.GetUserById(userVMObj.Id);

                if (success)
                {
                    // Generate token
                    // var token = new JwtTokenBuilder ()
                    //     .AddClaims (GetClaim (userObj, user.Tenant))
                    //     .Build ();
                    // authUser.accessToken = token.Value;
                    Tokens JwtTokenData = await jwtManager.GenerateJWTTokens(user.Email, false);

                    if (JwtTokenData != null)
                    {
                        userVMObj.accessToken = JwtTokenData.Access_Token;
                    }
                    return new OperationResult<UserVM>(true, System.Net.HttpStatusCode.OK, "", userVMObj);
                }
                else
                {
                    return new OperationResult<UserVM>(false, System.Net.HttpStatusCode.OK, "Invalid Tenant/Credentials", userVMObj);
                }
            }
            else
            {
                return new OperationResult<UserVM>(false, System.Net.HttpStatusCode.OK, "Invalid Tenant/Credentials", userVMObj);
            }
        }

        [NonAction]
        public bool Log(UserVM user)
        {
            //Tenant tenantToAdd = new Tenant()
            //{
            //    TenantName = user.Tenant,
            //    CreatedDate = DateTime.UtcNow,
            //    BlockedDate = null,
            //    IsBlocked = false,
            //    Token = user.ApiKey
            //};
            //var tenant =  _tenantService.Add(tenantToAdd);

            if (user.IsAdmin) return true;
            // if (!user.IsAdmin) return true;

            //User weClappUser = new User()
            //{
            //    UserId = user.Id,
            //    TenantId = tenant.TenantId,
            //    IsBlocked = false,
            //    LastLoggedIn = DateTime.UtcNow,
            //    CreatedDate = DateTime.UtcNow,
            //    BlockedDate = null
            //};

            //_userService.Add(weClappUser);
            _userService.UpdateLastLogin(user.Id);

            bool blockedTenant = _tenantService.IsBlocked(user.Tenant);
            if (blockedTenant) return false;

            bool blockedUser = _userService.IsBlocked(user.Id);
            if (blockedUser) return false;

            return true;
        }

        [HttpPost]
        public async Task<OperationResult<UserVM>> UserLogin([FromBody] UserVM userVM)
        {
            var userVMObj = new UserVM();
            // var authUser = _userService.Login (userVM);
            var authUser = _userService.Authenticate(userVM.Email, userVM.Password, userVM.TenantId.Value);
            if (authUser != null)
            {
                userVMObj.Id = authUser.Id;
                userVMObj.FirstName = authUser.FirstName;
                userVMObj.LastName = authUser.LastName;
                userVMObj.Email = authUser.Email;
                userVMObj.PhoneNo = authUser.PhoneNo;
                userVMObj.TenantId = authUser.TenantId;
                userVMObj.IsBlocked = authUser.IsBlocked;

                var TenantName = "";
                if (authUser.TenantId != null)
                {
                    Tenant tenantObj = _tenantService.GetTenantById(authUser.TenantId.Value);
                    if (tenantObj != null)
                    {
                        TenantName = tenantObj.TenantName;
                    }
                }

                // Generate token
                var token = new JwtTokenBuilder()
                    .AddClaims(GetClaim(authUser, TenantName))
                    .Build();
                userVMObj.accessToken = token.Value;
            }
            else
            {
                return new OperationResult<UserVM>(false, System.Net.HttpStatusCode.OK, "", userVMObj);
            }
            return new OperationResult<UserVM>(true, System.Net.HttpStatusCode.OK, "", userVMObj);
        }

        // [HttpPost ("LoginWithSocial")]
        // public async Task<OperationResult<UserVM>> LoginWithSocial ([FromBody] SocialUserVM user) {
        //     var model = new UserVM ();
        //     var userExistObj = _userService.GetMany (t => t.TenantId == user.TenantId.Value && t.Email == user.Email && t.IsBlocked == false).Result.FirstOrDefault ();

        //     if (userExistObj == null) {
        //         User userObj = new User ();
        //         userObj.TenantId = user.TenantId.Value;
        //         userObj.FirstName = user.FirstName;
        //         userObj.LastName = user.LastName;
        //         userObj.Email = user.Email;
        //         userExistObj = _userService.AddUser (userObj, "");
        //     }

        //     model.FirstName = user.FirstName;
        //     model.LastName = user.LastName;
        //     model.Email = user.Email;
        //     model.TenantId = userExistObj.TenantId.Value;
        //     model.PhoneNo = userExistObj.PhoneNo;
        //     model.Id = userExistObj.Id;
        //     // model.Password = userExistObj.Password;
        //     model.IsBlocked = userExistObj.IsBlocked;

        //     var TenantName = "";
        //     var tenantObj = _tenantService.GetTenantById (user.TenantId.Value);
        //     if (tenantObj != null) {
        //         TenantName = tenantObj.TenantName;
        //     }

        //     // Generate token
        //     var token = new JwtTokenBuilder ()
        //         .AddClaims (GetClaim (userExistObj, TenantName))
        //         .Build ();
        //     model.accessToken = token.Value;
        //     return new OperationResult<UserVM> (true, "", model);
        // }

        [HttpPost]
        public async Task<OperationResult<WeClappUserVM>> WeClappUser(UserVM currentUser)
        {
            WeClappUserVM weClappUserVMObj = new WeClappUserVM();
            if (string.IsNullOrEmpty(currentUser.ApiKey) || string.IsNullOrEmpty(currentUser.TenantName))
            {
                return new OperationResult<WeClappUserVM>(false, System.Net.HttpStatusCode.OK, "", weClappUserVMObj);
            }

            weClappUserVMObj = await _userService.GetWeClappUser(currentUser.ApiKey, currentUser.TenantName);

            if (weClappUserVMObj != null)
                return new OperationResult<WeClappUserVM>(true, System.Net.HttpStatusCode.OK, "", weClappUserVMObj);

            return new OperationResult<WeClappUserVM>(false, System.Net.HttpStatusCode.OK, "No user found.", weClappUserVMObj);
        }

        #region Private
        private Dictionary<string, string> GetClaim(User userObj, string Tenant)
        {
            var claim = new Dictionary<string, string>();
            if (userObj != null)
            {
                claim.Add("Id", userObj.Id.ToString());
                if (userObj.WeClappUserId != null)
                {
                    claim.Add("WeClappUserId", userObj.WeClappUserId.ToString());
                }
                if (userObj.WeClappToken != null)
                {
                    claim.Add("WeClappToken", userObj.WeClappToken);
                }
                if (Tenant == "admin")
                {
                    claim.Add("Role", "Admin");
                }
                else
                {
                    claim.Add("Role", "User");
                }
            }
            claim.Add("Tenant", Tenant.ToString());

            return claim;
        }
        #endregion
    }
}