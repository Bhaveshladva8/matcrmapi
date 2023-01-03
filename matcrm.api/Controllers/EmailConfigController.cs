using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using matcrm.data.Models.Dto;
using matcrm.service.Common;
using matcrm.service.Services;
using AutoMapper;
using System.Collections.Generic;
using matcrm.service.Utility;
using System.Security.Claims;
using System;
using matcrm.data.Context;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class EmailConfigController : Controller
    {
        private readonly IEmailConfigService _emailConfigService;
        private IMapper _mapper;
        int TenantId = 0;

        public EmailConfigController(IEmailConfigService emailConfigService, IMapper mapper)
        {
            _emailConfigService = emailConfigService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<OperationResult<List<EmailConfigDto>>> List()
        {
            var emailConfigsObj = _emailConfigService.GetAllActiveEmailConfig();

            List<EmailConfigDto> emailConfigDtoList = new List<EmailConfigDto>();
            emailConfigDtoList = _mapper.Map<List<EmailConfigDto>>(emailConfigsObj);

            if (emailConfigsObj != null)
            {

                //  EmailConfigObj. = ShaHashData.DecodeFrom64(EmailConfigObj.Password);
                return new OperationResult<List<EmailConfigDto>>(true, System.Net.HttpStatusCode.OK, "Email Config found successfully", emailConfigDtoList);
            }
            else
            {
                return new OperationResult<List<EmailConfigDto>>(false, System.Net.HttpStatusCode.OK, "Email Config not set yet", emailConfigDtoList);
            }
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<EmailConfigDto>> Detail(int Id)
        {
            var emailConfigObj = _emailConfigService.GetEmailConfigById(Id);

            if (emailConfigObj != null)
            {
                var emailConfigDto = _mapper.Map<EmailConfigDto>(emailConfigObj);

                return new OperationResult<EmailConfigDto>(true, System.Net.HttpStatusCode.OK, "Email Providers found successfully", emailConfigDto);
            }
            else
            {
                return new OperationResult<EmailConfigDto>(false, System.Net.HttpStatusCode.OK, "Email Providers not found ");
            }
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<EmailConfigDto>> BasedOnTenant()
        {

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var emailConfigObj = _emailConfigService.GetEmailConfigByTenant(TenantId);

            if (emailConfigObj != null)
            {
                var emailConfigDto = _mapper.Map<EmailConfigDto>(emailConfigObj);
                emailConfigDto.Password = ShaHashData.DecodeFrom64(emailConfigDto.Password);

                return new OperationResult<EmailConfigDto>(true, System.Net.HttpStatusCode.OK, "Email config found successfully", emailConfigDto);
            }
            else
            {
                return new OperationResult<EmailConfigDto>(true, System.Net.HttpStatusCode.OK, "Email config not found ");
            }

        }



        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<EmailConfigDto>> AddUpdate([FromBody] EmailConfigDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            model.TenantId = TenantId;
            var emailConfigObj = await _emailConfigService.CheckInsertOrUpdate(model);

            if (emailConfigObj != null)
            {
                emailConfigObj.Password = ShaHashData.DecodeFrom64(emailConfigObj.Password);
                var emailConfigDto = _mapper.Map<EmailConfigDto>(emailConfigObj);
                return new OperationResult<EmailConfigDto>(true, System.Net.HttpStatusCode.OK, "Email Config found successfully", emailConfigDto);
            }
            else
            {
                return new OperationResult<EmailConfigDto>(false, System.Net.HttpStatusCode.OK, "Somehting went wrong");
            }

        }

    }
}