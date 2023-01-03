using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using matcrm.data.Models.Dto;
using matcrm.service.Common;
using matcrm.service.Services;
using AutoMapper;
using System.Collections.Generic;
using System;
using System.Linq;
using matcrm.data;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ERPSystemColumnMapController : Controller
    {
        private readonly ICustomModuleService _customerModuleService;
        private readonly IERPSystemColumnMapService _eRPSystemColumnMapService;
        private readonly IWeClappUserService _weClappUserService;
        private IMapper _mapper;
        private int UserId = 0;

        public ERPSystemColumnMapController(
            ICustomModuleService customerModuleService,
            IERPSystemColumnMapService eRPSystemColumnMapService,
            IWeClappUserService weClappUserService,
            IMapper mapper)
        {
            _customerModuleService = customerModuleService;
            _eRPSystemColumnMapService = eRPSystemColumnMapService;
            _weClappUserService = weClappUserService;
            _mapper = mapper;
        }

        // Add Updated Email Provider Method
        [Authorize]
        [HttpPost]
        public async Task<OperationResult<ERPSystemColumnMapAddUpdateResponse>> AddUpdate([FromBody] ERPSystemColumnMapAddUpdateRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var requestmodel = _mapper.Map<ERPSystemColumnMapDto>(model);
            requestmodel.UserId = UserId;
            var customModuleObj = _customerModuleService.GetByName(requestmodel.TableName);
            if (customModuleObj != null)
            {
                requestmodel.CustomModuleId = customModuleObj.Id;
            }

            var WeClappUserObj = _weClappUserService.GetByUser(UserId);
            if (WeClappUserObj != null)
            {
                requestmodel.WeClappUserId = WeClappUserObj.Id;
            }

            var eRPSystemColumnMapObj = await _eRPSystemColumnMapService.CheckInsertOrUpdate(requestmodel);

            if (eRPSystemColumnMapObj != null)
            {
                var checkEmailProviderDtoObj = _mapper.Map<ERPSystemColumnMapDto>(eRPSystemColumnMapObj);
                var responseDto = _mapper.Map<ERPSystemColumnMapAddUpdateResponse>(checkEmailProviderDtoObj);
                return new OperationResult<ERPSystemColumnMapAddUpdateResponse>(true, System.Net.HttpStatusCode.OK,"Added successfully", responseDto);                
            }
            else
            {
                return new OperationResult<ERPSystemColumnMapAddUpdateResponse>(false, System.Net.HttpStatusCode.OK,"Somehting went wrong");                
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<OperationResult<List<ERPSystemColumnMapGetAllResponse>>> List([FromBody] ERPSystemColumnMapGetAllRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var requestmodel = _mapper.Map<ERPSystemColumnMapDto>(model);
            List<ERPSystemColumnMapDto> eRPSystemColumnMapDtoList = new List<ERPSystemColumnMapDto>();
            List<ERPSystemColumnMapGetAllResponse> responseERPSystemColumnMapDtos = new List<ERPSystemColumnMapGetAllResponse>();
            var ModuleObj = _customerModuleService.GetByName(requestmodel.TableName);
            if (ModuleObj != null)
            {
                requestmodel.CustomModuleId = ModuleObj.Id;
            }
            if (UserId != null && requestmodel.CustomModuleId != null)
            {
                // var UserId = Convert.ToInt32(model.UserId.Value);
                var erpColumnMapList = _eRPSystemColumnMapService.GetByUserAndModule(UserId, requestmodel.CustomModuleId.Value);

                eRPSystemColumnMapDtoList = _mapper.Map<List<ERPSystemColumnMapDto>>(erpColumnMapList);

                responseERPSystemColumnMapDtos = _mapper.Map<List<ERPSystemColumnMapGetAllResponse>>(eRPSystemColumnMapDtoList);
                return new OperationResult<List<ERPSystemColumnMapGetAllResponse>>(true, System.Net.HttpStatusCode.OK,"", responseERPSystemColumnMapDtos);                
            }
            else
            {
                responseERPSystemColumnMapDtos = _mapper.Map<List<ERPSystemColumnMapGetAllResponse>>(eRPSystemColumnMapDtoList);
                return new OperationResult<List<ERPSystemColumnMapGetAllResponse>>(false, System.Net.HttpStatusCode.OK,"Please provide user and module", responseERPSystemColumnMapDtos);
            }
        }

        [Authorize]
        [HttpDelete("{Id}")]
        public async Task<OperationResult> Remove(long Id)
        {
            ERPSystemColumnMapDto eRPSystemColumnMapDto = new ERPSystemColumnMapDto();
            var deletedErpColumn = await _eRPSystemColumnMapService.DeleteERPSystemColumnMap(Id);
            eRPSystemColumnMapDto = _mapper.Map<ERPSystemColumnMapDto>(deletedErpColumn);
            var responseeRPSystemColumnMapDto = _mapper.Map<ERPSystemColumnMapDeleteResponse>(eRPSystemColumnMapDto);
            return new OperationResult(true, System.Net.HttpStatusCode.OK, "", Id);            
        }
    }
}