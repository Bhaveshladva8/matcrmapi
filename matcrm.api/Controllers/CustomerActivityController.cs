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
using matcrm.data.Context;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.data.Models.ViewModels;
using matcrm.service.BusinessLogic;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.service.Services.ERP;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class CustomerActivityController : Controller
    {
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IActivityTypeService _customerActivityTypeService;
        private readonly IActivityAvailabilityService _customerActivityAvailabilityService;
        private readonly ICustomerActivityMemberService _customerActivityMemberService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEmailLogService _emailLogService;
        private readonly IEmailConfigService _emailConfigService;
        private readonly IEmailProviderService _emailProviderService;
        private readonly IUserService _userSerVice;
        private readonly IGoogleCalendarService _calendarService;
        private readonly IIntProviderService _intProviderService;
        private readonly IIntProviderAppService _intProviderAppService;
        private readonly IIntProviderAppSecretService _intProviderAppSecretService;
        private readonly ICustomModuleService _customModuleService;
        private readonly ICalendarSyncActivityService _calendarSyncActivityService;
        private readonly ICustomTableService _customTableService;
        private SendEmail sendEmail;
        private IMapper _mapper;
        private CustomFieldLogic customFieldLogic;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private string GoogleCalendarClientId, GoogleCalendarSecret, GoogleCalendarScope, GoogleCalendarApiKey;
        private int UserId = 0;
        private int TenantId = 0;

        public CustomerActivityController(
            ICustomerActivityService customerActivityService,
            IActivityTypeService customerActivityTypeService,
            IActivityAvailabilityService customerActivityAvailabilityService,
            ICustomerActivityMemberService customerActivityMemberService,
            IEmailTemplateService emailTemplateService,
            IEmailLogService emailLogService,
            IEmailConfigService emailConfigService,
            IEmailProviderService emailProviderService,
            IUserService userSerVice,
            IGoogleCalendarService calendarService,
            IIntProviderService intProviderService,
            IIntProviderAppService intProviderAppService,
            IIntProviderAppSecretService intProviderAppSecretService,
            ICustomModuleService customModuleService,
            ICalendarSyncActivityService calendarSyncActivityService,
            IMapper mapper,
            IHubContext<BroadcastHub, IHubClient> hubContext,
            ICustomTableService customTableService)
        {
            _customerActivityService = customerActivityService;
            _customerActivityTypeService = customerActivityTypeService;
            _customerActivityAvailabilityService = customerActivityAvailabilityService;
            _customerActivityMemberService = customerActivityMemberService;
            _emailTemplateService = emailTemplateService;
            _emailLogService = emailLogService;
            _emailProviderService = emailProviderService;
            _userSerVice = userSerVice;
            _calendarService = calendarService;
            _intProviderService = intProviderService;
            _intProviderAppService = intProviderAppService;
            _intProviderAppSecretService = intProviderAppSecretService;
            _customModuleService = customModuleService;
            _calendarSyncActivityService = calendarSyncActivityService;
            _emailConfigService = emailConfigService;
            _mapper = mapper;
            _hubContext = hubContext;
            _customTableService = customTableService;
            sendEmail = new SendEmail(emailTemplateService, emailLogService, emailConfigService, emailProviderService, mapper);
            GoogleCalendarClientId = OneClappContext.GoogleCalendarClientId;
            GoogleCalendarSecret = OneClappContext.GoogleCalendarSecretKey;
            GoogleCalendarApiKey = OneClappContext.GoogleCalendarApiKey;
            GoogleCalendarScope = OneClappContext.GoogleScopes;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]        
        [HttpGet("{CustomerId}")]
        public async Task<OperationResult<List<CustomerActivityGetAllResponse>>> List(long CustomerId)
        {
            List<CustomerActivityDto> customerActivityDtoList = new List<CustomerActivityDto>();
            var customerActivityList = _customerActivityService.GetByCustomer(CustomerId);

            customerActivityDtoList = _mapper.Map<List<CustomerActivityDto>>(customerActivityList);

            var customerActivityTypes = _customerActivityTypeService.GetAll();
            var customerAvailabilities = _customerActivityAvailabilityService.GetAll();
            var users = _userSerVice.GetAll();
            if (customerActivityDtoList != null && customerActivityDtoList.Count() > 0)
            {
                foreach (var item in customerActivityDtoList)
                {
                    List<CustomerActivityMember> customerActivityMemberList = new List<CustomerActivityMember>();
                    if (item.Id != null)
                    {
                        customerActivityMemberList = _customerActivityMemberService.GetAllByActivity(item.Id.Value);
                    }
                    item.Members = _mapper.Map<List<CustomerActivityMemberDto>>(customerActivityMemberList);
                    if (item.CustomerActivityAvailabilityId != null)
                    {
                        if (customerAvailabilities != null)
                        {
                            var customerActivityAvailableObj = customerAvailabilities.Where(t => t.Id == item.CustomerActivityAvailabilityId).FirstOrDefault();
                            if (customerActivityAvailableObj != null)
                            {
                                item.CustomerActivityAvailability = customerActivityAvailableObj.Name;
                            }
                        }
                    }

                    if (item.CustomerActivityTypeId != null)
                    {
                        if (customerActivityTypes != null)
                        {
                            var customerActivityTypeObj = customerActivityTypes.Where(t => t.Id == item.CustomerActivityTypeId).FirstOrDefault();
                            if (customerActivityTypeObj != null)
                            {
                                item.CustomerActivityType = customerActivityTypeObj.Name;
                            }
                        }
                    }
                    if (users != null)
                    {
                        var userObj = users.Where(t => t.Id == item.CreatedBy).FirstOrDefault();
                        if (userObj != null)
                        {
                            item.FirstName = userObj.FirstName;
                            item.LastName = userObj.LastName;
                            item.Email = userObj.Email;
                            if (item.FirstName != null)
                            {
                                item.ShortName = item.FirstName.Substring(0, 1);
                            }
                            if (item.LastName != null)
                            {
                                item.ShortName = item.ShortName + item.LastName.Substring(0, 1);
                            }
                        }
                    }
                }
            }            
            var responseCustomerActivityDtoList = _mapper.Map<List<CustomerActivityGetAllResponse>>(customerActivityDtoList);
            return new OperationResult<List<CustomerActivityGetAllResponse>>(true, System.Net.HttpStatusCode.OK,"", responseCustomerActivityDtoList);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<CustomerActivityAddUpdateResponse>> AddUpdate([FromBody] CustomerActivityAddUpdateRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var requestmodel = _mapper.Map<CustomerActivityDto>(model);
            if (requestmodel.Id == null)
            {
                requestmodel.CreatedBy = UserId;
            }
            else
            {
                requestmodel.UpdatedBy = UserId;
            }
            requestmodel.TenantId = TenantId;
            var customerActivityObj = await _customerActivityService.CheckInsertOrUpdate(requestmodel);
            if (customerActivityObj != null)
            {
                if (requestmodel.Members != null && requestmodel.Members.Count() > 0)
                {
                    foreach (var item in requestmodel.Members)
                    {
                        CustomerActivityMemberDto customerActivityMemberDto = new CustomerActivityMemberDto();
                        customerActivityMemberDto.Email = item.Email;
                        customerActivityMemberDto.CustomerActivityId = customerActivityObj.Id;
                        var isExist = _customerActivityMemberService.GetActivityMember(customerActivityMemberDto);

                        if (isExist == null)
                        {
                            var AddUpdate = await _customerActivityMemberService.CheckInsertOrUpdate(customerActivityMemberDto);
                            await sendEmail.CustomerActivityInviteMember(item.Email, item.Email, requestmodel);
                        }
                    }
                }
                requestmodel.Id = customerActivityObj.Id;
                var intProviderAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUser(UserId);

                if (intProviderAppSecretObj != null)
                {
                    //var customerModuleObj = _customModuleService.GetByName("Person");
                    CustomModule? customModuleObj = null;
                    var customTable = _customTableService.GetByName("Person");
                    if (customTable != null)
                    {
                        customModuleObj = _customModuleService.GetByCustomTable(customTable.Id);
                    }

                    if (customModuleObj != null)
                    {
                        CalendarSyncActivityDto calendarSyncActivityDto = new CalendarSyncActivityDto();
                        calendarSyncActivityDto.CreatedBy = UserId;
                        calendarSyncActivityDto.IntProviderAppSecretId = intProviderAppSecretObj.Id;
                        calendarSyncActivityDto.ActivityId = customerActivityObj.Id;
                        calendarSyncActivityDto.ModuleId = customModuleObj.Id;
                        var calendarSyncData = _calendarSyncActivityService.GetCalendarSyncActivity(calendarSyncActivityDto);



                        var intProviderAppSecretDto = await CheckAccessToken(UserId);
                        if (intProviderAppSecretDto != null && string.IsNullOrEmpty(intProviderAppSecretDto.error_description))
                        {
                            GoogleCalendarEventVM googleCalendarEventVMObj = new GoogleCalendarEventVM();
                            googleCalendarEventVMObj.summary = requestmodel.Title;
                            googleCalendarEventVMObj.description = requestmodel.Description;
                            // eventVM.start = new EventDate();
                            // eventVM.start.dateTime = model.EventStartTime;
                            // eventVM.start.timeZone = "UTC";
                            // eventVM.end = new EventDate();
                            // eventVM.end.dateTime = model.EventEndTime;
                            // eventVM.end.timeZone = "UTC";

                            googleCalendarEventVMObj.start = new EventDate();
                            googleCalendarEventVMObj.start.timeZone = "UTC";
                            if (customerActivityObj.ScheduleStartDate != null)
                            {
                                var startDateTime = Common.GetStartEndTime(customerActivityObj.ScheduleStartDate.Value, customerActivityObj.StartTime);

                                // eventVM.start.dateTime = startDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                                if (startDateTime != null)
                                {
                                    googleCalendarEventVMObj.start.dateTime = startDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                                }
                            }
                            googleCalendarEventVMObj.end = new EventDate();
                            googleCalendarEventVMObj.end.timeZone = "UTC";
                            // eventVM.end.dateTime = endDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                            if (customerActivityObj.ScheduleEndDate != null)
                            {
                                var endDateTime = Common.GetStartEndTime(customerActivityObj.ScheduleEndDate.Value, customerActivityObj.EndTime);
                                if (endDateTime != null)
                                {
                                    googleCalendarEventVMObj.end.dateTime = endDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                                }
                            }
                            googleCalendarEventVMObj.creator.email = intProviderAppSecretDto.Email;
                            googleCalendarEventVMObj.colorId = "1";
                            // eventVM.colorId = "yellow";
                            if (requestmodel.Members != null && requestmodel.Members.Count() > 0)
                            {
                                foreach (var item in requestmodel.Members)
                                {
                                    var calendarAttendeeObj = new CalendarAttendee();
                                    calendarAttendeeObj.email = item.Email;
                                    googleCalendarEventVMObj.attendees.Add(calendarAttendeeObj);
                                }
                            }
                            if (calendarSyncData == null)
                            {
                                var calendarActivity = await _calendarService.AddEvent(GoogleCalendarApiKey, googleCalendarEventVMObj, intProviderAppSecretDto.Email, intProviderAppSecretDto.Token);
                                calendarSyncActivityDto.CalendarEventId = calendarActivity.id;
                                calendarSyncActivityDto.TenantId = TenantId;
                                var AddUpdate = await _calendarSyncActivityService.CheckInsertOrUpdate(calendarSyncActivityDto);
                            }
                            else
                            {
                                googleCalendarEventVMObj.id = calendarSyncData.CalendarEventId;
                                var calendarActivity = await _calendarService.UpdateEvent(GoogleCalendarApiKey, googleCalendarEventVMObj, intProviderAppSecretDto.Email, intProviderAppSecretDto.Token);
                            }
                        }
                    }
                }


                await _hubContext.Clients.All.OnCustomerActivityEventEmit(requestmodel.CustomerId);
            }
            var resposemodel = _mapper.Map<CustomerActivityAddUpdateResponse>(requestmodel);
            return new OperationResult<CustomerActivityAddUpdateResponse>(true, System.Net.HttpStatusCode.OK,"", resposemodel);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<CustomerActivityDto>> MarkAsDone([FromBody] CustomerActivityDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);			
			TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            if(model.Id == null)
            {
                model.CreatedBy = UserId;
            }
            else
            {
                model.UpdatedBy = UserId;
            }
            model.TenantId = TenantId;
            model.IsDone = true;
            var customerActivityObj = await _customerActivityService.CheckInsertOrUpdate(model);
            await _hubContext.Clients.All.OnCustomerActivityEventEmit(model.CustomerId);
            return new OperationResult<CustomerActivityDto>(true, System.Net.HttpStatusCode.OK, "", model);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<CustomerActivityDto>> MaekAsToDo([FromBody] CustomerActivityDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);			
			TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            if(model.Id == null)
            {
                model.CreatedBy = UserId;
            }
            else
            {
                model.UpdatedBy = UserId;
            }
            model.TenantId = TenantId;
            model.IsDone = false;
            var customerActivityObj = await _customerActivityService.CheckInsertOrUpdate(model);
            await _hubContext.Clients.All.OnCustomerActivityEventEmit(model.CustomerId);
            return new OperationResult<CustomerActivityDto>(true, System.Net.HttpStatusCode.OK, "", model);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]        
        [HttpDelete]
        public async Task<OperationResult<CustomerActivityDeleteResponse>> Remove(CustomerActivityDeleteRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            var requestmodel = _mapper.Map<CustomerActivityDto>(model);
            if (requestmodel.Id != null)
            {
                var customerActivityMembers = await _customerActivityMemberService.DeleteByActivityId(requestmodel.Id.Value);

                //var customerModuleObj = _customModuleService.GetByName("Person");
                CustomModule? customModuleObj = null;
                var customTable = _customTableService.GetByName("Person");
                if (customTable != null)
                {
                    customModuleObj = _customModuleService.GetByCustomTable(customTable.Id);
                }

                if (requestmodel.CreatedBy != null)
                {
                    var intProviderAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUser(requestmodel.CreatedBy.Value);
                    if (customModuleObj != null && intProviderAppSecretObj != null)
                    {
                        CalendarSyncActivityDto calendarSyncActivityDto = new CalendarSyncActivityDto();
                        calendarSyncActivityDto.CreatedBy = UserId;
                        calendarSyncActivityDto.IntProviderAppSecretId = intProviderAppSecretObj.Id;
                        calendarSyncActivityDto.ActivityId = requestmodel.Id.Value;
                        calendarSyncActivityDto.ModuleId = customModuleObj.Id;
                        var calendarSyncData = _calendarSyncActivityService.GetCalendarSyncActivity(calendarSyncActivityDto);
                        if (calendarSyncData != null)
                        {
                            GoogleCalendarEventVM googleCalendarEventVM = new GoogleCalendarEventVM();
                            googleCalendarEventVM.id = calendarSyncData.CalendarEventId;
                            var intProviderAppSecretDto = await CheckAccessToken(requestmodel.CreatedBy.Value);
                            if (intProviderAppSecretDto != null && string.IsNullOrEmpty(intProviderAppSecretDto.error_description))
                            {
                                var customers = await _calendarService.DeleteEvent(GoogleCalendarApiKey, googleCalendarEventVM, intProviderAppSecretDto.Email, intProviderAppSecretDto.Token);
                                var deleted = _calendarSyncActivityService.DeleteCalendarSyncActivity(calendarSyncData.Id);
                            }
                        }

                    }
                }
                var activity = _customerActivityService.DeleteCustomerActivity(requestmodel);
                await _hubContext.Clients.All.OnCustomerActivityEventEmit(requestmodel.CustomerId);
                var responsemodel = _mapper.Map<CustomerActivityDeleteResponse>(requestmodel);
                return new OperationResult<CustomerActivityDeleteResponse>(true, System.Net.HttpStatusCode.OK,"", responsemodel);
            }
            else
            {
                var responsemodel = _mapper.Map<CustomerActivityDeleteResponse>(requestmodel);
                return new OperationResult<CustomerActivityDeleteResponse>(false, System.Net.HttpStatusCode.OK, "Please provide id", responsemodel);
            }
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<CustomerActivityDto>> Detail(long Id)
        {
            CustomerActivityDto customerActivityDto = new CustomerActivityDto();
            var customerActivityObj = _customerActivityService.GetById(Id);
            customerActivityDto = _mapper.Map<CustomerActivityDto>(customerActivityObj);
            if (customerActivityDto.CustomerActivityTypeId != null)
            {
                var customerActivityTypeObj = _customerActivityTypeService.GetById(customerActivityDto.CustomerActivityTypeId.Value);
                if (customerActivityTypeObj != null)
                {
                    customerActivityDto.CustomerActivityType = customerActivityTypeObj.Name;
                }
            }
            if (customerActivityDto.CustomerActivityAvailabilityId != null)
            {
                var customerActivityAvailabilityObj = _customerActivityAvailabilityService.GetById(customerActivityDto.CustomerActivityAvailabilityId.Value);
                if (customerActivityAvailabilityObj != null)
                {
                    customerActivityDto.CustomerActivityAvailability = customerActivityAvailabilityObj.Name;
                }
            }
            return new OperationResult<CustomerActivityDto>(true, System.Net.HttpStatusCode.OK, "", customerActivityDto);
        }
        //get all activity type        
        [HttpGet]
        public async Task<OperationResult<List<CustomerActivityTypeResponse>>> Types()
        {
            List<ActivityTypeDto> activityTypeDtoList = new List<ActivityTypeDto>();
            var activityTypes = _customerActivityTypeService.GetAll();
            activityTypeDtoList = _mapper.Map<List<ActivityTypeDto>>(activityTypes);
            var responseCustomerActivityTypes = _mapper.Map<List<CustomerActivityTypeResponse>>(activityTypeDtoList);
            return new OperationResult<List<CustomerActivityTypeResponse>>(true, System.Net.HttpStatusCode.OK,"", responseCustomerActivityTypes);            
        }

        [HttpGet]
        public async Task<OperationResult<List<CustomerActivityAvailResposne>>> AvailabilityList()
        {
            List<ActivityAvailabilityDto> activityAvailabilityDtoList = new List<ActivityAvailabilityDto>();
            var activityAvailabilities = _customerActivityAvailabilityService.GetAll();
            activityAvailabilityDtoList = _mapper.Map<List<ActivityAvailabilityDto>>(activityAvailabilities);
            var responseActivityAvailabilityDtoList = _mapper.Map<List<CustomerActivityAvailResposne>>(activityAvailabilityDtoList);
            return new OperationResult<List<CustomerActivityAvailResposne>>(true, System.Net.HttpStatusCode.OK,"", responseActivityAvailabilityDtoList);            
        }

        [HttpDelete("{Id}")]
        public async Task<OperationResult> RemoveActivityMember(long Id)
        {
            if (Id != null && Id > 0)
            {
                var data = _customerActivityMemberService.DeleteById(Id);

                return new OperationResult(true, System.Net.HttpStatusCode.OK, "", Id);
            }
            else
            {
                return new OperationResult(false, System.Net.HttpStatusCode.OK, "Please provide id", Id);
            }
        }

        private async Task<IntProviderAppSecretDto> CheckAccessToken(int userId)
        {
            string Token = "";

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            userId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            var intProviderAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUser(userId);
            var intProviderAppSecretDto = _mapper.Map<IntProviderAppSecretDto>(intProviderAppSecretObj);
            if (intProviderAppSecretObj != null)
            {

                var tokenInfo = await _calendarService.GetTokenInfo(intProviderAppSecretObj.Access_Token, GoogleCalendarSecret);
                if ((tokenInfo == null) || (tokenInfo != null && (!string.IsNullOrEmpty(tokenInfo.error_description))))
                {
                    GoogleCalendarTokenVM googleCalendarTokenVMObj = new GoogleCalendarTokenVM();
                    googleCalendarTokenVMObj.refresh_token = intProviderAppSecretObj.Refresh_Token;
                    googleCalendarTokenVMObj.client_id = GoogleCalendarClientId;
                    googleCalendarTokenVMObj.client_secret = GoogleCalendarSecret;
                    googleCalendarTokenVMObj.grant_type = "refresh_token";
                    googleCalendarTokenVMObj.scope = GoogleCalendarScope;
                    var accessTokenObj = await _calendarService.GetRefreshToken(GoogleCalendarSecret, googleCalendarTokenVMObj);
                    if (string.IsNullOrEmpty(accessTokenObj.error_description))
                    {
                        Token = "Bearer " + accessTokenObj.access_token;
                        intProviderAppSecretDto.Access_Token = accessTokenObj.access_token;
                        intProviderAppSecretDto.Id_Token = accessTokenObj.id_token;
                        intProviderAppSecretDto.LastAccessedOn = DateTime.UtcNow;
                        intProviderAppSecretDto.Token = Token;
                    }
                    else
                    {
                        intProviderAppSecretDto.error_description = accessTokenObj.error_description;
                        return intProviderAppSecretDto;
                    }

                }
                else
                {
                    Token = "Bearer " + intProviderAppSecretObj.Access_Token;
                    intProviderAppSecretDto.Token = Token;
                    intProviderAppSecretDto.LastAccessedOn = DateTime.UtcNow;
                }
                var AddUpdate = await _intProviderAppSecretService.CheckInsertOrUpdate(intProviderAppSecretDto);
            }
            return intProviderAppSecretDto;
            // return Token;
        }

    }
}
