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
    public class SatisficationLevelController : Controller
    {
        private readonly ISatisficationLevelService _satisficationLevelService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;

        public SatisficationLevelController(ISatisficationLevelService satisficationLevelService,
        IMapper mapper)
        {
            _satisficationLevelService = satisficationLevelService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<OperationResult<SatisficationLevelAddUpdateResponse>> Add([FromBody] SatisficationLevelAddUpdateRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);            
            var model = _mapper.Map<SatisficationLevel>(requestmodel);
            if (model.Id == 0)
            {
                model.CreatedBy = UserId;
            }
            var satisficationLevelObj = await _satisficationLevelService.CheckInsertOrUpdate(model);
            var responseObj = _mapper.Map<SatisficationLevelAddUpdateResponse>(satisficationLevelObj);
            return new OperationResult<SatisficationLevelAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Added successfully.", responseObj);
        }

        [HttpPut]
        public async Task<OperationResult<SatisficationLevelAddUpdateResponse>> Update([FromBody] SatisficationLevelAddUpdateRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            
            var model = _mapper.Map<SatisficationLevel>(requestmodel);
            if (model.Id > 0)
            {
                model.UpdatedBy = UserId;
            }
            var satisficationLevelObj = await _satisficationLevelService.CheckInsertOrUpdate(model);
            var responseObj = _mapper.Map<SatisficationLevelAddUpdateResponse>(satisficationLevelObj);
            return new OperationResult<SatisficationLevelAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Updated successfully.", responseObj);
        }

        [HttpDelete("{Id}")]
        public async Task<OperationResult> Remove(int Id)
        {
            if (Id > 0)
            {
                var satisficationLevelObj = await _satisficationLevelService.DeleteById(Id);
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "Deleted successfully", Id);
            }
            else
            {
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "Please provide Id", Id);
            }
        }

        [HttpGet]
        public async Task<OperationResult<List<SatisficationLevelListResponse>>> List()
        {
            var satisficationLevels = _satisficationLevelService.GetAll();
            var satisficationLevelListResponses = _mapper.Map<List<SatisficationLevelListResponse>>(satisficationLevels);
            return new OperationResult<List<SatisficationLevelListResponse>>(true, System.Net.HttpStatusCode.OK, "", satisficationLevelListResponses);
        }

        [HttpGet("{Id}")]
        public async Task<OperationResult<SatisficationLevelDetailResponse>> Detail(long Id)
        {
            var satisficationLevelObj = _satisficationLevelService.GetById(Id);
            var ResponseObj = _mapper.Map<SatisficationLevelDetailResponse>(satisficationLevelObj);
            return new OperationResult<SatisficationLevelDetailResponse>(true, System.Net.HttpStatusCode.OK, "", ResponseObj);
        }

        [HttpGet]
        public async Task<OperationResult<List<SatisficationLevelDropDownListResponse>>> DropDownList()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            
            var satisficationLevels = _satisficationLevelService.GetByTenant(TenantId);
            var satisficationLevelListResponses = _mapper.Map<List<SatisficationLevelDropDownListResponse>>(satisficationLevels);
            return new OperationResult<List<SatisficationLevelDropDownListResponse>>(true, System.Net.HttpStatusCode.OK, "", satisficationLevelListResponses);
        }
    }
}