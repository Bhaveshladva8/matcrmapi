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

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class CustomerLabelController : Controller
    {

        private readonly ICustomerLabelService _labelService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;
        public CustomerLabelController(
            ICustomerLabelService labelService,
            IMapper mapper)
        {
            _labelService = labelService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        //get all customer label
        [HttpGet]
        public async Task<OperationResult<List<CustomerLabelDto>>> List()
        {

            List<CustomerLabelDto> customerLabelDtoList = new List<CustomerLabelDto>();

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            var labels = _labelService.GetByUserAndDefault(UserId);
            customerLabelDtoList = _mapper.Map<List<CustomerLabelDto>>(labels);

            return new OperationResult<List<CustomerLabelDto>>(true, System.Net.HttpStatusCode.OK, "", customerLabelDtoList);
        }

        [Authorize]
        [HttpGet]
        public async Task<OperationResult<List<CustomerLabelDto>>> BasedOnTenant()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            List<CustomerLabelDto> customerLabelDtosList = new List<CustomerLabelDto>();
            var labels = _labelService.GetByTenant(TenantId);
            customerLabelDtosList = _mapper.Map<List<CustomerLabelDto>>(labels);
            return new OperationResult<List<CustomerLabelDto>>(true, System.Net.HttpStatusCode.OK, "", customerLabelDtosList);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<CustomerLabel>> AddUpdate(CustomerLabelDto model)
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
            var customerLabelObj = await _labelService.CheckInsertOrUpdate(model);

            return new OperationResult<CustomerLabel>(true, System.Net.HttpStatusCode.OK, "", customerLabelObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete]
        public async Task<OperationResult<CustomerLabel>> Remove(CustomerLabelDto model)
        {
            var customerLabelObj = _labelService.DeleteLabel(model);
            return new OperationResult<CustomerLabel>(true, System.Net.HttpStatusCode.OK, "", customerLabelObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<CustomerLabel>> Detail(int Id)
        {
            CustomerLabel customerLabelObj = new CustomerLabel();
            customerLabelObj = _labelService.GetById(Id);
            return new OperationResult<CustomerLabel>(true, System.Net.HttpStatusCode.OK, "", customerLabelObj);
        }

    }
}