using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data.Models.Response;
using matcrm.service.Common;
using matcrm.service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class SalutationController : Controller
    {
        private readonly ISalutationService _salutationService;
        private IMapper _mapper;

        public SalutationController(ISalutationService salutationService,
        IMapper mapper)
        {
            _salutationService = salutationService;
             _mapper = mapper;
        }

        //[Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<SalutationListResponse>>> List()
        {
            //List<SalutationDto> salutationDtoList = new List<SalutationDto>();
            var salutations = _salutationService.GetAll();
            //salutationDtoList = _mapper.Map<List<SalutationDto>>(salutations);
            var responseSalutationDtoList = _mapper.Map<List<SalutationListResponse>>(salutations);
            return new OperationResult<List<SalutationListResponse>>(true, System.Net.HttpStatusCode.OK, "", responseSalutationDtoList);
        }
    }
}