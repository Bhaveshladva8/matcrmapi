using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;
using matcrm.service.Services;
using matcrm.service.Common;
using System.Security.Claims;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using System.IdentityModel.Tokens.Jwt;
using matcrm.data.Context;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class StatusController : Controller
    {
        private readonly IStatusService _statusService;
        private readonly ICustomTableService _customTableService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;
        public StatusController(IStatusService statusService, IMapper mapper, ICustomTableService customTableService)
        {
            _statusService = statusService;
            _customTableService = customTableService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, ExternalUser, TenantUser")]
        [HttpGet]
        public async Task<OperationResult<List<StatusListResponse>>> List()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            List<StatusDto> StatusDtoList = new List<StatusDto>();
            var statusList = _statusService.GetByTenant(TenantId);

            StatusDtoList = _mapper.Map<List<StatusDto>>(statusList);
            var responseStatusList = _mapper.Map<List<StatusListResponse>>(StatusDtoList);
            if (responseStatusList != null && responseStatusList.Count() > 0)
            {
                foreach (var item in responseStatusList)
                {
                    var customTableObj = statusList.Where(t => t.CustomTable.Id == item.CustomTableId).FirstOrDefault();
                    if (customTableObj != null)
                    {
                        item.Category = customTableObj.CustomTable.Name;
                    }
                }
            }

            return new OperationResult<List<StatusListResponse>>(true, System.Net.HttpStatusCode.OK, "", responseStatusList);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<StatusDetailResponse>> Detail(int Id)
        {
            Status statusObj = new Status();
            StatusDetailResponse responseStatusObj = new StatusDetailResponse();
            statusObj = _statusService.GetById(Id);
            responseStatusObj = _mapper.Map<StatusDetailResponse>(statusObj);
            return new OperationResult<StatusDetailResponse>(true, System.Net.HttpStatusCode.OK, "", responseStatusObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<List<StatusAddResponse>>> Add([FromBody] StatusAddRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            StatusAddResponse statusAddResponseObj = new StatusAddResponse();

            List<StatusAddResponse> statusAddResponseList = new List<StatusAddResponse>();

            var model = _mapper.Map<StatusDto>(requestModel);

            if (model.CustomTableIds != null && model.CustomTableIds.Count() > 0)
            {
                foreach (var item in model.CustomTableIds)
                {
                    model.CustomTableId = item;
                    if (model.Id != null && model.Id > 0)
                    {
                        model.UpdatedBy = UserId;
                    }
                    else
                    {
                        model.CreatedBy = UserId;
                    }
                    model.TenantId = TenantId;
                    var AddUpdate = await _statusService.CheckInsertOrUpdate(model);
                    statusAddResponseObj = _mapper.Map<StatusAddResponse>(AddUpdate);

                    statusAddResponseObj.CustomTableId = item;

                    var customModuleObj = _customTableService.GetById(item);
                    if (customModuleObj != null)
                    {
                        statusAddResponseObj.Category = customModuleObj.Name;
                    }
                    statusAddResponseList.Add(statusAddResponseObj);
                }
            }
            //responseAddupdate.CustomTableIds = requestModel.CustomTableIds;
            return new OperationResult<List<StatusAddResponse>>(true, System.Net.HttpStatusCode.OK, "Status added successfully", statusAddResponseList);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<StatusUpdateResponse>> Update([FromBody] StatusUpdateRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            StatusUpdateResponse statusUpdateResponseObj = new StatusUpdateResponse();

            List<StatusUpdateResponse> statusUpdateResponseList = new List<StatusUpdateResponse>();

            var model = _mapper.Map<StatusDto>(requestModel);
           
            if (model.Id != null && model.Id > 0)
            {
                model.UpdatedBy = UserId;
            }
            else
            {
                model.CreatedBy = UserId;
            }
            model.TenantId = TenantId;
            var AddUpdate = await _statusService.CheckInsertOrUpdate(model);
            statusUpdateResponseObj = _mapper.Map<StatusUpdateResponse>(AddUpdate);
            var customModuleObj = _customTableService.GetById(requestModel.CustomTableId);
            if (customModuleObj != null)
            {
                statusUpdateResponseObj.Category = customModuleObj.Name;
            }
            //tatusUpdateResponseList.Add(statusUpdateResponseObj);
            //return new OperationResult<List<StatusUpdateResponse>>(true, System.Net.HttpStatusCode.OK, "Status updated successfully", statusUpdateResponseList);
            return new OperationResult<StatusUpdateResponse> (true, System.Net.HttpStatusCode.OK, "Status updated successfully", statusUpdateResponseObj);
        }


        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete("{Id}")]
        public async Task<OperationResult<StatusDeleteResponse>> Remove(long Id)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            if (Id != null)
            {
                var statusObj = await _statusService.DeleteStatus(Id);
                var responseObj = _mapper.Map<StatusDeleteResponse>(statusObj);
                return new OperationResult<StatusDeleteResponse>(true, System.Net.HttpStatusCode.OK, "", responseObj);
            }
            else
            {
                return new OperationResult<StatusDeleteResponse>(false, System.Net.HttpStatusCode.OK, "Please provide id");
            }
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{CustomTableName}")]
        public async Task<OperationResult<List<StatusListByTableResponse>>> ListByTable(string CustomTableName)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            List<Status> StatusList = new List<Status>();

            var customTable = _customTableService.GetByName(CustomTableName);
            if (customTable != null)
            {
                StatusList = _statusService.GetByCustomTableName(customTable.Id,TenantId);
            }

            var responseStatusList = _mapper.Map<List<StatusListByTableResponse>>(StatusList);
            return new OperationResult<List<StatusListByTableResponse>>(true, System.Net.HttpStatusCode.OK, "", responseStatusList);
        }

    }
}