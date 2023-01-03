using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
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
using matcrm.data.Models.Response;
using matcrm.data.Context;

namespace matcrm.api.Controllers
{

    [Route("[controller]/[action]")]
    [Authorize]
    public class SubscriptionPlanController : Controller
    {
        private readonly ISubscriptionPlanService _subscriptionPlanService;
        private readonly ISubscriptionPlanDetailService _subscriptionPlanDetailService;
        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly ISubscriptionTypeService _subscriptionTypeService;
        private readonly IUserService _userService;
        private IMapper _mapper;
        private int UserId = 0;

        public SubscriptionPlanController(
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
        public async Task<OperationResult<List<SubscriptionPlanGetAllResponse>>> List()
        {
            List<SubscriptionPlanDto> subscriptionPlanDtoList = new List<SubscriptionPlanDto>();
            var subscriptionPlanList = _subscriptionPlanService.GetAll();

            subscriptionPlanDtoList = _mapper.Map<List<SubscriptionPlanDto>>(subscriptionPlanList);
            if (subscriptionPlanDtoList != null && subscriptionPlanDtoList.Count() > 0)
            {
                foreach (var item in subscriptionPlanDtoList)
                {
                    if (item.Id != null)
                    {
                        var subscriptionPlanDetails = _subscriptionPlanDetailService.GetAllByPlan(item.Id.Value);
                        item.Details = _mapper.Map<List<SubscriptionPlanDetailDto>>(subscriptionPlanDetails);
                    }
                }
            }
            var responsesubscriptionPlanDtos = _mapper.Map<List<SubscriptionPlanGetAllResponse>>(subscriptionPlanDtoList);
            return new OperationResult<List<SubscriptionPlanGetAllResponse>>(true, System.Net.HttpStatusCode.OK,"", responsesubscriptionPlanDtos);            
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]        
        [HttpGet]
        public async Task<OperationResult<SubscriptionPlanGetByUserResponse>> UserPlan()
        {
            UserSubscriptionDto userSubscriptionDtoObj = new UserSubscriptionDto();

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            var userSubscription = _userSubscriptionService.GetByUser(UserId);

            userSubscriptionDtoObj = _mapper.Map<UserSubscriptionDto>(userSubscription);
            var responseDtos = _mapper.Map<SubscriptionPlanGetByUserResponse>(userSubscriptionDtoObj);
            return new OperationResult<SubscriptionPlanGetByUserResponse>(true, System.Net.HttpStatusCode.OK,"", responseDtos);            
        }
    }
}