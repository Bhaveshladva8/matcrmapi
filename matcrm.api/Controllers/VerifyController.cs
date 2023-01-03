using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using matcrm.data.Context;
using matcrm.data.Helpers;
using matcrm.data.Models.Dto;
using matcrm.service.BusinessLogic;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.service.Utility;
using static matcrm.data.Helpers.DataUtility;

namespace matcrm.api.Controllers
{
    [AllowAnonymous]
    [Route("[controller]/[action]")]
    public class VerifyController : Controller
    {
        private VerifyUser verifyUser;
        private readonly IEmailConfigService _emailConfigService;
        private readonly IEmailProviderService _emailProviderService;
        private IMapper _mapper;

        public VerifyController(
            IUserService userService,
            IHostingEnvironment hostingEnvironment,
            OneClappContext context,
            IEmailTemplateService emailTemplateService,
            IEmailLogService emailLogService,
            IEmailConfigService emailConfigService,
            IEmailProviderService emailProviderService,
            IMapper mapper,
            IVerificationCodeService verificationCodeService)
        {
            verifyUser = new VerifyUser(userService, emailTemplateService, emailLogService, emailConfigService, emailProviderService, verificationCodeService, mapper);
        }

        [HttpPost]
        public async Task<OperationResult<VerifyUserDto>> NewAccount([FromBody] VerifyUserDto model)
        {
            var result = await verifyUser.VerifyUserByVerificationCode(model);
            return new OperationResult<VerifyUserDto>(result.IsVerified, System.Net.HttpStatusCode.OK,result.Message, result);

        }

        [HttpPost]
        public async Task<OperationResult<VerifyUserDto>> IsEmailAlreadyVerified([FromBody] VerifyUserDto model)
        {

            var result = await verifyUser.IsEmailAlreadyVerified(model);
            return new OperationResult<VerifyUserDto>(result.IsVerified, System.Net.HttpStatusCode.OK,result.Message, result);

        }

    }
}