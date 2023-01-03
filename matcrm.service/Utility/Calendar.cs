using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
// using Microsoft.Graph;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using matcrm.data.Context;
using matcrm.data.Helpers;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.data.Models.ViewModels;
using matcrm.data.Models.ViewModels.Calendar;
using matcrm.service.BusinessLogic;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.service.Services.ERP;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Net.Mime;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using static matcrm.data.Helpers.DataUtility;

namespace matcrm.service.Utility
{
    public class Calendar
    {
        #region 'Service Object'
        private readonly OneClappContext _context;
        private readonly IUserService _userService;
        private readonly IIntProviderAppService _intProviderAppService;
        private readonly IIntProviderAppSecretService _intProviderAppSecretService;
        private readonly ICustomerAttachmentService _customerAttachmentService;
        private readonly IGoogleCalendarService _calendarService;
        private readonly IHostingEnvironment _hostingEnvironment;
        #endregion

        #region 'Global Initialization'
        private static readonly HttpClient client = new HttpClient();
        private InboxThreads inboxThreads = new InboxThreads();
        private IntProviderAppSecret intProviderAppSecretObj = new IntProviderAppSecret();
        private List<NextPageToken> NextPageToken = new List<NextPageToken>();
        private List<InboxThreadItem> inboxThreadItems = new List<InboxThreadItem>();
        private string token;
        private User user;
        private FileUpload fileUpload;
        private MailTokenDto mailTokenDto = new MailTokenDto();
        private string MicrosoftScope;
        #endregion

        #region 'Constructor'
        public Calendar(OneClappContext context, IUserService userService,
        IHostingEnvironment hostingEnvironment,
         IIntProviderAppService intProviderAppService,
         IIntProviderAppSecretService intProviderAppSecretService,
         ICustomerAttachmentService customerAttachmentService,
         IGoogleCalendarService calendarService)
        {
            _userService = userService;
            _hostingEnvironment = hostingEnvironment;
            _intProviderAppService = intProviderAppService;
            _intProviderAppSecretService = intProviderAppSecretService;
            _customerAttachmentService = customerAttachmentService;
            _calendarService = calendarService;
            fileUpload = new FileUpload(hostingEnvironment, customerAttachmentService);
            // System settings
            // systemSettingService.GetSystemSettingListByCodes(("GMTOK,GMTRD,GMTRI,GMLMS,GMMSI,GMLML,GMLSM,GMLGL,GMGLC,GMATH,GMLST,GMUNR,GMCID,GMCSC,OFCID,OFCSC,OFTOK,OFTRD,OFATR,OFTDS,OFTDI,OFTBI,OFSDM,OFCRM,OFDLE,OFFDE,OFRPE,OFATH,OFUNR,OFFOL,OFALL,OFINB,OFSNT,OFDRF,OFDLI,OFJKE,AZFLS").Split(","));
            MicrosoftScope = "offline_access Mail.ReadWrite Mail.Send User.Read MailboxSettings.Read Calendars.Read Calendars.ReadWrite Calendars.Read.Shared";
        }
        #endregion

        #region 'Get Function'
        // public async Task<MSCalendarEvent> GetEvents(int userId, InboxVM model)
        // {
        //     MSCalendarEvent eventVM = new MSCalendarEvent();
        //     var user = _userService.GetUserById(userId);
        //     if (user == null)
        //     {
        //         eventVM.IsValid = false;
        //         eventVM.ErrorMessage = CommonMessage.UnAuthorizedUser;
        //         return eventVM;
        //     }

        //     var task = new List<Task>();
        //     var intproviderApp = _intProviderAppService.GetIntProviderApp(model.Provider);

        //     var intAppSecretObj = _intProviderAppSecretService.GetByUserAndEmail(userId, model.SelectedEmail);


        //     if (intproviderApp != null && intAppSecretObj != null)
        //     {
        //         mailTokenDto.refresh_token = intAppSecretObj.Refresh_Token;
        //         mailTokenDto.access_token = intAppSecretObj.Access_Token;
        //         mailTokenDto.code = model.Code;
        //         mailTokenDto.providerApp = model.Provider;
        //         mailTokenDto.UserId = userId;
        //         intProviderAppSecretObj.Refresh_Token = intAppSecretObj.Refresh_Token;
        //         intProviderAppSecretObj.Access_Token = intAppSecretObj.Access_Token;
        //         intProviderAppSecretObj.Email = intAppSecretObj.Email;
        //     }
        //     // var emailAccount = _emailAccountService.GetEmailAccountById(model.UserEmail.AccountId);
        //     // userEmail = _userEmailService.GetUserEmailById(model.UserEmail.UserEmailId);

        //     switch (intproviderApp.Name)
        //     {
        //         case "Gmail":
        //             await SetGmailToken();
        //             client.DefaultRequestHeaders.Clear();
        //             client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
        //             // GmailThreads gmailThreads = null;
        //             // model.Label = "inbox";
        //             // model.NextPageToken = "";
        //             // model.Query = "";
        //             // var api = string.Format(DataUtility.GmailThreads, LabelBuilder(model.Label), 30, model.NextPageToken, model.Query);
        //             // //var gmailResponse = await client.GetAsync(string.Format(DataUtility.GmailThreads, LabelBuilder(model.Label), 30, model.NextPageToken, model.Query));
        //             // var gmailResponse = await client.GetAsync(api);
        //             // if (gmailResponse.StatusCode == HttpStatusCode.OK)
        //             // {
        //             //     var stream = await gmailResponse.Content.ReadAsStreamAsync();
        //             //     var serializer = new DataContractJsonSerializer(typeof(GmailThreads));
        //             //     gmailThreads = (GmailThreads)serializer.ReadObject(stream);
        //             // }



        //             break;

        //         case "Office 365":
        //         case "Outlook":

        //             await SetOffice365Token();
        //             client.DefaultRequestHeaders.Clear();
        //             client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

        //             string office365Uri = DataUtility.AllMSCalendarEvents;

        //             var office365Response = await client.GetAsync(office365Uri);
        //             if (office365Response.StatusCode == HttpStatusCode.OK)
        //             {

        //                 var json = JObject.Parse(await office365Response.Content.ReadAsStringAsync());
        //                 eventVM.Events = JsonConvert.DeserializeObject<List<MicrosoftCalendarEventVM>>(json.ToString());

        //                 //var json = await client.GetStringAsync(label == "All" ? string.Format(office365Uri, skip, top) : string.Format(office365Uri, label, skip, top));
        //                 //var odata = JsonConvert.DeserializeObject<OData>(json);
        //                 //office365Threads.count = odata.count;

        //             }


        //             break;

        //         default:
        //             break;
        //     }

        //     return eventVM;
        // }

        public async Task<MicrosoftCalendarEventVM> GetEventByEventId(int userId, string eventId, MicrosoftCalendarEventVM model)
        {
            user = _userService.GetUserById(userId);
            if (user == null)
            {
                model.IsValid = false;
                model.ErrorMessage = CommonMessage.UnAuthorizedUser;
                return model;
            }

            // var emailAccount = _emailAccountService.GetEmailAccountById(model.UserEmail.AccountId);
            // userEmail = _userEmailService.GetUserEmailById(model.UserEmail.UserEmailId);

            var intproviderApp = _intProviderAppService.GetIntProviderApp(model.ProviderApp);

            var intAppSecretObj = _intProviderAppSecretService.GetByUserAndEmail(userId, model.SelectedEmail);


            if (intproviderApp != null && intAppSecretObj != null)
            {
                mailTokenDto.refresh_token = intAppSecretObj.Refresh_Token;
                mailTokenDto.access_token = intAppSecretObj.Access_Token;
                mailTokenDto.code = model.Code;
                mailTokenDto.ProviderApp = model.Provider;
                mailTokenDto.UserId = userId;
                intProviderAppSecretObj.Refresh_Token = intAppSecretObj.Refresh_Token;
                intProviderAppSecretObj.Access_Token = intAppSecretObj.Access_Token;
                intProviderAppSecretObj.Email = intAppSecretObj.Email;
            }

            switch (intproviderApp.Name)
            {
                case "Gmail":
                    await SetGmailToken();
                    // var task = new List<Task>();
                    // inboxThreadItems = new List<InboxThreadItem>();
                    // InboxVM objInbox = new InboxVM();
                    // // objInbox.UserEmail = userEmail;
                    // task.Add(SetInboxThreadDetail(threadId, objInbox, true));
                    // Task.WaitAll(task.ToArray());

                    break;

                case "Office 365":
                case "Outlook":

                    await SetOffice365Token();
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    inboxThreadItems = new List<InboxThreadItem>();
                    // var threadResponse = await client.GetAsync(string.Format(DataUtility.Office365ThreadByConversionId, threadId)); //DataUtility.Office365ThreadByConversionId
                    var url = string.Format(DataUtility.GetMSCalendarEventById, eventId);
                    // var url2 = "https://graph.microsoft.com/v1.0/me/messages/AQMkADAwATMwMAItNDg3YS0zYzYyLTAwAi0wMAoARgAAAzLJyQDbpCyjSqlr6NglX8N7BwBh1tJcKOcASqMJqUbgSxJVAAACAQkAAABh1tJcKOcASqMJqUbgSxJVAAAAA7NxNAAAAA==";
                    var threadResponse = await client.GetAsync(url);
                    if (threadResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var stream = await threadResponse.Content.ReadAsStreamAsync();
                        var serializer = new DataContractJsonSerializer(typeof(MicrosoftCalendarEventVM));
                        model = (MicrosoftCalendarEventVM)serializer.ReadObject(stream);
                    }

                    break;

                default:
                    break;
            }

            return model;
        }

        public async Task<MSCalendarEvent> GetEvents(int userId, InboxVM model)
        {
            MSCalendarEvent eventVM = new MSCalendarEvent();
            var user = _userService.GetUserById(userId);
            if (user == null)
            {
                eventVM.IsValid = false;
                eventVM.ErrorMessage = CommonMessage.UnAuthorizedUser;
                return eventVM;
            }

            var task = new List<Task>();
            var intproviderApp = _intProviderAppService.GetIntProviderApp(model.ProviderApp);

            var intAppSecretObj = _intProviderAppSecretService.GetByUserAndEmail(userId, model.SelectedEmail);


            if (intproviderApp != null && intAppSecretObj != null)
            {
                mailTokenDto.refresh_token = intAppSecretObj.Refresh_Token;
                mailTokenDto.access_token = intAppSecretObj.Access_Token;
                mailTokenDto.code = model.Code;
                mailTokenDto.ProviderApp = model.Provider;
                mailTokenDto.UserId = userId;
                intProviderAppSecretObj.Refresh_Token = intAppSecretObj.Refresh_Token;
                intProviderAppSecretObj.Access_Token = intAppSecretObj.Access_Token;
                intProviderAppSecretObj.Email = intAppSecretObj.Email;
            }
            // var emailAccount = _emailAccountService.GetEmailAccountById(model.UserEmail.AccountId);
            // userEmail = _userEmailService.GetUserEmailById(model.UserEmail.UserEmailId);

            switch (intproviderApp.Name)
            {
                case "Gmail":
                    await SetGmailToken();
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    GoogleCalendarEventVM gmailThreads = null;
                    model.Label = "inbox";
                    model.NextPageToken = "";
                    model.Query = "";
                    var api = string.Format(DataUtility.AllGoogleCalendarEvents, model.SelectedEmail);
                    //var gmailResponse = await client.GetAsync(string.Format(DataUtility.GmailThreads, LabelBuilder(model.Label), 30, model.NextPageToken, model.Query));
                    var googleCalendarResponse = await client.GetAsync(api);
                    if (googleCalendarResponse.StatusCode == HttpStatusCode.OK)
                    {
                        // var stream = await googleCalendarResponse.Content.ReadAsStreamAsync();
                        // var serializer = new DataContractJsonSerializer(typeof(GoogleCalendarEventVM));
                        // gmailThreads = (GoogleCalendarEventVM)serializer.ReadObject(stream);
                        var json = JObject.Parse(await googleCalendarResponse.Content.ReadAsStringAsync());
                        var data1 = JsonConvert.DeserializeObject<MSCalendarEvent>(json.ToString());
                        eventVM = data1;
                    }



                    break;

                case "Office 365":
                case "Calendar":
                case "Outlook":

                    await SetOffice365Token();
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    string office365Uri = DataUtility.AllMSCalendarEvents;

                    var office365Response = await client.GetAsync(office365Uri);
                    if (office365Response.StatusCode == HttpStatusCode.OK)
                    {

                        var json = JObject.Parse(await office365Response.Content.ReadAsStringAsync());
                        eventVM = JsonConvert.DeserializeObject<MSCalendarEvent>(json.ToString());

                        //var json = await client.GetStringAsync(label == "All" ? string.Format(office365Uri, skip, top) : string.Format(office365Uri, label, skip, top));
                        //var odata = JsonConvert.DeserializeObject<OData>(json);
                        //office365Threads.count = odata.count;

                    }


                    break;

                default:
                    break;
            }

            return eventVM;
        }

        public async Task<GoogleMicrosoftCalendarEventVM> UpdateEvent(int userId, string eventId, GoogleMicrosoftCalendarEventVM model)
        {
            user = _userService.GetUserById(userId);
            if (user == null)
            {
                model.IsValid = false;
                model.ErrorMessage = CommonMessage.UnAuthorizedUser;
                return model;
            }

            IntProviderApp? intProviderApp;
            IntProviderAppSecret? intAppSecretObj;
            if (model.IntProviderAppSecretId != null)
            {
                intAppSecretObj = _intProviderAppSecretService.GetIntProviderAppSecretById(model.IntProviderAppSecretId.Value);
                intProviderApp = null;
                if (intAppSecretObj.IntProviderApp != null)
                {
                    intProviderApp = intAppSecretObj.IntProviderApp;
                    if (intAppSecretObj.IntProviderApp.IntProvider != null)
                    {
                        model.Provider = intAppSecretObj.IntProviderApp.IntProvider.Name;
                    }
                }
            }
            else
            {
                intProviderApp = _intProviderAppService.GetIntProviderApp(model.Provider);

                intAppSecretObj = _intProviderAppSecretService.GetByUserAndEmail(userId, model.SelectedEmail);
            }

            if (intProviderApp != null && intAppSecretObj != null)
            {
                mailTokenDto.refresh_token = intAppSecretObj.Refresh_Token;
                mailTokenDto.access_token = intAppSecretObj.Access_Token;
                mailTokenDto.code = model.Code;
                mailTokenDto.ProviderApp = model.Provider;
                mailTokenDto.UserId = userId;
                intProviderAppSecretObj.Refresh_Token = intAppSecretObj.Refresh_Token;
                intProviderAppSecretObj.Access_Token = intAppSecretObj.Access_Token;
                intProviderAppSecretObj.Email = intAppSecretObj.Email;
                model.SelectedEmail = intAppSecretObj.Email;
            }

            switch (model.Provider)
            {
                case "Google":
                    await SetGmailToken();
                    dynamic myObject = new ExpandoObject();
                    myObject.isAllDay = model.isAllDay;
                    if (model.isAllDay == true)
                    {
                        var StartDate = Convert.ToDateTime(model.start.dateTime);
                        var StartDateNew = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, 0, 0, 0);
                        myObject.start = new ExpandoObject();
                        myObject.start.date = StartDateNew.ToString("yyyy-MM-dd");

                        var EndDate = Convert.ToDateTime(model.end.dateTime);
                        var EndDateNew = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day, 0, 0, 0);
                        myObject.end = new ExpandoObject();
                        myObject.end.date = EndDateNew.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        myObject.start = model.start;
                        myObject.end = model.end;
                    }
                    if (!string.IsNullOrEmpty(model.summary))
                    {
                        myObject.summary = model.summary;
                    }
                    // else
                    // {
                    //     myObject.summary = "";
                    // }
                    if (!string.IsNullOrEmpty(model.description))
                    {
                        myObject.description = model.description;
                    }
                    // else
                    // {
                    //     myObject.description = "";
                    // }
                    if (model.Recurrence != null && model.Recurrence.Length > 0)
                    {
                        myObject.Recurrence = model.Recurrence;
                    }
                    if (model.attendee.Count > 0)
                    {
                        // myObject.Attendee = model.attendee;

                        myObject.attendees = new List<CalendarAttendee>();
                        foreach (var attendeeObj in model.attendee)
                        {
                            CalendarAttendee attendeeObj1 = new CalendarAttendee();
                            attendeeObj1.email = attendeeObj.email;
                            myObject.attendees.Add(attendeeObj1);
                        }
                        myObject.attendees = myObject.attendees.ToArray();
                    }
                    if (!string.IsNullOrEmpty(model.Location))
                    {
                        myObject.Location = model.Location;
                    }
                    if (!string.IsNullOrEmpty(model.colorId))
                    {
                        myObject.colorId = model.colorId;
                    }
                    if (model.creator != null)
                    {
                        myObject.creator = new EventCreatorOrganizer();
                        myObject.creator.email = model.creator.email;
                    }
                    // Serialize our concrete class into a JSON String
                    var stringPayload1 = JsonConvert.SerializeObject(myObject);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
                    var httpContent1 = new StringContent(stringPayload1, Encoding.UTF8, "application/json");
                    var apiUrl = string.Format(DataUtility.GoogleCalendarUpdateEvent, model.SelectedEmail, eventId);
                    var googleResponse = await client.PutAsync(apiUrl, httpContent1);
                    if (googleResponse.StatusCode == HttpStatusCode.OK)
                    {
                        model.IsValid = true;
                        var stream = await googleResponse.Content.ReadAsStreamAsync();
                        var serializer = new DataContractJsonSerializer(typeof(GoogleCalendarEventVM));
                        var data1 = (GoogleCalendarEventVM)serializer.ReadObject(stream);
                    }

                    break;

                case "Office 365":
                case "Outlook":
                case "Microsoft":

                    await SetOffice365Token();
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    // var threadResponse = await client.GetAsync(string.Format(DataUtility.Office365ThreadByConversionId, threadId)); //DataUtility.Office365ThreadByConversionId
                    var url = string.Format(DataUtility.UpdateMSCalendarEvent, eventId);

                    dynamic MyDynamic = new ExpandoObject();
                    MyDynamic.id = model.id;
                    MyDynamic.subject = model.summary;
                    if (model.isAllDay)
                    {
                        var date = Convert.ToDateTime(model.start.dateTime);
                        var NewDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                        var EndDateNew = NewDate.AddDays(1);
                        model.start.dateTime = NewDate.ToString("yyyy-MM-ddTHH:mm:ss");
                        model.end.dateTime = EndDateNew.ToString("yyyy-MM-ddTHH:mm:ss");
                    }

                    MyDynamic.start = model.start;
                    MyDynamic.end = model.end;

                    if (model.attendee.Count() > 0)
                    {
                        MyDynamic.attendees = new List<MSEventAttendee>();
                        foreach (var attendeeObj in model.attendee)
                        {
                            MSEventAttendee MSEventAttendeeObj = new MSEventAttendee();
                            MSEventAttendeeObj.emailAddress = new MSEventEmail();
                            MSEventAttendeeObj.emailAddress.address = attendeeObj.email;
                            MSEventAttendeeObj.emailAddress.name = attendeeObj.email;
                            MyDynamic.attendees.Add(MSEventAttendeeObj);
                        }
                        // MyDynamic.attendees = model.attendees;
                    }
                     if (model.body != null)
                    {
                        model.body = new CalendarEventBody();
                        model.body.contentType = "HTML";
                        model.body.content = model.description;
                        MyDynamic.body = model.body;
                    }
                    else
                    {
                        model.body = new CalendarEventBody();
                        model.body.contentType = "HTML";
                        model.body.content = model.description;
                        MyDynamic.body = model.body;
                    }
                    if (model.Location != null)
                    {
                        MyDynamic.location = model.Location;
                    }
                    // if (model.locations.Count() > 0)
                    // {
                    //     MyDynamic.locations = model.locations;
                    // }
                    MyDynamic.isAllDay = model.isAllDay;
                    MyDynamic.isCancelled = model.isCancelled;

                    // Serialize our concrete class into a JSON String
                    var stringPayload = JsonConvert.SerializeObject(MyDynamic);

                    // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
                    var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                    var MSResponse = await client.PatchAsync(url, httpContent);
                    if (MSResponse.StatusCode == HttpStatusCode.OK)
                    {
                        model.IsValid = true;
                        var stream = await MSResponse.Content.ReadAsStreamAsync();
                        var serializer = new DataContractJsonSerializer(typeof(MicrosoftCalendarEventVM));
                        // var data1 = (MicrosoftCalendarEventVM)serializer.ReadObject(stream);
                    }

                    break;

                default:
                    break;
            }

            return model;
        }

        public async Task<GoogleMicrosoftCalendarEventVM> CreateEvent(int userId, string eventId, GoogleMicrosoftCalendarEventVM model)
        {
            user = _userService.GetUserById(userId);
            if (user == null)
            {
                model.IsValid = false;
                model.ErrorMessage = CommonMessage.UnAuthorizedUser;
                return model;
            }

            // var emailAccount = _emailAccountService.GetEmailAccountById(model.UserEmail.AccountId);
            // userEmail = _userEmailService.GetUserEmailById(model.UserEmail.UserEmailId);
            IntProviderApp? intproviderApp = null;
            IntProviderAppSecret? intAppSecretObj;
            if (model.IntProviderAppSecretId != null)
            {
                intAppSecretObj = _intProviderAppSecretService.GetIntProviderAppSecretById(model.IntProviderAppSecretId.Value);

                if (intAppSecretObj.IntProviderApp != null)
                {
                    intproviderApp = intAppSecretObj.IntProviderApp;
                }
                if (intAppSecretObj.IntProviderApp.IntProvider != null)
                {
                    model.Provider = intAppSecretObj.IntProviderApp.IntProvider.Name;
                }
            }
            else
            {
                intproviderApp = _intProviderAppService.GetIntProviderApp(model.Provider);

                intAppSecretObj = _intProviderAppSecretService.GetByUserAndEmail(userId, model.SelectedEmail);
            }

            if (intproviderApp != null && intAppSecretObj != null)
            {
                mailTokenDto.refresh_token = intAppSecretObj.Refresh_Token;
                mailTokenDto.access_token = intAppSecretObj.Access_Token;
                mailTokenDto.code = model.Code;
                mailTokenDto.ProviderApp = model.Provider;
                mailTokenDto.UserId = userId;
                intProviderAppSecretObj.Refresh_Token = intAppSecretObj.Refresh_Token;
                intProviderAppSecretObj.Access_Token = intAppSecretObj.Access_Token;
                intProviderAppSecretObj.Email = intAppSecretObj.Email;
                model.SelectedEmail = intAppSecretObj.Email;
            }

            switch (model.Provider)
            {
                case "Google":
                    await SetGmailToken();
                    dynamic myObject = new ExpandoObject();
                    myObject.isAllDay = model.isAllDay;
                    if (model.isAllDay == true)
                    {
                        var StartDate = Convert.ToDateTime(model.start.dateTime);
                        var StartDateNew = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, 0, 0, 0);
                        myObject.start = new ExpandoObject();
                        myObject.start.date = StartDateNew.ToString("yyyy-MM-dd");

                        var EndDate = Convert.ToDateTime(model.end.dateTime);
                        var EndDateNew = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day, 0, 0, 0);
                        myObject.end = new ExpandoObject();
                        myObject.end.date = EndDateNew.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        myObject.start = model.start;
                        myObject.end = model.end;
                    }

                    if (!string.IsNullOrEmpty(model.summary))
                    {
                        myObject.summary = model.summary;
                    }
                    // else
                    // {
                    //     myObject.summary = "";
                    // }
                    if (!string.IsNullOrEmpty(model.description))
                    {
                        myObject.description = model.description;
                    }
                    // else
                    // {
                    //     myObject.description = "";
                    // }
                    if (model.Recurrence != null && model.Recurrence.Length > 0)
                    {
                        myObject.Recurrence = model.Recurrence;
                    }
                    if (model.attendee.Count > 0)
                    {
                        // myObject.Attendee = model.attendee;

                        myObject.attendees = new List<CalendarAttendee>();
                        foreach (var attendeeObj in model.attendee)
                        {
                            CalendarAttendee attendeeObj1 = new CalendarAttendee();
                            attendeeObj1.email = attendeeObj.email;
                            myObject.attendees.Add(attendeeObj1);
                        }
                        myObject.attendees = myObject.attendees.ToArray();
                    }

                    if (!string.IsNullOrEmpty(model.Location))
                    {
                        myObject.Location = model.Location;
                    }
                    if (!string.IsNullOrEmpty(model.colorId))
                    {
                        myObject.colorId = model.colorId;
                    }
                    if (model.creator != null)
                    {
                        myObject.creator = new EventCreatorOrganizer();
                        myObject.creator.email = model.creator.email;
                    }
                    // Serialize our concrete class into a JSON String
                    var stringPayload1 = JsonConvert.SerializeObject(myObject);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
                    var httpContent1 = new StringContent(stringPayload1, Encoding.UTF8, "application/json");
                    var apiUrl = string.Format(DataUtility.AllGoogleCalendarEvents, model.SelectedEmail);
                    var googleResponse = await client.PostAsync(apiUrl, httpContent1);
                    if (googleResponse.StatusCode == HttpStatusCode.OK)
                    {
                        model.IsValid = true;
                        model.error_description = "";
                        var data = await googleResponse.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(data))
                        {
                            GoogleCalendarEventVM result = JsonConvert.DeserializeObject<GoogleCalendarEventVM>(data);
                            if (result != null)
                            {
                                model.id = result.id;
                            }
                        }
                    }

                    break;
                case "Office 365":
                case "Outlook":
                case "Microsoft":

                    await SetOffice365Token();
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    // var threadResponse = await client.GetAsync(string.Format(DataUtility.Office365ThreadByConversionId, threadId)); //DataUtility.Office365ThreadByConversionId
                    var url = string.Format(DataUtility.CreateMSCalendarEvent);
                    dynamic MyDynamic = new ExpandoObject();
                    MyDynamic.subject = model.summary;
                    if (model.isAllDay)
                    {
                        var date = Convert.ToDateTime(model.start.dateTime);
                        var NewDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                        var EndDateNew = NewDate.AddDays(1);
                        model.start.dateTime = NewDate.ToString("yyyy-MM-ddTHH:mm:ss");
                        model.end.dateTime = EndDateNew.ToString("yyyy-MM-ddTHH:mm:ss");
                    }
                    if (!string.IsNullOrEmpty(model.description))
                    {
                        MyDynamic.bodyPreview = model.description;
                    }

                    MyDynamic.start = model.start;
                    MyDynamic.end = model.end;

                    if (model.attendee.Count > 0)
                    {
                        MyDynamic.attendees = new List<MSEventAttendee>();
                        foreach (var attendeeObj in model.attendee)
                        {
                            MSEventAttendee attendeeObj1 = new MSEventAttendee();
                            attendeeObj1.emailAddress.address = attendeeObj.email;
                            attendeeObj1.emailAddress.name = attendeeObj.email;
                            MyDynamic.attendees.Add(attendeeObj1);
                        }
                        //  MyDynamic.attendees = myObject.attendees.ToArray();
                    }
                    if (model.body != null)
                    {
                        model.body = new CalendarEventBody();
                        model.body.contentType = "HTML";
                        model.body.content = model.description;
                        MyDynamic.body = model.body;
                    }
                    else
                    {
                        model.body = new CalendarEventBody();
                        model.body.contentType = "HTML";
                        model.body.content = model.description;
                        MyDynamic.body = model.body;
                    }
                    if (model.Location != null)
                    {
                        MyDynamic.location = model.Location;
                    }
                    // if (model.locations.Count() > 0)
                    // {
                    //     MyDynamic.locations = model.locations;
                    // }
                    MyDynamic.isAllDay = model.isAllDay;
                    MyDynamic.isCancelled = model.isCancelled;

                    // Serialize our concrete class into a JSON String
                    var stringPayload = JsonConvert.SerializeObject(MyDynamic);

                    // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
                    var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                    var threadResponse = await client.PostAsync(url, httpContent);
                    if (threadResponse.StatusCode == HttpStatusCode.Created)
                    {
                        model.IsValid = true;
                        model.error_description = "";
                        var data = await threadResponse.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(data))
                        {
                            MicrosoftCalendarEventVM result = JsonConvert.DeserializeObject<MicrosoftCalendarEventVM>(data);
                            if (result != null)
                            {
                                model.id = result.id;
                            }
                        }
                    }
                    else
                    {
                        model.IsValid = false;
                    }

                    break;

                default:
                    break;
            }

            return model;
        }


        #region Get Email Contact wise

        public async Task<GoogleMicrosoftCalendarEventVM> DeleteEvent(int userId, string eventId, GoogleMicrosoftCalendarEventVM model)
        {
            user = _userService.GetUserById(userId);
            if (user == null)
            {
                model.IsValid = false;
                model.ErrorMessage = CommonMessage.UnAuthorizedUser;
                return model;
            }

            IntProviderApp? intproviderApp = null;
            IntProviderAppSecret? intAppSecretObj;
            if (model.IntProviderAppSecretId != null)
            {
                intAppSecretObj = _intProviderAppSecretService.GetIntProviderAppSecretById(model.IntProviderAppSecretId.Value);

                if (intAppSecretObj.IntProviderApp != null)
                {
                    intproviderApp = intAppSecretObj.IntProviderApp;
                }
                if (intAppSecretObj.IntProviderApp.IntProvider != null)
                {
                    model.Provider = intAppSecretObj.IntProviderApp.IntProvider.Name;
                }
            }
            else
            {
                intproviderApp = _intProviderAppService.GetIntProviderApp(model.Provider);

                intAppSecretObj = _intProviderAppSecretService.GetByUserAndEmail(userId, model.SelectedEmail);
            }

            if (intproviderApp != null && intAppSecretObj != null)
            {
                mailTokenDto.refresh_token = intAppSecretObj.Refresh_Token;
                mailTokenDto.access_token = intAppSecretObj.Access_Token;
                mailTokenDto.code = model.Code;
                mailTokenDto.ProviderApp = model.Provider;
                mailTokenDto.UserId = userId;
                intProviderAppSecretObj.Refresh_Token = intAppSecretObj.Refresh_Token;
                intProviderAppSecretObj.Access_Token = intAppSecretObj.Access_Token;
                intProviderAppSecretObj.Email = intAppSecretObj.Email;
                model.SelectedEmail = intAppSecretObj.Email;
            }

            switch (intproviderApp.Name)
            {
                case "Gmail":
                    await SetGmailToken();

                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    var apiUrl = string.Format(DataUtility.GoogleCalendarUpdateEvent, model.SelectedEmail, eventId);
                    var googleResponse = await client.DeleteAsync(apiUrl);
                    if (googleResponse.StatusCode == HttpStatusCode.NoContent)
                    {
                        model.IsValid = true;
                    }


                    break;

                case "Office 365":
                case "Outlook":

                    await SetOffice365Token();
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    var url = string.Format(DataUtility.DeleteMSCalendarEventById, eventId);
                    var threadResponse = await client.DeleteAsync(url);
                    if (threadResponse.StatusCode == HttpStatusCode.NoContent)
                    {
                        // model.ErrorMessage = "Delete successfully.";
                        model.IsValid = true;
                    }
                    else
                    {
                        model.IsValid = false;
                    }

                    break;

                default:
                    break;
            }

            return model;
        }

        #endregion
        #endregion
        #region 'Set Function'
        public async Task<MailTokenDto> AuthenticationComplete(int userId, MailTokenDto model)
        {

            user = _userService.GetUserById(userId);
            if (user == null)
            {
                model.IsValid = false;
                model.ErrorMessage = CommonMessage.UnAuthorizedUser;
                return model;
            }
            // var userAllEmail = _userEmailService.GetAllUserEmailByUserId(userId);

            // if (model.Filterdata != null && model.Filterdata.Count > 0)
            // {
            //     userAllEmail = userAllEmail.Where(a => model.Filterdata.Any(f => a.UserEmailId == f)).ToList();
            // }
            // else if (model.UserEmailId > 0)
            // {
            //     userAllEmail = userAllEmail.FindAll(x => x.UserEmailId == model.UserEmailId).ToList();
            // }

            var task = new List<Task>();
            // if (user != null)
            // {
            inboxThreads.InboxThread = new List<InboxThread>();
            // inboxThreads.EmailAccount = _emailAccountService.GetAllEmailAccount();
            // foreach (var item in userAllEmail)
            // {
            // var emailAccount = _emailAccountService.GetEmailAccountById(item.AccountId);
            // userEmail = item;

            var intproviderApp = _intProviderAppService.GetIntProviderApp(model.ProviderApp);

            var intAppSecretObj = _intProviderAppSecretService.GetByUserAndEmail(model.UserId, model.SelectedEmail);


            if (intproviderApp != null && intAppSecretObj != null)
            {
                mailTokenDto.refresh_token = intAppSecretObj.Refresh_Token;
                mailTokenDto.access_token = intAppSecretObj.Access_Token;
                // mailTokenDto.code = model.Code;
                mailTokenDto.ProviderApp = model.Provider;
                mailTokenDto.UserId = userId;
                intProviderAppSecretObj.Refresh_Token = intAppSecretObj.Refresh_Token;
                intProviderAppSecretObj.Access_Token = intAppSecretObj.Access_Token;
                intProviderAppSecretObj.Email = intAppSecretObj.Email;
            }
            UserEmail userEmail = new UserEmail();


            userEmail.UserId = model.UserId;
            userEmail.Email = model.SelectedEmail;
            // userEmail.Code = model.Code;
            // userEmail.AccountId = model.AccountId;
            userEmail.CreatedOn = DataUtility.GetCurrentDateTime();
            // model.UserEmailId = userEmail.UserEmailId;

            // if (userEmail == null || userEmail.UserEmailId <= 0)
            // {
            //     model.IsValid = false;
            //     model.ErrorMessage = CommonMessage.SomethingWentWrong;
            //     return model;
            // }

            switch (intproviderApp.Name)
            {
                case "Gmail":
                    var isTokenReceived = await GetGmailToken(model, model.redirect_uri);
                    if (!isTokenReceived)
                    {
                        // model.IsTokenReceived = false;
                        model.IsValid = false;
                        model.ErrorMessage = CommonMessage.ErrorOccuredInTokenGet;
                        return model;
                    }

                    break;

                case "Office 365":
                case "Outlook":
                    var isToken365Received = await GetOffice365Token(model, model.redirect_uri);
                    if (!isToken365Received)
                    {
                        // model.IsTokenReceived = false;
                        model.IsValid = false;
                        model.ErrorMessage = CommonMessage.ErrorOccuredInTokenGet;
                        return model;
                    }

                    break;
            }

            model.IsValid = true;
            model.ErrorMessage = CommonMessage.AuthenticationSuccess;
            return model;
        }


        private async Task<bool> GetGmailToken(MailTokenDto model, string redirectUri)
        {
            var isTokenReceived = false;
            client.DefaultRequestHeaders.Clear();

            var param = new Dictionary<string, string>();
            param.Add("code", model.code);
            param.Add("client_id", OneClappContext.GoogleCalendarClientId);   //DataUtility.GmailApiClientId
            param.Add("client_secret", OneClappContext.GoogleCalendarSecretKey);   //DataUtility.GmailApiClientSecret
            param.Add("redirect_uri", redirectUri);
            param.Add("grant_type", "authorization_code");

            //var response = await client.PostAsync(DataUtility.GmailToken, new FormUrlEncodedContent(param));
            var response = await client.PostAsync(DataUtility.GmailToken, new FormUrlEncodedContent(param));
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var serializer = new DataContractJsonSerializer(typeof(Token));
                var gmailToken = (Token)serializer.ReadObject(stream);

                if (gmailToken != null && !string.IsNullOrEmpty(gmailToken.access_token))
                {

                    var IntProviderAppObj = _intProviderAppService.GetIntProviderApp(model.Provider);
                    IntProviderAppSecretDto secretDto = new IntProviderAppSecretDto();
                    secretDto.Access_Token = gmailToken.access_token;
                    secretDto.Expires_In = gmailToken.expires_in;
                    secretDto.Refresh_Token = gmailToken.refresh_token;
                    secretDto.Scope = gmailToken.scope;
                    secretDto.Token_Type = gmailToken.token_type;
                    secretDto.Id_Token = gmailToken.id_token;
                    secretDto.CreatedBy = model.UserId;
                    secretDto.IntProviderAppId = IntProviderAppObj.Id;

                    var tokenInfo = await _calendarService.GetTokenInfo(gmailToken.access_token, OneClappContext.GoogleCalendarSecretKey);
                    if ((tokenInfo != null && (string.IsNullOrEmpty(tokenInfo.error_description))))
                    {
                        secretDto.Email = tokenInfo.email;
                    }

                    intProviderAppSecretObj = await _intProviderAppSecretService.CheckInsertOrUpdate(secretDto);
                    token = gmailToken.access_token;
                    model.access_token = token;
                    model.refresh_token = gmailToken.refresh_token;
                    model.expires_in = Convert.ToInt64(gmailToken.expires_in);
                    model.scope = gmailToken.scope;
                    var dt = DataUtility.GetCurrentDateTime();
                    //model.ExpireOn = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second + Convert.ToInt32(model.ExpireIn));
                    model.refresh_token = gmailToken.refresh_token;

                    // _userEmailService.CheckInsertOrUpdate(model);
                    isTokenReceived = true;
                }
            }

            return isTokenReceived;
        }

        private async Task SetGmailToken()
        {
            //string reToken = "1/aGnkz31yzmqGcnr2BIHlHjNJ8ViLXrGFX8iYbOFp1CTSehEjdTnLbY8WC-X_Z5VF";

            client.DefaultRequestHeaders.Clear();

            var param = new Dictionary<string, string>();
            param.Add("client_id", OneClappContext.GoogleCalendarClientId);           //DataUtility.GmailApiClientId
            param.Add("client_secret", OneClappContext.GoogleCalendarSecretKey);   //DataUtility.GmailApiClientSecret
            param.Add("refresh_token", intProviderAppSecretObj.Refresh_Token);
            param.Add("grant_type", "refresh_token");

            //var response = await client.PostAsync(DataUtility.GmailToken, new FormUrlEncodedContent(param));
            var response = await client.PostAsync(DataUtility.GmailToken, new FormUrlEncodedContent(param));
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var serializer = new DataContractJsonSerializer(typeof(Token));
                var gmailToken = (Token)serializer.ReadObject(stream);

                if (gmailToken != null) token = gmailToken.access_token;
            }
        }

        #endregion

        #region 'Office 365 Private Function'
        private async Task<bool> GetOffice365Token(MailTokenDto model, string redirectUri)
        {
            var isTokenReceived = false;
            client.DefaultRequestHeaders.Clear();

            var param = new Dictionary<string, string>();
            param.Add("grant_type", "authorization_code");
            param.Add("code", model.code);
            param.Add("client_id", DataUtility.Office365ClientId);          //DataUtility.Office365ClientId
            param.Add("client_secret", DataUtility.Office365ClientSecret);  //DataUtility.Office365ClientSecret
            param.Add("redirect_uri", redirectUri);
            param.Add("scope", "offline_access Mail.ReadWrite Mail.Send User.Read");

            //var response = await client.PostAsync(DataUtility.Office365Token, new FormUrlEncodedContent(param));
            var response = await client.PostAsync(DataUtility.Office365Token, new FormUrlEncodedContent(param));
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var serializer = new DataContractJsonSerializer(typeof(Token));
                var office365Token = (Token)serializer.ReadObject(stream);

                if (office365Token != null && !string.IsNullOrEmpty(office365Token.access_token))
                {
                    token = office365Token.access_token;
                    model.access_token = token;
                    model.refresh_token = office365Token.refresh_token;
                    // model.expires_in = Convert.ToString(office365Token.expires_in);
                    model.scope = office365Token.scope;
                    var dt = DataUtility.GetCurrentDateTime();
                    //model.ExpireOn = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second + Convert.ToInt32(model.ExpireIn));
                    model.refresh_token = office365Token.refresh_token;

                    // _userEmailService.CheckInsertOrUpdate(model);
                    isTokenReceived = true;
                }
            }

            return isTokenReceived;
        }

        public async Task SetOffice365Token()
        {
            client.DefaultRequestHeaders.Clear();

            var param = new Dictionary<string, string>();
            param.Add("client_id", OneClappContext.MicroSoftClientId);  //DataUtility.Office365ClientId
            param.Add("client_secret", OneClappContext.MicroSecretKey);  //DataUtility.Office365ClientSecret
            param.Add("refresh_token", intProviderAppSecretObj.Refresh_Token);
            param.Add("code", mailTokenDto.code);
            param.Add("scope", MicrosoftScope);
            param.Add("grant_type", "refresh_token");

            //var response = await client.PostAsync(DataUtility.Office365Token, new FormUrlEncodedContent(param));
            var response = await client.PostAsync(DataUtility.Office365Token, new FormUrlEncodedContent(param));

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var serializer = new DataContractJsonSerializer(typeof(Office365Token));
                var office365Token = (Office365Token)serializer.ReadObject(stream);

                if (office365Token != null)
                {
                    token = office365Token.access_token;
                }
            }
        }

        #endregion
    }
}