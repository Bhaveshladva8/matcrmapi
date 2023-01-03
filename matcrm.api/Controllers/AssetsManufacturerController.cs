using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using matcrm.service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper;
using matcrm.service.Common;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using matcrm.data.Models.Tables;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AssetsManufacturerController : Controller
    {
        private readonly IAssetsManufacturerService _assetsManufacturerService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;
        public AssetsManufacturerController(IAssetsManufacturerService assetsManufacturerService,
        IMapper mapper)
        {
            _assetsManufacturerService = assetsManufacturerService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<OperationResult<AssetsManufacturerAddResponse>> Add([FromBody] AssetsManufacturerAddRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var model = _mapper.Map<AssetsManufacturer>(requestmodel);
            if (model.Id == 0)
            {
                model.CreatedBy = UserId;
            }
            var assetsManufacturerObj = await _assetsManufacturerService.CheckInsertOrUpdate(model);
            var assetsManufacturerResponseObj = _mapper.Map<AssetsManufacturerAddResponse>(assetsManufacturerObj);
            return new OperationResult<AssetsManufacturerAddResponse>(true, System.Net.HttpStatusCode.OK, "Added successfully.", assetsManufacturerResponseObj);
        }

        [HttpPut]
        public async Task<OperationResult<AssetsManufacturerAddResponse>> Update([FromBody] AssetsManufacturerAddRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var model = _mapper.Map<AssetsManufacturer>(requestmodel);
            if (model.Id != 0)
            {
                model.UpdatedBy = UserId;
            }
            var assetsManufacturerObj = await _assetsManufacturerService.CheckInsertOrUpdate(model);
            var assetsManufacturerResponseObj = _mapper.Map<AssetsManufacturerAddResponse>(assetsManufacturerObj);
            return new OperationResult<AssetsManufacturerAddResponse>(true, System.Net.HttpStatusCode.OK, "Updated successfully.", assetsManufacturerResponseObj);
        }

        [HttpGet]
        public async Task<OperationResult<List<AssetsManufacturerListResponse>>> List()
        {
            var assetsManufacturerList = _assetsManufacturerService.GetAll();
            var assetsManufacturerListResponses = _mapper.Map<List<AssetsManufacturerListResponse>>(assetsManufacturerList);
            return new OperationResult<List<AssetsManufacturerListResponse>>(true, System.Net.HttpStatusCode.OK, "", assetsManufacturerListResponses);
        }

        [HttpGet("{Id}")]
        public async Task<OperationResult<AssetsManufacturerDetailResponse>> Detail(long Id)
        {
            var assetsManufacturerObj = _assetsManufacturerService.GetById(Id);
            var ResponseObj = _mapper.Map<AssetsManufacturerDetailResponse>(assetsManufacturerObj);
            return new OperationResult<AssetsManufacturerDetailResponse>(true, System.Net.HttpStatusCode.OK, "", ResponseObj);
        }

        [HttpDelete("{Id}")]
        public async Task<OperationResult> Remove(long Id)
        {
            if (Id != null && Id > 0)
            {                
                var assetsManufacturerObj = await _assetsManufacturerService.DeleteById(Id);
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "Deleted", Id);
            }
            else
            {
                return new OperationResult(false, System.Net.HttpStatusCode.OK, "Please provide id", Id);
            }
        }

        [HttpGet]
        public async Task<OperationResult<List<AssetsManufacturerDropDownListResponse>>> DropDownList()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var assetsManufacturerList = _assetsManufacturerService.GetByTenant(TenantId);
            var assetsManufacturerListResponses = _mapper.Map<List<AssetsManufacturerDropDownListResponse>>(assetsManufacturerList);
            return new OperationResult<List<AssetsManufacturerDropDownListResponse>>(true, System.Net.HttpStatusCode.OK, "", assetsManufacturerListResponses);
        }

    }
}