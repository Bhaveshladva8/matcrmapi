using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
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
using matcrm.data.Models.Response;

namespace matcrm.api.Controllers
{
    // [Authorize]
   [Route("[controller]/[action]")]
    [ApiController]
    public class IntProviderController : Controller
    {
        private readonly IIntProviderService _intProviderService;
        private readonly IIntProviderAppService _intProviderAppService;
        private readonly IIntProviderAppSecretService _intProviderAppSecretService;
        private readonly IMapper _mapper;
        private int UserId = 0;

        public IntProviderController(
            IIntProviderService intProviderService,
            IIntProviderAppService intProviderAppService,
            IIntProviderAppSecretService intProviderAppSecretService,
            IMapper mapper
        )
        {
            _intProviderService = intProviderService;
            _intProviderAppService = intProviderAppService;
            _intProviderAppSecretService = intProviderAppSecretService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<OperationResult<List<IntProviderGetAllResponse>>> List()
        {
            var intProviders = _intProviderService.GetAll();

            var intProviderDtoList = _mapper.Map<List<IntProviderDto>>(intProviders);
            if (intProviderDtoList != null && intProviderDtoList.Count() > 0)
            {
                foreach (var Provider in intProviderDtoList)
                {
                    if (Provider.Id != null)
                    {
                        var IntProviderAppList = _intProviderAppService.GetByProviderId(Provider.Id.Value);
                        Provider.Apps = _mapper.Map<List<IntProviderAppDto>>(IntProviderAppList);
                    }
                }
            }
            var ResponseIntProviderAppList = _mapper.Map<List<IntProviderGetAllResponse>>(intProviderDtoList);
            return new OperationResult<List<IntProviderGetAllResponse>>(true, System.Net.HttpStatusCode.OK,"", ResponseIntProviderAppList);            
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<IntProviderAppSecretDto>>> BasedOnUser()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var intProviderAppSecretList = _intProviderAppSecretService.GetAllByUser(UserId);
            var intProviderAppSecretDtoList = _mapper.Map<List<IntProviderAppSecretDto>>(intProviderAppSecretList);

            return new OperationResult<List<IntProviderAppSecretDto>>(true, System.Net.HttpStatusCode.OK, "", intProviderAppSecretDtoList);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<IntProviderAppDto>>> Apps()
        {
            var intProviderAppList = _intProviderAppService.GetAll();
            var intProviderAppDtoList = _mapper.Map<List<IntProviderAppDto>>(intProviderAppList);

            return new OperationResult<List<IntProviderAppDto>>(true, System.Net.HttpStatusCode.OK, "", intProviderAppDtoList);
        }


    }
}