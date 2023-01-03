// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Security.Claims;
// using System.Threading;
// using System.Threading.Tasks;
// using AutoMapper;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Configuration;
// using matcrm.data.Context;
// using matcrm.data.Models.Dto;
// using matcrm.data.Models.Tables;
// using matcrm.data.Models.ViewModels;
// using matcrm.data.Models.ViewModels.Calendar;
// using matcrm.service.Common;
// using matcrm.service.Services;
// using matcrm.service.Services.ERP;
// using matcrm.service.Utility;

// namespace matcrm.api.Controllers
// {
//     // [Authorize]
//     [Route("[controller]")]
//     [ApiController]
//     public class MicrosoftCalendarController
//     {
//         private readonly IGoogleCalendarService _calendarService;
//         private readonly IIntProviderService _intProviderService;
//         private readonly IIntProviderAppService _intProviderAppService;
//         private readonly IIntProviderAppSecretService _intProviderAppSecretService;
//         private readonly ICustomerActivityService _customerActivityService;
//         private readonly IOrganizationActivityService _organizationActivityService;
//         private readonly ILeadActivityService _leadActivityService;
//         private readonly ICalendarSyncActivityService _calendarSyncActivityService;
//         private readonly ICustomModuleService _customModuleService;
//         private readonly ICustomerActivityMemberService _customerActivityMemberService;
//         private readonly IOrganizationActivityMemberService _organizationActivityMemberService;
//         private readonly ILeadActivityMemberService _leadActivityMemberService;
//         private readonly ICustomerAttachmentService _customerAttachmentService;
//         private readonly IHostingEnvironment _hostingEnvironment;
//         private readonly IUserService _userService;
//         private IMapper _mapper;
//         private Calendar calendar;
//         private string GoogleCalendarClientId, GoogleCalendarSecret, GoogleCalendarScope, GoogleCalendarApiKey;
//         public MicrosoftCalendarController(
//             IGoogleCalendarService calendarService,
//             IIntProviderService intProviderService,
//             IIntProviderAppService intProviderAppService,
//             IIntProviderAppSecretService intProviderAppSecretService,
//             ICustomerActivityService customerActivityService,
//             IOrganizationActivityService organizationActivityService,
//             ILeadActivityService leadActivityService,
//             ICalendarSyncActivityService calendarSyncActivityService,
//             ICustomModuleService customModuleService,
//             ICustomerActivityMemberService customerActivityMemberService,
//             IOrganizationActivityMemberService organizationActivityMemberService,
//             ILeadActivityMemberService leadActivityMemberService,
//             ICustomerAttachmentService customerAttachmentService,
//             IUserService userService,
//             IMapper mapper,
//             IHostingEnvironment hostingEnvironment,
//             OneClappContext context
//         )
//         {
//             _calendarService = calendarService;
//             _intProviderService = intProviderService;
//             _intProviderAppService = intProviderAppService;
//             _intProviderAppSecretService = intProviderAppSecretService;
//             _mapper = mapper;
//             _customerActivityService = customerActivityService;
//             _organizationActivityService = organizationActivityService;
//             _leadActivityService = leadActivityService;
//             _calendarSyncActivityService = calendarSyncActivityService;
//             _customModuleService = customModuleService;
//             _customerActivityMemberService = customerActivityMemberService;
//             _organizationActivityMemberService = organizationActivityMemberService;
//             _leadActivityMemberService = leadActivityMemberService;
//             _customerAttachmentService = customerAttachmentService;
//             GoogleCalendarClientId = OneClappContext.GoogleCalendarClientId;
//             GoogleCalendarSecret = OneClappContext.GoogleCalendarSecretKey;
//             GoogleCalendarApiKey = OneClappContext.GoogleCalendarApiKey;
//            GoogleCalendarScope = OneClappContext.GoogleScopes;
//             calendar = new Calendar(context, userService, hostingEnvironment, intProviderAppService, intProviderAppSecretService, customerAttachmentService, calendarService);
//         }

//         // [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
//         [HttpPost("GetEvents")]
//         public async Task<OperationResult<MSCalendarEvent>> GetEvents([FromBody] InboxVM Model)
//         {
//             MSCalendarEvent eventVM = new MSCalendarEvent();
//             eventVM = await calendar.GetEvents(Model.UserId.Value, Model);
//             if (string.IsNullOrEmpty(eventVM.ErrorMessage))
//             {
//                 return new OperationResult<MSCalendarEvent>(true, "", eventVM);
//             }
//             else
//             {
//                 return new OperationResult<MSCalendarEvent>(false, eventVM.ErrorMessage, eventVM);
//             }
//         }

//         [HttpPost("GetEventById")]
//         public async Task<OperationResult<MicrosoftCalendarEventVM>> GetEventById([FromBody] MicrosoftCalendarEventVM Model)
//         {
//             var calendarEvent = await calendar.GetEventByEventId(Model.UserId, Model.id, Model);
//             if (string.IsNullOrEmpty(calendarEvent.ErrorMessage))
//             {
//                 return new OperationResult<MicrosoftCalendarEventVM>(true, "", calendarEvent);
//             }
//             else
//             {
//                 return new OperationResult<MicrosoftCalendarEventVM>(true, calendarEvent.ErrorMessage, calendarEvent);
//             }
//         }

//         [HttpPost("GetToken")]
//         public async Task<OperationResult<GoogleCalendarTokenVM>> GetToken([FromBody] GoogleCalendarTokenVM model)
//         {
//             GoogleCalendarTokenVM calendarTokenObj = new GoogleCalendarTokenVM();
//             var googleCalendarKey = GoogleCalendarApiKey;
//             if (!string.IsNullOrEmpty(googleCalendarKey))
//             {
//                 model.grant_type = "authorization_code";
//                 var customers = await _calendarService.GetAccessToken(googleCalendarKey, model);
//                 if (customers != null)
//                 {
//                     calendarTokenObj = customers;
//                 }

//                 if (calendarTokenObj.error == null || calendarTokenObj.error == "")
//                 {
//                     if (model.userId != null)
//                     {
//                         var IntProviderObj = _intProviderService.GetIntProvider("Google");
//                         if (IntProviderObj != null)
//                         {
//                             var IntProviderAppObj = _intProviderAppService.GetIntProviderAppByProviderId(IntProviderObj.Id, "Calendar");
//                             IntProviderAppSecretDto secretDto = new IntProviderAppSecretDto();
//                             secretDto.Access_Token = calendarTokenObj.access_token;
//                             secretDto.Expires_In = calendarTokenObj.expires_in;
//                             secretDto.Refresh_Token = calendarTokenObj.refresh_token;
//                             secretDto.Scope = calendarTokenObj.scope;
//                             secretDto.Token_Type = calendarTokenObj.token_type;
//                             secretDto.Id_Token = calendarTokenObj.id_token;
//                             secretDto.CreatedBy = calendarTokenObj.userId;
//                             secretDto.IntProviderAppId = IntProviderAppObj.Id;
//                             secretDto.CreatedBy = model.userId;
//                             GoogleCalendarUser googleCalendarUserInfo = await _calendarService.GetUserInfo("Bearer " + calendarTokenObj.access_token, OneClappContext.GoogleSecretKey);
//                             if (googleCalendarUserInfo != null)
//                             {
//                                 secretDto.Email = googleCalendarUserInfo.email;
//                             }
//                             var IntProvicerAppSecret = await _intProviderAppSecretService.CheckInsertOrUpdate(secretDto);
//                         }
//                     }
//                 }
//                 return new OperationResult<GoogleCalendarTokenVM>(true, "", calendarTokenObj);
//             }
//             else
//             {
//                 return new OperationResult<GoogleCalendarTokenVM>(false, "", calendarTokenObj);
//             }
//         }

//         [HttpPost("AddEvent")]
//         public async Task<OperationResult<MicrosoftCalendarEventVM>> AddUpdateEvent([FromBody] MicrosoftCalendarEventVM model)
//         {
//             model.start = new MSEventDate();
//             model.start.timeZone = "UTC";
//             model.end = new MSEventDate();
//             model.end.timeZone = "UTC";
//             if (model.isAllDay)
//             {
//                 model.start.dateTime = model.startDate.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
//                 model.end.dateTime = model.endDate.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
//             }
//             else
//             {
//                 var startDateTime = Common.GetStartEndTime(model.startDate.Value, model.startTime);
//                 var endDateTime = Common.GetStartEndTime(model.endDate.Value, model.endTime);
//                 if (startDateTime != null)
//                 {
//                     model.start.dateTime = startDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
//                 }

//                 if (endDateTime != null)
//                 {
//                     model.end.dateTime = endDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
//                 }
//             }

//             // model = await calendar.CreateEvent(model.UserId, null, model);
//             if (string.IsNullOrEmpty(model.ErrorMessage))
//             {
//                 return new OperationResult<MicrosoftCalendarEventVM>(true, "", model);
//             }
//             else
//             {
//                 return new OperationResult<MicrosoftCalendarEventVM>(false, model.ErrorMessage, model);
//             }
//         }

//         [HttpPost("UpdateEvent")]
//         public async Task<OperationResult<MicrosoftCalendarEventVM>> UpdateEvent([FromBody] MicrosoftCalendarEventVM model)
//         {
//             model.start = new MSEventDate();
//             model.start.timeZone = "UTC";
//             model.end = new MSEventDate();
//             model.end.timeZone = "UTC";
//             if (model.isAllDay)
//             {
//                 model.start.dateTime = model.startDate.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
//                 model.end.dateTime = model.endDate.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
//             }
//             else
//             {
//                 var startDateTime = Common.GetStartEndTime(model.startDate.Value, model.startTime);
//                 var endDateTime = Common.GetStartEndTime(model.endDate.Value, model.endTime);
//                 if (startDateTime != null)
//                 {
//                     model.start.dateTime = startDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
//                 }

//                 if (endDateTime != null)
//                 {
//                     model.end.dateTime = endDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
//                 }
//             }

//             model = await calendar.UpdateEvent(model.UserId, model.id, model);
//             if (string.IsNullOrEmpty(model.ErrorMessage))
//             {
//                 return new OperationResult<MicrosoftCalendarEventVM>(true, "", model);
//             }
//             else
//             {
//                 return new OperationResult<MicrosoftCalendarEventVM>(false, model.ErrorMessage, model);
//             }
//         }

//         [HttpPost("DeleteEvent")]
//         public async Task<OperationResult<MicrosoftCalendarEventVM>> DeleteEvent([FromBody] MicrosoftCalendarEventVM model)
//         {

//             if (!string.IsNullOrEmpty(model.id))
//             {
//                 model = await calendar.DeleteEvent(model.UserId, model.id, model);
//                 if (model.IsValid)
//                 {
//                     return new OperationResult<MicrosoftCalendarEventVM>(true, "Event deleted successfully.", model);
//                 }
//                 else
//                 {
//                     return new OperationResult<MicrosoftCalendarEventVM>(false, CommonMessage.DefaultErrorMessage, model);
//                 }
//             }
//             else
//             {
//                 return new OperationResult<MicrosoftCalendarEventVM>(false, "Please provide eventid.", model);
//             }



//         }

//         [HttpPost("GetRefreshToken")]
//         public async Task<OperationResult<GoogleCalendarTokenVM>> GetRefreshToken([FromBody] GoogleCalendarTokenVM model)
//         {
//             GoogleCalendarTokenVM calendarTokenObj = new GoogleCalendarTokenVM();
//             var googleCalendarKey = GoogleCalendarApiKey;
//             if (!string.IsNullOrEmpty(googleCalendarKey))
//             {
//                 // var Token = "Bearer ya29.a0ARrdaM9H73KZ0HF-Uhg-64MlSSttedMmk8lKmmZpKWLWsqwN-6EOtA0dtWoYD4lt7y1TKA1DTez-bg-zsH1ezMzwHxhzX4AXuwShfTOtEsuTvb30TM8XkwMtTx_RSAnMpzQu0th2QMMFz4coocjfljvMLnu0";
//                 var customers = await _calendarService.GetRefreshToken(googleCalendarKey, model);
//                 if (customers != null)
//                 {
//                     calendarTokenObj = customers;
//                 }
//                 return new OperationResult<GoogleCalendarTokenVM>(true, "", calendarTokenObj);
//             }
//             else
//             {
//                 return new OperationResult<GoogleCalendarTokenVM>(false, "", calendarTokenObj);
//             }
//         }

//         private async Task<IntProviderAppSecretDto> CheckAccessToken(IntProviderAppSecret intProviderAppSecretObj)
//         {
//             string Token = "";
//             var intProviderAppSecretDto = _mapper.Map<IntProviderAppSecretDto>(intProviderAppSecretObj);
//             if (intProviderAppSecretObj != null)
//             {

//                 var tokenInfo = await _calendarService.GetTokenInfo(intProviderAppSecretObj.Access_Token, GoogleCalendarSecret);
//                 if ((tokenInfo == null) || (tokenInfo != null && (!string.IsNullOrEmpty(tokenInfo.error_description))))
//                 {
//                     GoogleCalendarTokenVM tokenVM = new GoogleCalendarTokenVM();
//                     tokenVM.refresh_token = intProviderAppSecretObj.Refresh_Token;
//                     tokenVM.client_id = GoogleCalendarClientId;
//                     tokenVM.client_secret = GoogleCalendarSecret;
//                     tokenVM.grant_type = "refresh_token";
//                     tokenVM.scope = GoogleCalendarScope;
//                     var accessTokenObj = await _calendarService.GetRefreshToken(GoogleCalendarSecret, tokenVM);
//                     if (string.IsNullOrEmpty(accessTokenObj.error_description))
//                     {
//                         Token = "Bearer " + accessTokenObj.access_token;
//                         intProviderAppSecretDto.Access_Token = accessTokenObj.access_token;
//                         intProviderAppSecretDto.Id_Token = accessTokenObj.id_token;
//                         intProviderAppSecretDto.LastAccessedOn = DateTime.UtcNow;
//                         intProviderAppSecretDto.Token = Token;
//                     }
//                     else
//                     {
//                         intProviderAppSecretDto.error_description = accessTokenObj.error_description;
//                         return intProviderAppSecretDto;
//                     }

//                 }
//                 else
//                 {
//                     Token = "Bearer " + intProviderAppSecretObj.Access_Token;
//                     intProviderAppSecretDto.Token = Token;
//                     intProviderAppSecretDto.LastAccessedOn = DateTime.UtcNow;
//                 }
//                 var AddUpdate = await _intProviderAppSecretService.CheckInsertOrUpdate(intProviderAppSecretDto);
//             }
//             return intProviderAppSecretDto;
//             // return Token;
//         }


//         [HttpPost("GetAllEvent")]
//         public async Task<OperationResult<List<CalendarEventVM>>> GetAllEvent(int UserId)
//         {
//             List<CalendarEventVM> calendarEventVMList = new List<CalendarEventVM>();
//             var IntProviderAppSecretList = _intProviderAppSecretService.GetAllSelected(UserId);
//             if (IntProviderAppSecretList.Count() > 0)
//             {
//                 foreach (var IntProviderAppSecretItem in IntProviderAppSecretList)
//                 {
//                     if (IntProviderAppSecretItem.IntProviderApp != null && IntProviderAppSecretItem.IntProviderApp.IntProvider != null)
//                     {
//                         var IntProviderObj = IntProviderAppSecretItem.IntProviderApp.IntProvider;
//                         var IntProviderAppObj = IntProviderAppSecretItem.IntProviderApp;
//                         CalendarEventVM calendarEventVM = new CalendarEventVM();
//                         calendarEventVM.Email = IntProviderAppSecretItem.Email;
//                         calendarEventVM.Provider = IntProviderObj.Name;
//                         if (IntProviderObj.Name.ToLower() == "google")
//                         {
//                             var intProviderAppSecretDto = await CheckAccessToken(IntProviderAppSecretItem);
//                             if (intProviderAppSecretDto != null && string.IsNullOrEmpty(intProviderAppSecretDto.error_description))
//                             {
//                                 var events = await _calendarService.GetEvents(intProviderAppSecretDto.Token, GoogleCalendarApiKey, intProviderAppSecretDto.Email);

//                                 calendarEventVM.GoogleEvents = events;
//                                 calendarEventVMList.Add(calendarEventVM);
//                             }
//                         }
//                         else if (IntProviderObj.Name.ToLower() == "microsoft")
//                         {
//                             MSCalendarEvent eventVM = new MSCalendarEvent();
//                             InboxVM inboxVM = new InboxVM();
//                             inboxVM.SelectedEmail = IntProviderAppSecretItem.Email;
//                             inboxVM.UserId = UserId;
//                             inboxVM.Provider = "Calendar";

//                             var calendarEvents = await calendar.GetEvents(UserId, inboxVM);
//                             calendarEventVM.MSEvents = calendarEvents.value;
//                             calendarEventVMList.Add(calendarEventVM);
//                         }
//                     }
//                 }
//             }
//             return new OperationResult<List<CalendarEventVM>>(true, "", calendarEventVMList);
//         }


//         [HttpPost("SyncTaskToAll")]
//         public async Task<OperationResult<GoogleCalendarEventVM>> SyncTaskToAll(int UserId, int TenantId)
//         {
//             GoogleCalendarEventVM googleCalendarEventVM = new GoogleCalendarEventVM();

//             var IntProviderAppSecretList = _intProviderAppSecretService.GetAllSelected(UserId);
//             if (IntProviderAppSecretList.Count() > 0)
//             {
//                 var customerActivities = _customerActivityService.GetByUser(UserId);
//                 var organizationActivities = _organizationActivityService.GetByUser(UserId);
//                 var leadActivities = _leadActivityService.GetByUser(UserId);
//                 var calendarsyncActivitites = _calendarSyncActivityService.GetByUser(UserId);
//                 var moduleList = _customModuleService.GetAll();
//                 var customerModuleObj = moduleList.Where(t => t.Name == "Person").FirstOrDefault();
//                 var organizationModuleObj = moduleList.Where(t => t.Name == "Organization").FirstOrDefault();
//                 var leadModuleObj = moduleList.Where(t => t.Name == "Lead").FirstOrDefault();
//                 foreach (var IntProviderAppSecretItem in IntProviderAppSecretList)
//                 {
//                     var IntProviderObj = IntProviderAppSecretItem.IntProviderApp.IntProvider;
//                     var IntProviderAppObj = IntProviderAppSecretItem.IntProviderApp;
//                     CalendarEventVM calendarEventVM = new CalendarEventVM();
//                     calendarEventVM.Email = IntProviderAppSecretItem.Email;
//                     calendarEventVM.Provider = IntProviderObj.Name;

//                     customerActivities = customerActivities.Where(t => t.ScheduleStartDate != null && t.ScheduleEndDate != null &&
//                                 t.StartTime != null && t.EndTime != null).ToList();

//                     organizationActivities = organizationActivities.Where(t => t.ScheduleStartDate != null && t.ScheduleEndDate != null &&
//                                   t.StartTime != null && t.EndTime != null).ToList();

//                     leadActivities = leadActivities.Where(t => t.ScheduleStartDate != null && t.ScheduleEndDate != null &&
//                    t.StartTime != null && t.EndTime != null).ToList();

//                     if (IntProviderObj.Name.ToLower() == "google")
//                     {
//                         foreach (var customerActivityObj in customerActivities)
//                         {
//                             var isExistData = calendarsyncActivitites.Where(t => t.ActivityId == customerActivityObj.Id && t.ModuleId == customerModuleObj.Id && t.IntProviderAppSecretId == IntProviderAppSecretItem.Id && t.CreatedBy == UserId).FirstOrDefault();
//                             var members = _customerActivityMemberService.GetAllByActivity(customerActivityObj.Id);
//                             GoogleCalendarEventVM obj = new GoogleCalendarEventVM();
//                             obj.summary = customerActivityObj.Title;
//                             obj.description = customerActivityObj.Description;
//                             obj.start = new EventDate();
//                             obj.start.timeZone = "UTC";
//                             var startDateTime = Common.GetStartEndTime(customerActivityObj.ScheduleStartDate.Value, customerActivityObj.StartTime);
//                             var endDateTime = Common.GetStartEndTime(customerActivityObj.ScheduleEndDate.Value, customerActivityObj.EndTime);
//                             if (startDateTime != null)
//                             {
//                                 obj.start.dateTime = startDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
//                             }
//                             obj.end = new EventDate();
//                             obj.end.timeZone = "UTC";
//                             if (endDateTime != null)
//                             {
//                                 obj.end.dateTime = endDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
//                             }
//                             obj.creator.email = IntProviderAppSecretItem.Email;

//                             if (members.Count() > 0)
//                             {
//                                 foreach (var customerMemberObj in members)
//                                 {
//                                     CalendarAttendee obj1 = new CalendarAttendee();
//                                     obj1.email = customerMemberObj.Email;
//                                     obj.Attendee.Add(obj1);
//                                 }
//                             }
//                             var intProviderAppSecretDto = await CheckAccessToken(IntProviderAppSecretItem);
//                             if (isExistData == null)
//                             {
//                                 CalendarSyncActivityDto syncActivityDto = new CalendarSyncActivityDto();
//                                 syncActivityDto.CreatedBy = UserId;
//                                 syncActivityDto.IntProviderAppSecretId = IntProviderAppSecretItem.Id;
//                                 syncActivityDto.ActivityId = customerActivityObj.Id;
//                                 syncActivityDto.ModuleId = customerModuleObj.Id;
//                                 var calendarCustomerActivity = await _calendarService.AddEvent(GoogleCalendarApiKey, obj, IntProviderAppSecretItem.Email, intProviderAppSecretDto.Token);
//                                 if (string.IsNullOrEmpty(calendarCustomerActivity.error_description))
//                                 {
//                                     syncActivityDto.CalendarEventId = calendarCustomerActivity.id;
//                                     syncActivityDto.TenantId = TenantId;
//                                     var AddUpdate = await _calendarSyncActivityService.CheckInsertOrUpdate(syncActivityDto);
//                                 }
//                             }
//                             else
//                             {
//                                 obj.id = isExistData.CalendarEventId;
//                                 var customers = await _calendarService.UpdateEvent(GoogleCalendarApiKey, obj, IntProviderAppSecretItem.Email, intProviderAppSecretDto.Token);
//                             }
//                         }

//                         foreach (var organizationActivityObj in organizationActivities)
//                         {
//                             var isExistData1 = calendarsyncActivitites.Where(t => t.ActivityId == organizationActivityObj.Id && t.ModuleId == organizationModuleObj.Id && t.IntProviderAppSecretId == IntProviderAppSecretItem.Id && t.CreatedBy == UserId).FirstOrDefault();

//                             var organizationMembers = _organizationActivityMemberService.GetAllByActivity(organizationActivityObj.Id);

//                             var intProviderAppSecretDto = await CheckAccessToken(IntProviderAppSecretItem);

//                             GoogleCalendarEventVM obj1 = new GoogleCalendarEventVM();
//                             obj1.summary = organizationActivityObj.Title;
//                             obj1.description = organizationActivityObj.Description;
//                             obj1.start = new EventDate();
//                             obj1.start.timeZone = "UTC";
//                             var startDateTime = Common.GetStartEndTime(organizationActivityObj.ScheduleStartDate.Value, organizationActivityObj.StartTime);
//                             var endDateTime = Common.GetStartEndTime(organizationActivityObj.ScheduleEndDate.Value, organizationActivityObj.EndTime);
//                             if (startDateTime != null)
//                             {
//                                 obj1.start.dateTime = startDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
//                             }

//                             obj1.end = new EventDate();
//                             obj1.end.timeZone = "UTC";
//                             if (endDateTime != null)
//                             {
//                                 obj1.end.dateTime = endDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
//                             }

//                             obj1.creator.email = intProviderAppSecretDto.Email;

//                             if (organizationMembers.Count() > 0)
//                             {
//                                 foreach (var organizationMemberObj in organizationMembers)
//                                 {
//                                     CalendarAttendee obj22 = new CalendarAttendee();
//                                     obj22.email = organizationMemberObj.Email;
//                                     obj1.Attendee.Add(obj22);
//                                 }
//                             }

//                             if (isExistData1 == null)
//                             {
//                                 CalendarSyncActivityDto syncActivityDto1 = new CalendarSyncActivityDto();
//                                 syncActivityDto1.CreatedBy = UserId;
//                                 syncActivityDto1.IntProviderAppSecretId = intProviderAppSecretDto.Id;
//                                 syncActivityDto1.ActivityId = organizationActivityObj.Id;
//                                 syncActivityDto1.ModuleId = organizationModuleObj.Id;
//                                 syncActivityDto1.IntProviderAppSecretId = IntProviderAppSecretItem.Id;
//                                 var calendarOrganizationActivity = await _calendarService.AddEvent(GoogleCalendarApiKey, obj1, intProviderAppSecretDto.Email, intProviderAppSecretDto.Token);
//                                 if (string.IsNullOrEmpty(calendarOrganizationActivity.error_description))
//                                 {
//                                     syncActivityDto1.CalendarEventId = calendarOrganizationActivity.id;
//                                     syncActivityDto1.TenantId = TenantId;
//                                     var AddUpdate = await _calendarSyncActivityService.CheckInsertOrUpdate(syncActivityDto1);
//                                 }
//                             }
//                             else
//                             {
//                                 obj1.id = isExistData1.CalendarEventId;
//                                 var customers = await _calendarService.UpdateEvent(GoogleCalendarApiKey, obj1, intProviderAppSecretDto.Email, intProviderAppSecretDto.Token);
//                             }
//                         }


//                         foreach (var leadActivityObj in leadActivities)
//                         {
//                             var intProviderAppSecretDto = await CheckAccessToken(IntProviderAppSecretItem);

//                             var isExistData2 = calendarsyncActivitites.Where(t => t.ActivityId == leadActivityObj.Id && t.ModuleId == leadModuleObj.Id).FirstOrDefault();

//                             var leadMembers = _leadActivityMemberService.GetAllByActivity(leadActivityObj.Id);

//                             GoogleCalendarEventVM obj2 = new GoogleCalendarEventVM();
//                             obj2.summary = leadActivityObj.Title;
//                             obj2.description = leadActivityObj.Description;
//                             obj2.start = new EventDate();
//                             obj2.start.timeZone = "UTC";
//                             var startDateTime = Common.GetStartEndTime(leadActivityObj.ScheduleStartDate.Value, leadActivityObj.StartTime);
//                             var endDateTime = Common.GetStartEndTime(leadActivityObj.ScheduleEndDate.Value, leadActivityObj.EndTime);
//                             if (startDateTime != null)
//                             {
//                                 obj2.start.dateTime = startDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
//                             }


//                             obj2.end = new EventDate();
//                             obj2.end.timeZone = "UTC";
//                             if (endDateTime != null)
//                             {
//                                 obj2.end.dateTime = endDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
//                             }

//                             obj2.creator.email = intProviderAppSecretDto.Email;

//                             if (leadMembers.Count() > 0)
//                             {
//                                 foreach (var leadMemberObj in leadMembers)
//                                 {
//                                     CalendarAttendee obj33 = new CalendarAttendee();
//                                     obj33.email = leadMemberObj.Email;
//                                     obj2.Attendee.Add(obj33);
//                                 }
//                             }

//                             if (isExistData2 == null)
//                             {
//                                 CalendarSyncActivityDto syncActivityDto2 = new CalendarSyncActivityDto();
//                                 syncActivityDto2.CreatedBy = UserId;
//                                 syncActivityDto2.IntProviderAppSecretId = intProviderAppSecretDto.Id;
//                                 syncActivityDto2.ActivityId = leadActivityObj.Id;
//                                 syncActivityDto2.ModuleId = leadModuleObj.Id;
//                                 var calendarLeadActivity = await _calendarService.AddEvent(GoogleCalendarApiKey, obj2, intProviderAppSecretDto.Email, intProviderAppSecretDto.Token);
//                                 if (string.IsNullOrEmpty(calendarLeadActivity.error_description))
//                                 {
//                                     syncActivityDto2.CalendarEventId = calendarLeadActivity.id;
//                                     syncActivityDto2.TenantId = TenantId;
//                                     var AddUpdate = await _calendarSyncActivityService.CheckInsertOrUpdate(syncActivityDto2);
//                                 }
//                             }
//                             else
//                             {
//                                 obj2.id = isExistData2.CalendarEventId;
//                                 var customers = await _calendarService.UpdateEvent(GoogleCalendarApiKey, obj2, intProviderAppSecretDto.Email, intProviderAppSecretDto.Token);
//                             }
//                         }
//                     }
//                     else if (IntProviderObj.Name.ToLower() == "microsoft")
//                     {
//                         foreach (var customerActivityObj in customerActivities)
//                         {
//                             MicrosoftCalendarEventVM MSEventVM = new MicrosoftCalendarEventVM();
//                             MSEventVM.UserId = UserId;
//                             MSEventVM.SelectedEmail = IntProviderAppSecretItem.Email;
//                             MSEventVM.Provider = IntProviderObj.Name;
//                             var isExistData = calendarsyncActivitites.Where(t => t.ActivityId == customerActivityObj.Id && t.ModuleId == customerModuleObj.Id && t.IntProviderAppSecretId == IntProviderAppSecretItem.Id && t.CreatedBy == UserId).FirstOrDefault();
//                             var members = _customerActivityMemberService.GetAllByActivity(customerActivityObj.Id);
//                             MSEventVM.start = new MSEventDate();
//                             MSEventVM.start.timeZone = "UTC";
//                             MSEventVM.end = new MSEventDate();
//                             MSEventVM.end.timeZone = "UTC";
//                             // if (customerActivityObj.isAllDay)
//                             // {
//                             //     model.start.dateTime = model.startDate.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
//                             //     model.end.dateTime = model.endDate.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
//                             // }
//                             // else
//                             // {
//                             var startDateTime = Common.GetStartEndTime(customerActivityObj.ScheduleStartDate.Value, customerActivityObj.StartTime);
//                             var endDateTime = Common.GetStartEndTime(customerActivityObj.ScheduleEndDate.Value, customerActivityObj.EndTime);
//                             if (startDateTime != null)
//                             {
//                                 MSEventVM.start.dateTime = startDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
//                             }

//                             if (endDateTime != null)
//                             {
//                                 MSEventVM.end.dateTime = endDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
//                             }

//                             if (members.Count() > 0)
//                             {
//                                 foreach (var customerMemberObj in members)
//                                 {
//                                     MSEventAttendee obj1 = new MSEventAttendee();
//                                     obj1.emailAddress = new MSEventEmail();
//                                     obj1.emailAddress.address = customerMemberObj.Email;
//                                     obj1.emailAddress.name = customerMemberObj.Email;
//                                     MSEventVM.attendees.Add(obj1);
//                                 }
//                             }

//                             MSEventVM.IntProviderAppSecretId = IntProviderAppSecretItem.Id;
//                             // }
//                             if (isExistData == null)
//                             {
//                                 CalendarSyncActivityDto syncActivityDto = new CalendarSyncActivityDto();
//                                 syncActivityDto.CreatedBy = UserId;
//                                 syncActivityDto.IntProviderAppSecretId = IntProviderAppSecretItem.Id;
//                                 syncActivityDto.ActivityId = customerActivityObj.Id;
//                                 syncActivityDto.ModuleId = customerModuleObj.Id;
//                                 MSEventVM = await calendar.CreateEvent(MSEventVM.UserId, null, MSEventVM);

//                                 if (string.IsNullOrEmpty(MSEventVM.ErrorMessage))
//                                 {
//                                     syncActivityDto.CalendarEventId = MSEventVM.id;
//                                     syncActivityDto.TenantId = TenantId;
//                                     var AddUpdate = await _calendarSyncActivityService.CheckInsertOrUpdate(syncActivityDto);
//                                 }
//                             }
//                             else
//                             {
//                                 MSEventVM.id = isExistData.CalendarEventId;
//                                 var customers = await calendar.UpdateEvent(UserId, isExistData.CalendarEventId, MSEventVM);
//                             }
//                         }

//                         foreach (var organizationActivityObj in organizationActivities)
//                         {
//                             var isExistData1 = calendarsyncActivitites.Where(t => t.ActivityId == organizationActivityObj.Id && t.ModuleId == organizationModuleObj.Id && t.IntProviderAppSecretId == IntProviderAppSecretItem.Id && t.CreatedBy == UserId).FirstOrDefault();

//                             var organizationMembers = _organizationActivityMemberService.GetAllByActivity(organizationActivityObj.Id);


//                             MicrosoftCalendarEventVM MSEventVM = new MicrosoftCalendarEventVM();
//                             MSEventVM.UserId = UserId;
//                             MSEventVM.SelectedEmail = IntProviderAppSecretItem.Email;
//                             MSEventVM.Provider = IntProviderObj.Name;
//                             MSEventVM.start = new MSEventDate();
//                             MSEventVM.start.timeZone = "UTC";
//                             MSEventVM.end = new MSEventDate();
//                             MSEventVM.end.timeZone = "UTC";
//                             // if (customerActivityObj.isAllDay)
//                             // {
//                             //     model.start.dateTime = model.startDate.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
//                             //     model.end.dateTime = model.endDate.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
//                             // }
//                             // else
//                             // {
//                             var startDateTime = Common.GetStartEndTime(organizationActivityObj.ScheduleStartDate.Value, organizationActivityObj.StartTime);
//                             var endDateTime = Common.GetStartEndTime(organizationActivityObj.ScheduleEndDate.Value, organizationActivityObj.EndTime);
//                             if (startDateTime != null)
//                             {
//                                 MSEventVM.start.dateTime = startDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
//                             }

//                             if (endDateTime != null)
//                             {
//                                 MSEventVM.end.dateTime = endDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
//                             }

//                             if (organizationMembers.Count() > 0)
//                             {
//                                 foreach (var organizationMemberObj in organizationMembers)
//                                 {
//                                     MSEventAttendee obj1 = new MSEventAttendee();
//                                     obj1.emailAddress = new MSEventEmail();
//                                     obj1.emailAddress.address = organizationMemberObj.Email;
//                                     obj1.emailAddress.name = organizationMemberObj.Email;
//                                     MSEventVM.attendees.Add(obj1);
//                                 }
//                             }

//                             MSEventVM.IntProviderAppSecretId = IntProviderAppSecretItem.Id;
//                             // }
//                             if (isExistData1 == null)
//                             {
//                                 CalendarSyncActivityDto syncActivityDto = new CalendarSyncActivityDto();
//                                 syncActivityDto.CreatedBy = UserId;
//                                 syncActivityDto.IntProviderAppSecretId = IntProviderAppSecretItem.Id;
//                                 syncActivityDto.ActivityId = organizationActivityObj.Id;
//                                 syncActivityDto.ModuleId = organizationModuleObj.Id;
//                                 MSEventVM = await calendar.CreateEvent(MSEventVM.UserId, null, MSEventVM);

//                                 if (string.IsNullOrEmpty(MSEventVM.ErrorMessage))
//                                 {
//                                     syncActivityDto.CalendarEventId = MSEventVM.id;
//                                     syncActivityDto.TenantId = TenantId;
//                                     var AddUpdate = await _calendarSyncActivityService.CheckInsertOrUpdate(syncActivityDto);
//                                 }
//                             }
//                             else
//                             {
//                                 MSEventVM.id = isExistData1.CalendarEventId;
//                                 var customers = await calendar.UpdateEvent(UserId, isExistData1.CalendarEventId, MSEventVM);
//                             }
//                         }


//                         foreach (var leadActivityObj in leadActivities)
//                         {
//                             var intProviderAppSecretDto = await CheckAccessToken(IntProviderAppSecretItem);

//                             var isExistData2 = calendarsyncActivitites.Where(t => t.ActivityId == leadActivityObj.Id && t.ModuleId == leadModuleObj.Id).FirstOrDefault();

//                             var leadMembers = _leadActivityMemberService.GetAllByActivity(leadActivityObj.Id);

//                             MicrosoftCalendarEventVM MSEventVM = new MicrosoftCalendarEventVM();
//                             MSEventVM.UserId = UserId;
//                             MSEventVM.SelectedEmail = IntProviderAppSecretItem.Email;
//                             MSEventVM.Provider = IntProviderObj.Name;
//                             MSEventVM.start = new MSEventDate();
//                             MSEventVM.start.timeZone = "UTC";
//                             MSEventVM.end = new MSEventDate();
//                             MSEventVM.end.timeZone = "UTC";
//                             // if (customerActivityObj.isAllDay)
//                             // {
//                             //     model.start.dateTime = model.startDate.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
//                             //     model.end.dateTime = model.endDate.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
//                             // }
//                             // else
//                             // {
//                             var startDateTime = Common.GetStartEndTime(leadActivityObj.ScheduleStartDate.Value, leadActivityObj.StartTime);
//                             var endDateTime = Common.GetStartEndTime(leadActivityObj.ScheduleEndDate.Value, leadActivityObj.EndTime);
//                             if (startDateTime != null)
//                             {
//                                 MSEventVM.start.dateTime = startDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
//                             }

//                             if (endDateTime != null)
//                             {
//                                 MSEventVM.end.dateTime = endDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
//                             }

//                             if (leadMembers.Count() > 0)
//                             {
//                                 foreach (var leadMemberObj in leadMembers)
//                                 {
//                                     MSEventAttendee obj1 = new MSEventAttendee();
//                                     obj1.emailAddress = new MSEventEmail();
//                                     obj1.emailAddress.address = leadMemberObj.Email;
//                                     obj1.emailAddress.name = leadMemberObj.Email;
//                                     MSEventVM.attendees.Add(obj1);
//                                 }
//                             }

//                             MSEventVM.IntProviderAppSecretId = IntProviderAppSecretItem.Id;
//                             // }
//                             if (isExistData2 == null)
//                             {
//                                 CalendarSyncActivityDto syncActivityDto = new CalendarSyncActivityDto();
//                                 syncActivityDto.CreatedBy = UserId;
//                                 syncActivityDto.IntProviderAppSecretId = IntProviderAppSecretItem.Id;
//                                 syncActivityDto.ActivityId = leadActivityObj.Id;
//                                 syncActivityDto.ModuleId = leadModuleObj.Id;
//                                 MSEventVM = await calendar.CreateEvent(MSEventVM.UserId, null, MSEventVM);

//                                 if (string.IsNullOrEmpty(MSEventVM.ErrorMessage))
//                                 {
//                                     syncActivityDto.CalendarEventId = MSEventVM.id;
//                                     syncActivityDto.TenantId = TenantId;
//                                     var AddUpdate = await _calendarSyncActivityService.CheckInsertOrUpdate(syncActivityDto);
//                                 }
//                             }
//                             else
//                             {
//                                 MSEventVM.id = isExistData2.CalendarEventId;
//                                 var customers = await calendar.UpdateEvent(UserId, isExistData2.CalendarEventId, MSEventVM);
//                             }
//                         }
//                     }
//                 }
//             }
//             return new OperationResult<GoogleCalendarEventVM>(true, "", googleCalendarEventVM);
//         }

//     }
// }