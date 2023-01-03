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
    public class StateController : Controller
    {
        private readonly IStateService _stateService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;       

        public StateController(IStateService stateService,
            IMapper mapper)
        {
            _stateService = stateService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{countryId}")]
        public async Task<OperationResult<List<StateDropDownListResponse>>> DropDownList(int countryId)
        {            
            var stateList = _stateService.GetAllByCountryId(countryId);
            var stateListResponseList = _mapper.Map<List<StateDropDownListResponse>>(stateList);
            return new OperationResult<List<StateDropDownListResponse>>(true, System.Net.HttpStatusCode.OK, "", stateListResponseList);
        }
        
    }
}