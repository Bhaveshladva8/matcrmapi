using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using matcrm.data.Models.Dto;
using matcrm.service.Common;
using matcrm.service.Services;
using AutoMapper;
using System.Collections.Generic;
using matcrm.data.Context;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class EmailProviderController : Controller
    {

        private readonly IEmailProviderService _emailProviderService;
        private IMapper _mapper;
        public EmailProviderController(IEmailProviderService emailProviderService, IMapper mapper)
        {
            _emailProviderService = emailProviderService;
            _mapper = mapper;
        }

        // GetAll Email Provider Method
        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<EmailProviderDto>>> List()
        {
            var emailProvidersObj = _emailProviderService.GetAll();

            List<EmailProviderDto> emailProviderDtoList = new List<EmailProviderDto>();

            if (emailProvidersObj != null)
            {
                emailProviderDtoList = _mapper.Map<List<EmailProviderDto>>(emailProvidersObj);

                return new OperationResult<List<EmailProviderDto>>(true, System.Net.HttpStatusCode.OK, "Email Providers found successfully", emailProviderDtoList);
            }

            return new OperationResult<List<EmailProviderDto>>(false, System.Net.HttpStatusCode.OK, "Email Providers not found ");

        }

        // GetAll Email Provider Method
        [Authorize(Roles = "Admin")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<EmailProviderDto>> Detail(int Id)
        {
            var emailProviderObj = _emailProviderService.GetEmailProviderById(Id);

            if (emailProviderObj != null)
            {
                var emailProviderDto = _mapper.Map<EmailProviderDto>(emailProviderObj);

                return new OperationResult<EmailProviderDto>(true, System.Net.HttpStatusCode.OK, "Email Providers found successfully", emailProviderDto);
            }
            else
            {
                return new OperationResult<EmailProviderDto>(false, System.Net.HttpStatusCode.OK, "Email Providers not found ");
            }

        }

        // Add Updated Email Provider Method
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<OperationResult<EmailProviderDto>> AddUpdate([FromBody] EmailProviderDto model)
        {
            var emailProviderObj = await _emailProviderService.CheckInsertOrUpdate(model);

            if (emailProviderObj != null)
            {
                var emailProviderDtoObj = _mapper.Map<EmailProviderDto>(emailProviderObj);
                return new OperationResult<EmailProviderDto>(true, System.Net.HttpStatusCode.OK, "Email Providers found successfully", emailProviderDtoObj);
            }
            else
            {
                return new OperationResult<EmailProviderDto>(false, System.Net.HttpStatusCode.OK, "Somehting went wrong");
            }

        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{Id}")]
        public async Task<OperationResult<EmailProviderDto>> Remove(int Id)
        {
            var emailProviderObj = _emailProviderService.DeleteEmailProvider(Id);

            if (emailProviderObj != null)
            {
                var emailProviderDtoObj = _mapper.Map<EmailProviderDto>(emailProviderObj);
                return new OperationResult<EmailProviderDto>(true, System.Net.HttpStatusCode.OK, "Email Providers deleted successfully", emailProviderDtoObj);
            }
            else
            {
                return new OperationResult<EmailProviderDto>(false, System.Net.HttpStatusCode.OK, "Somehting went wrong");
            }

        }

    }

}