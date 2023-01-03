using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using matcrm.data.Models.Dto;
using matcrm.service.Common;
using matcrm.service.Services;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class CustomerTypeController : Controller
    {
        private readonly ICustomerTypeService _CustomerTypeService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;
        public CustomerTypeController(
            ICustomerTypeService CustomerTypeService,
            IMapper mapper
        )
        {
            _CustomerTypeService = CustomerTypeService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<CustomerTypeDto>> Add(CustomerTypeDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            if (model.Id == null)
            {
                model.CreatedBy = UserId;
            }
            else
            {
                model.UpdatedBy = UserId;
            }
            model.TenantId = TenantId;
            var customerTypeObj = _CustomerTypeService.CheckInsertOrUpdate(model);
            var customerTypeDto = _mapper.Map<CustomerTypeDto>(customerTypeObj);
            return new OperationResult<CustomerTypeDto>(true, System.Net.HttpStatusCode.OK, "", customerTypeDto);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<CustomerTypeDto>> Update(CustomerTypeDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            if (model.Id == null)
            {
                model.CreatedBy = UserId;
            }
            else
            {
                model.UpdatedBy = UserId;
            }
            model.TenantId = TenantId;
            var customerTypeObj = _CustomerTypeService.CheckInsertOrUpdate(model);
            var customerTypeDto = _mapper.Map<CustomerTypeDto>(customerTypeObj);
            return new OperationResult<CustomerTypeDto>(true, System.Net.HttpStatusCode.OK, "", customerTypeDto);
        }

        [Authorize]
        [HttpGet]
        public async Task<OperationResult<List<CustomerTypeDto>>> List()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var customerTypes = _CustomerTypeService.GetCustomerTypesByTenant(TenantId);
            var customerTypeDtoList = _mapper.Map<List<CustomerTypeDto>>(customerTypes);
            return new OperationResult<List<CustomerTypeDto>>(true, System.Net.HttpStatusCode.OK, "", customerTypeDtoList);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete("{Id}")]
        public async Task<OperationResult<CustomerTypeDto>> Remove(long Id)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            CustomerTypeDto customerTypeDto = new CustomerTypeDto();
            var customerTypeObj = _CustomerTypeService.DeleteCustomerType(Id, UserId);
            if (customerTypeObj != null)
            {
                customerTypeDto = _mapper.Map<CustomerTypeDto>(customerTypeObj);
            }
            return new OperationResult<CustomerTypeDto>(true, System.Net.HttpStatusCode.OK, "", customerTypeDto);
        }
    }
}