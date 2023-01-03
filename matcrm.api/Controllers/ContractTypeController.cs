using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;
using matcrm.data.Models.Tables;
using matcrm.service.Common;
using matcrm.service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]

    public class ContractTypeController : Controller
    {
        private readonly IContractTypeService _contractTypeService;
        private readonly IUserService _userService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;

        public ContractTypeController(
             IContractTypeService contractTypeService,
             IUserService userService,
             IMapper mapper
        )
        {
            _contractTypeService = contractTypeService;
            _userService = userService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager")]
        [HttpPost]
        public async Task<OperationResult<ContractTypeRequestResponse>> Add([FromBody] ContractTypeRequestResponse Model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            ContractType contractTypeObj = _mapper.Map<ContractType>(Model);
            contractTypeObj.CreatedBy = UserId;
            var InsetedContractTypeObj = await _contractTypeService.CheckInsertOrUpdate(contractTypeObj);
            if (InsetedContractTypeObj != null)
            {
                Model.Id = InsetedContractTypeObj.Id;
            }
            return new OperationResult<ContractTypeRequestResponse>(true, System.Net.HttpStatusCode.OK, "", Model);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager")]
        [HttpPut]
        public async Task<OperationResult<ContractTypeRequestResponse>> Update([FromBody] ContractTypeRequestResponse Model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            if (Model.Id != null)
            {
                ContractType contractTypeObj = _mapper.Map<ContractType>(Model);
                contractTypeObj.UpdatedBy = UserId;
                var UpdatedContractTypeObj = await _contractTypeService.CheckInsertOrUpdate(contractTypeObj);

                return new OperationResult<ContractTypeRequestResponse>(true, System.Net.HttpStatusCode.OK, "", Model);
            }
            else
            {
                return new OperationResult<ContractTypeRequestResponse>(false, System.Net.HttpStatusCode.OK, "Please provide id");
            }
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager")]
        [HttpDelete("{Id}")]
        public async Task<OperationResult> Remove(long Id)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            if (Id != null && Id > 0)
            {
                var contractTypeObj = await _contractTypeService.DeleteContractType(Id, UserId);

                return new OperationResult(true, System.Net.HttpStatusCode.OK, "", Id);
            }
            else
            {
                return new OperationResult(false, System.Net.HttpStatusCode.OK, "Please provide id", Id);
            }
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<ContractTypeRequestResponse>>> List()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            List<ContractTypeRequestResponse> ContractTypeDtoList = new List<ContractTypeRequestResponse>();
            var ContractTypeList = _contractTypeService.GetByTenant(TenantId);

            ContractTypeDtoList = _mapper.Map<List<ContractTypeRequestResponse>>(ContractTypeList);

            return new OperationResult<List<ContractTypeRequestResponse>>(true, System.Net.HttpStatusCode.OK, "", ContractTypeDtoList);
        }

        [HttpGet]
        public async Task<OperationResult<List<ContractTypeDropDownListResponse>>> DropDownList()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            List<ContractTypeDropDownListResponse> ContractTypeDtoList = new List<ContractTypeDropDownListResponse>();
            var ContractTypeList = _contractTypeService.GetAllByTenant(TenantId);

            ContractTypeDtoList = _mapper.Map<List<ContractTypeDropDownListResponse>>(ContractTypeList);

            return new OperationResult<List<ContractTypeDropDownListResponse>>(true, System.Net.HttpStatusCode.OK, "", ContractTypeDtoList);
        }
    }
}