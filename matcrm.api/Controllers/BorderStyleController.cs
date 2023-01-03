using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using matcrm.data.Models.Dto;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class BorderStyleController
    {
        private readonly IBorderStyleService _BorderStyleService;
        private IMapper _mapper;
        public BorderStyleController(
            IBorderStyleService BorderStyleService,
            IMapper mapper
        )
        {
            _BorderStyleService = BorderStyleService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<BorderStyleDto>> AddUpdate(BorderStyleAddUpdateRequest model)
        {
            BorderStyleDto borderStyleDto = _mapper.Map<BorderStyleDto>(model);
            var BorderStyleObj = await _BorderStyleService.CheckInsertOrUpdate(borderStyleDto);
            var BorderStyleDto = _mapper.Map<BorderStyleDto>(BorderStyleObj);
            return new OperationResult<BorderStyleDto>(true, "", BorderStyleDto);
        }

        //Get All borderstyle
        [Authorize]
        [HttpGet]
        public async Task<OperationResult<List<BorderStyleGetAllResponse>>> List()
        {
            var BorderStyleList = _BorderStyleService.GetAll();
            var BorderStyleListDto = _mapper.Map<List<BorderStyleDto>>(BorderStyleList);
            var responseListDto = _mapper.Map<List<BorderStyleGetAllResponse>>(BorderStyleListDto);
            return new OperationResult<List<BorderStyleGetAllResponse>>(true, System.Net.HttpStatusCode.OK, "", responseListDto);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete("{Id}")]
        public async Task<OperationResult<BorderStyleDto>> Remove(long Id)
        {
            var BorderStyleObj = _BorderStyleService.DeleteBorderStyle(Id);
            BorderStyleDto borderStyleDto = new BorderStyleDto();
            if (BorderStyleObj != null)
            {
                borderStyleDto = _mapper.Map<BorderStyleDto>(BorderStyleObj);
            }
            return new OperationResult<BorderStyleDto>(true, "", borderStyleDto);
        }
    }
}