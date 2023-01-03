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
using Swashbuckle.AspNetCore.Annotations;

namespace matcrm.api.Controllers
{
    [Route("[controller]/[action]")]
    public class UserController : Controller
    {

        #region  Service Initialization
        private readonly IUserService _userService;
        private readonly ITenantService _tenantService;
        private readonly ITenantActivityService _tenantActivityService;
        private readonly IVerificationCodeService _verificationCodeService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEmailLogService _emailLogService;
        private readonly IEmailConfigService _emailConfigService;
        private readonly IEmailProviderService _emailProviderService;
        private readonly ILanguageService _languageService;
        private readonly IRoleService _roleService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly IIntProviderService _intProviderService;
        private readonly IExternalUserService _externalUserService;
        private readonly IMailBoxTeamService _mailBoxTeamService;
        private readonly IPaymentStorageClient _paymentStorageClient;
        private readonly IMollieSubscriptionService _mollieSubscriptionService;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private SignupUser signupUser;
        private IMapper _mapper;
        private SendEmail sendEmail;
        private JwtHandler _jwtHandler;
        private readonly OneClappContext _context;
        private JwtManager jwtManager;
        private Common Common;
        private int UserId = 0;
        private int TenantId = 0;

        #endregion

        #region  Constructor
        public UserController(
            IMapper mapper,
            ITenantService tenantService,
            ITenantActivityService tenantActivityService,
            IUserService userService,
            IVerificationCodeService verificationCodeService,
            IEmailTemplateService emailTemplateService,
            IEmailLogService emailLogService,
            IEmailConfigService emailConfigService,
            IEmailProviderService emailProviderService,
            ILanguageService languageService,
            IRoleService roleService,
            OneClappContext context,
            IHostingEnvironment hostingEnvironment,
            IUserSubscriptionService userSubscriptionService,
            IConfiguration configuration,
            IIntProviderService intProviderService,
            IExternalUserService externalUserService,
            IMailBoxTeamService mailBoxTeamService,
            IPaymentStorageClient paymentStorageClient,
            IMollieSubscriptionService mollieSubscriptionService,
            IHubContext<BroadcastHub, IHubClient> hubContext
        )
        {
            _tenantService = tenantService;
            _tenantActivityService = tenantActivityService;
            _userService = userService;
            _verificationCodeService = verificationCodeService;
            _emailTemplateService = emailTemplateService;
            _emailLogService = emailLogService;
            _roleService = roleService;
            _emailProviderService = emailProviderService;
            _emailConfigService = emailConfigService;
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;
            // _localizer = localizer;
            _languageService = languageService;
            _userSubscriptionService = userSubscriptionService;
            _intProviderService = intProviderService;
            _externalUserService = externalUserService;
            _mailBoxTeamService = mailBoxTeamService;
            _paymentStorageClient = paymentStorageClient;
            _mollieSubscriptionService = mollieSubscriptionService;
            _hubContext = hubContext;
            _context = context;
            sendEmail = new SendEmail(emailTemplateService, emailLogService, emailConfigService, emailProviderService, mapper);
            signupUser = new SignupUser(emailTemplateService, emailLogService, verificationCodeService, emailConfigService, emailProviderService, mapper, userService, context);
            Common = new Common();
            _jwtHandler = new JwtHandler(configuration);
            jwtManager = new JwtManager(userService, tenantService, roleService);
        }

        #endregion

        [AllowAnonymous]
        [HttpPost]
        public async Task<OperationResult<UserSignUpResponseDto>> Register([FromBody] UserRegisterRequest model)
        {
            UserDto userDto = new UserDto();
            var requestmodel = _mapper.Map<UserDto>(model);
            UserSignUpResponseDto userSignUpResponseDto = new UserSignUpResponseDto();
            if (requestmodel.RoleId == null)
            {
                // var userRole = _roleService.GetRole("TenantUser");
                var userRole = _roleService.GetRole("TenantAdmin");
                if (userRole != null)
                {
                    requestmodel.RoleId = userRole.RoleId;
                }
            }

            TenantDto tenantDto = new TenantDto();
            Random generator = new Random();
            String r = generator.Next(0, 1000000).ToString("D6");
            tenantDto.TenantName = requestmodel.UserName + r;
            var AddUpdateTenant = await _tenantService.CheckInsertOrUpdate(tenantDto);
            requestmodel.TenantId = AddUpdateTenant.TenantId;

            var result = await signupUser.ProcessUserSignup(requestmodel);

            string address = requestmodel.Email;
            string host;

            // using Split
            host = address.Split('@')[1];
            host = host.Split('.')[0];

            var teamName = host + "_" + r;

            // var user = _mapper.Map<User> (model);
            // userObj = _userService.AddUser (user, model.Password);
            // if (userObj == null)
            //     return new OperationResult<User> (false, "User not found", userObj);
            if (!result.IsSuccessSignUp)
            {
                return new OperationResult<UserSignUpResponseDto>(result.IsSuccessSignUp, System.Net.HttpStatusCode.OK, result.ErrorMessage, userSignUpResponseDto);
            }

            User user = _mapper.Map<User>(result);
            var TenantName = "";
            TenantName = tenantDto.TenantName;
            userDto.Tenant = TenantName;

            // Generate token
            var RoleName = "TenantAdmin";

            userDto.RoleName = RoleName;
            userDto.IsSubscribed = true;

            if (user.CreatedBy == null)
            {
                var mailBoxTeamObj = _mailBoxTeamService.GetByUser(user.Id);
                if (mailBoxTeamObj == null)
                {
                    MailBoxTeamDto mailBoxTeamDto = new MailBoxTeamDto();
                    mailBoxTeamDto.Name = teamName;
                    mailBoxTeamDto.CreatedBy = user.Id;
                    mailBoxTeamDto.TenantId = user.TenantId;
                    var AddUpdate = await _mailBoxTeamService.CheckInsertOrUpdate(mailBoxTeamDto);
                }
            }

            // var token = new JwtTokenBuilder()
            //    .AddClaims(GetClaim(user, TenantName, RoleName, userDto.IsSubscribed))
            //    .Build();

            Tokens JwtTokenData = await jwtManager.GenerateJWTTokens(user.Email, userDto.IsSubscribed);
            var token = "";
            if (JwtTokenData != null)
            {
                token = JwtTokenData.Access_Token;
            }

            userSignUpResponseDto.Id = userDto.Id;
            userSignUpResponseDto.Token = token;
            userSignUpResponseDto.TenantId = user.TenantId;
            await sendEmail.SendNewSignUpEmail(requestmodel.Email, user.TempGuid, user.FirstName + ' ' + user.LastName, result.EmailOTP, user.TenantId, token);
            return new OperationResult<UserSignUpResponseDto>(true, System.Net.HttpStatusCode.OK, "User register successfully", userSignUpResponseDto);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<OperationResult<User>> InviteUser([FromBody] UserDto model)
        {
            User userObj = new User();
            var user = _mapper.Map<User>(model);
            model.Password = "Test@123";
            userObj = await _userService.AddUser(user, model.Password);
            if (userObj == null)
                return new OperationResult<User>(false, System.Net.HttpStatusCode.OK, "User not found", userObj);
            else
                await sendEmail.SendSignUpEmail(model.Email, ShaHashData.GetHash(user.Email), user.FirstName + ' ' + user.LastName, "123123", model.TenantId.Value, "");
            return new OperationResult<User>(true, System.Net.HttpStatusCode.OK, "User revoked successfully", userObj);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<OperationResult<UserAuthenticateResponse>> Authenticate([FromBody] UserAuthenticateRequest model)
        {
            var requestmodel = _mapper.Map<AuthenticateModel>(model);
            UserDto userModel = new UserDto();
            UserAuthenticateResponse responsemodel = new UserAuthenticateResponse();
            string address = requestmodel.Email;
            string host;

            // using Split
            host = address.Split('@')[1];
            host = host.Split('.')[0];

            Random generator = new Random();
            String r = generator.Next(0, 1000000).ToString("D6");

            var teamName = host + "_" + r;

            // using Split with maximum number of substrings (more explicit)
            // host = address.Split(new char[] { '@' }, 2)[1];
            // host = host.Split(new char[] { '.' }, 2)[1];

            var Password = ShaHashData.DecodePassWord(requestmodel.Password);
            requestmodel.Password = Password;
            var checkEmail = _userService.GetUserByEmail(requestmodel.Email);
            if (checkEmail == null)
            {
                responsemodel = _mapper.Map<UserAuthenticateResponse>(userModel);
                return new OperationResult<UserAuthenticateResponse>(false, System.Net.HttpStatusCode.OK, "Email not registered.", responsemodel);
            }
            // else if (checkEmail.PasswordHash == null && checkEmail.PasswordSalt == null)
            // {
            //     responsemodel = _mapper.Map<UserAuthenticateResponse>(userModel);
            //     return new OperationResult<UserAuthenticateResponse>(false, System.Net.HttpStatusCode.OK,"Please update your password.", responsemodel);
            // }
            var externalRoleObj = _roleService.GetRole("ExternalUser");

            int? externalRoleId;
            if (externalRoleObj != null)
            {
                externalRoleId = externalRoleObj.RoleId;

                if (checkEmail != null && checkEmail.RoleId == externalRoleId && checkEmail.PasswordHash != null && checkEmail.PasswordHash.Length == 0)
                {
                    responsemodel = _mapper.Map<UserAuthenticateResponse>(userModel);
                    return new OperationResult<UserAuthenticateResponse>(false, System.Net.HttpStatusCode.OK, "E-Mail or password wrong.", responsemodel);
                }
            }
            User? user = null;
            if (checkEmail.PasswordHash == null && checkEmail.PasswordSalt == null)
            {
                user = checkEmail;
            }
            else
            {
                user = _userService.Authenticate(requestmodel.Email, requestmodel.Password, requestmodel.TenantId);
            }
            string TenantName = "";
            if (user != null)
            {
                if (user.IsEmailVerified == false)
                {
                    responsemodel = _mapper.Map<UserAuthenticateResponse>(userModel);
                    return new OperationResult<UserAuthenticateResponse>(false, System.Net.HttpStatusCode.OK, "Kindly verify your email first.", responsemodel);
                }

                userModel = _mapper.Map<UserDto>(user);
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
                if (checkEmail.PasswordHash != null && checkEmail.PasswordSalt != null)
                {
                    var updatedUser = await _userService.UpdateUser(user, true);
                }

                userModel.Tenant = TenantName;

                // Generate token
                var RoleName = "";
                if (userModel.RoleId != null)
                {
                    var roleObj = _roleService.GetRoleById(userModel.RoleId.Value);
                    if (roleObj != null)
                        RoleName = roleObj.RoleName;
                }

                userModel.RoleName = RoleName;

                var userSubscriptionObj = _userSubscriptionService.GetByUser(userModel.Id);
                var mollieSubscriptionObj = _mollieSubscriptionService.GetByUser(userModel.Id);

                if (userSubscriptionObj != null)
                {
                    var subscribedOn = userSubscriptionObj.CreatedOn.Value;
                    if (userSubscriptionObj.SubscribedOn != null)
                    {
                        subscribedOn = userSubscriptionObj.SubscribedOn.Value;
                    }
                    if (userSubscriptionObj.UpdatedOn != null)
                    {
                        subscribedOn = userSubscriptionObj.UpdatedOn.Value;
                    }
                    var Today = DateTime.Today;
                    TimeSpan diff = Today.Date - subscribedOn.Date;

                    if (userSubscriptionObj.SubscriptionType != null)
                    {
                        if (mollieSubscriptionObj != null)
                        {
                            if (userSubscriptionObj.SubscriptionType.Name.ToLower() == "monthly")
                            {
                                if (diff.Days <= 30)
                                {
                                    if (!string.IsNullOrEmpty(mollieSubscriptionObj.PaymentId))
                                    {
                                        PaymentResponse molliPaymentObj = await _paymentStorageClient.GetPayment(mollieSubscriptionObj.PaymentId);
                                        if (molliPaymentObj != null && molliPaymentObj.Status.ToLower() == "paid")
                                        {
                                            userModel.IsSubscribed = true;
                                        }
                                        else
                                        {
                                            userModel.IsSubscribed = false;
                                        }
                                    }
                                    else
                                    {
                                        userModel.IsSubscribed = true;
                                    }
                                }
                            }

                            else if (userSubscriptionObj.SubscriptionType.Name.ToLower() == "yearly")
                            {
                                if (diff.Days <= 365)
                                {
                                    // userModel.IsSubscribed = true;
                                    if (!string.IsNullOrEmpty(mollieSubscriptionObj.PaymentId))
                                    {
                                        PaymentResponse molliPaymentObj = await _paymentStorageClient.GetPayment(mollieSubscriptionObj.PaymentId);
                                        if (molliPaymentObj != null && molliPaymentObj.Status.ToLower() == "paid")
                                        {
                                            userModel.IsSubscribed = true;
                                        }
                                        else
                                        {
                                            userModel.IsSubscribed = false;
                                        }
                                    }
                                    else
                                    {
                                        userModel.IsSubscribed = true;
                                    }
                                }
                                else
                                {
                                    userModel.IsSubscribed = false;
                                }
                            }
                        }
                        else
                        {
                            userModel.IsSubscribed = false;
                        }
                    }
                }
                else
                {
                    if (userModel.CreatedOn != null)
                    {
                        DateTime createdOn = userModel.CreatedOn.Value;
                        var Today = DateTime.Today;
                        TimeSpan diff = Today.Date - createdOn.Date;
                        if (diff.Days <= 30)
                        {
                            userModel.IsSubscribed = true;
                        }
                    }
                }

                if (user.CreatedBy == null)
                {
                    var mailBoxTeamObj = _mailBoxTeamService.GetByUser(user.Id);
                    if (mailBoxTeamObj == null)
                    {
                        MailBoxTeamDto mailBoxTeamDto = new MailBoxTeamDto();
                        mailBoxTeamDto.Name = teamName;
                        mailBoxTeamDto.CreatedBy = user.Id;
                        mailBoxTeamDto.TenantId = user.TenantId;
                        var AddUpdate = await _mailBoxTeamService.CheckInsertOrUpdate(mailBoxTeamDto);
                    }
                }

                Tokens JwtTokenData = await jwtManager.GenerateJWTTokens(user.Email, userModel.IsSubscribed);
                userModel.AccessToken = JwtTokenData.Access_Token;
                // var token = new JwtTokenBuilder()
                //    .AddClaims(GetClaim(user, TenantName, RoleName, userModel.IsSubscribed))
                //    .Build();
                // userModel.AccessToken = token.Value;
                responsemodel = _mapper.Map<UserAuthenticateResponse>(userModel);
                responsemodel.RefreshToken = JwtTokenData.Refresh_Token;
                if (checkEmail.PasswordHash == null && checkEmail.PasswordSalt == null)
                {
                    return new OperationResult<UserAuthenticateResponse>(false, System.Net.HttpStatusCode.OK, "", responsemodel);
                }
                else
                {
                    return new OperationResult<UserAuthenticateResponse>(true, System.Net.HttpStatusCode.OK, "", responsemodel);
                }
            }
            responsemodel = _mapper.Map<UserAuthenticateResponse>(userModel);
            return new OperationResult<UserAuthenticateResponse>(false, System.Net.HttpStatusCode.OK, "E-Mail or password wrong.", responsemodel);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<OperationResult<UserAuthenticateResponse>> Refresh([FromBody] Tokens token)
        {
            UserAuthenticateResponse userAuthenticateResponse = new UserAuthenticateResponse();
            var principal = jwtManager.GetPrincipalFromExpiredToken(token.Access_Token);
            var selectedEmail = "";
            bool IsSubscribed = false;
            foreach (var claimObj in principal.Claims)
            {
                var data = claimObj;
                if (claimObj.Type == "Email")
                {
                    selectedEmail = claimObj.Value;
                }
                else if (claimObj.Type == "IsSubscribed")
                {
                    IsSubscribed = Convert.ToBoolean(claimObj.Value);
                }
            }
            var userObj = _userService.GetUserByEmail(selectedEmail);

            var savedRefreshToken = userObj.RefreshToken;

            //retrieve the saved refresh token from database
            // var savedRefreshToken = _userService.GetSavedRefreshTokens(username, token.Refresh_Token);

            if (savedRefreshToken != token.Refresh_Token)
            {
                // return Unauthorized("Invalid attempt!");
                return new OperationResult<UserAuthenticateResponse>(false, System.Net.HttpStatusCode.BadRequest, "Invalid attempt!");
            }

            var newJwtToken = await jwtManager.GenerateRefreshToken(selectedEmail, IsSubscribed);

            if (newJwtToken == null)
            {
                // return Unauthorized("Invalid attempt!");
                return new OperationResult<UserAuthenticateResponse>(false, System.Net.HttpStatusCode.BadRequest, "Invalid attempt!");
            }

            // saving refresh token to the db
            UserRefreshTokens obj = new UserRefreshTokens
            {
                RefreshToken = newJwtToken?.Refresh_Token,
                UserName = selectedEmail
            };

            // userServiceRepository.DeleteUserRefreshTokens(username, token.Refresh_Token);
            // userServiceRepository.AddUserRefreshTokens(obj);
            // userServiceRepository.SaveCommit();
            // var newJwtToken = "aaaaaaaa";

            // return Ok(newJwtToken);
            var userResponse = _mapper.Map<UserAuthenticateResponse>(userObj);
            userResponse.AccessToken = newJwtToken.Access_Token;
            userResponse.RefreshToken = newJwtToken.Refresh_Token;
            userResponse.IsSubscribed = IsSubscribed;
            return new OperationResult<UserAuthenticateResponse>(true, "", userResponse);
        }

        [Authorize]
        [HttpGet]
        public async Task<OperationResult<UserAuthenticateResponse>> UpdatedToken()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            UserId = Convert.ToInt32(user.FindFirst("Sid").Value);
            UserDto userModel = new UserDto();
            UserAuthenticateResponse responsemodel = new UserAuthenticateResponse();
            var userObj = _userService.GetUserById(UserId);

            string TenantName = "";
            if (userObj != null)
            {
                userModel = _mapper.Map<UserDto>(userObj);
                // if (userObj.LastLoggedIn != null)
                // {
                if (userObj.TenantId != null)
                {
                    var tenantObj = _tenantService.GetTenantById(userObj.TenantId.Value);
                    if (tenantObj != null)
                    {
                        TenantName = tenantObj.TenantName;
                    }
                }
                // }
                // var updatedUser = await _userService.UpdateUser(userObj, true);
                userModel.Tenant = TenantName;

                // Generate token
                var RoleName = "";
                if (userModel.RoleId != null)
                {
                    var roleObj = _roleService.GetRoleById(userModel.RoleId.Value);
                    if (roleObj != null)
                        RoleName = roleObj.RoleName;
                }

                userModel.RoleName = RoleName;


                var userSubscriptionObj = _userSubscriptionService.GetByUser(userModel.Id);
                var mollieSubscriptionObj = _mollieSubscriptionService.GetByUser(userModel.Id);

                if (userSubscriptionObj != null)
                {
                    var subscribedOn = userSubscriptionObj.CreatedOn.Value;
                    if (userSubscriptionObj.SubscribedOn != null)
                    {
                        subscribedOn = userSubscriptionObj.SubscribedOn.Value;
                    }
                    if (userSubscriptionObj.UpdatedOn != null)
                    {
                        subscribedOn = userSubscriptionObj.UpdatedOn.Value;
                    }
                    var Today = DateTime.Today;
                    TimeSpan diff = Today.Date - subscribedOn.Date;

                    if (userSubscriptionObj.SubscriptionType != null)
                    {
                        if (userSubscriptionObj.SubscriptionType.Name.ToLower() == "monthly")
                        {
                            if (diff.Days <= 30)
                            {
                                if (!string.IsNullOrEmpty(mollieSubscriptionObj.PaymentId))
                                {
                                    PaymentResponse molliPaymentObj = await _paymentStorageClient.GetPayment(mollieSubscriptionObj.PaymentId);
                                    if (molliPaymentObj != null && molliPaymentObj.Status.ToLower() == "paid")
                                    {
                                        userModel.IsSubscribed = true;
                                    }
                                    else
                                    {
                                        userModel.IsSubscribed = false;
                                    }
                                }
                                else
                                {
                                    userModel.IsSubscribed = true;
                                }
                            }
                        }
                        else if (userSubscriptionObj.SubscriptionType.Name.ToLower() == "yearly")
                        {
                            if (diff.Days <= 365)
                            {
                                // userModel.IsSubscribed = true;
                                if (!string.IsNullOrEmpty(mollieSubscriptionObj.PaymentId))
                                {
                                    PaymentResponse molliPaymentObj = await _paymentStorageClient.GetPayment(mollieSubscriptionObj.PaymentId);
                                    if (molliPaymentObj != null && molliPaymentObj.Status.ToLower() == "paid")
                                    {
                                        userModel.IsSubscribed = true;
                                    }
                                    else
                                    {
                                        userModel.IsSubscribed = false;
                                    }
                                }
                                else
                                {
                                    userModel.IsSubscribed = true;
                                }
                            }
                            else
                            {
                                userModel.IsSubscribed = false;
                            }
                        }
                    }
                }
                else
                {
                    if (userModel.CreatedOn != null)
                    {
                        DateTime createdOn = userModel.CreatedOn.Value;
                        var Today = DateTime.Today;
                        TimeSpan diff = Today.Date - createdOn.Date;
                        if (diff.Days <= 30)
                        {
                            userModel.IsSubscribed = true;
                        }
                    }
                }

                // var token = new JwtTokenBuilder()
                //    .AddClaims(GetClaim(userObj, TenantName, RoleName, userModel.IsSubscribed))
                //    .Build();
                // userModel.AccessToken = token.Value;

                Tokens JwtTokenData = await jwtManager.GenerateJWTTokens(userObj.Email, userModel.IsSubscribed);
                if (JwtTokenData != null)
                {
                    userModel.AccessToken = JwtTokenData.Access_Token;
                }
                responsemodel = _mapper.Map<UserAuthenticateResponse>(userModel);
                return new OperationResult<UserAuthenticateResponse>(true, System.Net.HttpStatusCode.OK, "", responsemodel);

            }
            responsemodel = _mapper.Map<UserAuthenticateResponse>(userModel);
            return new OperationResult<UserAuthenticateResponse>(false, System.Net.HttpStatusCode.OK, "E-Mail or password wrong.", responsemodel);
        }

        [HttpPost]
        public async Task<OperationResult<UserExternalLoginResponse>> ExternalLogin([FromBody] UserExternalLoginRequest externalAuth)
        {
            // externalAuth.Provider = "GOOGLE";
            var requestExternalAuth = _mapper.Map<ExternalAuthDto>(externalAuth);
            UserDto userModel = new UserDto();
            UserExternalLoginResponse responsemodel = new UserExternalLoginResponse();
            var ischeck = Common.IsTokenExpired(requestExternalAuth.IdToken);
            long? exp;
            DateTime? expiredOn;

            if (ischeck == false)
            {
                var jwt = requestExternalAuth.IdToken;
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt) as JwtSecurityToken;
                var jti = token.Claims.First(claim => claim.Type == "exp").Value;
                if (string.IsNullOrEmpty(requestExternalAuth.Email))
                {
                    requestExternalAuth.Email = token.Claims.First(claim => claim.Type == "email").Value;
                }
                if (string.IsNullOrEmpty(requestExternalAuth.FirstName))
                {
                    requestExternalAuth.FirstName = token.Claims.First(claim => claim.Type == "email").Value;
                }
                if (string.IsNullOrEmpty(requestExternalAuth.LastName))
                {
                    requestExternalAuth.LastName = token.Claims.First(claim => claim.Type == "email").Value;
                }
                if (string.IsNullOrEmpty(requestExternalAuth.Name))
                {
                    requestExternalAuth.Name = token.Claims.First(claim => claim.Type == "email").Value;
                }
                exp = Convert.ToInt64(jti);
                expiredOn = Common.UnixTimeStampToDateTime(exp.Value);

                var userData = _userService.GetUserByEmail(requestExternalAuth.Email);
                var RoleName = "";
                var TenantName = "";
                var isAddUser = false;
                if (userData == null)
                {
                    userModel.Email = requestExternalAuth.Email;
                    userModel.UserName = requestExternalAuth.Name;
                    userModel.FirstName = requestExternalAuth.FirstName;
                    userModel.LastName = requestExternalAuth.LastName;
                    string guid = Convert.ToString(Guid.NewGuid());
                    userModel.TempGuid = guid;
                    TenantDto tenantDto = new TenantDto();
                    Random generator = new Random();
                    String r = generator.Next(0, 1000000).ToString("D6");
                    tenantDto.TenantName = userModel.UserName + r;
                    var AddUpdateTenant = await _tenantService.CheckInsertOrUpdate(tenantDto);

                    if (AddUpdateTenant != null)
                    {
                        TenantName = AddUpdateTenant.TenantName;
                    }
                    userModel.TenantId = AddUpdateTenant.TenantId;

                    var roleObj = _roleService.GetRole("ExternalUser");

                    if (roleObj != null)
                    {
                        userModel.RoleId = roleObj.RoleId;
                        RoleName = roleObj.RoleName;
                    }
                    userModel.IsEmailVerified = true;
                    userModel.VerifiedOn = DateTime.UtcNow;
                    userData = await _userService.ExternalUser(userModel);
                    isAddUser = true;
                    userModel.Id = userData.Id;

                    try
                    {
                        await sendEmail.SendSetPasswordEmail(userData.Email, userData.Id.ToString(), userData.TempGuid.ToString(), userData.FirstName + ' ' + userData.LastName, userData.TenantId, "Set password");
                    }
                    catch (Exception ex)
                    {
                        // errorlogservice.LogException (ex);
                        throw;
                    }
                    // userModel = _mapper.Map<UserDto>(userObj);
                    // userObj = _mapper.Map<User>(userModel);


                }
                else
                {
                    userModel = _mapper.Map<UserDto>(userData);

                    if (string.IsNullOrEmpty(RoleName) && userData.RoleId != null)
                    {
                        var roleObj = _roleService.GetRoleById(userData.RoleId.Value);
                        if (roleObj != null)
                        {
                            RoleName = roleObj.RoleName;
                        }
                    }

                    if (string.IsNullOrEmpty(TenantName) && userData.TenantId != null)
                    {
                        var tenantObj = _tenantService.GetTenantById(userData.TenantId.Value);
                        if (tenantObj != null)
                        {
                            TenantName = tenantObj.TenantName;
                        }
                    }

                    if (isAddUser == false)
                    {
                        userData = await _userService.UpdateUser(userData, true);
                    }
                }
                ExternalUserDto externalUserDto = new ExternalUserDto();
                var intProviderObj = _intProviderService.GetIntProvider(requestExternalAuth.Provider);

                if (intProviderObj != null)
                {
                    externalUserDto.IntProviderId = intProviderObj.Id;
                }
                else
                {
                    IntProviderDto intProviderDto = new IntProviderDto();
                    intProviderDto.Name = requestExternalAuth.Name;
                    intProviderObj = _intProviderService.CheckInsertOrUpdate(intProviderDto);
                    externalUserDto.IntProviderId = intProviderObj.Id;
                }
                externalUserDto.Email = requestExternalAuth.Email;
                externalUserDto.FirstName = requestExternalAuth.FirstName;
                externalUserDto.LastName = requestExternalAuth.LastName;
                externalUserDto.Id_Token = requestExternalAuth.IdToken;
                externalUserDto.Token_Type = "Bearer";
                if (expiredOn != null)
                {
                    externalUserDto.ExpiredOn = expiredOn.Value;
                }
                externalUserDto.UserId = userData.Id;
                var externalUserObj = await _externalUserService.CheckInsertOrUpdate(externalUserDto);
                userModel.Tenant = TenantName;


                userModel.RoleName = RoleName;
                userModel.Tenant = TenantName;
                var userSubscriptionObj = _userSubscriptionService.GetByUser(userModel.Id);

                if (userSubscriptionObj != null)
                {
                    var subscribedOn = userSubscriptionObj.CreatedOn.Value;
                    if (userSubscriptionObj.SubscribedOn != null)
                    {
                        subscribedOn = userSubscriptionObj.SubscribedOn.Value;
                    }
                    if (userSubscriptionObj.UpdatedOn != null)
                    {
                        subscribedOn = userSubscriptionObj.UpdatedOn.Value;
                    }
                    var Today = DateTime.Today;
                    TimeSpan diff = Today.Date - subscribedOn.Date;

                    if (userSubscriptionObj.SubscriptionType != null)
                    {
                        if (userSubscriptionObj.SubscriptionType.Name == "Monthly")
                        {
                            if (diff.Days <= 30)
                            {
                                userModel.IsSubscribed = true;
                            }
                        }
                        else if (userSubscriptionObj.SubscriptionType.Name == "Yearly")
                        {
                            if (diff.Days <= 365)
                            {
                                userModel.IsSubscribed = true;
                            }
                        }
                    }
                }
                else
                {
                    if (userData.CreatedOn != null)
                    {
                        DateTime createdOn = userData.CreatedOn.Value;
                        var Today = DateTime.Today;
                        TimeSpan diff = Today.Date - createdOn.Date;
                        if (diff.Days <= 30)
                        {
                            userModel.IsSubscribed = true;
                        }
                    }
                }
                //     var token1 = new JwtTokenBuilder()
                //    .AddClaims(GetClaim(userData, TenantName, RoleName, userModel.IsSubscribed))
                //    .Build();
                //     userModel.AccessToken = token1.Value;
                Tokens JwtTokenData = await jwtManager.GenerateJWTTokens(userData.Email, userModel.IsSubscribed);
                if (JwtTokenData != null)
                {
                    userModel.AccessToken = JwtTokenData.Access_Token;
                }
                responsemodel = _mapper.Map<UserExternalLoginResponse>(userModel);
                return new OperationResult<UserExternalLoginResponse>(true, System.Net.HttpStatusCode.OK, "", responsemodel);

            }
            else
            {
                responsemodel = _mapper.Map<UserExternalLoginResponse>(userModel);
                return new OperationResult<UserExternalLoginResponse>(false, System.Net.HttpStatusCode.OK, "Invalid External Authentication.", responsemodel);
            }
            // if (userData == null)
            //     return new OperationResult<UserDto>(false, "Invalid External Authentication.", userModel);
            //check for the Locked out account

        }

        [AllowAnonymous]
        [HttpGet("{Id}")]
        public async Task<FileResult> ProfileImageView(int Id)
        {
            var userDetailsObj = _userService.GetUserById(Id);
            // var dirPath = _hostingEnvironment.WebRootPath + "\\ProfileImageUpload\\Original";
            var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.OriginalUserProfileDirPath;
            var filePath = dirPath + "\\" + "default-profile.jpg";
            filePath = dirPath + "\\" + userDetailsObj.ProfileImage;
            if (System.IO.File.Exists(filePath))
            {
                var bytes = await System.IO.File.ReadAllBytesAsync(filePath);

                // return File (b,GetMimeTypes(userDetailsObj.ProfileImage), userDetailsObj.ProfileImage);
                return File(bytes, Common.GetMimeTypes(userDetailsObj.ProfileImage), userDetailsObj.ProfileImage);
            }
            else
            {
                filePath = dirPath + "\\" + "default-profile.jpg";
                var bytes = await System.IO.File.ReadAllBytesAsync(filePath);

                // return File (b,GetMimeTypes(userDetailsObj.ProfileImage), userDetailsObj.ProfileImage);
                return File(bytes, Common.GetMimeTypes(userDetailsObj.ProfileImage), userDetailsObj.ProfileImage);
            }
            //  Byte[] b = System.IO.File.ReadAllBytes (filePath);

        }

        [AllowAnonymous]
        [HttpPut]
        public async Task<OperationResult<UserUpdateProfileResponse>> UpdateUserProfile([FromForm] UserUpdateProfileRequest currentUser)
        {
            User userObj = new User();
            UserDto userDto = new UserDto();
            var requestmodel = _mapper.Map<UserDto>(currentUser);
            var currentUserObj = _mapper.Map<User>(requestmodel);
            userObj = _userService.GetUserById(currentUserObj.Id);
            if (userObj != null)
            {
                if (!string.IsNullOrEmpty(requestmodel.Password) && requestmodel.IsSignUp)
                {
                    var user = await signupUser.ChangePassword(userObj, requestmodel.Password);
                }
                if (requestmodel.File != null)
                {

                    // // full path to file in temp location
                    // var dirPath = _hostingEnvironment.WebRootPath + "\\ProfileImageUpload";
                    // var filePath = dirPath + "\\" + userObj.ProfileImage;

                    // if (!Directory.Exists(dirPath))
                    // {
                    //     Directory.CreateDirectory(dirPath);
                    // }

                    // if (System.IO.File.Exists(filePath))
                    // {
                    //     System.IO.File.Delete(Path.Combine(filePath));
                    // }

                    // var fileName = string.Concat(
                    //     Path.GetFileNameWithoutExtension(currentUser.File.FileName + "_" + currentUser.Id),
                    //     DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                    //     Path.GetExtension(currentUser.File.FileName)
                    // );
                    // filePath = dirPath + "\\" + fileName;

                    // using (var oStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                    // {
                    //     await currentUser.File.CopyToAsync(oStream);
                    // }
                    // userObj.ProfileImage = fileName;
                    if (requestmodel.File != null && requestmodel.File.Length > 0)
                    {
                        // var dirPath = _hostingEnvironment.WebRootPath + "/ProfileImageUpload/Original";
                        // var dirResizedPath = _hostingEnvironment.WebRootPath + "/ProfileImageUpload/Resized";
                        var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.OriginalUserProfileDirPath;
                        var dirResizedPath = _hostingEnvironment.WebRootPath + OneClappContext.ReSizedUserProfileDirPath;

                        if (!Directory.Exists(dirPath))
                        {
                            Directory.CreateDirectory(dirPath);
                        }
                        if (!Directory.Exists(dirResizedPath))
                        {
                            Directory.CreateDirectory(dirResizedPath);
                        }

                        if (System.IO.File.Exists(dirPath + "/" + userObj.ProfileImage))
                        {
                            System.IO.File.Delete(dirPath + "/" + userObj.ProfileImage);
                        }
                        if (System.IO.File.Exists(dirResizedPath + "/" + userObj.ProfileImage))
                        {
                            System.IO.File.Delete(dirResizedPath + "/" + userObj.ProfileImage);
                        }

                        var fileName = string.Concat(
                                Path.GetFileNameWithoutExtension(requestmodel.File.FileName + "_" + requestmodel.Id),
                                DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                                Path.GetExtension(requestmodel.File.FileName)
                            );


                        var filePath = dirPath + "/" + fileName;
                        var extension = requestmodel.File.FileName.Split('.');

                        if (OneClappContext.ClamAVServerIsLive)
                        {
                            ScanDocument scanDocumentObj = new ScanDocument();
                            bool fileStatus = await scanDocumentObj.ScanDocumentWithClam(requestmodel.File);
                            if (fileStatus)
                            {
                                return new OperationResult<UserUpdateProfileResponse>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                            }
                        }

                        using (var oStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                        {
                            await requestmodel.File.CopyToAsync(oStream);
                        }

                        // Resize Image
                        if (extension != null && (extension[1] == "png" || extension[1] == "jpg" || extension[1] == "jpeg"))
                        {
                            var pathWithFileName = dirResizedPath + "/" + fileName;
                            var resizeWidth = 150;
                            var resizeHeight = 150;
                            Utility.ResizeImage(resizeHeight, resizeWidth, filePath, pathWithFileName);
                        }

                        userObj.ProfileImage = fileName;
                    }

                }
                userObj.FirstName = currentUserObj.FirstName;
                userObj.LastName = currentUserObj.LastName;
                userObj.UserName = currentUserObj.UserName;
                userObj.Address = currentUserObj.Address;
                userObj.PhoneNo = currentUserObj.PhoneNo;
                userObj.DialCode = currentUserObj.DialCode;
                // userObj.UpdatedBy = currentUserObj.UpdatedBy;
                var updatedUser = await _userService.UpdateUser(userObj, false);
                var reponseuser = _mapper.Map<UserUpdateProfileResponse>(updatedUser);
                if (updatedUser != null)
                {
                    var data = await GenerateRefreshToken(updatedUser.Id);
                    userDto = _mapper.Map<UserDto>(updatedUser);
                    if (data != null && !string.IsNullOrEmpty(data.AccessToken))
                    {
                        userDto.AccessToken = data.AccessToken;
                        userDto.IsSubscribed = data.IsSubscribed;
                        if (string.IsNullOrEmpty(userDto.RoleName))
                        {
                            userDto.RoleName = data.RoleName;
                        }
                        if (string.IsNullOrEmpty(userDto.Tenant))
                        {
                            userDto.Tenant = data.Tenant;
                        }
                    }
                    reponseuser = _mapper.Map<UserUpdateProfileResponse>(userDto);
                    return new OperationResult<UserUpdateProfileResponse>(true, System.Net.HttpStatusCode.OK, "Updated Successfully", reponseuser);
                }
                else
                {
                    return new OperationResult<UserUpdateProfileResponse>(false, System.Net.HttpStatusCode.OK, "Something went wrong", reponseuser);
                }
            }
            else
            {
                return new OperationResult<UserUpdateProfileResponse>(false, System.Net.HttpStatusCode.OK, "User not found");
            }
        }


        [AllowAnonymous]
        [HttpPut]
        public async Task<OperationResult<UserDetailResponse>> Account([FromForm] UserUpdateProfileRequest currentUser)
        {
            User userObj = new User();
            UserDto userDto = new UserDto();
            var requestmodel = _mapper.Map<UserDto>(currentUser);
            var currentUserObj = _mapper.Map<User>(requestmodel);
            userObj = _userService.GetUserById(currentUserObj.Id);
            if (userObj != null)
            {
                if (!string.IsNullOrEmpty(requestmodel.Password) && requestmodel.IsSignUp)
                {
                    var user = await signupUser.ChangePassword(userObj, requestmodel.Password);
                }
                if (requestmodel.File != null)
                {

                    // // full path to file in temp location
                    // var dirPath = _hostingEnvironment.WebRootPath + "\\ProfileImageUpload";
                    // var filePath = dirPath + "\\" + userObj.ProfileImage;

                    // if (!Directory.Exists(dirPath))
                    // {
                    //     Directory.CreateDirectory(dirPath);
                    // }

                    // if (System.IO.File.Exists(filePath))
                    // {
                    //     System.IO.File.Delete(Path.Combine(filePath));
                    // }

                    // var fileName = string.Concat(
                    //     Path.GetFileNameWithoutExtension(currentUser.File.FileName + "_" + currentUser.Id),
                    //     DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                    //     Path.GetExtension(currentUser.File.FileName)
                    // );
                    // filePath = dirPath + "\\" + fileName;

                    // using (var oStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                    // {
                    //     await currentUser.File.CopyToAsync(oStream);
                    // }
                    // userObj.ProfileImage = fileName;
                    if (requestmodel.File != null && requestmodel.File.Length > 0)
                    {
                        // var dirPath = _hostingEnvironment.WebRootPath + "/ProfileImageUpload/Original";
                        // var dirResizedPath = _hostingEnvironment.WebRootPath + "/ProfileImageUpload/Resized";
                        var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.OriginalUserProfileDirPath;
                        var dirResizedPath = _hostingEnvironment.WebRootPath + OneClappContext.ReSizedUserProfileDirPath;

                        if (!Directory.Exists(dirPath))
                        {
                            Directory.CreateDirectory(dirPath);
                        }
                        if (!Directory.Exists(dirResizedPath))
                        {
                            Directory.CreateDirectory(dirResizedPath);
                        }

                        if (System.IO.File.Exists(dirPath + "/" + userObj.ProfileImage))
                        {
                            System.IO.File.Delete(dirPath + "/" + userObj.ProfileImage);
                        }
                        if (System.IO.File.Exists(dirResizedPath + "/" + userObj.ProfileImage))
                        {
                            System.IO.File.Delete(dirResizedPath + "/" + userObj.ProfileImage);
                        }

                        var fileName = string.Concat(
                                Path.GetFileNameWithoutExtension(requestmodel.File.FileName + "_" + requestmodel.Id),
                                DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                                Path.GetExtension(requestmodel.File.FileName)
                            );


                        var filePath = dirPath + "/" + fileName;
                        var extension = requestmodel.File.FileName.Split('.');

                        if (OneClappContext.ClamAVServerIsLive)
                        {
                            ScanDocument scanDocumentObj = new ScanDocument();
                            bool fileStatus = await scanDocumentObj.ScanDocumentWithClam(requestmodel.File);
                            if (fileStatus)
                            {
                                return new OperationResult<UserDetailResponse>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                            }
                        }

                        using (var oStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                        {
                            await requestmodel.File.CopyToAsync(oStream);
                        }

                        // Resize Image
                        if (extension != null && (extension[1] == "png" || extension[1] == "jpg" || extension[1] == "jpeg"))
                        {
                            var pathWithFileName = dirResizedPath + "/" + fileName;
                            var resizeWidth = 150;
                            var resizeHeight = 150;
                            Utility.ResizeImage(resizeHeight, resizeWidth, filePath, pathWithFileName);
                        }

                        userObj.ProfileImage = fileName;
                    }

                }
                userObj.FirstName = currentUserObj.FirstName;
                userObj.LastName = currentUserObj.LastName;
                userObj.UserName = currentUserObj.UserName;
                userObj.Address = currentUserObj.Address;
                userObj.PhoneNo = currentUserObj.PhoneNo;
                userObj.DialCode = currentUserObj.DialCode;
                // userObj.UpdatedBy = currentUserObj.UpdatedBy;
                var updatedUser = await _userService.UpdateUser(userObj, false);
                var reponseuser = _mapper.Map<UserDetailResponse>(updatedUser);
                if (updatedUser != null)
                {
                    var data = await GenerateRefreshToken(updatedUser.Id);
                    userDto = _mapper.Map<UserDto>(updatedUser);
                    if (data != null && !string.IsNullOrEmpty(data.AccessToken))
                    {
                        userDto.AccessToken = data.AccessToken;
                        userDto.IsSubscribed = data.IsSubscribed;
                        if (string.IsNullOrEmpty(userDto.RoleName))
                        {
                            userDto.RoleName = data.RoleName;
                        }
                        if (string.IsNullOrEmpty(userDto.Tenant))
                        {
                            userDto.Tenant = data.Tenant;
                        }
                    }
                    var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                    if (userDto.ProfileImage == null)
                    {
                        userDto.Avatar = null;
                    }
                    else
                    {
                        userDto.Avatar = OneClappContext.CurrentURL + "User/ProfileImageView/" + userDto.Id + "?" + Timestamp;
                    }
                    reponseuser = _mapper.Map<UserDetailResponse>(userDto);

                    return new OperationResult<UserDetailResponse>(true, System.Net.HttpStatusCode.OK, "Updated Successfully", reponseuser);
                }
                else
                {
                    return new OperationResult<UserDetailResponse>(false, System.Net.HttpStatusCode.OK, "Something went wrong", reponseuser);
                }
            }
            else
            {
                return new OperationResult<UserDetailResponse>(false, System.Net.HttpStatusCode.OK, "User not found");
            }
        }

        // [AllowAnonymous]
        // [HttpDelete("deleteUserProfile")]
        // public async Task<OperationResult<UserDto>> deleteUser(int userId)
        // {
        //     var userObject = _userService.GetUserById(userId);
        //     if (userObject != null)
        //     {
        //         var deleteUserObj = _userService.DeleteUser(userId);
        //         if (deleteUserObj != null)
        //         {
        //             var deleteUserObjDto = _mapper.Map<UserDto>(deleteUserObj);
        //             return new OperationResult<UserDto>(true, "Deleted Successfully", deleteUserObjDto);

        //         }
        //         else
        //         {
        //             return new OperationResult<UserDto>(false, "Something went wrong");
        //         }
        //     }
        //     else
        //     {
        //         return new OperationResult<UserDto>(false, "User not found");
        //     }
        // }

        [AllowAnonymous]
        //[HttpPost("deleteUserProfile")]
        [HttpDelete]
        public async Task<OperationResult<UserDto>> Remove([FromBody] UserDto model)
        {
            var userObject = _userService.GetUserById(model.Id);
            if (userObject != null)
            {
                userObject = _mapper.Map<User>(model);
                var deleteUserObj = _userService.DeleteUserModel(userObject);

                if (deleteUserObj != null)
                {
                    var deleteUserObjDto = _mapper.Map<UserDto>(deleteUserObj);
                    return new OperationResult<UserDto>(true, System.Net.HttpStatusCode.OK, "Deleted Successfully", deleteUserObjDto);
                }
                else
                {
                    return new OperationResult<UserDto>(false, System.Net.HttpStatusCode.OK, "Something went wrong", model);
                }

            }
            else
            {
                return new OperationResult<UserDto>(false, System.Net.HttpStatusCode.OK, "User not found", model);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<OperationResult<UserDto>> UpdateUser([FromBody] UserVM currentUser)
        {
            User userObj = new User();
            UserDto userModel = new UserDto();
            userObj = _userService.GetUserById(currentUser.Id);
            // currentUser = AuthData.GetAll (this.User);
            if (currentUser.Tenant != null && currentUser.Tenant.ToLower() == "admin")
            {
                var tenantObj = _tenantService.GetAdmin();
                if (userObj.TenantId == null)
                {
                    userObj.TenantId = tenantObj.TenantId;
                    var updatedUser = await _userService.UpdateUser(userObj, true);
                }
                var RoleName = "";
                if (userObj.RoleId != null)
                {
                    var roleObj = _roleService.GetRoleById(userObj.RoleId.Value);
                    if (roleObj != null)
                        RoleName = roleObj.RoleName;
                }
                if (tenantObj.TenantId == userObj.TenantId)
                {
                    // var token = new JwtTokenBuilder()
                    //     .AddClaims(GetClaim(userObj, currentUser.Tenant, RoleName))
                    //     .Build();
                    // userModel.AccessToken = token.Value;
                    Tokens JwtTokenData = await jwtManager.GenerateJWTTokens(userObj.Email, userModel.IsSubscribed);
                    if (JwtTokenData != null)
                    {
                        userModel.AccessToken = JwtTokenData.Access_Token;
                    }
                    userModel.Tenant = currentUser.Tenant;
                    return new OperationResult<UserDto>(true, System.Net.HttpStatusCode.OK, "", userModel);
                }
            }
            if (currentUser.Tenant != null && currentUser.Tenant != "" && currentUser.ApiKey != null && currentUser.ApiKey != "")
            {
                var response = await _userService.GetWeClappUser(currentUser.ApiKey, currentUser.Tenant);
                if (response != null)
                {
                    // userObj = _userService.GetUserById (currentUser.Id);
                    if (userObj != null)
                    {
                        userObj.WeClappToken = currentUser.ApiKey;
                        userObj.WeClappUserId = Convert.ToInt32(response.Id);
                        var tenant = _tenantService.GetTenant(currentUser.Tenant);
                        if (tenant != null)
                        {
                            userObj.TenantId = tenant.TenantId;
                            var userRole = _roleService.GetRole("TenantUser");
                            if (userRole != null && userObj.RoleId == null)
                            {
                                userObj.RoleId = userRole.RoleId;
                            }
                        }
                        else
                        {
                            TenantDto tenantDto = new TenantDto();
                            tenantDto.TenantName = currentUser.Tenant;
                            var tanantObj = await _tenantService.CheckInsertOrUpdate(tenantDto);
                            if (tanantObj != null)
                            {
                                userObj.TenantId = tanantObj.TenantId;
                                var userRole = _roleService.GetRole("TenantAdmin");
                                if (userRole != null && userObj.RoleId == null)
                                {
                                    userObj.RoleId = userRole.RoleId;
                                }
                            }

                            TenantActivity tenantActivity = new TenantActivity();
                            tenantActivity.TenantId = tanantObj.TenantId;
                            tenantActivity.UserId = userObj.Id;
                            tenantActivity.Activity = "Created tenant";
                            var tenantActivityObj = await _tenantActivityService.CheckInsertOrUpdate(tenantActivity);

                        }

                        var RoleName = "";
                        if (userObj.RoleId != null)
                        {
                            var roleObj = _roleService.GetRoleById(userObj.RoleId.Value);
                            if (roleObj != null)
                                RoleName = roleObj.RoleName;
                        }

                        var updatedUser = await _userService.UpdateUser(userObj, false);
                        // var token = new JwtTokenBuilder()
                        //     .AddClaims(GetClaim(userObj, currentUser.Tenant, RoleName))
                        //     .Build();

                        userModel = _mapper.Map<UserDto>(userObj);
                        Tokens JwtTokenData = await jwtManager.GenerateJWTTokens(userObj.Email, userModel.IsSubscribed);
                        if (JwtTokenData != null)
                        {
                            userModel.AccessToken = JwtTokenData.Access_Token;
                        }
                        // userModel.AccessToken = token.Value;
                        userModel.Tenant = currentUser.Tenant;
                        return new OperationResult<UserDto>(true, System.Net.HttpStatusCode.OK, "", userModel);
                    }
                }
            }
            else if (currentUser.CreatedBy != null && currentUser.Tenant == null)
            {
                var RoleName = "";
                if (userObj.RoleId != null)
                {
                    var roleObj = _roleService.GetRoleById(userObj.RoleId.Value);
                    if (roleObj != null)
                        RoleName = roleObj.RoleName;
                }
                // var userUpdateObj = _mapper.Map<User>(currentUser);

                userObj.FirstName = currentUser.FirstName;
                userObj.LastName = currentUser.LastName;
                userObj.RoleId = currentUser.RoleId;
                userObj.TenantId = currentUser.TenantId;
                userObj.Address = currentUser.Address;
                userObj.DialCode = currentUser.DialCode;
                userObj.PhoneNo = currentUser.PhoneNo;
                userObj.UpdatedBy = currentUser.UpdatedBy;
                var updatedUser = await _userService.UpdateUser(userObj, false);
                // var token = new JwtTokenBuilder()
                //     .AddClaims(GetClaim(userObj, "", RoleName, userModel.IsSubscribed))
                //     .Build();

                userModel = _mapper.Map<UserDto>(userObj);
                // userModel.AccessToken = token.Value;
                Tokens JwtTokenData = await jwtManager.GenerateJWTTokens(userObj.Email, userModel.IsSubscribed);
                if (JwtTokenData != null)
                {
                    userModel.AccessToken = JwtTokenData.Access_Token;
                }
                return new OperationResult<UserDto>(true, System.Net.HttpStatusCode.OK, "", userModel);
            }
            return new OperationResult<UserDto>(false, System.Net.HttpStatusCode.OK, "", userModel);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<OperationResult<ResetPasswordDto>> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (model != null)
            {
                var result = await signupUser.ResetPassword(model);
                return new OperationResult<ResetPasswordDto>(result.IsEmailValid, System.Net.HttpStatusCode.OK, result.EmailErrorMessage, result);
            }
            else
            {
                return new OperationResult<ResetPasswordDto>(false, System.Net.HttpStatusCode.OK, "Error", model);
            }
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<UserGetAllUsersByTenantResponse>>> ListByTenant()
        {

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var users = _userService.GetAllUsersByTenantAdmin(TenantId);
            List<UserDto> userDtos = new List<UserDto>();
            userDtos = _mapper.Map<List<UserDto>>(users);
            foreach (var userObj in userDtos)
            {
                if (userObj.FirstName != null)
                {
                    userObj.ShortName = userObj.FirstName.Substring(0, 1);
                }
                if (userObj.LastName != null)
                {
                    userObj.ShortName = userObj.ShortName + userObj.LastName.Substring(0, 1);
                }

            }
            var responseDto = _mapper.Map<List<UserGetAllUsersByTenantResponse>>(userDtos);
            return new OperationResult<List<UserGetAllUsersByTenantResponse>>(true, System.Net.HttpStatusCode.OK, "", responseDto);
        }

        [SwaggerOperation(Description = "getting user list for user drop down")]
        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<UserDropDownListResponse>>> DropDownList()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var users = _userService.GetAllUsersByTenantAdmin(TenantId);
            List<UserDropDownListResponse> userDropDownListResponseList = new List<UserDropDownListResponse>();
            //userDtos = _mapper.Map<List<UserDto>>(users);
            foreach (var userObj in users)
            {
                UserDropDownListResponse userDropDownListResponseObj = new UserDropDownListResponse();
                userDropDownListResponseObj.Id = userObj.Id;
                if (userObj.FirstName != null && userObj.FirstName != null)
                {
                    userDropDownListResponseObj.UserName = userObj.FirstName + " " + userObj.LastName;
                }
                else
                {
                    userDropDownListResponseObj.UserName = userObj.Email;
                }
                var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                if (userObj.ProfileImage == null)
                {
                    userDropDownListResponseObj.ProfileURL = null;
                }
                else
                {
                    userDropDownListResponseObj.ProfileURL = OneClappContext.CurrentURL + "User/ProfileImageView/" + userObj.Id + "?" + Timestamp;
                }
                userDropDownListResponseList.Add(userDropDownListResponseObj);
            }
            return new OperationResult<List<UserDropDownListResponse>>(true, System.Net.HttpStatusCode.OK, "", userDropDownListResponseList);
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

        [HttpGet]
        [AllowAnonymous]
        public OperationResult<List<Language>> Languages()
        {
            var languages = _languageService.GetAllActive();
            return new OperationResult<List<Language>>(true, System.Net.HttpStatusCode.OK, "", languages);
        }

        // Get User Email By TempGuid
        [AllowAnonymous]
        [HttpPost]
        public OperationResult<UsergetUserEmailResponse> UserEmail([FromBody] UsergetUserEmailRequest Model)
        {
            var result = new User();
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            var requestmodel = _mapper.Map<UserDto>(Model);
            var userModel = _userService.GetUserByTempGuid(requestmodel.TempGuid);
            var responseusermodel = _mapper.Map<UsergetUserEmailResponse>(userModel);
            var responseresult = _mapper.Map<UsergetUserEmailResponse>(result);
            if (userModel != null)
            {
                return new OperationResult<UsergetUserEmailResponse>(true, System.Net.HttpStatusCode.OK, "", responseusermodel);
            }
            else
            {
                return new OperationResult<UsergetUserEmailResponse>(false, System.Net.HttpStatusCode.OK, CommonMessage.DefaultErrorMessage, responseresult);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<OperationResult<UserEmailVerificationResponse>> EmailVerification([FromBody] UserEmailVerificationRequest Model)
        {
            var requestmodel = _mapper.Map<UserDto>(Model);
            var data = await signupUser.VerifyUser(requestmodel);
            UserDto userModel = new UserDto();
            if (requestmodel.IsSignUp)
            {

                UserEmailVerificationResponse responsemodel = new UserEmailVerificationResponse();
                string address = data.Email;
                string host;

                // using Split
                host = address.Split('@')[1];
                host = host.Split('.')[0];

                Random generator = new Random();
                String r = generator.Next(0, 1000000).ToString("D6");

                var teamName = host + "_" + r;

                // using Split with maximum number of substrings (more explicit)
                // host = address.Split(new char[] { '@' }, 2)[1];
                // host = host.Split(new char[] { '.' }, 2)[1];

                var externalRoleObj = _roleService.GetRole("ExternalUser");
                User user = _mapper.Map<User>(data);
                string TenantName = "";
                if (user != null)
                {
                    if (user.IsEmailVerified == false)
                    {
                        responsemodel = _mapper.Map<UserEmailVerificationResponse>(userModel);
                        return new OperationResult<UserEmailVerificationResponse>(false, System.Net.HttpStatusCode.OK, "Kindly verify your email first.", responsemodel);
                    }

                    userModel = _mapper.Map<UserDto>(user);

                    if (user.TenantId != null)
                    {
                        var tenantObj = _tenantService.GetTenantById(user.TenantId.Value);
                        if (tenantObj != null)
                        {
                            TenantName = tenantObj.TenantName;
                        }
                    }
                    var updatedUser = await _userService.UpdateUser(user, true);
                    userModel.Tenant = TenantName;

                    // Generate token
                    var RoleName = "";
                    if (userModel.RoleId != null)
                    {
                        var roleObj = _roleService.GetRoleById(userModel.RoleId.Value);
                        if (roleObj != null)
                            RoleName = roleObj.RoleName;
                    }

                    userModel.RoleName = RoleName;

                    var userSubscriptionObj = _userSubscriptionService.GetByUser(userModel.Id);
                    var mollieSubscriptionObj = _mollieSubscriptionService.GetByUser(userModel.Id);

                    if (userSubscriptionObj != null)
                    {
                        var subscribedOn = userSubscriptionObj.CreatedOn.Value;
                        if (userSubscriptionObj.SubscribedOn != null)
                        {
                            subscribedOn = userSubscriptionObj.SubscribedOn.Value;
                        }
                        if (userSubscriptionObj.UpdatedOn != null)
                        {
                            subscribedOn = userSubscriptionObj.UpdatedOn.Value;
                        }
                        var Today = DateTime.Today;
                        TimeSpan diff = Today.Date - subscribedOn.Date;

                        if (userSubscriptionObj.SubscriptionType != null)
                        {
                            if (userSubscriptionObj.SubscriptionType.Name.ToLower() == "monthly")
                            {
                                if (diff.Days <= 30)
                                {
                                    if (!string.IsNullOrEmpty(mollieSubscriptionObj.PaymentId))
                                    {
                                        PaymentResponse molliPaymentObj = await _paymentStorageClient.GetPayment(mollieSubscriptionObj.PaymentId);
                                        if (molliPaymentObj != null && molliPaymentObj.Status.ToLower() == "paid")
                                        {
                                            userModel.IsSubscribed = true;
                                        }
                                        else
                                        {
                                            userModel.IsSubscribed = false;
                                        }
                                    }
                                    else
                                    {
                                        userModel.IsSubscribed = true;
                                    }
                                }
                            }
                            else if (userSubscriptionObj.SubscriptionType.Name.ToLower() == "yearly")
                            {
                                if (diff.Days <= 365)
                                {
                                    // userModel.IsSubscribed = true;
                                    if (!string.IsNullOrEmpty(mollieSubscriptionObj.PaymentId))
                                    {
                                        PaymentResponse molliPaymentObj = await _paymentStorageClient.GetPayment(mollieSubscriptionObj.PaymentId);
                                        if (molliPaymentObj != null && molliPaymentObj.Status.ToLower() == "paid")
                                        {
                                            userModel.IsSubscribed = true;
                                        }
                                        else
                                        {
                                            userModel.IsSubscribed = false;
                                        }
                                    }
                                    else
                                    {
                                        userModel.IsSubscribed = true;
                                    }
                                }
                                else
                                {
                                    userModel.IsSubscribed = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (userModel.CreatedOn != null)
                        {
                            DateTime createdOn = userModel.CreatedOn.Value;
                            var Today = DateTime.Today;
                            TimeSpan diff = Today.Date - createdOn.Date;
                            if (diff.Days <= 30)
                            {
                                userModel.IsSubscribed = true;
                            }
                        }

                    }

                    if (user.CreatedBy == null)
                    {
                        var mailBoxTeamObj = _mailBoxTeamService.GetByUser(user.Id);
                        if (mailBoxTeamObj == null)
                        {
                            MailBoxTeamDto mailBoxTeamDto = new MailBoxTeamDto();
                            mailBoxTeamDto.Name = teamName;
                            mailBoxTeamDto.CreatedBy = user.Id;
                            mailBoxTeamDto.TenantId = user.TenantId;
                            var AddUpdate = await _mailBoxTeamService.CheckInsertOrUpdate(mailBoxTeamDto);
                        }
                    }

                    // var token = new JwtTokenBuilder()
                    //    .AddClaims(GetClaim(user, TenantName, RoleName, userModel.IsSubscribed))
                    //    .Build();
                    // userModel.AccessToken = token.Value;
                    Tokens JwtTokenData = await jwtManager.GenerateJWTTokens(user.Email, userModel.IsSubscribed);
                    if (JwtTokenData != null)
                    {
                        userModel.AccessToken = JwtTokenData.Access_Token;
                    }
                    responsemodel = _mapper.Map<UserEmailVerificationResponse>(userModel);
                    return new OperationResult<UserEmailVerificationResponse>(data.IsEmailVerified, System.Net.HttpStatusCode.OK, "", responsemodel);
                }
            }
            var responsedata = _mapper.Map<UserEmailVerificationResponse>(data);
            return new OperationResult<UserEmailVerificationResponse>(data.IsEmailVerified, System.Net.HttpStatusCode.OK, "", responsedata);
        }

        // User change password
        [AllowAnonymous]
        [HttpPost]
        public async Task<OperationResult<UserChangePasswordResponse>> UserChangePassword([FromBody] UserChangePasswordRequest Model)
        {
            // User Table
            var userTable = new User();
            var requestmodel = _mapper.Map<UserDto>(Model);
            var objuser = _userService.GetUserByUserIdAndTempGuid(requestmodel.Id, requestmodel.TempGuid);
            var responsetable = _mapper.Map<UserChangePasswordResponse>(userTable);
            if (objuser != null)
            {
                var user = await signupUser.ChangePassword(objuser, requestmodel.Password);
                // var user = await _userService.UpdatePassword (objuser, Model.Password);
                if (user != null)
                {
                    var responsemodel = _mapper.Map<UserChangePasswordResponse>(user);
                    return new OperationResult<UserChangePasswordResponse>(true, System.Net.HttpStatusCode.OK, "", responsemodel);
                }
            }
            return new OperationResult<UserChangePasswordResponse>(false, System.Net.HttpStatusCode.OK, CommonMessage.DefaultErrorMessage, responsetable);
        }

        // User set password
        [AllowAnonymous]
        [HttpPost]
        public async Task<OperationResult<UserChangePasswordResponse>> UserSetPassword([FromBody] UserChangePasswordRequest Model)
        {
            // User Table
            var userTable = new User();
            var requestmodel = _mapper.Map<UserDto>(Model);
            var objuser = _userService.GetUserByTempGuid(requestmodel.Id, requestmodel.TempGuid);
            var responsetable = _mapper.Map<UserChangePasswordResponse>(userTable);
            if (objuser != null)
            {
                userTable = objuser;
                var user = await signupUser.SetPassword(objuser, requestmodel.Password);
                // var user = await _userService.UpdatePassword (objuser, Model.Password);
                if (user != null)
                {
                    var responsemodel = _mapper.Map<UserChangePasswordResponse>(user);
                    return new OperationResult<UserChangePasswordResponse>(true, System.Net.HttpStatusCode.OK, "", responsemodel);
                }
            }
            return new OperationResult<UserChangePasswordResponse>(false, System.Net.HttpStatusCode.OK, CommonMessage.DefaultErrorMessage, responsetable);
        }

        [Authorize(Roles = "TenantUser,Admin,TenantAdmin,TenantManager,ExternalUser")]
        [HttpGet]
        public OperationResult<UserDetailResponse> Detail()
        {
            var user = new User();

            ClaimsPrincipal claimuser = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(claimuser.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            user = _userService.GetUserById(UserId);
            UserDto userDto = new UserDto();
            userDto = _mapper.Map<UserDto>(user);
            userDto.Name = userDto.UserName;
            var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            if (userDto.ProfileImage == null)
            {
                userDto.Avatar = "assets/images/default-profile.jpg";
            }
            else
            {
                userDto.Avatar = OneClappContext.CurrentURL + "User/ProfileImageView/" + userDto.Id + "?" + Timestamp;
            }
            var responseDto = _mapper.Map<UserDetailResponse>(userDto);
            responseDto.RefreshToken = user.RefreshToken;
            return new OperationResult<UserDetailResponse>(true, System.Net.HttpStatusCode.OK, "", responseDto);
        }

        // User Email Verification
        [AllowAnonymous]
        [HttpPost]
        public async Task<OperationResult<UserResendOTPResponse>> ResendOTP([FromBody] UserResendOTPRequest Model)
        {
            var requestmodel = _mapper.Map<ResetPasswordDto>(Model);
            ResetPasswordDto data = await signupUser.ResetPassword(requestmodel);
            if (data.IsEmailValid == true)
            {
                var responsedata = _mapper.Map<UserResendOTPResponse>(data);
                return new OperationResult<UserResendOTPResponse>(true, System.Net.HttpStatusCode.OK, "", responsedata);
            }
            else
            {
                var responsedata = _mapper.Map<UserResendOTPResponse>(data);
                return new OperationResult<UserResendOTPResponse>(false, System.Net.HttpStatusCode.OK, data.EmailErrorMessage, responsedata);
            }

        }

        [Authorize(Roles = "TenantUser,Admin,TenantAdmin,TenantManager,ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<UserInviteResponse>> Invite([FromBody] UserInviteRequest Model)
        {
            ClaimsPrincipal claimuser = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(claimuser.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(claimuser.FindFirst("TenantId").Value);

            UserDto userDto1 = new UserDto();
            List<User> UserList = new List<User>();
            UserList = _userService.GetAll();
            var TenantUserRole = _roleService.GetRole("TenantUser");
            var requestmodel = _mapper.Map<InviteUserDto>(Model);
            foreach (var emailObj in requestmodel.Emails)
            {
                var isExistData = UserList.Where(t => t.Email == emailObj.Email).FirstOrDefault();
                if (isExistData == null)
                {
                    UserDto userDto = new UserDto();
                    userDto.Email = emailObj.Email;
                    userDto.CreatedBy = UserId;
                    userDto.TenantId = TenantId;
                    if (TenantUserRole != null)
                    {
                        userDto.RoleId = TenantUserRole.RoleId;
                    }

                    var AddUpdate = await signupUser.ProcessInviteUser(userDto);
                }
            }
            var teamMates = UserList.Where(t => t.Id == requestmodel.UserId || t.CreatedBy == requestmodel.UserId).ToList();
            var userIds = teamMates.Select(t => t.Id).ToList();
            await _hubContext.Clients.All.OnInviteUserEvent(userIds);
            var reposneDtos = _mapper.Map<UserInviteResponse>(userDto1);
            return new OperationResult<UserInviteResponse>(true, System.Net.HttpStatusCode.OK, "", reposneDtos);
        }

        [Authorize]
        [HttpGet("{email}")]
        public bool IsExistUser(string email)
        {
            var isExistData = _userService.GetUserByEmail(email);
            if (isExistData != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task<UserDto> GenerateRefreshToken(int UserId)
        {
            UserDto userDto = new UserDto();
            User userObj = _userService.GetUserById(UserId);
            string TenantName = "";
            if (userObj != null)
            {
                userDto = _mapper.Map<UserDto>(userObj);
                if (userObj.LastLoggedIn != null)
                {
                    if (userObj.TenantId != null)
                    {
                        var tenantObj = _tenantService.GetTenantById(userObj.TenantId.Value);
                        if (tenantObj != null)
                        {
                            TenantName = tenantObj.TenantName;
                        }
                    }
                }
                // var updatedUser = await _userService.UpdateUser(userObj, true);
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

                if (userSubscriptionObj != null)
                {
                    var subscribedOn = userSubscriptionObj.CreatedOn.Value;
                    if (userSubscriptionObj.SubscribedOn != null)
                    {
                        subscribedOn = userSubscriptionObj.SubscribedOn.Value;
                    }
                    if (userSubscriptionObj.UpdatedOn != null)
                    {
                        subscribedOn = userSubscriptionObj.UpdatedOn.Value;
                    }
                    var Today = DateTime.Today;
                    TimeSpan diff = Today.Date - subscribedOn.Date;

                    if (userSubscriptionObj.SubscriptionType != null)
                    {
                        if (userSubscriptionObj.SubscriptionType.Name.ToLower() == "monthly")
                        {
                            if (diff.Days <= 30)
                            {
                                if (!string.IsNullOrEmpty(mollieSubscriptionObj.PaymentId))
                                {
                                    PaymentResponse molliPaymentObj = await _paymentStorageClient.GetPayment(mollieSubscriptionObj.PaymentId);
                                    if (molliPaymentObj != null && molliPaymentObj.Status.ToLower() == "paid")
                                    {
                                        userDto.IsSubscribed = true;
                                    }
                                    else
                                    {
                                        userDto.IsSubscribed = false;
                                    }
                                }
                                else
                                {
                                    userDto.IsSubscribed = true;
                                }
                            }
                        }
                        else if (userSubscriptionObj.SubscriptionType.Name.ToLower() == "yearly")
                        {
                            if (diff.Days <= 365)
                            {
                                // userModel.IsSubscribed = true;
                                if (!string.IsNullOrEmpty(mollieSubscriptionObj.PaymentId))
                                {
                                    PaymentResponse molliPaymentObj = await _paymentStorageClient.GetPayment(mollieSubscriptionObj.PaymentId);
                                    if (molliPaymentObj != null && molliPaymentObj.Status.ToLower() == "paid")
                                    {
                                        userDto.IsSubscribed = true;
                                    }
                                    else
                                    {
                                        userDto.IsSubscribed = false;
                                    }
                                }
                                else
                                {
                                    userDto.IsSubscribed = true;
                                }
                            }
                            else
                            {
                                userDto.IsSubscribed = false;
                            }
                        }
                    }
                }
                else
                {
                    if (userDto.CreatedOn != null)
                    {
                        DateTime createdOn = userDto.CreatedOn.Value;
                        var Today = DateTime.Today;
                        TimeSpan diff = Today.Date - createdOn.Date;
                        if (diff.Days <= 30)
                        {
                            userDto.IsSubscribed = true;
                        }
                    }

                }



                // var token = new JwtTokenBuilder()
                //    .AddClaims(GetClaim(userObj, TenantName, RoleName, userDto.IsSubscribed))
                //    .Build();
                // userDto.AccessToken = token.Value;
                 Tokens JwtTokenData = await jwtManager.GenerateJWTTokens(userObj.Email, userDto.IsSubscribed);
                    if (JwtTokenData != null)
                    {
                        userDto.AccessToken = JwtTokenData.Access_Token;
                    }
                return userDto;
            }
            else
            {
                return userDto;
            }
        }

        [HttpGet("{Email}")]
        public async Task<OperationResult<string>> RefreshToken(string Email)
        {
            if (!string.IsNullOrEmpty(Email))
            {
                var user = _userService.GetUserByEmail(Email);
                if (user != null)
                {
                    UserDto userDtoObj = await GenerateRefreshToken(user.Id);
                    if (userDtoObj != null)
                    {
                        return new OperationResult<string>(true, System.Net.HttpStatusCode.OK, "", userDtoObj.AccessToken);
                    }
                    else
                    {
                        return new OperationResult<string>(false, System.Net.HttpStatusCode.OK, "Something went to wrong", "");
                    }
                }
                else
                {
                    return new OperationResult<string>(false, System.Net.HttpStatusCode.OK, "User not found", "");
                }
            }
            else
            {
                return new OperationResult<string>(false, System.Net.HttpStatusCode.OK, "Please provide email", "");
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<OperationResult<RecapchaDto>> ValidateHCapchaResponse([FromBody] RecapchaDto model)
        {
            var result = await Common.ValidateHCapchaResponse(model);
            return new OperationResult<RecapchaDto>(true, System.Net.HttpStatusCode.OK, "", result);
        }
    }
}