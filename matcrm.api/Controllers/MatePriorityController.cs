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
using matcrm.data.Models.Request;
using matcrm.service.Common;
using matcrm.data.Models.Response;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class MatePriorityController : Controller
    {
        private readonly IMatePriorityService _matePriorityService;
        private readonly ICustomTableService _customTableService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;
        public MatePriorityController(IMatePriorityService matePriorityService,
        ICustomTableService customTableService,
        IMapper mapper
        )
        {
            _matePriorityService = matePriorityService;
            _customTableService = customTableService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<List<MatePriorityAddResponse>>> Add([FromBody] MatePriorityAddRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            MatePriorityAddResponse responseObj = new MatePriorityAddResponse();

            List<MatePriorityAddResponse> responsesList = new List<MatePriorityAddResponse>();

            var model = _mapper.Map<MatePriorityDto>(requestModel);
            if (model.Id != null && model.Id > 0)
            {
                model.UpdatedBy = UserId;
            }
            else
            {
                model.CreatedBy = UserId;
            }
            model.TenantId = TenantId;

            if (requestModel.CustomTableIds != null && requestModel.CustomTableIds.Count() > 0)
            {
                foreach (var item in requestModel.CustomTableIds)
                {
                    model.CustomTableId = item;

                    var AddUpdate = await _matePriorityService.CheckInsertOrUpdate(model);
                    responseObj = _mapper.Map<MatePriorityAddResponse>(AddUpdate);

                    responseObj.CustomTableId = item;

                    var customModuleObj = _customTableService.GetById(item);
                    if (customModuleObj != null)
                    {
                        responseObj.CustomTableName = customModuleObj.Name;
                    }
                    responsesList.Add(responseObj);
                }
            }
            return new OperationResult<List<MatePriorityAddResponse>>(true, System.Net.HttpStatusCode.OK, "Priority added successfully", responsesList);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<MatePriorityUpdateResponse>> Update([FromBody] MatePriorityUpdateRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var model = _mapper.Map<MatePriorityDto>(requestModel);

            if (model.Id != null && model.Id > 0)
            {
                model.UpdatedBy = UserId;
            }
            else
            {
                model.CreatedBy = UserId;
            }
            model.TenantId = TenantId;
            var AddUpdate = await _matePriorityService.CheckInsertOrUpdate(model);
            var responseObj = _mapper.Map<MatePriorityUpdateResponse>(AddUpdate);
            var customModuleObj = _customTableService.GetById(requestModel.CustomTableId);
            if (customModuleObj != null)
            {
                responseObj.CustomTableName = customModuleObj.Name;
            }
            return new OperationResult<MatePriorityUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Priority updated successfully", responseObj);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, ExternalUser, TenantUser")]
        [HttpGet]
        public async Task<OperationResult<List<MatePriorityListResponse>>> List()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var matePriorityList = _matePriorityService.GetByTenant(TenantId);
            var responseStatusList = _mapper.Map<List<MatePriorityListResponse>>(matePriorityList);
            if (responseStatusList != null && responseStatusList.Count() > 0)
            {
                foreach (var item in responseStatusList)
                {
                    var customTableObj = matePriorityList.Where(t => t.CustomTable.Id == item.CustomTableId).FirstOrDefault();
                    if (customTableObj != null)
                    {
                        item.CustomTableName = customTableObj.CustomTable?.Name;
                    }
                }
            }
            return new OperationResult<List<MatePriorityListResponse>>(true, System.Net.HttpStatusCode.OK, "", responseStatusList);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete("{Id}")]
        public async Task<OperationResult> Remove(long Id)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            if (Id != null)
            {
                var mateCategoryObj = await _matePriorityService.DeleteById(Id);

                return new OperationResult(true, System.Net.HttpStatusCode.OK, "", Id);
            }
            else
            {
                return new OperationResult(false, System.Net.HttpStatusCode.OK, "Please provide id", Id);
            }
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{CustomTableName}")]
        public async Task<OperationResult<List<MatePriorityListByTableResponse>>> ListByTable(string CustomTableName)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            List<MatePriority> matePriorityList = new List<MatePriority>();
            var customTable = _customTableService.GetByName(CustomTableName);
            if (customTable != null)
            {
                matePriorityList = _matePriorityService.GetByCustomTableId(customTable.Id, TenantId);
            }
            var responseList = _mapper.Map<List<MatePriorityListByTableResponse>>(matePriorityList);
            return new OperationResult<List<MatePriorityListByTableResponse>>(true, System.Net.HttpStatusCode.OK, "", responseList);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<MatePriorityDetailResponse>> Detail(int Id)
        {            
            var matePriorityObj = _matePriorityService.GetById(Id);
            var responseObj = _mapper.Map<MatePriorityDetailResponse>(matePriorityObj);
            return new OperationResult<MatePriorityDetailResponse>(true, System.Net.HttpStatusCode.OK, "", responseObj);
        }
    }
}