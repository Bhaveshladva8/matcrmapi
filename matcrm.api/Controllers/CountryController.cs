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
    public class CountryController : Controller
    {
        private readonly ICountryService _countryService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;
        public CountryController(ICountryService countryService,
            IMapper mapper)
        {
            _countryService = countryService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<CountryDropDownListResponse>>> DropDownList()
        {            
            var countryList= _countryService.GetAll();
            var countryListResponseList = _mapper.Map<List<CountryDropDownListResponse>>(countryList);
            return new OperationResult<List<CountryDropDownListResponse>>(true, System.Net.HttpStatusCode.OK, "", countryListResponseList);
        }
    }
}