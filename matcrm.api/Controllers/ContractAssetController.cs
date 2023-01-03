using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using matcrm.service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using matcrm.service.Common;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using matcrm.data.Models.Tables;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ContractAssetController : Controller
    {
        private readonly IContractAssetService _contractAssetService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;
        public ContractAssetController(IContractAssetService contractAssetService,
        IMapper mapper)
        {
            _contractAssetService = contractAssetService;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<OperationResult<ContractAssetAddUpdateResponse>> Add([FromBody] ContractAssetAddUpdateRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var model = _mapper.Map<ContractAsset>(requestmodel);
            if (model.Id == 0)
            {
                model.CreatedBy = UserId;
            }
            var contractAssetObj = await _contractAssetService.CheckInsertOrUpdate(model);
            var responseObj = _mapper.Map<ContractAssetAddUpdateResponse>(contractAssetObj);
            return new OperationResult<ContractAssetAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Contract Asset added successfully", responseObj);

        }

        [HttpPut]
        public async Task<OperationResult<ContractAssetAddUpdateResponse>> Update([FromBody] ContractAssetAddUpdateRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var model = _mapper.Map<ContractAsset>(requestmodel);
            if (model.Id > 0)
            {
                model.UpdatedBy = UserId;
            }
            var contractAssetObj = await _contractAssetService.CheckInsertOrUpdate(model);
            var responseObj = _mapper.Map<ContractAssetAddUpdateResponse>(contractAssetObj);
            return new OperationResult<ContractAssetAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Updated successfully", responseObj);
        }

        [HttpGet("{ContractId}")]
        public async Task<OperationResult<List<ContractAssetListResponse>>> List(long ContractId)
        {
            List<ContractAssetListResponse> contractAssetListResponse = new List<ContractAssetListResponse>();
            var contractAssetList = _contractAssetService.GetByContractId(ContractId);
            if (contractAssetList != null && contractAssetList.Count > 0)
            {
                foreach (var item in contractAssetList)
                {
                    ContractAssetListResponse contractAssetObj = new ContractAssetListResponse();
                    contractAssetObj.Id = item.Id;
                    contractAssetObj.SerialNumber = item.SerialNumber;
                    contractAssetObj.Manufacturer = item.AssetsManufacturer?.Name;
                    contractAssetObj.BuyDate = item.BuyDate;
                    contractAssetObj.ServiceExpireDate = item.ServiceExpireDate;
                    contractAssetListResponse.Add(contractAssetObj);
                }
            }
            //var responseObj = _mapper.Map<List<ContractAssetListResponse>>(contractAssetList);
            return new OperationResult<List<ContractAssetListResponse>>(true, System.Net.HttpStatusCode.OK, "", contractAssetListResponse);
        }

        [HttpGet("{Id}")]
        public async Task<OperationResult<ContractAssetDetailResponse>> Detail(long Id)
        {
            var contractAssetObj = _contractAssetService.GetbyId(Id);
            var responseObj = _mapper.Map<ContractAssetDetailResponse>(contractAssetObj);
            return new OperationResult<ContractAssetDetailResponse>(true, System.Net.HttpStatusCode.OK, "", responseObj);
        }

        [HttpDelete("{Id}")]
        public async Task<OperationResult> Remove(long Id)
        {
            if (Id != null && Id > 0)
            {                
                var contractAssetObj = await _contractAssetService.DeleteById(Id);
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "Deleted", Id);
            }
            else
            {
                return new OperationResult(false, System.Net.HttpStatusCode.OK, "Please provide id", Id);
            }
        }
    }
}