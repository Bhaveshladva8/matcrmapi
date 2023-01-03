using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using matcrm.api.SignalR;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.data.Context;

namespace matcrm.api.Controllers
{

    [Route("[controller]/[action]")]
    public class SubscriptionTypeController : Controller
    {
        private readonly ISubscriptionPlanService _subscriptionPlanService;
        private readonly ISubscriptionPlanDetailService _subscriptionPlanDetailService;
        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly ISubscriptionTypeService _subscriptionTypeService;
        private readonly IUserService _userService;
        private IMapper _mapper;

        public SubscriptionTypeController(
             ISubscriptionPlanService subscriptionPlanService,
             ISubscriptionPlanDetailService subscriptionPlanDetailService,
             IUserSubscriptionService userSubscriptionService,
             ISubscriptionTypeService subscriptionTypeService,
             IUserService userService,
             IMapper mapper
        )
        {
            _subscriptionPlanService = subscriptionPlanService;
            _subscriptionPlanDetailService = subscriptionPlanDetailService;
            _userSubscriptionService = userSubscriptionService;
            _subscriptionTypeService = subscriptionTypeService;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<OperationResult<List<SubscriptionTypeDto>>> List()
        {
            List<SubscriptionTypeDto> subscriptionTypeDtoList = new List<SubscriptionTypeDto>();
            var subscriptionTypeList = _subscriptionTypeService.GetAll();

            subscriptionTypeDtoList = _mapper.Map<List<SubscriptionTypeDto>>(subscriptionTypeList);

            return new OperationResult<List<SubscriptionTypeDto>>(true, System.Net.HttpStatusCode.OK,"", subscriptionTypeDtoList);
        }
    }
}