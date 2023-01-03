using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using matcrm.data.Context;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.data.Models.ViewModels;
using matcrm.data.Models.ViewModels.Calendar;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.service.Services.ERP;
using matcrm.service.Utility;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class CalendarController : Controller
    {
        private readonly ILogger<CalendarController> _logger;
        private readonly IGoogleCalendarService _calendarService;
        private readonly IIntProviderService _intProviderService;
        private readonly IIntProviderAppService _intProviderAppService;
        private readonly IIntProviderAppSecretService _intProviderAppSecretService;
        private readonly IUserService _userSerVice;
        private readonly ICustomerAttachmentService _customerAttachmentService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ITeamInboxService _teamInboxService;
        private readonly ITeamInboxAccessService _teamInboxAccessService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IOrganizationActivityService _organizationActivityService;
        private readonly ILeadActivityService _leadActivityService;
        private readonly ICalendarSyncActivityService _calendarSyncActivityService;
        private readonly ICustomModuleService _customModuleService;
        private readonly ICustomerActivityMemberService _customerActivityMemberService;
        private readonly IOrganizationActivityMemberService _organizationActivityMemberService;
        private readonly ILeadActivityMemberService _leadActivityMemberService;
        private IMapper _mapper;
        private Calendar calendar;
        private int UserId = 0;
        private int TenantId = 0;

        public CalendarController(ILogger<CalendarController> logger,
            IGoogleCalendarService calendarService,
            IIntProviderService intProviderService,
            IIntProviderAppService intProviderAppService,
            IIntProviderAppSecretService intProviderAppSecretService,
            IUserService userService,
            ICustomerAttachmentService customerAttachmentService,
            IHostingEnvironment hostingEnvironment,
            ITeamInboxService teamInboxService,
            ITeamInboxAccessService teamInboxAccessService,
             ICustomerActivityService customerActivityService,
            IOrganizationActivityService organizationActivityService,
            ILeadActivityService leadActivityService,
            ICalendarSyncActivityService calendarSyncActivityService,
            ICustomModuleService customModuleService,
            ICustomerActivityMemberService customerActivityMemberService,
            IOrganizationActivityMemberService organizationActivityMemberService,
            ILeadActivityMemberService leadActivityMemberService,
            IMapper mapper,
            OneClappContext context)
        {
            _logger = logger;
            _intProviderService = intProviderService;
            _intProviderAppService = intProviderAppService;
            _intProviderAppSecretService = intProviderAppSecretService;
            _userSerVice = userService;
            _customerAttachmentService = customerAttachmentService;
            _hostingEnvironment = hostingEnvironment;
            _teamInboxService = teamInboxService;
            _teamInboxAccessService = teamInboxAccessService;
            _customerActivityService = customerActivityService;
            _organizationActivityService = organizationActivityService;
            _leadActivityService = leadActivityService;
            _calendarSyncActivityService = calendarSyncActivityService;
            _customModuleService = customModuleService;
            _customerActivityMemberService = customerActivityMemberService;
            _organizationActivityMemberService = organizationActivityMemberService;
            _leadActivityMemberService = leadActivityMemberService;
            _mapper = mapper;
            calendar = new Calendar(context, userService, hostingEnvironment, intProviderAppService, intProviderAppSecretService, customerAttachmentService, calendarService);
        }

        //get all calendar events        
        [HttpGet]
        public async Task<OperationResult<List<CalendarGetEventsResponse>>> Events()
        {
            List<IntProviderAppSecretDto> intProviderAppSecretListDtoList = new List<IntProviderAppSecretDto>();
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            MSCalendarEvent mSCalendarEventObj = new MSCalendarEvent();

            var intProviderAppSecretsList = _intProviderAppSecretService.GetAllByUser(UserId);
            intProviderAppSecretsList = intProviderAppSecretsList.Where(t => t.IntProviderApp != null && (!string.IsNullOrEmpty(t.IntProviderApp.Name))).ToList();
            intProviderAppSecretListDtoList = _mapper.Map<List<IntProviderAppSecretDto>>(intProviderAppSecretsList);

            var teamInboxes = _teamInboxService.GetByUser(UserId);
            // foreach (var teamInboxObj in teamInboxes)
            // {
            //     var isExistData = intProviderAppSecretList.Where(t => t.Id == teamInboxObj.IntProviderAppSecretId).FirstOrDefault();
            //     if(isExistData != null){
            //         intProviderAppSecretList.Remove(isExistData);
            //     }
            // }
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

            intProviderAppSecretListDtoList = intProviderAppSecretListDtoList.Where(p => !teamInboxIntProviderIdList.Contains(p.Id.Value) && p.IsSelect == true).ToList();
            if (intProviderAppSecretListDtoList != null && intProviderAppSecretListDtoList.Count() > 0)
            {
                foreach (var intProviderAppSecretItem in intProviderAppSecretListDtoList)
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

                foreach (var intProviderAppSecretObj in intProviderAppSecretListDtoList)
                {
                    InboxVM inboxVMObj = new InboxVM();
                    inboxVMObj.SelectedEmail = intProviderAppSecretObj.Email;
                    inboxVMObj.Top = 20;
                    inboxVMObj.Skip = 0;
                    inboxVMObj.Provider = intProviderAppSecretObj.ProviderName;
                    inboxVMObj.ProviderApp = intProviderAppSecretObj.IntProviderAppName;
                    var data = await calendar.GetEvents(UserId, inboxVMObj);
                    if (intProviderAppSecretObj.ProviderName.ToLower() == "microsoft")
                    {
                        if (data.value != null && data.value.Count() > 0)
                        {
                            foreach (var MSCalendarEventObj in data.value)
                            {
                                GoogleMicrosoftCalendarEventVM googleMicrosoftCalendarEventVMObj = new GoogleMicrosoftCalendarEventVM();
                                googleMicrosoftCalendarEventVMObj.start = _mapper.Map<CalendarEventDate>(MSCalendarEventObj.start);
                                googleMicrosoftCalendarEventVMObj.end = _mapper.Map<CalendarEventDate>(MSCalendarEventObj.end);
                                googleMicrosoftCalendarEventVMObj.body = _mapper.Map<CalendarEventBody>(MSCalendarEventObj.body);
                                if (MSCalendarEventObj.attendees != null && MSCalendarEventObj.attendees.Count() > 0)
                                {
                                    foreach (var attendeeObj in MSCalendarEventObj.attendees)
                                    {
                                        GoogleMSCalendarAttendee googleMSCalendarAttendeeObj = new GoogleMSCalendarAttendee();
                                        googleMSCalendarAttendeeObj.status = _mapper.Map<CalendarEventAttendeeStatus>(attendeeObj.status);
                                        // googleMSCalendarAttendeeObj.emailAddress = _mapper.Map<CalendarEventEmail>(attendeeObj.emailAddress);
                                        googleMSCalendarAttendeeObj.emailAddress = new CalendarEventEmail();
                                        googleMSCalendarAttendeeObj.emailAddress.address = attendeeObj.emailAddress.address;
                                        googleMSCalendarAttendeeObj.emailAddress.name = attendeeObj.emailAddress.name;
                                        googleMSCalendarAttendeeObj.email = attendeeObj.emailAddress.address;
                                        googleMicrosoftCalendarEventVMObj.attendees.Add(googleMSCalendarAttendeeObj);
                                    }
                                    // googleMicrosoftCalendarEventVM.attendees = _mapper.Map<List<GoogleMSCalendarAttendee>>(MSCalendarEventObj.attendees);
                                }
                                // if (MSCalendarEventObj.locations.Count() > 0)
                                // {
                                //     googleMicrosoftCalendarEventVM.locations = _mapper.Map<List<CalendarEventLocation>>(MSCalendarEventObj.locations);
                                // }
                                // if (MSCalendarEventObj.location != null)
                                // {
                                //     googleMicrosoftCalendarEventVM.location = _mapper.Map<CalendarEventLocation>(MSCalendarEventObj.location);
                                // }
                                if (MSCalendarEventObj.organizer != null)
                                {
                                    googleMicrosoftCalendarEventVMObj.organizer = _mapper.Map<EventCreatorOrganizerTest>(MSCalendarEventObj.organizer);
                                }
                                googleMicrosoftCalendarEventVMObj.id = MSCalendarEventObj.id;
                                googleMicrosoftCalendarEventVMObj.createdDateTime = MSCalendarEventObj.createdDateTime;
                                googleMicrosoftCalendarEventVMObj.lastModifiedDateTime = MSCalendarEventObj.lastModifiedDateTime;
                                googleMicrosoftCalendarEventVMObj.changeKey = MSCalendarEventObj.changeKey;
                                googleMicrosoftCalendarEventVMObj.reminderMinutesBeforeStart = MSCalendarEventObj.reminderMinutesBeforeStart;
                                googleMicrosoftCalendarEventVMObj.transactionId = MSCalendarEventObj.transactionId;
                                googleMicrosoftCalendarEventVMObj.isReminderOn = MSCalendarEventObj.isReminderOn;
                                googleMicrosoftCalendarEventVMObj.hasAttachments = MSCalendarEventObj.hasAttachments;
                                googleMicrosoftCalendarEventVMObj.subject = MSCalendarEventObj.subject;
                                googleMicrosoftCalendarEventVMObj.bodyPreview = MSCalendarEventObj.bodyPreview;
                                googleMicrosoftCalendarEventVMObj.description = MSCalendarEventObj.bodyPreview;
                                googleMicrosoftCalendarEventVMObj.importance = MSCalendarEventObj.importance;
                                googleMicrosoftCalendarEventVMObj.sensitivity = MSCalendarEventObj.sensitivity;
                                googleMicrosoftCalendarEventVMObj.isAllDay = MSCalendarEventObj.isAllDay;
                                googleMicrosoftCalendarEventVMObj.isCancelled = MSCalendarEventObj.isCancelled;
                                googleMicrosoftCalendarEventVMObj.isOrganizer = MSCalendarEventObj.isOrganizer;
                                googleMicrosoftCalendarEventVMObj.responseRequested = MSCalendarEventObj.responseRequested;
                                googleMicrosoftCalendarEventVMObj.showAs = MSCalendarEventObj.showAs;
                                googleMicrosoftCalendarEventVMObj.type = MSCalendarEventObj.type;
                                googleMicrosoftCalendarEventVMObj.startDate = MSCalendarEventObj.startDate;
                                googleMicrosoftCalendarEventVMObj.endDate = MSCalendarEventObj.endDate;
                                googleMicrosoftCalendarEventVMObj.startTime = MSCalendarEventObj.startTime;
                                googleMicrosoftCalendarEventVMObj.endTime = MSCalendarEventObj.endTime;
                                googleMicrosoftCalendarEventVMObj.hideAttendees = MSCalendarEventObj.hideAttendees;
                                googleMicrosoftCalendarEventVMObj.Code = MSCalendarEventObj.Code;
                                googleMicrosoftCalendarEventVMObj.IntProviderAppSecretId = intProviderAppSecretObj.Id;
                                intProviderAppSecretObj.Events.Add(googleMicrosoftCalendarEventVMObj);

                            }
                        }
                        // List<GoogleMicrosoftCalendarEventVM> events = _mapper.Map<List<GoogleMicrosoftCalendarEventVM>>(data.value);
                        // intProviderAppSecretObj.Events = events;
                    }
                    else if (intProviderAppSecretObj.ProviderName.ToLower() == "google")
                    {
                        if (data.Items != null && data.Items.Count() > 0)
                        {
                            foreach (var googleCalendarEventVMObj in data.Items)
                            {
                                GoogleMicrosoftCalendarEventVM googleMicrosoftCalendarEventVMObj = new GoogleMicrosoftCalendarEventVM();
                                if (googleCalendarEventVMObj.start != null)
                                {
                                    googleMicrosoftCalendarEventVMObj.start.dateTime = googleCalendarEventVMObj.start.dateTime;
                                    googleMicrosoftCalendarEventVMObj.start.timeZone = googleCalendarEventVMObj.start.timeZone;
                                }
                                if (googleCalendarEventVMObj.end != null)
                                {
                                    googleMicrosoftCalendarEventVMObj.end.dateTime = googleCalendarEventVMObj.end.dateTime;
                                    googleMicrosoftCalendarEventVMObj.end.timeZone = googleCalendarEventVMObj.end.timeZone;
                                }
                                if (googleCalendarEventVMObj.start != null && googleCalendarEventVMObj.end != null && string.IsNullOrEmpty(googleCalendarEventVMObj.end.dateTime)
                                && string.IsNullOrEmpty(googleCalendarEventVMObj.start.dateTime) && !string.IsNullOrEmpty(googleCalendarEventVMObj.end.date) && !string.IsNullOrEmpty(googleCalendarEventVMObj.start.date))
                                {
                                    googleMicrosoftCalendarEventVMObj.isAllDay = true;
                                    // googleMicrosoftCalendarEventVM.end.date = googleCalendarEventVMObj.end.date;
                                    // googleMicrosoftCalendarEventVM.start.date = googleCalendarEventVMObj.start.date;
                                    googleMicrosoftCalendarEventVMObj.end.dateTime = googleCalendarEventVMObj.end.date;
                                    googleMicrosoftCalendarEventVMObj.start.dateTime = googleCalendarEventVMObj.start.date;
                                }

                                var date = Convert.ToDateTime(googleMicrosoftCalendarEventVMObj.start.dateTime);
                                var date1 = Convert.ToDateTime(googleMicrosoftCalendarEventVMObj.end.dateTime);
                                if ((date1 - date).TotalDays == 1 && date.Hour == 0 && date.Minute == 0 && date1.Hour == 0 && date1.Minute == 0)
                                {
                                    googleMicrosoftCalendarEventVMObj.isAllDay = true;
                                }

                                // googleMicrosoftCalendarEventVM.start = _mapper.Map<CalendarEventDate>(googleCalendarEventVMObj.start);
                                // googleMicrosoftCalendarEventVM.end = _mapper.Map<CalendarEventDate>(googleCalendarEventVMObj.end);
                                foreach (var attendeeObj in googleCalendarEventVMObj.attendees)
                                {
                                    GoogleMSCalendarAttendee googleMSCalendarAttendeeObj = new GoogleMSCalendarAttendee();
                                    googleMSCalendarAttendeeObj.email = attendeeObj.email;
                                    googleMicrosoftCalendarEventVMObj.attendee.Add(googleMSCalendarAttendeeObj);
                                }
                                googleMicrosoftCalendarEventVMObj.etag = googleCalendarEventVMObj.etag;
                                googleMicrosoftCalendarEventVMObj.id = googleCalendarEventVMObj.id;
                                googleMicrosoftCalendarEventVMObj.status = googleCalendarEventVMObj.status;
                                googleMicrosoftCalendarEventVMObj.htmlLink = googleCalendarEventVMObj.htmlLink;
                                googleMicrosoftCalendarEventVMObj.created = googleCalendarEventVMObj.created;
                                googleMicrosoftCalendarEventVMObj.created = googleCalendarEventVMObj.created;
                                googleMicrosoftCalendarEventVMObj.colorId = googleCalendarEventVMObj.colorId;
                                googleMicrosoftCalendarEventVMObj.description = googleCalendarEventVMObj.description;
                                googleMicrosoftCalendarEventVMObj.iCalUID = googleCalendarEventVMObj.iCalUID;
                                googleMicrosoftCalendarEventVMObj.eventType = googleCalendarEventVMObj.eventType;
                                googleMicrosoftCalendarEventVMObj.access_token = googleCalendarEventVMObj.access_token;
                                googleMicrosoftCalendarEventVMObj.summary = googleCalendarEventVMObj.summary;
                                // googleMicrosoftCalendarEventVM.isAllDay = googleCalendarEventVMObj.isAllDay;
                                googleMicrosoftCalendarEventVMObj.isCancelled = googleCalendarEventVMObj.isCancelled;
                                intProviderAppSecretObj.Email = intProviderAppSecretObj.Email;
                                googleMicrosoftCalendarEventVMObj.IntProviderAppSecretId = intProviderAppSecretObj.Id;
                                intProviderAppSecretObj.Events.Add(googleMicrosoftCalendarEventVMObj);
                            }
                        }
                    }
                }
            }            
            var responseDtoList = _mapper.Map<List<CalendarGetEventsResponse>>(intProviderAppSecretListDtoList);
            return new OperationResult<List<CalendarGetEventsResponse>>(true, System.Net.HttpStatusCode.OK,"", responseDtoList);
        }

        [Authorize]
        [HttpPost]
        public async Task<OperationResult<CalendarAddUpdateEventResponse>> AddEvent([FromBody] CalendarAddUpdateEventRequest model)
        {
            GoogleCalendarEventVM googleCalendarEventVMObj = new GoogleCalendarEventVM();
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            
            var requestmodel = _mapper.Map<GoogleMicrosoftCalendarEventVM>(model);
            requestmodel.start.timeZone = "UTC";
            requestmodel.end.timeZone = "UTC";
            requestmodel.TenantId = TenantId;
            if (requestmodel.isAllDay)
            {
                var date = Convert.ToDateTime(requestmodel.start.dateTime);
                var NewDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                var EndDate = Convert.ToDateTime(requestmodel.end.dateTime);
                var EndDateNew = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day, 0, 0, 0);
                requestmodel.start.dateTime = NewDate.ToString("yyyy-MM-ddTHH:mm:ss");
                requestmodel.end.dateTime = EndDateNew.ToString("yyyy-MM-ddTHH:mm:ss");
            }


            // model.end.dateTime = model.endDate.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            GoogleMicrosoftCalendarEventVM? data;
            if (string.IsNullOrEmpty(requestmodel.id))
            {
                data = await calendar.CreateEvent(UserId, "", requestmodel);
            }
            else
            {
                data = await calendar.UpdateEvent(UserId, requestmodel.id, requestmodel);
            }

            // if (data != null && !string.IsNullOrEmpty(data.id) && string.IsNullOrEmpty(data.error_description))
            // {
            var responsedata = _mapper.Map<CalendarAddUpdateEventResponse>(data);    
            return new OperationResult<CalendarAddUpdateEventResponse>(data.IsValid, System.Net.HttpStatusCode.OK,data.error_description, responsedata);
            // }
            // else
            // {
            //     return new OperationResult<GoogleMicrosoftCalendarEventVM>(false, data.error_description, data);
            // }
        }

        [Authorize]
        [HttpDelete]
        public async Task<OperationResult<CalendarDeleteEventResponse>> RemoveEvent([FromBody] CalendarDeleteEventRequest model)
        {
            GoogleCalendarEventVM googleCalendarEventVMObj = new GoogleCalendarEventVM();
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var requestmodel = _mapper.Map<GoogleMicrosoftCalendarEventVM>(model);
            var responsemodel = _mapper.Map<CalendarDeleteEventResponse>(requestmodel);
            if (string.IsNullOrEmpty(requestmodel.id))
            {
                return new OperationResult<CalendarDeleteEventResponse>(false, System.Net.HttpStatusCode.OK,"Please provide event id.", responsemodel);
            }
            if (requestmodel.IntProviderAppSecretId == null)
            {
                return new OperationResult<CalendarDeleteEventResponse>(false, System.Net.HttpStatusCode.OK,"Please provide selected email.", responsemodel);
            }
            else
            {
                var data = await calendar.DeleteEvent(UserId, requestmodel.id, requestmodel);
                var responsedata = _mapper.Map<CalendarDeleteEventResponse>(data);
                if (data != null && !string.IsNullOrEmpty(data.id) && string.IsNullOrEmpty(data.error_description))
                {
                    return new OperationResult<CalendarDeleteEventResponse>(true, System.Net.HttpStatusCode.OK,"", responsedata);
                }
                else
                {
                    return new OperationResult<CalendarDeleteEventResponse>(false, System.Net.HttpStatusCode.OK,responsedata.error_description, responsedata);
                }
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<OperationResult<CalendarSyncActivityResponse>> SyncActivity([FromBody] CalendarSyncActivityRequest model)
        {
            GoogleCalendarEventVM googleCalendarEventVMObj = new GoogleCalendarEventVM();
            CalendarSyncActivityResponse responsemodel = new CalendarSyncActivityResponse();
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var requestmodel = _mapper.Map<GoogleMicrosoftCalendarEventVM>(model);

            var intProviderAppSecretObj = new IntProviderAppSecret();
            if (requestmodel.IntProviderAppSecretId == null)
            {
                responsemodel = _mapper.Map<CalendarSyncActivityResponse>(requestmodel);
                return new OperationResult<CalendarSyncActivityResponse>(false, System.Net.HttpStatusCode.OK,"Please provide selected email.", responsemodel);
            }
            else
            {
                if (requestmodel.IntProviderAppSecretId != null)
                {
                    intProviderAppSecretObj = _intProviderAppSecretService.GetIntProviderAppSecretById(requestmodel.IntProviderAppSecretId.Value);
                }
                IntProviderAppSecretDto intProviderAppSecretDto = _mapper.Map<IntProviderAppSecretDto>(intProviderAppSecretObj);
                if (intProviderAppSecretObj != null)
                {
                    requestmodel.SelectedEmail = intProviderAppSecretObj.Email;
                    if (intProviderAppSecretObj.IntProviderApp != null)
                    {
                        requestmodel.ProviderApp = intProviderAppSecretObj.IntProviderApp.Name;
                        if (intProviderAppSecretObj.IntProviderApp.IntProvider != null)
                        {
                            requestmodel.Provider = intProviderAppSecretObj.IntProviderApp.IntProvider.Name;
                        }
                    }
                }

                var customerActivities = _customerActivityService.GetByUser(UserId);
                var organizationActivities = _organizationActivityService.GetByUser(UserId);
                var leadActivities = _leadActivityService.GetByUser(UserId);
                var calendarsyncActivitites = _calendarSyncActivityService.GetByUser(UserId);
                var moduleList = _customModuleService.GetAll();
                var customerModuleObj = moduleList.Where(t => t.Name == "Person").FirstOrDefault();
                var organizationModuleObj = moduleList.Where(t => t.Name == "Organization").FirstOrDefault();
                var leadModuleObj = moduleList.Where(t => t.Name == "Lead").FirstOrDefault();

                customerActivities = customerActivities.Where(t => t.ScheduleStartDate != null && t.ScheduleEndDate != null &&
               t.StartTime != null && t.EndTime != null).ToList();

                if (customerActivities != null && customerActivities.Count() > 0)
                {
                    foreach (var customerActivityObj in customerActivities)
                    {
                        if (requestmodel.IntProviderAppSecretId != null)
                        {
                            var isExistData = calendarsyncActivitites.Where(t => t.ActivityId == customerActivityObj.Id && t.ModuleId == customerModuleObj.Id && t.IntProviderAppSecretId == model.IntProviderAppSecretId.Value).FirstOrDefault();
                            var members = _customerActivityMemberService.GetAllByActivity(customerActivityObj.Id);
                            GoogleMicrosoftCalendarEventVM googleMicrosoftCalendarEventVMObj = new GoogleMicrosoftCalendarEventVM();
                            googleMicrosoftCalendarEventVMObj.IntProviderAppSecretId = requestmodel.IntProviderAppSecretId;
                            googleMicrosoftCalendarEventVMObj.summary = customerActivityObj.Title;
                            googleMicrosoftCalendarEventVMObj.description = customerActivityObj.Description;
                            googleMicrosoftCalendarEventVMObj.start = new CalendarEventDate();
                            googleMicrosoftCalendarEventVMObj.start.timeZone = "UTC";

                            if (customerActivityObj.ScheduleStartDate != null)
                            {
                                var startDateTime = Common.GetStartEndTime(customerActivityObj.ScheduleStartDate.Value, customerActivityObj.StartTime);

                                if (startDateTime != null)
                                {
                                    googleMicrosoftCalendarEventVMObj.start.dateTime = startDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                                }
                            }
                            googleMicrosoftCalendarEventVMObj.end = new CalendarEventDate();
                            googleMicrosoftCalendarEventVMObj.end.timeZone = "UTC";

                            if (customerActivityObj.ScheduleEndDate != null)
                            {
                                var endDateTime = Common.GetStartEndTime(customerActivityObj.ScheduleEndDate.Value, customerActivityObj.EndTime);

                                if (endDateTime != null)
                                {
                                    googleMicrosoftCalendarEventVMObj.end.dateTime = endDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                                }
                            }

                            googleMicrosoftCalendarEventVMObj.creator.email = intProviderAppSecretDto.Email;

                            if (members != null && members.Count() > 0)
                            {
                                foreach (var customerMemberObj in members)
                                {
                                    GoogleMSCalendarAttendee googleMSCalendarAttendeeObj = new GoogleMSCalendarAttendee();

                                    if (requestmodel.Provider.ToLower() == "google")
                                    {
                                        googleMSCalendarAttendeeObj.email = customerMemberObj.Email;
                                    }
                                    else
                                    {
                                        googleMSCalendarAttendeeObj.emailAddress = new CalendarEventEmail();
                                        googleMSCalendarAttendeeObj.emailAddress.address = customerMemberObj.Email;
                                        googleMSCalendarAttendeeObj.emailAddress.name = customerMemberObj.Email;
                                    }
                                    googleMicrosoftCalendarEventVMObj.attendee.Add(googleMSCalendarAttendeeObj);
                                }
                            }

                            if (isExistData == null)
                            {
                                CalendarSyncActivityDto calendarSyncActivityDto = new CalendarSyncActivityDto();
                                calendarSyncActivityDto.CreatedBy = UserId;
                                calendarSyncActivityDto.IntProviderAppSecretId = intProviderAppSecretDto.Id;
                                calendarSyncActivityDto.ActivityId = customerActivityObj.Id;
                                calendarSyncActivityDto.ModuleId = customerModuleObj.Id;


                                var calendarCustomerActivity = await calendar.CreateEvent(UserId, "", googleMicrosoftCalendarEventVMObj);
                                if (string.IsNullOrEmpty(calendarCustomerActivity.error_description))
                                {
                                    calendarSyncActivityDto.CalendarEventId = calendarCustomerActivity.id;
                                    calendarSyncActivityDto.TenantId = TenantId;
                                    var AddUpdate = await _calendarSyncActivityService.CheckInsertOrUpdate(calendarSyncActivityDto);
                                }
                            }
                            else
                            {
                                googleMicrosoftCalendarEventVMObj.id = isExistData.CalendarEventId;
                                var customers = await calendar.UpdateEvent(UserId, googleMicrosoftCalendarEventVMObj.id, googleMicrosoftCalendarEventVMObj);
                            }
                        }
                    }
                }

                organizationActivities = organizationActivities.Where(t => t.ScheduleStartDate != null && t.ScheduleEndDate != null &&
                                                                    t.StartTime != null && t.EndTime != null).ToList();

                if (organizationActivities != null && organizationActivities.Count() > 0)
                {
                    foreach (var organizationActivityObj in organizationActivities)
                    {
                        if (requestmodel.IntProviderAppSecretId != null)
                        {
                            var isExistData1 = calendarsyncActivitites.Where(t => t.ActivityId == organizationActivityObj.Id && t.ModuleId == organizationModuleObj.Id && t.IntProviderAppSecretId == model.IntProviderAppSecretId.Value).FirstOrDefault();

                            var organizationMembers = _organizationActivityMemberService.GetAllByActivity(organizationActivityObj.Id);
                            GoogleMicrosoftCalendarEventVM googleMicrosoftCalendarEventVMObj1 = new GoogleMicrosoftCalendarEventVM();
                            googleMicrosoftCalendarEventVMObj1.IntProviderAppSecretId = requestmodel.IntProviderAppSecretId;
                            googleMicrosoftCalendarEventVMObj1.summary = organizationActivityObj.Title;
                            googleMicrosoftCalendarEventVMObj1.description = organizationActivityObj.Description;
                            googleMicrosoftCalendarEventVMObj1.start = new CalendarEventDate();
                            googleMicrosoftCalendarEventVMObj1.start.timeZone = "UTC";
                            if (organizationActivityObj.ScheduleStartDate != null)
                            {
                                var startDateTime = Common.GetStartEndTime(organizationActivityObj.ScheduleStartDate.Value, organizationActivityObj.StartTime);

                                if (startDateTime != null)
                                {
                                    googleMicrosoftCalendarEventVMObj1.start.dateTime = startDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                                }
                            }
                            googleMicrosoftCalendarEventVMObj1.end = new CalendarEventDate();
                            googleMicrosoftCalendarEventVMObj1.end.timeZone = "UTC";
                            if (organizationActivityObj.ScheduleEndDate != null)
                            {
                                var endDateTime = Common.GetStartEndTime(organizationActivityObj.ScheduleEndDate.Value, organizationActivityObj.EndTime);
                                if (endDateTime != null)
                                {
                                    googleMicrosoftCalendarEventVMObj1.end.dateTime = endDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                                }
                            }
                            googleMicrosoftCalendarEventVMObj1.creator.email = intProviderAppSecretDto.Email;

                            if (organizationMembers != null && organizationMembers.Count() > 0)
                            {
                                foreach (var organizationMemberObj in organizationMembers)
                                {
                                    GoogleMSCalendarAttendee googleMSCalendarAttendeeObj1 = new GoogleMSCalendarAttendee();

                                    if (requestmodel.Provider.ToLower() == "google")
                                    {
                                        googleMSCalendarAttendeeObj1.email = organizationMemberObj.Email;
                                    }
                                    else
                                    {
                                        googleMSCalendarAttendeeObj1.emailAddress = new CalendarEventEmail();
                                        googleMSCalendarAttendeeObj1.emailAddress.address = organizationMemberObj.Email;
                                        googleMSCalendarAttendeeObj1.emailAddress.name = organizationMemberObj.Email;
                                    }
                                    googleMicrosoftCalendarEventVMObj1.attendee.Add(googleMSCalendarAttendeeObj1);
                                }
                            }

                            if (isExistData1 == null)
                            {
                                CalendarSyncActivityDto calendarSyncActivityDto1 = new CalendarSyncActivityDto();
                                calendarSyncActivityDto1.CreatedBy = UserId;
                                calendarSyncActivityDto1.IntProviderAppSecretId = intProviderAppSecretDto.Id;
                                calendarSyncActivityDto1.ActivityId = organizationActivityObj.Id;
                                calendarSyncActivityDto1.ModuleId = organizationModuleObj.Id;

                                var calendarOrganizationActivity = await calendar.CreateEvent(UserId, "", googleMicrosoftCalendarEventVMObj1);
                                if (string.IsNullOrEmpty(calendarOrganizationActivity.error_description))
                                {
                                    calendarSyncActivityDto1.CalendarEventId = calendarOrganizationActivity.id;
                                    calendarSyncActivityDto1.TenantId = TenantId;
                                    var AddUpdate = await _calendarSyncActivityService.CheckInsertOrUpdate(calendarSyncActivityDto1);
                                }
                            }
                            else
                            {
                                googleMicrosoftCalendarEventVMObj1.id = isExistData1.CalendarEventId;
                                var organizations = await calendar.UpdateEvent(UserId, googleMicrosoftCalendarEventVMObj1.id, googleMicrosoftCalendarEventVMObj1);
                            }
                        }
                    }
                }


                leadActivities = leadActivities.Where(t => t.ScheduleStartDate != null && t.ScheduleEndDate != null &&
               t.StartTime != null && t.EndTime != null).ToList();

                if (leadActivities != null && leadActivities.Count() > 0)
                {
                    foreach (var leadActivityObj in leadActivities)
                    {
                        if (requestmodel.IntProviderAppSecretId != null)
                        {
                            var isExistData2 = calendarsyncActivitites.Where(t => t.ActivityId == leadActivityObj.Id && t.ModuleId == leadModuleObj.Id && t.IntProviderAppSecretId == model.IntProviderAppSecretId.Value).FirstOrDefault();

                            var leadMembers = _leadActivityMemberService.GetAllByActivity(leadActivityObj.Id);

                            GoogleMicrosoftCalendarEventVM googleMicrosoftCalendarEventVMObj2 = new GoogleMicrosoftCalendarEventVM();
                            googleMicrosoftCalendarEventVMObj2.IntProviderAppSecretId = requestmodel.IntProviderAppSecretId;
                            googleMicrosoftCalendarEventVMObj2.summary = leadActivityObj.Title;
                            googleMicrosoftCalendarEventVMObj2.description = leadActivityObj.Description;
                            googleMicrosoftCalendarEventVMObj2.start = new CalendarEventDate();
                            googleMicrosoftCalendarEventVMObj2.start.timeZone = "UTC";
                            if (leadActivityObj.ScheduleStartDate != null)
                            {
                                var startDateTime = Common.GetStartEndTime(leadActivityObj.ScheduleStartDate.Value, leadActivityObj.StartTime);

                                if (startDateTime != null)
                                {
                                    googleMicrosoftCalendarEventVMObj2.start.dateTime = startDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                                }
                            }
                            googleMicrosoftCalendarEventVMObj2.end = new CalendarEventDate();
                            googleMicrosoftCalendarEventVMObj2.end.timeZone = "UTC";
                            
                            if (leadActivityObj.ScheduleEndDate != null)
                            {
                                var endDateTime = Common.GetStartEndTime(leadActivityObj.ScheduleEndDate.Value, leadActivityObj.EndTime);
                                if (endDateTime != null)
                                {
                                    googleMicrosoftCalendarEventVMObj2.end.dateTime = endDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                                }
                            }

                            googleMicrosoftCalendarEventVMObj2.creator.email = intProviderAppSecretDto.Email;

                            if (leadMembers != null && leadMembers.Count() > 0)
                            {
                                foreach (var leadMemberObj in leadMembers)
                                {
                                    GoogleMSCalendarAttendee googleMSCalendarAttendeeObj2 = new GoogleMSCalendarAttendee();

                                    if (requestmodel.Provider.ToLower() == "google")
                                    {
                                        googleMSCalendarAttendeeObj2.email = leadMemberObj.Email;
                                    }
                                    else
                                    {
                                        googleMSCalendarAttendeeObj2.emailAddress = new CalendarEventEmail();
                                        googleMSCalendarAttendeeObj2.emailAddress.address = leadMemberObj.Email;
                                        googleMSCalendarAttendeeObj2.emailAddress.name = leadMemberObj.Email;
                                    }
                                    googleMicrosoftCalendarEventVMObj2.attendee.Add(googleMSCalendarAttendeeObj2);
                                }
                            }

                            if (isExistData2 == null)
                            {
                                CalendarSyncActivityDto calendarSyncActivityDto2 = new CalendarSyncActivityDto();
                                calendarSyncActivityDto2.CreatedBy = UserId;
                                calendarSyncActivityDto2.IntProviderAppSecretId = intProviderAppSecretDto.Id;
                                calendarSyncActivityDto2.ActivityId = leadActivityObj.Id;
                                calendarSyncActivityDto2.ModuleId = leadModuleObj.Id;

                                var calendarLeadActivity = await calendar.CreateEvent(UserId, "", googleMicrosoftCalendarEventVMObj2);
                                if (string.IsNullOrEmpty(calendarLeadActivity.error_description))
                                {
                                    calendarSyncActivityDto2.CalendarEventId = calendarLeadActivity.id;
                                    calendarSyncActivityDto2.TenantId = TenantId;
                                    var AddUpdate = await _calendarSyncActivityService.CheckInsertOrUpdate(calendarSyncActivityDto2);
                                }
                            }
                            else
                            {
                                googleMicrosoftCalendarEventVMObj2.id = isExistData2.CalendarEventId;
                                var leads = await calendar.UpdateEvent(UserId, googleMicrosoftCalendarEventVMObj2.id, googleMicrosoftCalendarEventVMObj2);
                            }
                        }
                    }
                }
            }
            responsemodel = _mapper.Map<CalendarSyncActivityResponse>(requestmodel);
            return new OperationResult<CalendarSyncActivityResponse>(true, System.Net.HttpStatusCode.OK,"", responsemodel);
        }
    }
}