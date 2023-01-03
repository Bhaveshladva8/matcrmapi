using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class OrganizationLabelController : Controller
    {

        private readonly IOrganizationLabelService _labelService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;
        public OrganizationLabelController(
            IOrganizationLabelService labelService,
            IMapper mapper)
        {
            _labelService = labelService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<OrganizationLabelDto>>> List()
        {
            List<OrganizationLabelDto> organizationLabelDtoList = new List<OrganizationLabelDto>();

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            var organizationLabelList = _labelService.GetByUserAndDefault(UserId);
            organizationLabelDtoList = _mapper.Map<List<OrganizationLabelDto>>(organizationLabelList);

            return new OperationResult<List<OrganizationLabelDto>>(true, System.Net.HttpStatusCode.OK, "", organizationLabelDtoList);
        }

        [Authorize]
        [HttpGet]
        public async Task<OperationResult<List<OrganizationLabelDto>>> BasedOnTenant()
        {
            List<OrganizationLabelDto> organizationLabelDtoList = new List<OrganizationLabelDto>();

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);


            var organizationLabels = _labelService.GetByTenant(TenantId);
            organizationLabelDtoList = _mapper.Map<List<OrganizationLabelDto>>(organizationLabels);
            return new OperationResult<List<OrganizationLabelDto>>(true, System.Net.HttpStatusCode.OK, "", organizationLabelDtoList);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<OrganizationLabel>> AddUpdate(OrganizationLabelDto model)
        {
            var organizationLabelObj = await _labelService.CheckInsertOrUpdate(model);

            return new OperationResult<OrganizationLabel>(true, System.Net.HttpStatusCode.OK, "", organizationLabelObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete]
        public async Task<OperationResult<OrganizationLabel>> Remove(OrganizationLabelDto model)
        {
            if (model.Id != null)
            {
                var organizationLabelObj = _labelService.DeleteLabel(model);
                return new OperationResult<OrganizationLabel>(true, System.Net.HttpStatusCode.OK, "", organizationLabelObj);
            }
            else
            {
                return new OperationResult<OrganizationLabel>(false, System.Net.HttpStatusCode.OK, "Please provide id");
            }
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<OrganizationLabel>> Detail(int Id)
        {
            OrganizationLabel organizationLabelObj = new OrganizationLabel();
            organizationLabelObj = _labelService.GetById(Id);
            return new OperationResult<OrganizationLabel>(true, System.Net.HttpStatusCode.OK, "", organizationLabelObj);
        }

    }
}