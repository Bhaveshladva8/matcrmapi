using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using matcrm.service.Services;
using matcrm.data.Models.Response;
using matcrm.service.Common;
using AutoMapper;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class CityController : Controller
    {
        private readonly ICityService _cityService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;
        public CityController(ICityService cityService,
            IMapper mapper)
        {
            _cityService = cityService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{stateId}")]
        public async Task<OperationResult<List<CityDropDownListResponse>>> DropDownList(int stateId)
        {            
            var cityList = _cityService.GetAllByStateId(stateId);
            var cityListResponseList = _mapper.Map<List<CityDropDownListResponse>>(cityList);
            return new OperationResult<List<CityDropDownListResponse>>(true, System.Net.HttpStatusCode.OK, "", cityListResponseList);
        }
        
    }
}