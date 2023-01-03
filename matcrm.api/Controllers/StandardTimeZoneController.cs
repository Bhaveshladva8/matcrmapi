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
    public class StandardTimeZoneController : Controller
    {
        private readonly IStandardTimeZoneService _standardTimeZoneService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;        
        public StandardTimeZoneController(IStandardTimeZoneService standardTimeZoneService,
            IMapper mapper)
        {
            _standardTimeZoneService = standardTimeZoneService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<StandardTimeZoneDropDownListResponse>>> DropDownList()
        {            
            var standardTimeZoneList = _standardTimeZoneService.GetAll();
            var standardTimeZoneListResponseList= _mapper.Map<List<StandardTimeZoneDropDownListResponse>>(standardTimeZoneList);
            return new OperationResult<List<StandardTimeZoneDropDownListResponse>>(true, System.Net.HttpStatusCode.OK, "", standardTimeZoneListResponseList);
        }
    }
}