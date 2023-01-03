using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.service.BusinessLogic;
using matcrm.service.Common;
using matcrm.service.Services;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class OneClappModuleController : Controller
    {
        private readonly IOneClappModuleService _oneClappModuleService;
        public IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;

        public OneClappModuleController(
            IOneClappModuleService oneClappModuleService,
            IMapper mapper
        )
        {
            _oneClappModuleService = oneClappModuleService;
            _mapper = mapper;
        }

        #region CheckList
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<OperationResult<OneClappModuleDto>> AddUpdate([FromBody] OneClappModuleDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            if(model.Id == null)
            {
                model.CreatedBy = UserId;
            }
            else
            {
                model.UpdatedBy = UserId;
            }
            var oneClappModuleObj = await _oneClappModuleService.CheckInsertOrUpdate(model);

            model = _mapper.Map<OneClappModuleDto>(oneClappModuleObj);
            if (model.Id > 0)
                return new OperationResult<OneClappModuleDto>(true, System.Net.HttpStatusCode.OK,"Updated successfully", model);
            return new OperationResult<OneClappModuleDto>(false, System.Net.HttpStatusCode.OK,"Added successfully.", model);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<OperationResult<OneClappModuleDto>> Remove([FromBody] OneClappModuleDto model)
        {
            if (model.Id != null)
            {
                var oneClappModuleObj = _oneClappModuleService.DeleteById(model.Id.Value);
                model = _mapper.Map<OneClappModuleDto>(oneClappModuleObj);
                return new OperationResult<OneClappModuleDto>(true, System.Net.HttpStatusCode.OK,"Module deleted successfully.", model);
            }
            else
            {
                return new OperationResult<OneClappModuleDto>(false, System.Net.HttpStatusCode.OK,"Please provide module id.", model);
            }
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<OneClappModuleDto>>> List([FromBody] OneClappModuleDto model)
        {
            List<OneClappModuleDto> oneClappModuleDtoList = new List<OneClappModuleDto>();
            var oneClappModuleList = _oneClappModuleService.GetAll();
            oneClappModuleDtoList = _mapper.Map<List<OneClappModuleDto>>(oneClappModuleList);

            return new OperationResult<List<OneClappModuleDto>>(true, System.Net.HttpStatusCode.OK,"", oneClappModuleDtoList);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<OneClappModuleDto>> Detail(long Id)
        {
            OneClappModuleDto oneClappModuleDto = new OneClappModuleDto();
            var oneClappModuleObj = _oneClappModuleService.GetById(Id);
            oneClappModuleDto = _mapper.Map<OneClappModuleDto>(oneClappModuleObj);

            return new OperationResult<OneClappModuleDto>(true, System.Net.HttpStatusCode.OK,"", oneClappModuleDto);
        }
        #endregion
    }
}