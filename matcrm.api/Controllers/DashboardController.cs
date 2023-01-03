using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using matcrm.api.SignalR;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.data.Models.ViewModels;
using matcrm.service.BusinessLogic;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.data.Models.Response;
using matcrm.data.Context;

namespace matcrm.api.Controllers
{

  [Route("[controller]/[action]")]
    public class DashboardController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IOrganizationService _organizationService;
        private readonly ILeadService _leadService;
        private readonly IIntProviderAppSecretService _intProviderAppSecretService;
        private readonly IOneClappFormActionService _oneClappFormActionService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;
        public DashboardController(ICustomerService customerService,
            IOrganizationService organizationService,
            ILeadService leadService,
            IIntProviderAppSecretService intProviderAppSecretService,
            IOneClappFormActionService oneClappFormActionService,
            IMapper mapper)
        {
            _customerService = customerService;
            _organizationService = organizationService;
            _leadService = leadService;
            _intProviderAppSecretService = intProviderAppSecretService;
            _oneClappFormActionService = oneClappFormActionService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]        
        [HttpGet]      
        public async Task<OperationResult<DashboardNewInfo>> Detail()
        {
            DashboardInfoDto dashboardInfoDto = new DashboardInfoDto();

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            //ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            
            dashboardInfoDto.Persons = _customerService.GetCustomerCount(TenantId);
            dashboardInfoDto.Organizations = _organizationService.GetOrganizationCount(TenantId);
            dashboardInfoDto.Leads = _leadService.GetLeadCount(TenantId);
            dashboardInfoDto.GoogleCalendars = _intProviderAppSecretService.GetCalendarCount(UserId);
            dashboardInfoDto.FormActions = _oneClappFormActionService.GetAll();
            var responsedto = _mapper.Map<DashboardNewInfo>(dashboardInfoDto);
            return new OperationResult<DashboardNewInfo>(true, System.Net.HttpStatusCode.OK,"", responsedto);            
        }

    }
}