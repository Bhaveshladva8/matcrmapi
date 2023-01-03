using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using matcrm.data.Context;
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
    public class AdminController : Controller
    {

        #region  Service Initialization
        private readonly ITenantService _tenantService;
        private readonly IUserService _userService;
        private readonly IVerificationCodeService _verificationCodeService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEmailLogService _emailLogService;
        private readonly ITenantActivityService _tenantActivityService;
        private readonly ILanguageService _languageService;
        private readonly IRoleService _roleService;
        private readonly ITenantConfigService _tenantConfigService;
        private readonly IEmailProviderService _emailProviderService;
        private readonly IEmailConfigService _emailConfigService;
        private readonly IUserERPSystemService _userERPSystemService;
        private SignupUser signupUser;
        private SendEmail sendEmail;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;

        #endregion

        #region  Constructor
        public AdminController(ITenantService tenantService,
            IUserService userService,
            IVerificationCodeService verificationCodeService,
            IEmailTemplateService emailTemplateService,
            IEmailLogService emailLogService,
            ITenantActivityService tenantActivityService,
            ILanguageService languageService,
            IRoleService roleService,
            ITenantConfigService tenantConfigService,
            IEmailProviderService emailProviderService,
            IEmailConfigService emailConfigService,
            IUserERPSystemService userERPSystemService,
            OneClappContext context,
            IMapper mapper)
        {
            _tenantService = tenantService;
            _userService = userService;
            _verificationCodeService = verificationCodeService;
            _emailTemplateService = emailTemplateService;
            _emailLogService = emailLogService;
            _tenantActivityService = tenantActivityService;
            _languageService = languageService;
            _roleService = roleService;
            _tenantConfigService = tenantConfigService;
            _emailProviderService = emailProviderService;
            _emailConfigService = emailConfigService;
            _userERPSystemService = userERPSystemService;
            _mapper = mapper;
            signupUser = new SignupUser(emailTemplateService, emailLogService, verificationCodeService, emailConfigService, emailProviderService, mapper, userService, context);
            sendEmail = new SendEmail(emailTemplateService, emailLogService, emailConfigService, emailProviderService, mapper);
        }

        #endregion

        #region  User

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<OperationResult<List<User>>> Users()
        {
            var userList = _userService.GetAll();
            return new OperationResult<List<User>>(true, System.Net.HttpStatusCode.OK, "", userList);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<OperationResult<User>> AddUser([FromBody] UserDto model)
        {
            User userObj = new User();
            userObj = _mapper.Map<User>(model);
            // var result = await signupUser.ProcessUserSignup (model);
            var result = await _userService.AddUser(userObj, model.Password);
            return new OperationResult<User>(true, System.Net.HttpStatusCode.OK, "User added successfully.", result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<OperationResult<User>> UpdateUser([FromBody] UserDto model)
        {
            User userObj = new User();
            var result = await _userService.UpdateUser(userObj, false);
            return new OperationResult<User>(true, System.Net.HttpStatusCode.OK, "User updated successfully.", result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<OperationResult<User>> RemoveUser([FromBody] UserDto model)
        {
            User userObj = new User();
            userObj = _userService.DeleteUser(model.Id);
            if (userObj != null)
                return new OperationResult<User>(true, System.Net.HttpStatusCode.OK, "User deleted successfully", userObj);
            else
                return new OperationResult<User>(false, System.Net.HttpStatusCode.OK, "User not found!", userObj);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{UserId}")]
        public async Task<OperationResult<User>> BlockUser(int UserId)
        {
            User userObj = new User();
            userObj = _userService.Block(UserId);
            if (userObj == null)
                return new OperationResult<User>(false, System.Net.HttpStatusCode.OK, "User not found", userObj);
            else
                return new OperationResult<User>(true, System.Net.HttpStatusCode.OK, "User blocked successfully", userObj);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{UserId}")]
        public async Task<OperationResult<User>> RevokeUser(int UserId)
        {
            User userObj = new User();

            userObj = _userService.Unblock(UserId);
            if (userObj == null)
                return new OperationResult<User>(false, System.Net.HttpStatusCode.OK, "User not found", userObj);
            else
                return new OperationResult<User>(true, System.Net.HttpStatusCode.OK, "User revoked successfully", userObj);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<OperationResult<List<User>>> RevokeAllUser()
        {
            var userList = _userService.RevokeAllBlocked();
            return new OperationResult<List<User>>(true, System.Net.HttpStatusCode.OK, "Users revoked successfully", userList);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<OperationResult<User>> VerifyUser(UserDto model)
        {

            var userObj = _userService.GetUserByEmailForVerify(model.Email);
            if (userObj != null)
            {
                userObj.IsEmailVerified = true;
                userObj.VerifiedOn = DateTime.UtcNow;
                userObj = await _userService.UpdateUser(userObj, false);

                ClaimsPrincipal user = this.User as ClaimsPrincipal;
                TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);


                await sendEmail.SendEmailVerifiedNotificationByAdmin(userObj.Email, userObj.FirstName + ' ' + userObj.LastName, TenantId);
                // }
                model.IsEmailVerified = true;
                var userObj1 = _mapper.Map<User>(model);
                return new OperationResult<User>(true, System.Net.HttpStatusCode.OK, "User verified", userObj1);


                //     var tenantObj =new Tenant();
                //     if(userObj.CreatedBy != null)
                //     {
                //          tenantObj = _tenantService.GetTenantById(userObj.TenantId.Value);
                //     }
                //     else
                //     {
                //         tenantObj = _tenantService.GetAdmin();
                //     }

                // if(tenantObj.TenantName == "admin")
                // {   
                //     await sendEmail.SendEmailVerifiedNotificationByAdmin (userObj.Email, userObj.FirstName + ' ' + userObj.LastName, userObj.TenantId.Value);
                //       return new OperationResult<User> (true, "User verified successfully", userObj);
                // }

            }

            return new OperationResult<User>(false, System.Net.HttpStatusCode.OK, "User not found ", userObj);
        }

        #endregion

        #region Tenant Methods 

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<OperationResult<List<Tenant>>> Tenants()
        {
            List<Tenant> tenantList = new List<Tenant>();
            tenantList = _tenantService.GetAll();
            return new OperationResult<List<Tenant>>(true, System.Net.HttpStatusCode.OK, "", tenantList);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<OperationResult<Tenant>> AddTenant([FromBody] TenantDto tenantDto)
        {
            Tenant tenantObj = new Tenant();
            if (tenantDto.ApiKey == null || tenantDto.TenantName == null)
            {
                return new OperationResult<Tenant>(false, System.Net.HttpStatusCode.OK, "Tenant not found", tenantObj);
            }
            var obj = await _userService.GetWeClappUser(tenantDto.ApiKey, tenantDto.TenantName);
            if (obj == null)
            {
                return new OperationResult<Tenant>(false, System.Net.HttpStatusCode.OK, "Tenant not found", tenantObj);
            }
            var existingItem = _tenantService.GetTenant(tenantDto.TenantName);
            var adminRoleObj = _roleService.GetRole("Admin");
            var adminUserObj = _userService.GetAdminUser(adminRoleObj.RoleId);

            tenantObj = await _tenantService.CheckInsertOrUpdate(tenantDto);
            TenantActivity tenantActivity = new TenantActivity();
            tenantActivity.UserId = adminUserObj.Id;
            tenantActivity.TenantId = tenantObj.TenantId;

            if (existingItem != null)
            {
                tenantActivity.Activity = "Updated Tenant by Admin";
            }
            else
            {
                tenantActivity.Activity = "Created tenant by Admin";
            }
            var result = await _tenantActivityService.CheckInsertOrUpdate(tenantActivity);

            return new OperationResult<Tenant>(true, System.Net.HttpStatusCode.OK, "Tenant added successfully", tenantObj);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<OperationResult<Tenant>> UpdateTenant([FromBody] TenantDto tenant)
        {
            Tenant tenantObj = new Tenant();
            tenantObj = await _tenantService.CheckInsertOrUpdate(tenant);

            if (tenantObj == null)
            {
                return new OperationResult<Tenant>(false, System.Net.HttpStatusCode.OK, "Tenant not found", tenantObj);
            }
            else
            {
                var adminRoleObj = _roleService.GetRole("Admin");
                var adminUserObj = _userService.GetAdminUser(adminRoleObj.RoleId);

                TenantActivity tenantActivityObj = new TenantActivity();
                tenantActivityObj.UserId = adminUserObj.Id;
                tenantActivityObj.TenantId = tenantObj.TenantId;
                tenantActivityObj.Activity = "Updated Tenant by Admin";

                var result = await _tenantActivityService.CheckInsertOrUpdate(tenantActivityObj);

                return new OperationResult<Tenant>(true, System.Net.HttpStatusCode.OK, "Tenant updated successfully", tenantObj);
            }

        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{TenantId}")]
        public async Task<OperationResult<Tenant>> BlockTenant(int TenantId)
        {
            Tenant tenantObj = new Tenant();
            tenantObj = await _tenantService.Block(TenantId);
            if (tenantObj == null)
            {
                return new OperationResult<Tenant>(false, System.Net.HttpStatusCode.OK, "Tenant not found", tenantObj);
            }
            else
            {
                var adminRoleObj = _roleService.GetRole("Admin");
                var adminUserObj = _userService.GetAdminUser(adminRoleObj.RoleId);

                TenantActivity tenantActivityObj = new TenantActivity();
                tenantActivityObj.UserId = adminUserObj.Id;
                tenantActivityObj.TenantId = tenantObj.TenantId;
                tenantActivityObj.Activity = "Blocked tenant by Admin";

                var result = await _tenantActivityService.CheckInsertOrUpdate(tenantActivityObj);

                return new OperationResult<Tenant>(true, System.Net.HttpStatusCode.OK, "Tenant blocked successfully", tenantObj);
            }

        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{TenantId}")]
        public async Task<OperationResult<Tenant>> RevokeTenant(int TenantId)
        {
            Tenant tenantObj = new Tenant();

            tenantObj = await _tenantService.Revoke(TenantId);

            if (tenantObj == null)
            {
                return new OperationResult<Tenant>(false, System.Net.HttpStatusCode.OK, "Tenant not found", tenantObj);
            }
            else
            {
                var adminRoleObj = _roleService.GetRole("Admin");
                var adminUserObj = _userService.GetAdminUser(adminRoleObj.RoleId);

                TenantActivity tenantActivityObj = new TenantActivity();
                tenantActivityObj.UserId = adminUserObj.Id;
                tenantActivityObj.TenantId = tenantObj.TenantId;
                tenantActivityObj.Activity = "Revoked tenant by Admin";

                var result = await _tenantActivityService.CheckInsertOrUpdate(tenantActivityObj);

                return new OperationResult<Tenant>(true, System.Net.HttpStatusCode.OK, "Tenant revoked successfully", tenantObj);
            }

        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<OperationResult<List<Tenant>>> RevokeAllTenants()
        {
            List<Tenant> tenantList = new List<Tenant>();
            tenantList = _tenantService.RevokeAllBlocked();
            var adminRoleObj = _roleService.GetRole("Admin");
            var adminUserObj = _userService.GetAdminUser(adminRoleObj.RoleId);

            TenantActivity tenantActivityObj = new TenantActivity();
            tenantActivityObj.UserId = adminUserObj.Id;
            tenantActivityObj.Activity = "Revoked all tenant by Admin";

            var result = await _tenantActivityService.CheckInsertOrUpdate(tenantActivityObj);

            return new OperationResult<List<Tenant>>(true, System.Net.HttpStatusCode.OK, "", tenantList);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{TenantId}")]
        public async Task<OperationResult<Tenant>> RemoveTenant(int TenantId)
        {
            Tenant tenantObj = new Tenant();
            tenantObj = _tenantService.DeleteTenant(TenantId);
            if (tenantObj != null)
            {
                var adminRoleObj = _roleService.GetRole("Admin");
                var adminUserObj = _userService.GetAdminUser(adminRoleObj.RoleId);

                TenantActivity tenantActivityObj = new TenantActivity();
                tenantActivityObj.UserId = adminUserObj.Id;
                tenantActivityObj.TenantId = tenantObj.TenantId;
                tenantActivityObj.Activity = "Delete tenant by Admin";

                var result = await _tenantActivityService.CheckInsertOrUpdate(tenantActivityObj);
                return new OperationResult<Tenant>(true, System.Net.HttpStatusCode.OK, "Tenant deleted successfully", tenantObj);
            }
            else
                return new OperationResult<Tenant>(false, System.Net.HttpStatusCode.OK, "Tenant not found!", tenantObj);
        }
        #endregion

        #region Language

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<OperationResult<Language>> AddUpdateLanguage([FromBody] LanguageDto model)
        {
            Language languageObj = new Language();
            if (model != null)
            {
                languageObj = _languageService.CheckInsertOrUpdate(model);
            }
            return new OperationResult<Language>(true, System.Net.HttpStatusCode.OK, "", languageObj);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<OperationResult<Language>> RemoveLanguage([FromBody] LanguageDto model)
        {
            Language languageObj = new Language();
            if (model != null)
            {
                languageObj = _languageService.DeleteLanguage(model.LanguageId);
            }
            return new OperationResult<Language>(true, System.Net.HttpStatusCode.OK, "Language deleted successfully.", languageObj);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<OperationResult<List<Language>>> Languages()
        {
            List<Language> languagesList = new List<Language>();
            languagesList = _languageService.GetAll();
            return new OperationResult<List<Language>>(true, System.Net.HttpStatusCode.OK, "", languagesList);
        }
        #endregion

        #region  Role

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<OperationResult<Role>> AddUpdateRole([FromBody] RoleDto model)
        {
            Role roleObj = new Role();
            if (model != null)
            {
                roleObj = await _roleService.CheckInsertOrUpdate(model);
            }
            return new OperationResult<Role>(true, System.Net.HttpStatusCode.OK, "", roleObj);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<OperationResult<Role>> RemoveRole([FromBody] RoleDto model)
        {
            Role roleObj = new Role();
            if (model != null && model.RoleId != null)
            {
                roleObj = _roleService.DeleteRole(model.RoleId.Value);
            }
            return new OperationResult<Role>(true, System.Net.HttpStatusCode.OK, "Role deleted successfully.", roleObj);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<OperationResult<List<Role>>> Roles()
        {
            List<Role> roleList = new List<Role>();
            roleList = _roleService.GetAllByAdmin();
            return new OperationResult<List<Role>>(true, System.Net.HttpStatusCode.OK, "", roleList);
        }

        #endregion

        #region  EmailTemplate

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<OperationResult<EmailTemplate>> AddUpdateEmailTemplate([FromBody] EmailTemplateDto model)
        {
            EmailTemplate emailTemplateObj = new EmailTemplate();
            emailTemplateObj = await _emailTemplateService.CheckInsertOrUpdate(model);
            return new OperationResult<EmailTemplate>(true, System.Net.HttpStatusCode.OK, "", emailTemplateObj);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<OperationResult<EmailTemplate>> RemoveEmailTemplate([FromBody] EmailTemplateDto model)
        {
            EmailTemplate emailTemplateObj = new EmailTemplate();
            emailTemplateObj = _emailTemplateService.DeleteEmailTemplate(model.EmailTemplateId);
            return new OperationResult<EmailTemplate>(true, System.Net.HttpStatusCode.OK, "Email template deleted successfully.", emailTemplateObj);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<OperationResult<List<EmailTemplate>>> EmailTemplates()
        {
            List<EmailTemplate> emailTemplateList = new List<EmailTemplate>();
            emailTemplateList = _emailTemplateService.GetAll();
            return new OperationResult<List<EmailTemplate>>(true, System.Net.HttpStatusCode.OK, "", emailTemplateList);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{EmailTemplateId}")]
        public async Task<OperationResult<EmailTemplate>> EmailTemplate(long EmailTemplateId)
        {
            EmailTemplate emailTemplatObj = new EmailTemplate();
            emailTemplatObj = _emailTemplateService.GetEmailTemplateById(EmailTemplateId);
            return new OperationResult<EmailTemplate>(true, System.Net.HttpStatusCode.OK, "", emailTemplatObj);
        }

        #endregion

        #region  UserEmailConfig

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<OperationResult<List<EmailConfigDto>>> EmailConfigList()
        {
            List<EmailConfigDto> emailConfigDtoList = new List<EmailConfigDto>();
            var emailConfigs = _emailConfigService.GetAll();
            emailConfigDtoList = _mapper.Map<List<EmailConfigDto>>(emailConfigs);
            return new OperationResult<List<EmailConfigDto>>(true, System.Net.HttpStatusCode.OK, "", emailConfigDtoList);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<OperationResult<EmailConfigDto>> AddUpdateEmailConfig([FromBody] EmailConfigDto model)
        {
            EmailConfigDto emailConfigDto = new EmailConfigDto();
            var AddUpdate = await _emailConfigService.CheckInsertOrUpdate(model);
            model = _mapper.Map<EmailConfigDto>(AddUpdate);
            return new OperationResult<EmailConfigDto>(true, System.Net.HttpStatusCode.OK, "", model);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<OperationResult<EmailConfigDto>> RemoveEmailConfig([FromBody] EmailConfigDto model)
        {
            if (model.Id != null)
            {
                var deleteEmailConfig = _emailConfigService.DeleteEmailConfig(model.Id.Value);
            }
            return new OperationResult<EmailConfigDto>(true, System.Net.HttpStatusCode.OK, "Email config deleted successfully.", model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{ConfigId}")]
        public async Task<OperationResult<EmailConfigDto>> EmailConfig(long ConfigId)
        {
            EmailConfigDto emailConfigDto = new EmailConfigDto();
            var emailConfig = _emailConfigService.GetEmailConfigById(ConfigId);
            emailConfigDto = _mapper.Map<EmailConfigDto>(emailConfig);
            return new OperationResult<EmailConfigDto>(true, System.Net.HttpStatusCode.OK, "", emailConfigDto);
        }

        #endregion

        #region  UserERPSystem

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<OperationResult<List<UserERPSystemDto>>> ERPSystems()
        {
            List<UserERPSystemDto> userERPSystemDtoList = new List<UserERPSystemDto>();
            var userERPSystems = _userERPSystemService.GetAllByAdmin();
            userERPSystemDtoList = _mapper.Map<List<UserERPSystemDto>>(userERPSystems);
            return new OperationResult<List<UserERPSystemDto>>(true, System.Net.HttpStatusCode.OK, "", userERPSystemDtoList);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<OperationResult<UserERPSystemDto>> AddUpdateUserERPSystem([FromBody] UserERPSystemDto model)
        {
            UserERPSystemDto userERPSystemDto = new UserERPSystemDto();
            var AddUpdate = await _userERPSystemService.CheckInsertOrUpdate(model);
            model = _mapper.Map<UserERPSystemDto>(AddUpdate);
            return new OperationResult<UserERPSystemDto>(true, System.Net.HttpStatusCode.OK, "", model);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<OperationResult<UserERPSystemDto>> RemoveUserERPSystem([FromBody] UserERPSystemDto model)
        {
            if (model.Id != null)
            {
                var deleteUserERPSystem = _userERPSystemService.DeleteUserERPSystem(model.Id.Value);
                return new OperationResult<UserERPSystemDto>(true, System.Net.HttpStatusCode.OK, "User Erp system deleted successfully.", model);
            }
            else
            {
                return new OperationResult<UserERPSystemDto>(false, System.Net.HttpStatusCode.OK, "Please provide id", model);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<UserERPSystemDto>> UserERPSystem(long Id)
        {
            UserERPSystemDto userERPSystemDto = new UserERPSystemDto();
            var userERPSystem = _userERPSystemService.GetUserERPSystemById(Id);
            userERPSystemDto = _mapper.Map<UserERPSystemDto>(userERPSystem);
            return new OperationResult<UserERPSystemDto>(true, System.Net.HttpStatusCode.OK, "", userERPSystemDto);
        }

        #endregion

        #region  EmailLog

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<OperationResult<List<EmailLogDto>>> EmailLogList()
        {
            List<EmailLogDto> emailLogDtoList = new List<EmailLogDto>();
            var emailLogs = _emailLogService.GetAllByAdmin();
            emailLogDtoList = _mapper.Map<List<EmailLogDto>>(emailLogs);
            return new OperationResult<List<EmailLogDto>>(true, System.Net.HttpStatusCode.OK, "", emailLogDtoList);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{TenantId}")]
        public async Task<OperationResult<List<EmailLogDto>>> EmailLogByTenant(long TenantId)
        {
            List<EmailLogDto> emailLogDtoList = new List<EmailLogDto>();

            var emailLogs = _emailLogService.GetEmailLogByTenant(TenantId);
            emailLogDtoList = _mapper.Map<List<EmailLogDto>>(emailLogs);
            return new OperationResult<List<EmailLogDto>>(true, System.Net.HttpStatusCode.OK, "", emailLogDtoList);
        }

        #endregion

    }
}