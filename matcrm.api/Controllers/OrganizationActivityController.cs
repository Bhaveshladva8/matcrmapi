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
using matcrm.data.Context;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class OrganizationActivityController : Controller
    {

        private readonly IOrganizationActivityService _organizationActivityService;
        private readonly IActivityTypeService _organizationActivityTypeService;
        private readonly IActivityAvailabilityService _organizationActivityAvailabilityService;
        private readonly IOrganizationActivityMemberService _organizationActivityMemberService;
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
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private string GoogleCalendarClientId, GoogleCalendarSecret, GoogleCalendarScope, GoogleCalendarApiKey;
        private CustomFieldLogic customFieldLogic;
        private int UserId = 0;
        private int TenantId = 0;
        public OrganizationActivityController(
            IOrganizationActivityService organizationActivityService,
            IActivityTypeService organizationActivityTypeService,
            IActivityAvailabilityService organizationActivityAvailabilityService,
            IOrganizationActivityMemberService organizationActivityMemberService,
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
            _organizationActivityService = organizationActivityService;
            _organizationActivityTypeService = organizationActivityTypeService;
            _organizationActivityMemberService = organizationActivityMemberService;
            _organizationActivityAvailabilityService = organizationActivityAvailabilityService;
            _emailTemplateService = emailTemplateService;
            _emailLogService = emailLogService;
            _emailProviderService = emailProviderService;
            _emailConfigService = emailConfigService;
            _userSerVice = userSerVice;
            _calendarService = calendarService;
            _intProviderService = intProviderService;
            _intProviderAppService = intProviderAppService;
            _intProviderAppSecretService = intProviderAppSecretService;
            _customModuleService = customModuleService;
            _calendarSyncActivityService = calendarSyncActivityService;
            _mapper = mapper;
            _hubContext = hubContext;
            _customTableService = customTableService;
            sendEmail = new SendEmail(emailTemplateService, emailLogService, emailConfigService, emailProviderService, mapper);
            GoogleCalendarClientId = OneClappContext.GoogleCalendarClientId;
            GoogleCalendarSecret = OneClappContext.GoogleCalendarSecretKey;
            GoogleCalendarApiKey = OneClappContext.GoogleCalendarApiKey;
            GoogleCalendarScope = OneClappContext.GoogleScopes;
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet("{OrganizationId}")]
        public async Task<OperationResult<List<OrganizationActivityGetAllResponse>>> List(long OrganizationId)
        {
            List<OrganizationActivityDto> organizationActivityDtoList = new List<OrganizationActivityDto>();

            var organizationActivityList = _organizationActivityService.GetByOrganization(OrganizationId);

            organizationActivityDtoList = _mapper.Map<List<OrganizationActivityDto>>(organizationActivityList);

            var organizationActivityTypes = _organizationActivityTypeService.GetAll();
            var organizationAvailabilities = _organizationActivityAvailabilityService.GetAll();
            var users = _userSerVice.GetAll();
            if (organizationActivityDtoList != null && organizationActivityDtoList.Count() > 0)
            {
                foreach (var item in organizationActivityDtoList)
                {
                    if (item.Id != null)
                    {
                        var organizationActivityMembers = _organizationActivityMemberService.GetAllByActivity(item.Id.Value);
                        item.Members = _mapper.Map<List<OrganizationActivityMemberDto>>(organizationActivityMembers);
                        if (item.OrganizationActivityAvailabilityId != null)
                        {
                            if (organizationAvailabilities != null && organizationAvailabilities.Count() > 0)
                            {
                                var organizationActivityAvailableObj = organizationAvailabilities.Where(t => t.Id == item.OrganizationActivityAvailabilityId).FirstOrDefault();
                                if (organizationActivityAvailableObj != null)
                                {
                                    item.OrganizationActivityAvailability = organizationActivityAvailableObj.Name;
                                }
                            }
                        }
                    }

                    if (item.OrganizationActivityTypeId != null)
                    {
                        if (organizationActivityTypes != null)
                        {
                            var organizationActivityTypeObj = organizationActivityTypes.Where(t => t.Id == item.OrganizationActivityTypeId).FirstOrDefault();
                            if (organizationActivityTypeObj != null)
                            {
                                item.OrganizationActivityType = organizationActivityTypeObj.Name;
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
            var responseOrganizationActivities = _mapper.Map<List<OrganizationActivityGetAllResponse>>(organizationActivityDtoList);
            return new OperationResult<List<OrganizationActivityGetAllResponse>>(true, System.Net.HttpStatusCode.OK, "", responseOrganizationActivities);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<OrganizationActivityAddUpdateResponse>> AddUpdate([FromBody] OrganizationActivityAddUpdateRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var requestmodel = _mapper.Map<OrganizationActivityDto>(model);
            if (requestmodel.Id == null)
            {
                requestmodel.CreatedBy = UserId;
            }
            else
            {
                requestmodel.UpdatedBy = UserId;
            }
            requestmodel.TenantId = TenantId;

            var organizationActivityObj = await _organizationActivityService.CheckInsertOrUpdate(requestmodel);
            if (organizationActivityObj != null)
            {
                if (requestmodel.Members != null && requestmodel.Members.Count() > 0)
                {
                    foreach (var item in requestmodel.Members)
                    {
                        OrganizationActivityMemberDto organizationActivityMemberDto = new OrganizationActivityMemberDto();
                        organizationActivityMemberDto.Email = item.Email;
                        organizationActivityMemberDto.OrganizationActivityId = organizationActivityObj.Id;
                        var isExist = _organizationActivityMemberService.GetActivityMember(organizationActivityMemberDto);

                        if (isExist == null)
                        {
                            var AddUpdate = await _organizationActivityMemberService.CheckInsertOrUpdate(organizationActivityMemberDto);
                            await sendEmail.OrganizationActivityInviteMember(item.Email, item.Email, requestmodel);
                        }
                    }
                }
                requestmodel.Id = organizationActivityObj.Id;

                if (requestmodel.CreatedBy != null)
                {
                    var intProviderAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUser(requestmodel.CreatedBy.Value);

                    if (intProviderAppSecretObj != null)
                    {
                        //var organizationModuleObj = _customModuleService.GetByName("Organization");

                        CustomModule? customModuleObj = null;
                        var customTable = _customTableService.GetByName("Organization");
                        if (customTable != null)
                        {
                            customModuleObj = _customModuleService.GetByCustomTable(customTable.Id);
                        }

                        if (customModuleObj != null)
                        {
                            CalendarSyncActivityDto calendarSyncActivityDto = new CalendarSyncActivityDto();
                            calendarSyncActivityDto.CreatedBy = requestmodel.CreatedBy;
                            calendarSyncActivityDto.IntProviderAppSecretId = intProviderAppSecretObj.Id;
                            calendarSyncActivityDto.ActivityId = organizationActivityObj.Id;
                            calendarSyncActivityDto.ModuleId = customModuleObj.Id;
                            var calendarSyncData = _calendarSyncActivityService.GetCalendarSyncActivity(calendarSyncActivityDto);
                            var intProviderAppSecretDto = await CheckAccessToken(UserId);
                            if (intProviderAppSecretDto != null && string.IsNullOrEmpty(intProviderAppSecretDto.error_description))
                            {
                                GoogleCalendarEventVM googleCalendarEventVMObj = new GoogleCalendarEventVM();
                                googleCalendarEventVMObj.summary = requestmodel.Title;
                                googleCalendarEventVMObj.description = requestmodel.Description;
                                // eventVM.start = new EventDate();
                                //  eventVM.start.timeZone = "UTC";
                                // eventVM.start.dateTime = model.EventStartTime;
                                // eventVM.end = new EventDate();
                                // eventVM.end.timeZone = "UTC";
                                // eventVM.end.dateTime = model.EventEndTime;

                                googleCalendarEventVMObj.start = new EventDate();
                                googleCalendarEventVMObj.start.timeZone = "UTC";
                                if (organizationActivityObj.ScheduleStartDate != null)
                                {
                                    var startDateTime = Common.GetStartEndTime(organizationActivityObj.ScheduleStartDate.Value, organizationActivityObj.StartTime);

                                    // eventVM.start.dateTime = startDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                                    if (startDateTime != null)
                                    {
                                        googleCalendarEventVMObj.start.dateTime = startDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                                    }
                                }
                                googleCalendarEventVMObj.end = new EventDate();
                                googleCalendarEventVMObj.end.timeZone = "UTC";
                                if (organizationActivityObj.ScheduleEndDate != null)
                                {
                                    var endDateTime = Common.GetStartEndTime(organizationActivityObj.ScheduleEndDate.Value, organizationActivityObj.EndTime);
                                    if (endDateTime != null)
                                    {
                                        googleCalendarEventVMObj.end.dateTime = endDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                                    }
                                }
                                // eventVM.end.dateTime = endDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");

                                googleCalendarEventVMObj.creator.email = intProviderAppSecretDto.Email;
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
                }


                await _hubContext.Clients.All.OnOrganizationActivityEventEmit(requestmodel.OrganizationId);
            }

            var responsemodel = _mapper.Map<OrganizationActivityAddUpdateResponse>(requestmodel);
            return new OperationResult<OrganizationActivityAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "", responsemodel);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<OrganizationActivityDto>> MarkAsDone([FromBody] OrganizationActivityDto model)
        {
            model.IsDone = true;
            var organizationActivityObj = await _organizationActivityService.CheckInsertOrUpdate(model);
            await _hubContext.Clients.All.OnOrganizationActivityEventEmit(model.OrganizationId);
            return new OperationResult<OrganizationActivityDto>(true, System.Net.HttpStatusCode.OK, "", model);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<OrganizationActivityDto>> MarkAsToDo([FromBody] OrganizationActivityDto model)
        {
            model.IsDone = false;
            var organizationActivityObj = await _organizationActivityService.CheckInsertOrUpdate(model);
            await _hubContext.Clients.All.OnOrganizationActivityEventEmit(model.OrganizationId);
            return new OperationResult<OrganizationActivityDto>(true, System.Net.HttpStatusCode.OK, "", model);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpDelete]
        public async Task<OperationResult<OrganizationActivityDeleteResponse>> Remove(OrganizationActivityDeleteRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var requestmodel = _mapper.Map<OrganizationActivityDto>(model);
            if (requestmodel.Id != null)
            {
                var members = await _organizationActivityMemberService.DeleteByActivityId(requestmodel.Id.Value);

                //var organizationModuleObj = _customModuleService.GetByName("Organization");

                CustomModule? customModuleObj = null;
                var customTable = _customTableService.GetByName("Organization");
                if (customTable != null)
                {
                    customModuleObj = _customModuleService.GetByCustomTable(customTable.Id);
                }

                if (requestmodel.CreatedBy != null)
                {
                    var intProviderAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUser(model.CreatedBy.Value);
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
                            GoogleCalendarEventVM googleCalendarEventVMObj = new GoogleCalendarEventVM();
                            googleCalendarEventVMObj.id = calendarSyncData.CalendarEventId;
                            var intProviderAppSecretDto = await CheckAccessToken(requestmodel.CreatedBy.Value);
                            if (intProviderAppSecretDto != null && string.IsNullOrEmpty(intProviderAppSecretDto.error_description))
                            {
                                var customers = await _calendarService.DeleteEvent(GoogleCalendarApiKey, googleCalendarEventVMObj, intProviderAppSecretDto.Email, intProviderAppSecretDto.Token);
                                var deleted = _calendarSyncActivityService.DeleteCalendarSyncActivity(calendarSyncData.Id);
                            }
                        }

                    }
                }

                var activity = _organizationActivityService.DeleteOrganizationActivity(requestmodel);
                await _hubContext.Clients.All.OnOrganizationActivityEventEmit(requestmodel.OrganizationId);

                var responsemodel = _mapper.Map<OrganizationActivityDeleteResponse>(requestmodel);
                return new OperationResult<OrganizationActivityDeleteResponse>(true, System.Net.HttpStatusCode.OK, "", responsemodel);
            }
            else
            {
                var responsemodel = _mapper.Map<OrganizationActivityDeleteResponse>(requestmodel);
                return new OperationResult<OrganizationActivityDeleteResponse>(true, System.Net.HttpStatusCode.OK, "", responsemodel);
            }
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<OrganizationActivityDto>> Detail(long Id)
        {

            OrganizationActivityDto organizationActivityDto = new OrganizationActivityDto();
            var organizationActivityObj = _organizationActivityService.GetById(Id);
            organizationActivityDto = _mapper.Map<OrganizationActivityDto>(organizationActivityObj);
            if (organizationActivityDto.OrganizationActivityTypeId != null)
            {
                var organizationActivityTypeObj = _organizationActivityTypeService.GetById(organizationActivityDto.OrganizationActivityTypeId.Value);
                if (organizationActivityTypeObj != null)
                {
                    organizationActivityDto.OrganizationActivityType = organizationActivityTypeObj.Name;
                }
            }
            if (organizationActivityDto.OrganizationActivityAvailabilityId != null)
            {
                var organizationActivityAvailabilityObj = _organizationActivityAvailabilityService.GetById(organizationActivityDto.OrganizationActivityAvailabilityId.Value);
                if (organizationActivityAvailabilityObj != null)
                {
                    organizationActivityDto.OrganizationActivityAvailability = organizationActivityAvailabilityObj.Name;
                }
            }
            return new OperationResult<OrganizationActivityDto>(true, System.Net.HttpStatusCode.OK, "", organizationActivityDto);
        }

        [HttpGet]
        public async Task<OperationResult<List<OrganizationActivityTypeResponse>>> ActivityTypes()
        {

            List<ActivityTypeDto> activityTypeDtoList = new List<ActivityTypeDto>();
            var activityTypeList = _organizationActivityTypeService.GetAll();
            activityTypeDtoList = _mapper.Map<List<ActivityTypeDto>>(activityTypeList);

            var responseListtype = _mapper.Map<List<OrganizationActivityTypeResponse>>(activityTypeDtoList);
            return new OperationResult<List<OrganizationActivityTypeResponse>>(true, System.Net.HttpStatusCode.OK,"", responseListtype);            
        }

        [HttpGet]
        public async Task<OperationResult<List<OrganizationActivityAvailabilityResponse>>> Availabilities()
        {
            List<ActivityAvailabilityDto> activityAvailabilityDtoList = new List<ActivityAvailabilityDto>();
            var activityAvailabilityList = _organizationActivityAvailabilityService.GetAll();
            activityAvailabilityDtoList = _mapper.Map<List<ActivityAvailabilityDto>>(activityAvailabilityList);

            var responseActivityAvailList = _mapper.Map<List<OrganizationActivityAvailabilityResponse>>(activityAvailabilityDtoList);
            return new OperationResult<List<OrganizationActivityAvailabilityResponse>>(true, System.Net.HttpStatusCode.OK,"", responseActivityAvailList);
        }

        [HttpDelete("{Id}")]
        public async Task<OperationResult> RemoveActivityMember(long Id)
        {           
            if (Id != null && Id > 0)
            {
                var data = _organizationActivityMemberService.DeleteById(Id);
                
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