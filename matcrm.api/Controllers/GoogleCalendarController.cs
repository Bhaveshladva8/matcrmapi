using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using matcrm.data.Context;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.data.Models.ViewModels;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.service.Services.ERP;
using System.Dynamic;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using matcrm.api.SignalR;
using matcrm.service.Utility;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class GoogleCalendarController : Controller
    {
        private readonly IGoogleCalendarService _calendarService;
        private readonly IIntProviderService _intProviderService;
        private readonly IIntProviderAppService _intProviderAppService;
        private readonly IIntProviderAppSecretService _intProviderAppSecretService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IOrganizationActivityService _organizationActivityService;
        private readonly ILeadActivityService _leadActivityService;
        private readonly ICalendarSyncActivityService _calendarSyncActivityService;
        private readonly ICustomModuleService _customModuleService;
        private readonly ICustomerActivityMemberService _customerActivityMemberService;
        private readonly IOrganizationActivityMemberService _organizationActivityMemberService;
        private readonly ILeadActivityMemberService _leadActivityMemberService;
        private readonly IMailAssignUserService _mailAssignUserService;
        private readonly IMailParticipantService _mailParticipantService;
        private readonly ITeamInboxService _teamInboxService;
        private readonly ITeamInboxAccessService _teamInboxAccessService;
        private readonly ICustomDomainEmailConfigService _customDomainEmailConfigService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;

        private string GoogleCalendarClientId, GoogleCalendarSecret, GoogleCalendarScope, GoogleCalendarApiKey;
        public GoogleCalendarController(
            IGoogleCalendarService calendarService,
            IIntProviderService intProviderService,
            IIntProviderAppService intProviderAppService,
            IIntProviderAppSecretService intProviderAppSecretService,
            ICustomerActivityService customerActivityService,
            IOrganizationActivityService organizationActivityService,
            ILeadActivityService leadActivityService,
            ICalendarSyncActivityService calendarSyncActivityService,
            ICustomModuleService customModuleService,
            ICustomerActivityMemberService customerActivityMemberService,
            IOrganizationActivityMemberService organizationActivityMemberService,
            ILeadActivityMemberService leadActivityMemberService,
            ITeamInboxService teamInboxService,
            ITeamInboxAccessService teamInboxAccessService,
            ICustomDomainEmailConfigService customDomainEmailConfigService,
            IMapper mapper
        // OneClappContext context
        )
        {
            _calendarService = calendarService;
            _intProviderService = intProviderService;
            _intProviderAppService = intProviderAppService;
            _intProviderAppSecretService = intProviderAppSecretService;
            _mapper = mapper;
            _customerActivityService = customerActivityService;
            _organizationActivityService = organizationActivityService;
            _leadActivityService = leadActivityService;
            _calendarSyncActivityService = calendarSyncActivityService;
            _customModuleService = customModuleService;
            _customerActivityMemberService = customerActivityMemberService;
            _organizationActivityMemberService = organizationActivityMemberService;
            _leadActivityMemberService = leadActivityMemberService;
            _teamInboxService = teamInboxService;
            _teamInboxAccessService = teamInboxAccessService;
            _customDomainEmailConfigService = customDomainEmailConfigService;
            GoogleCalendarClientId = OneClappContext.GoogleCalendarClientId;
            GoogleCalendarSecret = OneClappContext.GoogleCalendarSecretKey;
            GoogleCalendarApiKey = OneClappContext.GoogleCalendarApiKey;
            GoogleCalendarScope = OneClappContext.GoogleScopes;
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<GoogleCalendarEventVM>>> Events()
        {
            GoogleCalendarTokenVM googleCalendarTokenVMObj = new GoogleCalendarTokenVM();
            List<GoogleCalendarEventVM> googleCalendarEventVMList = new List<GoogleCalendarEventVM>();
            var googleCalendarKey = GoogleCalendarApiKey;

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            if (!string.IsNullOrEmpty(googleCalendarKey))
            {
                // var Token = await CheckAccessToken(Model.userId.Value);
                var intProviderAppSecretDto = await CheckAccessToken(UserId);
                // var intProviderAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUser(Model.userId.Value);
                if (intProviderAppSecretDto != null && string.IsNullOrEmpty(intProviderAppSecretDto.error_description))
                {
                    var customers = await _calendarService.GetEvents(intProviderAppSecretDto.Token, GoogleCalendarApiKey, intProviderAppSecretDto.Email);
                    if (customers != null && customers.Count() > 0)
                    {
                        googleCalendarEventVMList = customers;
                    }
                }
                else
                {
                    return new OperationResult<List<GoogleCalendarEventVM>>(false, System.Net.HttpStatusCode.OK, intProviderAppSecretDto.error_description, googleCalendarEventVMList);
                }

                return new OperationResult<List<GoogleCalendarEventVM>>(true, System.Net.HttpStatusCode.OK, "", googleCalendarEventVMList);

            }
            else
            {
                return new OperationResult<List<GoogleCalendarEventVM>>(false, System.Net.HttpStatusCode.OK, "", googleCalendarEventVMList);
            }
        }

        [HttpPost]
        public async Task<OperationResult<List<GoogleCalendarEventVM>>> EventDetail([FromBody] GoogleCalendarTokenVM Model)
        {
            List<GoogleCalendarEventVM> googleCalendarEventVMList = new List<GoogleCalendarEventVM>();
            var googleCalendarKey = GoogleCalendarApiKey;

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            if (!string.IsNullOrEmpty(googleCalendarKey))
            {
                // var userCalendarTokenObj = _intProviderAppSecretService.GetByUserAndEmail(Model.userId.Value, Model.email);

                // var Token = "Bearer ya29.a0ARrdaM9H73KZ0HF-Uhg-64MlSSttedMmk8lKmmZpKWLWsqwN-6EOtA0dtWoYD4lt7y1TKA1DTez-bg-zsH1ezMzwHxhzX4AXuwShfTOtEsuTvb30TM8XkwMtTx_RSAnMpzQu0th2QMMFz4coocjfljvMLnu0";
                // var Token = "Bearer ya29.a0ARrdaM9KQlgiULkkWTUxqYhZM9Q55iInNmjuBHmVon5yUWtU7moRJfYD7JKZY0dE4J_B-J3t4mqEEibAgjxaPlnQKHL1NoEE2dRo86a6o6E7PcBMy0f-dwdLiV0lrLSogRoGQPd4x3w52e6clOEZXCTcOxWdqQ";
                // var Token = "Bearer " + Model.access_token;
                var intProviderAppSecretDto = await CheckAccessToken(UserId);
                var customers = await _calendarService.GetEventById(intProviderAppSecretDto.Token, GoogleCalendarApiKey, Model.email, Model.eventId);
                if (customers != null && customers.Count() > 0)
                {
                    googleCalendarEventVMList = customers;
                }

                return new OperationResult<List<GoogleCalendarEventVM>>(true, System.Net.HttpStatusCode.OK, "", googleCalendarEventVMList);

            }
            else
            {
                return new OperationResult<List<GoogleCalendarEventVM>>(false, System.Net.HttpStatusCode.OK, "", googleCalendarEventVMList);
            }
        }

        [HttpPost]
        public async Task<OperationResult<GoogleCalendarTokenVM>> GoogleToken([FromBody] GoogleCalendarTokenVM model)
        {
            GoogleCalendarTokenVM googleCalendarTokenVMObj = new GoogleCalendarTokenVM();

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            var googleCalendarKey = GoogleCalendarApiKey;
            if (!string.IsNullOrEmpty(googleCalendarKey))
            {
                model.grant_type = "authorization_code";
                var customers = await _calendarService.GetAccessToken(googleCalendarKey, model);
                if (customers != null)
                {
                    googleCalendarTokenVMObj = customers;
                }

                if (googleCalendarTokenVMObj.error == null || googleCalendarTokenVMObj.error == "")
                {
                    if (UserId != null)
                    {
                        var IntProviderObj = _intProviderService.GetIntProvider("Google");
                        if (IntProviderObj != null)
                        {
                            var intProviderAppObj = _intProviderAppService.GetIntProviderAppByProviderId(IntProviderObj.Id, "Calendar");
                            IntProviderAppSecretDto intProviderAppSecretDto = new IntProviderAppSecretDto();
                            intProviderAppSecretDto.Access_Token = googleCalendarTokenVMObj.access_token;
                            intProviderAppSecretDto.Expires_In = googleCalendarTokenVMObj.expires_in;
                            intProviderAppSecretDto.Refresh_Token = googleCalendarTokenVMObj.refresh_token;
                            intProviderAppSecretDto.Scope = googleCalendarTokenVMObj.scope;
                            intProviderAppSecretDto.Token_Type = googleCalendarTokenVMObj.token_type;
                            intProviderAppSecretDto.Id_Token = googleCalendarTokenVMObj.id_token;
                            //intProviderAppSecretDto.CreatedBy = googleCalendarTokenVMObj.userId;
                            intProviderAppSecretDto.IntProviderAppId = intProviderAppObj.Id;
                            intProviderAppSecretDto.CreatedBy = UserId;
                            GoogleCalendarUser googleCalendarUserObj = await _calendarService.GetUserInfo("Bearer " + googleCalendarTokenVMObj.access_token, OneClappContext.GoogleSecretKey);
                            if (googleCalendarUserObj != null)
                            {
                                intProviderAppSecretDto.Email = googleCalendarUserObj.email;
                            }
                            var IntProvicerAppSecret = await _intProviderAppSecretService.CheckInsertOrUpdate(intProviderAppSecretDto);
                        }
                    }
                }
                return new OperationResult<GoogleCalendarTokenVM>(true, System.Net.HttpStatusCode.OK, "", googleCalendarTokenVMObj);
            }
            else
            {
                return new OperationResult<GoogleCalendarTokenVM>(false, System.Net.HttpStatusCode.OK, "", googleCalendarTokenVMObj);
            }
        }

        [HttpPost]
        public async Task<OperationResult<GoogleCalendarEventVM>> AddEvent([FromBody] GoogleCalendarEventVM model)
        {
            GoogleCalendarEventVM googleCalendarEventVMObj = new GoogleCalendarEventVM();
            var googleCalendarKey = GoogleCalendarApiKey;

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            if (!string.IsNullOrEmpty(googleCalendarKey))
            {
                if (UserId != null)
                {
                    // var intProviderAppSecretObj = _intProviderAppSecretService.GetIntProviderAppSecretByUser(model.UserId.Value);
                    // if (intProviderAppSecretObj != null)
                    // {
                    //     string Token;
                    //     var tokenInfo = await _calendarService.GetTokenInfo(intProviderAppSecretObj.Access_Token, googleCalendarKey);
                    //     if (tokenInfo.error_description != "" || tokenInfo.error_description != null)
                    //     {
                    //         GoogleCalendarTokenVM tokenVM = new GoogleCalendarTokenVM();
                    //         tokenVM.refresh_token = intProviderAppSecretObj.Refresh_Token;
                    //         tokenVM.client_id = GoogleCalendarClientId;
                    //         tokenVM.client_secret = GoogleCalendarSecret;
                    //         tokenVM.grant_type = "refresh_token";
                    //         tokenVM.scope = GoogleCalendarScope;
                    //         var accessTokenObj = await _calendarService.GetRefreshToken(googleCalendarKey, tokenVM);
                    //         Token = "Bearer " + accessTokenObj.access_token;
                    //     }
                    //     else
                    //     {
                    //         Token = "Bearer " + intProviderAppSecretObj.Access_Token;
                    //     }

                    //     model.access_token = null;
                    //     // var Token = "Bearer ya29.a0ARrdaM9H73KZ0HF-Uhg-64MlSSttedMmk8lKmmZpKWLWsqwN-6EOtA0dtWoYD4lt7y1TKA1DTez-bg-zsH1ezMzwHxhzX4AXuwShfTOtEsuTvb30TM8XkwMtTx_RSAnMpzQu0th2QMMFz4coocjfljvMLnu0";
                    //     var customers = await _calendarService.AddEvent(googleCalendarKey, model, googleCalendarId, Token);
                    //     if (customers != null)
                    //     {
                    //         calendarTokenObj = customers;
                    //     }
                    // }

                    var intProviderAppSecretDto = await CheckAccessToken(UserId);
                    // var intProviderAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUser(model.UserId.Value);
                    if (intProviderAppSecretDto != null)
                    {
                        model.creator.email = intProviderAppSecretDto.Email;
                        var customers = await _calendarService.AddEvent(googleCalendarKey, model, intProviderAppSecretDto.Email, intProviderAppSecretDto.Token);
                        if (customers != null)
                        {
                            googleCalendarEventVMObj = customers;
                        }
                    }
                }
                if (googleCalendarEventVMObj.error != null || googleCalendarEventVMObj.error_description != null)
                {
                    return new OperationResult<GoogleCalendarEventVM>(false, System.Net.HttpStatusCode.OK, googleCalendarEventVMObj.error_description, googleCalendarEventVMObj);
                }
                else
                {
                    return new OperationResult<GoogleCalendarEventVM>(true, System.Net.HttpStatusCode.OK, "", googleCalendarEventVMObj);
                }
            }
            else
            {
                return new OperationResult<GoogleCalendarEventVM>(false, System.Net.HttpStatusCode.OK, "", googleCalendarEventVMObj);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<OperationResult<GoogleCalendarEventVM>> SyncTask([FromBody] GoogleCalendarEventVM model)
        {

            // DateTime dt = DateTime.Now;
            // var utcTime = dt.ToUniversalTime();
            // var data = utcTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
            // Console.WriteLine(utcTime.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            GoogleCalendarEventVM googleCalendarEventVMObj = new GoogleCalendarEventVM();
            List<GoogleCalendarEventVM> googleCalendarEventVMList = new List<GoogleCalendarEventVM>();
            var intProviderAppSecretDto = await CheckAccessToken(UserId);
            if (intProviderAppSecretDto != null && string.IsNullOrEmpty(intProviderAppSecretDto.error_description))
            {
                var customerActivities = _customerActivityService.GetByUser(UserId);
                var organizationActivities = _organizationActivityService.GetByUser(UserId);
                var leadActivities = _leadActivityService.GetByUser(UserId);
                var calendarsyncActivitites = _calendarSyncActivityService.GetByUser(UserId);
                var moduleList = _customModuleService.GetAll();
                if (moduleList != null)
                {
                    var customerModuleObj = moduleList.Where(t => t.Name == "Person").FirstOrDefault();
                    var organizationModuleObj = moduleList.Where(t => t.Name == "Organization").FirstOrDefault();
                    var leadModuleObj = moduleList.Where(t => t.Name == "Lead").FirstOrDefault();

                    if (customerActivities != null && customerActivities.Count() > 0)
                    {
                        customerActivities = customerActivities.Where(t => t.ScheduleStartDate != null && t.ScheduleEndDate != null &&
                                                                    t.StartTime != null && t.EndTime != null).ToList();

                        foreach (var customerActivityObj in customerActivities)
                        {
                            var isExistData = calendarsyncActivitites.Where(t => t.ActivityId == customerActivityObj.Id && t.ModuleId == customerModuleObj.Id).FirstOrDefault();
                            var members = _customerActivityMemberService.GetAllByActivity(customerActivityObj.Id);
                            GoogleCalendarEventVM googleCalendarEventVMObj1 = new GoogleCalendarEventVM();
                            googleCalendarEventVMObj1.summary = customerActivityObj.Title;
                            googleCalendarEventVMObj1.description = customerActivityObj.Description;
                            googleCalendarEventVMObj1.start = new EventDate();
                            googleCalendarEventVMObj1.start.timeZone = "UTC";
                            if (customerActivityObj.ScheduleStartDate != null)
                            {
                                var startDateTime = Common.GetStartEndTime(customerActivityObj.ScheduleStartDate.Value, customerActivityObj.StartTime);

                                if (startDateTime != null)
                                {
                                    googleCalendarEventVMObj1.start.dateTime = startDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                                }
                            }
                            googleCalendarEventVMObj1.end = new EventDate();
                            googleCalendarEventVMObj1.end.timeZone = "UTC";
                            if (customerActivityObj.ScheduleEndDate != null)
                            {
                                var endDateTime = Common.GetStartEndTime(customerActivityObj.ScheduleEndDate.Value, customerActivityObj.EndTime);
                                if (endDateTime != null)
                                {
                                    googleCalendarEventVMObj1.end.dateTime = endDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                                }
                            }
                            googleCalendarEventVMObj1.creator.email = intProviderAppSecretDto.Email;

                            if (members != null && members.Count() > 0)
                            {
                                foreach (var customerMemberObj in members)
                                {
                                    CalendarAttendee calendarAttendeeObj1 = new CalendarAttendee();
                                    calendarAttendeeObj1.email = customerMemberObj.Email;
                                    googleCalendarEventVMObj1.attendees.Add(calendarAttendeeObj1);
                                }
                            }

                            if (isExistData == null)
                            {
                                CalendarSyncActivityDto calendarSyncActivityDto = new CalendarSyncActivityDto();
                                calendarSyncActivityDto.CreatedBy = UserId;
                                calendarSyncActivityDto.IntProviderAppSecretId = intProviderAppSecretDto.Id;
                                calendarSyncActivityDto.ActivityId = customerActivityObj.Id;
                                calendarSyncActivityDto.ModuleId = customerModuleObj.Id;

                                var calendarCustomerActivity = await _calendarService.AddEvent(GoogleCalendarApiKey, googleCalendarEventVMObj1, intProviderAppSecretDto.Email, intProviderAppSecretDto.Token);
                                if (string.IsNullOrEmpty(calendarCustomerActivity.error_description))
                                {
                                    calendarSyncActivityDto.CalendarEventId = calendarCustomerActivity.id;
                                    calendarSyncActivityDto.TenantId = TenantId;
                                    var AddUpdate = await _calendarSyncActivityService.CheckInsertOrUpdate(calendarSyncActivityDto);
                                }

                            }
                            else
                            {
                                googleCalendarEventVMObj1.id = isExistData.CalendarEventId;
                                var customers = await _calendarService.UpdateEvent(GoogleCalendarApiKey, googleCalendarEventVMObj1, intProviderAppSecretDto.Email, intProviderAppSecretDto.Token);
                            }
                        }
                    }

                    if (organizationActivities != null && organizationActivities.Count() > 0)
                    {
                        organizationActivities = organizationActivities.Where(t => t.ScheduleStartDate != null && t.ScheduleEndDate != null &&
                       t.StartTime != null && t.EndTime != null).ToList();
                        foreach (var organizationActivityObj in organizationActivities)
                        {
                            var isExistData1 = calendarsyncActivitites.Where(t => t.ActivityId == organizationActivityObj.Id && t.ModuleId == organizationModuleObj.Id).FirstOrDefault();

                            var organizationMembers = _organizationActivityMemberService.GetAllByActivity(organizationActivityObj.Id);

                            GoogleCalendarEventVM googleCalendarEventVMObj2 = new GoogleCalendarEventVM();
                            googleCalendarEventVMObj2.summary = organizationActivityObj.Title;
                            googleCalendarEventVMObj2.description = organizationActivityObj.Description;
                            googleCalendarEventVMObj2.start = new EventDate();
                            googleCalendarEventVMObj2.start.timeZone = "UTC";
                            if (organizationActivityObj.ScheduleStartDate != null)
                            {
                                var startDateTime = Common.GetStartEndTime(organizationActivityObj.ScheduleStartDate.Value, organizationActivityObj.StartTime);

                                if (startDateTime != null)
                                {
                                    googleCalendarEventVMObj2.start.dateTime = startDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                                }
                            }

                            googleCalendarEventVMObj2.end = new EventDate();
                            googleCalendarEventVMObj2.end.timeZone = "UTC";
                            if (organizationActivityObj.ScheduleEndDate != null)
                            {
                                var endDateTime = Common.GetStartEndTime(organizationActivityObj.ScheduleEndDate.Value, organizationActivityObj.EndTime);
                                if (endDateTime != null)
                                {
                                    googleCalendarEventVMObj2.end.dateTime = endDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                                }
                            }

                            googleCalendarEventVMObj2.creator.email = intProviderAppSecretDto.Email;

                            if (organizationMembers != null && organizationMembers.Count() > 0)
                            {
                                foreach (var organizationMemberObj in organizationMembers)
                                {
                                    CalendarAttendee calendarAttendeeObj2 = new CalendarAttendee();
                                    calendarAttendeeObj2.email = organizationMemberObj.Email;
                                    googleCalendarEventVMObj2.attendees.Add(calendarAttendeeObj2);
                                }
                            }

                            if (isExistData1 == null)
                            {
                                CalendarSyncActivityDto calendarSyncActivityDto1 = new CalendarSyncActivityDto();
                                calendarSyncActivityDto1.CreatedBy = UserId;
                                calendarSyncActivityDto1.IntProviderAppSecretId = intProviderAppSecretDto.Id;
                                calendarSyncActivityDto1.ActivityId = organizationActivityObj.Id;
                                calendarSyncActivityDto1.ModuleId = organizationModuleObj.Id;
                                var calendarOrganizationActivity = await _calendarService.AddEvent(GoogleCalendarApiKey, googleCalendarEventVMObj2, intProviderAppSecretDto.Email, intProviderAppSecretDto.Token);
                                if (string.IsNullOrEmpty(calendarOrganizationActivity.error_description))
                                {
                                    calendarSyncActivityDto1.CalendarEventId = calendarOrganizationActivity.id;
                                    calendarSyncActivityDto1.TenantId = TenantId;
                                    var AddUpdate = await _calendarSyncActivityService.CheckInsertOrUpdate(calendarSyncActivityDto1);
                                }
                            }
                            else
                            {
                                googleCalendarEventVMObj2.id = isExistData1.CalendarEventId;
                                var customers = await _calendarService.UpdateEvent(GoogleCalendarApiKey, googleCalendarEventVMObj2, intProviderAppSecretDto.Email, intProviderAppSecretDto.Token);
                            }
                        }
                    }

                    if (leadActivities != null && leadActivities.Count() > 0)
                    {
                        leadActivities = leadActivities.Where(t => t.ScheduleStartDate != null && t.ScheduleEndDate != null &&
                       t.StartTime != null && t.EndTime != null).ToList();
                        foreach (var leadActivityObj in leadActivities)
                        {
                            var isExistData2 = calendarsyncActivitites.Where(t => t.ActivityId == leadActivityObj.Id && t.ModuleId == leadModuleObj.Id).FirstOrDefault();

                            var leadMembers = _leadActivityMemberService.GetAllByActivity(leadActivityObj.Id);

                            GoogleCalendarEventVM googleCalendarEventVMObj3 = new GoogleCalendarEventVM();
                            googleCalendarEventVMObj3.summary = leadActivityObj.Title;
                            googleCalendarEventVMObj3.description = leadActivityObj.Description;
                            googleCalendarEventVMObj3.start = new EventDate();
                            googleCalendarEventVMObj3.start.timeZone = "UTC";
                            if (leadActivityObj.ScheduleStartDate != null)
                            {
                                var startDateTime = Common.GetStartEndTime(leadActivityObj.ScheduleStartDate.Value, leadActivityObj.StartTime);

                                if (startDateTime != null)
                                {
                                    googleCalendarEventVMObj3.start.dateTime = startDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                                }
                            }

                            googleCalendarEventVMObj3.end = new EventDate();
                            googleCalendarEventVMObj3.end.timeZone = "UTC";
                            if (leadActivityObj.ScheduleEndDate != null)
                            {
                                var endDateTime = Common.GetStartEndTime(leadActivityObj.ScheduleEndDate.Value, leadActivityObj.EndTime);
                                if (endDateTime != null)
                                {
                                    googleCalendarEventVMObj3.end.dateTime = endDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                                }
                            }

                            googleCalendarEventVMObj3.creator.email = intProviderAppSecretDto.Email;

                            if (leadMembers != null && leadMembers.Count() > 0)
                            {
                                foreach (var leadMemberObj in leadMembers)
                                {
                                    CalendarAttendee calendarAttendeeObj3 = new CalendarAttendee();
                                    calendarAttendeeObj3.email = leadMemberObj.Email;
                                    googleCalendarEventVMObj3.attendees.Add(calendarAttendeeObj3);
                                }
                            }

                            if (isExistData2 == null)
                            {
                                CalendarSyncActivityDto calendarSyncActivityDto2 = new CalendarSyncActivityDto();
                                calendarSyncActivityDto2.CreatedBy = UserId;
                                calendarSyncActivityDto2.IntProviderAppSecretId = intProviderAppSecretDto.Id;
                                calendarSyncActivityDto2.ActivityId = leadActivityObj.Id;
                                calendarSyncActivityDto2.ModuleId = leadModuleObj.Id;
                                var calendarLeadActivity = await _calendarService.AddEvent(GoogleCalendarApiKey, googleCalendarEventVMObj3, intProviderAppSecretDto.Email, intProviderAppSecretDto.Token);
                                if (string.IsNullOrEmpty(calendarLeadActivity.error_description))
                                {
                                    calendarSyncActivityDto2.CalendarEventId = calendarLeadActivity.id;
                                    calendarSyncActivityDto2.TenantId = TenantId;
                                    var AddUpdate = await _calendarSyncActivityService.CheckInsertOrUpdate(calendarSyncActivityDto2);
                                }
                            }
                            else
                            {
                                googleCalendarEventVMObj3.id = isExistData2.CalendarEventId;
                                var customers = await _calendarService.UpdateEvent(GoogleCalendarApiKey, googleCalendarEventVMObj3, intProviderAppSecretDto.Email, intProviderAppSecretDto.Token);
                            }
                        }
                    }
                }

                return new OperationResult<GoogleCalendarEventVM>(true, System.Net.HttpStatusCode.OK, "", googleCalendarEventVMObj);
            }
            else
            {
                return new OperationResult<GoogleCalendarEventVM>(false, System.Net.HttpStatusCode.OK, intProviderAppSecretDto.error_description, googleCalendarEventVMObj);
            }

        }

        [HttpPut]
        public async Task<OperationResult<GoogleCalendarEventVM>> UpdateEvent([FromBody] GoogleCalendarEventVM model)
        {
            GoogleCalendarEventVM googleCalendarEventVMObj = new GoogleCalendarEventVM();

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            var googleCalendarKey = GoogleCalendarApiKey;
            if (!string.IsNullOrEmpty(googleCalendarKey) && (UserId != null))
            {
                var intProviderAppSecretDto = await CheckAccessToken(UserId);

                // var intProviderAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUser(model.UserId.Value);
                if (intProviderAppSecretDto != null)
                {
                    //  obj2.start.dateTime = startDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                    var customers = await _calendarService.UpdateEvent(googleCalendarKey, model, intProviderAppSecretDto.Email, intProviderAppSecretDto.Token);
                    if (customers != null)
                    {
                        googleCalendarEventVMObj = customers;
                        if (googleCalendarEventVMObj.error != null || googleCalendarEventVMObj.error_description != null)
                        {
                            return new OperationResult<GoogleCalendarEventVM>(false, System.Net.HttpStatusCode.OK, googleCalendarEventVMObj.error_description, googleCalendarEventVMObj);
                        }
                    }
                }
                return new OperationResult<GoogleCalendarEventVM>(true, System.Net.HttpStatusCode.OK, "", googleCalendarEventVMObj);
            }
            else
            {
                return new OperationResult<GoogleCalendarEventVM>(false, System.Net.HttpStatusCode.OK, "", googleCalendarEventVMObj);
            }
        }


        [HttpDelete]
        public async Task<OperationResult<GoogleCalendarEventVM>> RemoveEvent([FromBody] GoogleCalendarEventVM model)
        {
            GoogleCalendarEventVM googleCalendarEventVMObj = new GoogleCalendarEventVM();
            var googleCalendarKey = GoogleCalendarApiKey;

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            if (!string.IsNullOrEmpty(googleCalendarKey) && UserId != null && !string.IsNullOrEmpty(model.id))
            {
                var intProviderAppSecretDto = await CheckAccessToken(UserId);
                // var intProviderAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUser(model.UserId.Value);
                if (intProviderAppSecretDto != null)
                {
                    var customers = await _calendarService.DeleteEvent(googleCalendarKey, model, intProviderAppSecretDto.Email, intProviderAppSecretDto.Token);
                    if (customers != null)
                    {
                        googleCalendarEventVMObj = customers;
                        if (googleCalendarEventVMObj.error != null || googleCalendarEventVMObj.error_description != null)
                        {
                            return new OperationResult<GoogleCalendarEventVM>(false, System.Net.HttpStatusCode.OK, googleCalendarEventVMObj.error_description, googleCalendarEventVMObj);
                        }
                    }
                    else
                    {
                        return new OperationResult<GoogleCalendarEventVM>(true, System.Net.HttpStatusCode.OK, "", googleCalendarEventVMObj);
                    }
                }
                return new OperationResult<GoogleCalendarEventVM>(true, System.Net.HttpStatusCode.OK, "", googleCalendarEventVMObj);
            }
            else
            {
                return new OperationResult<GoogleCalendarEventVM>(false, System.Net.HttpStatusCode.OK, "", googleCalendarEventVMObj);
            }
        }

        [HttpPost]
        public async Task<OperationResult<GoogleCalendarTokenVM>> RefreshToken([FromBody] GoogleCalendarTokenVM model)
        {
            GoogleCalendarTokenVM googleCalendarTokenVMObj = new GoogleCalendarTokenVM();
            var googleCalendarKey = GoogleCalendarApiKey;
            if (!string.IsNullOrEmpty(googleCalendarKey))
            {
                // var Token = "Bearer ya29.a0ARrdaM9H73KZ0HF-Uhg-64MlSSttedMmk8lKmmZpKWLWsqwN-6EOtA0dtWoYD4lt7y1TKA1DTez-bg-zsH1ezMzwHxhzX4AXuwShfTOtEsuTvb30TM8XkwMtTx_RSAnMpzQu0th2QMMFz4coocjfljvMLnu0";
                var customers = await _calendarService.GetRefreshToken(googleCalendarKey, model);
                if (customers != null)
                {
                    googleCalendarTokenVMObj = customers;
                }
                return new OperationResult<GoogleCalendarTokenVM>(true, System.Net.HttpStatusCode.OK, "", googleCalendarTokenVMObj);
            }
            else
            {
                return new OperationResult<GoogleCalendarTokenVM>(false, System.Net.HttpStatusCode.OK, "", googleCalendarTokenVMObj);
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

        [Authorize]
        [HttpGet]
        public async Task<OperationResult<List<GoogleCalendarGetAllResponse>>> List()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var intProviderAppSecretList = _intProviderAppSecretService.GetAllByUser(UserId);
            var intProviderAppSecretDtoList = _mapper.Map<List<IntProviderAppSecretDto>>(intProviderAppSecretList);

            var teamInboxes = _teamInboxService.GetByUser(UserId);
            var teamInboxAccess = _teamInboxAccessService.GetByTeamMate(UserId);

            List<long> teamInboxIntProviderIdList = new List<long>();
            if (teamInboxAccess != null && teamInboxAccess.Count() > 0)
            {
                foreach (var item in teamInboxAccess)
                {
                    if (item.TeamInbox != null && item.TeamInbox.IntProviderAppSecretId != null)
                    {
                        teamInboxIntProviderIdList.Add(item.TeamInbox.IntProviderAppSecretId.Value);
                    }
                }
            }
            if (intProviderAppSecretDtoList != null && intProviderAppSecretDtoList.Count() > 0)
            {
                intProviderAppSecretDtoList = intProviderAppSecretDtoList.Where(p => !teamInboxIntProviderIdList.Contains(p.Id.Value)).ToList();
                foreach (var intProviderAppSecretItem in intProviderAppSecretDtoList)
                {
                    if (intProviderAppSecretItem.IntProviderApp != null)
                    {
                        intProviderAppSecretItem.IntProviderAppName = intProviderAppSecretItem.IntProviderApp.Name;

                        if (intProviderAppSecretItem.IntProviderApp.IntProvider != null)
                        {
                            intProviderAppSecretItem.ProviderName = intProviderAppSecretItem.IntProviderApp.IntProvider.Name;
                        }
                    }
                    else
                    {
                        var customDomainEmailConfigObj = _customDomainEmailConfigService.GetByUserAndEmail(UserId, intProviderAppSecretItem.Email);
                        if (customDomainEmailConfigObj != null)
                        {
                            intProviderAppSecretItem.CustomDomainEmailConfigId = customDomainEmailConfigObj.Id;
                            intProviderAppSecretItem.CustomDomainEmailConfigDto = _mapper.Map<CustomDomainEmailConfigDto>(customDomainEmailConfigObj);
                        }
                    }
                }
            }

            // var intProviderAppSecretListDto = intProviderAppSecretObj.Select(t => new IntProviderAppSecretDto
            // {
            //     Id = t.Id,
            //     Email = t.Email,
            //     CreatedBy = t.CreatedBy,
            //     IsDefault = t.IsDefault,
            //     ProviderName = t.IntProviderApp.IntProvider.Name,
            //     IntProviderAppId = t.IntProviderAppId,
            //     IntProviderAppName = t.IntProviderApp.Name
            // }).ToList();
            // var intProviderAppSecretListDto = _mapper.Map<List<IntProviderAppSecretDto>>(intProviderAppSecretObj);
            var responseListDto = _mapper.Map<List<GoogleCalendarGetAllResponse>>(intProviderAppSecretDtoList);
            return new OperationResult<List<GoogleCalendarGetAllResponse>>(true, System.Net.HttpStatusCode.OK, "", responseListDto);
        }

        private async Task<OperationResult<List<IntProviderAppSecretDto>>> Accounts(int UserId)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            var intProviderAppSecretList = _intProviderAppSecretService.GetAllByUser(UserId);
            var intProviderAppSecretDtoList = _mapper.Map<List<IntProviderAppSecretDto>>(intProviderAppSecretList);
            if (intProviderAppSecretDtoList != null && intProviderAppSecretDtoList.Count() > 0)
            {
                foreach (var intProviderAppSecretItem in intProviderAppSecretDtoList)
                {
                    if (intProviderAppSecretItem.IntProviderApp != null)
                    {
                        intProviderAppSecretItem.IntProviderAppName = intProviderAppSecretItem.IntProviderApp.Name;

                        if (intProviderAppSecretItem.IntProviderApp.IntProvider != null)
                        {
                            intProviderAppSecretItem.ProviderName = intProviderAppSecretItem.IntProviderApp.IntProvider.Name;
                        }
                    }
                }
            }

            // var intProviderAppSecretListDto = intProviderAppSecretObj.Select(t => new IntProviderAppSecretDto
            // {
            //     Id = t.Id,
            //     Email = t.Email,
            //     CreatedBy = t.CreatedBy,
            //     IsDefault = t.IsDefault,
            //     ProviderName = t.IntProviderApp.IntProvider.Name,
            //     IntProviderAppId = t.IntProviderAppId,
            //     IntProviderAppName = t.IntProviderApp.Name
            // }).ToList();
            // var intProviderAppSecretListDto = _mapper.Map<List<IntProviderAppSecretDto>>(intProviderAppSecretObj);
            return new OperationResult<List<IntProviderAppSecretDto>>(true, System.Net.HttpStatusCode.OK, "", intProviderAppSecretDtoList);
        }

        [Authorize]
        [HttpPut]
        public async Task<OperationResult<GoogleCalendarUpdateAccountResponse>> UpdateAccount([FromBody] GoogleCalendarUpdateAccountRequest Model)
        {
            var requestmodel = _mapper.Map<IntProviderAppSecretDto>(Model);
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            requestmodel.CreatedBy = UserId;

            if (requestmodel.CreatedBy != null)
            {
                var allSecretByUserList = _intProviderAppSecretService.GetAllByUser(requestmodel.CreatedBy.Value);
                if (requestmodel.Id != null)
                {
                    var appSecretObj = allSecretByUserList.Where(t => t.Id == requestmodel.Id.Value).FirstOrDefault();
                    // var appSecretDto = _mapper.Map<IntProviderAppSecretDto>(appSecretObj);
                    appSecretObj.IsDefault = requestmodel.IsDefault;
                    appSecretObj.IsSelect = requestmodel.IsSelect;

                    var updatedSecretObj = await _intProviderAppSecretService.UpdateIntProviderAppSecret(appSecretObj, appSecretObj.Id);
                    if (requestmodel.IsDefault == true && allSecretByUserList != null && allSecretByUserList.Count() > 1)
                    {
                        foreach (var item in allSecretByUserList)
                        {
                            if (item.Id != appSecretObj.Id)
                            {
                                item.IsDefault = false;
                                var updatedSecretObj1 = await _intProviderAppSecretService.UpdateIntProviderAppSecret(item, item.Id);
                            }
                        }
                    }
                }
                var responsemodel = _mapper.Map<GoogleCalendarUpdateAccountResponse>(requestmodel);
                return new OperationResult<GoogleCalendarUpdateAccountResponse>(true, System.Net.HttpStatusCode.OK, "", responsemodel);
            }
            else
            {
                var responsemodel = _mapper.Map<GoogleCalendarUpdateAccountResponse>(requestmodel);
                return new OperationResult<GoogleCalendarUpdateAccountResponse>(false, System.Net.HttpStatusCode.OK, "", responsemodel);
            }
        }

        [Authorize]
        [HttpPut]
        public async Task<OperationResult<GoogleCalendarSelectResponse>> Select([FromBody] GoogleCalendarSelectRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var data = await Accounts(UserId);
            var requestmodel = _mapper.Map<SecretDto>(model);
            List<IntProviderAppSecretDto> intProviderAppSecretDtoList = data.Model;
            if (requestmodel.SelectedIds != null && requestmodel.SelectedIds.Count() > 0)
            {
                foreach (var secretObj in intProviderAppSecretDtoList)
                {
                    if (secretObj.Id != null)
                    {
                        if (requestmodel.SelectedIds.Contains(secretObj.Id.Value))
                        {
                            if (secretObj.IsSelect == false)
                            {
                                secretObj.IsSelect = true;
                                var intProviderAppSecretObj = _mapper.Map<IntProviderAppSecret>(secretObj);
                                await _intProviderAppSecretService.UpdateIntProviderAppSecret(intProviderAppSecretObj, intProviderAppSecretObj.Id);
                            }
                        }
                        else
                        {
                            if (secretObj.IsSelect == true)
                            {
                                secretObj.IsSelect = false;
                                var intProviderAppSecretObj = _mapper.Map<IntProviderAppSecret>(secretObj);
                                await _intProviderAppSecretService.UpdateIntProviderAppSecret(intProviderAppSecretObj, intProviderAppSecretObj.Id);
                            }
                        }
                    }
                }
            }
            var responsemodel = _mapper.Map<GoogleCalendarSelectResponse>(requestmodel);
            return new OperationResult<GoogleCalendarSelectResponse>(true, System.Net.HttpStatusCode.OK, "", responsemodel);
        }


        // [HttpDelete]
        // [Authorize]
        // public async Task<OperationResult<GoogleCalendarDeleteResponse>> Remove([FromBody] GoogleCalendarDeleteRequest Model)
        // {
        //     ClaimsPrincipal user = this.User as ClaimsPrincipal;
        //     UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
        //     var requestmodel = _mapper.Map<IntProviderAppSecretDto>(Model);
        //     if (requestmodel.Id != null)
        //     {
        //         var deleteCustomDomainEmailConfig = await _customDomainEmailConfigService.DeleteByIntProviderAppSecretId(requestmodel.Id.Value);
        //         var teamInboxObj = _teamInboxService.GetByAppSecretId(requestmodel.Id.Value, UserId);
        //         if (teamInboxObj != null)
        //         {
        //             teamInboxObj.IntProviderAppSecretId = null;
        //             var teamInboxDto = _mapper.Map<TeamInboxDto>(teamInboxObj);
        //             var addUpdate = _teamInboxService.CheckInsertOrUpdate(teamInboxDto);
        //         }
        //         var allSecretByUser = await _intProviderAppSecretService.DeleteIntProviderAppSecret(requestmodel.Id.Value);

        //         var responsemodel = _mapper.Map<GoogleCalendarDeleteResponse>(requestmodel);
        //         return new OperationResult<GoogleCalendarDeleteResponse>(true, System.Net.HttpStatusCode.OK,"", responsemodel);
        //     }
        //     else
        //     {
        //         var responsemodel = _mapper.Map<GoogleCalendarDeleteResponse>(requestmodel);
        //         return new OperationResult<GoogleCalendarDeleteResponse>(false, System.Net.HttpStatusCode.OK, "Please provide id", responsemodel);
        //     }
        // }

        [Authorize]
        [HttpDelete("{Id}")]
        public async Task<OperationResult> Remove(long Id)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            if (Id != null && Id > 0)
            {
                var deleteCustomDomainEmailConfig = await _customDomainEmailConfigService.DeleteByIntProviderAppSecretId(Id);
                var teamInboxObj = _teamInboxService.GetByAppSecretId(Id, UserId);
                if (teamInboxObj != null)
                {
                    teamInboxObj.IntProviderAppSecretId = null;
                    var teamInboxDto = _mapper.Map<TeamInboxDto>(teamInboxObj);
                    var addUpdate = _teamInboxService.CheckInsertOrUpdate(teamInboxDto);
                }
                var allSecretByUser = await _intProviderAppSecretService.DeleteIntProviderAppSecret(Id);

                return new OperationResult(true, System.Net.HttpStatusCode.OK, "", Id);
            }
            else
            {
                return new OperationResult(false, System.Net.HttpStatusCode.OK, "Please provide id", Id);
            }
        }

    }
}