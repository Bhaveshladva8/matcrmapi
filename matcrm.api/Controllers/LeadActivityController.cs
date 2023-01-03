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
    public class LeadActivityController : Controller
    {

        private readonly ILeadActivityService _leadActivityService;
        private readonly IActivityTypeService _leadActivityTypeService;
        private readonly IActivityAvailabilityService _leadActivityAvailabilityService;
        private readonly ILeadActivityMemberService _leadActivityMemberService;
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
        private SendEmail sendEmail;
        private IMapper _mapper;
        private CustomFieldLogic customFieldLogic;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private readonly ICustomTableService _customTableService;
        private string GoogleCalendarClientId, GoogleCalendarSecret, GoogleCalendarScope, GoogleCalendarApiKey;
        private int UserId = 0;
        private int TenantId = 0;
        public LeadActivityController(
            ILeadActivityService leadActivityService,
            IActivityTypeService leadActivityTypeService,
            IActivityAvailabilityService leadActivityAvailabilityService,
            ILeadActivityMemberService leadActivityMemberService,
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
            _leadActivityService = leadActivityService;
            _leadActivityTypeService = leadActivityTypeService;
            _leadActivityAvailabilityService = leadActivityAvailabilityService;
            _leadActivityMemberService = leadActivityMemberService;
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

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet("{LeadId}")]
        public async Task<OperationResult<List<LeadActivityGetAllResponse>>> List(long LeadId)
        {
            List<LeadActivityDto> leadActivityDtoList = new List<LeadActivityDto>();
            var LeadActivityList = _leadActivityService.GetByLead(LeadId);

            leadActivityDtoList = _mapper.Map<List<LeadActivityDto>>(LeadActivityList);

            var leadActivityTypes = _leadActivityTypeService.GetAll();
            var leadAvailabilities = _leadActivityAvailabilityService.GetAll();
            var users = _userSerVice.GetAll();
            if (leadActivityDtoList != null && leadActivityDtoList.Count() > 0)
            {
                foreach (var item in leadActivityDtoList)
                {
                    if (item.Id != null)
                    {
                        var leadActivityMembers = _leadActivityMemberService.GetAllByActivity(item.Id.Value);
                        item.Members = _mapper.Map<List<LeadActivityMemberDto>>(leadActivityMembers);
                        if (item.LeadActivityAvailabilityId != null)
                        {
                            if (leadAvailabilities != null)
                            {
                                var leadActivityAvailableObj = leadAvailabilities.Where(t => t.Id == item.LeadActivityAvailabilityId).FirstOrDefault();
                                if (leadActivityAvailableObj != null)
                                {
                                    item.LeadActivityAvailability = leadActivityAvailableObj.Name;
                                }
                            }
                        }

                        if (item.LeadActivityTypeId != null)
                        {
                            if (leadActivityTypes != null)
                            {
                                var leadActivityTypeObj = leadActivityTypes.Where(t => t.Id == item.LeadActivityTypeId).FirstOrDefault();
                                if (leadActivityTypeObj != null)
                                {
                                    item.LeadActivityType = leadActivityTypeObj.Name;
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
            }
            var responseLeadActivityDtoList = _mapper.Map<List<LeadActivityGetAllResponse>>(leadActivityDtoList);
            return new OperationResult<List<LeadActivityGetAllResponse>>(true, System.Net.HttpStatusCode.OK, "", responseLeadActivityDtoList);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<LeadActivityAddUpdateResponse>> AddUpdate([FromBody] LeadActivityAddUpdateRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var requestmodel = _mapper.Map<LeadActivityDto>(model);
            if (requestmodel.Id == null)
            {
                requestmodel.CreatedBy = UserId;
            }
            else
            {
                requestmodel.UpdatedBy = UserId;
            }
            requestmodel.TenantId = TenantId;
            var leadActivityObj = _leadActivityService.CheckInsertOrUpdate(requestmodel);

            if (leadActivityObj != null)
            {
                if (requestmodel.Members != null && requestmodel.Members.Count() > 0)
                {
                    foreach (var item in requestmodel.Members)
                    {
                        LeadActivityMemberDto leadActivityMemberDto = new LeadActivityMemberDto();
                        leadActivityMemberDto.Email = item.Email;
                        leadActivityMemberDto.LeadActivityId = leadActivityObj.Id;
                        var isExist = _leadActivityMemberService.GetActivityMember(leadActivityMemberDto);

                        if (isExist == null)
                        {
                            var AddUpdate = _leadActivityMemberService.CheckInsertOrUpdate(leadActivityMemberDto);
                            await sendEmail.LeadActivityInviteMember(item.Email, item.Email, requestmodel);
                        }
                    }
                }
                requestmodel.Id = leadActivityObj.Id;
                if (requestmodel.CreatedBy != null)
                {
                    var intProviderAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUser(requestmodel.CreatedBy.Value);

                    if (intProviderAppSecretObj != null)
                    {
                        //var organizationModuleObj = _customModuleService.GetByName("Lead");
                        CustomModule? customModuleObj = null;
                        var customTable = _customTableService.GetByName("Lead");
                        if (customTable != null)
                        {
                            customModuleObj = _customModuleService.GetByCustomTable(customTable.Id);
                        }

                        if (customModuleObj != null)
                        {
                            CalendarSyncActivityDto calendarSyncActivityDto = new CalendarSyncActivityDto();
                            calendarSyncActivityDto.CreatedBy = requestmodel.CreatedBy;
                            calendarSyncActivityDto.IntProviderAppSecretId = intProviderAppSecretObj.Id;
                            calendarSyncActivityDto.ActivityId = leadActivityObj.Id;
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
                                //  eventVM.end.timeZone = "UTC";
                                // eventVM.end.dateTime = model.EventEndTime;

                                googleCalendarEventVMObj.start = new EventDate();
                                googleCalendarEventVMObj.start.timeZone = "UTC";
                                if (leadActivityObj.ScheduleStartDate != null)
                                {
                                    var startDateTime = Common.GetStartEndTime(leadActivityObj.ScheduleStartDate.Value, leadActivityObj.StartTime);

                                    if (startDateTime != null)
                                    {
                                        googleCalendarEventVMObj.start.dateTime = startDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                                    }
                                }
                                googleCalendarEventVMObj.end = new EventDate();
                                googleCalendarEventVMObj.end.timeZone = "UTC";
                                if (leadActivityObj.ScheduleEndDate != null)
                                {
                                    var endDateTime = Common.GetStartEndTime(leadActivityObj.ScheduleEndDate.Value, leadActivityObj.EndTime);
                                    if (endDateTime != null)
                                    {
                                        googleCalendarEventVMObj.end.dateTime = endDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                                    }
                                }

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
                                    calendarSyncActivityDto.CreatedBy = UserId;
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

                await _hubContext.Clients.All.OnLeadActivityEventEmit(requestmodel.LeadId);
            }
            var responsenodel = _mapper.Map<LeadActivityAddUpdateResponse>(requestmodel);
            return new OperationResult<LeadActivityAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "", responsenodel);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<LeadActivityDto>> MarkAsDone([FromBody] LeadActivityDto model)
        {
            model.IsDone = true;
            var leadActivityObj = _leadActivityService.CheckInsertOrUpdate(model);
            await _hubContext.Clients.All.OnLeadActivityEventEmit(model.LeadId);
            return new OperationResult<LeadActivityDto>(true, System.Net.HttpStatusCode.OK, "", model);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<LeadActivityDto>> MarkAsToDo([FromBody] LeadActivityDto model)
        {
            model.IsDone = false;
            var leadActivityObj = _leadActivityService.CheckInsertOrUpdate(model);
            await _hubContext.Clients.All.OnLeadActivityEventEmit(model.LeadId);
            return new OperationResult<LeadActivityDto>(true, System.Net.HttpStatusCode.OK, "", model);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpDelete]
        public async Task<OperationResult<LeadActivityDeleteResponse>> Remove(LeadActivityDeleteRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            var requestmodel = _mapper.Map<LeadActivityDto>(model);
            if (requestmodel.Id != null)
            {
                var members = _leadActivityMemberService.DeleteByActivityId(requestmodel.Id.Value);

                //var leadModuleObj = _customModuleService.GetByName("Lead");
                CustomModule? customModuleObj = null;
                var customTable = _customTableService.GetByName("Lead");
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
                            GoogleCalendarEventVM googleCalendarEventVMObj = new GoogleCalendarEventVM();
                            googleCalendarEventVMObj.id = calendarSyncData.CalendarEventId;
                            var intProviderAppSecretDto = await CheckAccessToken(UserId);
                            if (intProviderAppSecretDto != null && string.IsNullOrEmpty(intProviderAppSecretDto.error_description))
                            {
                                var customers = await _calendarService.DeleteEvent(GoogleCalendarApiKey, googleCalendarEventVMObj, intProviderAppSecretDto.Email, intProviderAppSecretDto.Token);
                                var deleted = _calendarSyncActivityService.DeleteCalendarSyncActivity(calendarSyncData.Id);
                            }
                        }

                    }
                }

                var activity = _leadActivityService.DeleteLeadActivity(requestmodel);
                await _hubContext.Clients.All.OnLeadActivityEventEmit(requestmodel.LeadId);
                var resposnemodel = _mapper.Map<LeadActivityDeleteResponse>(requestmodel);
                return new OperationResult<LeadActivityDeleteResponse>(true, System.Net.HttpStatusCode.OK, "Lead activity deleted successfully", resposnemodel);
            }
            else
            {
                var resposnemodel = _mapper.Map<LeadActivityDeleteResponse>(requestmodel);
                return new OperationResult<LeadActivityDeleteResponse>(false, System.Net.HttpStatusCode.OK, "Please provide id", resposnemodel);
            }
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<LeadActivityDto>> Detail(long Id)
        {
            LeadActivityDto leadActivityDto = new LeadActivityDto();
            var LeadActivityObj = _leadActivityService.GetById(Id);
            leadActivityDto = _mapper.Map<LeadActivityDto>(LeadActivityObj);
            if (leadActivityDto.LeadActivityTypeId != null)
            {
                var leadActivityTypeObj = _leadActivityTypeService.GetById(leadActivityDto.LeadActivityTypeId.Value);
                if (leadActivityTypeObj != null)
                {
                    leadActivityDto.LeadActivityType = leadActivityTypeObj.Name;
                }
            }
            if (leadActivityDto.LeadActivityAvailabilityId != null)
            {
                var LeadActivityAvailabilityObj = _leadActivityAvailabilityService.GetById(leadActivityDto.LeadActivityAvailabilityId.Value);
                if (LeadActivityAvailabilityObj != null)
                {
                    leadActivityDto.LeadActivityAvailability = LeadActivityAvailabilityObj.Name;
                }
            }
            return new OperationResult<LeadActivityDto>(true, System.Net.HttpStatusCode.OK, "", leadActivityDto);
        }

        //get activity type        
        [HttpGet]
        public async Task<OperationResult<List<LeadActivityTypeResponse>>> ActivityTypes()
        {

            List<ActivityTypeDto> activityTypeDtoList = new List<ActivityTypeDto>();
            var activityTypeList = _leadActivityTypeService.GetAll();
            activityTypeDtoList = _mapper.Map<List<ActivityTypeDto>>(activityTypeList);
            var responseActivityTypeDtoList = _mapper.Map<List<LeadActivityTypeResponse>>(activityTypeDtoList);
            return new OperationResult<List<LeadActivityTypeResponse>>(true, System.Net.HttpStatusCode.OK, "", responseActivityTypeDtoList);
        }


        [HttpGet]
        public async Task<OperationResult<List<LeadActivityAvailabilityResponse>>> Availabilities()
        {
            List<ActivityAvailabilityDto> activityAvailabilityDtoList = new List<ActivityAvailabilityDto>();
            var activityAvailabilityList = _leadActivityAvailabilityService.GetAll();
            activityAvailabilityDtoList = _mapper.Map<List<ActivityAvailabilityDto>>(activityAvailabilityList);
            var resposneAvailability = _mapper.Map<List<LeadActivityAvailabilityResponse>>(activityAvailabilityDtoList);
            return new OperationResult<List<LeadActivityAvailabilityResponse>>(true, System.Net.HttpStatusCode.OK, "", resposneAvailability);
        }


        [HttpDelete("{Id}")]
        public async Task<OperationResult> RemoveActivityMember(long Id)
        {
            if (Id != null && Id > 0)
            {
                var data = _leadActivityMemberService.DeleteById(Id);
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