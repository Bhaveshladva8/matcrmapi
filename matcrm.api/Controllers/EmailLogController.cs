using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
using matcrm.data.Context;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class EmailLogController : Controller
    {
        private readonly IUserService _userService;
        private readonly IVerificationCodeService _verificationCodeService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEmailLogService _emailLogService;
        private readonly IEmailConfigService _emailConfigService;
        private readonly IEmailProviderService _emailProviderService;
        private SignupUser signupUser;
        private IMapper _mapper;
        int TenantId = 0;
        public EmailLogController(
            IUserService userService,
            IVerificationCodeService verificationCodeService,
            IEmailTemplateService emailTemplateService,
            IEmailLogService emailLogService,
            IEmailConfigService emailConfigService,
            IEmailProviderService emailProviderService,
            OneClappContext context,
            IMapper mapper
        )
        {
            _userService = userService;
            _verificationCodeService = verificationCodeService;
            _emailTemplateService = emailTemplateService;
            _emailLogService = emailLogService;
            _emailProviderService = emailProviderService;
            _emailConfigService = emailConfigService;
            _mapper = mapper;
            signupUser = new SignupUser(emailTemplateService, emailLogService, verificationCodeService, emailConfigService, emailProviderService, mapper, userService, context);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin,TenantUser")]
        [HttpGet]
        public async Task<OperationResult<List<EmailLogDto>>> BasedOnTenant()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var emailLogsObj = _emailLogService.GetEmailLogByTenant(TenantId);
            if (emailLogsObj != null)
            {
                var emailLogDtoList = _mapper.Map<List<EmailLogDto>>(emailLogsObj);
                return new OperationResult<List<EmailLogDto>>(true, System.Net.HttpStatusCode.OK, "Email log found successfully", emailLogDtoList);
            }
            else
            {
                return new OperationResult<List<EmailLogDto>>(false, System.Net.HttpStatusCode.OK, "Email log not found");
            }

        }

        [Authorize(Roles = "Admin")]

        [HttpGet("{TenantId}")]
        public async Task<OperationResult<List<EmailLogDto>>> ListByAdmin(long? TenantId)
        {

            var emailLogByAdminObj = _emailLogService.GetAllEmailByAdmin(TenantId);            
            //List<EmailLogDto> emailLogDtoList = new List<EmailLogDto>();

            if (emailLogByAdminObj != null)
            {
                var emailLogDtoList = _mapper.Map<List<EmailLogDto>>(emailLogByAdminObj);
                return new OperationResult<List<EmailLogDto>>(true, System.Net.HttpStatusCode.OK, "Admin Email log found successfully", emailLogDtoList);
            }
            else
            {
                return new OperationResult<List<EmailLogDto>>(false, System.Net.HttpStatusCode.OK, "Email log not found");
            }
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete("{EmailId}")]
        public async Task<OperationResult<EmailLogDto>> RemoveByAdmin(int EmailId)
        {
            var deleteEmailLogByAdminObj = _emailLogService.DeleteEmailLog(EmailId);

            if (deleteEmailLogByAdminObj != null)
            {
                var deleteEmailLogByAdminDtoObj = _mapper.Map<EmailLogDto>(deleteEmailLogByAdminObj);
                return new OperationResult<EmailLogDto>(true, System.Net.HttpStatusCode.OK,"Email Log deleted successfully", deleteEmailLogByAdminDtoObj);
            }
            else
            {
                return new OperationResult<EmailLogDto>(false, System.Net.HttpStatusCode.OK, "Somehting went wrong");
            }

        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser")]
        [HttpPost]
        public async Task<OperationResult<EmailLogDto>> ResendEmail([FromBody] EmailLogDto model)
        {
            var emailLogDto = await signupUser.ResendEmailToUser(model);
            return new OperationResult<EmailLogDto>(true, System.Net.HttpStatusCode.OK,"Email send successfully.", model);
        }
    }
}